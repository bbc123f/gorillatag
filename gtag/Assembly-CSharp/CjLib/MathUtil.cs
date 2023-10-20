using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000358 RID: 856
	public class MathUtil
	{
		// Token: 0x060018DD RID: 6365 RVA: 0x00089C80 File Offset: 0x00087E80
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x060018DE RID: 6366 RVA: 0x00089C97 File Offset: 0x00087E97
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x00089CB0 File Offset: 0x00087EB0
		public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}

		// Token: 0x040019AB RID: 6571
		public static readonly float Pi = 3.1415927f;

		// Token: 0x040019AC RID: 6572
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x040019AD RID: 6573
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x040019AE RID: 6574
		public static readonly float ThirdPi = 1.0471976f;

		// Token: 0x040019AF RID: 6575
		public static readonly float QuarterPi = 0.7853982f;

		// Token: 0x040019B0 RID: 6576
		public static readonly float FifthPi = 0.62831855f;

		// Token: 0x040019B1 RID: 6577
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x040019B2 RID: 6578
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x040019B3 RID: 6579
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x040019B4 RID: 6580
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x040019B5 RID: 6581
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x040019B6 RID: 6582
		public static readonly float Epsilon = 1E-09f;

		// Token: 0x040019B7 RID: 6583
		public static readonly float EpsilonComp = 1f - MathUtil.Epsilon;

		// Token: 0x040019B8 RID: 6584
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x040019B9 RID: 6585
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
