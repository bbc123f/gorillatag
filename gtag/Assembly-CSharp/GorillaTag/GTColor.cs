using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000304 RID: 772
	public static class GTColor
	{
		// Token: 0x06001597 RID: 5527 RVA: 0x000779F8 File Offset: 0x00075BF8
		public static Color RandomHSV(GTColor.HSVRanges ranges)
		{
			return Color.HSVToRGB(Random.Range(ranges.h.x, ranges.h.y), Random.Range(ranges.s.x, ranges.s.y), Random.Range(ranges.v.x, ranges.v.y));
		}

		// Token: 0x020004FD RID: 1277
		[Serializable]
		public struct HSVRanges
		{
			// Token: 0x06001F4D RID: 8013 RVA: 0x000A1F46 File Offset: 0x000A0146
			public HSVRanges(float hMin = 0f, float hMax = 1f, float sMin = 0f, float sMax = 1f, float vMin = 0f, float vMax = 1f)
			{
				this.h = new Vector2(hMin, hMax);
				this.s = new Vector2(sMin, sMax);
				this.v = new Vector2(vMin, vMax);
			}

			// Token: 0x040020D1 RID: 8401
			public Vector2 h;

			// Token: 0x040020D2 RID: 8402
			public Vector2 s;

			// Token: 0x040020D3 RID: 8403
			public Vector2 v;
		}
	}
}
