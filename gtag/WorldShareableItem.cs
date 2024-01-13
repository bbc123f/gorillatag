using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class WorldShareableItem : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IOnPhotonViewOwnerChange, IPhotonViewCallback, IPunObservable, IRequestableOwnershipGuardCallbacks
{
	public delegate void Delegate();

	public delegate void OnOwnerChangeDelegate(Player newOwner, Player prevOwner);

	public struct CachedData
	{
		public TransferrableObject.PositionState cachedTransferableObjectState;

		public TransferrableObject.ItemStates cachedTransferableObjectItemState;
	}

	private bool validShareable = true;

	private PhotonView view;

	public RequestableOwnershipGuard guard;

	[DevInspectorShow]
	[CanBeNull]
	private WorldTargetItem target;

	public OnOwnerChangeDelegate onOwnerChangeCb;

	public Action rpcCallBack;

	private bool enableRemoteSync = true;

	public Dictionary<Player, CachedData> cachedDatas = new Dictionary<Player, CachedData>();

	[DevInspectorShow]
	public TransferrableObject.PositionState transferableObjectState { get; set; }

	public TransferrableObject.ItemStates transferableObjectItemState { get; set; }

	[DevInspectorShow]
	public WorldTargetItem Target => target;

	[DevInspectorShow]
	public bool EnableRemoteSync
	{
		get
		{
			return enableRemoteSync;
		}
		set
		{
			enableRemoteSync = value;
		}
	}

	private void Awake()
	{
		view = GetComponent<PhotonView>();
		guard = GetComponent<RequestableOwnershipGuard>();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		guard.AddCallbackTarget(this);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		target = null;
		PhotonView[] components = GetComponents<PhotonView>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ViewID = 0;
		}
		transferableObjectState = TransferrableObject.PositionState.None;
		transferableObjectItemState = TransferrableObject.ItemStates.State0;
		guard.RemoveCallbackTarget(this);
		rpcCallBack = null;
		onOwnerChangeCb = null;
	}

	public void OnDestroy()
	{
		if (!ObjectPools.instance.isQuitting)
		{
			Debug.LogError("World sharable item is being destroyed, This should never happen in game");
		}
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
	}

	public void SetupSharableViewIDs(Player player, int slotID)
	{
		PhotonView[] components = GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		int num = player.ActorNumber * 1000 + 990 + slotID * 2;
		guard.giveCreatorAbsoluteAuthority = true;
		if (num != photonView.ViewID)
		{
			photonView.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2;
			photonView2.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2 + 1;
			guard.SetCreator(player);
		}
	}

	public void ResetViews()
	{
		PhotonView[] components = GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView obj = components[1];
		photonView.ViewID = 0;
		obj.ViewID = 0;
	}

	public void SetupSharableObject(int itemIDx, Player owner, Transform targetXform)
	{
		target = WorldTargetItem.GenerateTargetFromPlayerAndID(owner, itemIDx);
		if (target.targetObject != targetXform)
		{
			Debug.LogError($"The target object found a transform that does not match the target transform, this should never happen. owner: {owner} itemIDx: {itemIDx} targetXformPath: {targetXform.GetPath()}, target.targetObject: {target.targetObject.GetPath()}");
		}
		TransferrableObject component = target.targetObject.GetComponent<TransferrableObject>();
		validShareable = component.canDrop || component.shareable || component.allowWorldSharableInstance;
		if (!validShareable)
		{
			Debug.LogError($"tried to setup an invalid shareable {owner} {itemIDx} {targetXform.GetPath()}");
			base.gameObject.SetActive(value: false);
			Invalidate();
		}
		else
		{
			guard.AddCallbackTarget(component);
			guard.giveCreatorAbsoluteAuthority = true;
			component.SetWorldShareableItem(this);
		}
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.LogError("A world Sharable Item was insteanteated over photon. this shouldn't happen");
	}

	void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (onOwnerChangeCb != null)
		{
			onOwnerChangeCb(newOwner, previousOwner);
		}
	}

	private void Update()
	{
		if (!IsValid())
		{
			return;
		}
		if (guard.isTrulyMine)
		{
			if ((bool)target.transferrableObject)
			{
				_ = target.transferrableObject.worldShareableInstance != this;
			}
			base.transform.position = target.targetObject.position;
			base.transform.rotation = target.targetObject.rotation;
		}
		else if (!view.IsMine && EnableRemoteSync)
		{
			target.targetObject.position = base.transform.position;
			target.targetObject.rotation = base.transform.rotation;
		}
	}

	public void SyncToSceneObject(TransferrableObject transferrableObject)
	{
		Debug.Log("Syncing to scene object", this);
		target = WorldTargetItem.GenerateTargetFromWorldSharableItem(null, -2, transferrableObject.transform);
		base.transform.parent = null;
	}

	public void SetupSceneObjectOnNetwork(Player owner)
	{
		guard.SetOwnership(owner);
	}

	public bool IsValid()
	{
		return target != null;
	}

	public void Invalidate()
	{
		Debug.Log("Invalidating", this);
		target = null;
		transferableObjectState = TransferrableObject.PositionState.None;
		transferableObjectItemState = TransferrableObject.ItemStates.State0;
	}

	public void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		if (toPlayer != null && cachedDatas.TryGetValue(toPlayer, out var value))
		{
			transferableObjectState = value.cachedTransferableObjectState;
			transferableObjectItemState = value.cachedTransferableObjectItemState;
			cachedDatas.Remove(toPlayer);
		}
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(transferableObjectState);
			stream.SendNext(transferableObjectItemState);
		}
		else if (!object.Equals(info.Sender, guard.actualOwner))
		{
			Debug.Log("Blocking info from non owner");
			cachedDatas.AddOrUpdate(info.Sender, new CachedData
			{
				cachedTransferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext(),
				cachedTransferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext()
			});
		}
		else
		{
			transferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext();
			transferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext();
		}
	}

	[PunRPC]
	private void RPCWorldShareable(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPCWorldShareable");
		rpcCallBack();
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
}
