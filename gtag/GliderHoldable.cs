using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AA;
using CjLib;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class GliderHoldable : HoldableObject, IPunObservable, IRequestableOwnershipGuardCallbacks
{
	private bool OutOfBounds
	{
		get
		{
			return this.maxDistanceRespawnOrigin != null && (this.maxDistanceRespawnOrigin.position - base.transform.position).sqrMagnitude > this.maxDistanceBeforeRespawn * this.maxDistanceBeforeRespawn;
		}
	}

	private void Awake()
	{
		this.spawnPosition = base.transform.position;
		this.spawnRotation = base.transform.rotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.yaw = base.transform.rotation.eulerAngles.y;
		this.oneHandRotationRateExp = Mathf.Exp(this.oneHandHoldRotationRate);
		this.twoHandRotationRateExp = Mathf.Exp(this.twoHandHoldRotationRate);
		this.subtlePlayerPitchRateExp = Mathf.Exp(this.subtlePlayerPitchRate);
		this.subtlePlayerRollRateExp = Mathf.Exp(this.subtlePlayerRollRate);
		this.accelSmoothingFollowRateExp = Mathf.Exp(this.accelSmoothingFollowRate);
		this.networkSyncFollowRateExp = Mathf.Exp(this.networkSyncFollowRate);
		this.ownershipGuard.AddCallbackTarget(this);
		this.syncedState.riderId = -1;
		this.syncedState.materialIndex = 0;
		this.syncedState.audioLevel = 0;
		this.syncedState.position = base.transform.position;
		this.syncedState.rotation = base.transform.rotation;
		this.calmAudio.volume = 0f;
		this.activeAudio.volume = 0f;
		this.whistlingAudio.volume = 0f;
		this.RefreshSubtlePlayerPitchButton();
		this.RefreshSubtlePlayerRollButton();
		this.RefreshLeanToTurnButton();
		this.RefreshTurnPlayerButton();
		this.RefreshTurnPitchPlayerButton();
		this.RefreshTurnPitchRollPlayerButton();
		this.RefreshFloatingPitchControlButton();
	}

	private void OnDestroy()
	{
		if (this.ownershipGuard != null)
		{
			this.ownershipGuard.RemoveCallbackTarget(this);
		}
	}

	public void Respawn()
	{
		if (base.photonView.IsMine || !PhotonNetwork.InRoom)
		{
			this.rb.isKinematic = true;
			this.leftHold.Deactivate();
			this.rightHold.Deactivate();
			base.transform.position = this.spawnPosition;
			base.transform.rotation = this.spawnRotation;
			this.lastHeldTime = -1f;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.audioLevel = 0;
			this.syncedState.position = base.transform.position;
			this.syncedState.rotation = base.transform.rotation;
		}
	}

	public override bool TwoHanded
	{
		get
		{
			return true;
		}
	}

	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!base.photonView.IsMine && PhotonNetwork.InRoom && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
		}
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (base.photonView.IsMine || !PhotonNetwork.InRoom || this.pendingOwnershipRequest)
		{
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
			return;
		}
		if (PhotonNetwork.InRoom && !base.photonView.IsMine && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
		}
	}

	public void OnGrabAuthority(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!base.photonView.IsMine && PhotonNetwork.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		bool flag2 = (flag && !EquipmentInteractor.instance.isLeftGrabbing) || (!flag && !EquipmentInteractor.instance.isRightGrabbing);
		GliderHoldable gliderHoldable = (flag ? (EquipmentInteractor.instance.rightHandHeldEquipment as GliderHoldable) : (EquipmentInteractor.instance.leftHandHeldEquipment as GliderHoldable));
		bool flag3 = gliderHoldable != null && gliderHoldable != this;
		if (flag2 || flag3)
		{
			return;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		Vector3 vector = this.ClosestPointInHandle(grabbingHand.transform.position, pointGrabbed);
		if (flag)
		{
			this.leftHold.Activate(grabbingHand.transform, base.transform, vector);
		}
		else
		{
			this.rightHold.Activate(grabbingHand.transform, base.transform, vector);
		}
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 handsVector = this.GetHandsVector(this.leftHold.transform.position, this.rightHold.transform.position, GorillaLocomotion.Player.Instance.headCollider.transform.position, true);
			this.twoHandRotationOffsetAxis = Vector3.Cross(handsVector, base.transform.right).normalized;
			if ((double)this.twoHandRotationOffsetAxis.sqrMagnitude < 0.001)
			{
				this.twoHandRotationOffsetAxis = base.transform.right;
				this.twoHandRotationOffsetAngle = 0f;
			}
			else
			{
				this.twoHandRotationOffsetAngle = Vector3.SignedAngle(handsVector, base.transform.right, this.twoHandRotationOffsetAxis);
			}
		}
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		this.ridersMaterialIndex = 0;
		if (this.cosmeticMaterialOverrides.Length != 0)
		{
			RigContainer rigContainer;
			VRRig vrrig;
			if (PhotonNetwork.InRoom && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer.Rig != null)
			{
				vrrig = rigContainer.Rig;
			}
			else
			{
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (vrrig != null)
			{
				for (int i = 0; i < this.cosmeticMaterialOverrides.Length; i++)
				{
					if (this.cosmeticMaterialOverrides[i].cosmeticName != null && vrrig.cosmeticSet != null && vrrig.cosmeticSet.HasItem(this.cosmeticMaterialOverrides[i].cosmeticName))
					{
						this.ridersMaterialIndex = (byte)(i + 2);
					}
				}
			}
		}
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		if (this.leftHold.active && this.rightHold.active)
		{
			if (flag)
			{
				this.rightHold.Activate(this.rightHold.transform, base.transform, this.ClosestPointInHandle(this.rightHold.transform.position, this.handle));
			}
			else
			{
				this.leftHold.Activate(this.leftHold.transform, base.transform, this.ClosestPointInHandle(this.leftHold.transform.position, this.handle));
			}
		}
		Vector3 vector = Vector3.zero;
		if (flag)
		{
			this.leftHold.Deactivate();
			vector = GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		else
		{
			this.rightHold.Deactivate();
			vector = GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
		if (!this.leftHold.active && !this.rightHold.active)
		{
			GorillaTagger.Instance.UnsetTagRadiusOverride();
			this.subtlePlayerPitch = 0f;
			this.subtlePlayerRoll = 0f;
			this.leftHoldPositionLocal = null;
			this.rightHoldPositionLocal = null;
			this.ridersMaterialIndex = 0;
			if (base.photonView.IsMine || !PhotonNetwork.InRoom)
			{
				this.rb.isKinematic = false;
				this.rb.useGravity = true;
				this.rb.velocity = vector;
				this.syncedState.riderId = -1;
				this.syncedState.materialIndex = 0;
				this.syncedState.position = base.transform.position;
				this.syncedState.rotation = base.transform.rotation;
				this.syncedState.audioLevel = 0;
			}
		}
		return true;
	}

	public void FixedUpdate()
	{
		if (!base.photonView.IsMine && PhotonNetwork.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			GorillaLocomotion.Player instance = GorillaLocomotion.Player.Instance;
			this.previousVelocity = this.currentVelocity;
			this.currentVelocity = instance.Velocity;
			float magnitude = this.currentVelocity.magnitude;
			this.accelerationAverage.AddSample((this.currentVelocity - this.previousVelocity) / Time.fixedDeltaTime, Time.fixedTime);
			float rollAngle180Wrapping = this.GetRollAngle180Wrapping();
			float num = this.liftIncreaseVsRoll.Evaluate(Mathf.Clamp01(Mathf.Abs(rollAngle180Wrapping / 180f))) * this.liftIncreaseVsRollMaxAngle;
			Vector3 vector = Vector3.RotateTowards(this.currentVelocity, Quaternion.AngleAxis(num, -base.transform.right) * base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime);
			Vector3 vector2 = vector - this.currentVelocity;
			float num2 = this.NormalizeAngle180(Vector3.SignedAngle(Vector3.ProjectOnPlane(this.currentVelocity, base.transform.right), base.transform.forward, base.transform.right));
			if (num2 > 90f)
			{
				num2 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num2));
			}
			else if (num2 < -90f)
			{
				num2 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num2));
			}
			float num3 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, num2));
			float num4 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, this.pitch));
			float num5 = this.liftVsAttack.Evaluate(num3);
			instance.AddForce(vector2 * num5, ForceMode.VelocityChange);
			float num6 = this.dragVsAttack.Evaluate(num3);
			float num7 = ((this.syncedState.riderId != -1 && this.syncedState.materialIndex == 1) ? (this.dragVsSpeedMaxSpeed + this.infectedSpeedIncrease) : this.dragVsSpeedMaxSpeed);
			float num8 = this.dragVsSpeed.Evaluate(Mathf.Clamp01(magnitude / num7));
			float num9 = Mathf.Clamp01(num6 * this.attackDragFactor + num8 * this.dragVsSpeedDragFactor);
			instance.AddForce(-this.currentVelocity * num9, ForceMode.Acceleration);
			if (this.pitch > 0f && this.currentVelocity.y > 0f && (this.currentVelocity - this.previousVelocity).y > 0f)
			{
				float num10 = Mathf.InverseLerp(0f, this.pullUpLiftActivationVelocity, this.currentVelocity.y);
				float num11 = Mathf.InverseLerp(0f, this.pullUpLiftActivationAcceleration, (this.currentVelocity - this.previousVelocity).y / fixedDeltaTime);
				float num12 = Mathf.Min(num10, num11);
				instance.AddForce(-Physics.gravity * this.pullUpLiftBonus * num12, ForceMode.Acceleration);
			}
			if (Vector3.Dot(vector, Physics.gravity) > 0f)
			{
				instance.AddForce(-Physics.gravity * this.gravityCompensation, ForceMode.Acceleration);
			}
			this.pitchNormalizedDebug = num4;
			this.attackNormalizedDebug = num3;
			this.liftMultiplierDebug = num5;
			this.dragAttackMultiplierDebug = num6;
			this.dragSpeedMultiplierDebug = num8;
			this.dragNetMultiplierDebug = num9;
			return;
		}
		Vector3 vector3 = this.WindResistanceForceOffset(base.transform.up, Vector3.down);
		Vector3 vector4 = base.transform.position - vector3 * this.gravityUprightTorqueMultiplier;
		this.rb.AddForceAtPosition(-this.fallingGravityReduction * Physics.gravity, vector4, ForceMode.Acceleration);
	}

	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (base.photonView.IsMine || !PhotonNetwork.InRoom || this.pendingOwnershipRequest)
		{
			this.AuthorityUpdate(deltaTime);
			return;
		}
		this.RemoteSyncUpdate(deltaTime);
	}

	private void AuthorityUpdate(float dt)
	{
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.position = base.transform.position;
			this.syncedState.rotation = base.transform.rotation;
			float num = Mathf.Clamp01((float)this.syncedState.audioLevel / 255f);
			this.syncedState.audioLevel = (byte)Mathf.FloorToInt(255f * Mathf.Lerp(0f, num, Mathf.Exp(-2f * dt)));
			this.UpdateAudioSource(this.calmAudio, Mathf.Lerp(0f, this.calmAudio.volume, Mathf.Exp(-2f * dt)));
			this.UpdateAudioSource(this.activeAudio, Mathf.Lerp(0f, this.activeAudio.volume, Mathf.Exp(-2f * dt)));
			this.UpdateAudioSource(this.whistlingAudio, Mathf.Lerp(0f, this.whistlingAudio.volume, Mathf.Exp(-2f * dt)));
			this.leafMesh.material = this.baseLeafMaterial;
			if (this.OutOfBounds || (this.lastHeldTime > 0f && this.lastHeldTime < Time.time - this.maxDroppedTimeToRespawn))
			{
				this.Respawn();
				return;
			}
		}
		else if (this.leftHold.active || this.rightHold.active)
		{
			this.rb.isKinematic = true;
			this.lastHeldTime = Time.time;
			if (this.leftHold.active)
			{
				this.leftHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.leftHold.holdLocalPos, Mathf.Exp(-5f * dt));
			}
			if (this.rightHold.active)
			{
				this.rightHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.rightHold.holdLocalPos, Mathf.Exp(-5f * dt));
			}
			Vector3 vector = Vector3.zero;
			if (this.leftHold.active && this.rightHold.active)
			{
				vector = (this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos)) * 0.5f;
			}
			else if (this.leftHold.active)
			{
				vector = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos);
			}
			else if (this.rightHold.active)
			{
				vector = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos);
			}
			this.UpdateGliderPosition();
			float magnitude = this.currentVelocity.magnitude;
			if (this.setMaxHandSlipDuringFlight && magnitude > this.maxSlipOverrideSpeedThreshold)
			{
				if (this.leftHold.active)
				{
					GorillaLocomotion.Player.Instance.SetLeftMaximumSlipThisFrame();
				}
				if (this.rightHold.active)
				{
					GorillaLocomotion.Player.Instance.SetRightMaximumSlipThisFrame();
				}
			}
			bool flag = false;
			GorillaTagManager gorillaTagManager = GorillaGameManager.instance as GorillaTagManager;
			if (gorillaTagManager != null)
			{
				flag = gorillaTagManager.IsInfected(PhotonNetwork.LocalPlayer);
			}
			this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			Vector3 average = this.accelerationAverage.GetAverage();
			this.accelerationSmoothed = Mathf.Lerp(average.magnitude, this.accelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
			float num2 = Mathf.InverseLerp(this.hapticMaxSpeedInputRange.x, this.hapticMaxSpeedInputRange.y, magnitude);
			float num3 = Mathf.InverseLerp(this.hapticAccelInputRange.x, this.hapticAccelInputRange.y, this.accelerationSmoothed);
			float num4 = Mathf.InverseLerp(this.hapticSpeedInputRange.x, this.hapticSpeedInputRange.y, magnitude);
			this.UpdateAudioSource(this.calmAudio, num2 * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, num3 * num2 * this.audioVolumeMultiplier);
			if (flag)
			{
				this.UpdateAudioSource(this.whistlingAudio, Mathf.InverseLerp(this.whistlingAudioSpeedInputRange.x, this.whistlingAudioSpeedInputRange.y, magnitude) * num3 * num2 * this.audioVolumeMultiplier);
			}
			else
			{
				this.UpdateAudioSource(this.whistlingAudio, 0f);
			}
			float num5 = Mathf.Max(num3 * this.hapticAccelOutputMax * num2, num4 * this.hapticSpeedOutputMax);
			if (this.rightHold.active)
			{
				GorillaTagger.Instance.DoVibration(XRNode.RightHand, num5, dt);
			}
			if (this.leftHold.active)
			{
				GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num5, dt);
			}
			Vector3 vector2 = this.handle.transform.position + this.handle.transform.rotation * new Vector3(0f, 0f, 1f);
			if (Time.frameCount % 2 == 0)
			{
				Vector3 vector3 = this.handle.transform.rotation * new Vector3(-0.707f, 0f, 0.707f);
				RaycastHit raycastHit;
				if (this.leftWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(vector2, vector3), out raycastHit, this.whooshCheckDistance, GorillaLocomotion.Player.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
				{
					this.leftWhooshStartTime = Time.time;
					this.leftWhooshHitPoint = raycastHit.point;
					this.leftWhooshAudio.Stop();
					this.leftWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
					this.leftWhooshAudio.Play();
				}
			}
			else
			{
				Vector3 vector4 = this.handle.transform.rotation * new Vector3(0.707f, 0f, 0.707f);
				RaycastHit raycastHit2;
				if (this.rightWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(vector2, vector4), out raycastHit2, this.whooshCheckDistance, GorillaLocomotion.Player.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
				{
					this.rightWhooshStartTime = Time.time;
					this.rightWhooshHitPoint = raycastHit2.point;
					this.rightWhooshAudio.Stop();
					this.rightWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
					this.rightWhooshAudio.Play();
				}
			}
			Vector3 headCenterPosition = GorillaLocomotion.Player.Instance.HeadCenterPosition;
			if (this.leftWhooshStartTime > Time.time - this.whooshSoundDuration)
			{
				this.leftWhooshAudio.transform.position = this.leftWhooshHitPoint;
			}
			else
			{
				this.leftWhooshAudio.transform.localPosition = new Vector3(-this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
			}
			if (this.rightWhooshStartTime > Time.time - this.whooshSoundDuration)
			{
				this.rightWhooshAudio.transform.position = this.rightWhooshHitPoint;
			}
			else
			{
				this.rightWhooshAudio.transform.localPosition = new Vector3(this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
			}
			if (this.extendTagRangeInFlight)
			{
				float num6 = Mathf.Lerp(this.tagRangeOutput.x, this.tagRangeOutput.y, Mathf.InverseLerp(this.tagRangeSpeedInput.x, this.tagRangeSpeedInput.y, magnitude));
				GorillaTagger.Instance.SetTagRadiusOverride(num6);
				if (this.debugDrawTagRange)
				{
					GorillaTagger.Instance.DebugDrawTagCasts(Color.yellow);
				}
			}
			Vector3 vector5 = Vector3.ProjectOnPlane(base.transform.right, Vector3.up).normalized;
			Vector3 vector6 = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
			if (this.rotatePlayerFully)
			{
				vector5 = base.transform.right;
				vector6 = base.transform.forward;
			}
			float num7 = -Vector3.Dot(vector - this.handle.transform.position, vector6);
			Vector3 vector7 = this.handle.transform.position - vector6 * (this.riderPosRange.y * 0.5f + this.riderPosRangeOffset + num7);
			float num8 = Vector3.Dot(headCenterPosition - vector7, vector5);
			float num9 = Vector3.Dot(headCenterPosition - vector7, vector6);
			num8 /= this.riderPosRange.x * 0.5f;
			num9 /= this.riderPosRange.y * 0.5f;
			this.riderPosition.x = Mathf.Sign(num8) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.x, 1f, Mathf.Abs(num8)));
			this.riderPosition.y = Mathf.Sign(num9) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.y, 1f, Mathf.Abs(num9)));
			bool flag2 = false;
			if (flag2 || this.directOrienting)
			{
				if ((this.leftHold.active && this.rightHold.active) || (this.limitedOneHandControls && (this.leftHold.active || this.rightHold.active)))
				{
					Vector3 vector8;
					Vector3 vector9;
					if (this.leftHold.active && this.rightHold.active)
					{
						vector8 = this.leftHold.transform.position;
						this.leftHoldPositionLocal = new Vector3?(GorillaLocomotion.Player.Instance.transform.InverseTransformPoint(vector8));
						vector9 = this.rightHold.transform.position;
						this.rightHoldPositionLocal = new Vector3?(GorillaLocomotion.Player.Instance.transform.InverseTransformPoint(vector9));
					}
					else if (this.leftHold.active)
					{
						vector8 = this.leftHold.transform.position;
						this.leftHoldPositionLocal = new Vector3?(GorillaLocomotion.Player.Instance.transform.InverseTransformPoint(vector8));
						Vector3 vector10;
						if (this.oneHandRollWithRotation)
						{
							vector10 = vector8 + this.leftHold.transform.up * this.oneHandSimulatedHoldOffset.x;
						}
						else
						{
							Vector3 normalized = Vector3.Cross(Vector3.up, vector8 - headCenterPosition).normalized;
							float num10 = this.oneHandSimulatedHoldOffset.x * 0.5f;
							float magnitude2 = (vector8 - headCenterPosition).magnitude;
							float num11 = Mathf.Sqrt(1f / (1f / Mathf.Pow(num10, 2f) - 1f / Mathf.Pow(magnitude2, 2f)));
							Vector3 vector11 = vector8 + normalized * num11 - headCenterPosition;
							Vector3 normalized2 = Vector3.Cross(Vector3.up, vector11).normalized;
							float num12 = Mathf.Clamp01(Vector3.Dot(Vector3.ProjectOnPlane(GorillaLocomotion.Player.Instance.headCollider.transform.right, Vector3.up), Vector3.ProjectOnPlane(base.transform.forward, Vector3.up)));
							Vector3 vector12 = Vector3.Lerp(normalized2, normalized, num12) * this.oneHandSimulatedHoldOffset.x;
							vector10 = this.leftHold.transform.position + vector12 + Vector3.up * (headCenterPosition.y + this.oneHandSimulatedHoldOffset.y - this.leftHold.transform.position.y);
						}
						if (this.rightHoldPositionLocal != null)
						{
							this.rightHoldPositionLocal = new Vector3?(Vector3.Lerp(GorillaLocomotion.Player.Instance.transform.InverseTransformPoint(vector10), this.rightHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
							vector9 = GorillaLocomotion.Player.Instance.transform.TransformPoint(this.rightHoldPositionLocal.Value);
						}
						else
						{
							vector9 = vector10;
							this.rightHoldPositionLocal = new Vector3?(GorillaLocomotion.Player.Instance.transform.InverseTransformPoint(vector9));
						}
					}
					else
					{
						vector9 = this.rightHold.transform.position;
						this.rightHoldPositionLocal = new Vector3?(GorillaLocomotion.Player.Instance.transform.InverseTransformPoint(vector9));
						Vector3 vector13;
						if (this.oneHandRollWithRotation)
						{
							vector13 = vector9 + this.rightHold.transform.up * this.oneHandSimulatedHoldOffset.x;
						}
						else
						{
							Vector3 normalized3 = Vector3.Cross(vector9 - headCenterPosition, Vector3.up).normalized;
							float num13 = this.oneHandSimulatedHoldOffset.x * 0.5f;
							float magnitude3 = (vector9 - headCenterPosition).magnitude;
							float num14 = Mathf.Sqrt(1f / (1f / Mathf.Pow(num13, 2f) - 1f / Mathf.Pow(magnitude3, 2f)));
							Vector3 normalized4 = Vector3.Cross(vector9 + normalized3 * num14 - headCenterPosition, Vector3.up).normalized;
							float num15 = Mathf.Clamp01(Vector3.Dot(Vector3.ProjectOnPlane(-GorillaLocomotion.Player.Instance.headCollider.transform.right, Vector3.up), Vector3.ProjectOnPlane(base.transform.forward, Vector3.up)));
							Vector3 vector14 = Vector3.Lerp(normalized4, normalized3, num15) * this.oneHandSimulatedHoldOffset.x;
							vector13 = this.rightHold.transform.position + vector14 + Vector3.up * (headCenterPosition.y + this.oneHandSimulatedHoldOffset.y - this.rightHold.transform.position.y);
						}
						if (this.leftHoldPositionLocal != null)
						{
							this.leftHoldPositionLocal = new Vector3?(Vector3.Lerp(GorillaLocomotion.Player.Instance.transform.InverseTransformPoint(vector13), this.leftHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
							vector8 = GorillaLocomotion.Player.Instance.transform.TransformPoint(this.leftHoldPositionLocal.Value);
						}
						else
						{
							vector8 = vector13;
							this.leftHoldPositionLocal = new Vector3?(GorillaLocomotion.Player.Instance.transform.InverseTransformPoint(vector8));
						}
					}
					Vector3 vector15;
					Vector3 vector16;
					this.GetHandsOrientationVectors(vector8, vector9, GorillaLocomotion.Player.Instance.headCollider.transform, false, out vector15, out vector16);
					float num16 = this.riderPosition.y * this.riderPosDirectPitchMax;
					if (!this.leftHold.active || !this.rightHold.active)
					{
						num16 *= this.oneHandPitchMultiplier;
					}
					Spring.CriticalSpringDamperExact(ref this.pitch, ref this.pitchVel, num16, 0f, this.pitchHalfLife, dt);
					this.pitch = Mathf.Clamp(this.pitch, this.pitchMinMax.x, this.pitchMinMax.y);
					Quaternion quaternion = Quaternion.AngleAxis(this.pitch, Vector3.right);
					this.twoHandRotationOffsetAngle = Mathf.Lerp(0f, this.twoHandRotationOffsetAngle, Mathf.Exp(-8f * dt));
					Vector3 vector17 = (this.twoHandGliderInversionOnYawInsteadOfRoll ? vector16 : Vector3.up);
					Quaternion quaternion2 = Quaternion.AngleAxis(this.twoHandRotationOffsetAngle, this.twoHandRotationOffsetAxis) * Quaternion.LookRotation(vector15, vector17) * Quaternion.AngleAxis(-90f, Vector3.up);
					float num17 = ((this.leftHold.active && this.rightHold.active) ? this.twoHandRotationRateExp : this.oneHandRotationRateExp);
					base.transform.rotation = Quaternion.Slerp(quaternion2 * quaternion, base.transform.rotation, Mathf.Exp(-num17 * dt));
				}
				else if (this.leftHold.active)
				{
					Quaternion quaternion3 = this.leftHold.transform.rotation * this.leftHold.localHoldRotation;
					base.transform.rotation = Quaternion.Slerp(quaternion3, base.transform.rotation, Mathf.Exp(-this.oneHandRotationRateExp * dt));
				}
				else if (this.rightHold.active)
				{
					Quaternion quaternion4 = this.rightHold.transform.rotation * this.rightHold.localHoldRotation;
					base.transform.rotation = Quaternion.Slerp(quaternion4, base.transform.rotation, Mathf.Exp(-this.oneHandRotationRateExp * dt));
				}
				if (this.subtlePlayerPitchActive || this.subtlePlayerRollActive)
				{
					float num18 = Mathf.InverseLerp(this.subtlePlayerRotationSpeedRampMinMax.x, this.subtlePlayerRotationSpeedRampMinMax.y, this.currentVelocity.magnitude);
					Quaternion quaternion5 = Quaternion.identity;
					if (this.subtlePlayerRollActive)
					{
						float num19 = this.GetRollAngle180Wrapping();
						if (num19 > 90f)
						{
							num19 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num19));
						}
						else if (num19 < -90f)
						{
							num19 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num19));
						}
						Vector3 normalized5 = new Vector3(this.currentVelocity.x, 0f, this.currentVelocity.z).normalized;
						Vector3 vector18 = new Vector3(average.x, 0f, average.z);
						float num20 = Vector3.Dot(vector18 - Vector3.Dot(vector18, normalized5) * normalized5, Vector3.Cross(normalized5, Vector3.up));
						this.turnAccelerationSmoothed = Mathf.Lerp(num20, this.turnAccelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
						float num21 = 0f;
						if (num20 * num19 > 0f)
						{
							num21 = Mathf.InverseLerp(this.subtlePlayerRollAccelMinMax.x, this.subtlePlayerRollAccelMinMax.y, Mathf.Abs(this.turnAccelerationSmoothed));
						}
						float num22 = num19 * this.subtlePlayerRollFactor * Mathf.Min(num18, num21);
						this.subtlePlayerRoll = Mathf.Lerp(num22, this.subtlePlayerRoll, Mathf.Exp(-this.subtlePlayerRollRateExp * dt));
						quaternion5 = Quaternion.AngleAxis(this.subtlePlayerRoll, base.transform.forward);
					}
					Quaternion quaternion6 = Quaternion.identity;
					if (this.subtlePlayerPitchActive)
					{
						float num23 = this.pitch * this.subtlePlayerPitchFactor * Mathf.Min(num18, 1f);
						this.subtlePlayerPitch = Mathf.Lerp(num23, this.subtlePlayerPitch, Mathf.Exp(-this.subtlePlayerPitchRateExp * dt));
						quaternion6 = Quaternion.AngleAxis(this.subtlePlayerPitch, -base.transform.right);
					}
					GorillaLocomotion.Player.Instance.PlayerRotationOverride = quaternion6 * quaternion5;
				}
				if (flag2)
				{
					Vector3 eulerAngles = base.transform.rotation.eulerAngles;
					this.pitch = eulerAngles.x;
					this.yaw = eulerAngles.y;
					this.roll = eulerAngles.z;
					this.pitchVel = 0f;
					this.rollVel = 0f;
				}
				this.UpdateGliderPosition();
			}
			else
			{
				if (!this.velocityBasedPitch)
				{
					float num24 = this.riderPosition.y * this.riderPosPitchMax;
					Spring.CriticalSpringDamperExact(ref this.pitch, ref this.pitchVel, num24, 0f, this.pitchHalfLife, dt);
					this.pitch = Mathf.Clamp(this.pitch, this.pitchMinMax.x, this.pitchMinMax.y);
				}
				else if (this.riderPosition.y >= 0f)
				{
					float num25 = this.riderPosition.y * this.pitchVelocityTargetMinMax.y;
					float num26 = this.pitchVelocityTargetMinMax.y / this.pitchVelocityRampTimeMinMax.y;
					this.pitchVel = Mathf.MoveTowards(this.pitchVel, num25, num26 * dt);
					this.pitch += this.pitchVel * dt;
				}
				else
				{
					float num27 = -this.riderPosition.y * this.pitchVelocityTargetMinMax.x;
					float num28 = this.pitchVelocityTargetMinMax.x / this.pitchVelocityRampTimeMinMax.x;
					this.pitchVel = Mathf.MoveTowards(this.pitchVel, num27, num28 * dt);
					this.pitch += this.pitchVel * dt;
				}
				float num29 = -this.riderPosition.x * this.riderPosRollMax;
				Spring.CriticalSpringDamperExact(ref this.roll, ref this.rollVel, num29, 0f, this.pitchHalfLife, dt);
				float num30 = -num29 * this.yawRateFactorRollOnly;
				this.yaw += num30 * dt;
				this.roll = Mathf.Clamp(this.roll, this.rollMinMax.x, this.rollMinMax.y);
				Quaternion quaternion7 = Quaternion.Euler(this.pitch, this.yaw, this.roll);
				base.transform.rotation = quaternion7;
				GorillaLocomotion.Player instance = GorillaLocomotion.Player.Instance;
				if (this.rotatePlayerFully)
				{
					instance.transform.rotation = quaternion7;
				}
				else if (this.rotatePlayerPitch)
				{
					Quaternion quaternion8 = quaternion7;
					Vector3 vector19 = quaternion8 * Vector3.right;
					Vector3 vector20 = Vector3.ProjectOnPlane(vector19, Vector3.up);
					quaternion8 = Quaternion.FromToRotation(vector19, vector20) * quaternion8;
					instance.transform.rotation = quaternion8;
				}
				else if (this.rotatePlayerYaw)
				{
					instance.turnParent.transform.RotateAround(vector, Vector3.up, num30 * dt);
				}
				this.UpdateGliderPosition();
				if (this.debugDraw)
				{
					Quaternion quaternion9 = Quaternion.AngleAxis(this.handle.transform.eulerAngles.y, Vector3.up);
					DebugUtil.DrawRect(vector7, quaternion9, this.riderPosRange, Color.white, false, DebugUtil.Style.Wireframe);
					Vector3 vector21 = this.handle.transform.TransformPoint(new Vector3(0f, 0.15f, 0f));
					float num31 = 0.15f;
					Quaternion quaternion10 = quaternion9 * Quaternion.AngleAxis(90f, Vector3.right);
					DebugUtil.DrawRect(vector21, quaternion10, new Vector2(num31, num31), Color.white, false, DebugUtil.Style.Wireframe);
					DebugUtil.DrawCircle(vector21, quaternion10, this.riderPosRangeNormalizedDeadzone.x * num31 * 0.5f, 16, Color.white, false, DebugUtil.Style.Wireframe);
					DebugUtil.DrawSphere(vector21 + quaternion9 * new Vector3(this.riderPosition.x * num31 * 0.5f, this.riderPosition.y * num31 * 0.5f, 0f), 0.01f, 12, 12, Color.white, false, DebugUtil.Style.SolidColor);
				}
			}
			if (this.debugDraw && this.directOrienting)
			{
				Vector3 vector22 = this.handle.transform.TransformPoint(new Vector3(0f, 0.15f, 0f));
				Quaternion quaternion11 = Quaternion.AngleAxis(this.handle.transform.eulerAngles.y, Vector3.up);
				float num32 = 0.075f;
				DebugUtil.DrawRect(vector22, quaternion11 * Quaternion.AngleAxis(90f, Vector3.right), Vector2.one * 2f * num32, Color.white, false, DebugUtil.Style.Wireframe);
				Vector3 vector23 = vector22 - quaternion11 * Vector3.up * num32;
				Vector3 vector24 = vector22 + quaternion11 * Vector3.up * num32;
				DebugUtil.DrawLine(vector23, vector24, Color.white, false);
				Vector3 vector25 = Vector3.Lerp(vector23, vector24, this.pitchNormalizedDebug * 0.5f + 0.5f);
				DebugUtil.DrawLine(vector25 - quaternion11 * Vector3.right * 0.01f, vector25, Color.white, true);
				DebugUtil.DrawLine(vector25 - quaternion11 * Vector3.right * num32, vector25 - quaternion11 * Vector3.right * (num32 - 0.01f), Color.white, true);
				Vector3 vector26 = Vector3.Lerp(vector23, vector24, this.attackNormalizedDebug * 0.5f + 0.5f);
				DebugUtil.DrawLine(vector26, vector26 + quaternion11 * Vector3.right * 0.01f, Color.white, true);
				DebugUtil.DrawLine(vector26 + quaternion11 * Vector3.right * num32, vector26 + quaternion11 * Vector3.right * (num32 - 0.01f), Color.white, true);
				Vector3 vector27 = vector25 - quaternion11 * Vector3.right * num32;
				Vector3 vector28 = Vector3.Lerp(vector27, vector25, this.liftMultiplierDebug);
				Color color = Color.Lerp(Color.white, Color.green, this.liftMultiplierDebug);
				DebugUtil.DrawLine(vector27, vector28, color, true);
				DebugUtil.DrawSphere(vector28, 0.001f, 12, 12, color, false, DebugUtil.Style.SolidColor);
				Vector3 vector29 = Vector3.Lerp(vector26, vector26 + quaternion11 * Vector3.right * num32, this.dragAttackMultiplierDebug);
				Color color2 = Color.Lerp(Color.white, Color.red, this.dragAttackMultiplierDebug);
				DebugUtil.DrawLine(vector26, vector29, color2, true);
				DebugUtil.DrawSphere(vector29, 0.001f, 12, 12, color2, false, DebugUtil.Style.SolidColor);
				float num33 = 0.15f;
				float num34 = 0.015f;
				Vector3 vector30 = vector22 + quaternion11 * Vector3.up * (num33 * 0.5f + num34 * 0.5f);
				DebugUtil.DrawRect(vector30, quaternion11 * Quaternion.AngleAxis(90f, Vector3.right), new Vector2(num33, num34), Color.white, false, DebugUtil.Style.Wireframe);
				float num35 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f * this.subtlePlayerRollFactor, 90f * this.subtlePlayerRollFactor, -this.subtlePlayerRoll));
				Vector3 vector31 = vector30 + quaternion11 * Vector3.right * num35 * num33 * 0.5f;
				DebugUtil.DrawLine(vector31 - quaternion11 * Vector3.up * num34 * 0.5f, vector31 + quaternion11 * Vector3.up * num34 * 0.5f, Color.white, false);
				Vector3 vector32 = vector22 - quaternion11 * Vector3.right * (num33 * 0.5f + num34 * 0.5f);
				DebugUtil.DrawRect(vector32, quaternion11 * Quaternion.AngleAxis(90f, Vector3.right), new Vector2(num34, num33), Color.white, false, DebugUtil.Style.Wireframe);
				float num36 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f * this.subtlePlayerPitchFactor, 90f * this.subtlePlayerPitchFactor, this.subtlePlayerPitch));
				vector31 = vector32 + quaternion11 * Vector3.up * num36 * num33 * 0.5f;
				DebugUtil.DrawLine(vector31 - quaternion11 * Vector3.right * num34 * 0.5f, vector31 + quaternion11 * Vector3.right * num34 * 0.5f, Color.white, false);
			}
			this.syncedState.riderId = PhotonNetwork.LocalPlayer.ActorNumber;
			this.syncedState.materialIndex = (flag ? 1 : this.ridersMaterialIndex);
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer.Rig != null)
			{
				this.syncedState.position = rigContainer.Rig.transform.InverseTransformPoint(base.transform.position);
				this.syncedState.rotation = Quaternion.Inverse(rigContainer.Rig.transform.rotation) * base.transform.rotation;
			}
			else
			{
				Debug.LogError("Glider failed to get a reference to the local player's VRRig while the player was flying");
			}
			this.syncedState.audioLevel = (byte)Mathf.FloorToInt(Mathf.Clamp(num3 * num2 * 255f, 0f, 255f));
			if (this.OutOfBounds)
			{
				this.Respawn();
			}
		}
	}

	private void RemoteSyncUpdate(float dt)
	{
		this.rb.isKinematic = true;
		if (this.syncedState.riderId == -1)
		{
			base.transform.position = Vector3.Lerp(this.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.rotation = Quaternion.Slerp(this.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
		}
		else
		{
			Photon.Realtime.Player player = null;
			if (PhotonNetwork.CurrentRoom != null)
			{
				player = PhotonNetwork.CurrentRoom.GetPlayer(this.syncedState.riderId, false);
			}
			else if (this.syncedState.riderId == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				player = PhotonNetwork.LocalPlayer;
			}
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer) && rigContainer.Rig != null)
			{
				this.positionLocalToVRRig = Vector3.Lerp(this.syncedState.position, this.positionLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				this.rotationLocalToVRRig = Quaternion.Slerp(this.syncedState.rotation, this.rotationLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				base.transform.position = rigContainer.Rig.transform.TransformPoint(this.positionLocalToVRRig);
				base.transform.rotation = rigContainer.Rig.transform.rotation * this.rotationLocalToVRRig;
			}
		}
		this.leafMesh.material = ((this.syncedState.riderId != -1) ? this.GetMaterialFromIndex(this.syncedState.materialIndex) : this.baseLeafMaterial);
		float num = Mathf.Clamp01((float)this.syncedState.audioLevel / 255f);
		if (this.syncedState.riderId != -1 && this.ridersMaterialIndex == 1)
		{
			this.UpdateAudioSource(this.calmAudio, num * this.infectedAudioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, num * this.infectedAudioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, num * this.infectedAudioVolumeMultiplier);
			return;
		}
		this.UpdateAudioSource(this.calmAudio, num * this.audioVolumeMultiplier);
		this.UpdateAudioSource(this.activeAudio, num * this.audioVolumeMultiplier);
		this.UpdateAudioSource(this.whistlingAudio, 0f);
	}

	private Vector3 ClosestPointInHandle(Vector3 startingPoint, InteractionPoint interactionPoint)
	{
		CapsuleCollider component = interactionPoint.GetComponent<CapsuleCollider>();
		Vector3 vector = startingPoint;
		if (component != null)
		{
			Vector3 vector2 = ((component.direction == 0) ? Vector3.right : ((component.direction == 1) ? Vector3.up : Vector3.forward));
			Vector3 vector3 = component.transform.rotation * vector2;
			Vector3 vector4 = component.transform.position + component.transform.rotation * component.center;
			float num = Mathf.Clamp(Vector3.Dot(vector - vector4, vector3), -component.height * 0.5f, component.height * 0.5f);
			vector = vector4 + vector3 * num;
		}
		return vector;
	}

	private void UpdateGliderPosition()
	{
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 vector = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			Vector3 vector2 = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
			base.transform.position = (vector + vector2) * 0.5f;
			return;
		}
		if (this.leftHold.active)
		{
			base.transform.position = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			return;
		}
		if (this.rightHold.active)
		{
			base.transform.position = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
		}
	}

	private Vector3 GetHandsVector(Vector3 leftHandPos, Vector3 rightHandPos, Vector3 headPos, bool flipBasedOnFacingDir)
	{
		Vector3 vector = rightHandPos - leftHandPos;
		Vector3 vector2 = (rightHandPos + leftHandPos) * 0.5f - headPos;
		Vector3 normalized = Vector3.Cross(Vector3.up, vector2).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(vector, normalized) < 0f)
		{
			vector = -vector;
		}
		return vector;
	}

	private void GetHandsOrientationVectors(Vector3 leftHandPos, Vector3 rightHandPos, Transform head, bool flipBasedOnFacingDir, out Vector3 handsVector, out Vector3 handsUpVector)
	{
		handsVector = rightHandPos - leftHandPos;
		float magnitude = handsVector.magnitude;
		handsVector /= Mathf.Max(magnitude, 0.001f);
		Vector3 position = head.position;
		float num = 1f;
		Vector3 vector = ((Vector3.Dot(head.right, handsVector) < 0f) ? handsVector : (-handsVector));
		Vector3 normalized = Vector3.ProjectOnPlane(-head.forward, vector).normalized;
		Vector3 vector2 = normalized * num + position;
		Vector3 vector3 = (leftHandPos + rightHandPos) * 0.5f;
		Vector3 vector4 = Vector3.ProjectOnPlane(vector3 - head.position, Vector3.up);
		float magnitude2 = vector4.magnitude;
		vector4 /= Mathf.Max(magnitude2, 0.001f);
		Vector3 normalized2 = Vector3.ProjectOnPlane(-base.transform.forward, Vector3.up).normalized;
		Vector3 vector5 = -vector4 * num + position;
		float num2 = Vector3.Dot(normalized2, -vector4);
		float num3 = Vector3.Dot(normalized2, normalized);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0f)
		{
			num2 = Mathf.Abs(num2);
			num3 = Mathf.Abs(num3);
		}
		num2 = Mathf.Max(num2, 0f);
		num3 = Mathf.Max(num3, 0f);
		Vector3 vector6 = (vector5 * num2 + vector2 * num3) / Mathf.Max(num2 + num3, 0.001f);
		Vector3 vector7 = vector3 - vector6;
		Vector3 normalized3 = Vector3.Cross(Vector3.up, vector7).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(handsVector, normalized3) < 0f)
		{
			handsVector = -handsVector;
		}
		handsUpVector = Vector3.Cross(Vector3.ProjectOnPlane(vector7, Vector3.up), handsVector).normalized;
	}

	private Material GetMaterialFromIndex(byte materialIndex)
	{
		if (materialIndex == 1)
		{
			return this.infectedLeafMaterial;
		}
		if (this.cosmeticMaterialOverrides.Length != 0 && materialIndex > 1 && (int)(materialIndex - 2) < this.cosmeticMaterialOverrides.Length && this.cosmeticMaterialOverrides[(int)(materialIndex - 2)].material != null)
		{
			return this.cosmeticMaterialOverrides[(int)(materialIndex - 2)].material;
		}
		return this.baseLeafMaterial;
	}

	private float GetRollAngle180Wrapping()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float num = Vector3.SignedAngle(Vector3.Cross(Vector3.up, normalized).normalized, base.transform.right, base.transform.forward);
		return this.NormalizeAngle180(num);
	}

	private float SignedAngleInPlane(Vector3 from, Vector3 to, Vector3 normal)
	{
		from = Vector3.ProjectOnPlane(from, normal);
		to = Vector3.ProjectOnPlane(to, normal);
		return Vector3.SignedAngle(from, to, normal);
	}

	private float NormalizeAngle180(float angle)
	{
		angle = (angle + 180f) % 360f;
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle - 180f;
	}

	private void UpdateAudioSource(AudioSource source, float level)
	{
		source.volume = level;
		if (!source.isPlaying && level > 0.01f)
		{
			source.Play();
			return;
		}
		if (source.isPlaying && level < 0.01f && this.syncedState.riderId == -1)
		{
			source.Stop();
		}
	}

	public void OnTriggerStay(Collider other)
	{
		GliderWindVolume component = other.GetComponent<GliderWindVolume>();
		if (component == null)
		{
			return;
		}
		if (!base.photonView.IsMine && PhotonNetwork.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		if (Time.frameCount == this.windVolumeForceAppliedFrame)
		{
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			Vector3 accelFromVelocity = component.GetAccelFromVelocity(GorillaLocomotion.Player.Instance.Velocity);
			GorillaLocomotion.Player.Instance.AddForce(accelFromVelocity, ForceMode.Acceleration);
			this.windVolumeForceAppliedFrame = Time.frameCount;
			return;
		}
		Vector3 accelFromVelocity2 = component.GetAccelFromVelocity(this.rb.velocity);
		Vector3 vector = this.WindResistanceForceOffset(base.transform.up, component.WindDirection);
		Vector3 vector2 = base.transform.position + vector * this.windUprightTorqueMultiplier;
		this.rb.AddForceAtPosition(accelFromVelocity2, vector2, ForceMode.Acceleration);
		this.windVolumeForceAppliedFrame = Time.frameCount;
	}

	private Vector3 WindResistanceForceOffset(Vector3 upDir, Vector3 windDir)
	{
		if (Vector3.Dot(upDir, windDir) < 0f)
		{
			upDir *= -1f;
		}
		return Vector3.ProjectOnPlane(upDir - windDir, upDir);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.syncedState.riderId);
			stream.SendNext(this.syncedState.materialIndex);
			stream.SendNext(this.syncedState.audioLevel);
			stream.SendNext(this.syncedState.position);
			stream.SendNext(this.syncedState.rotation);
			return;
		}
		int riderId = this.syncedState.riderId;
		this.syncedState.riderId = (int)stream.ReceiveNext();
		this.syncedState.materialIndex = (byte)stream.ReceiveNext();
		this.syncedState.audioLevel = (byte)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.syncedState.position).SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		(ref this.syncedState.rotation).SetValueSafe(quaternion);
		if (riderId != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	private IEnumerator ReenableOwnershipRequest()
	{
		yield return new WaitForSeconds(3f);
		this.pendingOwnershipRequest = false;
		yield break;
	}

	public void OnOwnershipTransferred(Photon.Realtime.Player toPlayer, Photon.Realtime.Player fromPlayer)
	{
		if (toPlayer == PhotonNetwork.LocalPlayer)
		{
			this.pendingOwnershipRequest = false;
		}
	}

	public bool OnOwnershipRequest(Photon.Realtime.Player fromPlayer)
	{
		return !base.photonView.IsMine || !PhotonNetwork.InRoom || (!this.leftHold.active && !this.rightHold.active);
	}

	public void OnMyOwnerLeft()
	{
	}

	public bool OnMasterClientAssistedTakeoverRequest(Photon.Realtime.Player fromPlayer, Photon.Realtime.Player toPlayer)
	{
		return false;
	}

	public void OnMyCreatorLeft()
	{
	}

	public void ToggleSubtlePlayerPitch()
	{
		this.subtlePlayerPitchActive = !this.subtlePlayerPitchActive;
		this.RefreshSubtlePlayerPitchButton();
	}

	private void RefreshSubtlePlayerPitchButton()
	{
		if (this.subtlePlayerPitchButton != null)
		{
			this.subtlePlayerPitchButton.isOn = this.subtlePlayerPitchActive;
			this.subtlePlayerPitchButton.UpdateColor();
		}
	}

	public void ToggleSubtlePlayerRoll()
	{
		this.subtlePlayerRollActive = !this.subtlePlayerRollActive;
		this.RefreshSubtlePlayerRollButton();
	}

	private void RefreshSubtlePlayerRollButton()
	{
		if (this.subtlePlayerRollButton != null)
		{
			this.subtlePlayerRollButton.isOn = this.subtlePlayerRollActive;
			this.subtlePlayerRollButton.UpdateColor();
		}
	}

	public void ToggleLeanToTurn()
	{
		this.directOrienting = !this.directOrienting;
		this.RefreshLeanToTurnButton();
	}

	private void RefreshLeanToTurnButton()
	{
		if (this.leanToTurnButton != null)
		{
			this.leanToTurnButton.isOn = !this.directOrienting;
			this.leanToTurnButton.UpdateColor();
		}
	}

	public void ToggleTurnPlayer()
	{
		this.rotatePlayerYaw = !this.rotatePlayerYaw;
		this.RefreshTurnPlayerButton();
	}

	private void RefreshTurnPlayerButton()
	{
		if (this.turnPlayerButton != null)
		{
			this.turnPlayerButton.isOn = this.rotatePlayerYaw;
			this.turnPlayerButton.UpdateColor();
		}
	}

	public void ToggleTurnPitchPlayer()
	{
		this.rotatePlayerPitch = !this.rotatePlayerPitch;
		this.RefreshTurnPitchPlayerButton();
	}

	private void RefreshTurnPitchPlayerButton()
	{
		if (this.turnPitchPlayerButton != null)
		{
			this.turnPitchPlayerButton.isOn = this.rotatePlayerPitch;
			this.turnPitchPlayerButton.UpdateColor();
		}
	}

	public void ToggleTurnPitchRollPlayer()
	{
		this.rotatePlayerFully = !this.rotatePlayerFully;
		this.RefreshTurnPitchRollPlayerButton();
	}

	private void RefreshTurnPitchRollPlayerButton()
	{
		if (this.turnPitchRollPlayerButton != null)
		{
			this.turnPitchRollPlayerButton.isOn = this.rotatePlayerFully;
			this.turnPitchRollPlayerButton.UpdateColor();
		}
	}

	public void ToggleFloatingPitchControl()
	{
		this.velocityBasedPitch = !this.velocityBasedPitch;
		this.RefreshFloatingPitchControlButton();
	}

	private void RefreshFloatingPitchControlButton()
	{
		if (this.floatingPitchControlButton != null)
		{
			this.floatingPitchControlButton.isOn = this.velocityBasedPitch;
			this.floatingPitchControlButton.UpdateColor();
		}
	}

	public GliderHoldable()
	{
	}

	[CompilerGenerated]
	private void <OnHover>b__166_0()
	{
		this.pendingOwnershipRequest = false;
	}

	[CompilerGenerated]
	private void <OnGrab>b__167_0()
	{
		this.pendingOwnershipRequest = false;
	}

	[Header("Flight Settings")]
	[SerializeField]
	private Vector2 pitchMinMax = new Vector2(-80f, 80f);

	[SerializeField]
	private Vector2 rollMinMax = new Vector2(-70f, 70f);

	[SerializeField]
	private float pitchHalfLife = 0.2f;

	[SerializeField]
	private float yawHalfLife = 0.2f;

	[SerializeField]
	private float rollHalfLife = 0.2f;

	[SerializeField]
	private float yawRateFactor = 0.1f;

	[SerializeField]
	private float yawRateFactorRollOnly = 1f;

	public Vector2 pitchVelocityTargetMinMax = new Vector2(-60f, 60f);

	public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-1f, 1f);

	[SerializeField]
	private float pitchVelocityFollowRateAngle = 60f;

	[SerializeField]
	private float pitchVelocityFollowRateMagnitude = 5f;

	[SerializeField]
	private float yawVelocityFollowRateAngle = 60f;

	[SerializeField]
	private float yawVelocityFollowRateMagnitude = 5f;

	[SerializeField]
	private AnimationCurve liftVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve dragVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	[Range(0f, 1f)]
	public float attackDragFactor = 0.1f;

	[SerializeField]
	private AnimationCurve dragVsSpeed = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	public float dragVsSpeedMaxSpeed = 30f;

	[SerializeField]
	[Range(0f, 1f)]
	public float dragVsSpeedDragFactor = 0.2f;

	[SerializeField]
	private AnimationCurve liftIncreaseVsRoll = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float liftIncreaseVsRollMaxAngle = 20f;

	[SerializeField]
	[Range(0f, 1f)]
	private float gravityCompensation = 0.8f;

	[Range(0f, 1f)]
	public float pullUpLiftBonus = 0.1f;

	public float pullUpLiftActivationVelocity = 1f;

	public float pullUpLiftActivationAcceleration = 3f;

	[Header("Body Positioning Control")]
	[SerializeField]
	private float riderPosPitchMax = 45f;

	[SerializeField]
	private float riderPosRollMax = 45f;

	[SerializeField]
	private float riderPosDirectPitchMax = 70f;

	[SerializeField]
	private Vector2 riderPosRange = new Vector2(2.2f, 0.75f);

	[SerializeField]
	private float riderPosRangeOffset = 0.15f;

	[SerializeField]
	private Vector2 riderPosRangeNormalizedDeadzone = new Vector2(0.15f, 0.05f);

	[Header("Direct Handle Control")]
	[SerializeField]
	private float holdRotationAngleMax = 45f;

	[SerializeField]
	private float oneHandHoldRotationRate = 2f;

	private Vector3 oneHandSimulatedHoldOffset = new Vector3(0.5f, -0.35f, 0.25f);

	private float oneHandPitchMultiplier = 0.8f;

	private bool oneHandRollWithRotation = true;

	[SerializeField]
	private float twoHandHoldRotationRate = 4f;

	[SerializeField]
	private bool twoHandGliderInversionOnYawInsteadOfRoll;

	[Header("Player Settings")]
	[SerializeField]
	private bool setMaxHandSlipDuringFlight = true;

	[SerializeField]
	private float maxSlipOverrideSpeedThreshold = 5f;

	[Header("Player Camera Rotation")]
	[SerializeField]
	private float subtlePlayerPitchFactor = 0.2f;

	[SerializeField]
	private float subtlePlayerPitchRate = 2f;

	[SerializeField]
	private float subtlePlayerRollFactor = 0.2f;

	[SerializeField]
	private float subtlePlayerRollRate = 2f;

	[SerializeField]
	private Vector2 subtlePlayerRotationSpeedRampMinMax = new Vector2(2f, 8f);

	[SerializeField]
	private Vector2 subtlePlayerRollAccelMinMax = new Vector2(0f, 30f);

	[SerializeField]
	private Vector2 subtlePlayerPitchAccelMinMax = new Vector2(0f, 10f);

	[SerializeField]
	private float accelSmoothingFollowRate = 2f;

	[SerializeField]
	private float horizontalAccelSmoothingFollowRate = 2f;

	[Header("Haptics")]
	[SerializeField]
	private Vector2 hapticAccelInputRange = new Vector2(5f, 20f);

	[SerializeField]
	private float hapticAccelOutputMax = 0.35f;

	[SerializeField]
	private Vector2 hapticMaxSpeedInputRange = new Vector2(5f, 10f);

	[SerializeField]
	private Vector2 hapticSpeedInputRange = new Vector2(3f, 30f);

	[SerializeField]
	private float hapticSpeedOutputMax = 0.15f;

	[SerializeField]
	private Vector2 whistlingAudioSpeedInputRange = new Vector2(15f, 30f);

	[Header("Audio")]
	[SerializeField]
	private float audioVolumeMultiplier = 0.25f;

	[SerializeField]
	private float infectedAudioVolumeMultiplier = 0.5f;

	[SerializeField]
	private Vector2 whooshSpeedThresholdInput = new Vector2(10f, 25f);

	[SerializeField]
	private Vector2 whooshVolumeOutput = new Vector2(0.2f, 0.75f);

	[SerializeField]
	private float whooshCheckDistance = 2f;

	[Header("Tag Adjustment")]
	[SerializeField]
	private bool extendTagRangeInFlight = true;

	[SerializeField]
	private Vector2 tagRangeSpeedInput = new Vector2(5f, 20f);

	[SerializeField]
	private Vector2 tagRangeOutput = new Vector2(0.03f, 3f);

	[SerializeField]
	private bool debugDrawTagRange = true;

	[Header("Infected State")]
	[SerializeField]
	private float infectedSpeedIncrease = 5f;

	[Header("Glider Materials")]
	[SerializeField]
	private MeshRenderer leafMesh;

	[SerializeField]
	private Material baseLeafMaterial;

	[SerializeField]
	private Material infectedLeafMaterial;

	[SerializeField]
	private GliderHoldable.CosmeticMaterialOverride[] cosmeticMaterialOverrides;

	[Header("Network Syncing")]
	[SerializeField]
	private float networkSyncFollowRate = 2f;

	[Header("Life Cycle")]
	[SerializeField]
	private Transform maxDistanceRespawnOrigin;

	[SerializeField]
	private float maxDistanceBeforeRespawn = 180f;

	[SerializeField]
	private float maxDroppedTimeToRespawn = 120f;

	[Header("Rigidbody")]
	[SerializeField]
	private float windUprightTorqueMultiplier = 1f;

	[SerializeField]
	private float gravityUprightTorqueMultiplier = 0.5f;

	[SerializeField]
	private float fallingGravityReduction = 0.1f;

	[Header("References")]
	[SerializeField]
	private AudioSource calmAudio;

	[SerializeField]
	private AudioSource activeAudio;

	[SerializeField]
	private AudioSource whistlingAudio;

	[SerializeField]
	private AudioSource leftWhooshAudio;

	[SerializeField]
	private AudioSource rightWhooshAudio;

	[SerializeField]
	private InteractionPoint handle;

	[SerializeField]
	private RequestableOwnershipGuard ownershipGuard;

	[SerializeField]
	private GorillaPressableButton subtlePlayerPitchButton;

	[SerializeField]
	private GorillaPressableButton subtlePlayerRollButton;

	[SerializeField]
	private GorillaPressableButton leanToTurnButton;

	[SerializeField]
	private GorillaPressableButton turnPlayerButton;

	[SerializeField]
	private GorillaPressableButton turnPitchPlayerButton;

	[SerializeField]
	private GorillaPressableButton turnPitchRollPlayerButton;

	[SerializeField]
	private GorillaPressableButton floatingPitchControlButton;

	private Action<InteractionPoint, GameObject> leftGrabOnOwnershipTransfer;

	private Action<InteractionPoint, GameObject> rightGrabOnOwnershipTransfer;

	private Func<InteractionPoint, GameObject> leftGrabFunc;

	private bool directOrienting = true;

	private bool velocityBasedPitch;

	private bool subtlePlayerPitchActive = true;

	private bool subtlePlayerRollActive = true;

	private float subtlePlayerPitch;

	private float subtlePlayerRoll;

	private float subtlePlayerPitchRateExp = 0.75f;

	private float subtlePlayerRollRateExp = 0.025f;

	private bool rotatePlayerFully;

	private bool rotatePlayerPitch;

	private bool rotatePlayerYaw;

	private GliderHoldable.HoldingHand leftHold;

	private GliderHoldable.HoldingHand rightHold;

	private GliderHoldable.SyncedState syncedState;

	private Vector3 twoHandRotationOffsetAxis = Vector3.forward;

	private float twoHandRotationOffsetAngle;

	private bool invertedHold;

	private Rigidbody rb;

	private Vector2 riderPosition = Vector2.zero;

	private Vector3 previousVelocity;

	private Vector3 currentVelocity;

	private float pitch;

	private float yaw;

	private float roll;

	private float pitchVel;

	private float yawVel;

	private float rollVel;

	private float oneHandRotationRateExp;

	private float twoHandRotationRateExp;

	private Quaternion playerFacingRotationOffset = Quaternion.identity;

	private const float accelAveragingWindow = 0.1f;

	private AverageVector3 accelerationAverage = new AverageVector3(0.1f);

	private float accelerationSmoothed;

	private float turnAccelerationSmoothed;

	private float accelSmoothingFollowRateExp = 1f;

	private float networkSyncFollowRateExp = 2f;

	private bool pendingOwnershipRequest;

	private Vector3 positionLocalToVRRig = Vector3.zero;

	private Quaternion rotationLocalToVRRig = Quaternion.identity;

	private Coroutine reenableOwnershipRequestCoroutine;

	private Vector3 spawnPosition;

	private Quaternion spawnRotation;

	private float lastHeldTime = -1f;

	private bool limitedOneHandControls = true;

	private Vector3? leftHoldPositionLocal;

	private Vector3? rightHoldPositionLocal;

	private float whooshSoundDuration = 1f;

	private float whooshSoundRetriggerThreshold = 0.5f;

	private float whooshAudioDist = 1f;

	private float leftWhooshStartTime = -1f;

	private Vector3 leftWhooshHitPoint = Vector3.zero;

	private Vector3 whooshAudioPositionOffset = new Vector3(0.5f, -0.25f, 0.5f);

	private float rightWhooshStartTime = -1f;

	private Vector3 rightWhooshHitPoint = Vector3.zero;

	private byte ridersMaterialIndex;

	private bool debugDraw;

	private float pitchNormalizedDebug;

	private float attackNormalizedDebug;

	private float liftMultiplierDebug;

	private float dragAttackMultiplierDebug;

	private float dragSpeedMultiplierDebug;

	private float dragNetMultiplierDebug;

	private int windVolumeForceAppliedFrame = -1;

	private enum GliderState
	{
		AtSpawn,
		LocallyHeld,
		LocallyDropped,
		RemoteSyncing
	}

	private struct HoldingHand
	{
		public void Activate(Transform handTransform, Transform gliderTransform, Vector3 worldGrabPoint)
		{
			this.active = true;
			this.transform = handTransform.transform;
			this.holdLocalPos = handTransform.InverseTransformPoint(worldGrabPoint);
			this.handleLocalPos = gliderTransform.InverseTransformVector(gliderTransform.position - worldGrabPoint);
			this.localHoldRotation = Quaternion.Inverse(handTransform.rotation) * gliderTransform.rotation;
		}

		public void Deactivate()
		{
			this.active = false;
			this.transform = null;
			this.holdLocalPos = Vector3.zero;
			this.handleLocalPos = Vector3.zero;
			this.localHoldRotation = Quaternion.identity;
		}

		public bool active;

		public Transform transform;

		public Vector3 holdLocalPos;

		public Vector3 handleLocalPos;

		public Quaternion localHoldRotation;
	}

	private struct SyncedState
	{
		public int riderId;

		public byte materialIndex;

		public byte audioLevel;

		public Vector3 position;

		public Quaternion rotation;
	}

	[Serializable]
	private struct CosmeticMaterialOverride
	{
		public string cosmeticName;

		public Material material;
	}

	[CompilerGenerated]
	private sealed class <ReenableOwnershipRequest>d__187 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <ReenableOwnershipRequest>d__187(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			GliderHoldable gliderHoldable = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(3f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			gliderHoldable.pendingOwnershipRequest = false;
			return false;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public GliderHoldable <>4__this;
	}
}
