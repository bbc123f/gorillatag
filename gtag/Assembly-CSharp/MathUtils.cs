using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000218 RID: 536
public static class MathUtils
{
	// Token: 0x06000D4E RID: 3406 RVA: 0x0004DE78 File Offset: 0x0004C078
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(float a, float b, float epsilon = 1E-06f)
	{
		double num = (double)a;
		double num2 = (double)b;
		double num3 = Math.Max(Math.Abs(num), Math.Abs(num2));
		double num4 = Math.Max((double)epsilon * num3, MathUtils.UnityEpsilon * 8.0);
		return Math.Abs(num - num2) < num4;
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x0004DEC0 File Offset: 0x0004C0C0
	public static float Linear(float value, float min, float max, float newMin, float newMax)
	{
		float num = (value - min) / (max - min) * (newMax - newMin) + newMin;
		if (num < newMin)
		{
			return newMin;
		}
		if (num > newMax)
		{
			return newMax;
		}
		return num;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x0004DEEB File Offset: 0x0004C0EB
	public static float LinearUnclamped(float value, float min, float max, float newMin, float newMax)
	{
		return (value - min) / (max - min) * (newMax - newMin) + newMin;
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x0004DEFC File Offset: 0x0004C0FC
	public static float GetCircleValue(float degrees)
	{
		if (degrees > 90f)
		{
			degrees -= 180f;
		}
		else if (degrees < -90f)
		{
			degrees += 180f;
		}
		if (degrees > 180f)
		{
			degrees -= 270f;
		}
		else if (degrees < -180f)
		{
			degrees += 270f;
		}
		return degrees / 90f;
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x0004DF58 File Offset: 0x0004C158
	public static Vector3 WeightedMaxVector(Vector3 a, Vector3 b, float eps = 0.0001f)
	{
		float magnitude = a.magnitude;
		float magnitude2 = b.magnitude;
		if (magnitude < eps || magnitude2 < eps)
		{
			return Vector3.zero;
		}
		a / magnitude;
		b / magnitude2;
		Vector3 a2 = a * (magnitude / (magnitude + magnitude2)) + b * (magnitude2 / (magnitude + magnitude2));
		float d = Mathf.Max(magnitude, magnitude2);
		return a2 * d;
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x0004DFBC File Offset: 0x0004C1BC
	public static Vector3 MatchMagnitudeInDirection(Vector3 input, Vector3 target, float eps = 0.0001f)
	{
		Vector3 result = input;
		float magnitude = target.magnitude;
		if (magnitude > eps)
		{
			Vector3 vector = target / magnitude;
			float num = Vector3.Dot(input, vector);
			float num2 = magnitude - num;
			if (num2 > 0f)
			{
				result = input + num2 * vector;
			}
		}
		return result;
	}

	// Token: 0x0400107A RID: 4218
	public static readonly double UnityEpsilon = (double)Mathf.Epsilon;
}
