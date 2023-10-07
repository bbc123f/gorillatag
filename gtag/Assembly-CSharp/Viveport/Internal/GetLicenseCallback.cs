using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000252 RID: 594
	// (Invoke) Token: 0x06000EBD RID: 3773
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void GetLicenseCallback([MarshalAs(UnmanagedType.LPStr)] string message, [MarshalAs(UnmanagedType.LPStr)] string signature);
}
