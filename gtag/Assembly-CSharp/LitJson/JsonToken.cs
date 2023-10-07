using System;

namespace LitJson
{
	// Token: 0x0200027C RID: 636
	public enum JsonToken
	{
		// Token: 0x040011F8 RID: 4600
		None,
		// Token: 0x040011F9 RID: 4601
		ObjectStart,
		// Token: 0x040011FA RID: 4602
		PropertyName,
		// Token: 0x040011FB RID: 4603
		ObjectEnd,
		// Token: 0x040011FC RID: 4604
		ArrayStart,
		// Token: 0x040011FD RID: 4605
		ArrayEnd,
		// Token: 0x040011FE RID: 4606
		Int,
		// Token: 0x040011FF RID: 4607
		Long,
		// Token: 0x04001200 RID: 4608
		Double,
		// Token: 0x04001201 RID: 4609
		String,
		// Token: 0x04001202 RID: 4610
		Boolean,
		// Token: 0x04001203 RID: 4611
		Null
	}
}
