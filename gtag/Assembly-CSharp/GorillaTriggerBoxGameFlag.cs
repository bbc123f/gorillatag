using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class GorillaTriggerBoxGameFlag : GorillaTriggerBox
{
	// Token: 0x06000836 RID: 2102 RVA: 0x0003330C File Offset: 0x0003150C
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		PhotonView.Get(Object.FindObjectOfType<GorillaGameManager>()).RPC(this.functionName, RpcTarget.MasterClient, null);
	}

	// Token: 0x04000A1C RID: 2588
	public string functionName;
}
