using Photon.Pun;
using UnityEngine;

public class MonkeyeAI_ReplState : MonoBehaviour, IPunObservable
{
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

	public EStates state;

	public string userId;

	public Vector3 attackPos;

	public float timer;

	public bool floorEnabled;

	public bool portalEnabled;

	public bool freezePlayer;

	public float alpha;

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(userId);
			stream.SendNext(attackPos);
			stream.SendNext(timer);
			stream.SendNext(floorEnabled);
			stream.SendNext(portalEnabled);
			stream.SendNext(freezePlayer);
			stream.SendNext(alpha);
			stream.SendNext(state);
		}
		else if (info.photonView.Owner != null && info.Sender.ActorNumber == info.photonView.Owner.ActorNumber)
		{
			userId = (string)stream.ReceiveNext();
			attackPos = (Vector3)stream.ReceiveNext();
			timer = (float)stream.ReceiveNext();
			floorEnabled = (bool)stream.ReceiveNext();
			portalEnabled = (bool)stream.ReceiveNext();
			freezePlayer = (bool)stream.ReceiveNext();
			alpha = (float)stream.ReceiveNext();
			state = (EStates)stream.ReceiveNext();
		}
	}
}
