using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x0200025F RID: 607
	internal struct LeaderboardEntry_t
	{
		// Token: 0x040011C4 RID: 4548
		internal int m_nGlobalRank;

		// Token: 0x040011C5 RID: 4549
		internal int m_nScore;

		// Token: 0x040011C6 RID: 4550
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string m_pUserName;
	}
}
