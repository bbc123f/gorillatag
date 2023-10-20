using System;
using UnityEngine;

// Token: 0x020001CC RID: 460
public static class Id128Ext
{
	// Token: 0x06000BCB RID: 3019 RVA: 0x000496DC File Offset: 0x000478DC
	public static Id128 ToId128(this Hash128 h)
	{
		return new Id128(h);
	}
}
