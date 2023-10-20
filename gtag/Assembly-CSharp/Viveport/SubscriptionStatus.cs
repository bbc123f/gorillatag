using System;
using System.Collections.Generic;

namespace Viveport
{
	// Token: 0x02000249 RID: 585
	public class SubscriptionStatus
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000E51 RID: 3665 RVA: 0x0005257D File Offset: 0x0005077D
		// (set) Token: 0x06000E52 RID: 3666 RVA: 0x00052585 File Offset: 0x00050785
		public List<SubscriptionStatus.Platform> Platforms { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000E53 RID: 3667 RVA: 0x0005258E File Offset: 0x0005078E
		// (set) Token: 0x06000E54 RID: 3668 RVA: 0x00052596 File Offset: 0x00050796
		public SubscriptionStatus.TransactionType Type { get; set; }

		// Token: 0x06000E55 RID: 3669 RVA: 0x0005259F File Offset: 0x0005079F
		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		// Token: 0x0200048B RID: 1163
		public enum Platform
		{
			// Token: 0x04001EEF RID: 7919
			Windows,
			// Token: 0x04001EF0 RID: 7920
			Android
		}

		// Token: 0x0200048C RID: 1164
		public enum TransactionType
		{
			// Token: 0x04001EF2 RID: 7922
			Unknown,
			// Token: 0x04001EF3 RID: 7923
			Paid,
			// Token: 0x04001EF4 RID: 7924
			Redeem,
			// Token: 0x04001EF5 RID: 7925
			FreeTrial
		}
	}
}
