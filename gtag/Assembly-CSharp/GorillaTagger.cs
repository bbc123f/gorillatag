using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Steamworks;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000185 RID: 389
public class GorillaTagger : MonoBehaviour, IGuidedRefReceiver, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060009DC RID: 2524 RVA: 0x0003C34B File Offset: 0x0003A54B
	public static GorillaTagger Instance
	{
		get
		{
			return GorillaTagger._instance;
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060009DD RID: 2525 RVA: 0x0003C352 File Offset: 0x0003A552
	public PhotonView myVRRig
	{
		get
		{
			return this.offlineVRRig.photonView;
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060009DE RID: 2526 RVA: 0x0003C35F File Offset: 0x0003A55F
	// (set) Token: 0x060009DF RID: 2527 RVA: 0x0003C367 File Offset: 0x0003A567
	public Rigidbody rigidbody { get; private set; }

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060009E0 RID: 2528 RVA: 0x0003C370 File Offset: 0x0003A570
	// (set) Token: 0x060009E1 RID: 2529 RVA: 0x0003C378 File Offset: 0x0003A578
	public Recorder myRecorder { get; private set; }

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060009E2 RID: 2530 RVA: 0x0003C381 File Offset: 0x0003A581
	public float sphereCastRadius
	{
		get
		{
			return 0.03f;
		}
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x0003C388 File Offset: 0x0003A588
	protected void Awake()
	{
		this.GuidedRefInitialize();
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
		if (!this.disableTutorial && (this.testTutorial || (PlayerPrefs.GetString("tutorial") != "done" && PhotonNetworkController.Instance.GameVersionString != "dev")))
		{
			base.transform.parent.position = new Vector3(-140f, 28f, -102f);
			base.transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
			GorillaLocomotion.Player.Instance.InitializeValues();
			PlayerPrefs.SetFloat("redValue", Random.value);
			PlayerPrefs.SetFloat("greenValue", Random.value);
			PlayerPrefs.SetFloat("blueValue", Random.value);
			PlayerPrefs.Save();
			this.UpdateColor(PlayerPrefs.GetFloat("redValue", 0f), PlayerPrefs.GetFloat("greenValue", 0f), PlayerPrefs.GetFloat("blueValue", 0f));
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

	// Token: 0x060009E4 RID: 2532 RVA: 0x0003C535 File Offset: 0x0003A735
	protected void OnDestroy()
	{
		if (GorillaTagger._instance == this)
		{
			GorillaTagger._instance = null;
			GorillaTagger.hasInstance = false;
		}
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x0003C550 File Offset: 0x0003A750
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

	// Token: 0x060009E6 RID: 2534 RVA: 0x0003C5C4 File Offset: 0x0003A7C4
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

	// Token: 0x060009E7 RID: 2535 RVA: 0x0003C70C File Offset: 0x0003A90C
	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		this.isGameOverlayActive = (pCallback.m_bActive > 0);
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0003C720 File Offset: 0x0003A920
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
		if (this.loadedDeviceName == "Oculus")
		{
			if (OVRManager.hasInputFocus && !this.overrideNotInFocus)
			{
				if (!this.leftHandTriggerCollider.activeSelf)
				{
					this.leftHandTriggerCollider.SetActive(true);
					this.rightHandTriggerCollider.SetActive(true);
				}
				GorillaLocomotion.Player.Instance.inOverlay = false;
				if (this.wasInOverlay && CosmeticsController.instance != null)
				{
					CosmeticsController.instance.LeaveSystemMenu();
				}
				this.wasInOverlay = false;
			}
			else
			{
				if (this.leftHandTriggerCollider.activeSelf)
				{
					this.leftHandTriggerCollider.SetActive(false);
					this.rightHandTriggerCollider.SetActive(true);
				}
				GorillaLocomotion.Player.Instance.inOverlay = true;
				this.wasInOverlay = true;
			}
			if (OVRManager.instance)
			{
				GorillaLocomotion.Player.Instance.isUserPresent = OVRManager.instance.isUserPresent;
			}
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
			while (num2 > 90f)
			{
				num--;
				if (num < 0)
				{
					break;
				}
				num2 = OVRManager.display.displayFrequenciesAvailable[num];
			}
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / num2 * 0.98f) > 0.0001f)
			{
				float num3 = Time.fixedDeltaTime - 1f / num2 * 0.98f;
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log("!!!!Time.fixedDeltaTime - (1f / newRefreshRate) * .98f)" + num3.ToString());
				Debug.Log("Old Refresh rate: " + num2.ToString());
				Debug.Log("New Refresh rate: " + num2.ToString());
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" refresh rate         :\t" + num2.ToString());
				Time.fixedDeltaTime = 1f / num2 * 0.98f;
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
		float num4 = this.sphereCastRadius * GorillaLocomotion.Player.Instance.scale;
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.lastLeftHandPositionForTag, num4, this.leftRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.leftRaycastSweep.magnitude, num4), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|96_0(false);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, num4, this.leftHeadRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num4), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|96_0(false);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.lastRightHandPositionForTag, num4, this.rightRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.rightRaycastSweep.magnitude, num4), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|96_0(false);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, num4, this.rightHeadRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num4), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|96_0(false);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, this.headCollider.radius * this.headCollider.transform.localScale.x * GorillaLocomotion.Player.Instance.scale, this.headRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.headRaycastSweep.magnitude, num4), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|96_0(true);
		this.topVector = this.lastBodyPositionForTag + this.bodyVector;
		this.bottomVector = this.lastBodyPositionForTag - this.bodyVector;
		this.nonAllocHits = Physics.CapsuleCastNonAlloc(this.topVector, this.bottomVector, this.bodyCollider.radius * 2f * GorillaLocomotion.Player.Instance.scale, this.bodyRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.bodyRaycastSweep.magnitude, num4), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|96_0(true);
		if (this.otherPlayer != null && GorillaGameManager.instance != null)
		{
			Debug.Log("tagging someone yeet");
			PhotonView.Get(GorillaGameManager.instance.GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", RpcTarget.MasterClient, new object[]
			{
				this.otherPlayer
			});
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(true) && GorillaLocomotion.Player.Instance.IsHandTouching(true) && !this.leftHandTouching && Time.time > this.lastLeftTap + this.tapCoolDown && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			this.StartVibration(true, this.tapHapticStrength, this.tapHapticDuration);
			this.tempInt = ((GorillaLocomotion.Player.Instance.leftHandSurfaceOverride != null) ? GorillaLocomotion.Player.Instance.leftHandSurfaceOverride.overrideIndex : GorillaLocomotion.Player.Instance.leftHandMaterialTouchIndex);
			if (PhotonNetwork.InRoom && this.myVRRig != null)
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
		if (GorillaComputer.instance.voiceChatOn == "TRUE")
		{
			this.myRecorder = PhotonNetworkController.Instance.GetComponent<Recorder>();
			if (GorillaComputer.instance.pttType != "ALL CHAT")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.myRecorder.TransmitEnabled = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.myRecorder.TransmitEnabled = true;
						return;
					}
				}
				else
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.myRecorder.TransmitEnabled = true;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.myRecorder.TransmitEnabled = false;
						return;
					}
				}
			}
			else if (!this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = true;
				return;
			}
		}
		else if (PhotonNetworkController.Instance.GetComponent<Recorder>().TransmitEnabled)
		{
			PhotonNetworkController.Instance.GetComponent<Recorder>().TransmitEnabled = false;
		}
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0003D604 File Offset: 0x0003B804
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

	// Token: 0x060009EA RID: 2538 RVA: 0x0003D749 File Offset: 0x0003B949
	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		base.StartCoroutine(this.HapticPulses(forLeftController, amplitude, duration));
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0003D75B File Offset: 0x0003B95B
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

	// Token: 0x060009EC RID: 2540 RVA: 0x0003D780 File Offset: 0x0003B980
	public void DoVibration(XRNode node, float amplitude, float duration)
	{
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(node);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.SendHapticImpulse(0U, amplitude, duration);
		}
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0003D7A8 File Offset: 0x0003B9A8
	public void UpdateColor(float red, float green, float blue)
	{
		if (GorillaComputer.instance != null)
		{
			this.offlineVRRig.InitializeNoobMaterialLocal(red, green, blue, GorillaComputer.instance.leftHanded);
		}
		else
		{
			this.offlineVRRig.InitializeNoobMaterialLocal(red, green, blue, false);
		}
		if (!PhotonNetwork.InRoom)
		{
			this.offlineVRRig.mainSkin.material = this.offlineVRRig.materialsToChangeTo[0];
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0003D814 File Offset: 0x0003BA14
	protected void OnTriggerEnter(Collider other)
	{
		if (PhotonNetwork.InRoom && other.gameObject.layer == 15 && other.gameObject != null && other.gameObject.GetComponent<GorillaTriggerBox>() != null)
		{
			other.gameObject.GetComponent<GorillaTriggerBox>().OnBoxTriggered();
		}
		if (other.GetComponentInChildren<GorillaTriggerBox>())
		{
			other.GetComponentInChildren<GorillaTriggerBox>().OnBoxTriggered();
			return;
		}
		if (other.GetComponentInParent<GorillaTrigger>())
		{
			other.GetComponentInParent<GorillaTrigger>().OnTriggered();
		}
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x0003D89C File Offset: 0x0003BA9C
	public void ShowCosmeticParticles(bool showParticles)
	{
		if (showParticles)
		{
			this.mainCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("GorillaCosmeticParticle");
			this.mirrorCamera.cullingMask |= 1 << LayerMask.NameToLayer("GorillaCosmeticParticle");
			return;
		}
		this.mainCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("GorillaCosmeticParticle"));
		this.mirrorCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("GorillaCosmeticParticle"));
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x0003D93D File Offset: 0x0003BB3D
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

	// Token: 0x060009F1 RID: 2545 RVA: 0x0003D97D File Offset: 0x0003BB7D
	private void CheckEndStatusEffect()
	{
		if (Time.time > this.statusEndTime)
		{
			this.EndStatusEffect(this.currentStatus);
		}
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0003D998 File Offset: 0x0003BB98
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

	// Token: 0x060009F3 RID: 2547 RVA: 0x0003D9C7 File Offset: 0x0003BBC7
	private float CalcSlideControl(float fps)
	{
		return Mathf.Pow(Mathf.Pow(1f - this.baseSlideControl, 120f), 1f / fps);
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0003D9EB File Offset: 0x0003BBEB
	public static void OnPlayerSpawned(Action action)
	{
		if (GorillaTagger._instance)
		{
			action();
			return;
		}
		GorillaTagger.onPlayerSpawnedRootCallback = (Action)Delegate.Combine(GorillaTagger.onPlayerSpawnedRootCallback, action);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x0003DA15 File Offset: 0x0003BC15
	public void GuidedRefInitialize()
	{
		this.offlineVRRig_guidedRef.fieldId = GuidedRefRelayHub.RegisterReceiverFieldWithParentHub(this, "offlineVRRig", this.offlineVRRig_guidedRef.targetId, this.offlineVRRig_guidedRef.hubId);
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x0003DA44 File Offset: 0x0003BC44
	public bool GuidRefResolveReference(int fieldId, IGuidedRefTarget target)
	{
		Debug.Log("poop target.GuidedRefTargetObject=" + target.GuidedRefTargetObject.name);
		if (this.offlineVRRig_guidedRef.fieldId == fieldId && this.offlineVRRig == null)
		{
			this.offlineVRRig = (target.GuidedRefTargetObject as VRRig);
			return this.offlineVRRig != null;
		}
		return false;
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x0003DB41 File Offset: 0x0003BD41
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0003DB49 File Offset: 0x0003BD49
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0003DB54 File Offset: 0x0003BD54
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHits|96_0(bool isBodyTag)
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

	// Token: 0x04000C1D RID: 3101
	private static GorillaTagger _instance;

	// Token: 0x04000C1E RID: 3102
	public static bool hasInstance;

	// Token: 0x04000C1F RID: 3103
	public bool inCosmeticsRoom;

	// Token: 0x04000C20 RID: 3104
	public SphereCollider headCollider;

	// Token: 0x04000C21 RID: 3105
	public CapsuleCollider bodyCollider;

	// Token: 0x04000C22 RID: 3106
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x04000C23 RID: 3107
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x04000C24 RID: 3108
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04000C25 RID: 3109
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04000C26 RID: 3110
	public Transform rightHandTransform;

	// Token: 0x04000C27 RID: 3111
	public Transform leftHandTransform;

	// Token: 0x04000C28 RID: 3112
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x04000C29 RID: 3113
	public float handTapVolume = 0.1f;

	// Token: 0x04000C2A RID: 3114
	public float tapCoolDown = 0.15f;

	// Token: 0x04000C2B RID: 3115
	public float lastLeftTap;

	// Token: 0x04000C2C RID: 3116
	public float lastRightTap;

	// Token: 0x04000C2D RID: 3117
	public float tapHapticDuration = 0.05f;

	// Token: 0x04000C2E RID: 3118
	public float tapHapticStrength = 0.5f;

	// Token: 0x04000C2F RID: 3119
	public float tagHapticDuration = 0.15f;

	// Token: 0x04000C30 RID: 3120
	public float tagHapticStrength = 1f;

	// Token: 0x04000C31 RID: 3121
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04000C32 RID: 3122
	public float taggedHapticStrength = 1f;

	// Token: 0x04000C33 RID: 3123
	private bool leftHandTouching;

	// Token: 0x04000C34 RID: 3124
	private bool rightHandTouching;

	// Token: 0x04000C35 RID: 3125
	public float taggedTime;

	// Token: 0x04000C36 RID: 3126
	public float tagCooldown;

	// Token: 0x04000C37 RID: 3127
	public float slowCooldown = 3f;

	// Token: 0x04000C38 RID: 3128
	public VRRig offlineVRRig;

	// Token: 0x04000C39 RID: 3129
	public GuidedRefBasicReceiverFieldInfo offlineVRRig_guidedRef;

	// Token: 0x04000C3A RID: 3130
	public GameObject thirdPersonCamera;

	// Token: 0x04000C3B RID: 3131
	public GameObject mainCamera;

	// Token: 0x04000C3C RID: 3132
	public bool testTutorial;

	// Token: 0x04000C3D RID: 3133
	public bool disableTutorial;

	// Token: 0x04000C3E RID: 3134
	public bool frameRateUpdated;

	// Token: 0x04000C3F RID: 3135
	public GameObject leftHandTriggerCollider;

	// Token: 0x04000C40 RID: 3136
	public GameObject rightHandTriggerCollider;

	// Token: 0x04000C41 RID: 3137
	public Camera mirrorCamera;

	// Token: 0x04000C42 RID: 3138
	public AudioSource leftHandSlideSource;

	// Token: 0x04000C43 RID: 3139
	public AudioSource rightHandSlideSource;

	// Token: 0x04000C44 RID: 3140
	public bool overrideNotInFocus;

	// Token: 0x04000C46 RID: 3142
	private Vector3 leftRaycastSweep;

	// Token: 0x04000C47 RID: 3143
	private Vector3 leftHeadRaycastSweep;

	// Token: 0x04000C48 RID: 3144
	private Vector3 rightRaycastSweep;

	// Token: 0x04000C49 RID: 3145
	private Vector3 rightHeadRaycastSweep;

	// Token: 0x04000C4A RID: 3146
	private Vector3 headRaycastSweep;

	// Token: 0x04000C4B RID: 3147
	private Vector3 bodyRaycastSweep;

	// Token: 0x04000C4C RID: 3148
	private InputDevice rightDevice;

	// Token: 0x04000C4D RID: 3149
	private InputDevice leftDevice;

	// Token: 0x04000C4E RID: 3150
	private bool primaryButtonPressRight;

	// Token: 0x04000C4F RID: 3151
	private bool secondaryButtonPressRight;

	// Token: 0x04000C50 RID: 3152
	private bool primaryButtonPressLeft;

	// Token: 0x04000C51 RID: 3153
	private bool secondaryButtonPressLeft;

	// Token: 0x04000C52 RID: 3154
	private RaycastHit hitInfo;

	// Token: 0x04000C53 RID: 3155
	public Photon.Realtime.Player otherPlayer;

	// Token: 0x04000C54 RID: 3156
	private Photon.Realtime.Player tryPlayer;

	// Token: 0x04000C55 RID: 3157
	private Photon.Realtime.Player touchedPlayer;

	// Token: 0x04000C56 RID: 3158
	private Vector3 topVector;

	// Token: 0x04000C57 RID: 3159
	private Vector3 bottomVector;

	// Token: 0x04000C58 RID: 3160
	private Vector3 bodyVector;

	// Token: 0x04000C59 RID: 3161
	private int tempInt;

	// Token: 0x04000C5A RID: 3162
	private InputDevice inputDevice;

	// Token: 0x04000C5B RID: 3163
	private bool wasInOverlay;

	// Token: 0x04000C5C RID: 3164
	private PhotonView tempView;

	// Token: 0x04000C5D RID: 3165
	private Photon.Realtime.Player tempCreator;

	// Token: 0x04000C5E RID: 3166
	public GorillaTagger.StatusEffect currentStatus;

	// Token: 0x04000C5F RID: 3167
	public float statusStartTime;

	// Token: 0x04000C60 RID: 3168
	public float statusEndTime;

	// Token: 0x04000C61 RID: 3169
	private float refreshRate;

	// Token: 0x04000C62 RID: 3170
	private float baseSlideControl;

	// Token: 0x04000C63 RID: 3171
	private int gorillaTagColliderLayerMask;

	// Token: 0x04000C64 RID: 3172
	private RaycastHit[] nonAllocRaycastHits = new RaycastHit[30];

	// Token: 0x04000C65 RID: 3173
	private int nonAllocHits;

	// Token: 0x04000C67 RID: 3175
	private bool xrSubsystemIsActive;

	// Token: 0x04000C68 RID: 3176
	public string loadedDeviceName = "";

	// Token: 0x04000C69 RID: 3177
	private Callback<GameOverlayActivated_t> gameOverlayActivatedCb;

	// Token: 0x04000C6A RID: 3178
	private bool isGameOverlayActive;

	// Token: 0x04000C6B RID: 3179
	private static Action onPlayerSpawnedRootCallback;

	// Token: 0x02000436 RID: 1078
	public enum StatusEffect
	{
		// Token: 0x04001D77 RID: 7543
		None,
		// Token: 0x04001D78 RID: 7544
		Frozen,
		// Token: 0x04001D79 RID: 7545
		Slowed,
		// Token: 0x04001D7A RID: 7546
		Dead,
		// Token: 0x04001D7B RID: 7547
		Infected,
		// Token: 0x04001D7C RID: 7548
		It
	}
}
