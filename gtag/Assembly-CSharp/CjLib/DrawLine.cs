using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000350 RID: 848
	[ExecuteInEditMode]
	public class DrawLine : DrawBase
	{
		// Token: 0x0600187C RID: 6268 RVA: 0x00083523 File Offset: 0x00081723
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
		}

		// Token: 0x0600187D RID: 6269 RVA: 0x00083533 File Offset: 0x00081733
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawLine(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), color, depthTest);
		}

		// Token: 0x04001970 RID: 6512
		public Vector3 LocalEndVector = Vector3.right;
	}
}
