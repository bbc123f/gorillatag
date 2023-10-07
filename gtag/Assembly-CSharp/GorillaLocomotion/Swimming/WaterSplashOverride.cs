using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000291 RID: 657
	public class WaterSplashOverride : MonoBehaviour
	{
		// Token: 0x0400139F RID: 5023
		public bool suppressWaterEffects;

		// Token: 0x040013A0 RID: 5024
		public bool playBigSplash;

		// Token: 0x040013A1 RID: 5025
		public bool playDrippingEffect = true;

		// Token: 0x040013A2 RID: 5026
		public bool scaleByPlayersScale;

		// Token: 0x040013A3 RID: 5027
		public bool overrideBoundingRadius;

		// Token: 0x040013A4 RID: 5028
		public float boundingRadiusOverride = 1f;
	}
}
