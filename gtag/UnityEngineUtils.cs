using System;
using UnityEngine;

public static class UnityEngineUtils
{
	public static bool Approx(Vector3 a, Vector3 b)
	{
		bool flag = MathUtils.Approx(a.x, b.x, 1E-06f);
		bool flag2 = MathUtils.Approx(a.y, b.y, 1E-06f);
		bool flag3 = MathUtils.Approx(a.z, b.z, 1E-06f);
		return flag && flag2 && flag3;
	}

	public static bool Approx(Vector3 a, Vector3 b, float epsilon)
	{
		bool flag = MathUtils.Approx(a.x, b.x, epsilon);
		bool flag2 = MathUtils.Approx(a.y, b.y, epsilon);
		bool flag3 = MathUtils.Approx(a.z, b.z, epsilon);
		return flag && flag2 && flag3;
	}

	public static Hash128 QuantizedHash128(this Matrix4x4 m)
	{
		Hash128 hash = default(Hash128);
		HashUtilities.QuantisedMatrixHash(ref m, ref hash);
		return hash;
	}

	public static Hash128 QuantizedHash128(this Vector3 v)
	{
		Hash128 hash = default(Hash128);
		HashUtilities.QuantisedVectorHash(ref v, ref hash);
		return hash;
	}

	public static Id128 QuantizedId128(this Vector3 v)
	{
		return v.QuantizedHash128();
	}

	public static Id128 QuantizedId128(this Matrix4x4 m)
	{
		return m.QuantizedHash128();
	}

	public static Id128 QuantizedId128(this Quaternion q)
	{
		int num = (int)((double)q[0] * 1000.0 + 0.5);
		int num2 = (int)((double)q[1] * 1000.0 + 0.5);
		int num3 = (int)((double)q[2] * 1000.0 + 0.5);
		int num4 = (int)((double)q[3] * 1000.0 + 0.5);
		return new Id128(num, num2, num3, num4);
	}

	public static Vector4 ToVector(this Quaternion q)
	{
		return new Vector4(q.x, q.y, q.z, q.w);
	}

	public static void CopyTo(this Quaternion q, Vector4 v)
	{
		v.x = q.x;
		v.y = q.y;
		v.z = q.z;
		v.w = q.w;
	}
}
