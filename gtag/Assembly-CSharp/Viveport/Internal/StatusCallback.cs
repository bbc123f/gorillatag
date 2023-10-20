using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000254 RID: 596
	// (Invoke) Token: 0x06000EC8 RID: 3784
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback(int nResult);
}
