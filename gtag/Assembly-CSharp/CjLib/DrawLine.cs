using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000352 RID: 850
	[ExecuteInEditMode]
	public class DrawLine : DrawBase
	{
		// Token: 0x06001885 RID: 6277 RVA: 0x00083A0B File Offset: 0x00081C0B
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x00083A1B File Offset: 0x00081C1B
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawLine(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), color, depthTest);
		}

		// Token: 0x0400197D RID: 6525
		public Vector3 LocalEndVector = Vector3.right;
	}
}
