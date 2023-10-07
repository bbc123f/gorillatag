using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000262 RID: 610
	internal class Api
	{
		// Token: 0x06000EE0 RID: 3808 RVA: 0x00053A1F File Offset: 0x00051C1F
		static Api()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06000EE1 RID: 3809
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_GetLicense")]
		internal static extern void GetLicense(GetLicenseCallback callback, string appId, string appKey);

		// Token: 0x06000EE2 RID: 3810
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_GetLicense")]
		internal static extern void GetLicense_64(GetLicenseCallback callback, string appId, string appKey);

		// Token: 0x06000EE3 RID: 3811
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Init")]
		internal static extern int Init(StatusCallback initCallback, string appId);

		// Token: 0x06000EE4 RID: 3812
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Init")]
		internal static extern int Init_64(StatusCallback initCallback, string appId);

		// Token: 0x06000EE5 RID: 3813
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Shutdown")]
		internal static extern int Shutdown(StatusCallback initCallback);

		// Token: 0x06000EE6 RID: 3814
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Shutdown")]
		internal static extern int Shutdown_64(StatusCallback initCallback);

		// Token: 0x06000EE7 RID: 3815
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Version")]
		internal static extern IntPtr Version();

		// Token: 0x06000EE8 RID: 3816
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Version")]
		internal static extern IntPtr Version_64();

		// Token: 0x06000EE9 RID: 3817
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_QueryRuntimeMode")]
		internal static extern void QueryRuntimeMode(QueryRuntimeModeCallback queryRunTimeCallback);

		// Token: 0x06000EEA RID: 3818
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_QueryRuntimeMode")]
		internal static extern void QueryRuntimeMode_64(QueryRuntimeModeCallback queryRunTimeCallback);

		// Token: 0x06000EEB RID: 3819
		[DllImport("kernel32.dll")]
		internal static extern IntPtr LoadLibrary(string dllToLoad);

		// Token: 0x06000EEC RID: 3820 RVA: 0x00053A2C File Offset: 0x00051C2C
		internal static void LoadLibraryManually(string dllName)
		{
			if (string.IsNullOrEmpty(dllName))
			{
				return;
			}
			if (IntPtr.Size == 8)
			{
				Api.LoadLibrary("x64/" + dllName + "64.dll");
				return;
			}
			Api.LoadLibrary("x86/" + dllName + ".dll");
		}
	}
}
