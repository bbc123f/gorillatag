using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class CatmullRomSpline : MonoBehaviour
{
	// Token: 0x06000D95 RID: 3477 RVA: 0x0004F570 File Offset: 0x0004D770
	private void RefreshControlPoints()
	{
		this.controlPoints.Clear();
		this.controlPointsTransformationMatricies.Clear();
		for (int i = 0; i < this.controlPointTransforms.Length; i++)
		{
			this.controlPoints.Add(this.controlPointTransforms[i].position);
			this.controlPointsTransformationMatricies.Add(this.controlPointTransforms[i].localToWorldMatrix);
		}
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x0004F5D6 File Offset: 0x0004D7D6
	private void Awake()
	{
		this.RefreshControlPoints();
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x0004F5E0 File Offset: 0x0004D7E0
	public static Vector3 Evaluate(List<Vector3> controlPoints, float t)
	{
		if (controlPoints.Count < 1)
		{
			return Vector3.zero;
		}
		if (controlPoints.Count < 2)
		{
			return controlPoints[0];
		}
		if (controlPoints.Count < 3)
		{
			return Vector3.Lerp(controlPoints[0], controlPoints[1], t);
		}
		if (controlPoints.Count >= 4)
		{
			float num = t * (float)(controlPoints.Count - 3);
			int num2 = Mathf.FloorToInt(num);
			float t2 = num - (float)num2;
			int num3 = num2;
			if (num3 >= controlPoints.Count - 3)
			{
				num3 = controlPoints.Count - 4;
				t2 = 1f;
			}
			return CatmullRomSpline.CatmullRom(t2, controlPoints[num3], controlPoints[num3 + 1], controlPoints[num3 + 2], controlPoints[num3 + 3]);
		}
		if (t < 0.5f)
		{
			return Vector3.Lerp(controlPoints[0], controlPoints[1], t * 2f);
		}
		return Vector3.Lerp(controlPoints[1], controlPoints[2], (t - 0.5f) * 2f);
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x0004F6D2 File Offset: 0x0004D8D2
	public Vector3 Evaluate(float t)
	{
		return CatmullRomSpline.Evaluate(this.controlPoints, t);
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0004F6E0 File Offset: 0x0004D8E0
	public static float GetClosestEvaluationOnSpline(List<Vector3> controlPoints, Vector3 worldPoint, out Vector3 linePoint)
	{
		float num = float.MaxValue;
		float num2 = 0f;
		int num3 = 0;
		linePoint = worldPoint;
		for (int i = 1; i < controlPoints.Count - 2; i++)
		{
			Vector3 vector = controlPoints[i];
			Vector3 vector2 = controlPoints[i + 1];
			Vector3 a = vector2 - vector;
			float magnitude = a.magnitude;
			if ((double)magnitude > 1E-05)
			{
				Vector3 vector3 = a / magnitude;
				float num4 = Vector3.Dot(worldPoint - vector, vector3);
				float sqrMagnitude;
				float num5;
				Vector3 vector4;
				if (num4 <= 0f)
				{
					sqrMagnitude = (worldPoint - vector).sqrMagnitude;
					num5 = 0f;
					vector4 = vector;
				}
				else if (num4 >= magnitude)
				{
					sqrMagnitude = (worldPoint - vector2).sqrMagnitude;
					num5 = 1f;
					vector4 = vector2;
				}
				else
				{
					sqrMagnitude = (worldPoint - (vector + vector3 * num4)).sqrMagnitude;
					num5 = num4 / magnitude;
					vector4 = vector + vector3 * num4;
				}
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					num2 = num5;
					num3 = i;
					linePoint = vector4;
				}
			}
		}
		return Mathf.Clamp01(((float)(num3 - 1) + num2) / (float)(controlPoints.Count - 3));
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x0004F823 File Offset: 0x0004DA23
	public float GetClosestEvaluationOnSpline(Vector3 worldPoint, out Vector3 linePoint)
	{
		return CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPoints, worldPoint, out linePoint);
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x0004F834 File Offset: 0x0004DA34
	public static Vector3 GetForwardTangent(List<Vector3> controlPoints, float t, float step = 0.01f)
	{
		t = Mathf.Clamp(t, 0f, 1f - step - Mathf.Epsilon);
		Vector3 b = CatmullRomSpline.Evaluate(controlPoints, t);
		return (CatmullRomSpline.Evaluate(controlPoints, t + step) - b).normalized;
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x0004F87C File Offset: 0x0004DA7C
	public Vector3 GetForwardTangent(float t, float step = 0.01f)
	{
		t = Mathf.Clamp(t, 0f, 1f - step - Mathf.Epsilon);
		Vector3 b = this.Evaluate(t);
		return (this.Evaluate(t + step) - b).normalized;
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x0004F8C4 File Offset: 0x0004DAC4
	private static Vector3 CatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 a = 2f * p1;
		Vector3 a2 = p2 - p0;
		Vector3 a3 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 a4 = -p0 + 3f * p1 - 3f * p2 + p3;
		return 0.5f * (a + a2 * t + a3 * t * t + a4 * t * t * t);
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0004F988 File Offset: 0x0004DB88
	private void OnDrawGizmosSelected()
	{
		if (this.testFloat > 0f)
		{
			Vector3 vector = this.Evaluate(this.testFloat);
			Matrix4x4 matrix4x = CatmullRomSpline.Evaluate(this.controlPointsTransformationMatricies, this.testFloat);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.up * 0.2f);
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.forward * 0.2f);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.right * 0.2f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(vector, 0.01f);
			Gizmos.DrawRay(vector, this.GetForwardTangent(this.testFloat, 0.01f));
		}
		this.RefreshControlPoints();
		Gizmos.color = Color.yellow;
		int num = 128;
		Vector3 from = this.Evaluate(0f);
		for (int i = 1; i <= num; i++)
		{
			float t = (float)i / (float)num;
			Vector3 vector2 = this.Evaluate(t);
			Gizmos.DrawLine(from, vector2);
			from = vector2;
		}
		if (this.debugTransform != null)
		{
			Vector3 center;
			float closestEvaluationOnSpline = this.GetClosestEvaluationOnSpline(this.debugTransform.position, out center);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.Evaluate(closestEvaluationOnSpline), 0.2f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(center, 0.25f);
			if (this.controlPoints.Count > 3)
			{
				Gizmos.color = Color.green;
				from = this.controlPoints[1];
				for (int j = 2; j < this.controlPoints.Count - 2; j++)
				{
					Vector3 vector3 = this.controlPoints[j];
					Gizmos.DrawLine(from, vector3);
					from = vector3;
				}
			}
		}
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x0004FB6C File Offset: 0x0004DD6C
	public static Matrix4x4 CatmullRom(float t, Matrix4x4 p0, Matrix4x4 p1, Matrix4x4 p2, Matrix4x4 p3)
	{
		Vector3 pos = CatmullRomSpline.CatmullRom(t, p0.GetColumn(3), p1.GetColumn(3), p2.GetColumn(3), p3.GetColumn(3));
		Quaternion q = Quaternion.Slerp(p1.rotation, p2.rotation, t);
		Vector3 s = Vector3.Lerp(p1.lossyScale, p2.lossyScale, t);
		return Matrix4x4.TRS(pos, q, s);
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x0004FBE4 File Offset: 0x0004DDE4
	public static Matrix4x4 Evaluate(List<Matrix4x4> controlPoints, float t)
	{
		if (controlPoints.Count < 1)
		{
			return Matrix4x4.identity;
		}
		if (controlPoints.Count < 2)
		{
			return controlPoints[0];
		}
		if (controlPoints.Count < 4)
		{
			return controlPoints[0];
		}
		float num = t * (float)(controlPoints.Count - 3);
		int num2 = Mathf.FloorToInt(num);
		float t2 = num - (float)num2;
		int num3 = num2;
		if (num3 >= controlPoints.Count - 3)
		{
			num3 = controlPoints.Count - 4;
			t2 = 1f;
		}
		return CatmullRomSpline.CatmullRom(t2, controlPoints[num3], controlPoints[num3 + 1], controlPoints[num3 + 2], controlPoints[num3 + 3]);
	}

	// Token: 0x040010AE RID: 4270
	public Transform[] controlPointTransforms = new Transform[0];

	// Token: 0x040010AF RID: 4271
	public Transform debugTransform;

	// Token: 0x040010B0 RID: 4272
	public List<Vector3> controlPoints = new List<Vector3>();

	// Token: 0x040010B1 RID: 4273
	public List<Matrix4x4> controlPointsTransformationMatricies = new List<Matrix4x4>();

	// Token: 0x040010B2 RID: 4274
	public float testFloat;
}
