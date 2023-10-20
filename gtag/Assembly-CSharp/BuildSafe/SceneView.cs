using System;

namespace BuildSafe
{
	// Token: 0x020002C5 RID: 709
	public static class SceneView
	{
		// Token: 0x14000021 RID: 33
		// (add) Token: 0x0600131E RID: 4894 RVA: 0x0006ECFF File Offset: 0x0006CEFF
		// (remove) Token: 0x0600131F RID: 4895 RVA: 0x0006ED01 File Offset: 0x0006CF01
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
		// (add) Token: 0x06001320 RID: 4896 RVA: 0x0006ED03 File Offset: 0x0006CF03
		// (remove) Token: 0x06001321 RID: 4897 RVA: 0x0006ED05 File Offset: 0x0006CF05
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
