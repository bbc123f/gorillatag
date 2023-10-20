using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200034F RID: 847
	public abstract class DrawBase : MonoBehaviour
	{
		// Token: 0x0600187C RID: 6268 RVA: 0x00083844 File Offset: 0x00081A44
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

		// Token: 0x0600187D RID: 6269
		protected abstract void Draw(Color color, DebugUtil.Style style, bool depthTest);

		// Token: 0x04001972 RID: 6514
		public Color WireframeColor = Color.white;

		// Token: 0x04001973 RID: 6515
		public Color ShadededColor = Color.gray;

		// Token: 0x04001974 RID: 6516
		public bool Wireframe;

		// Token: 0x04001975 RID: 6517
		public DebugUtil.Style Style = DebugUtil.Style.FlatShaded;

		// Token: 0x04001976 RID: 6518
		public bool DepthTest = true;
	}
}
