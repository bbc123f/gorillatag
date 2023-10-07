using System;
using System.Collections.Generic;

namespace Viveport
{
	// Token: 0x02000248 RID: 584
	public class SubscriptionStatus
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000E4A RID: 3658 RVA: 0x000521A1 File Offset: 0x000503A1
		// (set) Token: 0x06000E4B RID: 3659 RVA: 0x000521A9 File Offset: 0x000503A9
		public List<SubscriptionStatus.Platform> Platforms { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000E4C RID: 3660 RVA: 0x000521B2 File Offset: 0x000503B2
		// (set) Token: 0x06000E4D RID: 3661 RVA: 0x000521BA File Offset: 0x000503BA
		public SubscriptionStatus.TransactionType Type { get; set; }

		// Token: 0x06000E4E RID: 3662 RVA: 0x000521C3 File Offset: 0x000503C3
		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		// Token: 0x02000489 RID: 1161
		public enum Platform
		{
			// Token: 0x04001EE2 RID: 7906
			Windows,
			// Token: 0x04001EE3 RID: 7907
			Android
		}

		// Token: 0x0200048A RID: 1162
		public enum TransactionType
		{
			// Token: 0x04001EE5 RID: 7909
			Unknown,
			// Token: 0x04001EE6 RID: 7910
			Paid,
			// Token: 0x04001EE7 RID: 7911
			Redeem,
			// Token: 0x04001EE8 RID: 7912
			FreeTrial
		}
	}
}
