using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000351 RID: 849
	[ExecuteInEditMode]
	public class DrawCircle : DrawBase
	{
		// Token: 0x06001882 RID: 6274 RVA: 0x00083990 File Offset: 0x00081B90
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x000839BA File Offset: 0x00081BBA
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawCircle(base.transform.position, base.transform.rotation * Vector3.back, this.Radius, this.NumSegments, color, depthTest, style);
		}

		// Token: 0x0400197B RID: 6523
		public float Radius = 1f;

		// Token: 0x0400197C RID: 6524
		public int NumSegments = 64;
	}
}
