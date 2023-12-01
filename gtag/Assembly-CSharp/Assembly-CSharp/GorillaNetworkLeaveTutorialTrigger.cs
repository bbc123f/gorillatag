using System;
using ExitGames.Client.Photon;
using Photon.Pun;

public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", true);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}
}
