using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000250 RID: 592
	public class Subscription
	{
		// Token: 0x06000EAE RID: 3758 RVA: 0x00053871 File Offset: 0x00051A71
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			Subscription.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x00053880 File Offset: 0x00051A80
		public static void IsReady(StatusCallback2 callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Subscription.isReadyIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Subscription.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Subscription.IsReady_64(new StatusCallback2(Subscription.IsReadyIl2cppCallback));
				return;
			}
			Subscription.IsReady(new StatusCallback2(Subscription.IsReadyIl2cppCallback));
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x000538F0 File Offset: 0x00051AF0
		public static SubscriptionStatus GetUserStatus()
		{
			SubscriptionStatus subscriptionStatus = new SubscriptionStatus();
			if (IntPtr.Size == 8)
			{
				if (Subscription.IsWindowsSubscriber_64())
				{
					subscriptionStatus.Platforms.Add(SubscriptionStatus.Platform.Windows);
				}
				if (Subscription.IsAndroidSubscriber_64())
				{
					subscriptionStatus.Platforms.Add(SubscriptionStatus.Platform.Android);
				}
				switch (Subscription.GetTransactionType_64())
				{
				case ESubscriptionTransactionType.UNKNOWN:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Unknown;
					break;
				case ESubscriptionTransactionType.PAID:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Paid;
					break;
				case ESubscriptionTransactionType.REDEEM:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Redeem;
					break;
				case ESubscriptionTransactionType.FREEE_TRIAL:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.FreeTrial;
					break;
				default:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Unknown;
					break;
				}
			}
			else
			{
				if (Subscription.IsWindowsSubscriber())
				{
					subscriptionStatus.Platforms.Add(SubscriptionStatus.Platform.Windows);
				}
				if (Subscription.IsAndroidSubscriber())
				{
					subscriptionStatus.Platforms.Add(SubscriptionStatus.Platform.Android);
				}
				switch (Subscription.GetTransactionType())
				{
				case ESubscriptionTransactionType.UNKNOWN:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Unknown;
					break;
				case ESubscriptionTransactionType.PAID:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Paid;
					break;
				case ESubscriptionTransactionType.REDEEM:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Redeem;
					break;
				case ESubscriptionTransactionType.FREEE_TRIAL:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.FreeTrial;
					break;
				default:
					subscriptionStatus.Type = SubscriptionStatus.TransactionType.Unknown;
					break;
				}
			}
			return subscriptionStatus;
		}

		// Token: 0x04001192 RID: 4498
		private static StatusCallback2 isReadyIl2cppCallback;
	}
}
