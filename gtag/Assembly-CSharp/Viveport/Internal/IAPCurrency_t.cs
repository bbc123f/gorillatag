using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000261 RID: 609
	internal struct IAPCurrency_t
	{
		// Token: 0x040011C7 RID: 4551
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		internal string m_pName;

		// Token: 0x040011C8 RID: 4552
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		internal string m_pSymbol;
	}
}
