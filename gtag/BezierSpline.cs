using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
	[SerializeField]
	private Vector3[] points;

	[SerializeField]
	private BezierControlPointMode[] modes;

	[SerializeField]
	private bool loop;

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
			if (value)
			{
				modes[modes.Length - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount => points.Length;

	public int CurveCount => (points.Length - 1) / 3;

	public Vector3 GetControlPoint(int index)
	{
		return points[index];
	}

	public void SetControlPoint(int index, Vector3 point)
	{
		if (index % 3 == 0)
		{
			Vector3 vector = point - points[index];
			if (loop)
			{
				if (index == 0)
				{
					points[1] += vector;
					points[points.Length - 2] += vector;
					points[points.Length - 1] = point;
				}
				else if (index == points.Length - 1)
				{
					points[0] = point;
					points[1] += vector;
					points[index - 1] += vector;
				}
				else
				{
					points[index - 1] += vector;
					points[index + 1] += vector;
				}
			}
			else
			{
				if (index > 0)
				{
					points[index - 1] += vector;
				}
				if (index + 1 < points.Length)
				{
					points[index + 1] += vector;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
	}

	public BezierControlPointMode GetControlPointMode(int index)
	{
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		int num = (index + 1) / 3;
		modes[num] = mode;
		if (loop)
		{
			if (num == 0)
			{
				modes[modes.Length - 1] = mode;
			}
			else if (num == modes.Length - 1)
			{
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode(int index)
	{
		int num = (index + 1) / 3;
		BezierControlPointMode bezierControlPointMode = modes[num];
		if (bezierControlPointMode == BezierControlPointMode.Free || (!loop && (num == 0 || num == modes.Length - 1)))
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
				num3 = points.Length - 2;
			}
			num4 = num2 + 1;
			if (num4 >= points.Length)
			{
				num4 = 1;
			}
		}
		else
		{
			num3 = num2 + 1;
			if (num3 >= points.Length)
			{
				num3 = 1;
			}
			num4 = num2 - 1;
			if (num4 < 0)
			{
				num4 = points.Length - 2;
			}
		}
		Vector3 vector = points[num2];
		Vector3 vector2 = vector - points[num3];
		if (bezierControlPointMode == BezierControlPointMode.Aligned)
		{
			vector2 = vector2.normalized * Vector3.Distance(vector, points[num4]);
		}
		points[num4] = vector + vector2;
	}

	public Vector3 GetPoint(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetPoint(points[num], points[num + 1], points[num + 2], points[num + 3], t));
	}

	public Vector3 GetPointLocal(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return Bezier.GetPoint(points[num], points[num + 1], points[num + 2], points[num + 3], t);
	}

	public Vector3 GetVelocity(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(points[num], points[num + 1], points[num + 2], points[num + 3], t)) - base.transform.position;
	}

	public Vector3 GetDirection(float t)
	{
		return GetVelocity(t).normalized;
	}

	public void AddCurve()
	{
		Vector3 vector = points[points.Length - 1];
		Array.Resize(ref points, points.Length + 3);
		vector.x += 1f;
		points[points.Length - 3] = vector;
		vector.x += 1f;
		points[points.Length - 2] = vector;
		vector.x += 1f;
		points[points.Length - 1] = vector;
		Array.Resize(ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length - 2];
		EnforceMode(points.Length - 4);
		if (loop)
		{
			points[points.Length - 1] = points[0];
			modes[modes.Length - 1] = modes[0];
			EnforceMode(0);
		}
	}

	public void RemoveLastCurve()
	{
		if (points.Length > 4)
		{
			Array.Resize(ref points, points.Length - 3);
			Array.Resize(ref modes, modes.Length - 1);
		}
	}

	public void RemoveCurve(int index)
	{
		if (points.Length > 4)
		{
			List<Vector3> list = points.ToList();
			int i;
			for (i = 4; i < points.Length && index - 3 > i; i += 3)
			{
			}
			for (int j = 0; j < 3; j++)
			{
				list.RemoveAt(i);
			}
			points = list.ToArray();
			int index2 = (i - 4) / 3;
			List<BezierControlPointMode> list2 = modes.ToList();
			list2.RemoveAt(index2);
			modes = list2.ToArray();
		}
	}

	public void Reset()
	{
		points = new Vector3[4]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 2f),
			new Vector3(0f, -1f, 4f),
			new Vector3(0f, -1f, 6f)
		};
		modes = new BezierControlPointMode[2];
	}
}
