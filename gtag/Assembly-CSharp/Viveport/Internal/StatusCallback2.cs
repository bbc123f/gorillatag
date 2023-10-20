using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000255 RID: 597
	// (Invoke) Token: 0x06000ECC RID: 3788
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback2(int nResult, [MarshalAs(UnmanagedType.LPStr)] string message);
}
