using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200036E RID: 878
	public class BoingReactorFieldGPUSampler : MonoBehaviour
	{
		// Token: 0x060019E1 RID: 6625 RVA: 0x00090A50 File Offset: 0x0008EC50
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x00090A58 File Offset: 0x0008EC58
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x00090A60 File Offset: 0x0008EC60
		public void Update()
		{
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			if (this.m_fieldResourceSetId != component.GpuResourceSetId)
			{
				if (this.m_matProps == null)
				{
					this.m_matProps = new MaterialPropertyBlock();
				}
				if (component.UpdateShaderConstants(this.m_matProps, this.PositionSampleMultiplier, this.RotationSampleMultiplier))
				{
					this.m_fieldResourceSetId = component.GpuResourceSetId;
					foreach (Renderer renderer in new Renderer[]
					{
						base.GetComponent<MeshRenderer>(),
						base.GetComponent<SkinnedMeshRenderer>()
					})
					{
						if (!(renderer == null))
						{
							renderer.SetPropertyBlock(this.m_matProps);
						}
					}
				}
			}
		}

		// Token: 0x04001A8B RID: 6795
		public BoingReactorField ReactorField;

		// Token: 0x04001A8C RID: 6796
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x04001A8D RID: 6797
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x04001A8E RID: 6798
		private MaterialPropertyBlock m_matProps;

		// Token: 0x04001A8F RID: 6799
		private int m_fieldResourceSetId = -1;
	}
}
