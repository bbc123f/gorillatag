using System;

namespace Viveport
{
	// Token: 0x02000247 RID: 583
	public class Leaderboard
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000E43 RID: 3651 RVA: 0x00052166 File Offset: 0x00050366
		// (set) Token: 0x06000E44 RID: 3652 RVA: 0x0005216E File Offset: 0x0005036E
		public int Rank { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000E45 RID: 3653 RVA: 0x00052177 File Offset: 0x00050377
		// (set) Token: 0x06000E46 RID: 3654 RVA: 0x0005217F File Offset: 0x0005037F
		public int Score { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x00052188 File Offset: 0x00050388
		// (set) Token: 0x06000E48 RID: 3656 RVA: 0x00052190 File Offset: 0x00050390
		public string UserName { get; set; }
	}
}
