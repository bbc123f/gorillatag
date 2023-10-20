using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200037D RID: 893
	public class MathUtil
	{
		// Token: 0x06001A55 RID: 6741 RVA: 0x00092A5C File Offset: 0x00090C5C
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x00092A73 File Offset: 0x00090C73
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x00092A8A File Offset: 0x00090C8A
		public static float InvSafe(float x)
		{
			return 1f / Mathf.Max(MathUtil.Epsilon, x);
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x00092AA0 File Offset: 0x00090CA0
		public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
		{
			Vector2 vector = point - linePos;
			return (vector - Vector2.Dot(vector, lineDir) * lineDir).magnitude;
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x00092AD0 File Offset: 0x00090CD0
		public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
		{
			Vector2 a = segmentPosB - segmentPosA;
			float num = 1f / a.magnitude;
			Vector2 rhs = a * num;
			float value = Vector2.Dot(point - segmentPosA, rhs) * num;
			return (segmentPosA + Mathf.Clamp(value, 0f, 1f) * a - point).magnitude;
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x00092B38 File Offset: 0x00090D38
		public static float Seek(float current, float target, float maxDelta)
		{
			float num = target - current;
			num = Mathf.Sign(num) * Mathf.Min(maxDelta, Mathf.Abs(num));
			return current + num;
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x00092B60 File Offset: 0x00090D60
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

		// Token: 0x06001A5C RID: 6748 RVA: 0x00092BA2 File Offset: 0x00090DA2
		public static float Remainder(float a, float b)
		{
			return a - a / b * b;
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x00092BAB File Offset: 0x00090DAB
		public static int Remainder(int a, int b)
		{
			return a - a / b * b;
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x00092BB4 File Offset: 0x00090DB4
		public static float Modulo(float a, float b)
		{
			return Mathf.Repeat(a, b);
		}

		// Token: 0x06001A5F RID: 6751 RVA: 0x00092BC0 File Offset: 0x00090DC0
		public static int Modulo(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		// Token: 0x04001AC0 RID: 6848
		public static readonly float Pi = 3.1415927f;

		// Token: 0x04001AC1 RID: 6849
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x04001AC2 RID: 6850
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x04001AC3 RID: 6851
		public static readonly float QuaterPi = 0.7853982f;

		// Token: 0x04001AC4 RID: 6852
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x04001AC5 RID: 6853
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x04001AC6 RID: 6854
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x04001AC7 RID: 6855
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x04001AC8 RID: 6856
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04001AC9 RID: 6857
		public static readonly float Epsilon = 1E-06f;

		// Token: 0x04001ACA RID: 6858
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04001ACB RID: 6859
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
