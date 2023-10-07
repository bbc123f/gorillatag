using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002B4 RID: 692
	public class GorillaNetworkDisconnectTrigger : GorillaTriggerBox
	{
		// Token: 0x06001289 RID: 4745 RVA: 0x0006B9B4 File Offset: 0x00069BB4
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (this.makeSureThisIsEnabled != null)
			{
				this.makeSureThisIsEnabled.SetActive(true);
			}
			GameObject[] array = this.makeSureTheseAreEnabled;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.componentTypeToRemove != "" && this.componentTarget.GetComponent(this.componentTypeToRemove) != null)
				{
					Object.Destroy(this.componentTarget.GetComponent(this.componentTypeToRemove));
				}
				PhotonNetwork.Disconnect();
				SkinnedMeshRenderer[] array2 = this.photonNetworkController.offlineVRRig;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].enabled = true;
				}
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x0400157D RID: 5501
		public PhotonNetworkController photonNetworkController;

		// Token: 0x0400157E RID: 5502
		public GameObject offlineVRRig;

		// Token: 0x0400157F RID: 5503
		public GameObject makeSureThisIsEnabled;

		// Token: 0x04001580 RID: 5504
		public GameObject[] makeSureTheseAreEnabled;

		// Token: 0x04001581 RID: 5505
		public string componentTypeToRemove;

		// Token: 0x04001582 RID: 5506
		public GameObject componentTarget;
	}
}
