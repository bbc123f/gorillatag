using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000326 RID: 806
	public class PlanarSound : MonoBehaviour
	{
		// Token: 0x06001676 RID: 5750 RVA: 0x0007D3EE File Offset: 0x0007B5EE
		protected void OnEnable()
		{
			if (Camera.main != null)
			{
				this.cameraXform = Camera.main.transform;
				this.hasCamera = true;
			}
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x0007D414 File Offset: 0x0007B614
		protected void LateUpdate()
		{
			if (!this.hasCamera)
			{
				return;
			}
			Transform transform = base.transform;
			Vector3 localPosition = transform.parent.InverseTransformPoint(this.cameraXform.position);
			localPosition.y = 0f;
			transform.localPosition = localPosition;
		}

		// Token: 0x0400188E RID: 6286
		private Transform cameraXform;

		// Token: 0x0400188F RID: 6287
		private bool hasCamera;
	}
}
