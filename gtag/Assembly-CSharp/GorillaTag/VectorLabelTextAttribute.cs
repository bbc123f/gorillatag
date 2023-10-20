using System;
using System.Diagnostics;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020002FD RID: 765
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VectorLabelTextAttribute : PropertyAttribute
	{
		// Token: 0x0600157E RID: 5502 RVA: 0x00077138 File Offset: 0x00075338
		public VectorLabelTextAttribute(params string[] labels) : this(-1, labels)
		{
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x00077142 File Offset: 0x00075342
		public VectorLabelTextAttribute(int width, params string[] labels)
		{
		}
	}
}
