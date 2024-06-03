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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this Quaternion a, Quaternion b, float epsilon = 1E-06f)
	{
		return Quaternion.Dot(a, b) > 1f - epsilon;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3[] OrientedBoxCorners(Vector3 center, Vector3 size, Quaternion angles)
	{
		Vector3 b = angles * new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 b2 = angles * new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 b3 = angles * new Vector3(0f, 0f, size.z * 0.5f);
		return new Vector3[]
		{
			center + b + b2 + b3,
			center + b + b2 - b3,
			center - b + b2 - b3,
			center - b + b2 + b3,
			center + b - b2 + b3,
			center + b - b2 - b3,
			center - b - b2 - b3,
			center - b - b2 + b3
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void OrientedBoxCornersNonAlloc(Vector3 center, Vector3 size, Quaternion angles, Vector3[] array, int index = 0)
	{
		Vector3 b = angles * new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 b2 = angles * new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 b3 = angles * new Vector3(0f, 0f, size.z * 0.5f);
		array[index] = center + b + b2 + b3;
		array[index + 1] = center + b + b2 - b3;
		array[index + 2] = center - b + b2 - b3;
		array[index + 3] = center - b + b2 + b3;
		array[index + 4] = center + b - b2 + b3;
		array[index + 5] = center + b - b2 - b3;
		array[index + 6] = center - b - b2 - b3;
		array[index + 7] = center - b - b2 + b3;
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
		Vector3 b = MathUtils.ClampVectorValues(vector2, vector4, vector);
		if ((vector2 - b).sqrMagnitude > magnitude * magnitude)
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
	public static Bounds[] Partition(Bounds b, int x = 1, int y = 1, int z = 1)
	{
		if (x < 1)
		{
			x = 1;
		}
		if (y < 1)
		{
			y = 1;
		}
		if (z < 1)
		{
			z = 1;
		}
		Vector3 size = b.size;
		float num = size.x / (float)x;
		float num2 = size.y / (float)y;
		float num3 = size.z / (float)z;
		Vector3 size2 = new Vector3(num, num2, num3);
		Bounds[] array = new Bounds[x * y * z];
		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				for (int k = 0; k < z; k++)
				{
					float num4 = (float)i * num;
					float num5 = (float)j * num2;
					float num6 = (float)k * num3;
					float num7 = (float)(i + 1) * num;
					float num8 = (float)(j + 1) * num2;
					float num9 = (float)(k + 1) * num3;
					Vector3 center;
					center.x = (num4 + num7) * 0.5f - size.x * 0.5f;
					center.y = (num5 + num8) * 0.5f - size.y * 0.5f;
					center.z = (num6 + num9) * 0.5f - size.z * 0.5f;
					int num10 = k * x * y + j * x + i;
					array[num10] = new Bounds(center, size2);
				}
			}
		}
		return array;
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
	public static void ValidateTo(ref Quaternion q, Quaternion validForm)
	{
		float x = validForm.x;
		float y = validForm.y;
		float z = validForm.z;
		float w = validForm.w;
		q.x = MathUtils.ClampToReal(q.x, x, y, 1E-06f);
		q.x = MathUtils.ClampToReal(q.y, y, y, 1E-06f);
		q.y = MathUtils.ClampToReal(q.z, z, y, 1E-06f);
		q.z = MathUtils.ClampToReal(q.w, w, y, 1E-06f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ClampToReal(float f, float min, float max, float epsilon = 1E-06f)
	{
		if (float.IsNaN(f))
		{
			f = 0f;
		}
		if (float.IsNegativeInfinity(min))
		{
			min = float.MinValue;
		}
		if (float.IsPositiveInfinity(max))
		{
			max = float.MaxValue;
		}
		return MathUtils.ClampApprox(f, min, max, epsilon);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ClampApprox(float f, float min, float max, float epsilon = 1E-06f)
	{
		if (f < min || MathUtils.Approx(f, min, epsilon))
		{
			return min;
		}
		if (f > max || MathUtils.Approx(f, max, epsilon))
		{
			return max;
		}
		return f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(float a, float b, float epsilon = 1E-06f)
	{
		return Math.Abs(a - b) < epsilon;
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
		Vector3 a2 = a * (magnitude / (magnitude + magnitude2)) + b * (magnitude2 / (magnitude + magnitude2));
		float d = Mathf.Max(magnitude, magnitude2);
		return a2 * d;
	}

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
}
