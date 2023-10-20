using System;
using ExitGames.Client.Photon;
using Photon.Pun;

// Token: 0x020001DF RID: 479
public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	// Token: 0x06000C5F RID: 3167 RVA: 0x0004B310 File Offset: 0x00049510
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", true);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}
}
