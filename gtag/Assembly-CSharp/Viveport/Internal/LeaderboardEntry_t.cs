using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x0200025E RID: 606
	internal struct LeaderboardEntry_t
	{
		// Token: 0x040011BE RID: 4542
		internal int m_nGlobalRank;

		// Token: 0x040011BF RID: 4543
		internal int m_nScore;

		// Token: 0x040011C0 RID: 4544
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string m_pUserName;
	}
}
