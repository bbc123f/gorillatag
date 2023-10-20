using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000292 RID: 658
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
	public class WaterParameters : ScriptableObject
	{
		// Token: 0x04001396 RID: 5014
		[Header("Splash Effect")]
		public bool playSplashEffect = true;

		// Token: 0x04001397 RID: 5015
		public GameObject splashEffect;

		// Token: 0x04001398 RID: 5016
		public float splashEffectScale = 1f;

		// Token: 0x04001399 RID: 5017
		public bool sendSplashEffectRPCs;

		// Token: 0x0400139A RID: 5018
		public float splashSpeedRequirement = 0.8f;

		// Token: 0x0400139B RID: 5019
		public float bigSplashSpeedRequirement = 1.9f;

		// Token: 0x0400139C RID: 5020
		public Gradient splashColorBySpeedGradient;

		// Token: 0x0400139D RID: 5021
		[Header("Ripple Effect")]
		public bool playRippleEffect = true;

		// Token: 0x0400139E RID: 5022
		public GameObject rippleEffect;

		// Token: 0x0400139F RID: 5023
		public float rippleEffectScale = 1f;

		// Token: 0x040013A0 RID: 5024
		public float defaultDistanceBetweenRipples = 0.75f;

		// Token: 0x040013A1 RID: 5025
		public float minDistanceBetweenRipples = 0.2f;

		// Token: 0x040013A2 RID: 5026
		public float minTimeBetweenRipples = 0.75f;

		// Token: 0x040013A3 RID: 5027
		public Color rippleSpriteColor = Color.white;

		// Token: 0x040013A4 RID: 5028
		[Header("Drip Effect")]
		public bool playDripEffect = true;

		// Token: 0x040013A5 RID: 5029
		public float postExitDripDuration = 1.5f;

		// Token: 0x040013A6 RID: 5030
		public float perDripTimeDelay = 0.2f;

		// Token: 0x040013A7 RID: 5031
		public float perDripTimeRandRange = 0.15f;

		// Token: 0x040013A8 RID: 5032
		public float perDripDefaultRadius = 0.01f;

		// Token: 0x040013A9 RID: 5033
		public float perDripRadiusRandRange = 0.01f;

		// Token: 0x040013AA RID: 5034
		[Header("Misc")]
		public float recomputeSurfaceForColliderDist = 0.2f;

		// Token: 0x040013AB RID: 5035
		public bool allowBubblesInVolume;
	}
}
