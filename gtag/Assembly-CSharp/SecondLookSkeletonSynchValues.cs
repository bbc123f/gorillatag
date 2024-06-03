using System;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SecondLookSkeletonSynchValues : MonoBehaviourPunCallbacks, IInRoomCallbacks, IPunObservable, IOnPhotonViewOwnerChange, IPhotonViewCallback
{
	public void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.mySkeleton.SetNodes();
			if (this.mySkeleton.currentState != this.currentState)
			{
				this.mySkeleton.ChangeState(this.currentState);
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.photonView.IsMine && info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.mySkeleton.currentState);
			stream.SendNext(this.mySkeleton.spookyGhost.transform.position);
			stream.SendNext(this.mySkeleton.spookyGhost.transform.rotation);
			stream.SendNext(this.currentNode);
			stream.SendNext(this.nextNode);
			stream.SendNext(this.angerPoint);
			return;
		}
		this.currentState = (SecondLookSkeleton.GhostState)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.position.SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		ref this.rotation.SetValueSafe(quaternion);
		this.currentNode = (int)stream.ReceiveNext();
		this.nextNode = (int)stream.ReceiveNext();
		this.angerPoint = (int)stream.ReceiveNext();
		if (this.mySkeleton.tapped && this.currentState != this.mySkeleton.currentState)
		{
			this.mySkeleton.ChangeState(this.currentState);
		}
	}

	[PunRPC]
	public void RemoteActivateGhost(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteActivateGhost");
		if (!base.photonView.IsMine)
		{
			return;
		}
		this.mySkeleton.RemoteActivateGhost();
	}

	[PunRPC]
	public void RemotePlayerSeen(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemotePlayerSeen");
		NetPlayer netPlayer = (PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber);
		if (!this.mySkeleton.playersSeen.Contains(netPlayer))
		{
			this.mySkeleton.RemotePlayerSeen(netPlayer);
		}
	}

	[PunRPC]
	public void RemotePlayerCaught(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemotePlayerCaught");
		if (!base.photonView.IsMine)
		{
			return;
		}
		NetPlayer player = (PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber);
		if (this.mySkeleton.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.mySkeleton.RemotePlayerCaught(player);
		}
	}

	public SecondLookSkeletonSynchValues()
	{
	}

	public SecondLookSkeleton.GhostState currentState;

	public Vector3 position;

	public Quaternion rotation;

	public SecondLookSkeleton mySkeleton;

	public int currentNode;

	public int nextNode;

	public int angerPoint;
}
