using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000256 RID: 598
	// (Invoke) Token: 0x06000ED0 RID: 3792
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void QueryRuntimeModeCallback(int nResult, int nMode);
}
