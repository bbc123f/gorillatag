using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AA;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion
{
	public class Player : MonoBehaviour
	{
		public static Player Instance
		{
			get
			{
				return Player._instance;
			}
		}

		public bool IsDefaultScale
		{
			get
			{
				return Mathf.Abs(1f - this.scale) < 0.001f;
			}
		}

		public bool turnedThisFrame
		{
			get
			{
				return this.degreesTurnedThisFrame != 0f;
			}
		}

		public List<Player.MaterialData> materialData
		{
			get
			{
				return this.materialDatasSO.datas;
			}
		}

		public List<WaterVolume> HeadOverlappingWaterVolumes
		{
			get
			{
				return this.headOverlappingWaterVolumes;
			}
		}

		public bool InWater
		{
			get
			{
				return this.bodyInWater;
			}
		}

		public bool HeadInWater
		{
			get
			{
				return this.headInWater;
			}
		}

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

		public WaterVolume.SurfaceQuery WaterSurfaceForHead
		{
			get
			{
				return this.waterSurfaceForHead;
			}
		}

		public WaterVolume LeftHandWaterVolume
		{
			get
			{
				return this.leftHandWaterVolume;
			}
		}

		public WaterVolume RightHandWaterVolume
		{
			get
			{
				return this.rightHandWaterVolume;
			}
		}

		public WaterVolume.SurfaceQuery LeftHandWaterSurface
		{
			get
			{
				return this.leftHandWaterSurface;
			}
		}

		public WaterVolume.SurfaceQuery RightHandWaterSurface
		{
			get
			{
				return this.rightHandWaterSurface;
			}
		}

		public Vector3 LastLeftHandPosition
		{
			get
			{
				return this.lastLeftHandPosition;
			}
		}

		public Vector3 LastRightHandPosition
		{
			get
			{
				return this.lastRightHandPosition;
			}
		}

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

		protected void Start()
		{
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
			}
			this.mainCamera.farClipPlane = 500f;
			this.lastScale = this.scale;
		}

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

		public void SetHalloweenLevitation(float levitateStrength, float levitateDuration, float levitateBlendOutDuration, float levitateBonusStrength, float levitateBonusOffAtYSpeed, float levitateBonusFullAtYSpeed)
		{
			this.halloweenLevitationStrength = levitateStrength;
			this.halloweenLevitationFullStrengthDuration = levitateDuration;
			this.halloweenLevitationTotalDuration = levitateDuration + levitateBlendOutDuration;
			this.halloweenLevitateBonusFullAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitateBonusOffAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitationBonusStrength = levitateBonusStrength;
		}

		public void TeleportToTrain(bool enable)
		{
			this.teleportToTrain = enable;
		}

		public void FixedUpdate()
		{
			this.AntiTeleportTechnology();
			bool isDefaultScale = this.IsDefaultScale;
			if (!isDefaultScale && !this.isClimbing)
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
			this.handleClimbing(Time.fixedDeltaTime);
			this.stuckHandsCheckFixedUpdate();
		}

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

		private Vector3 GetLastLeftHandPosition()
		{
			return this.lastLeftHandPosition + this.MovingSurfaceMovement();
		}

		private Vector3 GetLastRightHandPosition()
		{
			return this.lastRightHandPosition + this.MovingSurfaceMovement();
		}

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

		private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
		{
			return transformToModify.position + transformToModify.rotation * offsetVector * this.scale;
		}

		private void LateUpdate()
		{
			if (this.playerRigidBody.isKinematic)
			{
				return;
			}
			float time = Time.time;
			Vector3 position = this.headCollider.transform.position;
			this.turnParent.transform.localScale = Vector3.one * this.scale;
			this.playerRigidBody.MovePosition(this.playerRigidBody.position + position - this.headCollider.transform.position);
			if (Math.Abs(this.lastScale - this.scale) > 0.001f)
			{
				if (this.mainCamera == null)
				{
					this.mainCamera = Camera.main;
				}
				this.mainCamera.nearClipPlane = ((this.scale > 0.5f) ? 0.01f : 0.002f);
			}
			this.lastScale = this.scale;
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
				this.slideAverage += 9.8f * Vector3.down * this.calcDeltaTime * this.scale;
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
			this.stuckHandsCheckLateUpdate(ref vector, ref vector2);
			if (this.currentPlatform == null)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.velocity += this.refMovement / this.calcDeltaTime;
				}
				this.refMovement = Vector3.zero;
			}
			Vector3 vector4 = Vector3.zero;
			float a2 = 0f;
			Vector3 b;
			if (this.GetSwimmingVelocityForHand(this.lastLeftHandPosition, vector, this.leftControllerTransform.right, this.calcDeltaTime, ref this.leftHandWaterVolume, ref this.leftHandWaterSurface, out b) && !this.turnedThisFrame)
			{
				a2 = Mathf.InverseLerp(0f, 0.2f, b.magnitude) * this.swimmingParams.swimmingHapticsStrength;
				vector4 += b;
			}
			float a3 = 0f;
			Vector3 b2;
			if (this.GetSwimmingVelocityForHand(this.lastRightHandPosition, vector2, -this.rightControllerTransform.right, this.calcDeltaTime, ref this.rightHandWaterVolume, ref this.rightHandWaterSurface, out b2) && !this.turnedThisFrame)
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
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.velocity += vector4 + vector5;
				}
			}
			else
			{
				this.swimmingVelocity = Vector3.zero;
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

		private void stuckHandsCheckFixedUpdate()
		{
			this.stuckLeft = (!this.controllerState.LeftValid || (this.leftHandColliding && (this.GetCurrentLeftHandPosition() - this.GetLastLeftHandPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (this.GetCurrentLeftHandPosition() - this.headCollider.transform.position).normalized, out this.hitInfo, (this.GetCurrentLeftHandPosition() - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)));
			this.stuckRight = (!this.controllerState.RightValid || (this.rightHandColliding && (this.GetCurrentRightHandPosition() - this.GetLastRightHandPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (this.GetCurrentRightHandPosition() - this.headCollider.transform.position).normalized, out this.hitInfo, (this.GetCurrentRightHandPosition() - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)));
		}

		private void stuckHandsCheckLateUpdate(ref Vector3 finalLeftHandPosition, ref Vector3 finalRightHandPosition)
		{
			if (this.stuckLeft)
			{
				finalLeftHandPosition = this.GetCurrentLeftHandPosition();
				this.stuckLeft = (this.leftHandColliding = false);
			}
			if (this.stuckRight)
			{
				finalRightHandPosition = this.GetCurrentRightHandPosition();
				this.stuckRight = (this.rightHandColliding = false);
			}
		}

		private void handleClimbing(float deltaTime)
		{
			if (this.isClimbing && (this.inOverlay || this.climbHelper == null || this.currentClimbable == null || !this.currentClimbable.isActiveAndEnabled))
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			Vector3 b = Vector3.zero;
			if (this.isClimbing && (this.currentClimber.transform.position - this.climbHelper.position).magnitude > 1f)
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			if (this.isClimbing)
			{
				this.playerRigidBody.velocity = Vector3.zero;
				this.climbHelper.localPosition = Vector3.MoveTowards(this.climbHelper.localPosition, this.climbHelperTargetPos, deltaTime * 12f);
				b = this.currentClimber.transform.position - this.climbHelper.position;
				this.playerRigidBody.MovePosition(this.playerRigidBody.position - b);
				if (this.currentSwing)
				{
					this.currentSwing.lastGrabTime = Time.time;
				}
			}
		}

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

		public bool IsHandTouching(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return this.wasLeftHandTouching;
			}
			return this.wasRightHandTouching;
		}

		public bool IsHandSliding(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return this.wasLeftHandSlide || this.leftHandSlide;
			}
			return this.wasRightHandSlide || this.rightHandSlide;
		}

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
				Player.<BeginClimbing>g__SnapAxis|259_0(ref localPosition.x, climbable.maxDistanceSnap);
			}
			if (climbable.snapY)
			{
				Player.<BeginClimbing>g__SnapAxis|259_0(ref localPosition.y, climbable.maxDistanceSnap);
			}
			if (climbable.snapZ)
			{
				Player.<BeginClimbing>g__SnapAxis|259_0(ref localPosition.z, climbable.maxDistanceSnap);
			}
			this.climbHelperTargetPos = localPosition;
			climbable.isBeingClimbed = true;
			hand.isClimbing = true;
			this.currentClimbable = climbable;
			this.currentClimber = hand;
			this.isClimbing = true;
			GorillaRopeSegment gorillaRopeSegment;
			GorillaZipline gorillaZipline;
			PhotonView view;
			if (climbable.TryGetComponent<GorillaRopeSegment>(out gorillaRopeSegment) && gorillaRopeSegment.swing)
			{
				this.currentSwing = gorillaRopeSegment.swing;
				this.currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, this.climbHelperTargetPos, this.currentVelocity);
			}
			else if (climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(out gorillaZipline))
			{
				this.currentZipline = gorillaZipline;
			}
			else if (climbable.TryGetComponent<PhotonView>(out view))
			{
				VRRig.AttachLocalPlayerToPhotonView(view, hand.xrNode, this.climbHelperTargetPos, this.currentVelocity);
			}
			this.enablePlayerGravity(false);
			GorillaTagger.Instance.StartVibration(this.currentClimber.xrNode == XRNode.LeftHand, 0.6f, 0.06f);
			if (climbable.clip)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == XRNode.LeftHand);
			}
		}

		private void VerifyClimbHelper()
		{
			if (this.climbHelper == null || this.climbHelper.gameObject == null)
			{
				this.climbHelper = new GameObject("Climb Helper").transform;
			}
		}

		public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
		{
			if (hand != this.currentClimber)
			{
				return;
			}
			if (!startingNewClimb)
			{
				this.enablePlayerGravity(true);
			}
			Rigidbody rigidbody = null;
			if (this.currentClimbable)
			{
				this.currentClimbable.TryGetComponent<Rigidbody>(out rigidbody);
				this.currentClimbable.isBeingClimbed = false;
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
			PhotonView photonView;
			if (this.currentClimbable.TryGetComponent<PhotonView>(out photonView))
			{
				VRRig.DetachLocalPlayerFromPhotonView();
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

		private void enablePlayerGravity(bool useGravity)
		{
			this.playerRigidBody.useGravity = useGravity;
		}

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

		private void AntiTeleportTechnology()
		{
			if ((this.headCollider.transform.position - this.lastHeadPosition).magnitude >= this.teleportThresholdNoVel + this.playerRigidBody.velocity.magnitude * this.calcDeltaTime)
			{
				base.transform.position = base.transform.position + this.lastHeadPosition - this.headCollider.transform.position;
			}
		}

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

		private void ClearColliderBuffer(ref Collider[] colliders)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i] = null;
			}
		}

		private void ClearRaycasthitBuffer(ref RaycastHit[] raycastHits)
		{
			for (int i = 0; i < raycastHits.Length; i++)
			{
				raycastHits[i] = this.emptyHit;
			}
		}

		private Vector3 MovingSurfaceMovement()
		{
			return this.refMovement;
		}

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

		public void SetMaximumSlipThisFrame()
		{
			this.disableGripFrameIdx = Time.frameCount;
		}

		public bool OverrideSlipToMax()
		{
			return this.disableGripFrameIdx == Time.frameCount;
		}

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

		[CompilerGenerated]
		internal static void <BeginClimbing>g__SnapAxis|259_0(ref float val, float maxDist)
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

		private static Player _instance;

		public static bool hasInstance;

		public SphereCollider headCollider;

		public CapsuleCollider bodyCollider;

		private float bodyInitialRadius;

		private float bodyInitialHeight;

		private RaycastHit bodyHitInfo;

		private RaycastHit lastHitInfoHand;

		public Transform leftHandFollower;

		public Transform rightHandFollower;

		public Transform rightControllerTransform;

		public Transform leftControllerTransform;

		public GorillaVelocityTracker rightHandCenterVelocityTracker;

		public GorillaVelocityTracker leftHandCenterVelocityTracker;

		public GorillaVelocityTracker rightInteractPointVelocityTracker;

		public GorillaVelocityTracker leftInteractPointVelocityTracker;

		public GorillaVelocityTracker bodyVelocityTracker;

		public PlayerAudioManager audioManager;

		private Vector3 lastLeftHandPosition;

		private Vector3 lastRightHandPosition;

		public Vector3 lastHeadPosition;

		private Vector3 lastRigidbodyPosition;

		private Rigidbody playerRigidBody;

		private Camera mainCamera;

		public int velocityHistorySize;

		public float maxArmLength = 1f;

		public float unStickDistance = 1f;

		public float velocityLimit;

		public float slideVelocityLimit;

		public float maxJumpSpeed;

		public float jumpMultiplier;

		public float minimumRaycastDistance = 0.05f;

		public float defaultSlideFactor = 0.03f;

		public float slidingMinimum = 0.9f;

		public float defaultPrecision = 0.995f;

		public float teleportThresholdNoVel = 1f;

		public float frictionConstant = 1f;

		public float slideControl = 0.00425f;

		public float stickDepth = 0.01f;

		private Vector3[] velocityHistory;

		private Vector3[] slideAverageHistory;

		private int velocityIndex;

		public Vector3 currentVelocity;

		private Vector3 denormalizedVelocityAverage;

		private Vector3 lastPosition;

		public Vector3 rightHandOffset;

		public Vector3 leftHandOffset;

		public Quaternion rightHandRotOffset = Quaternion.identity;

		public Quaternion leftHandRotOffset = Quaternion.identity;

		public Vector3 bodyOffset;

		public LayerMask locomotionEnabledLayers;

		public LayerMask waterLayer;

		public bool wasLeftHandTouching;

		public bool wasRightHandTouching;

		public bool wasHeadTouching;

		public int currentMaterialIndex;

		public bool leftHandSlide;

		public Vector3 leftHandSlideNormal;

		public bool rightHandSlide;

		public Vector3 rightHandSlideNormal;

		public Vector3 headSlideNormal;

		public float rightHandSlipPercentage;

		public float leftHandSlipPercentage;

		public float headSlipPercentage;

		public bool wasLeftHandSlide;

		public bool wasRightHandSlide;

		public Vector3 rightHandHitPoint;

		public Vector3 leftHandHitPoint;

		public float scale = 1f;

		public bool debugMovement;

		public bool disableMovement;

		[NonSerialized]
		public bool inOverlay;

		[NonSerialized]
		public bool isUserPresent;

		public GameObject turnParent;

		public int leftHandMaterialTouchIndex;

		public GorillaSurfaceOverride leftHandSurfaceOverride;

		public int rightHandMaterialTouchIndex;

		public GorillaSurfaceOverride rightHandSurfaceOverride;

		public GorillaSurfaceOverride currentOverride;

		public MaterialDatasSO materialDatasSO;

		private bool leftHandColliding;

		private bool rightHandColliding;

		private bool headColliding;

		private float degreesTurnedThisFrame;

		private Vector3 finalPosition;

		private Vector3 rigidBodyMovement;

		private Vector3 leftHandPushDisplacement;

		private Vector3 rightHandPushDisplacement;

		private RaycastHit hitInfo;

		private RaycastHit iterativeHitInfo;

		private RaycastHit collisionsInnerHit;

		private float slipPercentage;

		private Vector3 bodyOffsetVector;

		private Vector3 distanceTraveled;

		private Vector3 movementToProjectedAboveCollisionPlane;

		private MeshCollider meshCollider;

		private Mesh collidedMesh;

		private Player.MaterialData foundMatData;

		private string findMatName;

		private int vertex1;

		private int vertex2;

		private int vertex3;

		private List<int> trianglesList = new List<int>(1000000);

		private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(128);

		private int[] sharedMeshTris;

		private float lastRealTime;

		private float calcDeltaTime;

		private float tempRealTime;

		private Vector3 junkNormal;

		private Vector3 slideAverage;

		private Vector3 slideAverageNormal;

		private Vector3 tempVector3;

		private RaycastHit tempHitInfo;

		private RaycastHit junkHit;

		private Vector3 firstPosition;

		private RaycastHit tempIterativeHit;

		private bool collisionsReturnBool;

		private float overlapRadiusFunction;

		private float maxSphereSize1;

		private float maxSphereSize2;

		private Collider[] overlapColliders = new Collider[10];

		private int overlapAttempts;

		private int touchPoints;

		private float averageSlipPercentage;

		private Vector3 surfaceDirection;

		public float iceThreshold = 0.9f;

		private float bodyMaxRadius;

		public float bodyLerp = 0.17f;

		private bool areBothTouching;

		private float slideFactor;

		[DebugOption]
		public bool didAJump;

		private Renderer slideRenderer;

		private RaycastHit[] rayCastNonAllocColliders;

		private Vector3[] crazyCheckVectors;

		private RaycastHit emptyHit;

		private int bufferCount;

		private Vector3 lastOpenHeadPosition;

		private List<Material> tempMaterialArray = new List<Material>(16);

		private int disableGripFrameIdx = -1;

		private const float CameraFarClipDefault = 500f;

		private const float CameraNearClipDefault = 0.01f;

		private const float CameraNearClipTiny = 0.002f;

		[Header("Swimming")]
		public PlayerSwimmingParameters swimmingParams;

		public WaterParameters waterParams;

		public List<Player.LiquidProperties> liquidPropertiesList = new List<Player.LiquidProperties>(16);

		public bool debugDrawSwimming;

		[Header("Slam/Hit effects")]
		public GameObject wizardStaffSlamEffects;

		public GameObject geodeHitEffects;

		private WaterVolume leftHandWaterVolume;

		private WaterVolume rightHandWaterVolume;

		private WaterVolume.SurfaceQuery leftHandWaterSurface;

		private WaterVolume.SurfaceQuery rightHandWaterSurface;

		private Vector3 swimmingVelocity = Vector3.zero;

		private WaterVolume.SurfaceQuery waterSurfaceForHead;

		private bool bodyInWater;

		private bool headInWater;

		private float buoyancyExtension;

		private float lastWaterSurfaceJumpTimeLeft = -1f;

		private float lastWaterSurfaceJumpTimeRight = -1f;

		private float waterSurfaceJumpCooldown = 0.1f;

		private float leftHandNonDiveHapticsAmount;

		private float rightHandNonDiveHapticsAmount;

		private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16);

		private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16);

		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		private BasePlatform currentPlatform;

		private BasePlatform lastPlatformTouched;

		private Vector3 lastFrameTouchPosLocal;

		private Vector3 lastFrameTouchPosWorld;

		private bool lastFrameHasValidTouchPos;

		private Vector3 refMovement = Vector3.zero;

		private Vector3 platformTouchOffset;

		private Vector3 debugLastRightHandPosition;

		private Vector3 debugPlatformDeltaPosition;

		private const float climbingMaxThrowSpeed = 5.5f;

		private const float climbHelperSmoothSnapSpeed = 12f;

		[SerializeField]
		private float velocityThrowClimbingMultiplier = 1f;

		[SerializeField]
		private float climbingMaxThrowMagnitude = 10f;

		[NonSerialized]
		public bool isClimbing;

		private GorillaClimbable currentClimbable;

		private GorillaHandClimber currentClimber;

		private Vector3 climbHelperTargetPos = Vector3.zero;

		private Transform climbHelper;

		private GorillaRopeSwing currentSwing;

		private GorillaZipline currentZipline;

		[SerializeField]
		private ConnectedControllerHandler controllerState;

		public int sizeLayerMask;

		public bool InReportMenu;

		private float halloweenLevitationStrength;

		private float halloweenLevitationFullStrengthDuration;

		private float halloweenLevitationTotalDuration = 1f;

		private float halloweenLevitationBonusStrength;

		private float halloweenLevitateBonusOffAtYSpeed;

		private float halloweenLevitateBonusFullAtYSpeed = 1f;

		private float lastTouchedGroundTimestamp;

		private bool teleportToTrain;

		public bool isAttachedToTrain;

		private bool stuckLeft;

		private bool stuckRight;

		private float lastScale;

		[Serializable]
		public struct MaterialData
		{
			public string matName;

			public bool overrideAudio;

			public AudioClip audio;

			public bool overrideSlidePercent;

			public float slidePercent;
		}

		[Serializable]
		public struct LiquidProperties
		{
			[Range(0f, 2f)]
			[Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
			public float resistance;

			[Range(0f, 3f)]
			[Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
			public float buoyancy;

			[Range(0f, 3f)]
			[Tooltip("Damping Half-life Multiplier")]
			public float dampingFactor;

			[Range(0f, 1f)]
			public float surfaceJumpFactor;
		}

		public enum LiquidType
		{
			Water,
			Lava
		}
	}
}
