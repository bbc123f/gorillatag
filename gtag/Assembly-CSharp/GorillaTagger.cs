﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CjLib;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Steamworks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class GorillaTagger : MonoBehaviour, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	public static GorillaTagger Instance
	{
		get
		{
			return GorillaTagger._instance;
		}
	}

	public PhotonView myVRRig
	{
		get
		{
			return this.offlineVRRig.photonView;
		}
	}

	internal VRRigSerializer rigSerializer
	{
		get
		{
			return this.offlineVRRig.rigSerializer;
		}
	}

	public Rigidbody rigidbody
	{
		[CompilerGenerated]
		get
		{
			return this.<rigidbody>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<rigidbody>k__BackingField = value;
		}
	}

	public Recorder myRecorder
	{
		[CompilerGenerated]
		get
		{
			return this.<myRecorder>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<myRecorder>k__BackingField = value;
		}
	}

	public float sphereCastRadius
	{
		get
		{
			if (this.tagRadiusOverride == null)
			{
				return 0.03f;
			}
			return this.tagRadiusOverride.Value;
		}
	}

	public void SetTagRadiusOverrideThisFrame(float radius)
	{
		this.tagRadiusOverride = new float?(radius);
		this.tagRadiusOverrideFrame = Time.frameCount;
	}

	protected void Awake()
	{
		this.GuidedRefInitialize();
		this.MirrorCameraCullingMask = new Watchable<int>(this.BaseMirrorCameraCullingMask);
		if (GorillaTagger._instance != null && GorillaTagger._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GorillaTagger._instance = this;
			GorillaTagger.hasInstance = true;
			GorillaTagger.onPlayerSpawnedRootCallback();
		}
		if (!this.disableTutorial && (this.testTutorial || (PlayerPrefs.GetString("tutorial") != "done" && PlayerPrefs.GetString("didTutorial") != "done" && NetworkSystemConfig.GameVersionString != "dev")))
		{
			base.transform.parent.position = new Vector3(-140f, 28f, -102f);
			base.transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
			GorillaLocomotion.Player.Instance.InitializeValues();
			PlayerPrefs.SetFloat("redValue", Random.value);
			PlayerPrefs.SetFloat("greenValue", Random.value);
			PlayerPrefs.SetFloat("blueValue", Random.value);
			PlayerPrefs.Save();
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("didTutorial", true);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
		this.thirdPersonCamera.SetActive(Application.platform != RuntimePlatform.Android);
		this.inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		this.wasInOverlay = false;
		this.baseSlideControl = GorillaLocomotion.Player.Instance.slideControl;
		this.gorillaTagColliderLayerMask = LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		});
		this.rigidbody = base.GetComponent<Rigidbody>();
	}

	protected void OnDestroy()
	{
		if (GorillaTagger._instance == this)
		{
			GorillaTagger._instance = null;
			GorillaTagger.hasInstance = false;
		}
	}

	private void IsXRSubsystemActive()
	{
		this.loadedDeviceName = XRSettings.loadedDeviceName;
		List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>();
		SubsystemManager.GetInstances<XRDisplaySubsystem>(list);
		using (List<XRDisplaySubsystem>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.running)
				{
					this.xrSubsystemIsActive = true;
					return;
				}
			}
		}
		this.xrSubsystemIsActive = false;
	}

	protected void Start()
	{
		this.IsXRSubsystemActive();
		if (this.loadedDeviceName == "OpenVR Display")
		{
			GorillaLocomotion.Player.Instance.leftHandOffset = new Vector3(-0.02f, 0f, -0.07f);
			GorillaLocomotion.Player.Instance.rightHandOffset = new Vector3(0.02f, 0f, -0.07f);
			Quaternion rotation = Quaternion.Euler(new Vector3(-90f, 180f, -20f));
			Quaternion rotation2 = Quaternion.Euler(new Vector3(-90f, 180f, 20f));
			Quaternion lhs = Quaternion.Euler(new Vector3(-141f, 204f, -27f));
			Quaternion lhs2 = Quaternion.Euler(new Vector3(-141f, 156f, 27f));
			GorillaLocomotion.Player.Instance.leftHandRotOffset = lhs * Quaternion.Inverse(rotation);
			GorillaLocomotion.Player.Instance.rightHandRotOffset = lhs2 * Quaternion.Inverse(rotation2);
		}
		this.bodyVector = new Vector3(0f, this.bodyCollider.height / 2f - this.bodyCollider.radius, 0f);
		if (SteamManager.Initialized)
		{
			this.gameOverlayActivatedCb = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		this.isGameOverlayActive = (pCallback.m_bActive > 0);
	}

	protected void LateUpdate()
	{
		if (this.isGameOverlayActive)
		{
			if (this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(false);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GorillaLocomotion.Player.Instance.inOverlay = true;
		}
		else
		{
			if (!this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(true);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GorillaLocomotion.Player.Instance.inOverlay = false;
		}
		if (this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android)
		{
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / XRDevice.refreshRate) > 0.0001f)
			{
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" refresh rate         :\t" + XRDevice.refreshRate.ToString());
				Time.fixedDeltaTime = 1f / XRDevice.refreshRate;
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" history size before  :\t" + GorillaLocomotion.Player.Instance.velocityHistorySize.ToString());
				GorillaLocomotion.Player.Instance.velocityHistorySize = Mathf.Max(Mathf.Min(Mathf.FloorToInt(XRDevice.refreshRate * 0.083333336f), 10), 6);
				if (GorillaLocomotion.Player.Instance.velocityHistorySize > 9)
				{
					GorillaLocomotion.Player.Instance.velocityHistorySize--;
				}
				Debug.Log("new history size: " + GorillaLocomotion.Player.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GorillaLocomotion.Player.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GorillaLocomotion.Player.Instance.InitializeValues();
			}
		}
		else if (Application.platform != RuntimePlatform.Android && OVRManager.instance != null && OVRManager.OVRManagerinitialized && OVRManager.instance.gameObject != null && OVRManager.instance.gameObject.activeSelf)
		{
			Object.Destroy(OVRManager.instance.gameObject);
		}
		if (!this.frameRateUpdated && Application.platform == RuntimePlatform.Android && OVRManager.instance.gameObject.activeSelf)
		{
			int num = OVRManager.display.displayFrequenciesAvailable.Length - 1;
			float num2 = OVRManager.display.displayFrequenciesAvailable[num];
			float systemDisplayFrequency = OVRPlugin.systemDisplayFrequency;
			if (systemDisplayFrequency != 60f)
			{
				if (systemDisplayFrequency == 71f)
				{
					num2 = 72f;
				}
			}
			else
			{
				num2 = 60f;
			}
			while (num2 > 90f)
			{
				num--;
				if (num < 0)
				{
					break;
				}
				num2 = OVRManager.display.displayFrequenciesAvailable[num];
			}
			float num3 = 1f;
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / num2 * num3) > 0.0001f)
			{
				float num4 = Time.fixedDeltaTime - 1f / num2 * num3;
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log("!!!!Time.fixedDeltaTime - (1f / newRefreshRate) * " + num3.ToString() + ")" + num4.ToString());
				Debug.Log("Old Refresh rate: " + systemDisplayFrequency.ToString());
				Debug.Log("New Refresh rate: " + num2.ToString());
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" fixedDeltaTime after :\t" + (1f / num2).ToString());
				Time.fixedDeltaTime = 1f / num2 * num3;
				OVRPlugin.systemDisplayFrequency = num2;
				GorillaLocomotion.Player.Instance.velocityHistorySize = Mathf.FloorToInt(num2 * 0.083333336f);
				if (GorillaLocomotion.Player.Instance.velocityHistorySize > 9)
				{
					GorillaLocomotion.Player.Instance.velocityHistorySize--;
				}
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" history size before  :\t" + GorillaLocomotion.Player.Instance.velocityHistorySize.ToString());
				Debug.Log("new history size: " + GorillaLocomotion.Player.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GorillaLocomotion.Player.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GorillaLocomotion.Player.Instance.InitializeValues();
				OVRManager.instance.gameObject.SetActive(false);
				this.frameRateUpdated = true;
			}
		}
		if (!this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android && Mathf.Abs(Time.fixedDeltaTime - 0.0069444445f) > 0.0001f)
		{
			Debug.Log("updating delta time. was: " + Time.fixedDeltaTime.ToString() + ". now it's " + 0.0069444445f.ToString());
			Application.targetFrameRate = 144;
			Time.fixedDeltaTime = 0.0069444445f;
			GorillaLocomotion.Player.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt(12f), 10);
			if (GorillaLocomotion.Player.Instance.velocityHistorySize > 9)
			{
				GorillaLocomotion.Player.Instance.velocityHistorySize--;
			}
			Debug.Log("new history size: " + GorillaLocomotion.Player.Instance.velocityHistorySize.ToString());
			GorillaLocomotion.Player.Instance.slideControl = 1f - this.CalcSlideControl(144f);
			GorillaLocomotion.Player.Instance.InitializeValues();
		}
		this.leftRaycastSweep = this.leftHandTransform.position - this.lastLeftHandPositionForTag;
		this.leftHeadRaycastSweep = this.leftHandTransform.position - this.headCollider.transform.position;
		this.rightRaycastSweep = this.rightHandTransform.position - this.lastRightHandPositionForTag;
		this.rightHeadRaycastSweep = this.rightHandTransform.position - this.headCollider.transform.position;
		this.headRaycastSweep = this.headCollider.transform.position - this.lastHeadPositionForTag;
		this.bodyRaycastSweep = this.bodyCollider.transform.position - this.lastBodyPositionForTag;
		this.otherPlayer = null;
		this.touchedPlayer = null;
		if (this.tagRadiusOverrideFrame < Time.frameCount)
		{
			this.tagRadiusOverride = null;
		}
		float num5 = this.sphereCastRadius * GorillaLocomotion.Player.Instance.scale;
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.lastLeftHandPositionForTag, num5, this.leftRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.leftRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|102_0(false);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, num5, this.leftHeadRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|102_0(false);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.lastRightHandPositionForTag, num5, this.rightRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.rightRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|102_0(false);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, num5, this.rightHeadRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|102_0(false);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, this.headCollider.radius * this.headCollider.transform.localScale.x * GorillaLocomotion.Player.Instance.scale, this.headRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.headRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|102_0(true);
		this.topVector = this.lastBodyPositionForTag + this.bodyVector;
		this.bottomVector = this.lastBodyPositionForTag - this.bodyVector;
		this.nonAllocHits = Physics.CapsuleCastNonAlloc(this.topVector, this.bottomVector, this.bodyCollider.radius * 2f * GorillaLocomotion.Player.Instance.scale, this.bodyRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.bodyRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|102_0(true);
		if (this.otherPlayer != null)
		{
			GameMode.ReportTag(this.otherPlayer);
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(true) && GorillaLocomotion.Player.Instance.IsHandTouching(true) && !this.leftHandTouching && Time.time > this.lastLeftTap + this.tapCoolDown && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			this.StartVibration(true, this.tapHapticStrength, this.tapHapticDuration);
			this.tempInt = ((GorillaLocomotion.Player.Instance.leftHandSurfaceOverride != null) ? GorillaLocomotion.Player.Instance.leftHandSurfaceOverride.overrideIndex : GorillaLocomotion.Player.Instance.leftHandMaterialTouchIndex);
			if (NetworkSystem.Instance.InRoom && this.myVRRig != null)
			{
				PhotonView.Get(this.myVRRig).RPC("PlayHandTap", RpcTarget.Others, new object[]
				{
					this.tempInt,
					true,
					this.handTapVolume
				});
			}
			this.offlineVRRig.PlayHandTapLocal(this.tempInt, true, this.handTapVolume);
			this.lastLeftTap = Time.time;
			if (GorillaLocomotion.Player.Instance.leftHandSurfaceOverride != null && GorillaLocomotion.Player.Instance.leftHandSurfaceOverride.sendOnTapEvent)
			{
				Tappable component = GorillaLocomotion.Player.Instance.leftHandSurfaceOverride.gameObject.GetComponent<Tappable>();
				if (component != null)
				{
					component.OnTap(this.handTapVolume, this.lastLeftTap);
				}
			}
		}
		else if (GorillaLocomotion.Player.Instance.IsHandSliding(true) && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			this.StartVibration(true, this.tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!this.leftHandSlideSource.isPlaying)
			{
				this.leftHandSlideSource.Play();
			}
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(true))
		{
			this.leftHandSlideSource.Stop();
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(false) && GorillaLocomotion.Player.Instance.IsHandTouching(false) && !this.rightHandTouching && Time.time > this.lastRightTap + this.tapCoolDown && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			this.StartVibration(false, this.tapHapticStrength, this.tapHapticDuration);
			this.tempInt = ((GorillaLocomotion.Player.Instance.rightHandSurfaceOverride != null) ? GorillaLocomotion.Player.Instance.rightHandSurfaceOverride.overrideIndex : GorillaLocomotion.Player.Instance.rightHandMaterialTouchIndex);
			if (PhotonNetwork.InRoom && this.myVRRig != null)
			{
				PhotonView.Get(this.myVRRig).RPC("PlayHandTap", RpcTarget.Others, new object[]
				{
					this.tempInt,
					false,
					this.handTapVolume
				});
			}
			this.offlineVRRig.PlayHandTapLocal(this.tempInt, false, this.handTapVolume);
			this.lastRightTap = Time.time;
			if (GorillaLocomotion.Player.Instance.rightHandSurfaceOverride != null && GorillaLocomotion.Player.Instance.rightHandSurfaceOverride.sendOnTapEvent)
			{
				Tappable component2 = GorillaLocomotion.Player.Instance.rightHandSurfaceOverride.gameObject.GetComponent<Tappable>();
				if (component2 != null)
				{
					component2.OnTap(this.handTapVolume, this.lastRightTap);
				}
			}
		}
		else if (GorillaLocomotion.Player.Instance.IsHandSliding(false) && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			this.StartVibration(false, this.tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!this.rightHandSlideSource.isPlaying)
			{
				this.rightHandSlideSource.Play();
			}
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(false))
		{
			this.rightHandSlideSource.Stop();
		}
		this.CheckEndStatusEffect();
		this.leftHandTouching = GorillaLocomotion.Player.Instance.IsHandTouching(true);
		this.rightHandTouching = GorillaLocomotion.Player.Instance.IsHandTouching(false);
		this.lastLeftHandPositionForTag = this.leftHandTransform.position;
		this.lastRightHandPositionForTag = this.rightHandTransform.position;
		this.lastBodyPositionForTag = this.bodyCollider.transform.position;
		this.lastHeadPositionForTag = this.headCollider.transform.position;
		if (GorillaComputer.instance == null || NetworkSystem.Instance.LocalRecorder == null)
		{
			return;
		}
		if (GorillaComputer.instance.voiceChatOn == "TRUE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (GorillaComputer.instance.pttType != "ALL CHAT")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						bool transmitEnabled = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						bool transmitEnabled2 = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = true;
						return;
					}
				}
				else
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						bool transmitEnabled3 = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = true;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						bool transmitEnabled4 = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = false;
						return;
					}
				}
			}
			else
			{
				if (!this.myRecorder.TransmitEnabled)
				{
					this.myRecorder.TransmitEnabled = true;
				}
				if (!this.offlineVRRig.shouldSendSpeakingLoudness)
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					return;
				}
			}
		}
		else if (GorillaComputer.instance.voiceChatOn == "FALSE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (!this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = true;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "ALL CHAT")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
				}
				else
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
				}
			}
			else if (!this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = true;
				return;
			}
		}
		else
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = false;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
		}
	}

	private bool TryToTag(RaycastHit hitInfo, bool isBodyTag, out Photon.Realtime.Player taggedPlayer, out Photon.Realtime.Player touchedPlayer)
	{
		taggedPlayer = null;
		touchedPlayer = null;
		if (PhotonNetwork.InRoom)
		{
			VRRig componentInParent = hitInfo.collider.GetComponentInParent<VRRig>();
			this.tempCreator = ((componentInParent != null) ? componentInParent.creator : null);
			if (this.tempCreator != null && PhotonNetwork.LocalPlayer != this.tempCreator)
			{
				touchedPlayer = this.tempCreator;
				if (GorillaGameManager.instance != null && Time.time > this.taggedTime + this.tagCooldown && GorillaGameManager.instance.LocalCanTag(PhotonNetwork.LocalPlayer, this.tempCreator))
				{
					if (!isBodyTag)
					{
						this.StartVibration((this.leftHandTransform.position - hitInfo.collider.transform.position).magnitude < (this.rightHandTransform.position - hitInfo.collider.transform.position).magnitude, this.tagHapticStrength, this.tagHapticDuration);
					}
					else
					{
						this.StartVibration(true, this.tagHapticStrength, this.tagHapticDuration);
						this.StartVibration(false, this.tagHapticStrength, this.tagHapticDuration);
					}
					taggedPlayer = this.tempCreator;
					return true;
				}
			}
		}
		return false;
	}

	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		base.StartCoroutine(this.HapticPulses(forLeftController, amplitude, duration));
	}

	private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
	{
		float startTime = Time.time;
		uint channel = 0U;
		InputDevice device;
		if (forLeftController)
		{
			device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		}
		else
		{
			device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		while (Time.time < startTime + duration)
		{
			device.SendHapticImpulse(channel, amplitude, this.hapticWaitSeconds);
			yield return new WaitForSeconds(this.hapticWaitSeconds * 0.9f);
		}
		yield break;
	}

	public void DoVibration(XRNode node, float amplitude, float duration)
	{
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(node);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.SendHapticImpulse(0U, amplitude, duration);
		}
	}

	public void UpdateColor(float red, float green, float blue)
	{
		this.offlineVRRig.InitializeNoobMaterialLocal(red, green, blue);
		if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom)
		{
			this.offlineVRRig.mainSkin.material = this.offlineVRRig.materialsToChangeTo[0];
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxTriggered();
		}
	}

	public void ShowCosmeticParticles(bool showParticles)
	{
		if (showParticles)
		{
			this.mainCamera.GetComponent<Camera>().cullingMask |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			this.MirrorCameraCullingMask.value |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			return;
		}
		this.mainCamera.GetComponent<Camera>().cullingMask &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
		this.MirrorCameraCullingMask.value &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
	}

	public void ApplyStatusEffect(GorillaTagger.StatusEffect newStatus, float duration)
	{
		this.EndStatusEffect(this.currentStatus);
		this.currentStatus = newStatus;
		this.statusEndTime = Time.time + duration;
		switch (newStatus)
		{
		case GorillaTagger.StatusEffect.None:
		case GorillaTagger.StatusEffect.Slowed:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GorillaLocomotion.Player.Instance.disableMovement = true;
			break;
		default:
			return;
		}
	}

	private void CheckEndStatusEffect()
	{
		if (Time.time > this.statusEndTime)
		{
			this.EndStatusEffect(this.currentStatus);
		}
	}

	private void EndStatusEffect(GorillaTagger.StatusEffect effectToEnd)
	{
		switch (effectToEnd)
		{
		case GorillaTagger.StatusEffect.None:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GorillaLocomotion.Player.Instance.disableMovement = false;
			this.currentStatus = GorillaTagger.StatusEffect.None;
			return;
		case GorillaTagger.StatusEffect.Slowed:
			this.currentStatus = GorillaTagger.StatusEffect.None;
			break;
		default:
			return;
		}
	}

	private float CalcSlideControl(float fps)
	{
		return Mathf.Pow(Mathf.Pow(1f - this.baseSlideControl, 120f), 1f / fps);
	}

	public static void OnPlayerSpawned(Action action)
	{
		if (GorillaTagger._instance)
		{
			action();
			return;
		}
		GorillaTagger.onPlayerSpawnedRootCallback = (Action)Delegate.Combine(GorillaTagger.onPlayerSpawnedRootCallback, action);
	}

	public void DebugDrawTagCasts(Color color)
	{
		float num = this.sphereCastRadius * GorillaLocomotion.Player.Instance.scale;
		this.DrawSphereCast(this.lastLeftHandPositionForTag, this.leftRaycastSweep.normalized, num, Mathf.Max(this.leftRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.leftHeadRaycastSweep.normalized, num, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.lastRightHandPositionForTag, this.rightRaycastSweep.normalized, num, Mathf.Max(this.rightRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.rightHeadRaycastSweep.normalized, num, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num), color);
	}

	private void DrawSphereCast(Vector3 start, Vector3 dir, float radius, float dist, Color color)
	{
		DebugUtil.DrawCapsule(start, start + dir * dist, radius, 16, 16, color, true, DebugUtil.Style.Wireframe);
	}

	public void GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverField<GorillaTagger>(this, "offlineVRRig", ref this.offlineVRRig_gRef);
		GuidedRefHub.ReceiverFullyRegistered<GorillaTagger>(this);
	}

	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount
	{
		[CompilerGenerated]
		get
		{
			return this.<GorillaTag.GuidedRefs.IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<GorillaTag.GuidedRefs.IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount>k__BackingField = value;
		}
	}

	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		if (this.offlineVRRig_gRef.fieldId == target.fieldId && this.offlineVRRig == null)
		{
			this.offlineVRRig = (target.targetMono.GuidedRefTargetObject as VRRig);
			return this.offlineVRRig != null;
		}
		return false;
	}

	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
	}

	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
	}

	public GorillaTagger()
	{
	}

	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHits|102_0(bool isBodyTag)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			if (this.nonAllocRaycastHits[i].collider.gameObject.activeSelf && this.TryToTag(this.nonAllocRaycastHits[i], isBodyTag, out this.tryPlayer, out this.touchedPlayer))
			{
				this.otherPlayer = this.tryPlayer;
				return;
			}
		}
	}

	[OnEnterPlay_SetNull]
	private static GorillaTagger _instance;

	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	public bool inCosmeticsRoom;

	public SphereCollider headCollider;

	public CapsuleCollider bodyCollider;

	private Vector3 lastLeftHandPositionForTag;

	private Vector3 lastRightHandPositionForTag;

	private Vector3 lastBodyPositionForTag;

	private Vector3 lastHeadPositionForTag;

	public Transform rightHandTransform;

	public Transform leftHandTransform;

	public float hapticWaitSeconds = 0.05f;

	public float handTapVolume = 0.1f;

	public float tapCoolDown = 0.15f;

	public float lastLeftTap;

	public float lastRightTap;

	public float tapHapticDuration = 0.05f;

	public float tapHapticStrength = 0.5f;

	public float tagHapticDuration = 0.15f;

	public float tagHapticStrength = 1f;

	public float taggedHapticDuration = 0.35f;

	public float taggedHapticStrength = 1f;

	private bool leftHandTouching;

	private bool rightHandTouching;

	public float taggedTime;

	public float tagCooldown;

	public float slowCooldown = 3f;

	public VRRig offlineVRRig;

	[FormerlySerializedAs("offlineVRRig_guidedRef")]
	public GuidedRefReceiverFieldInfo offlineVRRig_gRef = new GuidedRefReceiverFieldInfo(false);

	public GameObject thirdPersonCamera;

	public GameObject mainCamera;

	public bool testTutorial;

	public bool disableTutorial;

	public bool frameRateUpdated;

	public GameObject leftHandTriggerCollider;

	public GameObject rightHandTriggerCollider;

	public AudioSource leftHandSlideSource;

	public AudioSource rightHandSlideSource;

	public bool overrideNotInFocus;

	[CompilerGenerated]
	private Rigidbody <rigidbody>k__BackingField;

	private Vector3 leftRaycastSweep;

	private Vector3 leftHeadRaycastSweep;

	private Vector3 rightRaycastSweep;

	private Vector3 rightHeadRaycastSweep;

	private Vector3 headRaycastSweep;

	private Vector3 bodyRaycastSweep;

	private InputDevice rightDevice;

	private InputDevice leftDevice;

	private bool primaryButtonPressRight;

	private bool secondaryButtonPressRight;

	private bool primaryButtonPressLeft;

	private bool secondaryButtonPressLeft;

	private RaycastHit hitInfo;

	public Photon.Realtime.Player otherPlayer;

	private Photon.Realtime.Player tryPlayer;

	private Photon.Realtime.Player touchedPlayer;

	private Vector3 topVector;

	private Vector3 bottomVector;

	private Vector3 bodyVector;

	private int tempInt;

	private InputDevice inputDevice;

	private bool wasInOverlay;

	private PhotonView tempView;

	private Photon.Realtime.Player tempCreator;

	public GorillaTagger.StatusEffect currentStatus;

	public float statusStartTime;

	public float statusEndTime;

	private float refreshRate;

	private float baseSlideControl;

	private int gorillaTagColliderLayerMask;

	private RaycastHit[] nonAllocRaycastHits = new RaycastHit[30];

	private int nonAllocHits;

	[CompilerGenerated]
	private Recorder <myRecorder>k__BackingField;

	private bool xrSubsystemIsActive;

	public string loadedDeviceName = "";

	[SerializeField]
	private LayerMask BaseMirrorCameraCullingMask;

	public Watchable<int> MirrorCameraCullingMask;

	private Callback<GameOverlayActivated_t> gameOverlayActivatedCb;

	private bool isGameOverlayActive;

	private float? tagRadiusOverride;

	private int tagRadiusOverrideFrame = -1;

	private static Action onPlayerSpawnedRootCallback;

	[CompilerGenerated]
	private int <GorillaTag.GuidedRefs.IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount>k__BackingField;

	public enum StatusEffect
	{
		None,
		Frozen,
		Slowed,
		Dead,
		Infected,
		It
	}

	[CompilerGenerated]
	private sealed class <HapticPulses>d__105 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <HapticPulses>d__105(int <>1__state)
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
			GorillaTagger gorillaTagger = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
			}
			else
			{
				this.<>1__state = -1;
				startTime = Time.time;
				channel = 0U;
				if (forLeftController)
				{
					device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
				}
				else
				{
					device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
				}
			}
			if (Time.time >= startTime + duration)
			{
				return false;
			}
			device.SendHapticImpulse(channel, amplitude, gorillaTagger.hapticWaitSeconds);
			this.<>2__current = new WaitForSeconds(gorillaTagger.hapticWaitSeconds * 0.9f);
			this.<>1__state = 1;
			return true;
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

		public bool forLeftController;

		public float amplitude;

		public GorillaTagger <>4__this;

		public float duration;

		private float <startTime>5__2;

		private InputDevice <device>5__3;

		private uint <channel>5__4;
	}
}
