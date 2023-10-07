using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000255 RID: 597
	// (Invoke) Token: 0x06000EC9 RID: 3785
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void QueryRuntimeModeCallback(int nResult, int nMode);
}
