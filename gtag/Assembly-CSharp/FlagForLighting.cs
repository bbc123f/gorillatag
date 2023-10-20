using System;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class FlagForLighting : MonoBehaviour
{
	// Token: 0x04001079 RID: 4217
	public FlagForLighting.TimeOfDay myTimeOfDay;

	// Token: 0x0200047A RID: 1146
	public enum TimeOfDay
	{
		// Token: 0x04001EA1 RID: 7841
		Sunrise,
		// Token: 0x04001EA2 RID: 7842
		TenAM,
		// Token: 0x04001EA3 RID: 7843
		Noon,
		// Token: 0x04001EA4 RID: 7844
		ThreePM,
		// Token: 0x04001EA5 RID: 7845
		Sunset,
		// Token: 0x04001EA6 RID: 7846
		Night,
		// Token: 0x04001EA7 RID: 7847
		RainingDay,
		// Token: 0x04001EA8 RID: 7848
		RainingNight,
		// Token: 0x04001EA9 RID: 7849
		None
	}
}
