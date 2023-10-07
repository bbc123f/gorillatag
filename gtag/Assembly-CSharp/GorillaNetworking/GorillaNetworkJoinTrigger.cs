using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002B5 RID: 693
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x0600128B RID: 4747 RVA: 0x0006BA7D File Offset: 0x00069C7D
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this);
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x0006BAAC File Offset: 0x00069CAC
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

		// Token: 0x04001583 RID: 5507
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04001584 RID: 5508
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04001585 RID: 5509
		public string gameModeName;

		// Token: 0x04001586 RID: 5510
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04001587 RID: 5511
		public string componentTypeToAdd;

		// Token: 0x04001588 RID: 5512
		public GameObject componentTarget;

		// Token: 0x04001589 RID: 5513
		public GorillaLevelScreen[] joinScreens;

		// Token: 0x0400158A RID: 5514
		public GorillaLevelScreen[] leaveScreens;

		// Token: 0x0400158B RID: 5515
		public GorillaFriendCollider myCollider;
	}
}
