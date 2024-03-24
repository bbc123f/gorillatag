using System;
using Photon.Pun;
using UnityEngine;

public class GorillaTriggerBoxGameFlag : GorillaTriggerBox
{
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		PhotonView.Get(Object.FindObjectOfType<GorillaGameManager>()).RPC(this.functionName, RpcTarget.MasterClient, null);
	}

	public GorillaTriggerBoxGameFlag()
	{
	}

	public string functionName;
}
