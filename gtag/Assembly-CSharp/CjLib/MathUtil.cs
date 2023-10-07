using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000356 RID: 854
	public class MathUtil
	{
		// Token: 0x060018D4 RID: 6356 RVA: 0x00089798 File Offset: 0x00087998
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x000897AF File Offset: 0x000879AF
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x000897C8 File Offset: 0x000879C8
		public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}

		// Token: 0x0400199E RID: 6558
		public static readonly float Pi = 3.1415927f;

		// Token: 0x0400199F RID: 6559
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x040019A0 RID: 6560
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x040019A1 RID: 6561
		public static readonly float ThirdPi = 1.0471976f;

		// Token: 0x040019A2 RID: 6562
		public static readonly float QuarterPi = 0.7853982f;

		// Token: 0x040019A3 RID: 6563
		public static readonly float FifthPi = 0.62831855f;

		// Token: 0x040019A4 RID: 6564
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x040019A5 RID: 6565
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x040019A6 RID: 6566
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x040019A7 RID: 6567
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x040019A8 RID: 6568
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x040019A9 RID: 6569
		public static readonly float Epsilon = 1E-09f;

		// Token: 0x040019AA RID: 6570
		public static readonly float EpsilonComp = 1f - MathUtil.Epsilon;

		// Token: 0x040019AB RID: 6571
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x040019AC RID: 6572
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
