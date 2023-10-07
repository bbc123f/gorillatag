using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x0200028A RID: 650
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerSwimmingParameters", order = 1)]
	public class PlayerSwimmingParameters : ScriptableObject
	{
		// Token: 0x04001317 RID: 4887
		[Header("Base Settings")]
		public float floatingWaterLevelBelowHead = 0.6f;

		// Token: 0x04001318 RID: 4888
		public float buoyancyFadeDist = 0.3f;

		// Token: 0x04001319 RID: 4889
		public bool extendBouyancyFromSpeed;

		// Token: 0x0400131A RID: 4890
		public float buoyancyExtensionDecayHalflife = 0.2f;

		// Token: 0x0400131B RID: 4891
		public float baseUnderWaterDampingHalfLife = 0.25f;

		// Token: 0x0400131C RID: 4892
		public float swimUnderWaterDampingHalfLife = 1.1f;

		// Token: 0x0400131D RID: 4893
		public AnimationCurve speedToBouyancyExtension = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400131E RID: 4894
		public Vector2 speedToBouyancyExtensionMinMax = Vector2.zero;

		// Token: 0x0400131F RID: 4895
		public float swimmingVelocityOutOfWaterDrainRate = 3f;

		// Token: 0x04001320 RID: 4896
		[Range(0f, 1f)]
		public float underwaterJumpsAsSwimVelocityFactor = 1f;

		// Token: 0x04001321 RID: 4897
		[Range(0f, 1f)]
		public float swimmingHapticsStrength = 0.5f;

		// Token: 0x04001322 RID: 4898
		[Header("Surface Jumping")]
		public bool allowWaterSurfaceJumps;

		// Token: 0x04001323 RID: 4899
		public float waterSurfaceJumpHandSpeedThreshold = 1f;

		// Token: 0x04001324 RID: 4900
		public float waterSurfaceJumpAmount;

		// Token: 0x04001325 RID: 4901
		public float waterSurfaceJumpMaxSpeed = 1f;

		// Token: 0x04001326 RID: 4902
		public AnimationCurve waterSurfaceJumpPalmFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001327 RID: 4903
		public AnimationCurve waterSurfaceJumpHandVelocityFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001328 RID: 4904
		[Header("Diving")]
		public bool applyDiveSteering;

		// Token: 0x04001329 RID: 4905
		public bool applyDiveDampingMultiplier;

		// Token: 0x0400132A RID: 4906
		public float diveDampingMultiplier = 1f;

		// Token: 0x0400132B RID: 4907
		[Tooltip("In degrees")]
		public float maxDiveSteerAnglePerStep = 1f;

		// Token: 0x0400132C RID: 4908
		public float diveVelocityAveragingWindow = 0.1f;

		// Token: 0x0400132D RID: 4909
		public bool applyDiveSwimVelocityConversion;

		// Token: 0x0400132E RID: 4910
		[Tooltip("In meters per second")]
		public float diveSwimVelocityConversionRate = 3f;

		// Token: 0x0400132F RID: 4911
		public float diveMaxSwimVelocityConversion = 3f;

		// Token: 0x04001330 RID: 4912
		public bool reduceDiveSteeringBelowVelocityPlane;

		// Token: 0x04001331 RID: 4913
		public float reduceDiveSteeringBelowPlaneFadeStartDist = 0.4f;

		// Token: 0x04001332 RID: 4914
		public float reduceDiveSteeringBelowPlaneFadeEndDist = 0.55f;

		// Token: 0x04001333 RID: 4915
		public AnimationCurve palmFacingToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001334 RID: 4916
		public Vector2 palmFacingToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04001335 RID: 4917
		public AnimationCurve swimSpeedToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001336 RID: 4918
		public Vector2 swimSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04001337 RID: 4919
		public AnimationCurve swimSpeedToMaxRedirectAngle = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001338 RID: 4920
		public Vector2 swimSpeedToMaxRedirectAngleMinMax = Vector2.zero;

		// Token: 0x04001339 RID: 4921
		public AnimationCurve handSpeedToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x0400133A RID: 4922
		public Vector2 handSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x0400133B RID: 4923
		public AnimationCurve handAccelToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x0400133C RID: 4924
		public Vector2 handAccelToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x0400133D RID: 4925
		public AnimationCurve nonDiveDampingHapticsAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400133E RID: 4926
		public Vector2 nonDiveDampingHapticsAmountMinMax = Vector2.zero;
	}
}
