using System;
using UnityEngine;

// Token: 0x0200022C RID: 556
public static class UnityEngineUtils
{
	// Token: 0x06000DD0 RID: 3536 RVA: 0x00050628 File Offset: 0x0004E828
	public static bool Approx(Vector3 a, Vector3 b)
	{
		bool flag = MathUtils.Approx(a.x, b.x, 1E-06f);
		bool flag2 = MathUtils.Approx(a.y, b.y, 1E-06f);
		bool flag3 = MathUtils.Approx(a.z, b.z, 1E-06f);
		return flag && flag2 && flag3;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x00050680 File Offset: 0x0004E880
	public static bool Approx(Vector3 a, Vector3 b, float epsilon)
	{
		bool flag = MathUtils.Approx(a.x, b.x, epsilon);
		bool flag2 = MathUtils.Approx(a.y, b.y, epsilon);
		bool flag3 = MathUtils.Approx(a.z, b.z, epsilon);
		return flag && flag2 && flag3;
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x000506CC File Offset: 0x0004E8CC
	public static Hash128 QuantizedHash(this Matrix4x4 m)
	{
		Hash128 result = default(Hash128);
		HashUtilities.QuantisedMatrixHash(ref m, ref result);
		return result;
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x000506EC File Offset: 0x0004E8EC
	public static Hash128 QuantizedHash(this Vector3 v)
	{
		Hash128 result = default(Hash128);
		HashUtilities.QuantisedVectorHash(ref v, ref result);
		return result;
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x0005070B File Offset: 0x0004E90B
	public static Vector4 ToVector(this Quaternion q)
	{
		return new Vector4(q.x, q.y, q.z, q.w);
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0005072A File Offset: 0x0004E92A
	public static void CopyTo(this Quaternion q, Vector4 v)
	{
		v.x = q.x;
		v.y = q.y;
		v.z = q.z;
		v.w = q.w;
	}
}
