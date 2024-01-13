using UnityEngine;

namespace GorillaNetworking;

public class GorillaNetworkJoinTrigger : GorillaTriggerBox
{
	public GameObject[] makeSureThisIsDisabled;

	public GameObject[] makeSureThisIsEnabled;

	public string gameModeName;

	public PhotonNetworkController photonNetworkController;

	public string componentTypeToAdd;

	public GameObject componentTarget;

	public GorillaLevelScreen[] joinScreens;

	public GorillaLevelScreen[] leaveScreens;

	public GorillaFriendCollider myCollider;

	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		GorillaComputer.instance.allowedMapsToJoin = myCollider.myAllowedMapsToJoin;
		PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this);
	}

	public void UpdateScreens()
	{
		GorillaLevelScreen[] array = joinScreens;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateText("THIS IS THE PLAYABLE AREA FOR THE ROOM YOU'RE CURRENTLY IN. HAVE FUN! MONKE!", setToGoodMaterial: true);
		}
		array = leaveScreens;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateText("WARNING! IF YOU CONTINUE, YOU WILL LEAVE THIS ROOM AND JOIN A NEW ROOM FOR THE AREA YOU ARE ENTERING! YOU WILL BE PLAYING WITH A NEW GROUP OF PLAYERS, AND LEAVE THE CURRENT PLAYERS BEHIND!", setToGoodMaterial: false);
		}
	}
}
