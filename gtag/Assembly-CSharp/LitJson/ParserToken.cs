using System;

namespace LitJson
{
	// Token: 0x02000283 RID: 643
	internal enum ParserToken
	{
		// Token: 0x0400123B RID: 4667
		None = 65536,
		// Token: 0x0400123C RID: 4668
		Number,
		// Token: 0x0400123D RID: 4669
		True,
		// Token: 0x0400123E RID: 4670
		False,
		// Token: 0x0400123F RID: 4671
		Null,
		// Token: 0x04001240 RID: 4672
		CharSeq,
		// Token: 0x04001241 RID: 4673
		Char,
		// Token: 0x04001242 RID: 4674
		Text,
		// Token: 0x04001243 RID: 4675
		Object,
		// Token: 0x04001244 RID: 4676
		ObjectPrime,
		// Token: 0x04001245 RID: 4677
		Pair,
		// Token: 0x04001246 RID: 4678
		PairRest,
		// Token: 0x04001247 RID: 4679
		Array,
		// Token: 0x04001248 RID: 4680
		ArrayPrime,
		// Token: 0x04001249 RID: 4681
		Value,
		// Token: 0x0400124A RID: 4682
		ValueRest,
		// Token: 0x0400124B RID: 4683
		String,
		// Token: 0x0400124C RID: 4684
		End,
		// Token: 0x0400124D RID: 4685
		Epsilon
	}
}
