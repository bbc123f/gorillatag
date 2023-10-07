using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200034E RID: 846
	[ExecuteInEditMode]
	public class DrawBox : DrawBase
	{
		// Token: 0x06001876 RID: 6262 RVA: 0x000833D8 File Offset: 0x000815D8
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x00083404 File Offset: 0x00081604
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x0400196A RID: 6506
		public float Radius = 1f;

		// Token: 0x0400196B RID: 6507
		public int NumSegments = 64;

		// Token: 0x0400196C RID: 6508
		public float StartAngle;

		// Token: 0x0400196D RID: 6509
		public float ArcAngle = 60f;
	}
}
