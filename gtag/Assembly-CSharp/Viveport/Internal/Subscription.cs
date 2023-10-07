using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000267 RID: 615
	internal class Subscription
	{
		// Token: 0x06000F41 RID: 3905 RVA: 0x00053AC5 File Offset: 0x00051CC5
		static Subscription()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06000F42 RID: 3906
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsReady")]
		internal static extern void IsReady(StatusCallback2 IsReadyCallback);

		// Token: 0x06000F43 RID: 3907
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsReady")]
		internal static extern void IsReady_64(StatusCallback2 IsReadyCallback);

		// Token: 0x06000F44 RID: 3908
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsWindowsSubscriber")]
		internal static extern bool IsWindowsSubscriber();

		// Token: 0x06000F45 RID: 3909
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsWindowsSubscriber")]
		internal static extern bool IsWindowsSubscriber_64();

		// Token: 0x06000F46 RID: 3910
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsAndroidSubscriber")]
		internal static extern bool IsAndroidSubscriber();

		// Token: 0x06000F47 RID: 3911
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_IsAndroidSubscriber")]
		internal static extern bool IsAndroidSubscriber_64();

		// Token: 0x06000F48 RID: 3912
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_GetTransactionType")]
		internal static extern ESubscriptionTransactionType GetTransactionType();

		// Token: 0x06000F49 RID: 3913
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportSubscription_GetTransactionType")]
		internal static extern ESubscriptionTransactionType GetTransactionType_64();
	}
}
