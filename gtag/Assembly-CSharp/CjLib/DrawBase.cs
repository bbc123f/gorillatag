using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200034D RID: 845
	public abstract class DrawBase : MonoBehaviour
	{
		// Token: 0x06001873 RID: 6259 RVA: 0x0008335C File Offset: 0x0008155C
		private void Update()
		{
			if (this.Style != DebugUtil.Style.Wireframe)
			{
				this.Draw(this.ShadededColor, this.Style, this.DepthTest);
			}
			if (this.Style == DebugUtil.Style.Wireframe || this.Wireframe)
			{
				this.Draw(this.WireframeColor, DebugUtil.Style.Wireframe, this.DepthTest);
			}
		}

		// Token: 0x06001874 RID: 6260
		protected abstract void Draw(Color color, DebugUtil.Style style, bool depthTest);

		// Token: 0x04001965 RID: 6501
		public Color WireframeColor = Color.white;

		// Token: 0x04001966 RID: 6502
		public Color ShadededColor = Color.gray;

		// Token: 0x04001967 RID: 6503
		public bool Wireframe;

		// Token: 0x04001968 RID: 6504
		public DebugUtil.Style Style = DebugUtil.Style.FlatShaded;

		// Token: 0x04001969 RID: 6505
		public bool DepthTest = true;
	}
}
