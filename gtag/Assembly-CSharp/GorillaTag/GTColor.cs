using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000302 RID: 770
	public static class GTColor
	{
		// Token: 0x0600158E RID: 5518 RVA: 0x00077510 File Offset: 0x00075710
		public static Color RandomHSV(GTColor.HSVRanges ranges)
		{
			return Color.HSVToRGB(Random.Range(ranges.h.x, ranges.h.y), Random.Range(ranges.s.x, ranges.s.y), Random.Range(ranges.v.x, ranges.v.y));
		}

		// Token: 0x020004FB RID: 1275
		[Serializable]
		public struct HSVRanges
		{
			// Token: 0x06001F44 RID: 8004 RVA: 0x000A1C3A File Offset: 0x0009FE3A
			public HSVRanges(float hMin = 0f, float hMax = 1f, float sMin = 0f, float sMax = 1f, float vMin = 0f, float vMax = 1f)
			{
				this.h = new Vector2(hMin, hMax);
				this.s = new Vector2(sMin, sMax);
				this.v = new Vector2(vMin, vMax);
			}

			// Token: 0x040020C4 RID: 8388
			public Vector2 h;

			// Token: 0x040020C5 RID: 8389
			public Vector2 s;

			// Token: 0x040020C6 RID: 8390
			public Vector2 v;
		}
	}
}
