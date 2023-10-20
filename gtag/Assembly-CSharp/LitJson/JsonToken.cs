using System;

namespace LitJson
{
	// Token: 0x0200027D RID: 637
	public enum JsonToken
	{
		// Token: 0x040011FE RID: 4606
		None,
		// Token: 0x040011FF RID: 4607
		ObjectStart,
		// Token: 0x04001200 RID: 4608
		PropertyName,
		// Token: 0x04001201 RID: 4609
		ObjectEnd,
		// Token: 0x04001202 RID: 4610
		ArrayStart,
		// Token: 0x04001203 RID: 4611
		ArrayEnd,
		// Token: 0x04001204 RID: 4612
		Int,
		// Token: 0x04001205 RID: 4613
		Long,
		// Token: 0x04001206 RID: 4614
		Double,
		// Token: 0x04001207 RID: 4615
		String,
		// Token: 0x04001208 RID: 4616
		Boolean,
		// Token: 0x04001209 RID: 4617
		Null
	}
}
