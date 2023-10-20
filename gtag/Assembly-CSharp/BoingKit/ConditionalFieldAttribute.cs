using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200037C RID: 892
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06001A53 RID: 6739 RVA: 0x000929D3 File Offset: 0x00090BD3
		public bool ShowRange
		{
			get
			{
				return this.Min != this.Max;
			}
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x000929E8 File Offset: 0x00090BE8
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

		// Token: 0x04001AB5 RID: 6837
		public string PropertyToCheck;

		// Token: 0x04001AB6 RID: 6838
		public object CompareValue;

		// Token: 0x04001AB7 RID: 6839
		public object CompareValue2;

		// Token: 0x04001AB8 RID: 6840
		public object CompareValue3;

		// Token: 0x04001AB9 RID: 6841
		public object CompareValue4;

		// Token: 0x04001ABA RID: 6842
		public object CompareValue5;

		// Token: 0x04001ABB RID: 6843
		public object CompareValue6;

		// Token: 0x04001ABC RID: 6844
		public string Label;

		// Token: 0x04001ABD RID: 6845
		public string Tooltip;

		// Token: 0x04001ABE RID: 6846
		public float Min;

		// Token: 0x04001ABF RID: 6847
		public float Max;
	}
}
