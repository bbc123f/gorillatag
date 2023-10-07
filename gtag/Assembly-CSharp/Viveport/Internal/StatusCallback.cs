using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000253 RID: 595
	// (Invoke) Token: 0x06000EC1 RID: 3777
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback(int nResult);
}
