using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200034C RID: 844
	[ExecuteInEditMode]
	public class DrawArrow : DrawBase
	{
		// Token: 0x06001870 RID: 6256 RVA: 0x00083264 File Offset: 0x00081464
		private void OnValidate()
		{
			this.ConeRadius = Mathf.Max(0f, this.ConeRadius);
			this.ConeHeight = Mathf.Max(0f, this.ConeHeight);
			this.StemThickness = Mathf.Max(0f, this.StemThickness);
			this.NumSegments = Mathf.Max(4, this.NumSegments);
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x000832C8 File Offset: 0x000814C8
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawArrow(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), this.ConeRadius, this.ConeHeight, this.NumSegments, this.StemThickness, color, depthTest, style);
		}

		// Token: 0x04001960 RID: 6496
		public Vector3 LocalEndVector = Vector3.right;

		// Token: 0x04001961 RID: 6497
		public float ConeRadius = 0.05f;

		// Token: 0x04001962 RID: 6498
		public float ConeHeight = 0.1f;

		// Token: 0x04001963 RID: 6499
		public float StemThickness = 0.05f;

		// Token: 0x04001964 RID: 6500
		public int NumSegments = 8;
	}
}
