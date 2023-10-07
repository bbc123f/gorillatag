using System;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class BezierCurve : MonoBehaviour
{
	// Token: 0x06000D78 RID: 3448 RVA: 0x0004E9FC File Offset: 0x0004CBFC
	public Vector3 GetPoint(float t)
	{
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t));
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x0004EA4C File Offset: 0x0004CC4C
	public Vector3 GetVelocity(float t)
	{
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t)) - base.transform.position;
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x0004EAAC File Offset: 0x0004CCAC
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x0004EAC8 File Offset: 0x0004CCC8
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

	// Token: 0x040010A5 RID: 4261
	public Vector3[] points;
}
