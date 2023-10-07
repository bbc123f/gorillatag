using System;
using System.Diagnostics;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020002FB RID: 763
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VectorLabelTextAttribute : PropertyAttribute
	{
		// Token: 0x06001575 RID: 5493 RVA: 0x00076C50 File Offset: 0x00074E50
		public VectorLabelTextAttribute(params string[] labels) : this(-1, labels)
		{
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x00076C5A File Offset: 0x00074E5A
		public VectorLabelTextAttribute(int width, params string[] labels)
		{
		}
	}
}
