using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000254 RID: 596
	// (Invoke) Token: 0x06000EC5 RID: 3781
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback2(int nResult, [MarshalAs(UnmanagedType.LPStr)] string message);
}
