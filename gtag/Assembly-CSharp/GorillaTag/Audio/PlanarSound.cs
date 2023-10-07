using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000324 RID: 804
	public class PlanarSound : MonoBehaviour
	{
		// Token: 0x0600166D RID: 5741 RVA: 0x0007CF06 File Offset: 0x0007B106
		protected void OnEnable()
		{
			if (Camera.main != null)
			{
				this.cameraXform = Camera.main.transform;
				this.hasCamera = true;
			}
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x0007CF2C File Offset: 0x0007B12C
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

		// Token: 0x04001881 RID: 6273
		private Transform cameraXform;

		// Token: 0x04001882 RID: 6274
		private bool hasCamera;
	}
}
