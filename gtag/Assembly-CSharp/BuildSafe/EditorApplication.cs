using System;

namespace BuildSafe
{
	// Token: 0x020002BE RID: 702
	public static class EditorApplication
	{
		// Token: 0x1400001D RID: 29
		// (add) Token: 0x060012FE RID: 4862 RVA: 0x0006E61A File Offset: 0x0006C81A
		// (remove) Token: 0x060012FF RID: 4863 RVA: 0x0006E61C File Offset: 0x0006C81C
		public static event Action hierarchyChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06001300 RID: 4864 RVA: 0x0006E61E File Offset: 0x0006C81E
		// (remove) Token: 0x06001301 RID: 4865 RVA: 0x0006E620 File Offset: 0x0006C820
		public static event Action update
		{
			add
			{
			}
			remove
			{
			}
		}

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06001302 RID: 4866 RVA: 0x0006E622 File Offset: 0x0006C822
		// (remove) Token: 0x06001303 RID: 4867 RVA: 0x0006E624 File Offset: 0x0006C824
		public static event Action delayCall
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
