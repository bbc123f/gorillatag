using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200037B RID: 891
	public class Collision
	{
		// Token: 0x06001A4C RID: 6732 RVA: 0x00092584 File Offset: 0x00090784
		public static bool SphereSphere(Vector3 centerA, float radiusA, Vector3 centerB, float radiusB, out Vector3 push)
		{
			push = Vector3.zero;
			Vector3 v = centerA - centerB;
			float sqrMagnitude = v.sqrMagnitude;
			float num = radiusA + radiusB;
			if (sqrMagnitude >= num * num)
			{
				return false;
			}
			float num2 = Mathf.Sqrt(sqrMagnitude);
			push = VectorUtil.NormalizeSafe(v, Vector3.zero) * (num - num2);
			return true;
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x000925EC File Offset: 0x000907EC
		public static bool SphereSphereInverse(Vector3 centerA, float radiusA, Vector3 centerB, float radiusB, out Vector3 push)
		{
			push = Vector3.zero;
			Vector3 v = centerB - centerA;
			float sqrMagnitude = v.sqrMagnitude;
			float num = radiusB - radiusA;
			if (sqrMagnitude <= num * num)
			{
				return false;
			}
			float num2 = Mathf.Sqrt(sqrMagnitude);
			push = VectorUtil.NormalizeSafe(v, Vector3.zero) * (num2 - num);
			return true;
		}

		// Token: 0x06001A4E RID: 6734 RVA: 0x00092654 File Offset: 0x00090854
		public static bool SphereCapsule(Vector3 centerA, float radiusA, Vector3 headB, Vector3 tailB, float radiusB, out Vector3 push)
		{
			push = Vector3.zero;
			Vector3 a = tailB - headB;
			float sqrMagnitude = a.sqrMagnitude;
			if (sqrMagnitude < MathUtil.Epsilon)
			{
				return Collision.SphereSphereInverse(centerA, radiusA, 0.5f * (headB + tailB), radiusB, out push);
			}
			float num = 1f / Mathf.Sqrt(sqrMagnitude);
			Vector3 rhs = a * num;
			float t = Mathf.Clamp01(Vector3.Dot(centerA - headB, rhs) * num);
			Vector3 centerB = Vector3.Lerp(headB, tailB, t);
			return Collision.SphereSphere(centerA, radiusA, centerB, radiusB, out push);
		}

		// Token: 0x06001A4F RID: 6735 RVA: 0x000926E8 File Offset: 0x000908E8
		public static bool SphereCapsuleInverse(Vector3 centerA, float radiusA, Vector3 headB, Vector3 tailB, float radiusB, out Vector3 push)
		{
			push = Vector3.zero;
			Vector3 a = tailB - headB;
			float sqrMagnitude = a.sqrMagnitude;
			if (sqrMagnitude < MathUtil.Epsilon)
			{
				return Collision.SphereSphereInverse(centerA, radiusA, 0.5f * (headB + tailB), radiusB, out push);
			}
			float num = 1f / Mathf.Sqrt(sqrMagnitude);
			Vector3 rhs = a * num;
			float t = Mathf.Clamp01(Vector3.Dot(centerA - headB, rhs) * num);
			Vector3 centerB = Vector3.Lerp(headB, tailB, t);
			return Collision.SphereSphereInverse(centerA, radiusA, centerB, radiusB, out push);
		}

		// Token: 0x06001A50 RID: 6736 RVA: 0x0009277C File Offset: 0x0009097C
		public static bool SphereBox(Vector3 centerOffsetA, float radiusA, Vector3 halfExtentB, out Vector3 push)
		{
			push = Vector3.zero;
			Vector3 b = new Vector3(Mathf.Clamp(centerOffsetA.x, -halfExtentB.x, halfExtentB.x), Mathf.Clamp(centerOffsetA.y, -halfExtentB.y, halfExtentB.y), Mathf.Clamp(centerOffsetA.z, -halfExtentB.z, halfExtentB.z));
			Vector3 v = centerOffsetA - b;
			float sqrMagnitude = v.sqrMagnitude;
			if (sqrMagnitude > radiusA * radiusA)
			{
				return false;
			}
			int num = ((centerOffsetA.x < -halfExtentB.x || centerOffsetA.x > halfExtentB.x) ? 0 : 1) + ((centerOffsetA.y < -halfExtentB.y || centerOffsetA.y > halfExtentB.y) ? 0 : 1) + ((centerOffsetA.z < -halfExtentB.z || centerOffsetA.z > halfExtentB.z) ? 0 : 1);
			if (num > 2)
			{
				if (num == 3)
				{
					Vector3 vector = new Vector3(halfExtentB.x - Mathf.Abs(centerOffsetA.x) + radiusA, halfExtentB.y - Mathf.Abs(centerOffsetA.y) + radiusA, halfExtentB.z - Mathf.Abs(centerOffsetA.z) + radiusA);
					if (vector.x < vector.y)
					{
						if (vector.x < vector.z)
						{
							push = new Vector3(Mathf.Sign(centerOffsetA.x) * vector.x, 0f, 0f);
						}
						else
						{
							push = new Vector3(0f, 0f, Mathf.Sign(centerOffsetA.z) * vector.z);
						}
					}
					else if (vector.y < vector.z)
					{
						push = new Vector3(0f, Mathf.Sign(centerOffsetA.y) * vector.y, 0f);
					}
					else
					{
						push = new Vector3(0f, 0f, Mathf.Sign(centerOffsetA.z) * vector.z);
					}
				}
			}
			else
			{
				push = VectorUtil.NormalizeSafe(v, Vector3.right) * (radiusA - Mathf.Sqrt(sqrMagnitude));
			}
			return true;
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x000929BD File Offset: 0x00090BBD
		public static bool SphereBoxInverse(Vector3 centerOffsetA, float radiusA, Vector3 halfExtentB, out Vector3 push)
		{
			push = Vector3.zero;
			return false;
		}
	}
}
