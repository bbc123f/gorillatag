using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000253 RID: 595
	// (Invoke) Token: 0x06000EC4 RID: 3780
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void GetLicenseCallback([MarshalAs(UnmanagedType.LPStr)] string message, [MarshalAs(UnmanagedType.LPStr)] string signature);
}
