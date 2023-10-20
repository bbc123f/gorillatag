using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000353 RID: 851
	[ExecuteInEditMode]
	public class DrawSphere : DrawBase
	{
		// Token: 0x06001888 RID: 6280 RVA: 0x00083A63 File Offset: 0x00081C63
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x00083A90 File Offset: 0x00081C90
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x0400197E RID: 6526
		public float Radius = 1f;

		// Token: 0x0400197F RID: 6527
		public int NumSegments = 64;

		// Token: 0x04001980 RID: 6528
		public float StartAngle;

		// Token: 0x04001981 RID: 6529
		public float ArcAngle = 60f;
	}
}
