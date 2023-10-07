using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000222 RID: 546
public class BezierSpline : MonoBehaviour
{
	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000D7D RID: 3453 RVA: 0x0004EB55 File Offset: 0x0004CD55
	// (set) Token: 0x06000D7E RID: 3454 RVA: 0x0004EB5D File Offset: 0x0004CD5D
	public bool Loop
	{
		get
		{
			return this.loop;
		}
		set
		{
			this.loop = value;
			if (value)
			{
				this.modes[this.modes.Length - 1] = this.modes[0];
				this.SetControlPoint(0, this.points[0]);
			}
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000D7F RID: 3455 RVA: 0x0004EB95 File Offset: 0x0004CD95
	public int ControlPointCount
	{
		get
		{
			return this.points.Length;
		}
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x0004EB9F File Offset: 0x0004CD9F
	public Vector3 GetControlPoint(int index)
	{
		return this.points[index];
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x0004EBB0 File Offset: 0x0004CDB0
	public void SetControlPoint(int index, Vector3 point)
	{
		if (index % 3 == 0)
		{
			Vector3 b = point - this.points[index];
			if (this.loop)
			{
				if (index == 0)
				{
					this.points[1] += b;
					this.points[this.points.Length - 2] += b;
					this.points[this.points.Length - 1] = point;
				}
				else if (index == this.points.Length - 1)
				{
					this.points[0] = point;
					this.points[1] += b;
					this.points[index - 1] += b;
				}
				else
				{
					this.points[index - 1] += b;
					this.points[index + 1] += b;
				}
			}
			else
			{
				if (index > 0)
				{
					this.points[index - 1] += b;
				}
				if (index + 1 < this.points.Length)
				{
					this.points[index + 1] += b;
				}
			}
		}
		this.points[index] = point;
		this.EnforceMode(index);
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x0004ED42 File Offset: 0x0004CF42
	public BezierControlPointMode GetControlPointMode(int index)
	{
		return this.modes[(index + 1) / 3];
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x0004ED50 File Offset: 0x0004CF50
	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		int num = (index + 1) / 3;
		this.modes[num] = mode;
		if (this.loop)
		{
			if (num == 0)
			{
				this.modes[this.modes.Length - 1] = mode;
			}
			else if (num == this.modes.Length - 1)
			{
				this.modes[0] = mode;
			}
		}
		this.EnforceMode(index);
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x0004EDA8 File Offset: 0x0004CFA8
	private void EnforceMode(int index)
	{
		int num = (index + 1) / 3;
		BezierControlPointMode bezierControlPointMode = this.modes[num];
		if (bezierControlPointMode == BezierControlPointMode.Free || (!this.loop && (num == 0 || num == this.modes.Length - 1)))
		{
			return;
		}
		int num2 = num * 3;
		int num3;
		int num4;
		if (index <= num2)
		{
			num3 = num2 - 1;
			if (num3 < 0)
			{
				num3 = this.points.Length - 2;
			}
			num4 = num2 + 1;
			if (num4 >= this.points.Length)
			{
				num4 = 1;
			}
		}
		else
		{
			num3 = num2 + 1;
			if (num3 >= this.points.Length)
			{
				num3 = 1;
			}
			num4 = num2 - 1;
			if (num4 < 0)
			{
				num4 = this.points.Length - 2;
			}
		}
		Vector3 a = this.points[num2];
		Vector3 b = a - this.points[num3];
		if (bezierControlPointMode == BezierControlPointMode.Aligned)
		{
			b = b.normalized * Vector3.Distance(a, this.points[num4]);
		}
		this.points[num4] = a + b;
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000D85 RID: 3461 RVA: 0x0004EE97 File Offset: 0x0004D097
	public int CurveCount
	{
		get
		{
			return (this.points.Length - 1) / 3;
		}
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x0004EEA8 File Offset: 0x0004D0A8
	public Vector3 GetPoint(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t));
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x0004EF38 File Offset: 0x0004D138
	public Vector3 GetPointLocal(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t);
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x0004EFBC File Offset: 0x0004D1BC
	public Vector3 GetVelocity(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t)) - base.transform.position;
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x0004F05C File Offset: 0x0004D25C
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x0004F078 File Offset: 0x0004D278
	public void AddCurve()
	{
		Vector3 vector = this.points[this.points.Length - 1];
		Array.Resize<Vector3>(ref this.points, this.points.Length + 3);
		vector.x += 1f;
		this.points[this.points.Length - 3] = vector;
		vector.x += 1f;
		this.points[this.points.Length - 2] = vector;
		vector.x += 1f;
		this.points[this.points.Length - 1] = vector;
		Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length + 1);
		this.modes[this.modes.Length - 1] = this.modes[this.modes.Length - 2];
		this.EnforceMode(this.points.Length - 4);
		if (this.loop)
		{
			this.points[this.points.Length - 1] = this.points[0];
			this.modes[this.modes.Length - 1] = this.modes[0];
			this.EnforceMode(0);
		}
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x0004F1B2 File Offset: 0x0004D3B2
	public void RemoveLastCurve()
	{
		if (this.points.Length <= 4)
		{
			return;
		}
		Array.Resize<Vector3>(ref this.points, this.points.Length - 3);
		Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length - 1);
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x0004F1EC File Offset: 0x0004D3EC
	public void RemoveCurve(int index)
	{
		if (this.points.Length <= 4)
		{
			return;
		}
		List<Vector3> list = this.points.ToList<Vector3>();
		int num = 4;
		while (num < this.points.Length && index - 3 > num)
		{
			num += 3;
		}
		for (int i = 0; i < 3; i++)
		{
			list.RemoveAt(num);
		}
		this.points = list.ToArray();
		int index2 = (num - 4) / 3;
		List<BezierControlPointMode> list2 = this.modes.ToList<BezierControlPointMode>();
		list2.RemoveAt(index2);
		this.modes = list2.ToArray();
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x0004F274 File Offset: 0x0004D474
	public void Reset()
	{
		this.points = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 2f),
			new Vector3(0f, -1f, 4f),
			new Vector3(0f, -1f, 6f)
		};
		this.modes = new BezierControlPointMode[2];
	}

	// Token: 0x040010A6 RID: 4262
	[SerializeField]
	private Vector3[] points;

	// Token: 0x040010A7 RID: 4263
	[SerializeField]
	private BezierControlPointMode[] modes;

	// Token: 0x040010A8 RID: 4264
	[SerializeField]
	private bool loop;
}
