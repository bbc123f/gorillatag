using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000350 RID: 848
	[ExecuteInEditMode]
	public class DrawBox : DrawBase
	{
		// Token: 0x0600187F RID: 6271 RVA: 0x000838C0 File Offset: 0x00081AC0
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x000838EC File Offset: 0x00081AEC
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04001977 RID: 6519
		public float Radius = 1f;

		// Token: 0x04001978 RID: 6520
		public int NumSegments = 64;

		// Token: 0x04001979 RID: 6521
		public float StartAngle;

		// Token: 0x0400197A RID: 6522
		public float ArcAngle = 60f;
	}
}
