using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x020002BE RID: 702
	public static class Callbacks
	{
		// Token: 0x020004E3 RID: 1251
		[Conditional("UNITY_EDITOR")]
		public class DidReloadScripts : Attribute
		{
			// Token: 0x06001EFE RID: 7934 RVA: 0x000A10AF File Offset: 0x0009F2AF
			public DidReloadScripts(bool activeOnly = false)
			{
				this.activeOnly = activeOnly;
			}

			// Token: 0x04002060 RID: 8288
			public bool activeOnly;
		}
	}
}
