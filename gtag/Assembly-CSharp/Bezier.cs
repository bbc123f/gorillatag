﻿using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
public static class Bezier
{
	// Token: 0x06000D74 RID: 3444 RVA: 0x0004E8A8 File Offset: 0x0004CAA8
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return num * num * p0 + 2f * num * t * p1 + t * t * p2;
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x0004E8F0 File Offset: 0x0004CAF0
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x0004E924 File Offset: 0x0004CB24
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x0004E990 File Offset: 0x0004CB90
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return 3f * num * num * (p1 - p0) + 6f * num * t * (p2 - p1) + 3f * t * t * (p3 - p2);
	}
}
