using System;
using ExitGames.Client.Photon;
using Photon.Pun;

// Token: 0x020001DE RID: 478
public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	// Token: 0x06000C59 RID: 3161 RVA: 0x0004B0A8 File Offset: 0x000492A8
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", true);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}
}
