using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000333 RID: 819
	public class CompassNeedleRotator : MonoBehaviour
	{
		// Token: 0x060016B9 RID: 5817 RVA: 0x0007E846 File Offset: 0x0007CA46
		protected void OnEnable()
		{
			this.currentVelocity = 0f;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x0007E864 File Offset: 0x0007CA64
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float angle = Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref this.currentVelocity, 0.005f);
			transform.Rotate(transform.up, angle, Space.World);
		}

		// Token: 0x040018E5 RID: 6373
		private const float smoothTime = 0.005f;

		// Token: 0x040018E6 RID: 6374
		private float currentVelocity;
	}
}
