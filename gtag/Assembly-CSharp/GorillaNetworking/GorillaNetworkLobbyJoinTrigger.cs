using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002B6 RID: 694
	public class GorillaNetworkLobbyJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x0400158C RID: 5516
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x0400158D RID: 5517
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x0400158E RID: 5518
		public string gameModeName;

		// Token: 0x0400158F RID: 5519
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04001590 RID: 5520
		public string componentTypeToRemove;

		// Token: 0x04001591 RID: 5521
		public GameObject componentRemoveTarget;

		// Token: 0x04001592 RID: 5522
		public string componentTypeToAdd;

		// Token: 0x04001593 RID: 5523
		public GameObject componentAddTarget;

		// Token: 0x04001594 RID: 5524
		public GameObject gorillaParent;

		// Token: 0x04001595 RID: 5525
		public GameObject joinFailedBlock;
	}
}
