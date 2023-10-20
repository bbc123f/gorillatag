using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000219 RID: 537
public static class MathUtils
{
	// Token: 0x06000D54 RID: 3412 RVA: 0x0004E0D8 File Offset: 0x0004C2D8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(float a, float b, float epsilon = 1E-06f)
	{
		double num = (double)a;
		double num2 = (double)b;
		double num3 = Math.Max(Math.Abs(num), Math.Abs(num2));
		double num4 = Math.Max((double)epsilon * num3, MathUtils.UnityEpsilon * 8.0);
		return Math.Abs(num - num2) < num4;
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x0004E120 File Offset: 0x0004C320
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

	// Token: 0x06000D56 RID: 3414 RVA: 0x0004E14B File Offset: 0x0004C34B
	public static float LinearUnclamped(float value, float min, float max, float newMin, float newMax)
	{
		return (value - min) / (max - min) * (newMax - newMin) + newMin;
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x0004E15C File Offset: 0x0004C35C
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

	// Token: 0x06000D58 RID: 3416 RVA: 0x0004E1B8 File Offset: 0x0004C3B8
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

	// Token: 0x06000D59 RID: 3417 RVA: 0x0004E21C File Offset: 0x0004C41C
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

	// Token: 0x0400107F RID: 4223
	public static readonly double UnityEpsilon = (double)Mathf.Epsilon;
}
