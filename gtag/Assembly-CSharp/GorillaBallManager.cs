using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class GorillaBallManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x060008DE RID: 2270 RVA: 0x00035F0B File Offset: 0x0003410B
	public override void Awake()
	{
		base.Awake();
		if (base.photonView.IsMine)
		{
			PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/GorillaThrowingRock", this.ballAnchor, Quaternion.identity, 0, null);
		}
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
	}

	// Token: 0x04000AEB RID: 2795
	public Vector3 ballAnchor;
}
