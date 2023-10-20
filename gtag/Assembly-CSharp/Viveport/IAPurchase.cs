using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x0200024E RID: 590
	public class IAPurchase
	{
		// Token: 0x06000E8D RID: 3725 RVA: 0x0005336B File Offset: 0x0005156B
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x00053379 File Offset: 0x00051579
		public static void IsReady(IAPurchase.IAPurchaseListener listener, string pchAppKey)
		{
			IAPurchase.isReadyIl2cppCallback = new IAPurchase.IAPHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.IsReady_64(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
				return;
			}
			IAPurchase.IsReady(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x000533B8 File Offset: 0x000515B8
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x000533C6 File Offset: 0x000515C6
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice)
		{
			IAPurchase.request01Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x00053405 File Offset: 0x00051605
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x00053414 File Offset: 0x00051614
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchUserData)
		{
			IAPurchase.request02Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x00053460 File Offset: 0x00051660
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void PurchaseIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.purchaseIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x0005346E File Offset: 0x0005166E
		public static void Purchase(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.purchaseIl2cppCallback = new IAPurchase.IAPHandler(listener).getPurchaseHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Purchase_64(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Purchase(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x000534AD File Offset: 0x000516AD
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x000534BB File Offset: 0x000516BB
		public static void Query(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.query01Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x000534FA File Offset: 0x000516FA
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x00053508 File Offset: 0x00051708
		public static void Query(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.query02Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x00053545 File Offset: 0x00051745
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void GetBalanceIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.getBalanceIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x00053553 File Offset: 0x00051753
		public static void GetBalance(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.getBalanceIl2cppCallback = new IAPurchase.IAPHandler(listener).getBalanceHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.GetBalance_64(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
				return;
			}
			IAPurchase.GetBalance(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
		}

		// Token: 0x06000E9B RID: 3739 RVA: 0x00053590 File Offset: 0x00051790
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x000535A0 File Offset: 0x000517A0
		public static void RequestSubscription(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId)
		{
			IAPurchase.requestSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscription_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
				return;
			}
			IAPurchase.RequestSubscription(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x000535FE File Offset: 0x000517FE
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionWithPlanIDIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x0005360C File Offset: 0x0005180C
		public static void RequestSubscriptionWithPlanID(IAPurchase.IAPurchaseListener listener, string pchPlanId)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionWithPlanIDHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscriptionWithPlanID_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
				return;
			}
			IAPurchase.RequestSubscriptionWithPlanID(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x0005364B File Offset: 0x0005184B
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void SubscribeIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.subscribeIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x00053659 File Offset: 0x00051859
		public static void Subscribe(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.subscribeIl2cppCallback = new IAPurchase.IAPHandler(listener).getSubscribeHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Subscribe_64(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.Subscribe(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x00053698 File Offset: 0x00051898
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x000536A6 File Offset: 0x000518A6
		public static void QuerySubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.querySubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscription_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.QuerySubscription(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x000536E5 File Offset: 0x000518E5
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionListIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionListIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x000536F3 File Offset: 0x000518F3
		public static void QuerySubscriptionList(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.querySubscriptionListIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscriptionList_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
				return;
			}
			IAPurchase.QuerySubscriptionList(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x00053730 File Offset: 0x00051930
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void CancelSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x0005373E File Offset: 0x0005193E
		public static void CancelSubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getCancelSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.CancelSubscription_64(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.CancelSubscription(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x04001183 RID: 4483
		private static IAPurchaseCallback isReadyIl2cppCallback;

		// Token: 0x04001184 RID: 4484
		private static IAPurchaseCallback request01Il2cppCallback;

		// Token: 0x04001185 RID: 4485
		private static IAPurchaseCallback request02Il2cppCallback;

		// Token: 0x04001186 RID: 4486
		private static IAPurchaseCallback purchaseIl2cppCallback;

		// Token: 0x04001187 RID: 4487
		private static IAPurchaseCallback query01Il2cppCallback;

		// Token: 0x04001188 RID: 4488
		private static IAPurchaseCallback query02Il2cppCallback;

		// Token: 0x04001189 RID: 4489
		private static IAPurchaseCallback getBalanceIl2cppCallback;

		// Token: 0x0400118A RID: 4490
		private static IAPurchaseCallback requestSubscriptionIl2cppCallback;

		// Token: 0x0400118B RID: 4491
		private static IAPurchaseCallback requestSubscriptionWithPlanIDIl2cppCallback;

		// Token: 0x0400118C RID: 4492
		private static IAPurchaseCallback subscribeIl2cppCallback;

		// Token: 0x0400118D RID: 4493
		private static IAPurchaseCallback querySubscriptionIl2cppCallback;

		// Token: 0x0400118E RID: 4494
		private static IAPurchaseCallback querySubscriptionListIl2cppCallback;

		// Token: 0x0400118F RID: 4495
		private static IAPurchaseCallback cancelSubscriptionIl2cppCallback;

		// Token: 0x02000495 RID: 1173
		private class IAPHandler : IAPurchase.BaseHandler
		{
			// Token: 0x06001D86 RID: 7558 RVA: 0x0009C6F2 File Offset: 0x0009A8F2
			public IAPHandler(IAPurchase.IAPurchaseListener cb)
			{
				IAPurchase.IAPHandler.listener = cb;
			}

			// Token: 0x06001D87 RID: 7559 RVA: 0x0009C700 File Offset: 0x0009A900
			public IAPurchaseCallback getIsReadyHandler()
			{
				return new IAPurchaseCallback(this.IsReadyHandler);
			}

			// Token: 0x06001D88 RID: 7560 RVA: 0x0009C710 File Offset: 0x0009A910
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[IsReadyHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[IsReadyHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
						}
						catch (Exception ex3)
						{
							string str2 = "[IsReadyHandler] currencyName ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] currencyName=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D89 RID: 7561 RVA: 0x0009C840 File Offset: 0x0009AA40
			public IAPurchaseCallback getRequestHandler()
			{
				return new IAPurchaseCallback(this.RequestHandler);
			}

			// Token: 0x06001D8A RID: 7562 RVA: 0x0009C850 File Offset: 0x0009AA50
			protected override void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[RequestHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[RequestHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
						}
						catch (Exception ex3)
						{
							string str2 = "[RequestHandler] purchase_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[RequestHandler] purchaseId =" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D8B RID: 7563 RVA: 0x0009C980 File Offset: 0x0009AB80
			public IAPurchaseCallback getPurchaseHandler()
			{
				return new IAPurchaseCallback(this.PurchaseHandler);
			}

			// Token: 0x06001D8C RID: 7564 RVA: 0x0009C990 File Offset: 0x0009AB90
			protected override void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[PurchaseHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				long num2 = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[PurchaseHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[PurchaseHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							num2 = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string str2 = "[PurchaseHandler] purchase_id,paid_timestamp ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[PurchaseHandler] purchaseId =" + text + ",paid_timestamp=" + num2.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnPurchaseSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D8D RID: 7565 RVA: 0x0009CAE0 File Offset: 0x0009ACE0
			public IAPurchaseCallback getQueryHandler()
			{
				return new IAPurchaseCallback(this.QueryHandler);
			}

			// Token: 0x06001D8E RID: 7566 RVA: 0x0009CAF0 File Offset: 0x0009ACF0
			protected override void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				string text6 = "";
				long paid_timestamp = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QueryHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							text3 = (string)jsonData["order_id"];
							text4 = (string)jsonData["status"];
							text5 = (string)jsonData["price"];
							text6 = (string)jsonData["currency"];
							paid_timestamp = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string str2 = "[QueryHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] status =",
							text4,
							",price=",
							text5,
							",currency=",
							text6
						}));
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] purchaseId =",
							text,
							",order_id=",
							text3,
							",paid_timestamp=",
							paid_timestamp.ToString()
						}));
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryResponse queryResponse = new IAPurchase.QueryResponse();
							queryResponse.purchase_id = text;
							queryResponse.order_id = text3;
							queryResponse.price = text5;
							queryResponse.currency = text6;
							queryResponse.paid_timestamp = paid_timestamp;
							queryResponse.status = text4;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D8F RID: 7567 RVA: 0x0009CD3C File Offset: 0x0009AF3C
			public IAPurchaseCallback getQueryListHandler()
			{
				return new IAPurchaseCallback(this.QueryListHandler);
			}

			// Token: 0x06001D90 RID: 7568 RVA: 0x0009CD4C File Offset: 0x0009AF4C
			protected override void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				int total = 0;
				int from = 0;
				int to = 0;
				List<IAPurchase.QueryResponse2> list = new List<IAPurchase.QueryResponse2>();
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QueryListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							JsonData jsonData2 = JsonMapper.ToObject(text);
							total = (int)jsonData2["total"];
							from = (int)jsonData2["from"];
							to = (int)jsonData2["to"];
							JsonData jsonData3 = jsonData2["purchases"];
							bool isArray = jsonData3.IsArray;
							foreach (object obj in ((IEnumerable)jsonData3))
							{
								JsonData jsonData4 = (JsonData)obj;
								IAPurchase.QueryResponse2 queryResponse = new IAPurchase.QueryResponse2();
								IDictionary dictionary = jsonData4;
								queryResponse.app_id = (dictionary.Contains("app_id") ? ((string)jsonData4["app_id"]) : "");
								queryResponse.currency = (dictionary.Contains("currency") ? ((string)jsonData4["currency"]) : "");
								queryResponse.purchase_id = (dictionary.Contains("purchase_id") ? ((string)jsonData4["purchase_id"]) : "");
								queryResponse.order_id = (dictionary.Contains("order_id") ? ((string)jsonData4["order_id"]) : "");
								queryResponse.price = (dictionary.Contains("price") ? ((string)jsonData4["price"]) : "");
								queryResponse.user_data = (dictionary.Contains("user_data") ? ((string)jsonData4["user_data"]) : "");
								if (dictionary.Contains("paid_timestamp"))
								{
									if (jsonData4["paid_timestamp"].IsLong)
									{
										queryResponse.paid_timestamp = (long)jsonData4["paid_timestamp"];
									}
									else if (jsonData4["paid_timestamp"].IsInt)
									{
										queryResponse.paid_timestamp = (long)((int)jsonData4["paid_timestamp"]);
									}
								}
								list.Add(queryResponse);
							}
						}
						catch (Exception ex3)
						{
							string str2 = "[QueryListHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryListResponse queryListResponse = new IAPurchase.QueryListResponse();
							queryListResponse.total = total;
							queryListResponse.from = from;
							queryListResponse.to = to;
							queryListResponse.purchaseList = list;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryListResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D91 RID: 7569 RVA: 0x0009D0D4 File Offset: 0x0009B2D4
			public IAPurchaseCallback getBalanceHandler()
			{
				return new IAPurchaseCallback(this.BalanceHandler);
			}

			// Token: 0x06001D92 RID: 7570 RVA: 0x0009D0E4 File Offset: 0x0009B2E4
			protected override void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[BalanceHandler] code=" + code.ToString() + ",message= " + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string str = "";
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str2 = "[BalanceHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[BalanceHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							str = (string)jsonData["currencyName"];
							text = (string)jsonData["balance"];
						}
						catch (Exception ex3)
						{
							string str3 = "[BalanceHandler] currencyName, balance ex=";
							Exception ex4 = ex3;
							Logger.Log(str3 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[BalanceHandler] currencyName=" + str + ",balance=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnBalanceSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D93 RID: 7571 RVA: 0x0009D240 File Offset: 0x0009B440
			public IAPurchaseCallback getRequestSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionHandler);
			}

			// Token: 0x06001D94 RID: 7572 RVA: 0x0009D250 File Offset: 0x0009B450
			protected override void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[RequestSubscriptionHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string str2 = "[RequestSubscriptionHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D95 RID: 7573 RVA: 0x0009D378 File Offset: 0x0009B578
			public IAPurchaseCallback getRequestSubscriptionWithPlanIDHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionWithPlanIDHandler);
			}

			// Token: 0x06001D96 RID: 7574 RVA: 0x0009D388 File Offset: 0x0009B588
			protected override void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[RequestSubscriptionWithPlanIDHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string str2 = "[RequestSubscriptionWithPlanIDHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionWithPlanIDHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionWithPlanIDSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D97 RID: 7575 RVA: 0x0009D4B0 File Offset: 0x0009B6B0
			public IAPurchaseCallback getSubscribeHandler()
			{
				return new IAPurchaseCallback(this.SubscribeHandler);
			}

			// Token: 0x06001D98 RID: 7576 RVA: 0x0009D4C0 File Offset: 0x0009B6C0
			protected override void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[SubscribeHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				long num2 = 0L;
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[SubscribeHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[SubscribeHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
						text3 = (string)jsonData["plan_id"];
						num2 = (long)jsonData["subscribed_timestamp"];
					}
					catch (Exception ex3)
					{
						string str2 = "[SubscribeHandler] subscription_id, plan_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log(string.Concat(new string[]
					{
						"[SubscribeHandler] subscription_id =",
						text,
						", plan_id=",
						text3,
						", timestamp=",
						num2.ToString()
					}));
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSubscribeSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D99 RID: 7577 RVA: 0x0009D648 File Offset: 0x0009B848
			public IAPurchaseCallback getQuerySubscriptionHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionHandler);
			}

			// Token: 0x06001D9A RID: 7578 RVA: 0x0009D658 File Offset: 0x0009B858
			protected override void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QuerySubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string str2 = "[QuerySubscriptionHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D9B RID: 7579 RVA: 0x0009D780 File Offset: 0x0009B980
			public IAPurchaseCallback getQuerySubscriptionListHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionListHandler);
			}

			// Token: 0x06001D9C RID: 7580 RVA: 0x0009D790 File Offset: 0x0009B990
			protected override void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QuerySubscriptionListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string str2 = "[QuerySubscriptionListHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionListSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06001D9D RID: 7581 RVA: 0x0009D8B8 File Offset: 0x0009BAB8
			public IAPurchaseCallback getCancelSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.CancelSubscriptionHandler);
			}

			// Token: 0x06001D9E RID: 7582 RVA: 0x0009D8C8 File Offset: 0x0009BAC8
			protected override void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[CancelSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				bool bCanceled = false;
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[CancelSubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[CancelSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						bCanceled = true;
						Logger.Log("[CancelSubscriptionHandler] isCanceled = " + bCanceled.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnCancelSubscriptionSuccess(bCanceled);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x04001F13 RID: 7955
			private static IAPurchase.IAPurchaseListener listener;
		}

		// Token: 0x02000496 RID: 1174
		private abstract class BaseHandler
		{
			// Token: 0x06001D9F RID: 7583
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA0 RID: 7584
			protected abstract void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA1 RID: 7585
			protected abstract void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA2 RID: 7586
			protected abstract void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA3 RID: 7587
			protected abstract void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA4 RID: 7588
			protected abstract void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA5 RID: 7589
			protected abstract void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA6 RID: 7590
			protected abstract void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA7 RID: 7591
			protected abstract void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA8 RID: 7592
			protected abstract void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA9 RID: 7593
			protected abstract void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DAA RID: 7594
			protected abstract void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000497 RID: 1175
		public class IAPurchaseListener
		{
			// Token: 0x06001DAC RID: 7596 RVA: 0x0009D9C0 File Offset: 0x0009BBC0
			public virtual void OnSuccess(string pchCurrencyName)
			{
			}

			// Token: 0x06001DAD RID: 7597 RVA: 0x0009D9C2 File Offset: 0x0009BBC2
			public virtual void OnRequestSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x06001DAE RID: 7598 RVA: 0x0009D9C4 File Offset: 0x0009BBC4
			public virtual void OnPurchaseSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x06001DAF RID: 7599 RVA: 0x0009D9C6 File Offset: 0x0009BBC6
			public virtual void OnQuerySuccess(IAPurchase.QueryResponse response)
			{
			}

			// Token: 0x06001DB0 RID: 7600 RVA: 0x0009D9C8 File Offset: 0x0009BBC8
			public virtual void OnQuerySuccess(IAPurchase.QueryListResponse response)
			{
			}

			// Token: 0x06001DB1 RID: 7601 RVA: 0x0009D9CA File Offset: 0x0009BBCA
			public virtual void OnBalanceSuccess(string pchBalance)
			{
			}

			// Token: 0x06001DB2 RID: 7602 RVA: 0x0009D9CC File Offset: 0x0009BBCC
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}

			// Token: 0x06001DB3 RID: 7603 RVA: 0x0009D9CE File Offset: 0x0009BBCE
			public virtual void OnRequestSubscriptionSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06001DB4 RID: 7604 RVA: 0x0009D9D0 File Offset: 0x0009BBD0
			public virtual void OnRequestSubscriptionWithPlanIDSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06001DB5 RID: 7605 RVA: 0x0009D9D2 File Offset: 0x0009BBD2
			public virtual void OnSubscribeSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06001DB6 RID: 7606 RVA: 0x0009D9D4 File Offset: 0x0009BBD4
			public virtual void OnQuerySubscriptionSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06001DB7 RID: 7607 RVA: 0x0009D9D6 File Offset: 0x0009BBD6
			public virtual void OnQuerySubscriptionListSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06001DB8 RID: 7608 RVA: 0x0009D9D8 File Offset: 0x0009BBD8
			public virtual void OnCancelSubscriptionSuccess(bool bCanceled)
			{
			}
		}

		// Token: 0x02000498 RID: 1176
		public class QueryResponse
		{
			// Token: 0x17000261 RID: 609
			// (get) Token: 0x06001DBA RID: 7610 RVA: 0x0009D9E2 File Offset: 0x0009BBE2
			// (set) Token: 0x06001DBB RID: 7611 RVA: 0x0009D9EA File Offset: 0x0009BBEA
			public string order_id { get; set; }

			// Token: 0x17000262 RID: 610
			// (get) Token: 0x06001DBC RID: 7612 RVA: 0x0009D9F3 File Offset: 0x0009BBF3
			// (set) Token: 0x06001DBD RID: 7613 RVA: 0x0009D9FB File Offset: 0x0009BBFB
			public string purchase_id { get; set; }

			// Token: 0x17000263 RID: 611
			// (get) Token: 0x06001DBE RID: 7614 RVA: 0x0009DA04 File Offset: 0x0009BC04
			// (set) Token: 0x06001DBF RID: 7615 RVA: 0x0009DA0C File Offset: 0x0009BC0C
			public string status { get; set; }

			// Token: 0x17000264 RID: 612
			// (get) Token: 0x06001DC0 RID: 7616 RVA: 0x0009DA15 File Offset: 0x0009BC15
			// (set) Token: 0x06001DC1 RID: 7617 RVA: 0x0009DA1D File Offset: 0x0009BC1D
			public string price { get; set; }

			// Token: 0x17000265 RID: 613
			// (get) Token: 0x06001DC2 RID: 7618 RVA: 0x0009DA26 File Offset: 0x0009BC26
			// (set) Token: 0x06001DC3 RID: 7619 RVA: 0x0009DA2E File Offset: 0x0009BC2E
			public string currency { get; set; }

			// Token: 0x17000266 RID: 614
			// (get) Token: 0x06001DC4 RID: 7620 RVA: 0x0009DA37 File Offset: 0x0009BC37
			// (set) Token: 0x06001DC5 RID: 7621 RVA: 0x0009DA3F File Offset: 0x0009BC3F
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000499 RID: 1177
		public class QueryResponse2
		{
			// Token: 0x17000267 RID: 615
			// (get) Token: 0x06001DC7 RID: 7623 RVA: 0x0009DA50 File Offset: 0x0009BC50
			// (set) Token: 0x06001DC8 RID: 7624 RVA: 0x0009DA58 File Offset: 0x0009BC58
			public string order_id { get; set; }

			// Token: 0x17000268 RID: 616
			// (get) Token: 0x06001DC9 RID: 7625 RVA: 0x0009DA61 File Offset: 0x0009BC61
			// (set) Token: 0x06001DCA RID: 7626 RVA: 0x0009DA69 File Offset: 0x0009BC69
			public string app_id { get; set; }

			// Token: 0x17000269 RID: 617
			// (get) Token: 0x06001DCB RID: 7627 RVA: 0x0009DA72 File Offset: 0x0009BC72
			// (set) Token: 0x06001DCC RID: 7628 RVA: 0x0009DA7A File Offset: 0x0009BC7A
			public string purchase_id { get; set; }

			// Token: 0x1700026A RID: 618
			// (get) Token: 0x06001DCD RID: 7629 RVA: 0x0009DA83 File Offset: 0x0009BC83
			// (set) Token: 0x06001DCE RID: 7630 RVA: 0x0009DA8B File Offset: 0x0009BC8B
			public string user_data { get; set; }

			// Token: 0x1700026B RID: 619
			// (get) Token: 0x06001DCF RID: 7631 RVA: 0x0009DA94 File Offset: 0x0009BC94
			// (set) Token: 0x06001DD0 RID: 7632 RVA: 0x0009DA9C File Offset: 0x0009BC9C
			public string price { get; set; }

			// Token: 0x1700026C RID: 620
			// (get) Token: 0x06001DD1 RID: 7633 RVA: 0x0009DAA5 File Offset: 0x0009BCA5
			// (set) Token: 0x06001DD2 RID: 7634 RVA: 0x0009DAAD File Offset: 0x0009BCAD
			public string currency { get; set; }

			// Token: 0x1700026D RID: 621
			// (get) Token: 0x06001DD3 RID: 7635 RVA: 0x0009DAB6 File Offset: 0x0009BCB6
			// (set) Token: 0x06001DD4 RID: 7636 RVA: 0x0009DABE File Offset: 0x0009BCBE
			public long paid_timestamp { get; set; }
		}

		// Token: 0x0200049A RID: 1178
		public class QueryListResponse
		{
			// Token: 0x1700026E RID: 622
			// (get) Token: 0x06001DD6 RID: 7638 RVA: 0x0009DACF File Offset: 0x0009BCCF
			// (set) Token: 0x06001DD7 RID: 7639 RVA: 0x0009DAD7 File Offset: 0x0009BCD7
			public int total { get; set; }

			// Token: 0x1700026F RID: 623
			// (get) Token: 0x06001DD8 RID: 7640 RVA: 0x0009DAE0 File Offset: 0x0009BCE0
			// (set) Token: 0x06001DD9 RID: 7641 RVA: 0x0009DAE8 File Offset: 0x0009BCE8
			public int from { get; set; }

			// Token: 0x17000270 RID: 624
			// (get) Token: 0x06001DDA RID: 7642 RVA: 0x0009DAF1 File Offset: 0x0009BCF1
			// (set) Token: 0x06001DDB RID: 7643 RVA: 0x0009DAF9 File Offset: 0x0009BCF9
			public int to { get; set; }

			// Token: 0x04001F24 RID: 7972
			public List<IAPurchase.QueryResponse2> purchaseList;
		}

		// Token: 0x0200049B RID: 1179
		public class StatusDetailTransaction
		{
			// Token: 0x17000271 RID: 625
			// (get) Token: 0x06001DDD RID: 7645 RVA: 0x0009DB0A File Offset: 0x0009BD0A
			// (set) Token: 0x06001DDE RID: 7646 RVA: 0x0009DB12 File Offset: 0x0009BD12
			public long create_time { get; set; }

			// Token: 0x17000272 RID: 626
			// (get) Token: 0x06001DDF RID: 7647 RVA: 0x0009DB1B File Offset: 0x0009BD1B
			// (set) Token: 0x06001DE0 RID: 7648 RVA: 0x0009DB23 File Offset: 0x0009BD23
			public string payment_method { get; set; }

			// Token: 0x17000273 RID: 627
			// (get) Token: 0x06001DE1 RID: 7649 RVA: 0x0009DB2C File Offset: 0x0009BD2C
			// (set) Token: 0x06001DE2 RID: 7650 RVA: 0x0009DB34 File Offset: 0x0009BD34
			public string status { get; set; }
		}

		// Token: 0x0200049C RID: 1180
		public class StatusDetail
		{
			// Token: 0x17000274 RID: 628
			// (get) Token: 0x06001DE4 RID: 7652 RVA: 0x0009DB45 File Offset: 0x0009BD45
			// (set) Token: 0x06001DE5 RID: 7653 RVA: 0x0009DB4D File Offset: 0x0009BD4D
			public long date_next_charge { get; set; }

			// Token: 0x17000275 RID: 629
			// (get) Token: 0x06001DE6 RID: 7654 RVA: 0x0009DB56 File Offset: 0x0009BD56
			// (set) Token: 0x06001DE7 RID: 7655 RVA: 0x0009DB5E File Offset: 0x0009BD5E
			public IAPurchase.StatusDetailTransaction[] transactions { get; set; }

			// Token: 0x17000276 RID: 630
			// (get) Token: 0x06001DE8 RID: 7656 RVA: 0x0009DB67 File Offset: 0x0009BD67
			// (set) Token: 0x06001DE9 RID: 7657 RVA: 0x0009DB6F File Offset: 0x0009BD6F
			public string cancel_reason { get; set; }
		}

		// Token: 0x0200049D RID: 1181
		public class TimePeriod
		{
			// Token: 0x17000277 RID: 631
			// (get) Token: 0x06001DEB RID: 7659 RVA: 0x0009DB80 File Offset: 0x0009BD80
			// (set) Token: 0x06001DEC RID: 7660 RVA: 0x0009DB88 File Offset: 0x0009BD88
			public string time_type { get; set; }

			// Token: 0x17000278 RID: 632
			// (get) Token: 0x06001DED RID: 7661 RVA: 0x0009DB91 File Offset: 0x0009BD91
			// (set) Token: 0x06001DEE RID: 7662 RVA: 0x0009DB99 File Offset: 0x0009BD99
			public int value { get; set; }
		}

		// Token: 0x0200049E RID: 1182
		public class Subscription
		{
			// Token: 0x17000279 RID: 633
			// (get) Token: 0x06001DF0 RID: 7664 RVA: 0x0009DBAA File Offset: 0x0009BDAA
			// (set) Token: 0x06001DF1 RID: 7665 RVA: 0x0009DBB2 File Offset: 0x0009BDB2
			public string app_id { get; set; }

			// Token: 0x1700027A RID: 634
			// (get) Token: 0x06001DF2 RID: 7666 RVA: 0x0009DBBB File Offset: 0x0009BDBB
			// (set) Token: 0x06001DF3 RID: 7667 RVA: 0x0009DBC3 File Offset: 0x0009BDC3
			public string order_id { get; set; }

			// Token: 0x1700027B RID: 635
			// (get) Token: 0x06001DF4 RID: 7668 RVA: 0x0009DBCC File Offset: 0x0009BDCC
			// (set) Token: 0x06001DF5 RID: 7669 RVA: 0x0009DBD4 File Offset: 0x0009BDD4
			public string subscription_id { get; set; }

			// Token: 0x1700027C RID: 636
			// (get) Token: 0x06001DF6 RID: 7670 RVA: 0x0009DBDD File Offset: 0x0009BDDD
			// (set) Token: 0x06001DF7 RID: 7671 RVA: 0x0009DBE5 File Offset: 0x0009BDE5
			public string price { get; set; }

			// Token: 0x1700027D RID: 637
			// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x0009DBEE File Offset: 0x0009BDEE
			// (set) Token: 0x06001DF9 RID: 7673 RVA: 0x0009DBF6 File Offset: 0x0009BDF6
			public string currency { get; set; }

			// Token: 0x1700027E RID: 638
			// (get) Token: 0x06001DFA RID: 7674 RVA: 0x0009DBFF File Offset: 0x0009BDFF
			// (set) Token: 0x06001DFB RID: 7675 RVA: 0x0009DC07 File Offset: 0x0009BE07
			public long subscribed_timestamp { get; set; }

			// Token: 0x1700027F RID: 639
			// (get) Token: 0x06001DFC RID: 7676 RVA: 0x0009DC10 File Offset: 0x0009BE10
			// (set) Token: 0x06001DFD RID: 7677 RVA: 0x0009DC18 File Offset: 0x0009BE18
			public IAPurchase.TimePeriod free_trial_period { get; set; }

			// Token: 0x17000280 RID: 640
			// (get) Token: 0x06001DFE RID: 7678 RVA: 0x0009DC21 File Offset: 0x0009BE21
			// (set) Token: 0x06001DFF RID: 7679 RVA: 0x0009DC29 File Offset: 0x0009BE29
			public IAPurchase.TimePeriod charge_period { get; set; }

			// Token: 0x17000281 RID: 641
			// (get) Token: 0x06001E00 RID: 7680 RVA: 0x0009DC32 File Offset: 0x0009BE32
			// (set) Token: 0x06001E01 RID: 7681 RVA: 0x0009DC3A File Offset: 0x0009BE3A
			public int number_of_charge_period { get; set; }

			// Token: 0x17000282 RID: 642
			// (get) Token: 0x06001E02 RID: 7682 RVA: 0x0009DC43 File Offset: 0x0009BE43
			// (set) Token: 0x06001E03 RID: 7683 RVA: 0x0009DC4B File Offset: 0x0009BE4B
			public string plan_id { get; set; }

			// Token: 0x17000283 RID: 643
			// (get) Token: 0x06001E04 RID: 7684 RVA: 0x0009DC54 File Offset: 0x0009BE54
			// (set) Token: 0x06001E05 RID: 7685 RVA: 0x0009DC5C File Offset: 0x0009BE5C
			public string plan_name { get; set; }

			// Token: 0x17000284 RID: 644
			// (get) Token: 0x06001E06 RID: 7686 RVA: 0x0009DC65 File Offset: 0x0009BE65
			// (set) Token: 0x06001E07 RID: 7687 RVA: 0x0009DC6D File Offset: 0x0009BE6D
			public string status { get; set; }

			// Token: 0x17000285 RID: 645
			// (get) Token: 0x06001E08 RID: 7688 RVA: 0x0009DC76 File Offset: 0x0009BE76
			// (set) Token: 0x06001E09 RID: 7689 RVA: 0x0009DC7E File Offset: 0x0009BE7E
			public IAPurchase.StatusDetail status_detail { get; set; }
		}

		// Token: 0x0200049F RID: 1183
		public class QuerySubscritionResponse
		{
			// Token: 0x17000286 RID: 646
			// (get) Token: 0x06001E0B RID: 7691 RVA: 0x0009DC8F File Offset: 0x0009BE8F
			// (set) Token: 0x06001E0C RID: 7692 RVA: 0x0009DC97 File Offset: 0x0009BE97
			public int statusCode { get; set; }

			// Token: 0x17000287 RID: 647
			// (get) Token: 0x06001E0D RID: 7693 RVA: 0x0009DCA0 File Offset: 0x0009BEA0
			// (set) Token: 0x06001E0E RID: 7694 RVA: 0x0009DCA8 File Offset: 0x0009BEA8
			public string message { get; set; }

			// Token: 0x17000288 RID: 648
			// (get) Token: 0x06001E0F RID: 7695 RVA: 0x0009DCB1 File Offset: 0x0009BEB1
			// (set) Token: 0x06001E10 RID: 7696 RVA: 0x0009DCB9 File Offset: 0x0009BEB9
			public List<IAPurchase.Subscription> subscriptions { get; set; }
		}
	}
}
