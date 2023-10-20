using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002B8 RID: 696
	public class GorillaNetworkLobbyJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x04001599 RID: 5529
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x0400159A RID: 5530
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x0400159B RID: 5531
		public string gameModeName;

		// Token: 0x0400159C RID: 5532
		public PhotonNetworkController photonNetworkController;

		// Token: 0x0400159D RID: 5533
		public string componentTypeToRemove;

		// Token: 0x0400159E RID: 5534
		public GameObject componentRemoveTarget;

		// Token: 0x0400159F RID: 5535
		public string componentTypeToAdd;

		// Token: 0x040015A0 RID: 5536
		public GameObject componentAddTarget;

		// Token: 0x040015A1 RID: 5537
		public GameObject gorillaParent;

		// Token: 0x040015A2 RID: 5538
		public GameObject joinFailedBlock;
	}
}
