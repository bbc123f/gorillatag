using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200037A RID: 890
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06001A4A RID: 6730 RVA: 0x000924EB File Offset: 0x000906EB
		public bool ShowRange
		{
			get
			{
				return this.Min != this.Max;
			}
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x00092500 File Offset: 0x00090700
		public ConditionalFieldAttribute(string propertyToCheck = null, object compareValue = null, object compareValue2 = null, object compareValue3 = null, object compareValue4 = null, object compareValue5 = null, object compareValue6 = null)
		{
			this.PropertyToCheck = propertyToCheck;
			this.CompareValue = compareValue;
			this.CompareValue2 = compareValue2;
			this.CompareValue3 = compareValue3;
			this.CompareValue4 = compareValue4;
			this.CompareValue5 = compareValue5;
			this.CompareValue6 = compareValue6;
			this.Label = "";
			this.Tooltip = "";
			this.Min = 0f;
			this.Max = 0f;
		}

		// Token: 0x04001AA8 RID: 6824
		public string PropertyToCheck;

		// Token: 0x04001AA9 RID: 6825
		public object CompareValue;

		// Token: 0x04001AAA RID: 6826
		public object CompareValue2;

		// Token: 0x04001AAB RID: 6827
		public object CompareValue3;

		// Token: 0x04001AAC RID: 6828
		public object CompareValue4;

		// Token: 0x04001AAD RID: 6829
		public object CompareValue5;

		// Token: 0x04001AAE RID: 6830
		public object CompareValue6;

		// Token: 0x04001AAF RID: 6831
		public string Label;

		// Token: 0x04001AB0 RID: 6832
		public string Tooltip;

		// Token: 0x04001AB1 RID: 6833
		public float Min;

		// Token: 0x04001AB2 RID: 6834
		public float Max;
	}
}
