using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002B6 RID: 694
	public class GorillaNetworkDisconnectTrigger : GorillaTriggerBox
	{
		// Token: 0x06001290 RID: 4752 RVA: 0x0006BE80 File Offset: 0x0006A080
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

		// Token: 0x0400158A RID: 5514
		public PhotonNetworkController photonNetworkController;

		// Token: 0x0400158B RID: 5515
		public GameObject offlineVRRig;

		// Token: 0x0400158C RID: 5516
		public GameObject makeSureThisIsEnabled;

		// Token: 0x0400158D RID: 5517
		public GameObject[] makeSureTheseAreEnabled;

		// Token: 0x0400158E RID: 5518
		public string componentTypeToRemove;

		// Token: 0x0400158F RID: 5519
		public GameObject componentTarget;
	}
}
