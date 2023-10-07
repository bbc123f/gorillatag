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
	// Token: 0x0200024D RID: 589
	public class IAPurchase
	{
		// Token: 0x06000E86 RID: 3718 RVA: 0x00052F8F File Offset: 0x0005118F
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x00052F9D File Offset: 0x0005119D
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

		// Token: 0x06000E88 RID: 3720 RVA: 0x00052FDC File Offset: 0x000511DC
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06000E89 RID: 3721 RVA: 0x00052FEA File Offset: 0x000511EA
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

		// Token: 0x06000E8A RID: 3722 RVA: 0x00053029 File Offset: 0x00051229
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x00053038 File Offset: 0x00051238
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

		// Token: 0x06000E8C RID: 3724 RVA: 0x00053084 File Offset: 0x00051284
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void PurchaseIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.purchaseIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x00053092 File Offset: 0x00051292
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

		// Token: 0x06000E8E RID: 3726 RVA: 0x000530D1 File Offset: 0x000512D1
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x000530DF File Offset: 0x000512DF
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

		// Token: 0x06000E90 RID: 3728 RVA: 0x0005311E File Offset: 0x0005131E
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x0005312C File Offset: 0x0005132C
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

		// Token: 0x06000E92 RID: 3730 RVA: 0x00053169 File Offset: 0x00051369
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void GetBalanceIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.getBalanceIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x00053177 File Offset: 0x00051377
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

		// Token: 0x06000E94 RID: 3732 RVA: 0x000531B4 File Offset: 0x000513B4
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x000531C4 File Offset: 0x000513C4
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

		// Token: 0x06000E96 RID: 3734 RVA: 0x00053222 File Offset: 0x00051422
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionWithPlanIDIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x00053230 File Offset: 0x00051430
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

		// Token: 0x06000E98 RID: 3736 RVA: 0x0005326F File Offset: 0x0005146F
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void SubscribeIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.subscribeIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x0005327D File Offset: 0x0005147D
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

		// Token: 0x06000E9A RID: 3738 RVA: 0x000532BC File Offset: 0x000514BC
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E9B RID: 3739 RVA: 0x000532CA File Offset: 0x000514CA
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

		// Token: 0x06000E9C RID: 3740 RVA: 0x00053309 File Offset: 0x00051509
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionListIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionListIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00053317 File Offset: 0x00051517
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

		// Token: 0x06000E9E RID: 3742 RVA: 0x00053354 File Offset: 0x00051554
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void CancelSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x00053362 File Offset: 0x00051562
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

		// Token: 0x0400117D RID: 4477
		private static IAPurchaseCallback isReadyIl2cppCallback;

		// Token: 0x0400117E RID: 4478
		private static IAPurchaseCallback request01Il2cppCallback;

		// Token: 0x0400117F RID: 4479
		private static IAPurchaseCallback request02Il2cppCallback;

		// Token: 0x04001180 RID: 4480
		private static IAPurchaseCallback purchaseIl2cppCallback;

		// Token: 0x04001181 RID: 4481
		private static IAPurchaseCallback query01Il2cppCallback;

		// Token: 0x04001182 RID: 4482
		private static IAPurchaseCallback query02Il2cppCallback;

		// Token: 0x04001183 RID: 4483
		private static IAPurchaseCallback getBalanceIl2cppCallback;

		// Token: 0x04001184 RID: 4484
		private static IAPurchaseCallback requestSubscriptionIl2cppCallback;

		// Token: 0x04001185 RID: 4485
		private static IAPurchaseCallback requestSubscriptionWithPlanIDIl2cppCallback;

		// Token: 0x04001186 RID: 4486
		private static IAPurchaseCallback subscribeIl2cppCallback;

		// Token: 0x04001187 RID: 4487
		private static IAPurchaseCallback querySubscriptionIl2cppCallback;

		// Token: 0x04001188 RID: 4488
		private static IAPurchaseCallback querySubscriptionListIl2cppCallback;

		// Token: 0x04001189 RID: 4489
		private static IAPurchaseCallback cancelSubscriptionIl2cppCallback;

		// Token: 0x02000493 RID: 1171
		private class IAPHandler : IAPurchase.BaseHandler
		{
			// Token: 0x06001D7D RID: 7549 RVA: 0x0009C3E6 File Offset: 0x0009A5E6
			public IAPHandler(IAPurchase.IAPurchaseListener cb)
			{
				IAPurchase.IAPHandler.listener = cb;
			}

			// Token: 0x06001D7E RID: 7550 RVA: 0x0009C3F4 File Offset: 0x0009A5F4
			public IAPurchaseCallback getIsReadyHandler()
			{
				return new IAPurchaseCallback(this.IsReadyHandler);
			}

			// Token: 0x06001D7F RID: 7551 RVA: 0x0009C404 File Offset: 0x0009A604
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

			// Token: 0x06001D80 RID: 7552 RVA: 0x0009C534 File Offset: 0x0009A734
			public IAPurchaseCallback getRequestHandler()
			{
				return new IAPurchaseCallback(this.RequestHandler);
			}

			// Token: 0x06001D81 RID: 7553 RVA: 0x0009C544 File Offset: 0x0009A744
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

			// Token: 0x06001D82 RID: 7554 RVA: 0x0009C674 File Offset: 0x0009A874
			public IAPurchaseCallback getPurchaseHandler()
			{
				return new IAPurchaseCallback(this.PurchaseHandler);
			}

			// Token: 0x06001D83 RID: 7555 RVA: 0x0009C684 File Offset: 0x0009A884
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

			// Token: 0x06001D84 RID: 7556 RVA: 0x0009C7D4 File Offset: 0x0009A9D4
			public IAPurchaseCallback getQueryHandler()
			{
				return new IAPurchaseCallback(this.QueryHandler);
			}

			// Token: 0x06001D85 RID: 7557 RVA: 0x0009C7E4 File Offset: 0x0009A9E4
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

			// Token: 0x06001D86 RID: 7558 RVA: 0x0009CA30 File Offset: 0x0009AC30
			public IAPurchaseCallback getQueryListHandler()
			{
				return new IAPurchaseCallback(this.QueryListHandler);
			}

			// Token: 0x06001D87 RID: 7559 RVA: 0x0009CA40 File Offset: 0x0009AC40
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

			// Token: 0x06001D88 RID: 7560 RVA: 0x0009CDC8 File Offset: 0x0009AFC8
			public IAPurchaseCallback getBalanceHandler()
			{
				return new IAPurchaseCallback(this.BalanceHandler);
			}

			// Token: 0x06001D89 RID: 7561 RVA: 0x0009CDD8 File Offset: 0x0009AFD8
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

			// Token: 0x06001D8A RID: 7562 RVA: 0x0009CF34 File Offset: 0x0009B134
			public IAPurchaseCallback getRequestSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionHandler);
			}

			// Token: 0x06001D8B RID: 7563 RVA: 0x0009CF44 File Offset: 0x0009B144
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

			// Token: 0x06001D8C RID: 7564 RVA: 0x0009D06C File Offset: 0x0009B26C
			public IAPurchaseCallback getRequestSubscriptionWithPlanIDHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionWithPlanIDHandler);
			}

			// Token: 0x06001D8D RID: 7565 RVA: 0x0009D07C File Offset: 0x0009B27C
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

			// Token: 0x06001D8E RID: 7566 RVA: 0x0009D1A4 File Offset: 0x0009B3A4
			public IAPurchaseCallback getSubscribeHandler()
			{
				return new IAPurchaseCallback(this.SubscribeHandler);
			}

			// Token: 0x06001D8F RID: 7567 RVA: 0x0009D1B4 File Offset: 0x0009B3B4
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

			// Token: 0x06001D90 RID: 7568 RVA: 0x0009D33C File Offset: 0x0009B53C
			public IAPurchaseCallback getQuerySubscriptionHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionHandler);
			}

			// Token: 0x06001D91 RID: 7569 RVA: 0x0009D34C File Offset: 0x0009B54C
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

			// Token: 0x06001D92 RID: 7570 RVA: 0x0009D474 File Offset: 0x0009B674
			public IAPurchaseCallback getQuerySubscriptionListHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionListHandler);
			}

			// Token: 0x06001D93 RID: 7571 RVA: 0x0009D484 File Offset: 0x0009B684
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

			// Token: 0x06001D94 RID: 7572 RVA: 0x0009D5AC File Offset: 0x0009B7AC
			public IAPurchaseCallback getCancelSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.CancelSubscriptionHandler);
			}

			// Token: 0x06001D95 RID: 7573 RVA: 0x0009D5BC File Offset: 0x0009B7BC
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

			// Token: 0x04001F06 RID: 7942
			private static IAPurchase.IAPurchaseListener listener;
		}

		// Token: 0x02000494 RID: 1172
		private abstract class BaseHandler
		{
			// Token: 0x06001D96 RID: 7574
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D97 RID: 7575
			protected abstract void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D98 RID: 7576
			protected abstract void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D99 RID: 7577
			protected abstract void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D9A RID: 7578
			protected abstract void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D9B RID: 7579
			protected abstract void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D9C RID: 7580
			protected abstract void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D9D RID: 7581
			protected abstract void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D9E RID: 7582
			protected abstract void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001D9F RID: 7583
			protected abstract void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA0 RID: 7584
			protected abstract void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06001DA1 RID: 7585
			protected abstract void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000495 RID: 1173
		public class IAPurchaseListener
		{
			// Token: 0x06001DA3 RID: 7587 RVA: 0x0009D6B4 File Offset: 0x0009B8B4
			public virtual void OnSuccess(string pchCurrencyName)
			{
			}

			// Token: 0x06001DA4 RID: 7588 RVA: 0x0009D6B6 File Offset: 0x0009B8B6
			public virtual void OnRequestSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x06001DA5 RID: 7589 RVA: 0x0009D6B8 File Offset: 0x0009B8B8
			public virtual void OnPurchaseSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x06001DA6 RID: 7590 RVA: 0x0009D6BA File Offset: 0x0009B8BA
			public virtual void OnQuerySuccess(IAPurchase.QueryResponse response)
			{
			}

			// Token: 0x06001DA7 RID: 7591 RVA: 0x0009D6BC File Offset: 0x0009B8BC
			public virtual void OnQuerySuccess(IAPurchase.QueryListResponse response)
			{
			}

			// Token: 0x06001DA8 RID: 7592 RVA: 0x0009D6BE File Offset: 0x0009B8BE
			public virtual void OnBalanceSuccess(string pchBalance)
			{
			}

			// Token: 0x06001DA9 RID: 7593 RVA: 0x0009D6C0 File Offset: 0x0009B8C0
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}

			// Token: 0x06001DAA RID: 7594 RVA: 0x0009D6C2 File Offset: 0x0009B8C2
			public virtual void OnRequestSubscriptionSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06001DAB RID: 7595 RVA: 0x0009D6C4 File Offset: 0x0009B8C4
			public virtual void OnRequestSubscriptionWithPlanIDSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06001DAC RID: 7596 RVA: 0x0009D6C6 File Offset: 0x0009B8C6
			public virtual void OnSubscribeSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06001DAD RID: 7597 RVA: 0x0009D6C8 File Offset: 0x0009B8C8
			public virtual void OnQuerySubscriptionSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06001DAE RID: 7598 RVA: 0x0009D6CA File Offset: 0x0009B8CA
			public virtual void OnQuerySubscriptionListSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06001DAF RID: 7599 RVA: 0x0009D6CC File Offset: 0x0009B8CC
			public virtual void OnCancelSubscriptionSuccess(bool bCanceled)
			{
			}
		}

		// Token: 0x02000496 RID: 1174
		public class QueryResponse
		{
			// Token: 0x1700025F RID: 607
			// (get) Token: 0x06001DB1 RID: 7601 RVA: 0x0009D6D6 File Offset: 0x0009B8D6
			// (set) Token: 0x06001DB2 RID: 7602 RVA: 0x0009D6DE File Offset: 0x0009B8DE
			public string order_id { get; set; }

			// Token: 0x17000260 RID: 608
			// (get) Token: 0x06001DB3 RID: 7603 RVA: 0x0009D6E7 File Offset: 0x0009B8E7
			// (set) Token: 0x06001DB4 RID: 7604 RVA: 0x0009D6EF File Offset: 0x0009B8EF
			public string purchase_id { get; set; }

			// Token: 0x17000261 RID: 609
			// (get) Token: 0x06001DB5 RID: 7605 RVA: 0x0009D6F8 File Offset: 0x0009B8F8
			// (set) Token: 0x06001DB6 RID: 7606 RVA: 0x0009D700 File Offset: 0x0009B900
			public string status { get; set; }

			// Token: 0x17000262 RID: 610
			// (get) Token: 0x06001DB7 RID: 7607 RVA: 0x0009D709 File Offset: 0x0009B909
			// (set) Token: 0x06001DB8 RID: 7608 RVA: 0x0009D711 File Offset: 0x0009B911
			public string price { get; set; }

			// Token: 0x17000263 RID: 611
			// (get) Token: 0x06001DB9 RID: 7609 RVA: 0x0009D71A File Offset: 0x0009B91A
			// (set) Token: 0x06001DBA RID: 7610 RVA: 0x0009D722 File Offset: 0x0009B922
			public string currency { get; set; }

			// Token: 0x17000264 RID: 612
			// (get) Token: 0x06001DBB RID: 7611 RVA: 0x0009D72B File Offset: 0x0009B92B
			// (set) Token: 0x06001DBC RID: 7612 RVA: 0x0009D733 File Offset: 0x0009B933
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000497 RID: 1175
		public class QueryResponse2
		{
			// Token: 0x17000265 RID: 613
			// (get) Token: 0x06001DBE RID: 7614 RVA: 0x0009D744 File Offset: 0x0009B944
			// (set) Token: 0x06001DBF RID: 7615 RVA: 0x0009D74C File Offset: 0x0009B94C
			public string order_id { get; set; }

			// Token: 0x17000266 RID: 614
			// (get) Token: 0x06001DC0 RID: 7616 RVA: 0x0009D755 File Offset: 0x0009B955
			// (set) Token: 0x06001DC1 RID: 7617 RVA: 0x0009D75D File Offset: 0x0009B95D
			public string app_id { get; set; }

			// Token: 0x17000267 RID: 615
			// (get) Token: 0x06001DC2 RID: 7618 RVA: 0x0009D766 File Offset: 0x0009B966
			// (set) Token: 0x06001DC3 RID: 7619 RVA: 0x0009D76E File Offset: 0x0009B96E
			public string purchase_id { get; set; }

			// Token: 0x17000268 RID: 616
			// (get) Token: 0x06001DC4 RID: 7620 RVA: 0x0009D777 File Offset: 0x0009B977
			// (set) Token: 0x06001DC5 RID: 7621 RVA: 0x0009D77F File Offset: 0x0009B97F
			public string user_data { get; set; }

			// Token: 0x17000269 RID: 617
			// (get) Token: 0x06001DC6 RID: 7622 RVA: 0x0009D788 File Offset: 0x0009B988
			// (set) Token: 0x06001DC7 RID: 7623 RVA: 0x0009D790 File Offset: 0x0009B990
			public string price { get; set; }

			// Token: 0x1700026A RID: 618
			// (get) Token: 0x06001DC8 RID: 7624 RVA: 0x0009D799 File Offset: 0x0009B999
			// (set) Token: 0x06001DC9 RID: 7625 RVA: 0x0009D7A1 File Offset: 0x0009B9A1
			public string currency { get; set; }

			// Token: 0x1700026B RID: 619
			// (get) Token: 0x06001DCA RID: 7626 RVA: 0x0009D7AA File Offset: 0x0009B9AA
			// (set) Token: 0x06001DCB RID: 7627 RVA: 0x0009D7B2 File Offset: 0x0009B9B2
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000498 RID: 1176
		public class QueryListResponse
		{
			// Token: 0x1700026C RID: 620
			// (get) Token: 0x06001DCD RID: 7629 RVA: 0x0009D7C3 File Offset: 0x0009B9C3
			// (set) Token: 0x06001DCE RID: 7630 RVA: 0x0009D7CB File Offset: 0x0009B9CB
			public int total { get; set; }

			// Token: 0x1700026D RID: 621
			// (get) Token: 0x06001DCF RID: 7631 RVA: 0x0009D7D4 File Offset: 0x0009B9D4
			// (set) Token: 0x06001DD0 RID: 7632 RVA: 0x0009D7DC File Offset: 0x0009B9DC
			public int from { get; set; }

			// Token: 0x1700026E RID: 622
			// (get) Token: 0x06001DD1 RID: 7633 RVA: 0x0009D7E5 File Offset: 0x0009B9E5
			// (set) Token: 0x06001DD2 RID: 7634 RVA: 0x0009D7ED File Offset: 0x0009B9ED
			public int to { get; set; }

			// Token: 0x04001F17 RID: 7959
			public List<IAPurchase.QueryResponse2> purchaseList;
		}

		// Token: 0x02000499 RID: 1177
		public class StatusDetailTransaction
		{
			// Token: 0x1700026F RID: 623
			// (get) Token: 0x06001DD4 RID: 7636 RVA: 0x0009D7FE File Offset: 0x0009B9FE
			// (set) Token: 0x06001DD5 RID: 7637 RVA: 0x0009D806 File Offset: 0x0009BA06
			public long create_time { get; set; }

			// Token: 0x17000270 RID: 624
			// (get) Token: 0x06001DD6 RID: 7638 RVA: 0x0009D80F File Offset: 0x0009BA0F
			// (set) Token: 0x06001DD7 RID: 7639 RVA: 0x0009D817 File Offset: 0x0009BA17
			public string payment_method { get; set; }

			// Token: 0x17000271 RID: 625
			// (get) Token: 0x06001DD8 RID: 7640 RVA: 0x0009D820 File Offset: 0x0009BA20
			// (set) Token: 0x06001DD9 RID: 7641 RVA: 0x0009D828 File Offset: 0x0009BA28
			public string status { get; set; }
		}

		// Token: 0x0200049A RID: 1178
		public class StatusDetail
		{
			// Token: 0x17000272 RID: 626
			// (get) Token: 0x06001DDB RID: 7643 RVA: 0x0009D839 File Offset: 0x0009BA39
			// (set) Token: 0x06001DDC RID: 7644 RVA: 0x0009D841 File Offset: 0x0009BA41
			public long date_next_charge { get; set; }

			// Token: 0x17000273 RID: 627
			// (get) Token: 0x06001DDD RID: 7645 RVA: 0x0009D84A File Offset: 0x0009BA4A
			// (set) Token: 0x06001DDE RID: 7646 RVA: 0x0009D852 File Offset: 0x0009BA52
			public IAPurchase.StatusDetailTransaction[] transactions { get; set; }

			// Token: 0x17000274 RID: 628
			// (get) Token: 0x06001DDF RID: 7647 RVA: 0x0009D85B File Offset: 0x0009BA5B
			// (set) Token: 0x06001DE0 RID: 7648 RVA: 0x0009D863 File Offset: 0x0009BA63
			public string cancel_reason { get; set; }
		}

		// Token: 0x0200049B RID: 1179
		public class TimePeriod
		{
			// Token: 0x17000275 RID: 629
			// (get) Token: 0x06001DE2 RID: 7650 RVA: 0x0009D874 File Offset: 0x0009BA74
			// (set) Token: 0x06001DE3 RID: 7651 RVA: 0x0009D87C File Offset: 0x0009BA7C
			public string time_type { get; set; }

			// Token: 0x17000276 RID: 630
			// (get) Token: 0x06001DE4 RID: 7652 RVA: 0x0009D885 File Offset: 0x0009BA85
			// (set) Token: 0x06001DE5 RID: 7653 RVA: 0x0009D88D File Offset: 0x0009BA8D
			public int value { get; set; }
		}

		// Token: 0x0200049C RID: 1180
		public class Subscription
		{
			// Token: 0x17000277 RID: 631
			// (get) Token: 0x06001DE7 RID: 7655 RVA: 0x0009D89E File Offset: 0x0009BA9E
			// (set) Token: 0x06001DE8 RID: 7656 RVA: 0x0009D8A6 File Offset: 0x0009BAA6
			public string app_id { get; set; }

			// Token: 0x17000278 RID: 632
			// (get) Token: 0x06001DE9 RID: 7657 RVA: 0x0009D8AF File Offset: 0x0009BAAF
			// (set) Token: 0x06001DEA RID: 7658 RVA: 0x0009D8B7 File Offset: 0x0009BAB7
			public string order_id { get; set; }

			// Token: 0x17000279 RID: 633
			// (get) Token: 0x06001DEB RID: 7659 RVA: 0x0009D8C0 File Offset: 0x0009BAC0
			// (set) Token: 0x06001DEC RID: 7660 RVA: 0x0009D8C8 File Offset: 0x0009BAC8
			public string subscription_id { get; set; }

			// Token: 0x1700027A RID: 634
			// (get) Token: 0x06001DED RID: 7661 RVA: 0x0009D8D1 File Offset: 0x0009BAD1
			// (set) Token: 0x06001DEE RID: 7662 RVA: 0x0009D8D9 File Offset: 0x0009BAD9
			public string price { get; set; }

			// Token: 0x1700027B RID: 635
			// (get) Token: 0x06001DEF RID: 7663 RVA: 0x0009D8E2 File Offset: 0x0009BAE2
			// (set) Token: 0x06001DF0 RID: 7664 RVA: 0x0009D8EA File Offset: 0x0009BAEA
			public string currency { get; set; }

			// Token: 0x1700027C RID: 636
			// (get) Token: 0x06001DF1 RID: 7665 RVA: 0x0009D8F3 File Offset: 0x0009BAF3
			// (set) Token: 0x06001DF2 RID: 7666 RVA: 0x0009D8FB File Offset: 0x0009BAFB
			public long subscribed_timestamp { get; set; }

			// Token: 0x1700027D RID: 637
			// (get) Token: 0x06001DF3 RID: 7667 RVA: 0x0009D904 File Offset: 0x0009BB04
			// (set) Token: 0x06001DF4 RID: 7668 RVA: 0x0009D90C File Offset: 0x0009BB0C
			public IAPurchase.TimePeriod free_trial_period { get; set; }

			// Token: 0x1700027E RID: 638
			// (get) Token: 0x06001DF5 RID: 7669 RVA: 0x0009D915 File Offset: 0x0009BB15
			// (set) Token: 0x06001DF6 RID: 7670 RVA: 0x0009D91D File Offset: 0x0009BB1D
			public IAPurchase.TimePeriod charge_period { get; set; }

			// Token: 0x1700027F RID: 639
			// (get) Token: 0x06001DF7 RID: 7671 RVA: 0x0009D926 File Offset: 0x0009BB26
			// (set) Token: 0x06001DF8 RID: 7672 RVA: 0x0009D92E File Offset: 0x0009BB2E
			public int number_of_charge_period { get; set; }

			// Token: 0x17000280 RID: 640
			// (get) Token: 0x06001DF9 RID: 7673 RVA: 0x0009D937 File Offset: 0x0009BB37
			// (set) Token: 0x06001DFA RID: 7674 RVA: 0x0009D93F File Offset: 0x0009BB3F
			public string plan_id { get; set; }

			// Token: 0x17000281 RID: 641
			// (get) Token: 0x06001DFB RID: 7675 RVA: 0x0009D948 File Offset: 0x0009BB48
			// (set) Token: 0x06001DFC RID: 7676 RVA: 0x0009D950 File Offset: 0x0009BB50
			public string plan_name { get; set; }

			// Token: 0x17000282 RID: 642
			// (get) Token: 0x06001DFD RID: 7677 RVA: 0x0009D959 File Offset: 0x0009BB59
			// (set) Token: 0x06001DFE RID: 7678 RVA: 0x0009D961 File Offset: 0x0009BB61
			public string status { get; set; }

			// Token: 0x17000283 RID: 643
			// (get) Token: 0x06001DFF RID: 7679 RVA: 0x0009D96A File Offset: 0x0009BB6A
			// (set) Token: 0x06001E00 RID: 7680 RVA: 0x0009D972 File Offset: 0x0009BB72
			public IAPurchase.StatusDetail status_detail { get; set; }
		}

		// Token: 0x0200049D RID: 1181
		public class QuerySubscritionResponse
		{
			// Token: 0x17000284 RID: 644
			// (get) Token: 0x06001E02 RID: 7682 RVA: 0x0009D983 File Offset: 0x0009BB83
			// (set) Token: 0x06001E03 RID: 7683 RVA: 0x0009D98B File Offset: 0x0009BB8B
			public int statusCode { get; set; }

			// Token: 0x17000285 RID: 645
			// (get) Token: 0x06001E04 RID: 7684 RVA: 0x0009D994 File Offset: 0x0009BB94
			// (set) Token: 0x06001E05 RID: 7685 RVA: 0x0009D99C File Offset: 0x0009BB9C
			public string message { get; set; }

			// Token: 0x17000286 RID: 646
			// (get) Token: 0x06001E06 RID: 7686 RVA: 0x0009D9A5 File Offset: 0x0009BBA5
			// (set) Token: 0x06001E07 RID: 7687 RVA: 0x0009D9AD File Offset: 0x0009BBAD
			public List<IAPurchase.Subscription> subscriptions { get; set; }
		}
	}
}
