using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000370 RID: 880
	public class BoingReactorFieldGPUSampler : MonoBehaviour
	{
		// Token: 0x060019EA RID: 6634 RVA: 0x00090F38 File Offset: 0x0008F138
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x00090F40 File Offset: 0x0008F140
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x00090F48 File Offset: 0x0008F148
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

		// Token: 0x04001A98 RID: 6808
		public BoingReactorField ReactorField;

		// Token: 0x04001A99 RID: 6809
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x04001A9A RID: 6810
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x04001A9B RID: 6811
		private MaterialPropertyBlock m_matProps;

		// Token: 0x04001A9C RID: 6812
		private int m_fieldResourceSetId = -1;
	}
}
