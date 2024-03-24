using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class MathUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this Vector3 a, Vector3 b, float epsilon = 1E-05f)
	{
		return (a - b).sqrMagnitude <= epsilon * epsilon;
	}

	public static bool Approx(this Quaternion a, Quaternion b, float epsilon = 1E-06f)
	{
		return Quaternion.Dot(a, b) > 1f - epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool OrientedBoxContains(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxAngles)
	{
		Vector3 vector = Matrix4x4.TRS(boxCenter, boxAngles, Vector3.one).inverse.MultiplyPoint3x4(point);
		Vector3 vector2 = boxSize * 0.5f;
		vector.x = Mathf.Abs(vector.x);
		vector.y = Mathf.Abs(vector.y);
		vector.z = Mathf.Abs(vector.z);
		return (Mathf.Approximately(vector.x, vector2.x) && Mathf.Approximately(vector.y, vector2.y) && Mathf.Approximately(vector.z, vector2.z)) || (vector.x < vector2.x && vector.y < vector2.y && vector.z < vector2.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int OrientedBoxSphereOverlap(Vector3 center, float radius, Vector3 boxCenter, Vector3 boxSize, Quaternion boxAngles)
	{
		Matrix4x4 inverse = Matrix4x4.TRS(boxCenter, boxAngles, Vector3.one).inverse;
		Vector3 vector = boxSize * 0.5f;
		Vector3 vector2 = inverse.MultiplyPoint3x4(center);
		Vector3 vector3 = Vector3.right * radius;
		float magnitude = inverse.MultiplyVector(vector3).magnitude;
		Vector3 vector4 = -vector;
		Vector3 vector5 = MathUtils.ClampVectorValues(vector2, vector4, vector);
		if ((vector2 - vector5).sqrMagnitude > magnitude * magnitude)
		{
			return -1;
		}
		if (vector4.x + magnitude <= vector2.x && vector2.x <= vector.x - magnitude && vector.x - vector4.x > magnitude && vector4.y + magnitude <= vector2.y && vector2.y <= vector.y - magnitude && vector.y - vector4.y > magnitude && vector4.z + magnitude <= vector2.z && vector2.z <= vector.z - magnitude && vector.z - vector4.z > magnitude)
		{
			return 1;
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 ClampVectorValues(Vector3 v, Vector3 min, Vector3 max)
	{
		float num = v.x;
		num = ((num > max.x) ? max.x : num);
		num = ((num < min.x) ? min.x : num);
		float num2 = v.y;
		num2 = ((num2 > max.y) ? max.y : num2);
		num2 = ((num2 < min.y) ? min.y : num2);
		float num3 = v.z;
		num3 = ((num3 > max.z) ? max.z : num3);
		num3 = ((num3 < min.z) ? min.z : num3);
		return new Vector3(num, num2, num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Validate(ref Vector3 v)
	{
		float x = v.x;
		float y = v.y;
		float z = v.z;
		bool flag = float.IsNaN(x) || float.IsInfinity(x);
		bool flag2 = float.IsNaN(y) || float.IsInfinity(y);
		bool flag3 = float.IsNaN(z) || float.IsInfinity(z);
		if (flag)
		{
			v.x = 0f;
		}
		if (flag2)
		{
			v.y = 0f;
		}
		if (flag3)
		{
			v.z = 0f;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Validate(ref Quaternion q)
	{
		float x = q.x;
		float y = q.y;
		float z = q.z;
		float w = q.w;
		bool flag = float.IsNaN(x) || float.IsInfinity(x);
		bool flag2 = float.IsNaN(y) || float.IsInfinity(y);
		bool flag3 = float.IsNaN(z) || float.IsInfinity(z);
		bool flag4 = float.IsNaN(w) || float.IsInfinity(w);
		if (flag)
		{
			q.x = 0f;
		}
		if (flag2)
		{
			q.y = 0f;
		}
		if (flag3)
		{
			q.z = 0f;
		}
		if (flag4)
		{
			q.w = 1f;
		}
	}

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

	// Note: this type is marked as 'beforefieldinit'.
	static MathUtils()
	{
	}

	public static readonly double UnityEpsilon = (double)Mathf.Epsilon;
}
