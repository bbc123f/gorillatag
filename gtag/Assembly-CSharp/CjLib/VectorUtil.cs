using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200035D RID: 861
	public class VectorUtil
	{
		// Token: 0x06001914 RID: 6420 RVA: 0x0008A704 File Offset: 0x00088904
		public static Vector3 Rotate2D(Vector3 v, float deg)
		{
			Vector3 result = v;
			float num = Mathf.Cos(MathUtil.Deg2Rad * deg);
			float num2 = Mathf.Sin(MathUtil.Deg2Rad * deg);
			result.x = num * v.x - num2 * v.y;
			result.y = num2 * v.x + num * v.y;
			return result;
		}

		// Token: 0x06001915 RID: 6421 RVA: 0x0008A75E File Offset: 0x0008895E
		public static Vector3 NormalizeSafe(Vector3 v, Vector3 fallback)
		{
			if (v.sqrMagnitude <= MathUtil.Epsilon)
			{
				return fallback;
			}
			return v.normalized;
		}

		// Token: 0x06001916 RID: 6422 RVA: 0x0008A778 File Offset: 0x00088978
		public static Vector3 FindOrthogonal(Vector3 v)
		{
			if (Mathf.Abs(v.x) >= MathUtil.Sqrt3Inv)
			{
				return Vector3.Normalize(new Vector3(v.y, -v.x, 0f));
			}
			return Vector3.Normalize(new Vector3(0f, v.z, -v.y));
		}

		// Token: 0x06001917 RID: 6423 RVA: 0x0008A7D0 File Offset: 0x000889D0
		public static void FormOrthogonalBasis(Vector3 v, out Vector3 a, out Vector3 b)
		{
			a = VectorUtil.FindOrthogonal(v);
			b = Vector3.Cross(a, v);
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x0008A7F0 File Offset: 0x000889F0
		public static Vector3 Integrate(Vector3 x, Vector3 v, float dt)
		{
			return x + v * dt;
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x0008A800 File Offset: 0x00088A00
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			float num = Vector3.Dot(a, b);
			if (num > 0.99999f)
			{
				return Vector3.Lerp(a, b, t);
			}
			if (num < -0.99999f)
			{
				Vector3 axis = VectorUtil.FindOrthogonal(a);
				return Quaternion.AngleAxis(180f * t, axis) * a;
			}
			float num2 = MathUtil.AcosSafe(num);
			return (Mathf.Sin((1f - t) * num2) * a + Mathf.Sin(t * num2) * b) / Mathf.Sin(num2);
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x0008A884 File Offset: 0x00088A84
		public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			float d = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * d + (-p0 + 3f * p1 - 3f * p2 + p3) * d * t);
		}
	}
}
