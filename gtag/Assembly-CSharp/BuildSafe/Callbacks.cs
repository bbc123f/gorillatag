using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x020002BC RID: 700
	public static class Callbacks
	{
		// Token: 0x020004E1 RID: 1249
		[Conditional("UNITY_EDITOR")]
		public class DidReloadScripts : Attribute
		{
			// Token: 0x06001EF5 RID: 7925 RVA: 0x000A0DA3 File Offset: 0x0009EFA3
			public DidReloadScripts(bool activeOnly = false)
			{
				this.activeOnly = activeOnly;
			}

			// Token: 0x04002053 RID: 8275
			public bool activeOnly;
		}
	}
}
