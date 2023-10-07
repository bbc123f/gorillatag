using System;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal.Arcade;

namespace Viveport.Arcade
{
	// Token: 0x0200026C RID: 620
	internal class Session
	{
		// Token: 0x06000F6A RID: 3946 RVA: 0x00053B09 File Offset: 0x00051D09
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			Session.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x00053B17 File Offset: 0x00051D17
		public static void IsReady(Session.SessionListener listener)
		{
			Session.isReadyIl2cppCallback = new Session.SessionHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				Session.IsReady_64(new SessionCallback(Session.IsReadyIl2cppCallback));
				return;
			}
			Session.IsReady(new SessionCallback(Session.IsReadyIl2cppCallback));
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x00053B54 File Offset: 0x00051D54
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StartIl2cppCallback(int errorCode, string message)
		{
			Session.startIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x00053B62 File Offset: 0x00051D62
		public static void Start(Session.SessionListener listener)
		{
			Session.startIl2cppCallback = new Session.SessionHandler(listener).getStartHandler();
			if (IntPtr.Size == 8)
			{
				Session.Start_64(new SessionCallback(Session.StartIl2cppCallback));
				return;
			}
			Session.Start(new SessionCallback(Session.StartIl2cppCallback));
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x00053B9F File Offset: 0x00051D9F
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StopIl2cppCallback(int errorCode, string message)
		{
			Session.stopIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x00053BAD File Offset: 0x00051DAD
		public static void Stop(Session.SessionListener listener)
		{
			Session.stopIl2cppCallback = new Session.SessionHandler(listener).getStopHandler();
			if (IntPtr.Size == 8)
			{
				Session.Stop_64(new SessionCallback(Session.StopIl2cppCallback));
				return;
			}
			Session.Stop(new SessionCallback(Session.StopIl2cppCallback));
		}

		// Token: 0x040011C3 RID: 4547
		private static SessionCallback isReadyIl2cppCallback;

		// Token: 0x040011C4 RID: 4548
		private static SessionCallback startIl2cppCallback;

		// Token: 0x040011C5 RID: 4549
		private static SessionCallback stopIl2cppCallback;

		// Token: 0x0200049E RID: 1182
		private class SessionHandler : Session.BaseHandler
		{
			// Token: 0x06001E09 RID: 7689 RVA: 0x0009D9BE File Offset: 0x0009BBBE
			public SessionHandler(Session.SessionListener cb)
			{
				Session.SessionHandler.listener = cb;
			}

			// Token: 0x06001E0A RID: 7690 RVA: 0x0009D9CC File Offset: 0x0009BBCC
			public SessionCallback getIsReadyHandler()
			{
				return new SessionCallback(this.IsReadyHandler);
			}

			// Token: 0x06001E0B RID: 7691 RVA: 0x0009D9DC File Offset: 0x0009BBDC
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session IsReadyHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string str = "[Session IsReadyHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string str2 = "[IsReadyHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							text2 = (string)jsonData["appID"];
						}
						catch (Exception ex5)
						{
							string str3 = "[IsReadyHandler] appID ex=";
							Exception ex6 = ex5;
							Logger.Log(str3 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] appID=" + text2);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnSuccess(text2);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001E0C RID: 7692 RVA: 0x0009DB50 File Offset: 0x0009BD50
			public SessionCallback getStartHandler()
			{
				return new SessionCallback(this.StartHandler);
			}

			// Token: 0x06001E0D RID: 7693 RVA: 0x0009DB60 File Offset: 0x0009BD60
			protected override void StartHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session StartHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string str = "[Session StartHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string str2 = "[StartHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StartHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							text2 = (string)jsonData["appID"];
							text3 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string str3 = "[StartHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(str3 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StartHandler] appID=" + text2 + ",Guid=" + text3);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStartSuccess(text2, text3);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001E0E RID: 7694 RVA: 0x0009DCF4 File Offset: 0x0009BEF4
			public SessionCallback getStopHandler()
			{
				return new SessionCallback(this.StopHandler);
			}

			// Token: 0x06001E0F RID: 7695 RVA: 0x0009DD04 File Offset: 0x0009BF04
			protected override void StopHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session StopHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string str = "[Session StopHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string str2 = "[StopHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StopHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							text2 = (string)jsonData["appID"];
							text3 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string str3 = "[StopHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(str3 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StopHandler] appID=" + text2 + ",Guid=" + text3);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStopSuccess(text2, text3);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x04001F30 RID: 7984
			private static Session.SessionListener listener;
		}

		// Token: 0x0200049F RID: 1183
		private abstract class BaseHandler
		{
			// Token: 0x06001E10 RID: 7696
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001E11 RID: 7697
			protected abstract void StartHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001E12 RID: 7698
			protected abstract void StopHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x020004A0 RID: 1184
		public class SessionListener
		{
			// Token: 0x06001E14 RID: 7700 RVA: 0x0009DEA0 File Offset: 0x0009C0A0
			public virtual void OnSuccess(string pchAppID)
			{
			}

			// Token: 0x06001E15 RID: 7701 RVA: 0x0009DEA2 File Offset: 0x0009C0A2
			public virtual void OnStartSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x06001E16 RID: 7702 RVA: 0x0009DEA4 File Offset: 0x0009C0A4
			public virtual void OnStopSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x06001E17 RID: 7703 RVA: 0x0009DEA6 File Offset: 0x0009C0A6
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}
		}
	}
}
