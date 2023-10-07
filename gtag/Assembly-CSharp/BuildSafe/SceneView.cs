using System;

namespace BuildSafe
{
	// Token: 0x020002C3 RID: 707
	public static class SceneView
	{
		// Token: 0x14000021 RID: 33
		// (add) Token: 0x06001317 RID: 4887 RVA: 0x0006E833 File Offset: 0x0006CA33
		// (remove) Token: 0x06001318 RID: 4888 RVA: 0x0006E835 File Offset: 0x0006CA35
		public static event Action duringSceneGui
		{
			add
			{
			}
			remove
			{
			}
		}

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x06001319 RID: 4889 RVA: 0x0006E837 File Offset: 0x0006CA37
		// (remove) Token: 0x0600131A RID: 4890 RVA: 0x0006E839 File Offset: 0x0006CA39
		public static event Action duringSceneGuiTick
		{
			add
			{
			}
			remove
			{
			}
		}
	}
}
