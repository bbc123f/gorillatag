using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200036D RID: 877
	public class BoingReactorFieldCPUSampler : MonoBehaviour
	{
		// Token: 0x060019DC RID: 6620 RVA: 0x00090936 File Offset: 0x0008EB36
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x0009093E File Offset: 0x0008EB3E
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x00090948 File Offset: 0x0008EB48
		public void SampleFromField()
		{
			this.m_objPosition = base.transform.position;
			this.m_objRotation = base.transform.rotation;
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				return;
			}
			Vector3 a;
			Vector4 v;
			if (!component.SampleCpuGrid(base.transform.position, out a, out v))
			{
				return;
			}
			base.transform.position = this.m_objPosition + a * this.PositionSampleMultiplier;
			base.transform.rotation = QuaternionUtil.Pow(QuaternionUtil.FromVector4(v, true), this.RotationSampleMultiplier) * this.m_objRotation;
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x00090A07 File Offset: 0x0008EC07
		public void Restore()
		{
			base.transform.position = this.m_objPosition;
			base.transform.rotation = this.m_objRotation;
		}

		// Token: 0x04001A85 RID: 6789
		public BoingReactorField ReactorField;

		// Token: 0x04001A86 RID: 6790
		[Tooltip("Match this mode with how you update your object's transform.\n\nUpdate - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\nFixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates.")]
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04001A87 RID: 6791
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x04001A88 RID: 6792
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x04001A89 RID: 6793
		private Vector3 m_objPosition;

		// Token: 0x04001A8A RID: 6794
		private Quaternion m_objRotation;
	}
}
