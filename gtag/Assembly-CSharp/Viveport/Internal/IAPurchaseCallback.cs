using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x0200025F RID: 607
	// (Invoke) Token: 0x06000ECD RID: 3789
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void IAPurchaseCallback(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
}
