using System;

namespace Viveport
{
	// Token: 0x02000248 RID: 584
	public class Leaderboard
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000E4A RID: 3658 RVA: 0x00052542 File Offset: 0x00050742
		// (set) Token: 0x06000E4B RID: 3659 RVA: 0x0005254A File Offset: 0x0005074A
		public int Rank { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000E4C RID: 3660 RVA: 0x00052553 File Offset: 0x00050753
		// (set) Token: 0x06000E4D RID: 3661 RVA: 0x0005255B File Offset: 0x0005075B
		public int Score { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000E4E RID: 3662 RVA: 0x00052564 File Offset: 0x00050764
		// (set) Token: 0x06000E4F RID: 3663 RVA: 0x0005256C File Offset: 0x0005076C
		public string UserName { get; set; }
	}
}
