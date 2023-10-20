using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class GorillaBallManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x060008E2 RID: 2274 RVA: 0x00035E0C File Offset: 0x0003400C
	public override void Awake()
	{
		base.Awake();
		if (base.photonView.IsMine)
		{
			PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/GorillaThrowingRock", this.ballAnchor, Quaternion.identity, 0, null);
		}
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
	}

	// Token: 0x04000AEF RID: 2799
	public Vector3 ballAnchor;
}
