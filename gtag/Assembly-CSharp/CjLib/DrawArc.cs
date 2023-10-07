using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200034B RID: 843
	[ExecuteInEditMode]
	public class DrawArc : DrawBase
	{
		// Token: 0x0600186D RID: 6253 RVA: 0x00083186 File Offset: 0x00081386
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x000831C0 File Offset: 0x000813C0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x0400195C RID: 6492
		public float Radius = 1f;

		// Token: 0x0400195D RID: 6493
		public int NumSegments = 64;

		// Token: 0x0400195E RID: 6494
		public float StartAngle;

		// Token: 0x0400195F RID: 6495
		public float ArcAngle = 60f;
	}
}
