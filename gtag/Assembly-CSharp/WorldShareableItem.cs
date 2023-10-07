using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class WorldShareableItem : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IOnPhotonViewOwnerChange, IPhotonViewCallback, IPunObservable, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000581 RID: 1409 RVA: 0x00022D5C File Offset: 0x00020F5C
	// (set) Token: 0x06000582 RID: 1410 RVA: 0x00022D64 File Offset: 0x00020F64
	[DevInspectorShow]
	public TransferrableObject.PositionState transferableObjectState { get; set; }

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000583 RID: 1411 RVA: 0x00022D6D File Offset: 0x00020F6D
	// (set) Token: 0x06000584 RID: 1412 RVA: 0x00022D75 File Offset: 0x00020F75
	public TransferrableObject.ItemStates transferableObjectItemState { get; set; }

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000585 RID: 1413 RVA: 0x00022D7E File Offset: 0x00020F7E
	[DevInspectorShow]
	public WorldTargetItem Target
	{
		get
		{
			return this.target;
		}
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00022D86 File Offset: 0x00020F86
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.teleportSerializer = base.GetComponent<TransformViewTeleportSerializer>();
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00022DAC File Offset: 0x00020FAC
	public override void OnEnable()
	{
		base.OnEnable();
		this.guard.AddCallbackTarget(this);
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00022DC0 File Offset: 0x00020FC0
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
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x00022E20 File Offset: 0x00021020
	public void OnDestroy()
	{
		if (!ObjectPools.instance.isQuitting)
		{
			Debug.LogError("World sharable item is being destroyed, This should never happen in game");
		}
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x00022E38 File Offset: 0x00021038
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x00022E40 File Offset: 0x00021040
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

	// Token: 0x0600058C RID: 1420 RVA: 0x00022ECC File Offset: 0x000210CC
	public void ResetViews()
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		photonView.ViewID = 0;
		photonView2.ViewID = 0;
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x00022EF4 File Offset: 0x000210F4
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
		this.validShareable = (component.canDrop || component.shareable || component.allowWorldSharableInstance);
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

	// Token: 0x0600058E RID: 1422 RVA: 0x00022FE5 File Offset: 0x000211E5
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.LogError("A world Sharable Item was insteanteated over photon. this shouldn't happen");
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00022FF1 File Offset: 0x000211F1
	void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			this.onOwnerChangeCb(newOwner, previousOwner);
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x06000590 RID: 1424 RVA: 0x00023008 File Offset: 0x00021208
	// (set) Token: 0x06000591 RID: 1425 RVA: 0x00023010 File Offset: 0x00021210
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

	// Token: 0x06000592 RID: 1426 RVA: 0x0002301C File Offset: 0x0002121C
	private void Update()
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

	// Token: 0x06000593 RID: 1427 RVA: 0x000230EA File Offset: 0x000212EA
	public void SyncToSceneObject(TransferrableObject transferrableObject)
	{
		Debug.Log("Syncing to scene object", this);
		this.target = WorldTargetItem.GenerateTargetFromWorldSharableItem(null, -2, transferrableObject.transform);
		base.transform.parent = null;
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00023117 File Offset: 0x00021317
	public void SetupSceneObjectOnNetwork(Player owner)
	{
		this.guard.SetOwnership(owner, false, false);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00023127 File Offset: 0x00021327
	public bool IsValid()
	{
		return this.target != null;
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x00023132 File Offset: 0x00021332
	public void Invalidate()
	{
		Debug.Log("Invalidating", this);
		this.target = null;
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x00023154 File Offset: 0x00021354
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

	// Token: 0x06000598 RID: 1432 RVA: 0x0002319C File Offset: 0x0002139C
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

	// Token: 0x06000599 RID: 1433 RVA: 0x00023257 File Offset: 0x00021457
	[PunRPC]
	private void RPCWorldShareable(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPCWorldShareable");
		this.rpcCallBack();
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0002326F File Offset: 0x0002146F
	public bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer)
	{
		return true;
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x00023272 File Offset: 0x00021472
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x00023274 File Offset: 0x00021474
	public bool OnOwnershipRequest(Player fromPlayer)
	{
		return true;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x00023277 File Offset: 0x00021477
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x00023279 File Offset: 0x00021479
	public void SetWillTeleport()
	{
		this.teleportSerializer.SetWillTeleport();
	}

	// Token: 0x04000681 RID: 1665
	private bool validShareable = true;

	// Token: 0x04000682 RID: 1666
	private PhotonView view;

	// Token: 0x04000683 RID: 1667
	public RequestableOwnershipGuard guard;

	// Token: 0x04000684 RID: 1668
	private TransformViewTeleportSerializer teleportSerializer;

	// Token: 0x04000685 RID: 1669
	[DevInspectorShow]
	[CanBeNull]
	private WorldTargetItem target;

	// Token: 0x04000686 RID: 1670
	public WorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	// Token: 0x04000687 RID: 1671
	public Action rpcCallBack;

	// Token: 0x04000688 RID: 1672
	private bool enableRemoteSync = true;

	// Token: 0x04000689 RID: 1673
	public Dictionary<Player, WorldShareableItem.CachedData> cachedDatas = new Dictionary<Player, WorldShareableItem.CachedData>();

	// Token: 0x020003ED RID: 1005
	// (Invoke) Token: 0x06001BCF RID: 7119
	public delegate void Delegate();

	// Token: 0x020003EE RID: 1006
	// (Invoke) Token: 0x06001BD3 RID: 7123
	public delegate void OnOwnerChangeDelegate(Player newOwner, Player prevOwner);

	// Token: 0x020003EF RID: 1007
	public struct CachedData
	{
		// Token: 0x04001C6F RID: 7279
		public TransferrableObject.PositionState cachedTransferableObjectState;

		// Token: 0x04001C70 RID: 7280
		public TransferrableObject.ItemStates cachedTransferableObjectItemState;
	}
}
