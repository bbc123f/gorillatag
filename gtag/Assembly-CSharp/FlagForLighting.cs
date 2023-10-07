using System;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class FlagForLighting : MonoBehaviour
{
	// Token: 0x04001074 RID: 4212
	public FlagForLighting.TimeOfDay myTimeOfDay;

	// Token: 0x02000478 RID: 1144
	public enum TimeOfDay
	{
		// Token: 0x04001E94 RID: 7828
		Sunrise,
		// Token: 0x04001E95 RID: 7829
		TenAM,
		// Token: 0x04001E96 RID: 7830
		Noon,
		// Token: 0x04001E97 RID: 7831
		ThreePM,
		// Token: 0x04001E98 RID: 7832
		Sunset,
		// Token: 0x04001E99 RID: 7833
		Night,
		// Token: 0x04001E9A RID: 7834
		RainingDay,
		// Token: 0x04001E9B RID: 7835
		RainingNight,
		// Token: 0x04001E9C RID: 7836
		None
	}
}
