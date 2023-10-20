using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002B7 RID: 695
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x06001292 RID: 4754 RVA: 0x0006BF49 File Offset: 0x0006A149
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this);
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x0006BF78 File Offset: 0x0006A178
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

		// Token: 0x04001590 RID: 5520
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04001591 RID: 5521
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04001592 RID: 5522
		public string gameModeName;

		// Token: 0x04001593 RID: 5523
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04001594 RID: 5524
		public string componentTypeToAdd;

		// Token: 0x04001595 RID: 5525
		public GameObject componentTarget;

		// Token: 0x04001596 RID: 5526
		public GorillaLevelScreen[] joinScreens;

		// Token: 0x04001597 RID: 5527
		public GorillaLevelScreen[] leaveScreens;

		// Token: 0x04001598 RID: 5528
		public GorillaFriendCollider myCollider;
	}
}
