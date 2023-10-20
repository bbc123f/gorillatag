using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001E3 RID: 483
internal class OwnershipGaurd : MonoBehaviour
{
	// Token: 0x06000C72 RID: 3186 RVA: 0x0004B939 File Offset: 0x00049B39
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

	// Token: 0x06000C73 RID: 3187 RVA: 0x0004B963 File Offset: 0x00049B63
	private void OnDestroy()
	{
		if (this.photonViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RemoveViews(this.photonViews);
	}

	// Token: 0x04000FF1 RID: 4081
	[SerializeField]
	private PhotonView[] photonViews;

	// Token: 0x04000FF2 RID: 4082
	[SerializeField]
	private bool autoRegisterAll = true;
}
