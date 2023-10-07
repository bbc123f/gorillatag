using System;
using UnityEngine;

// Token: 0x0200009F RID: 159
public static class VectorUtil
{
	// Token: 0x0600037C RID: 892 RVA: 0x000157E9 File Offset: 0x000139E9
	public static Vector4 ToVector(this Rect rect)
	{
		return new Vector4(rect.x, rect.y, rect.width, rect.height);
	}
}
