using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000293 RID: 659
	public class WaterSplashOverride : MonoBehaviour
	{
		// Token: 0x040013AC RID: 5036
		public bool suppressWaterEffects;

		// Token: 0x040013AD RID: 5037
		public bool playBigSplash;

		// Token: 0x040013AE RID: 5038
		public bool playDrippingEffect = true;

		// Token: 0x040013AF RID: 5039
		public bool scaleByPlayersScale;

		// Token: 0x040013B0 RID: 5040
		public bool overrideBoundingRadius;

		// Token: 0x040013B1 RID: 5041
		public float boundingRadiusOverride = 1f;
	}
}
