using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AA;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion
{
	// Token: 0x02000288 RID: 648
	public class Player : MonoBehaviour
	{
		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060010B6 RID: 4278 RVA: 0x00059009 File Offset: 0x00057209
		public static Player Instance
		{
			get
			{
				return Player._instance;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060010B7 RID: 4279 RVA: 0x00059010 File Offset: 0x00057210
		public bool IsDefaultScale
		{
			get
			{
				return Mathf.Abs(1f - this.scale) < 0.001f;
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060010B8 RID: 4280 RVA: 0x0005902A File Offset: 0x0005722A
		public bool turnedThisFrame
		{
			get
			{
				return this.degreesTurnedThisFrame != 0f;
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060010B9 RID: 4281 RVA: 0x0005903C File Offset: 0x0005723C
		public List<Player.MaterialData> materialData
		{
			get
			{
				return this.materialDatasSO.datas;
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060010BA RID: 4282 RVA: 0x00059049 File Offset: 0x00057249
		public List<WaterVolume> HeadOverlappingWaterVolumes
		{
			get
			{
				return this.headOverlappingWaterVolumes;
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060010BB RID: 4283 RVA: 0x00059051 File Offset: 0x00057251
		public bool InWater
		{
			get
			{
				return this.bodyInWater;
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060010BC RID: 4284 RVA: 0x00059059 File Offset: 0x00057259
		public bool HeadInWater
		{
			get
			{
				return this.headInWater;
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060010BD RID: 4285 RVA: 0x00059061 File Offset: 0x00057261
		public WaterVolume CurrentWaterVolume
		{
			get
			{
				if (this.bodyOverlappingWaterVolumes.Count <= 0)
				{
					return null;
				}
				return this.bodyOverlappingWaterVolumes[0];
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060010BE RID: 4286 RVA: 0x0005907F File Offset: 0x0005727F
		public WaterVolume.SurfaceQuery WaterSurfaceForHead
		{
			get
			{
				return this.waterSurfaceForHead;
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060010BF RID: 4287 RVA: 0x00059087 File Offset: 0x00057287
		public WaterVolume LeftHandWaterVolume
		{
			get
			{
				return this.leftHandWaterVolume;
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060010C0 RID: 4288 RVA: 0x0005908F File Offset: 0x0005728F
		public WaterVolume RightHandWaterVolume
		{
			get
			{
				return this.rightHandWaterVolume;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060010C1 RID: 4289 RVA: 0x00059097 File Offset: 0x00057297
		public WaterVolume.SurfaceQuery LeftHandWaterSurface
		{
			get
			{
				return this.leftHandWaterSurface;
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060010C2 RID: 4290 RVA: 0x0005909F File Offset: 0x0005729F
		public WaterVolume.SurfaceQuery RightHandWaterSurface
		{
			get
			{
				return this.rightHandWaterSurface;
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060010C3 RID: 4291 RVA: 0x000590A7 File Offset: 0x000572A7
		public Vector3 LastLeftHandPosition
		{
			get
			{
				return this.lastLeftHandPosition;
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060010C4 RID: 4292 RVA: 0x000590AF File Offset: 0x000572AF
		public Vector3 LastRightHandPosition
		{
			get
			{
				return this.lastRightHandPosition;
			}
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x000590B8 File Offset: 0x000572B8
		private void Awake()
		{
			if (Player._instance != null && Player._instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				Player._instance = this;
				Player.hasInstance = true;
			}
			this.InitializeValues();
			this.playerRigidBody.maxAngularVelocity = 0f;
			this.bodyOffsetVector = new Vector3(0f, -this.bodyCollider.height / 2f, 0f);
			this.bodyInitialHeight = this.bodyCollider.height;
			this.bodyInitialRadius = this.bodyCollider.radius;
			this.rayCastNonAllocColliders = new RaycastHit[5];
			this.crazyCheckVectors = new Vector3[7];
			this.emptyHit = default(RaycastHit);
			this.crazyCheckVectors[0] = Vector3.up;
			this.crazyCheckVectors[1] = Vector3.down;
			this.crazyCheckVectors[2] = Vector3.left;
			this.crazyCheckVectors[3] = Vector3.right;
			this.crazyCheckVectors[4] = Vector3.forward;
			this.crazyCheckVectors[5] = Vector3.back;
			this.crazyCheckVectors[6] = Vector3.zero;
			if (this.controllerState == null)
			{
				this.controllerState = base.GetComponent<ConnectedControllerHandler>();
			}
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x0005920C File Offset: 0x0005740C
		protected void OnDestroy()
		{
			if (Player._instance == this)
			{
				Player._instance = null;
				Player.hasInstance = false;
			}
			if (this.climbHelper)
			{
				Object.Destroy(this.climbHelper.gameObject);
			}
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x00059244 File Offset: 0x00057444
		public void InitializeValues()
		{
			Physics.SyncTransforms();
			this.playerRigidBody = base.GetComponent<Rigidbody>();
			this.velocityHistory = new Vector3[this.velocityHistorySize];
			this.slideAverageHistory = new Vector3[this.velocityHistorySize];
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Vector3.zero;
				this.slideAverageHistory[i] = Vector3.zero;
			}
			this.leftHandFollower.transform.position = this.leftControllerTransform.position;
			this.rightHandFollower.transform.position = this.rightControllerTransform.position;
			this.lastLeftHandPosition = this.leftHandFollower.transform.position;
			this.lastRightHandPosition = this.rightHandFollower.transform.position;
			this.lastHeadPosition = this.headCollider.transform.position;
			this.wasLeftHandTouching = false;
			this.wasRightHandTouching = false;
			this.velocityIndex = 0;
			this.denormalizedVelocityAverage = Vector3.zero;
			this.slideAverage = Vector3.zero;
			this.lastPosition = base.transform.position;
			this.lastRealTime = Time.realtimeSinceStartup;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x000593F4 File Offset: 0x000575F4
		public void SetHalloweenLevitation(float levitateStrength, float levitateDuration, float levitateBlendOutDuration, float levitateBonusStrength, float levitateBonusOffAtYSpeed, float levitateBonusFullAtYSpeed)
		{
			this.halloweenLevitationStrength = levitateStrength;
			this.halloweenLevitationFullStrengthDuration = levitateDuration;
			this.halloweenLevitationTotalDuration = levitateDuration + levitateBlendOutDuration;
			this.halloweenLevitateBonusFullAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitateBonusOffAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitationBonusStrength = levitateBonusStrength;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00059428 File Offset: 0x00057628
		public void FixedUpdate()
		{
			this.AntiTeleportTechnology();
			bool isDefaultScale = this.IsDefaultScale;
			if (!isDefaultScale)
			{
				this.playerRigidBody.AddForce(-Physics.gravity * (1f - this.scale), ForceMode.Acceleration);
			}
			if (this.halloweenLevitationBonusStrength > 0f || this.halloweenLevitationStrength > 0f)
			{
				float num = Time.time - this.lastTouchedGroundTimestamp;
				if (num < this.halloweenLevitationTotalDuration)
				{
					this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationStrength * Mathf.InverseLerp(this.halloweenLevitationFullStrengthDuration, this.halloweenLevitationTotalDuration, num), ForceMode.Acceleration);
				}
				float y = this.playerRigidBody.velocity.y;
				if (y <= this.halloweenLevitateBonusFullAtYSpeed)
				{
					this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength, ForceMode.Acceleration);
				}
				else if (y <= this.halloweenLevitateBonusOffAtYSpeed)
				{
					Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.velocity.y);
					this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength * Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.velocity.y), ForceMode.Acceleration);
				}
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.bodyInWater = false;
			Vector3 lhs = this.swimmingVelocity;
			this.swimmingVelocity = Vector3.MoveTowards(this.swimmingVelocity, Vector3.zero, this.swimmingParams.swimmingVelocityOutOfWaterDrainRate * fixedDeltaTime);
			this.leftHandNonDiveHapticsAmount = 0f;
			this.rightHandNonDiveHapticsAmount = 0f;
			if (this.bodyOverlappingWaterVolumes.Count > 0)
			{
				WaterVolume waterVolume = null;
				float num2 = float.MinValue;
				Vector3 vector = this.headCollider.transform.position + Vector3.down * this.swimmingParams.floatingWaterLevelBelowHead * this.scale;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.bodyOverlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery;
					if (this.bodyOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out surfaceQuery, false))
					{
						float num3 = Vector3.Dot(surfaceQuery.surfacePoint - vector, surfaceQuery.surfaceNormal);
						if (num3 > num2)
						{
							num2 = num3;
							waterVolume = this.bodyOverlappingWaterVolumes[i];
							this.waterSurfaceForHead = surfaceQuery;
						}
						WaterCurrent waterCurrent = this.bodyOverlappingWaterVolumes[i].Current;
						if (waterCurrent != null && num3 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (waterVolume != null)
				{
					Vector3 velocity = this.playerRigidBody.velocity;
					float magnitude = velocity.magnitude;
					bool flag = this.headInWater;
					this.headInWater = (this.headCollider.transform.position.y < this.waterSurfaceForHead.surfacePoint.y && this.headCollider.transform.position.y > this.waterSurfaceForHead.surfacePoint.y - this.waterSurfaceForHead.maxDepth);
					if (this.headInWater && !flag)
					{
						this.audioManager.SetMixerSnapshot(this.audioManager.underwaterSnapshot, 0.1f);
					}
					else if (!this.headInWater && flag)
					{
						this.audioManager.UnsetMixerSnapshot(0.1f);
					}
					this.bodyInWater = (vector.y < this.waterSurfaceForHead.surfacePoint.y && vector.y > this.waterSurfaceForHead.surfacePoint.y - this.waterSurfaceForHead.maxDepth);
					if (this.bodyInWater)
					{
						Player.LiquidProperties liquidProperties = this.liquidPropertiesList[(int)waterVolume.LiquidType];
						if (waterVolume != null)
						{
							float d;
							if (this.swimmingParams.extendBouyancyFromSpeed)
							{
								float time = Mathf.Clamp(Vector3.Dot(velocity / this.scale, this.waterSurfaceForHead.surfaceNormal), this.swimmingParams.speedToBouyancyExtensionMinMax.x, this.swimmingParams.speedToBouyancyExtensionMinMax.y);
								float b = this.swimmingParams.speedToBouyancyExtension.Evaluate(time);
								this.buoyancyExtension = Mathf.Max(this.buoyancyExtension, b);
								float num4 = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist + this.buoyancyExtension, num2 / this.scale + this.buoyancyExtension);
								this.buoyancyExtension = Spring.DamperDecayExact(this.buoyancyExtension, this.swimmingParams.buoyancyExtensionDecayHalflife, fixedDeltaTime, 1E-05f);
								d = num4;
							}
							else
							{
								d = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist, num2 / this.scale);
							}
							Vector3 a = Physics.gravity * this.scale;
							Vector3 force = liquidProperties.buoyancy * -a * d;
							this.playerRigidBody.AddForce(force, ForceMode.Acceleration);
						}
						Vector3 vector2 = Vector3.zero;
						Vector3 vector3 = Vector3.zero;
						for (int j = 0; j < this.activeWaterCurrents.Count; j++)
						{
							WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
							Vector3 startingVelocity = velocity + vector2;
							Vector3 b2;
							Vector3 b3;
							if (waterCurrent2.GetCurrentAtPoint(this.bodyCollider.transform.position, startingVelocity, fixedDeltaTime, out b2, out b3))
							{
								vector3 += b2;
								vector2 += b3;
							}
						}
						if (magnitude > Mathf.Epsilon)
						{
							float num5 = 0.01f;
							Vector3 vector4 = velocity / magnitude;
							Vector3 right = this.leftHandFollower.right;
							Vector3 dir = -this.rightHandFollower.right;
							Vector3 forward = this.leftHandFollower.forward;
							Vector3 forward2 = this.rightHandFollower.forward;
							Vector3 a2 = vector4;
							float num6 = 0f;
							float num7 = 0f;
							float num8 = 0f;
							if (this.swimmingParams.applyDiveSteering && !this.disableMovement && isDefaultScale)
							{
								float value = Vector3.Dot(velocity - vector3, vector4);
								float time2 = Mathf.Clamp(value, this.swimmingParams.swimSpeedToRedirectAmountMinMax.x, this.swimmingParams.swimSpeedToRedirectAmountMinMax.y);
								float b4 = this.swimmingParams.swimSpeedToRedirectAmount.Evaluate(time2);
								time2 = Mathf.Clamp(value, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.x, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.y);
								float num9 = this.swimmingParams.swimSpeedToMaxRedirectAngle.Evaluate(time2);
								float value2 = Mathf.Acos(Vector3.Dot(vector4, forward)) / 3.1415927f * -2f + 1f;
								float value3 = Mathf.Acos(Vector3.Dot(vector4, forward2)) / 3.1415927f * -2f + 1f;
								float num10 = Mathf.Clamp(value2, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float num11 = Mathf.Clamp(value3, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float a3 = (!float.IsNaN(num10)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num10) : 0f;
								float a4 = (!float.IsNaN(num11)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num11) : 0f;
								Vector3 a5 = Vector3.ProjectOnPlane(vector4, right);
								Vector3 a6 = Vector3.ProjectOnPlane(vector4, right);
								float num12 = Mathf.Min(a5.magnitude, 1f);
								float num13 = Mathf.Min(a6.magnitude, 1f);
								float magnitude2 = this.leftHandCenterVelocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float magnitude3 = this.rightHandCenterVelocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float time3 = Mathf.Clamp(magnitude2, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float time4 = Mathf.Clamp(magnitude3, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float a7 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(time3);
								float a8 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(time4);
								float averageSpeedChangeMagnitudeInDirection = this.leftHandCenterVelocityTracker.GetAverageSpeedChangeMagnitudeInDirection(right, false, this.swimmingParams.diveVelocityAveragingWindow);
								float averageSpeedChangeMagnitudeInDirection2 = this.rightHandCenterVelocityTracker.GetAverageSpeedChangeMagnitudeInDirection(dir, false, this.swimmingParams.diveVelocityAveragingWindow);
								float time5 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float time6 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection2, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float b5 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(time5);
								float b6 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(time6);
								num6 = Mathf.Min(a3, Mathf.Min(a7, b5));
								float num14 = (Vector3.Dot(vector4, forward) > 0f) ? (Mathf.Min(num6, b4) * num12) : 0f;
								num7 = Mathf.Min(a4, Mathf.Min(a8, b6));
								float num15 = (Vector3.Dot(vector4, forward2) > 0f) ? (Mathf.Min(num7, b4) * num13) : 0f;
								if (this.swimmingParams.reduceDiveSteeringBelowVelocityPlane)
								{
									Vector3 rhs;
									if (Vector3.Dot(this.headCollider.transform.up, vector4) > 0.95f)
									{
										rhs = -this.headCollider.transform.forward;
									}
									else
									{
										rhs = Vector3.Cross(Vector3.Cross(vector4, this.headCollider.transform.up), vector4).normalized;
									}
									Vector3 position = this.headCollider.transform.position;
									Vector3 lhs2 = position - this.leftHandFollower.position;
									Vector3 lhs3 = position - this.rightHandFollower.position;
									float reduceDiveSteeringBelowPlaneFadeStartDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeStartDist;
									float reduceDiveSteeringBelowPlaneFadeEndDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeEndDist;
									float f = Vector3.Dot(lhs2, Vector3.up);
									float f2 = Vector3.Dot(lhs3, Vector3.up);
									float f3 = Vector3.Dot(lhs2, rhs);
									float f4 = Vector3.Dot(lhs3, rhs);
									float num16 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(f), Mathf.Abs(f3)));
									float num17 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(f2), Mathf.Abs(f4)));
									num14 *= num16;
									num15 *= num17;
								}
								float num18 = num15 + num14;
								Vector3 vector5 = Vector3.zero;
								if (this.swimmingParams.applyDiveSteering && num18 > num5)
								{
									vector5 = ((num14 * a5 + num15 * a6) / num18).normalized;
									vector5 = Vector3.Lerp(vector4, vector5, num18);
									a2 = Vector3.RotateTowards(vector4, vector5, 0.017453292f * num9 * fixedDeltaTime, 0f);
								}
								else
								{
									a2 = vector4;
								}
								num8 = Mathf.Clamp01((num6 + num7) * 0.5f);
							}
							float num19 = Mathf.Clamp(Vector3.Dot(lhs, vector4), 0f, magnitude);
							float num20 = magnitude - num19;
							if (this.swimmingParams.applyDiveSwimVelocityConversion && !this.disableMovement && num8 > num5 && num19 < this.swimmingParams.diveMaxSwimVelocityConversion)
							{
								float num21 = Mathf.Min(this.swimmingParams.diveSwimVelocityConversionRate * fixedDeltaTime, num20) * num8;
								num19 += num21;
								num20 -= num21;
							}
							float halflife = this.swimmingParams.swimUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float halflife2 = this.swimmingParams.baseUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float num22 = Spring.DamperDecayExact(num19 / this.scale, halflife, fixedDeltaTime, 1E-05f) * this.scale;
							float num23 = Spring.DamperDecayExact(num20 / this.scale, halflife2, fixedDeltaTime, 1E-05f) * this.scale;
							if (this.swimmingParams.applyDiveDampingMultiplier && !this.disableMovement)
							{
								float t = Mathf.Lerp(1f, this.swimmingParams.diveDampingMultiplier, num8);
								num22 = Mathf.Lerp(num19, num22, t);
								num23 = Mathf.Lerp(num20, num23, t);
								float time7 = Mathf.Clamp((1f - num6) * (num19 + num20), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num5, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num5);
								float time8 = Mathf.Clamp((1f - num7) * (num19 + num20), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num5, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num5);
								this.leftHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time7);
								this.rightHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time8);
							}
							this.swimmingVelocity = num22 * a2 + vector2 * this.scale;
							this.playerRigidBody.velocity = this.swimmingVelocity + num23 * a2;
						}
					}
				}
			}
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0005A19C File Offset: 0x0005839C
		private void BodyCollider()
		{
			if (this.MaxSphereSizeForNoOverlap(this.bodyInitialRadius * this.scale, this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), false, out this.bodyMaxRadius))
			{
				if (this.scale > 0f)
				{
					this.bodyCollider.radius = this.bodyMaxRadius / this.scale;
				}
				if (Physics.SphereCast(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), this.bodyMaxRadius, Vector3.down, out this.bodyHitInfo, this.bodyInitialHeight * this.scale - this.bodyMaxRadius, this.locomotionEnabledLayers))
				{
					this.bodyCollider.height = (this.bodyHitInfo.distance + this.bodyMaxRadius) / this.scale;
				}
				else
				{
					this.bodyCollider.height = this.bodyInitialHeight;
				}
				if (!this.bodyCollider.gameObject.activeSelf)
				{
					this.bodyCollider.gameObject.SetActive(true);
				}
			}
			else
			{
				this.bodyCollider.gameObject.SetActive(false);
			}
			this.bodyCollider.height = Mathf.Lerp(this.bodyCollider.height, this.bodyInitialHeight, this.bodyLerp);
			this.bodyCollider.radius = Mathf.Lerp(this.bodyCollider.radius, this.bodyInitialRadius, this.bodyLerp);
			this.bodyOffsetVector = Vector3.down * this.bodyCollider.height / 2f;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector * this.scale;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x0005A3A4 File Offset: 0x000585A4
		private Vector3 GetCurrentHandPosition(Transform handTransform, Vector3 handOffset)
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(handTransform, handOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(handTransform, handOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(handTransform, handOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0005A481 File Offset: 0x00058681
		private Vector3 GetLastLeftHandPosition()
		{
			return this.lastLeftHandPosition + this.MovingSurfaceMovement();
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0005A494 File Offset: 0x00058694
		private Vector3 GetLastRightHandPosition()
		{
			return this.lastRightHandPosition + this.MovingSurfaceMovement();
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x0005A4A8 File Offset: 0x000586A8
		private Vector3 GetCurrentLeftHandPosition()
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x0005A5A4 File Offset: 0x000587A4
		private Vector3 GetCurrentRightHandPosition()
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x0005A69F File Offset: 0x0005889F
		private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
		{
			return transformToModify.position + transformToModify.rotation * offsetVector * this.scale;
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0005A6C4 File Offset: 0x000588C4
		private void LateUpdate()
		{
			float time = Time.time;
			Vector3 position = this.headCollider.transform.position;
			this.turnParent.transform.localScale = Vector3.one * this.scale;
			this.playerRigidBody.MovePosition(this.playerRigidBody.position + position - this.headCollider.transform.position);
			Camera.main.nearClipPlane = ((this.scale > 0.5f) ? 0.01f : 0.002f);
			Camera.main.farClipPlane = ((this.scale > 0.5f) ? 500f : 15f);
			this.debugLastRightHandPosition = this.lastRightHandPosition;
			this.debugPlatformDeltaPosition = this.MovingSurfaceMovement();
			this.rigidBodyMovement = Vector3.zero;
			this.leftHandPushDisplacement = Vector3.zero;
			this.rightHandPushDisplacement = Vector3.zero;
			if (this.debugMovement)
			{
				this.tempRealTime = Time.time;
				this.calcDeltaTime = Time.deltaTime;
				this.lastRealTime = this.tempRealTime;
			}
			else
			{
				this.tempRealTime = Time.realtimeSinceStartup;
				this.calcDeltaTime = this.tempRealTime - this.lastRealTime;
				this.lastRealTime = this.tempRealTime;
				if (this.calcDeltaTime > 0.1f)
				{
					this.calcDeltaTime = 0.05f;
				}
			}
			Vector3 a;
			if (this.lastFrameHasValidTouchPos && this.lastPlatformTouched != null && Player.ComputeWorldHitPoint(this.lastHitInfoHand, this.lastFrameTouchPosLocal, out a))
			{
				this.refMovement = a - this.lastFrameTouchPosWorld;
			}
			else
			{
				this.refMovement = Vector3.zero;
			}
			if (!this.didAJump && (this.wasLeftHandTouching || this.wasRightHandTouching))
			{
				base.transform.position = base.transform.position + 4.9f * Vector3.down * this.calcDeltaTime * this.calcDeltaTime * this.scale;
				if (Vector3.Dot(this.denormalizedVelocityAverage, this.slideAverageNormal) <= 0f && Vector3.Dot(Vector3.down, this.slideAverageNormal) <= 0f)
				{
					base.transform.position = base.transform.position - Vector3.Project(Mathf.Min(this.stickDepth * this.scale, Vector3.Project(this.denormalizedVelocityAverage, this.slideAverageNormal).magnitude * this.calcDeltaTime) * this.slideAverageNormal, Vector3.down);
				}
			}
			if (!this.didAJump && (this.wasLeftHandSlide || this.wasRightHandSlide))
			{
				base.transform.position = base.transform.position + this.slideAverage * this.calcDeltaTime;
				this.slideAverage += 9.8f * Vector3.down * this.calcDeltaTime;
			}
			this.FirstHandIteration(this.leftControllerTransform, this.leftHandOffset, this.GetLastLeftHandPosition(), this.wasLeftHandSlide, this.wasLeftHandTouching, out this.leftHandPushDisplacement, ref this.leftHandSlipPercentage, ref this.leftHandSlide, ref this.leftHandSlideNormal, ref this.leftHandColliding, ref this.leftHandMaterialTouchIndex, ref this.leftHandSurfaceOverride);
			this.leftHandColliding = (this.leftHandColliding && this.controllerState.LeftValid);
			this.leftHandSlide = (this.leftHandSlide && this.controllerState.LeftValid);
			this.FirstHandIteration(this.rightControllerTransform, this.rightHandOffset, this.GetLastRightHandPosition(), this.wasRightHandSlide, this.wasRightHandTouching, out this.rightHandPushDisplacement, ref this.rightHandSlipPercentage, ref this.rightHandSlide, ref this.rightHandSlideNormal, ref this.rightHandColliding, ref this.rightHandMaterialTouchIndex, ref this.rightHandSurfaceOverride);
			this.rightHandColliding = (this.rightHandColliding && this.controllerState.RightValid);
			this.rightHandSlide = (this.rightHandSlide && this.controllerState.RightValid);
			this.touchPoints = 0;
			this.rigidBodyMovement = Vector3.zero;
			if (this.leftHandColliding || this.wasLeftHandTouching)
			{
				this.rigidBodyMovement += this.leftHandPushDisplacement;
				this.touchPoints++;
			}
			if (this.rightHandColliding || this.wasRightHandTouching)
			{
				this.rigidBodyMovement += this.rightHandPushDisplacement;
				this.touchPoints++;
			}
			if (this.touchPoints != 0)
			{
				this.rigidBodyMovement /= (float)this.touchPoints;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition, true, out this.maxSphereSize1) && !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
			}
			if (this.IterativeCollisionSphereCast(this.lastHeadPosition, this.headCollider.radius * 0.9f * this.scale, this.headCollider.transform.position + this.rigidBodyMovement - this.lastHeadPosition, out this.finalPosition, false, out this.slipPercentage, out this.junkHit, true))
			{
				this.rigidBodyMovement = this.finalPosition - this.headCollider.transform.position;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition + this.rigidBodyMovement, true, out this.maxSphereSize1) || !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition + this.rigidBodyMovement))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
				this.rigidBodyMovement = this.lastHeadPosition - this.headCollider.transform.position;
			}
			else if (this.headCollider.radius * 0.9f * 0.825f * this.scale < this.maxSphereSize1)
			{
				this.lastOpenHeadPosition = this.headCollider.transform.position + this.rigidBodyMovement;
			}
			if (this.rigidBodyMovement != Vector3.zero)
			{
				base.transform.position += this.rigidBodyMovement;
			}
			this.lastHeadPosition = this.headCollider.transform.position;
			this.areBothTouching = ((!this.leftHandColliding && !this.wasLeftHandTouching) || (!this.rightHandColliding && !this.wasRightHandTouching));
			Vector3 vector = this.FinalHandPosition(this.leftControllerTransform, this.leftHandOffset, this.GetLastLeftHandPosition(), this.areBothTouching, this.leftHandColliding, out this.leftHandColliding, this.leftHandSlide, out this.leftHandSlide, this.leftHandMaterialTouchIndex, out this.leftHandMaterialTouchIndex, this.leftHandSurfaceOverride, out this.leftHandSurfaceOverride);
			this.leftHandColliding = (this.leftHandColliding && this.controllerState.LeftValid);
			this.leftHandSlide = (this.leftHandSlide && this.controllerState.LeftValid);
			Vector3 vector2 = this.FinalHandPosition(this.rightControllerTransform, this.rightHandOffset, this.GetLastRightHandPosition(), this.areBothTouching, this.rightHandColliding, out this.rightHandColliding, this.rightHandSlide, out this.rightHandSlide, this.rightHandMaterialTouchIndex, out this.rightHandMaterialTouchIndex, this.rightHandSurfaceOverride, out this.rightHandSurfaceOverride);
			this.rightHandColliding = (this.rightHandColliding && this.controllerState.RightValid);
			this.rightHandSlide = (this.rightHandSlide && this.controllerState.RightValid);
			this.StoreVelocities();
			this.didAJump = false;
			if (this.OverrideSlipToMax())
			{
				this.didAJump = true;
			}
			else if (this.rightHandSlide || this.leftHandSlide)
			{
				this.slideAverageNormal = Vector3.zero;
				this.touchPoints = 0;
				this.averageSlipPercentage = 0f;
				if (this.leftHandSlide)
				{
					this.slideAverageNormal += this.leftHandSlideNormal.normalized;
					this.averageSlipPercentage += this.leftHandSlipPercentage;
					this.touchPoints++;
				}
				if (this.rightHandSlide)
				{
					this.slideAverageNormal += this.rightHandSlideNormal.normalized;
					this.averageSlipPercentage += this.rightHandSlipPercentage;
					this.touchPoints++;
				}
				this.slideAverageNormal = this.slideAverageNormal.normalized;
				this.averageSlipPercentage /= (float)this.touchPoints;
				if (this.touchPoints == 1)
				{
					this.surfaceDirection = (this.rightHandSlide ? Vector3.ProjectOnPlane(this.rightControllerTransform.forward, this.rightHandSlideNormal) : Vector3.ProjectOnPlane(this.leftControllerTransform.forward, this.leftHandSlideNormal));
					if (Vector3.Dot(this.slideAverage, this.surfaceDirection) > 0f)
					{
						this.slideAverage = Vector3.Project(this.slideAverage, Vector3.Slerp(this.slideAverage, this.surfaceDirection.normalized * this.slideAverage.magnitude, this.slideControl));
					}
					else
					{
						this.slideAverage = Vector3.Project(this.slideAverage, Vector3.Slerp(this.slideAverage, -this.surfaceDirection.normalized * this.slideAverage.magnitude, this.slideControl));
					}
				}
				if (!this.wasLeftHandSlide && !this.wasRightHandSlide)
				{
					this.slideAverage = ((Vector3.Dot(this.playerRigidBody.velocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.playerRigidBody.velocity, this.slideAverageNormal) : this.playerRigidBody.velocity);
				}
				else
				{
					this.slideAverage = ((Vector3.Dot(this.slideAverage, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideAverage, this.slideAverageNormal) : this.slideAverage);
				}
				this.slideAverage = this.slideAverage.normalized * Mathf.Min(this.slideAverage.magnitude, Mathf.Max(0.5f, this.denormalizedVelocityAverage.magnitude * 2f));
				this.playerRigidBody.velocity = Vector3.zero;
			}
			else if (this.leftHandColliding || this.rightHandColliding)
			{
				if (!this.turnedThisFrame)
				{
					this.playerRigidBody.velocity = Vector3.zero;
				}
				else
				{
					this.playerRigidBody.velocity = this.playerRigidBody.velocity.normalized * Mathf.Min(2f, this.playerRigidBody.velocity.magnitude);
				}
			}
			else if (this.wasLeftHandSlide || this.wasRightHandSlide)
			{
				this.playerRigidBody.velocity = ((Vector3.Dot(this.slideAverage, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideAverage, this.slideAverageNormal) : this.slideAverage);
			}
			if ((this.rightHandColliding || this.leftHandColliding) && !this.disableMovement && !this.turnedThisFrame && !this.didAJump)
			{
				if (this.rightHandSlide || this.leftHandSlide)
				{
					if (Vector3.Project(this.denormalizedVelocityAverage, this.slideAverageNormal).magnitude > this.slideVelocityLimit && Vector3.Dot(this.denormalizedVelocityAverage, this.slideAverageNormal) > 0f && Vector3.Project(this.denormalizedVelocityAverage, this.slideAverageNormal).magnitude > Vector3.Project(this.slideAverage, this.slideAverageNormal).magnitude)
					{
						this.leftHandSlide = false;
						this.rightHandSlide = false;
						this.didAJump = true;
						this.playerRigidBody.velocity = Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * Vector3.Project(this.denormalizedVelocityAverage, this.slideAverageNormal).magnitude) * this.slideAverageNormal.normalized + Vector3.ProjectOnPlane(this.slideAverage, this.slideAverageNormal);
					}
				}
				else if (this.denormalizedVelocityAverage.magnitude > this.velocityLimit * this.scale)
				{
					float num = (this.InWater && this.CurrentWaterVolume != null) ? this.liquidPropertiesList[(int)this.CurrentWaterVolume.LiquidType].surfaceJumpFactor : 1f;
					Vector3 vector3 = Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * num * this.denormalizedVelocityAverage.magnitude) * this.denormalizedVelocityAverage.normalized;
					this.didAJump = true;
					this.playerRigidBody.velocity = vector3;
					if (this.InWater)
					{
						this.swimmingVelocity += vector3 * this.swimmingParams.underwaterJumpsAsSwimVelocityFactor;
					}
				}
			}
			if (!this.controllerState.LeftValid || (this.leftHandColliding && (this.GetCurrentLeftHandPosition() - this.GetLastLeftHandPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (this.GetCurrentLeftHandPosition() - this.headCollider.transform.position).normalized, out this.hitInfo, (this.GetCurrentLeftHandPosition() - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)))
			{
				vector = this.GetCurrentLeftHandPosition();
				this.leftHandColliding = false;
			}
			if (!this.controllerState.RightValid || (this.rightHandColliding && (this.GetCurrentRightHandPosition() - this.GetLastRightHandPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (this.GetCurrentRightHandPosition() - this.headCollider.transform.position).normalized, out this.hitInfo, (this.GetCurrentRightHandPosition() - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)))
			{
				vector2 = this.GetCurrentRightHandPosition();
				this.rightHandColliding = false;
			}
			if (this.currentPlatform == null)
			{
				this.playerRigidBody.velocity += this.refMovement / this.calcDeltaTime;
				this.refMovement = Vector3.zero;
			}
			Vector3 vector4 = Vector3.zero;
			float a2 = 0f;
			Vector3 b;
			if (this.GetSwimmingVelocityForHand(this.lastLeftHandPosition, vector, this.leftControllerTransform.right, this.calcDeltaTime, ref this.leftHandWaterVolume, ref this.leftHandWaterSurface, out b))
			{
				a2 = Mathf.InverseLerp(0f, 0.2f, b.magnitude) * this.swimmingParams.swimmingHapticsStrength;
				vector4 += b;
			}
			float a3 = 0f;
			Vector3 b2;
			if (this.GetSwimmingVelocityForHand(this.lastRightHandPosition, vector2, -this.rightControllerTransform.right, this.calcDeltaTime, ref this.rightHandWaterVolume, ref this.rightHandWaterSurface, out b2))
			{
				a3 = Mathf.InverseLerp(0f, 0.15f, b2.magnitude) * this.swimmingParams.swimmingHapticsStrength;
				vector4 += b2;
			}
			Vector3 vector5 = Vector3.zero;
			Vector3 b3;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.lastLeftHandPosition, vector, this.leftControllerTransform.right, this.leftHandCenterVelocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.leftHandWaterVolume, this.leftHandWaterSurface, out b3))
			{
				if (time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown)
				{
					vector5 += b3;
				}
				this.lastWaterSurfaceJumpTimeLeft = Time.time;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			Vector3 b4;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.lastRightHandPosition, vector2, -this.rightControllerTransform.right, this.rightHandCenterVelocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.rightHandWaterVolume, this.rightHandWaterSurface, out b4))
			{
				if (time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown)
				{
					vector5 += b4;
				}
				this.lastWaterSurfaceJumpTimeRight = Time.time;
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			vector5 = Vector3.ClampMagnitude(vector5, this.swimmingParams.waterSurfaceJumpMaxSpeed * this.scale);
			float num2 = Mathf.Max(a2, this.leftHandNonDiveHapticsAmount);
			if (num2 > 0.001f && time - this.lastWaterSurfaceJumpTimeLeft > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num2, this.calcDeltaTime);
			}
			float num3 = Mathf.Max(a3, this.rightHandNonDiveHapticsAmount);
			if (num3 > 0.001f && time - this.lastWaterSurfaceJumpTimeRight > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.RightHand, num3, this.calcDeltaTime);
			}
			if (!this.disableMovement)
			{
				this.swimmingVelocity += vector4;
				this.playerRigidBody.velocity += vector4 + vector5;
			}
			else
			{
				this.swimmingVelocity = Vector3.zero;
			}
			if (this.isClimbing && (this.inOverlay || this.climbHelper == null || this.currentClimbable == null || !this.currentClimbable.isActiveAndEnabled))
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			Vector3 b5 = Vector3.zero;
			if (this.isClimbing)
			{
				b5 = this.currentClimber.transform.position - this.climbHelper.position;
				if (b5.magnitude > 1f)
				{
					this.EndClimbing(this.currentClimber, false, false);
				}
			}
			if (this.isClimbing)
			{
				this.playerRigidBody.velocity = Vector3.zero;
				this.climbHelper.localPosition = Vector3.Lerp(this.climbHelper.localPosition, this.climbHelperTargetPos, Time.deltaTime * 12f);
				this.playerRigidBody.MovePosition(this.playerRigidBody.position - b5);
				if (this.currentSwing)
				{
					this.currentSwing.lastGrabTime = Time.time;
				}
			}
			this.leftHandFollower.position = vector;
			this.rightHandFollower.position = vector2;
			this.leftHandFollower.rotation = this.leftControllerTransform.rotation * this.leftHandRotOffset;
			this.rightHandFollower.rotation = this.rightControllerTransform.rotation * this.rightHandRotOffset;
			this.wasLeftHandTouching = this.leftHandColliding;
			this.wasRightHandTouching = this.rightHandColliding;
			this.wasLeftHandSlide = this.leftHandSlide;
			this.wasRightHandSlide = this.rightHandSlide;
			if ((this.leftHandColliding && !this.leftHandSlide) || (this.rightHandColliding && !this.rightHandSlide))
			{
				this.lastTouchedGroundTimestamp = Time.time;
			}
			this.degreesTurnedThisFrame = 0f;
			this.lastPlatformTouched = this.currentPlatform;
			this.currentPlatform = null;
			this.lastLeftHandPosition = vector;
			this.lastRightHandPosition = vector2;
			Vector3 vector6;
			if (Player.ComputeLocalHitPoint(this.lastHitInfoHand, out vector6))
			{
				this.lastFrameHasValidTouchPos = true;
				this.lastFrameTouchPosLocal = vector6;
				this.lastFrameTouchPosWorld = this.lastHitInfoHand.point;
			}
			else
			{
				this.lastFrameHasValidTouchPos = false;
				this.lastFrameTouchPosLocal = Vector3.zero;
				this.lastFrameTouchPosWorld = Vector3.zero;
			}
			this.lastRigidbodyPosition = this.playerRigidBody.transform.position;
			this.BodyCollider();
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x0005BBC0 File Offset: 0x00059DC0
		private Vector3 FirstHandIteration(Transform handTransform, Vector3 handOffset, Vector3 lastHandPosition, bool wasHandSlide, bool wasHandTouching, out Vector3 pushDisplacement, ref float handSlipPercentage, ref bool handSlide, ref Vector3 slideNormal, ref bool handColliding, ref int materialTouchIndex, ref GorillaSurfaceOverride touchedOverride)
		{
			Vector3 currentHandPosition = this.GetCurrentHandPosition(handTransform, handOffset);
			Vector3 vector = currentHandPosition;
			this.distanceTraveled = currentHandPosition - lastHandPosition;
			if (!this.didAJump && wasHandSlide && Vector3.Dot(slideNormal, Vector3.up) > 0f)
			{
				this.distanceTraveled += Vector3.Project(-this.slideAverageNormal * this.stickDepth * this.scale, Vector3.down);
			}
			if (this.IterativeCollisionSphereCast(lastHandPosition, this.minimumRaycastDistance * this.scale, this.distanceTraveled, out this.finalPosition, true, out this.slipPercentage, out this.tempHitInfo, false) && !this.InReportMenu)
			{
				if (wasHandTouching && this.slipPercentage <= this.defaultSlideFactor)
				{
					vector = lastHandPosition;
				}
				else
				{
					vector = this.finalPosition;
				}
				pushDisplacement = vector - currentHandPosition;
				handSlipPercentage = this.slipPercentage;
				handSlide = (this.slipPercentage > this.iceThreshold);
				slideNormal = this.tempHitInfo.normal;
				handColliding = true;
				materialTouchIndex = this.currentMaterialIndex;
				touchedOverride = this.currentOverride;
				this.lastHitInfoHand = this.tempHitInfo;
			}
			else
			{
				pushDisplacement = Vector3.zero;
				handSlipPercentage = 0f;
				handSlide = false;
				slideNormal = Vector3.up;
				handColliding = false;
				materialTouchIndex = 0;
				touchedOverride = null;
			}
			return vector;
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x0005BD30 File Offset: 0x00059F30
		private Vector3 FinalHandPosition(Transform handTransform, Vector3 handOffset, Vector3 lastHandPosition, bool bothTouching, bool isHandTouching, out bool handColliding, bool isHandSlide, out bool handSlide, int currentMaterialTouchIndex, out int materialTouchIndex, GorillaSurfaceOverride currentSurface, out GorillaSurfaceOverride touchedOverride)
		{
			handColliding = isHandTouching;
			handSlide = isHandSlide;
			materialTouchIndex = currentMaterialTouchIndex;
			touchedOverride = currentSurface;
			this.distanceTraveled = this.GetCurrentHandPosition(handTransform, handOffset) - lastHandPosition;
			if (this.IterativeCollisionSphereCast(lastHandPosition, this.minimumRaycastDistance * this.scale, this.distanceTraveled, out this.finalPosition, bothTouching, out this.slipPercentage, out this.junkHit, false))
			{
				handColliding = true;
				handSlide = (this.slipPercentage > this.iceThreshold);
				materialTouchIndex = this.currentMaterialIndex;
				touchedOverride = this.currentOverride;
				this.lastHitInfoHand = this.junkHit;
				return this.finalPosition;
			}
			return this.GetCurrentHandPosition(handTransform, handOffset);
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0005BDD8 File Offset: 0x00059FD8
		private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 endPosition, bool singleHand, out float slipPercentage, out RaycastHit iterativeHitInfo, bool fullSlide)
		{
			slipPercentage = this.defaultSlideFactor;
			if (!this.CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				endPosition = Vector3.zero;
				return false;
			}
			this.firstPosition = endPosition;
			iterativeHitInfo = this.tempIterativeHit;
			this.slideFactor = this.GetSlidePercentage(iterativeHitInfo);
			slipPercentage = ((this.slideFactor != this.defaultSlideFactor) ? this.slideFactor : ((!singleHand) ? this.defaultSlideFactor : 0.001f));
			if (fullSlide || this.OverrideSlipToMax())
			{
				slipPercentage = 1f;
			}
			this.movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - this.firstPosition, iterativeHitInfo.normal) * slipPercentage;
			if (this.CollisionsSphereCast(this.firstPosition, sphereRadius, this.movementToProjectedAboveCollisionPlane, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			if (this.CollisionsSphereCast(this.movementToProjectedAboveCollisionPlane + this.firstPosition, sphereRadius, startPosition + movementVector - (this.movementToProjectedAboveCollisionPlane + this.firstPosition), out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			endPosition = Vector3.zero;
			return false;
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x0005BF40 File Offset: 0x0005A140
		private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 finalPosition, out RaycastHit collisionsHitInfo)
		{
			this.MaxSphereSizeForNoOverlap(sphereRadius, startPosition, false, out this.maxSphereSize1);
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.SphereCastNonAlloc(startPosition, this.maxSphereSize1, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int i = 0; i < this.bufferCount; i++)
				{
					if (this.rayCastNonAllocColliders[i].distance < this.tempHitInfo.distance)
					{
						this.tempHitInfo = this.rayCastNonAllocColliders[i];
					}
				}
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int j = 0; j < this.bufferCount; j++)
					{
						if (this.rayCastNonAllocColliders[j].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[j];
						}
					}
					finalPosition = startPosition + movementVector.normalized * this.tempHitInfo.distance;
				}
				this.MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, false, out this.maxSphereSize2);
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.SphereCastNonAlloc(startPosition, Mathf.Min(this.maxSphereSize1, this.maxSphereSize2), (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int k = 0; k < this.bufferCount; k++)
					{
						if (this.rayCastNonAllocColliders[k].collider != null && this.rayCastNonAllocColliders[k].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[k];
						}
					}
					finalPosition = startPosition + this.tempHitInfo.distance * (finalPosition - startPosition).normalized;
					collisionsHitInfo = this.tempHitInfo;
				}
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int l = 0; l < this.bufferCount; l++)
					{
						if (this.rayCastNonAllocColliders[l].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[l];
						}
					}
					collisionsHitInfo = this.tempHitInfo;
					finalPosition = startPosition;
				}
				return true;
			}
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.RaycastNonAlloc(startPosition, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int m = 0; m < this.bufferCount; m++)
				{
					if (this.rayCastNonAllocColliders[m].collider != null && this.rayCastNonAllocColliders[m].distance < this.tempHitInfo.distance)
					{
						this.tempHitInfo = this.rayCastNonAllocColliders[m];
					}
				}
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = startPosition;
				return true;
			}
			finalPosition = startPosition + movementVector;
			collisionsHitInfo = default(RaycastHit);
			return false;
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x0005C3DB File Offset: 0x0005A5DB
		public bool IsHandTouching(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return this.wasLeftHandTouching;
			}
			return this.wasRightHandTouching;
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x0005C3ED File Offset: 0x0005A5ED
		public bool IsHandSliding(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return this.wasLeftHandSlide || this.leftHandSlide;
			}
			return this.wasRightHandSlide || this.rightHandSlide;
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x0005C414 File Offset: 0x0005A614
		public float GetSlidePercentage(RaycastHit raycastHit)
		{
			this.currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
			BasePlatform component = raycastHit.collider.gameObject.GetComponent<BasePlatform>();
			if (component != null)
			{
				this.currentPlatform = component;
			}
			if (this.currentOverride != null)
			{
				this.currentMaterialIndex = this.currentOverride.overrideIndex;
				if (!this.materialData[this.currentMaterialIndex].overrideSlidePercent)
				{
					return this.defaultSlideFactor;
				}
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			else
			{
				this.meshCollider = (raycastHit.collider as MeshCollider);
				if (this.meshCollider == null || this.meshCollider.sharedMesh == null || this.meshCollider.convex)
				{
					return this.defaultSlideFactor;
				}
				this.collidedMesh = this.meshCollider.sharedMesh;
				if (!this.meshTrianglesDict.TryGetValue(this.collidedMesh, out this.sharedMeshTris))
				{
					this.sharedMeshTris = this.collidedMesh.triangles;
					this.meshTrianglesDict.Add(this.collidedMesh, (int[])this.sharedMeshTris.Clone());
				}
				this.vertex1 = this.sharedMeshTris[raycastHit.triangleIndex * 3];
				this.vertex2 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
				this.vertex3 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
				this.slideRenderer = raycastHit.collider.GetComponent<Renderer>();
				this.slideRenderer.GetSharedMaterials(this.tempMaterialArray);
				if (this.tempMaterialArray.Count > 1)
				{
					for (int i = 0; i < this.tempMaterialArray.Count; i++)
					{
						this.collidedMesh.GetTriangles(this.trianglesList, i);
						int j = 0;
						while (j < this.trianglesList.Count)
						{
							if (this.trianglesList[j] == this.vertex1 && this.trianglesList[j + 1] == this.vertex2 && this.trianglesList[j + 2] == this.vertex3)
							{
								this.findMatName = this.tempMaterialArray[i].name;
								if (this.findMatName.EndsWith("Uber"))
								{
									string text = this.findMatName;
									this.findMatName = text.Substring(0, text.Length - 4);
								}
								this.foundMatData = this.materialData.Find((Player.MaterialData matData) => matData.matName == this.findMatName);
								this.currentMaterialIndex = this.materialData.FindIndex((Player.MaterialData matData) => matData.matName == this.findMatName);
								if (this.currentMaterialIndex == -1)
								{
									this.currentMaterialIndex = 0;
								}
								if (!this.foundMatData.overrideSlidePercent)
								{
									return this.defaultSlideFactor;
								}
								return this.foundMatData.slidePercent;
							}
							else
							{
								j += 3;
							}
						}
					}
					this.currentMaterialIndex = 0;
					return this.defaultSlideFactor;
				}
				this.findMatName = this.tempMaterialArray[0].name;
				if (this.findMatName.EndsWith("Uber"))
				{
					string text = this.findMatName;
					this.findMatName = text.Substring(0, text.Length - 4);
				}
				this.foundMatData = this.materialData.Find((Player.MaterialData matData) => matData.matName == this.findMatName);
				this.currentMaterialIndex = this.materialData.FindIndex((Player.MaterialData matData) => matData.matName == this.findMatName);
				if (this.currentMaterialIndex == -1)
				{
					this.currentMaterialIndex = 0;
				}
				if (!this.foundMatData.overrideSlidePercent)
				{
					return this.defaultSlideFactor;
				}
				return this.foundMatData.slidePercent;
			}
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x0005C7D0 File Offset: 0x0005A9D0
		public void Turn(float degrees)
		{
			this.turnParent.transform.RotateAround(this.headCollider.transform.position, base.transform.up, degrees);
			this.degreesTurnedThisFrame = degrees;
			this.denormalizedVelocityAverage = Vector3.zero;
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Quaternion.Euler(0f, degrees, 0f) * this.velocityHistory[i];
				this.denormalizedVelocityAverage += this.velocityHistory[i];
			}
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x0005C878 File Offset: 0x0005AA78
		public void BeginClimbing(GorillaClimbable climbable, GorillaHandClimber hand, GorillaClimbableRef climbableRef = null)
		{
			if (this.currentClimber != null)
			{
				this.EndClimbing(this.currentClimber, true, false);
			}
			try
			{
				Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb = climbable.onBeforeClimb;
				if (onBeforeClimb != null)
				{
					onBeforeClimb(hand, climbableRef);
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			Rigidbody rigidbody;
			climbable.TryGetComponent<Rigidbody>(out rigidbody);
			this.VerifyClimbHelper();
			this.climbHelper.SetParent(climbable.transform);
			this.climbHelper.position = hand.transform.position;
			Vector3 localPosition = this.climbHelper.localPosition;
			if (climbable.snapX)
			{
				Player.<BeginClimbing>g__SnapAxis|245_0(ref localPosition.x, climbable.maxDistanceSnap);
			}
			if (climbable.snapY)
			{
				Player.<BeginClimbing>g__SnapAxis|245_0(ref localPosition.y, climbable.maxDistanceSnap);
			}
			if (climbable.snapZ)
			{
				Player.<BeginClimbing>g__SnapAxis|245_0(ref localPosition.z, climbable.maxDistanceSnap);
			}
			this.climbHelperTargetPos = localPosition;
			climbable.isBeingClimbed = true;
			hand.isClimbing = true;
			this.currentClimbable = climbable;
			this.currentClimber = hand;
			this.isClimbing = true;
			GorillaRopeSegment gorillaRopeSegment;
			GorillaZipline gorillaZipline;
			NoncontrollableBroomstick noncontrollableBroomstick;
			if (climbable.TryGetComponent<GorillaRopeSegment>(out gorillaRopeSegment) && gorillaRopeSegment.swing)
			{
				this.currentSwing = gorillaRopeSegment.swing;
				this.currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, this.climbHelperTargetPos, this.currentVelocity);
			}
			else if (climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(out gorillaZipline))
			{
				this.currentZipline = gorillaZipline;
			}
			else if (climbable.TryGetComponent<NoncontrollableBroomstick>(out noncontrollableBroomstick))
			{
				noncontrollableBroomstick.OnGrabbed();
			}
			this.playerRigidBody.useGravity = false;
			GorillaTagger.Instance.StartVibration(this.currentClimber.xrNode == XRNode.LeftHand, 0.6f, 0.06f);
			if (climbable.clip)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == XRNode.LeftHand);
			}
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x0005CA6C File Offset: 0x0005AC6C
		private void VerifyClimbHelper()
		{
			if (this.climbHelper == null || this.climbHelper.gameObject == null)
			{
				this.climbHelper = new GameObject("Climb Helper").transform;
			}
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x0005CAA4 File Offset: 0x0005ACA4
		public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
		{
			if (hand != this.currentClimber)
			{
				return;
			}
			if (!startingNewClimb)
			{
				this.playerRigidBody.useGravity = true;
			}
			Rigidbody rigidbody = null;
			if (this.currentClimbable)
			{
				this.currentClimbable.TryGetComponent<Rigidbody>(out rigidbody);
				this.currentClimbable.isBeingClimbed = false;
				NoncontrollableBroomstick noncontrollableBroomstick;
				if (this.currentClimbable.TryGetComponent<NoncontrollableBroomstick>(out noncontrollableBroomstick))
				{
					noncontrollableBroomstick.OnGrabReleased();
				}
			}
			Vector3 vector = Vector3.zero;
			if (this.currentClimber)
			{
				this.currentClimber.isClimbing = false;
				if (doDontReclimb)
				{
					this.currentClimber.dontReclimbLast = this.currentClimbable;
				}
				else
				{
					this.currentClimber.dontReclimbLast = null;
				}
				this.currentClimber.queuedToBecomeValidToGrabAgain = true;
				this.currentClimber.lastAutoReleasePos = this.currentClimber.handRoot.localPosition;
				if (!startingNewClimb && this.currentClimbable)
				{
					GorillaVelocityTracker gorillaVelocityTracker;
					if (this.currentClimber.xrNode == XRNode.LeftHand)
					{
						gorillaVelocityTracker = this.leftInteractPointVelocityTracker;
					}
					else
					{
						gorillaVelocityTracker = this.rightInteractPointVelocityTracker;
					}
					if (rigidbody)
					{
						this.playerRigidBody.velocity = rigidbody.velocity;
					}
					else if (this.currentSwing)
					{
						this.playerRigidBody.velocity = this.currentSwing.velocityTracker.GetAverageVelocity(true, 0.25f, false);
					}
					else if (this.currentZipline)
					{
						this.playerRigidBody.velocity = this.currentZipline.GetCurrentDirection() * this.currentZipline.currentSpeed;
					}
					else
					{
						this.playerRigidBody.velocity = this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false) * 0.7f;
					}
					vector = this.turnParent.transform.rotation * -gorillaVelocityTracker.GetAverageVelocity(false, 0.1f, true) * this.scale;
					vector = Vector3.ClampMagnitude(vector, 5.5f * this.scale);
					this.playerRigidBody.AddForce(vector, ForceMode.VelocityChange);
				}
			}
			if (this.currentSwing)
			{
				this.currentSwing.DetachLocalPlayer();
			}
			if (!startingNewClimb && vector.magnitude > 2f && this.currentClimbable && this.currentClimbable.clipOnFullRelease)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(this.currentClimbable.clipOnFullRelease, hand.xrNode == XRNode.LeftHand);
			}
			this.currentClimbable = null;
			this.currentClimber = null;
			this.currentSwing = null;
			this.currentZipline = null;
			this.isClimbing = false;
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x0005CD38 File Offset: 0x0005AF38
		private void StoreVelocities()
		{
			this.velocityIndex = (this.velocityIndex + 1) % this.velocityHistorySize;
			this.currentVelocity = (base.transform.position - this.lastPosition - this.MovingSurfaceMovement()) / this.calcDeltaTime;
			this.velocityHistory[this.velocityIndex] = this.currentVelocity;
			this.denormalizedVelocityAverage = Vector3.zero;
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.denormalizedVelocityAverage += this.velocityHistory[i];
			}
			this.denormalizedVelocityAverage /= (float)this.velocityHistorySize;
			this.lastPosition = base.transform.position;
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0005CE08 File Offset: 0x0005B008
		private void AntiTeleportTechnology()
		{
			if ((this.headCollider.transform.position - this.lastHeadPosition).magnitude >= this.teleportThresholdNoVel + this.playerRigidBody.velocity.magnitude * this.calcDeltaTime)
			{
				base.transform.position = base.transform.position + this.lastHeadPosition - this.headCollider.transform.position;
			}
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x0005CE94 File Offset: 0x0005B094
		private bool MaxSphereSizeForNoOverlap(float testRadius, Vector3 checkPosition, bool ignoreOneWay, out float overlapRadiusTest)
		{
			overlapRadiusTest = testRadius;
			this.overlapAttempts = 0;
			int num = 100;
			while (this.overlapAttempts < num && overlapRadiusTest > testRadius * 0.75f)
			{
				this.ClearColliderBuffer(ref this.overlapColliders);
				this.bufferCount = Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, this.overlapColliders, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (ignoreOneWay)
				{
					int num2 = 0;
					for (int i = 0; i < this.bufferCount; i++)
					{
						if (this.overlapColliders[i].CompareTag("NoCrazyCheck"))
						{
							num2++;
						}
					}
					if (num2 == this.bufferCount)
					{
						return true;
					}
				}
				if (this.bufferCount <= 0)
				{
					overlapRadiusTest *= 0.995f;
					return true;
				}
				overlapRadiusTest = Mathf.Lerp(testRadius, 0f, (float)this.overlapAttempts / (float)num);
				this.overlapAttempts++;
			}
			return false;
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x0005CF74 File Offset: 0x0005B174
		private bool CrazyCheck2(float sphereSize, Vector3 startPosition)
		{
			for (int i = 0; i < this.crazyCheckVectors.Length; i++)
			{
				if (this.NonAllocRaycast(startPosition, startPosition + this.crazyCheckVectors[i] * sphereSize) > 0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x0005CFBC File Offset: 0x0005B1BC
		private int NonAllocRaycast(Vector3 startPosition, Vector3 endPosition)
		{
			Vector3 direction = endPosition - startPosition;
			int num = Physics.RaycastNonAlloc(startPosition, direction, this.rayCastNonAllocColliders, direction.magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!this.rayCastNonAllocColliders[i].collider.gameObject.CompareTag("NoCrazyCheck"))
				{
					num2++;
				}
			}
			return num2;
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x0005D028 File Offset: 0x0005B228
		private void ClearColliderBuffer(ref Collider[] colliders)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i] = null;
			}
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x0005D04C File Offset: 0x0005B24C
		private void ClearRaycasthitBuffer(ref RaycastHit[] raycastHits)
		{
			for (int i = 0; i < raycastHits.Length; i++)
			{
				raycastHits[i] = this.emptyHit;
			}
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x0005D076 File Offset: 0x0005B276
		private Vector3 MovingSurfaceMovement()
		{
			return this.refMovement;
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x0005D080 File Offset: 0x0005B280
		private static bool ComputeLocalHitPoint(RaycastHit hit, out Vector3 localHitPoint)
		{
			if (hit.collider == null || hit.point.sqrMagnitude < 0.001f)
			{
				localHitPoint = Vector3.zero;
				return false;
			}
			localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
			return true;
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x0005D0DE File Offset: 0x0005B2DE
		private static bool ComputeWorldHitPoint(RaycastHit hit, Vector3 localPoint, out Vector3 worldHitPoint)
		{
			if (hit.collider == null)
			{
				worldHitPoint = Vector3.zero;
				return false;
			}
			worldHitPoint = hit.collider.transform.TransformPoint(localPoint);
			return true;
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x0005D118 File Offset: 0x0005B318
		private float ExtraVelMultiplier()
		{
			float num = 1f;
			if (this.leftHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHandSurfaceOverride.extraVelMultiplier);
			}
			if (this.rightHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHandSurfaceOverride.extraVelMultiplier);
			}
			return num;
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x0005D16C File Offset: 0x0005B36C
		private float ExtraVelMaxMultiplier()
		{
			float num = 1f;
			if (this.leftHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHandSurfaceOverride.extraVelMaxMultiplier);
			}
			if (this.rightHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHandSurfaceOverride.extraVelMaxMultiplier);
			}
			return num * this.scale;
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x0005D1C9 File Offset: 0x0005B3C9
		public void SetMaximumSlipThisFrame()
		{
			this.disableGripFrameIdx = Time.frameCount;
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x0005D1D6 File Offset: 0x0005B3D6
		public bool OverrideSlipToMax()
		{
			return this.disableGripFrameIdx == Time.frameCount;
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x0005D1E8 File Offset: 0x0005B3E8
		public void OnEnterWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (playerCollider == this.headCollider)
			{
				if (!this.headOverlappingWaterVolumes.Contains(volume))
				{
					this.headOverlappingWaterVolumes.Add(volume);
					return;
				}
			}
			else if (playerCollider == this.bodyCollider && !this.bodyOverlappingWaterVolumes.Contains(volume))
			{
				this.bodyOverlappingWaterVolumes.Add(volume);
			}
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x0005D246 File Offset: 0x0005B446
		public void OnExitWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (playerCollider == this.headCollider)
			{
				this.headOverlappingWaterVolumes.Remove(volume);
				return;
			}
			if (playerCollider == this.bodyCollider)
			{
				this.bodyOverlappingWaterVolumes.Remove(volume);
			}
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x0005D280 File Offset: 0x0005B480
		private bool GetSwimmingVelocityForHand(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, float dt, ref WaterVolume contactingWaterVolume, ref WaterVolume.SurfaceQuery waterSurface, out Vector3 swimmingVelocityChange)
		{
			contactingWaterVolume = null;
			this.bufferCount = Physics.OverlapSphereNonAlloc(endingHandPosition, this.minimumRaycastDistance, this.overlapColliders, this.waterLayer.value, QueryTriggerInteraction.Collide);
			if (this.bufferCount > 0)
			{
				float minValue = float.MinValue;
				for (int i = 0; i < this.bufferCount; i++)
				{
					WaterVolume component = this.overlapColliders[i].GetComponent<WaterVolume>();
					WaterVolume.SurfaceQuery surfaceQuery;
					if (component != null && component.GetSurfaceQueryForPoint(endingHandPosition, out surfaceQuery, false) && surfaceQuery.surfacePoint.y > minValue)
					{
						contactingWaterVolume = component;
						waterSurface = surfaceQuery;
					}
				}
			}
			if (contactingWaterVolume != null)
			{
				Vector3 a = endingHandPosition - startingHandPosition;
				Vector3 b = Vector3.zero;
				Vector3 b2 = this.playerRigidBody.transform.position - this.lastRigidbodyPosition;
				if (this.turnedThisFrame)
				{
					Vector3 vector = startingHandPosition - this.headCollider.transform.position;
					b = Quaternion.AngleAxis(this.degreesTurnedThisFrame, Vector3.up) * vector - vector;
				}
				float num = Vector3.Dot(a - b - b2, palmForwardDirection);
				float num2 = 0f;
				if (num > 0f)
				{
					Plane surfacePlane = waterSurface.surfacePlane;
					float distanceToPoint = surfacePlane.GetDistanceToPoint(startingHandPosition);
					float distanceToPoint2 = surfacePlane.GetDistanceToPoint(endingHandPosition);
					if (distanceToPoint <= 0f && distanceToPoint2 <= 0f)
					{
						num2 = 1f;
					}
					else if (distanceToPoint > 0f && distanceToPoint2 <= 0f)
					{
						num2 = -distanceToPoint2 / (distanceToPoint - distanceToPoint2);
					}
					else if (distanceToPoint <= 0f && distanceToPoint2 > 0f)
					{
						num2 = -distanceToPoint / (distanceToPoint2 - distanceToPoint);
					}
					if (num2 > Mathf.Epsilon)
					{
						float resistance = this.liquidPropertiesList[(int)contactingWaterVolume.LiquidType].resistance;
						swimmingVelocityChange = -palmForwardDirection * num * 2f * resistance * num2;
						return true;
					}
				}
			}
			swimmingVelocityChange = Vector3.zero;
			return false;
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x0005D488 File Offset: 0x0005B688
		private bool CheckWaterSurfaceJump(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, Vector3 handAvgVelocity, PlayerSwimmingParameters parameters, WaterVolume contactingWaterVolume, WaterVolume.SurfaceQuery waterSurface, out Vector3 jumpVelocity)
		{
			if (contactingWaterVolume != null)
			{
				Plane surfacePlane = waterSurface.surfacePlane;
				bool flag = handAvgVelocity.sqrMagnitude > parameters.waterSurfaceJumpHandSpeedThreshold * parameters.waterSurfaceJumpHandSpeedThreshold;
				if (surfacePlane.GetSide(startingHandPosition) && !surfacePlane.GetSide(endingHandPosition) && flag)
				{
					float value = Vector3.Dot(palmForwardDirection, -waterSurface.surfaceNormal);
					float value2 = Vector3.Dot(handAvgVelocity.normalized, -waterSurface.surfaceNormal);
					float d = parameters.waterSurfaceJumpPalmFacingCurve.Evaluate(Mathf.Clamp(value, 0.01f, 0.99f));
					float d2 = parameters.waterSurfaceJumpHandVelocityFacingCurve.Evaluate(Mathf.Clamp(value2, 0.01f, 0.99f));
					jumpVelocity = -handAvgVelocity * parameters.waterSurfaceJumpAmount * d * d2;
					return true;
				}
			}
			jumpVelocity = Vector3.zero;
			return false;
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x0005D581 File Offset: 0x0005B781
		private bool TryNormalize(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > eps)
			{
				normalized = input / magnitude;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x0005D5AE File Offset: 0x0005B7AE
		private bool TryNormalizeDown(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > 1f)
			{
				normalized = input / magnitude;
				return true;
			}
			if (magnitude >= eps)
			{
				normalized = input;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x0005D7D7 File Offset: 0x0005B9D7
		[CompilerGenerated]
		internal static void <BeginClimbing>g__SnapAxis|245_0(ref float val, float maxDist)
		{
			if (val > maxDist)
			{
				val = maxDist;
				return;
			}
			if (val < -maxDist)
			{
				val = -maxDist;
			}
		}

		// Token: 0x04001257 RID: 4695
		private static Player _instance;

		// Token: 0x04001258 RID: 4696
		public static bool hasInstance;

		// Token: 0x04001259 RID: 4697
		public SphereCollider headCollider;

		// Token: 0x0400125A RID: 4698
		public CapsuleCollider bodyCollider;

		// Token: 0x0400125B RID: 4699
		private float bodyInitialRadius;

		// Token: 0x0400125C RID: 4700
		private float bodyInitialHeight;

		// Token: 0x0400125D RID: 4701
		private RaycastHit bodyHitInfo;

		// Token: 0x0400125E RID: 4702
		private RaycastHit lastHitInfoHand;

		// Token: 0x0400125F RID: 4703
		public Transform leftHandFollower;

		// Token: 0x04001260 RID: 4704
		public Transform rightHandFollower;

		// Token: 0x04001261 RID: 4705
		public Transform rightControllerTransform;

		// Token: 0x04001262 RID: 4706
		public Transform leftControllerTransform;

		// Token: 0x04001263 RID: 4707
		public GorillaVelocityTracker rightHandCenterVelocityTracker;

		// Token: 0x04001264 RID: 4708
		public GorillaVelocityTracker leftHandCenterVelocityTracker;

		// Token: 0x04001265 RID: 4709
		public GorillaVelocityTracker rightInteractPointVelocityTracker;

		// Token: 0x04001266 RID: 4710
		public GorillaVelocityTracker leftInteractPointVelocityTracker;

		// Token: 0x04001267 RID: 4711
		public GorillaVelocityTracker bodyVelocityTracker;

		// Token: 0x04001268 RID: 4712
		public PlayerAudioManager audioManager;

		// Token: 0x04001269 RID: 4713
		private Vector3 lastLeftHandPosition;

		// Token: 0x0400126A RID: 4714
		private Vector3 lastRightHandPosition;

		// Token: 0x0400126B RID: 4715
		public Vector3 lastHeadPosition;

		// Token: 0x0400126C RID: 4716
		private Vector3 lastRigidbodyPosition;

		// Token: 0x0400126D RID: 4717
		private Rigidbody playerRigidBody;

		// Token: 0x0400126E RID: 4718
		public int velocityHistorySize;

		// Token: 0x0400126F RID: 4719
		public float maxArmLength = 1f;

		// Token: 0x04001270 RID: 4720
		public float unStickDistance = 1f;

		// Token: 0x04001271 RID: 4721
		public float velocityLimit;

		// Token: 0x04001272 RID: 4722
		public float slideVelocityLimit;

		// Token: 0x04001273 RID: 4723
		public float maxJumpSpeed;

		// Token: 0x04001274 RID: 4724
		public float jumpMultiplier;

		// Token: 0x04001275 RID: 4725
		public float minimumRaycastDistance = 0.05f;

		// Token: 0x04001276 RID: 4726
		public float defaultSlideFactor = 0.03f;

		// Token: 0x04001277 RID: 4727
		public float slidingMinimum = 0.9f;

		// Token: 0x04001278 RID: 4728
		public float defaultPrecision = 0.995f;

		// Token: 0x04001279 RID: 4729
		public float teleportThresholdNoVel = 1f;

		// Token: 0x0400127A RID: 4730
		public float frictionConstant = 1f;

		// Token: 0x0400127B RID: 4731
		public float slideControl = 0.00425f;

		// Token: 0x0400127C RID: 4732
		public float stickDepth = 0.01f;

		// Token: 0x0400127D RID: 4733
		private Vector3[] velocityHistory;

		// Token: 0x0400127E RID: 4734
		private Vector3[] slideAverageHistory;

		// Token: 0x0400127F RID: 4735
		private int velocityIndex;

		// Token: 0x04001280 RID: 4736
		public Vector3 currentVelocity;

		// Token: 0x04001281 RID: 4737
		private Vector3 denormalizedVelocityAverage;

		// Token: 0x04001282 RID: 4738
		private Vector3 lastPosition;

		// Token: 0x04001283 RID: 4739
		public Vector3 rightHandOffset;

		// Token: 0x04001284 RID: 4740
		public Vector3 leftHandOffset;

		// Token: 0x04001285 RID: 4741
		public Quaternion rightHandRotOffset = Quaternion.identity;

		// Token: 0x04001286 RID: 4742
		public Quaternion leftHandRotOffset = Quaternion.identity;

		// Token: 0x04001287 RID: 4743
		public Vector3 bodyOffset;

		// Token: 0x04001288 RID: 4744
		public LayerMask locomotionEnabledLayers;

		// Token: 0x04001289 RID: 4745
		public LayerMask waterLayer;

		// Token: 0x0400128A RID: 4746
		public bool wasLeftHandTouching;

		// Token: 0x0400128B RID: 4747
		public bool wasRightHandTouching;

		// Token: 0x0400128C RID: 4748
		public bool wasHeadTouching;

		// Token: 0x0400128D RID: 4749
		public int currentMaterialIndex;

		// Token: 0x0400128E RID: 4750
		public bool leftHandSlide;

		// Token: 0x0400128F RID: 4751
		public Vector3 leftHandSlideNormal;

		// Token: 0x04001290 RID: 4752
		public bool rightHandSlide;

		// Token: 0x04001291 RID: 4753
		public Vector3 rightHandSlideNormal;

		// Token: 0x04001292 RID: 4754
		public Vector3 headSlideNormal;

		// Token: 0x04001293 RID: 4755
		public float rightHandSlipPercentage;

		// Token: 0x04001294 RID: 4756
		public float leftHandSlipPercentage;

		// Token: 0x04001295 RID: 4757
		public float headSlipPercentage;

		// Token: 0x04001296 RID: 4758
		public bool wasLeftHandSlide;

		// Token: 0x04001297 RID: 4759
		public bool wasRightHandSlide;

		// Token: 0x04001298 RID: 4760
		public Vector3 rightHandHitPoint;

		// Token: 0x04001299 RID: 4761
		public Vector3 leftHandHitPoint;

		// Token: 0x0400129A RID: 4762
		public float scale = 1f;

		// Token: 0x0400129B RID: 4763
		public bool debugMovement;

		// Token: 0x0400129C RID: 4764
		public bool disableMovement;

		// Token: 0x0400129D RID: 4765
		[NonSerialized]
		public bool inOverlay;

		// Token: 0x0400129E RID: 4766
		[NonSerialized]
		public bool isUserPresent;

		// Token: 0x0400129F RID: 4767
		public GameObject turnParent;

		// Token: 0x040012A0 RID: 4768
		public int leftHandMaterialTouchIndex;

		// Token: 0x040012A1 RID: 4769
		public GorillaSurfaceOverride leftHandSurfaceOverride;

		// Token: 0x040012A2 RID: 4770
		public int rightHandMaterialTouchIndex;

		// Token: 0x040012A3 RID: 4771
		public GorillaSurfaceOverride rightHandSurfaceOverride;

		// Token: 0x040012A4 RID: 4772
		public GorillaSurfaceOverride currentOverride;

		// Token: 0x040012A5 RID: 4773
		public MaterialDatasSO materialDatasSO;

		// Token: 0x040012A6 RID: 4774
		private bool leftHandColliding;

		// Token: 0x040012A7 RID: 4775
		private bool rightHandColliding;

		// Token: 0x040012A8 RID: 4776
		private bool headColliding;

		// Token: 0x040012A9 RID: 4777
		private float degreesTurnedThisFrame;

		// Token: 0x040012AA RID: 4778
		private Vector3 finalPosition;

		// Token: 0x040012AB RID: 4779
		private Vector3 rigidBodyMovement;

		// Token: 0x040012AC RID: 4780
		private Vector3 leftHandPushDisplacement;

		// Token: 0x040012AD RID: 4781
		private Vector3 rightHandPushDisplacement;

		// Token: 0x040012AE RID: 4782
		private RaycastHit hitInfo;

		// Token: 0x040012AF RID: 4783
		private RaycastHit iterativeHitInfo;

		// Token: 0x040012B0 RID: 4784
		private RaycastHit collisionsInnerHit;

		// Token: 0x040012B1 RID: 4785
		private float slipPercentage;

		// Token: 0x040012B2 RID: 4786
		private Vector3 bodyOffsetVector;

		// Token: 0x040012B3 RID: 4787
		private Vector3 distanceTraveled;

		// Token: 0x040012B4 RID: 4788
		private Vector3 movementToProjectedAboveCollisionPlane;

		// Token: 0x040012B5 RID: 4789
		private MeshCollider meshCollider;

		// Token: 0x040012B6 RID: 4790
		private Mesh collidedMesh;

		// Token: 0x040012B7 RID: 4791
		private Player.MaterialData foundMatData;

		// Token: 0x040012B8 RID: 4792
		private string findMatName;

		// Token: 0x040012B9 RID: 4793
		private int vertex1;

		// Token: 0x040012BA RID: 4794
		private int vertex2;

		// Token: 0x040012BB RID: 4795
		private int vertex3;

		// Token: 0x040012BC RID: 4796
		private List<int> trianglesList = new List<int>(1000000);

		// Token: 0x040012BD RID: 4797
		private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(128);

		// Token: 0x040012BE RID: 4798
		private int[] sharedMeshTris;

		// Token: 0x040012BF RID: 4799
		private float lastRealTime;

		// Token: 0x040012C0 RID: 4800
		private float calcDeltaTime;

		// Token: 0x040012C1 RID: 4801
		private float tempRealTime;

		// Token: 0x040012C2 RID: 4802
		private Vector3 junkNormal;

		// Token: 0x040012C3 RID: 4803
		private Vector3 slideAverage;

		// Token: 0x040012C4 RID: 4804
		private Vector3 slideAverageNormal;

		// Token: 0x040012C5 RID: 4805
		private Vector3 tempVector3;

		// Token: 0x040012C6 RID: 4806
		private RaycastHit tempHitInfo;

		// Token: 0x040012C7 RID: 4807
		private RaycastHit junkHit;

		// Token: 0x040012C8 RID: 4808
		private Vector3 firstPosition;

		// Token: 0x040012C9 RID: 4809
		private RaycastHit tempIterativeHit;

		// Token: 0x040012CA RID: 4810
		private bool collisionsReturnBool;

		// Token: 0x040012CB RID: 4811
		private float overlapRadiusFunction;

		// Token: 0x040012CC RID: 4812
		private float maxSphereSize1;

		// Token: 0x040012CD RID: 4813
		private float maxSphereSize2;

		// Token: 0x040012CE RID: 4814
		private Collider[] overlapColliders = new Collider[10];

		// Token: 0x040012CF RID: 4815
		private int overlapAttempts;

		// Token: 0x040012D0 RID: 4816
		private int touchPoints;

		// Token: 0x040012D1 RID: 4817
		private float averageSlipPercentage;

		// Token: 0x040012D2 RID: 4818
		private Vector3 surfaceDirection;

		// Token: 0x040012D3 RID: 4819
		public float iceThreshold = 0.9f;

		// Token: 0x040012D4 RID: 4820
		private float bodyMaxRadius;

		// Token: 0x040012D5 RID: 4821
		public float bodyLerp = 0.17f;

		// Token: 0x040012D6 RID: 4822
		private bool areBothTouching;

		// Token: 0x040012D7 RID: 4823
		private float slideFactor;

		// Token: 0x040012D8 RID: 4824
		[DebugOption]
		public bool didAJump;

		// Token: 0x040012D9 RID: 4825
		private Renderer slideRenderer;

		// Token: 0x040012DA RID: 4826
		private RaycastHit[] rayCastNonAllocColliders;

		// Token: 0x040012DB RID: 4827
		private Vector3[] crazyCheckVectors;

		// Token: 0x040012DC RID: 4828
		private RaycastHit emptyHit;

		// Token: 0x040012DD RID: 4829
		private int bufferCount;

		// Token: 0x040012DE RID: 4830
		private Vector3 lastOpenHeadPosition;

		// Token: 0x040012DF RID: 4831
		private List<Material> tempMaterialArray = new List<Material>(16);

		// Token: 0x040012E0 RID: 4832
		private int disableGripFrameIdx = -1;

		// Token: 0x040012E1 RID: 4833
		[Header("Swimming")]
		public PlayerSwimmingParameters swimmingParams;

		// Token: 0x040012E2 RID: 4834
		public WaterParameters waterParams;

		// Token: 0x040012E3 RID: 4835
		public List<Player.LiquidProperties> liquidPropertiesList = new List<Player.LiquidProperties>(16);

		// Token: 0x040012E4 RID: 4836
		public bool debugDrawSwimming;

		// Token: 0x040012E5 RID: 4837
		[Header("Slam/Hit effects")]
		public GameObject wizardStaffSlamEffects;

		// Token: 0x040012E6 RID: 4838
		public GameObject geodeHitEffects;

		// Token: 0x040012E7 RID: 4839
		private WaterVolume leftHandWaterVolume;

		// Token: 0x040012E8 RID: 4840
		private WaterVolume rightHandWaterVolume;

		// Token: 0x040012E9 RID: 4841
		private WaterVolume.SurfaceQuery leftHandWaterSurface;

		// Token: 0x040012EA RID: 4842
		private WaterVolume.SurfaceQuery rightHandWaterSurface;

		// Token: 0x040012EB RID: 4843
		private Vector3 swimmingVelocity = Vector3.zero;

		// Token: 0x040012EC RID: 4844
		private WaterVolume.SurfaceQuery waterSurfaceForHead;

		// Token: 0x040012ED RID: 4845
		private bool bodyInWater;

		// Token: 0x040012EE RID: 4846
		private bool headInWater;

		// Token: 0x040012EF RID: 4847
		private float buoyancyExtension;

		// Token: 0x040012F0 RID: 4848
		private float lastWaterSurfaceJumpTimeLeft = -1f;

		// Token: 0x040012F1 RID: 4849
		private float lastWaterSurfaceJumpTimeRight = -1f;

		// Token: 0x040012F2 RID: 4850
		private float waterSurfaceJumpCooldown = 0.1f;

		// Token: 0x040012F3 RID: 4851
		private float leftHandNonDiveHapticsAmount;

		// Token: 0x040012F4 RID: 4852
		private float rightHandNonDiveHapticsAmount;

		// Token: 0x040012F5 RID: 4853
		private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x040012F6 RID: 4854
		private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x040012F7 RID: 4855
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x040012F8 RID: 4856
		private BasePlatform currentPlatform;

		// Token: 0x040012F9 RID: 4857
		private BasePlatform lastPlatformTouched;

		// Token: 0x040012FA RID: 4858
		private Vector3 lastFrameTouchPosLocal;

		// Token: 0x040012FB RID: 4859
		private Vector3 lastFrameTouchPosWorld;

		// Token: 0x040012FC RID: 4860
		private bool lastFrameHasValidTouchPos;

		// Token: 0x040012FD RID: 4861
		private Vector3 refMovement = Vector3.zero;

		// Token: 0x040012FE RID: 4862
		private Vector3 platformTouchOffset;

		// Token: 0x040012FF RID: 4863
		private Vector3 debugLastRightHandPosition;

		// Token: 0x04001300 RID: 4864
		private Vector3 debugPlatformDeltaPosition;

		// Token: 0x04001301 RID: 4865
		private const float climbingMaxThrowSpeed = 5.5f;

		// Token: 0x04001302 RID: 4866
		private const float climbHelperSmoothSnapSpeed = 12f;

		// Token: 0x04001303 RID: 4867
		[SerializeField]
		private float velocityThrowClimbingMultiplier = 1f;

		// Token: 0x04001304 RID: 4868
		[SerializeField]
		private float climbingMaxThrowMagnitude = 10f;

		// Token: 0x04001305 RID: 4869
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x04001306 RID: 4870
		private GorillaClimbable currentClimbable;

		// Token: 0x04001307 RID: 4871
		private GorillaHandClimber currentClimber;

		// Token: 0x04001308 RID: 4872
		private Vector3 climbHelperTargetPos = Vector3.zero;

		// Token: 0x04001309 RID: 4873
		private Transform climbHelper;

		// Token: 0x0400130A RID: 4874
		private GorillaRopeSwing currentSwing;

		// Token: 0x0400130B RID: 4875
		private GorillaZipline currentZipline;

		// Token: 0x0400130C RID: 4876
		[SerializeField]
		private ConnectedControllerHandler controllerState;

		// Token: 0x0400130D RID: 4877
		public int sizeLayerMask;

		// Token: 0x0400130E RID: 4878
		public bool InReportMenu;

		// Token: 0x0400130F RID: 4879
		private float halloweenLevitationStrength;

		// Token: 0x04001310 RID: 4880
		private float halloweenLevitationFullStrengthDuration;

		// Token: 0x04001311 RID: 4881
		private float halloweenLevitationTotalDuration = 1f;

		// Token: 0x04001312 RID: 4882
		private float halloweenLevitationBonusStrength;

		// Token: 0x04001313 RID: 4883
		private float halloweenLevitateBonusOffAtYSpeed;

		// Token: 0x04001314 RID: 4884
		private float halloweenLevitateBonusFullAtYSpeed = 1f;

		// Token: 0x04001315 RID: 4885
		private float lastTouchedGroundTimestamp;

		// Token: 0x020004A6 RID: 1190
		[Serializable]
		public struct MaterialData
		{
			// Token: 0x04001F50 RID: 8016
			public string matName;

			// Token: 0x04001F51 RID: 8017
			public bool overrideAudio;

			// Token: 0x04001F52 RID: 8018
			public AudioClip audio;

			// Token: 0x04001F53 RID: 8019
			public bool overrideSlidePercent;

			// Token: 0x04001F54 RID: 8020
			public float slidePercent;
		}

		// Token: 0x020004A7 RID: 1191
		[Serializable]
		public struct LiquidProperties
		{
			// Token: 0x04001F55 RID: 8021
			[Range(0f, 2f)]
			[Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
			public float resistance;

			// Token: 0x04001F56 RID: 8022
			[Range(0f, 3f)]
			[Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
			public float buoyancy;

			// Token: 0x04001F57 RID: 8023
			[Range(0f, 3f)]
			[Tooltip("Damping Half-life Multiplier")]
			public float dampingFactor;

			// Token: 0x04001F58 RID: 8024
			[Range(0f, 1f)]
			public float surfaceJumpFactor;
		}

		// Token: 0x020004A8 RID: 1192
		public enum LiquidType
		{
			// Token: 0x04001F5A RID: 8026
			Water,
			// Token: 0x04001F5B RID: 8027
			Lava
		}
	}
}
