using System;

namespace BuildSafe
{
	// Token: 0x020002C4 RID: 708
	public class SessionState
	{
		// Token: 0x17000123 RID: 291
		public string this[string key]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x040015FE RID: 5630
		public static readonly SessionState Shared = new SessionState();
	}
}
