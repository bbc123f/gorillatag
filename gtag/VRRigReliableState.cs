using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

public class VRRigReliableState : MonoBehaviourPunCallbacks, IGorillaSerializeable
{
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

	private bool isOfflineVRRig;

	private BodyDockPositions bDock;

	[NonSerialized]
	public int sizeLayerMask = 1;

	public bool isDirty { get; private set; } = true;


	private void Awake()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Combine(VRRig.newPlayerJoined, new Action(SetIsDirty));
	}

	private void OnDestroy()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Remove(VRRig.newPlayerJoined, new Action(SetIsDirty));
	}

	public void SetIsDirty()
	{
		isDirty = true;
	}

	public void SetIsNotDirty()
	{
		isDirty = false;
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		SetIsDirty();
	}

	public void SharedStart(bool isOfflineVRRig_, BodyDockPositions bDock_)
	{
		isOfflineVRRig = isOfflineVRRig_;
		bDock = bDock_;
		activeTransferrableObjectIndex = new int[CosmeticsController.maximumTransferrableItems];
		for (int i = 0; i < activeTransferrableObjectIndex.Length; i++)
		{
			activeTransferrableObjectIndex[i] = -1;
		}
		transferrablePosStates = new TransferrableObject.PositionState[CosmeticsController.maximumTransferrableItems];
		transferrableItemStates = new TransferrableObject.ItemStates[CosmeticsController.maximumTransferrableItems];
		transferableDockPositions = new BodyDockPositions.DropPositions[CosmeticsController.maximumTransferrableItems];
	}

	void IGorillaSerializeable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		if (isDirty)
		{
			isDirty = false;
			for (int i = 0; i < activeTransferrableObjectIndex.Length; i++)
			{
				stream.SendNext(activeTransferrableObjectIndex[i]);
				stream.SendNext(transferrablePosStates[i]);
				stream.SendNext(transferrableItemStates[i]);
				stream.SendNext(transferableDockPositions[i]);
			}
			stream.SendNext(wearablesPackedStates);
			stream.SendNext(lThrowableProjectileIndex);
			stream.SendNext(rThrowableProjectileIndex);
			stream.SendNext(lThrowableProjectileColor.r);
			stream.SendNext(lThrowableProjectileColor.g);
			stream.SendNext(lThrowableProjectileColor.b);
			stream.SendNext(rThrowableProjectileColor.r);
			stream.SendNext(rThrowableProjectileColor.g);
			stream.SendNext(rThrowableProjectileColor.b);
			stream.SendNext(sizeLayerMask);
		}
	}

	void IGorillaSerializeable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < activeTransferrableObjectIndex.Length; i++)
		{
			activeTransferrableObjectIndex[i] = (int)stream.ReceiveNext();
			transferrablePosStates[i] = (TransferrableObject.PositionState)stream.ReceiveNext();
			transferrableItemStates[i] = (TransferrableObject.ItemStates)stream.ReceiveNext();
			transferableDockPositions[i] = (BodyDockPositions.DropPositions)stream.ReceiveNext();
		}
		wearablesPackedStates = (int)stream.ReceiveNext();
		lThrowableProjectileIndex = (int)stream.ReceiveNext();
		rThrowableProjectileIndex = (int)stream.ReceiveNext();
		lThrowableProjectileColor.r = (float)stream.ReceiveNext();
		lThrowableProjectileColor.g = (float)stream.ReceiveNext();
		lThrowableProjectileColor.b = (float)stream.ReceiveNext();
		rThrowableProjectileColor.r = (float)stream.ReceiveNext();
		rThrowableProjectileColor.g = (float)stream.ReceiveNext();
		rThrowableProjectileColor.b = (float)stream.ReceiveNext();
		sizeLayerMask = (int)stream.ReceiveNext();
		bDock.RefreshTransferrableItems();
	}
}
