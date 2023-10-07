using System;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x020002BD RID: 701
	public static class ComponentUtils
	{
		// Token: 0x060012FC RID: 4860 RVA: 0x0006E5F3 File Offset: 0x0006C7F3
		public static Hash128 GetComponentID(Component c, string k)
		{
			return ComponentUtils.GetComponentID(c, StaticHash.Calculate(k));
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x0006E604 File Offset: 0x0006C804
		public static Hash128 GetComponentID(Component c, int k = 0)
		{
			return default(Hash128);
		}
	}
}
