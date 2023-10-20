using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200034E RID: 846
	[ExecuteInEditMode]
	public class DrawArrow : DrawBase
	{
		// Token: 0x06001879 RID: 6265 RVA: 0x0008374C File Offset: 0x0008194C
		private void OnValidate()
		{
			this.ConeRadius = Mathf.Max(0f, this.ConeRadius);
			this.ConeHeight = Mathf.Max(0f, this.ConeHeight);
			this.StemThickness = Mathf.Max(0f, this.StemThickness);
			this.NumSegments = Mathf.Max(4, this.NumSegments);
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x000837B0 File Offset: 0x000819B0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawArrow(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), this.ConeRadius, this.ConeHeight, this.NumSegments, this.StemThickness, color, depthTest, style);
		}

		// Token: 0x0400196D RID: 6509
		public Vector3 LocalEndVector = Vector3.right;

		// Token: 0x0400196E RID: 6510
		public float ConeRadius = 0.05f;

		// Token: 0x0400196F RID: 6511
		public float ConeHeight = 0.1f;

		// Token: 0x04001970 RID: 6512
		public float StemThickness = 0.05f;

		// Token: 0x04001971 RID: 6513
		public int NumSegments = 8;
	}
}
