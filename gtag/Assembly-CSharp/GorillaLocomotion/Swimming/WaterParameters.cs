using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000290 RID: 656
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
	public class WaterParameters : ScriptableObject
	{
		// Token: 0x04001389 RID: 5001
		[Header("Splash Effect")]
		public bool playSplashEffect = true;

		// Token: 0x0400138A RID: 5002
		public GameObject splashEffect;

		// Token: 0x0400138B RID: 5003
		public float splashEffectScale = 1f;

		// Token: 0x0400138C RID: 5004
		public bool sendSplashEffectRPCs;

		// Token: 0x0400138D RID: 5005
		public float splashSpeedRequirement = 0.8f;

		// Token: 0x0400138E RID: 5006
		public float bigSplashSpeedRequirement = 1.9f;

		// Token: 0x0400138F RID: 5007
		public Gradient splashColorBySpeedGradient;

		// Token: 0x04001390 RID: 5008
		[Header("Ripple Effect")]
		public bool playRippleEffect = true;

		// Token: 0x04001391 RID: 5009
		public GameObject rippleEffect;

		// Token: 0x04001392 RID: 5010
		public float rippleEffectScale = 1f;

		// Token: 0x04001393 RID: 5011
		public float defaultDistanceBetweenRipples = 0.75f;

		// Token: 0x04001394 RID: 5012
		public float minDistanceBetweenRipples = 0.2f;

		// Token: 0x04001395 RID: 5013
		public float minTimeBetweenRipples = 0.75f;

		// Token: 0x04001396 RID: 5014
		public Color rippleSpriteColor = Color.white;

		// Token: 0x04001397 RID: 5015
		[Header("Drip Effect")]
		public bool playDripEffect = true;

		// Token: 0x04001398 RID: 5016
		public float postExitDripDuration = 1.5f;

		// Token: 0x04001399 RID: 5017
		public float perDripTimeDelay = 0.2f;

		// Token: 0x0400139A RID: 5018
		public float perDripTimeRandRange = 0.15f;

		// Token: 0x0400139B RID: 5019
		public float perDripDefaultRadius = 0.01f;

		// Token: 0x0400139C RID: 5020
		public float perDripRadiusRandRange = 0.01f;

		// Token: 0x0400139D RID: 5021
		[Header("Misc")]
		public float recomputeSurfaceForColliderDist = 0.2f;

		// Token: 0x0400139E RID: 5022
		public bool allowBubblesInVolume;
	}
}
