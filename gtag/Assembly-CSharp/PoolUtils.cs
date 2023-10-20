using System;
using UnityEngine;

// Token: 0x0200021A RID: 538
public static class PoolUtils
{
	// Token: 0x06000D5B RID: 3419 RVA: 0x0004E272 File Offset: 0x0004C472
	public static int GameObjHashCode(GameObject obj)
	{
		return obj.tag.GetHashCode();
	}
}
