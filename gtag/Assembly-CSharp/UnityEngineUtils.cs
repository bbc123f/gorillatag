using System;
using UnityEngine;

// Token: 0x0200022D RID: 557
public static class UnityEngineUtils
{
	// Token: 0x06000DD6 RID: 3542 RVA: 0x00050888 File Offset: 0x0004EA88
	public static bool Approx(Vector3 a, Vector3 b)
	{
		bool flag = MathUtils.Approx(a.x, b.x, 1E-06f);
		bool flag2 = MathUtils.Approx(a.y, b.y, 1E-06f);
		bool flag3 = MathUtils.Approx(a.z, b.z, 1E-06f);
		return flag && flag2 && flag3;
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x000508E0 File Offset: 0x0004EAE0
	public static bool Approx(Vector3 a, Vector3 b, float epsilon)
	{
		bool flag = MathUtils.Approx(a.x, b.x, epsilon);
		bool flag2 = MathUtils.Approx(a.y, b.y, epsilon);
		bool flag3 = MathUtils.Approx(a.z, b.z, epsilon);
		return flag && flag2 && flag3;
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x0005092C File Offset: 0x0004EB2C
	public static Hash128 QuantizedHash(this Matrix4x4 m)
	{
		Hash128 result = default(Hash128);
		HashUtilities.QuantisedMatrixHash(ref m, ref result);
		return result;
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x0005094C File Offset: 0x0004EB4C
	public static Hash128 QuantizedHash(this Vector3 v)
	{
		Hash128 result = default(Hash128);
		HashUtilities.QuantisedVectorHash(ref v, ref result);
		return result;
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0005096B File Offset: 0x0004EB6B
	public static Vector4 ToVector(this Quaternion q)
	{
		return new Vector4(q.x, q.y, q.z, q.w);
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x0005098A File Offset: 0x0004EB8A
	public static void CopyTo(this Quaternion q, Vector4 v)
	{
		v.x = q.x;
		v.y = q.y;
		v.z = q.z;
		v.w = q.w;
	}
}
