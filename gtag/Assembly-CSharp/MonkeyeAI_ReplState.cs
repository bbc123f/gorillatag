using System;
using Photon.Pun;
using UnityEngine;

public class MonkeyeAI_ReplState : MonoBehaviour, IPunObservable
{
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.userId);
			stream.SendNext(this.attackPos);
			stream.SendNext(this.timer);
			stream.SendNext(this.floorEnabled);
			stream.SendNext(this.portalEnabled);
			stream.SendNext(this.freezePlayer);
			stream.SendNext(this.alpha);
			stream.SendNext(this.state);
			return;
		}
		if (info.photonView.Owner == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != info.photonView.Owner.ActorNumber)
		{
			return;
		}
		this.userId = (string)stream.ReceiveNext();
		this.attackPos = (Vector3)stream.ReceiveNext();
		this.timer = (float)stream.ReceiveNext();
		this.floorEnabled = (bool)stream.ReceiveNext();
		this.portalEnabled = (bool)stream.ReceiveNext();
		this.freezePlayer = (bool)stream.ReceiveNext();
		this.alpha = (float)stream.ReceiveNext();
		this.state = (MonkeyeAI_ReplState.EStates)stream.ReceiveNext();
	}

	public MonkeyeAI_ReplState()
	{
	}

	public MonkeyeAI_ReplState.EStates state;

	public string userId;

	public Vector3 attackPos;

	public float timer;

	public bool floorEnabled;

	public bool portalEnabled;

	public bool freezePlayer;

	public float alpha;

	public enum EStates
	{
		Sleeping,
		Patrolling,
		Chasing,
		ReturnToSleepPt,
		GoToSleep,
		BeginAttack,
		OpenFloor,
		DropPlayer,
		CloseFloor
	}
}
