using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001E2 RID: 482
internal class OwnershipGaurd : MonoBehaviour
{
	// Token: 0x06000C6C RID: 3180 RVA: 0x0004B6D1 File Offset: 0x000498D1
	private void Start()
	{
		if (this.autoRegisterAll)
		{
			this.photonViews = base.GetComponents<PhotonView>();
		}
		if (this.photonViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RegisterViews(this.photonViews);
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x0004B6FB File Offset: 0x000498FB
	private void OnDestroy()
	{
		if (this.photonViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RemoveViews(this.photonViews);
	}

	// Token: 0x04000FED RID: 4077
	[SerializeField]
	private PhotonView[] photonViews;

	// Token: 0x04000FEE RID: 4078
	[SerializeField]
	private bool autoRegisterAll = true;
}
