using Photon.Pun;
using UnityEngine;

public class GorillaFlag : GorillaTrigger
{
	public bool isRedFlag;

	public override void OnTriggered()
	{
		base.OnTriggered();
		PhotonView.Get(Object.FindObjectOfType<GorillaCTFManager>()).RPC("TagFlag", RpcTarget.MasterClient, isRedFlag);
	}
}
