using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000250 RID: 592
	public class Deeplink
	{
		// Token: 0x06000EAB RID: 3755 RVA: 0x00053621 File Offset: 0x00051821
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			Deeplink.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x00053630 File Offset: 0x00051830
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Deeplink.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.IsReady_64(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
				return;
			}
			Deeplink.IsReady(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x0005369D File Offset: 0x0005189D
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x000536AC File Offset: 0x000518AC
		public static void GoToApp(StatusCallback2 callback, string viveportId, string launchData)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToApp_64(new StatusCallback2(Deeplink.GoToAppIl2cppCallback), viveportId, launchData);
				return;
			}
			Deeplink.GoToApp(new StatusCallback2(Deeplink.GoToAppIl2cppCallback), viveportId, launchData);
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x00053725 File Offset: 0x00051925
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppWithBranchNameIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppWithBranchNameIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x00053734 File Offset: 0x00051934
		public static void GoToApp(StatusCallback2 callback, string viveportId, string launchData, string branchName)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppWithBranchNameIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToApp_64(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback), viveportId, launchData, branchName);
				return;
			}
			Deeplink.GoToApp(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback), viveportId, launchData, branchName);
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x000537AF File Offset: 0x000519AF
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToStoreIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToStoreIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x000537C0 File Offset: 0x000519C0
		public static void GoToStore(StatusCallback2 callback, string viveportId = "")
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToStoreIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToStore_64(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback), viveportId);
				return;
			}
			Deeplink.GoToStore(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback), viveportId);
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0005382F File Offset: 0x00051A2F
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppOrGoToStoreIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppOrGoToStoreIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x00053840 File Offset: 0x00051A40
		public static void GoToAppOrGoToStore(StatusCallback2 callback, string viveportId, string launchData)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppOrGoToStoreIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToAppOrGoToStore_64(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback), viveportId, launchData);
				return;
			}
			Deeplink.GoToAppOrGoToStore(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback), viveportId, launchData);
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x000538BC File Offset: 0x00051ABC
		public static string GetAppLaunchData()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				Deeplink.GetAppLaunchData_64(stringBuilder, 256);
			}
			else
			{
				Deeplink.GetAppLaunchData(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0400118D RID: 4493
		private const int MaxIdLength = 256;

		// Token: 0x0400118E RID: 4494
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400118F RID: 4495
		private static StatusCallback2 goToAppIl2cppCallback;

		// Token: 0x04001190 RID: 4496
		private static StatusCallback2 goToAppWithBranchNameIl2cppCallback;

		// Token: 0x04001191 RID: 4497
		private static StatusCallback2 goToStoreIl2cppCallback;

		// Token: 0x04001192 RID: 4498
		private static StatusCallback2 goToAppOrGoToStoreIl2cppCallback;
	}
}
