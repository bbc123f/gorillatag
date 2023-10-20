using System;
using UnityEngine;

// Token: 0x0200009F RID: 159
public static class VectorUtil
{
	// Token: 0x0600037C RID: 892 RVA: 0x000155C5 File Offset: 0x000137C5
	public static Vector4 ToVector(this Rect rect)
	{
		return new Vector4(rect.x, rect.y, rect.width, rect.height);
	}
}
