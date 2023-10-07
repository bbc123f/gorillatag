using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000260 RID: 608
	internal struct IAPCurrency_t
	{
		// Token: 0x040011C1 RID: 4545
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		internal string m_pName;

		// Token: 0x040011C2 RID: 4546
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		internal string m_pSymbol;
	}
}
