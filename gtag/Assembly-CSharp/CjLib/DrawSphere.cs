using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000351 RID: 849
	[ExecuteInEditMode]
	public class DrawSphere : DrawBase
	{
		// Token: 0x0600187F RID: 6271 RVA: 0x0008357B File Offset: 0x0008177B
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x000835A8 File Offset: 0x000817A8
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04001971 RID: 6513
		public float Radius = 1f;

		// Token: 0x04001972 RID: 6514
		public int NumSegments = 64;

		// Token: 0x04001973 RID: 6515
		public float StartAngle;

		// Token: 0x04001974 RID: 6516
		public float ArcAngle = 60f;
	}
}
