using System;
using UnityEngine;

// Token: 0x02000222 RID: 546
public class BezierCurve : MonoBehaviour
{
	// Token: 0x06000D7E RID: 3454 RVA: 0x0004EC5C File Offset: 0x0004CE5C
	public Vector3 GetPoint(float t)
	{
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t));
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x0004ECAC File Offset: 0x0004CEAC
	public Vector3 GetVelocity(float t)
	{
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t)) - base.transform.position;
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x0004ED0C File Offset: 0x0004CF0C
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x0004ED28 File Offset: 0x0004CF28
	public void Reset()
	{
		this.points = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
	}

	// Token: 0x040010AA RID: 4266
	public Vector3[] points;
}
