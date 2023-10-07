using System;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class GorillaSurfaceOverride : MonoBehaviour
{
	// Token: 0x04000A18 RID: 2584
	[GorillaSoundLookup]
	public int overrideIndex;

	// Token: 0x04000A19 RID: 2585
	public float extraVelMultiplier = 1f;

	// Token: 0x04000A1A RID: 2586
	public float extraVelMaxMultiplier = 1f;

	// Token: 0x04000A1B RID: 2587
	public bool sendOnTapEvent;
}
