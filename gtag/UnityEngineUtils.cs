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

	public static Hash128 QuantizedHash(this Matrix4x4 m)
	{
		Hash128 hash = default(Hash128);
		HashUtilities.QuantisedMatrixHash(ref m, ref hash);
		return hash;
	}

	public static Hash128 QuantizedHash(this Vector3 v)
	{
		Hash128 hash = default(Hash128);
		HashUtilities.QuantisedVectorHash(ref v, ref hash);
		return hash;
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
