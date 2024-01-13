using Photon.Pun;
using UnityEngine;

public class GorillaTriggerBoxGameFlag : GorillaTriggerBox
{
	public string functionName;

	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		PhotonView.Get(Object.FindObjectOfType<GorillaGameManager>()).RPC(functionName, RpcTarget.MasterClient, null);
	}
}
