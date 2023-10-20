using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class VRRigReliableState : MonoBehaviourPunCallbacks, IGorillaSerializeable
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000620 RID: 1568 RVA: 0x00026B68 File Offset: 0x00024D68
	// (set) Token: 0x06000621 RID: 1569 RVA: 0x00026B70 File Offset: 0x00024D70
	public bool isDirty { get; private set; } = true;

	// Token: 0x06000622 RID: 1570 RVA: 0x00026B79 File Offset: 0x00024D79
	private void Awake()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Combine(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x00026B9B File Offset: 0x00024D9B
	private void OnDestroy()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Remove(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00026BBD File Offset: 0x00024DBD
	public void SetIsDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00026BC6 File Offset: 0x00024DC6
	public void SetIsNotDirty()
	{
		this.isDirty = false;
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00026BCF File Offset: 0x00024DCF
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.SetIsDirty();
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00026BE0 File Offset: 0x00024DE0
	public void SharedStart(bool isOfflineVRRig_, BodyDockPositions bDock_)
	{
		this.isOfflineVRRig = isOfflineVRRig_;
		this.bDock = bDock_;
		this.activeTransferrableObjectIndex = new int[CosmeticsController.maximumTransferrableItems];
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			this.activeTransferrableObjectIndex[i] = -1;
		}
		this.transferrablePosStates = new TransferrableObject.PositionState[CosmeticsController.maximumTransferrableItems];
		this.transferrableItemStates = new TransferrableObject.ItemStates[CosmeticsController.maximumTransferrableItems];
		this.transferableDockPositions = new BodyDockPositions.DropPositions[CosmeticsController.maximumTransferrableItems];
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00026C58 File Offset: 0x00024E58
	void IGorillaSerializeable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			stream.SendNext(this.activeTransferrableObjectIndex[i]);
			stream.SendNext(this.transferrablePosStates[i]);
			stream.SendNext(this.transferrableItemStates[i]);
			stream.SendNext(this.transferableDockPositions[i]);
		}
		stream.SendNext(this.wearablesPackedStates);
		stream.SendNext(this.lThrowableProjectileIndex);
		stream.SendNext(this.rThrowableProjectileIndex);
		stream.SendNext(this.lThrowableProjectileColor.r);
		stream.SendNext(this.lThrowableProjectileColor.g);
		stream.SendNext(this.lThrowableProjectileColor.b);
		stream.SendNext(this.rThrowableProjectileColor.r);
		stream.SendNext(this.rThrowableProjectileColor.g);
		stream.SendNext(this.rThrowableProjectileColor.b);
		stream.SendNext(this.sizeLayerMask);
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00026D9C File Offset: 0x00024F9C
	void IGorillaSerializeable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			this.activeTransferrableObjectIndex[i] = (int)stream.ReceiveNext();
			this.transferrablePosStates[i] = (TransferrableObject.PositionState)stream.ReceiveNext();
			this.transferrableItemStates[i] = (TransferrableObject.ItemStates)stream.ReceiveNext();
			this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)stream.ReceiveNext();
		}
		this.wearablesPackedStates = (int)stream.ReceiveNext();
		this.lThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.rThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.lThrowableProjectileColor.r = (float)stream.ReceiveNext();
		this.lThrowableProjectileColor.g = (float)stream.ReceiveNext();
		this.lThrowableProjectileColor.b = (float)stream.ReceiveNext();
		this.rThrowableProjectileColor.r = (float)stream.ReceiveNext();
		this.rThrowableProjectileColor.g = (float)stream.ReceiveNext();
		this.rThrowableProjectileColor.b = (float)stream.ReceiveNext();
		this.sizeLayerMask = (int)stream.ReceiveNext();
		this.bDock.RefreshTransferrableItems();
	}

	// Token: 0x04000775 RID: 1909
	[NonSerialized]
	public int[] activeTransferrableObjectIndex;

	// Token: 0x04000776 RID: 1910
	[NonSerialized]
	public TransferrableObject.PositionState[] transferrablePosStates;

	// Token: 0x04000777 RID: 1911
	[NonSerialized]
	public TransferrableObject.ItemStates[] transferrableItemStates;

	// Token: 0x04000778 RID: 1912
	[NonSerialized]
	public BodyDockPositions.DropPositions[] transferableDockPositions;

	// Token: 0x04000779 RID: 1913
	[NonSerialized]
	public int wearablesPackedStates;

	// Token: 0x0400077A RID: 1914
	[NonSerialized]
	public int lThrowableProjectileIndex = -1;

	// Token: 0x0400077B RID: 1915
	[NonSerialized]
	public int rThrowableProjectileIndex = -1;

	// Token: 0x0400077C RID: 1916
	[NonSerialized]
	public Color lThrowableProjectileColor = Color.white;

	// Token: 0x0400077D RID: 1917
	[NonSerialized]
	public Color rThrowableProjectileColor = Color.white;

	// Token: 0x0400077E RID: 1918
	private bool isOfflineVRRig;

	// Token: 0x0400077F RID: 1919
	private BodyDockPositions bDock;

	// Token: 0x04000780 RID: 1920
	[NonSerialized]
	public int sizeLayerMask = 1;
}
