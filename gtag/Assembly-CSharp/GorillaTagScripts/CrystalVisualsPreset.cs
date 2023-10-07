using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x020002A8 RID: 680
	[CreateAssetMenu(fileName = "CrystalVisualsPreset", menuName = "ScriptableObjects/CrystalVisualsPreset", order = 0)]
	public class CrystalVisualsPreset : ScriptableObject
	{
		// Token: 0x060011B3 RID: 4531 RVA: 0x000649C8 File Offset: 0x00062BC8
		public override int GetHashCode()
		{
			return new ValueTuple<CrystalVisualsPreset.VisualState, CrystalVisualsPreset.VisualState>(this.stateA, this.stateB).GetHashCode();
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x000649F4 File Offset: 0x00062BF4
		[Conditional("UNITY_EDITOR")]
		private void Save()
		{
		}

		// Token: 0x04001469 RID: 5225
		public CrystalVisualsPreset.VisualState stateA;

		// Token: 0x0400146A RID: 5226
		public CrystalVisualsPreset.VisualState stateB;

		// Token: 0x020004AF RID: 1199
		[Serializable]
		public struct VisualState
		{
			// Token: 0x06001E4C RID: 7756 RVA: 0x0009E158 File Offset: 0x0009C358
			public override int GetHashCode()
			{
				int item = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.albedo);
				int item2 = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.emission);
				return new ValueTuple<int, int>(item, item2).GetHashCode();
			}

			// Token: 0x06001E4D RID: 7757 RVA: 0x0009E190 File Offset: 0x0009C390
			[CompilerGenerated]
			internal static int <GetHashCode>g__GetColorHash|2_0(Color c)
			{
				return new ValueTuple<float, float, float>(c.r, c.g, c.b).GetHashCode();
			}

			// Token: 0x04001F6B RID: 8043
			[ColorUsage(false, false)]
			public Color albedo;

			// Token: 0x04001F6C RID: 8044
			[ColorUsage(false, false)]
			public Color emission;
		}
	}
}
