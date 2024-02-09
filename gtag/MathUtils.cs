using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class MathUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(float a, float b, float epsilon = 1E-06f)
	{
		double num = (double)a;
		double num2 = (double)b;
		double num3 = Math.Max(Math.Abs(num), Math.Abs(num2));
		double num4 = Math.Max((double)epsilon * num3, MathUtils.UnityEpsilon * 8.0);
		return Math.Abs(num - num2) < num4;
	}

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

	public static float LinearUnclamped(float value, float min, float max, float newMin, float newMax)
	{
		return (value - min) / (max - min) * (newMax - newMin) + newMin;
	}

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
		Vector3 vector = a * (magnitude / (magnitude + magnitude2)) + b * (magnitude2 / (magnitude + magnitude2));
		float num = Mathf.Max(magnitude, magnitude2);
		return vector * num;
	}

	public static Vector3 MatchMagnitudeInDirection(Vector3 input, Vector3 target, float eps = 0.0001f)
	{
		Vector3 vector = input;
		float magnitude = target.magnitude;
		if (magnitude > eps)
		{
			Vector3 vector2 = target / magnitude;
			float num = Vector3.Dot(input, vector2);
			float num2 = magnitude - num;
			if (num2 > 0f)
			{
				vector = input + num2 * vector2;
			}
		}
		return vector;
	}

	public static readonly double UnityEpsilon = (double)Mathf.Epsilon;
}
