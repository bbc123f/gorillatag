using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200037B RID: 891
	public class MathUtil
	{
		// Token: 0x06001A4C RID: 6732 RVA: 0x00092574 File Offset: 0x00090774
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x0009258B File Offset: 0x0009078B
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06001A4E RID: 6734 RVA: 0x000925A2 File Offset: 0x000907A2
		public static float InvSafe(float x)
		{
			return 1f / Mathf.Max(MathUtil.Epsilon, x);
		}

		// Token: 0x06001A4F RID: 6735 RVA: 0x000925B8 File Offset: 0x000907B8
		public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
		{
			Vector2 vector = point - linePos;
			return (vector - Vector2.Dot(vector, lineDir) * lineDir).magnitude;
		}

		// Token: 0x06001A50 RID: 6736 RVA: 0x000925E8 File Offset: 0x000907E8
		public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
		{
			Vector2 a = segmentPosB - segmentPosA;
			float num = 1f / a.magnitude;
			Vector2 rhs = a * num;
			float value = Vector2.Dot(point - segmentPosA, rhs) * num;
			return (segmentPosA + Mathf.Clamp(value, 0f, 1f) * a - point).magnitude;
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x00092650 File Offset: 0x00090850
		public static float Seek(float current, float target, float maxDelta)
		{
			float num = target - current;
			num = Mathf.Sign(num) * Mathf.Min(maxDelta, Mathf.Abs(num));
			return current + num;
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x00092678 File Offset: 0x00090878
		public static Vector2 Seek(Vector2 current, Vector2 target, float maxDelta)
		{
			Vector2 b = target - current;
			float magnitude = b.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return target;
			}
			b = Mathf.Min(maxDelta, magnitude) * b.normalized;
			return current + b;
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x000926BA File Offset: 0x000908BA
		public static float Remainder(float a, float b)
		{
			return a - a / b * b;
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x000926C3 File Offset: 0x000908C3
		public static int Remainder(int a, int b)
		{
			return a - a / b * b;
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x000926CC File Offset: 0x000908CC
		public static float Modulo(float a, float b)
		{
			return Mathf.Repeat(a, b);
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x000926D8 File Offset: 0x000908D8
		public static int Modulo(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		// Token: 0x04001AB3 RID: 6835
		public static readonly float Pi = 3.1415927f;

		// Token: 0x04001AB4 RID: 6836
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x04001AB5 RID: 6837
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x04001AB6 RID: 6838
		public static readonly float QuaterPi = 0.7853982f;

		// Token: 0x04001AB7 RID: 6839
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x04001AB8 RID: 6840
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x04001AB9 RID: 6841
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x04001ABA RID: 6842
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x04001ABB RID: 6843
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04001ABC RID: 6844
		public static readonly float Epsilon = 1E-06f;

		// Token: 0x04001ABD RID: 6845
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04001ABE RID: 6846
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
