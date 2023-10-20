using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class GorillaTriggerColliderHandIndicator : MonoBehaviour
{
	// Token: 0x06000B3B RID: 2875 RVA: 0x00045488 File Offset: 0x00043688
	private void LateUpdate()
	{
		this.currentVelocity = (this.lastPosition - base.transform.position) / Time.fixedDeltaTime;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x000454C1 File Offset: 0x000436C1
	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	// Token: 0x04000E9F RID: 3743
	public Vector3 currentVelocity;

	// Token: 0x04000EA0 RID: 3744
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04000EA1 RID: 3745
	public bool isLeftHand;

	// Token: 0x04000EA2 RID: 3746
	public GorillaThrowableController throwableController;
}
