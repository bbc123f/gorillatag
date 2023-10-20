﻿using System;
using Photon.Realtime;

// Token: 0x020000E0 RID: 224
public class LegacyWorldTargetItem
{
	// Token: 0x0600052B RID: 1323 RVA: 0x000210E5 File Offset: 0x0001F2E5
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x000210FB File Offset: 0x0001F2FB
	public void Invalidate()
	{
		this.itemIdx = -1;
		this.owner = null;
	}

	// Token: 0x0400061F RID: 1567
	public Player owner;

	// Token: 0x04000620 RID: 1568
	public int itemIdx;
}