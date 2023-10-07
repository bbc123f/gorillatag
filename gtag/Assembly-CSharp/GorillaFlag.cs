using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class GorillaFlag : GorillaTrigger
{
	// Token: 0x06000880 RID: 2176 RVA: 0x00034BA1 File Offset: 0x00032DA1
	public override void OnTriggered()
	{
		base.OnTriggered();
		PhotonView.Get(Object.FindObjectOfType<GorillaCTFManager>()).RPC("TagFlag", RpcTarget.MasterClient, new object[]
		{
			this.isRedFlag
		});
	}

	// Token: 0x04000AAF RID: 2735
	public bool isRedFlag;
}
