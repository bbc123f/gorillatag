using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaBallManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks
{
	public override void Awake()
	{
		base.Awake();
		if (base.photonView.IsMine)
		{
			PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/GorillaThrowingRock", this.ballAnchor, Quaternion.identity, 0, null);
		}
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
	}

	public Vector3 ballAnchor;
}
