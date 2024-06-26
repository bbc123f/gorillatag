﻿using System;
using UnityEngine;

namespace BoingKit
{
	public class MathUtil
	{
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		public static float InvSafe(float x)
		{
			return 1f / Mathf.Max(MathUtil.Epsilon, x);
		}

		public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
		{
			Vector2 vector = point - linePos;
			return (vector - Vector2.Dot(vector, lineDir) * lineDir).magnitude;
		}

		public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
		{
			Vector2 a = segmentPosB - segmentPosA;
			float num = 1f / a.magnitude;
			Vector2 rhs = a * num;
			float value = Vector2.Dot(point - segmentPosA, rhs) * num;
			return (segmentPosA + Mathf.Clamp(value, 0f, 1f) * a - point).magnitude;
		}

		public static float Seek(float current, float target, float maxDelta)
		{
			float num = target - current;
			num = Mathf.Sign(num) * Mathf.Min(maxDelta, Mathf.Abs(num));
			return current + num;
		}

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

		public static float Remainder(float a, float b)
		{
			return a - a / b * b;
		}

		public static int Remainder(int a, int b)
		{
			return a - a / b * b;
		}

		public static float Modulo(float a, float b)
		{
			return Mathf.Repeat(a, b);
		}

		public static int Modulo(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		public MathUtil()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MathUtil()
		{
		}

		public static readonly float Pi = 3.1415927f;

		public static readonly float TwoPi = 6.2831855f;

		public static readonly float HalfPi = 1.5707964f;

		public static readonly float QuaterPi = 0.7853982f;

		public static readonly float SixthPi = 0.5235988f;

		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		public static readonly float Epsilon = 1E-06f;

		public static readonly float Rad2Deg = 57.295776f;

		public static readonly float Deg2Rad = 0.017453292f;
	}
}
