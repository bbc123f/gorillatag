using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x020002B1 RID: 689
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600122E RID: 4654 RVA: 0x00068936 File Offset: 0x00066B36
		// (set) Token: 0x0600122F RID: 4655 RVA: 0x0006893E File Offset: 0x00066B3E
		public string Title { get; set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06001230 RID: 4656 RVA: 0x00068947 File Offset: 0x00066B47
		// (set) Token: 0x06001231 RID: 4657 RVA: 0x0006894F File Offset: 0x00066B4F
		public List<string> Entries { get; set; }
	}
}
