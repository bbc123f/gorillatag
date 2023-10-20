using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200036F RID: 879
	public class BoingReactorFieldCPUSampler : MonoBehaviour
	{
		// Token: 0x060019E5 RID: 6629 RVA: 0x00090E1E File Offset: 0x0008F01E
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x00090E26 File Offset: 0x0008F026
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x00090E30 File Offset: 0x0008F030
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

		// Token: 0x060019E8 RID: 6632 RVA: 0x00090EEF File Offset: 0x0008F0EF
		public void Restore()
		{
			base.transform.position = this.m_objPosition;
			base.transform.rotation = this.m_objRotation;
		}

		// Token: 0x04001A92 RID: 6802
		public BoingReactorField ReactorField;

		// Token: 0x04001A93 RID: 6803
		[Tooltip("Match this mode with how you update your object's transform.\n\nUpdate - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\nFixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates.")]
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04001A94 RID: 6804
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x04001A95 RID: 6805
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x04001A96 RID: 6806
		private Vector3 m_objPosition;

		// Token: 0x04001A97 RID: 6807
		private Quaternion m_objRotation;
	}
}
