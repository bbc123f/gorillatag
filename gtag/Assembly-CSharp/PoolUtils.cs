using System;
using UnityEngine;

// Token: 0x02000219 RID: 537
public static class PoolUtils
{
	// Token: 0x06000D55 RID: 3413 RVA: 0x0004E012 File Offset: 0x0004C212
	public static int GameObjHashCode(GameObject obj)
	{
		return obj.tag.GetHashCode();
	}
}
