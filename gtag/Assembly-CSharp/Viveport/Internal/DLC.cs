using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Viveport.Internal
{
	// Token: 0x02000266 RID: 614
	internal class DLC
	{
		// Token: 0x06000F37 RID: 3895 RVA: 0x00053AB1 File Offset: 0x00051CB1
		static DLC()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06000F38 RID: 3896
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_IsReady")]
		internal static extern int IsReady(StatusCallback callback);

		// Token: 0x06000F39 RID: 3897
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_IsReady")]
		internal static extern int IsReady_64(StatusCallback callback);

		// Token: 0x06000F3A RID: 3898
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_GetCount")]
		internal static extern int GetCount();

		// Token: 0x06000F3B RID: 3899
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_GetCount")]
		internal static extern int GetCount_64();

		// Token: 0x06000F3C RID: 3900
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_GetIsAvailable")]
		internal static extern bool GetIsAvailable(int index, StringBuilder appId, out bool isAvailable);

		// Token: 0x06000F3D RID: 3901
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_GetIsAvailable")]
		internal static extern bool GetIsAvailable_64(int index, StringBuilder appId, out bool isAvailable);

		// Token: 0x06000F3E RID: 3902
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_IsSubscribed")]
		internal static extern int IsSubscribed();

		// Token: 0x06000F3F RID: 3903
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_IsSubscribed")]
		internal static extern int IsSubscribed_64();
	}
}
