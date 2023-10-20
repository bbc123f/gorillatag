using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000260 RID: 608
	// (Invoke) Token: 0x06000ED4 RID: 3796
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void IAPurchaseCallback(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
}
