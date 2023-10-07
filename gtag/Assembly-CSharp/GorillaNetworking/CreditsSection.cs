using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x020002AF RID: 687
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06001227 RID: 4647 RVA: 0x0006846A File Offset: 0x0006666A
		// (set) Token: 0x06001228 RID: 4648 RVA: 0x00068472 File Offset: 0x00066672
		public string Title { get; set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06001229 RID: 4649 RVA: 0x0006847B File Offset: 0x0006667B
		// (set) Token: 0x0600122A RID: 4650 RVA: 0x00068483 File Offset: 0x00066683
		public List<string> Entries { get; set; }
	}
}
