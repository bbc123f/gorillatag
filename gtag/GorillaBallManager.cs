using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaBallManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks
{
	public Vector3 ballAnchor;

	public override void Awake()
	{
		base.Awake();
		if (base.photonView.IsMine)
		{
			PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/GorillaThrowingRock", ballAnchor, Quaternion.identity, 0);
		}
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
	}
}
