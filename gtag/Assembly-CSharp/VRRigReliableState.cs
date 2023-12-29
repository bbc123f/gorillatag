using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

public class VRRigReliableState : MonoBehaviourPunCallbacks, IGorillaSerializeable
{
	public bool isDirty { get; private set; } = true;

	private void Awake()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Combine(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	private void OnDestroy()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Remove(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	public void SetIsDirty()
	{
		this.isDirty = true;
	}

	public void SetIsNotDirty()
	{
		this.isDirty = false;
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.SetIsDirty();
	}

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
		stream.SendNext(this.randomThrowableIndex);
	}

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
		this.randomThrowableIndex = (int)stream.ReceiveNext();
		this.bDock.RefreshTransferrableItems();
	}

	[NonSerialized]
	public int[] activeTransferrableObjectIndex;

	[NonSerialized]
	public TransferrableObject.PositionState[] transferrablePosStates;

	[NonSerialized]
	public TransferrableObject.ItemStates[] transferrableItemStates;

	[NonSerialized]
	public BodyDockPositions.DropPositions[] transferableDockPositions;

	[NonSerialized]
	public int wearablesPackedStates;

	[NonSerialized]
	public int lThrowableProjectileIndex = -1;

	[NonSerialized]
	public int rThrowableProjectileIndex = -1;

	[NonSerialized]
	public Color lThrowableProjectileColor = Color.white;

	[NonSerialized]
	public Color rThrowableProjectileColor = Color.white;

	[NonSerialized]
	public int randomThrowableIndex;

	private bool isOfflineVRRig;

	private BodyDockPositions bDock;

	[NonSerialized]
	public int sizeLayerMask = 1;
}
