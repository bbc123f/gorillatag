using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Viveport
{
	public class SubscriptionStatus
	{
		public List<SubscriptionStatus.Platform> Platforms
		{
			[CompilerGenerated]
			get
			{
				return this.<Platforms>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Platforms>k__BackingField = value;
			}
		}

		public SubscriptionStatus.TransactionType Type
		{
			[CompilerGenerated]
			get
			{
				return this.<Type>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Type>k__BackingField = value;
			}
		}

		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		[CompilerGenerated]
		private List<SubscriptionStatus.Platform> <Platforms>k__BackingField;

		[CompilerGenerated]
		private SubscriptionStatus.TransactionType <Type>k__BackingField;

		public enum Platform
		{
			Windows,
			Android
		}

		public enum TransactionType
		{
			Unknown,
			Paid,
			Redeem,
			FreeTrial
		}
	}
}
