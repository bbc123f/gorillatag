using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class GorillaJoinTeamBox : GorillaTriggerBox
{
	// Token: 0x0600099F RID: 2463 RVA: 0x0003B06C File Offset: 0x0003926C
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (GameObject.FindGameObjectWithTag("GorillaGameManager").GetComponent<GorillaGameManager>() != null)
		{
			bool inRoom = PhotonNetwork.InRoom;
		}
	}

	// Token: 0x04000BD1 RID: 3025
	public bool joinRedTeam;
}
