using System;
using UnityEngine;

namespace GorillaNetworking
{
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this);
		}

		public void UpdateScreens()
		{
			GorillaLevelScreen[] array = this.joinScreens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateText("THIS IS THE PLAYABLE AREA FOR THE ROOM YOU'RE CURRENTLY IN. HAVE FUN! MONKE!", true);
			}
			array = this.leaveScreens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateText("WARNING! IF YOU CONTINUE, YOU WILL LEAVE THIS ROOM AND JOIN A NEW ROOM FOR THE AREA YOU ARE ENTERING! YOU WILL BE PLAYING WITH A NEW GROUP OF PLAYERS, AND LEAVE THE CURRENT PLAYERS BEHIND!", false);
			}
		}

		public GameObject[] makeSureThisIsDisabled;

		public GameObject[] makeSureThisIsEnabled;

		public string gameModeName;

		public PhotonNetworkController photonNetworkController;

		public string componentTypeToAdd;

		public GameObject componentTarget;

		public GorillaLevelScreen[] joinScreens;

		public GorillaLevelScreen[] leaveScreens;

		public GorillaFriendCollider myCollider;
	}
}
