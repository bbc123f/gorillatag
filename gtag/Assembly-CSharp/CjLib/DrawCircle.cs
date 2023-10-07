using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200034F RID: 847
	[ExecuteInEditMode]
	public class DrawCircle : DrawBase
	{
		// Token: 0x06001879 RID: 6265 RVA: 0x000834A8 File Offset: 0x000816A8
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x000834D2 File Offset: 0x000816D2
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawCircle(base.transform.position, base.transform.rotation * Vector3.back, this.Radius, this.NumSegments, color, depthTest, style);
		}

		// Token: 0x0400196E RID: 6510
		public float Radius = 1f;

		// Token: 0x0400196F RID: 6511
		public int NumSegments = 64;
	}
}
