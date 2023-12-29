using System;
using Photon.Pun;
using UnityEngine;

public class GorillaFlag : GorillaTrigger
{
	public override void OnTriggered()
	{
		base.OnTriggered();
		PhotonView.Get(Object.FindObjectOfType<GorillaCTFManager>()).RPC("TagFlag", RpcTarget.MasterClient, new object[]
		{
			this.isRedFlag
		});
	}

	public bool isRedFlag;
}
