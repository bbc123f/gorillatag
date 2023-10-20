using System;

namespace BuildSafe
{
	// Token: 0x020002C6 RID: 710
	public class SessionState
	{
		// Token: 0x17000125 RID: 293
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

		// Token: 0x0400160B RID: 5643
		public static readonly SessionState Shared = new SessionState();
	}
}
