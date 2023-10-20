using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x020002AA RID: 682
	[CreateAssetMenu(fileName = "CrystalVisualsPreset", menuName = "ScriptableObjects/CrystalVisualsPreset", order = 0)]
	public class CrystalVisualsPreset : ScriptableObject
	{
		// Token: 0x060011BA RID: 4538 RVA: 0x00064E30 File Offset: 0x00063030
		public override int GetHashCode()
		{
			return new ValueTuple<CrystalVisualsPreset.VisualState, CrystalVisualsPreset.VisualState>(this.stateA, this.stateB).GetHashCode();
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x00064E5C File Offset: 0x0006305C
		[Conditional("UNITY_EDITOR")]
		private void Save()
		{
		}

		// Token: 0x04001476 RID: 5238
		public CrystalVisualsPreset.VisualState stateA;

		// Token: 0x04001477 RID: 5239
		public CrystalVisualsPreset.VisualState stateB;

		// Token: 0x020004B1 RID: 1201
		[Serializable]
		public struct VisualState
		{
			// Token: 0x06001E55 RID: 7765 RVA: 0x0009E464 File Offset: 0x0009C664
			public override int GetHashCode()
			{
				int item = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.albedo);
				int item2 = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.emission);
				return new ValueTuple<int, int>(item, item2).GetHashCode();
			}

			// Token: 0x06001E56 RID: 7766 RVA: 0x0009E49C File Offset: 0x0009C69C
			[CompilerGenerated]
			internal static int <GetHashCode>g__GetColorHash|2_0(Color c)
			{
				return new ValueTuple<float, float, float>(c.r, c.g, c.b).GetHashCode();
			}

			// Token: 0x04001F78 RID: 8056
			[ColorUsage(false, false)]
			public Color albedo;

			// Token: 0x04001F79 RID: 8057
			[ColorUsage(false, false)]
			public Color emission;
		}
	}
}
