using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200034D RID: 845
	[ExecuteInEditMode]
	public class DrawArc : DrawBase
	{
		// Token: 0x06001876 RID: 6262 RVA: 0x0008366E File Offset: 0x0008186E
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x000836A8 File Offset: 0x000818A8
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04001969 RID: 6505
		public float Radius = 1f;

		// Token: 0x0400196A RID: 6506
		public int NumSegments = 64;

		// Token: 0x0400196B RID: 6507
		public float StartAngle;

		// Token: 0x0400196C RID: 6508
		public float ArcAngle = 60f;
	}
}
