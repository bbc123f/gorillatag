using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000331 RID: 817
	public class CompassNeedleRotator : MonoBehaviour
	{
		// Token: 0x060016B0 RID: 5808 RVA: 0x0007E35E File Offset: 0x0007C55E
		protected void OnEnable()
		{
			this.currentVelocity = 0f;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x0007E37C File Offset: 0x0007C57C
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float angle = Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref this.currentVelocity, 0.005f);
			transform.Rotate(transform.up, angle, Space.World);
		}

		// Token: 0x040018D8 RID: 6360
		private const float smoothTime = 0.005f;

		// Token: 0x040018D9 RID: 6361
		private float currentVelocity;
	}
}
