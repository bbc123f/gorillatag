using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class WorldShareableItem : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IOnPhotonViewOwnerChange, IPhotonViewCallback, IPunObservable, IRequestableOwnershipGuardCallbacks
{
	[DevInspectorShow]
	public TransferrableObject.PositionState transferableObjectState { get; set; }

	public TransferrableObject.ItemStates transferableObjectItemState { get; set; }

	[DevInspectorShow]
	public WorldTargetItem Target
	{
		get
		{
			return this.target;
		}
	}

	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.teleportSerializer = base.GetComponent<TransformViewTeleportSerializer>();
	}

	public override void OnEnable()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		base.OnEnable();
		this.guard.AddCallbackTarget(this);
		WorldShareableItemManager.Register(this);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		this.target = null;
		PhotonView[] components = base.GetComponents<PhotonView>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ViewID = 0;
		}
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
		this.guard.RemoveCallbackTarget(this);
		this.rpcCallBack = null;
		this.onOwnerChangeCb = null;
		WorldShareableItemManager.Unregister(this);
	}

	public void OnDestroy()
	{
		if (!GTAppState.isQuitting)
		{
			Debug.LogError("World sharable item is being destroyed, This should never happen in game");
		}
		WorldShareableItemManager.Unregister(this);
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
	}

	public void SetupSharableViewIDs(Player player, int slotID)
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		int num = player.ActorNumber * 1000 + 990 + slotID * 2;
		this.guard.giveCreatorAbsoluteAuthority = true;
		if (num != photonView.ViewID)
		{
			photonView.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2;
			photonView2.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2 + 1;
			this.guard.SetCreator(player);
		}
	}

	public void ResetViews()
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		photonView.ViewID = 0;
		photonView2.ViewID = 0;
	}

	public void SetupSharableObject(int itemIDx, Player owner, Transform targetXform)
	{
		this.target = WorldTargetItem.GenerateTargetFromPlayerAndID(owner, itemIDx);
		if (this.target.targetObject != targetXform)
		{
			Debug.LogError(string.Format("The target object found a transform that does not match the target transform, this should never happen. owner: {0} itemIDx: {1} targetXformPath: {2}, target.targetObject: {3}", new object[]
			{
				owner,
				itemIDx,
				targetXform.GetPath(),
				this.target.targetObject.GetPath()
			}));
		}
		TransferrableObject component = this.target.targetObject.GetComponent<TransferrableObject>();
		this.validShareable = component.canDrop || component.shareable || component.allowWorldSharableInstance;
		if (!this.validShareable)
		{
			Debug.LogError(string.Format("tried to setup an invalid shareable {0} {1} {2}", owner, itemIDx, targetXform.GetPath()));
			base.gameObject.SetActive(false);
			this.Invalidate();
			return;
		}
		this.guard.AddCallbackTarget(component);
		this.guard.giveCreatorAbsoluteAuthority = true;
		component.SetWorldShareableItem(this);
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.LogError("A world Sharable Item was insteanteated over photon. this shouldn't happen");
	}

	void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			this.onOwnerChangeCb(newOwner, previousOwner);
		}
	}

	[DevInspectorShow]
	public bool EnableRemoteSync
	{
		get
		{
			return this.enableRemoteSync;
		}
		set
		{
			this.enableRemoteSync = value;
		}
	}

	public void TriggeredUpdate()
	{
		if (!this.IsValid())
		{
			return;
		}
		if (this.guard.isTrulyMine)
		{
			if (this.target.transferrableObject)
			{
				this.target.transferrableObject.worldShareableInstance != this;
			}
			base.transform.position = this.target.targetObject.position;
			base.transform.rotation = this.target.targetObject.rotation;
			return;
		}
		if (!this.view.IsMine && this.EnableRemoteSync)
		{
			this.target.targetObject.position = base.transform.position;
			this.target.targetObject.rotation = base.transform.rotation;
		}
	}

	public void SyncToSceneObject(TransferrableObject transferrableObject)
	{
		Debug.Log("Syncing to scene object", this);
		this.target = WorldTargetItem.GenerateTargetFromWorldSharableItem(null, -2, transferrableObject.transform);
		base.transform.parent = null;
	}

	public void SetupSceneObjectOnNetwork(Player owner)
	{
		this.guard.SetOwnership(owner, false, false);
	}

	public bool IsValid()
	{
		return this.target != null;
	}

	public void Invalidate()
	{
		Debug.Log("Invalidating", this);
		this.target = null;
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
	}

	public void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		if (toPlayer == null)
		{
			return;
		}
		WorldShareableItem.CachedData cachedData;
		if (this.cachedDatas.TryGetValue(toPlayer, out cachedData))
		{
			this.transferableObjectState = cachedData.cachedTransferableObjectState;
			this.transferableObjectItemState = cachedData.cachedTransferableObjectItemState;
			this.cachedDatas.Remove(toPlayer);
		}
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.transferableObjectState);
			stream.SendNext(this.transferableObjectItemState);
			return;
		}
		if (!object.Equals(info.Sender, this.guard.actualOwner))
		{
			Debug.Log("Blocking info from non owner");
			this.cachedDatas.AddOrUpdate(info.Sender, new WorldShareableItem.CachedData
			{
				cachedTransferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext(),
				cachedTransferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext()
			});
			return;
		}
		this.transferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext();
		this.transferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext();
	}

	[PunRPC]
	private void RPCWorldShareable(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPCWorldShareable");
		this.rpcCallBack();
	}

	public bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer)
	{
		return true;
	}

	public void OnMyCreatorLeft()
	{
	}

	public bool OnOwnershipRequest(Player fromPlayer)
	{
		return true;
	}

	public void OnMyOwnerLeft()
	{
	}

	public void SetWillTeleport()
	{
		this.teleportSerializer.SetWillTeleport();
	}

	private bool validShareable = true;

	private PhotonView view;

	public RequestableOwnershipGuard guard;

	private TransformViewTeleportSerializer teleportSerializer;

	[DevInspectorShow]
	[CanBeNull]
	private WorldTargetItem target;

	public WorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	public Action rpcCallBack;

	private bool enableRemoteSync = true;

	public Dictionary<Player, WorldShareableItem.CachedData> cachedDatas = new Dictionary<Player, WorldShareableItem.CachedData>();

	public delegate void Delegate();

	public delegate void OnOwnerChangeDelegate(Player newOwner, Player prevOwner);

	public struct CachedData
	{
		public TransferrableObject.PositionState cachedTransferableObjectState;

		public TransferrableObject.ItemStates cachedTransferableObjectItemState;
	}
}
