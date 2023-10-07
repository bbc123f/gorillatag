using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class GorillaTriggerColliderHandIndicator : MonoBehaviour
{
	// Token: 0x06000B35 RID: 2869 RVA: 0x00045220 File Offset: 0x00043420
	private void LateUpdate()
	{
		this.currentVelocity = (this.lastPosition - base.transform.position) / Time.fixedDeltaTime;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00045259 File Offset: 0x00043459
	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	// Token: 0x04000E9B RID: 3739
	public Vector3 currentVelocity;

	// Token: 0x04000E9C RID: 3740
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04000E9D RID: 3741
	public bool isLeftHand;

	// Token: 0x04000E9E RID: 3742
	public GorillaThrowableController throwableController;
}
