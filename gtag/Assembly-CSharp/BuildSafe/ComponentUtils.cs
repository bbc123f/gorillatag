using System;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x020002BF RID: 703
	public static class ComponentUtils
	{
		// Token: 0x06001303 RID: 4867 RVA: 0x0006EABF File Offset: 0x0006CCBF
		public static Hash128 GetComponentID(Component c, string k)
		{
			return ComponentUtils.GetComponentID(c, StaticHash.Calculate(k));
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0006EAD0 File Offset: 0x0006CCD0
		public static Hash128 GetComponentID(Component c, int k = 0)
		{
			return default(Hash128);
		}
	}
}
