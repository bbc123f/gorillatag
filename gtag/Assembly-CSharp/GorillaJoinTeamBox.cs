using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class GorillaJoinTeamBox : GorillaTriggerBox
{
	// Token: 0x060009A3 RID: 2467 RVA: 0x0003B024 File Offset: 0x00039224
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (GameObject.FindGameObjectWithTag("GorillaGameManager").GetComponent<GorillaGameManager>() != null)
		{
			bool inRoom = PhotonNetwork.InRoom;
		}
	}

	// Token: 0x04000BD5 RID: 3029
	public bool joinRedTeam;
}
