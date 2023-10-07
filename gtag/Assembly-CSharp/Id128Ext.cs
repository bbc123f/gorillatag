using System;
using UnityEngine;

// Token: 0x020001CB RID: 459
public static class Id128Ext
{
	// Token: 0x06000BC5 RID: 3013 RVA: 0x00049474 File Offset: 0x00047674
	public static Id128 ToId128(this Hash128 h)
	{
		return new Id128(h);
	}
}
