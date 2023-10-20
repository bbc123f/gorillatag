using System;

namespace LitJson
{
	// Token: 0x02000284 RID: 644
	internal enum ParserToken
	{
		// Token: 0x04001241 RID: 4673
		None = 65536,
		// Token: 0x04001242 RID: 4674
		Number,
		// Token: 0x04001243 RID: 4675
		True,
		// Token: 0x04001244 RID: 4676
		False,
		// Token: 0x04001245 RID: 4677
		Null,
		// Token: 0x04001246 RID: 4678
		CharSeq,
		// Token: 0x04001247 RID: 4679
		Char,
		// Token: 0x04001248 RID: 4680
		Text,
		// Token: 0x04001249 RID: 4681
		Object,
		// Token: 0x0400124A RID: 4682
		ObjectPrime,
		// Token: 0x0400124B RID: 4683
		Pair,
		// Token: 0x0400124C RID: 4684
		PairRest,
		// Token: 0x0400124D RID: 4685
		Array,
		// Token: 0x0400124E RID: 4686
		ArrayPrime,
		// Token: 0x0400124F RID: 4687
		Value,
		// Token: 0x04001250 RID: 4688
		ValueRest,
		// Token: 0x04001251 RID: 4689
		String,
		// Token: 0x04001252 RID: 4690
		End,
		// Token: 0x04001253 RID: 4691
		Epsilon
	}
}
