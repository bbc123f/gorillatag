using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Steamworks;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public class GorillaTagger : MonoBehaviour
{
	public enum StatusEffect
	{
		None,
		Frozen,
		Slowed,
		Dead,
		Infected,
		It
	}

	private static GorillaTagger _instance;

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

	public GameObject thirdPersonCamera;

	public GameObject mainCamera;

	public bool testTutorial;

	public bool disableTutorial;

	public bool frameRateUpdated;

	public GameObject leftHandTriggerCollider;

	public GameObject rightHandTriggerCollider;

	public Camera mirrorCamera;

	public AudioSource leftHandSlideSource;

	public AudioSource rightHandSlideSource;

	public bool overrideNotInFocus;

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

	private Vector3 topVector;

	private Vector3 bottomVector;

	private Vector3 bodyVector;

	private int tempInt;

	private InputDevice inputDevice;

	private bool wasInOverlay;

	private PhotonView tempView;

	private Photon.Realtime.Player tempCreator;

	public StatusEffect currentStatus;

	public float statusStartTime;

	public float statusEndTime;

	private float refreshRate;

	private float baseSlideControl;

	private int gorillaTagColliderLayerMask;

	private RaycastHit[] nonAllocRaycastHits = new RaycastHit[30];

	private int nonAllocHits;

	private bool xrSubsystemIsActive;

	public string loadedDeviceName = "";

	private Callback<GameOverlayActivated_t> gameOverlayActivatedCb;

	private bool isGameOverlayActive;

	private static Action onPlayerSpawnedRootCallback;

	public static GorillaTagger Instance => _instance;

	public PhotonView myVRRig => offlineVRRig.photonView;

	public Rigidbody rigidbody { get; private set; }

	public Recorder myRecorder { get; private set; }

	public float sphereCastRadius => 0.03f;

	protected void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
			hasInstance = true;
			onPlayerSpawnedRootCallback();
		}
		if (!disableTutorial && (testTutorial || (PlayerPrefs.GetString("tutorial") != "done" && PhotonNetworkController.Instance.GameVersionString != "dev")))
		{
			base.transform.parent.position = new Vector3(-140f, 28f, -102f);
			base.transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
			GorillaLocomotion.Player.Instance.InitializeValues();
			PlayerPrefs.SetFloat("redValue", UnityEngine.Random.value);
			PlayerPrefs.SetFloat("greenValue", UnityEngine.Random.value);
			PlayerPrefs.SetFloat("blueValue", UnityEngine.Random.value);
			PlayerPrefs.Save();
			UpdateColor(PlayerPrefs.GetFloat("redValue", 0f), PlayerPrefs.GetFloat("greenValue", 0f), PlayerPrefs.GetFloat("blueValue", 0f));
		}
		thirdPersonCamera.SetActive(Application.platform != RuntimePlatform.Android);
		inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		wasInOverlay = false;
		baseSlideControl = GorillaLocomotion.Player.Instance.slideControl;
		gorillaTagColliderLayerMask = LayerMask.GetMask("Gorilla Tag Collider");
		rigidbody = GetComponent<Rigidbody>();
	}

	protected void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
			hasInstance = false;
		}
	}

	private void IsXRSubsystemActive()
	{
		loadedDeviceName = XRSettings.loadedDeviceName;
		List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>();
		SubsystemManager.GetInstances(list);
		foreach (XRDisplaySubsystem item in list)
		{
			if (item.running)
			{
				xrSubsystemIsActive = true;
			}
		}
		xrSubsystemIsActive = false;
	}

	protected void Start()
	{
		IsXRSubsystemActive();
		if (loadedDeviceName == "OpenVR")
		{
			GorillaLocomotion.Player.Instance.leftHandOffset = new Vector3(-0.02f, 0f, -0.07f);
			GorillaLocomotion.Player.Instance.rightHandOffset = new Vector3(0.02f, 0f, -0.07f);
			Quaternion rotation = Quaternion.Euler(new Vector3(-90f, 180f, -20f));
			Quaternion rotation2 = Quaternion.Euler(new Vector3(-90f, 180f, 20f));
			Quaternion quaternion = Quaternion.Euler(new Vector3(-141f, 204f, -27f));
			Quaternion quaternion2 = Quaternion.Euler(new Vector3(-141f, 156f, 27f));
			GorillaLocomotion.Player.Instance.leftHandRotOffset = quaternion * Quaternion.Inverse(rotation);
			GorillaLocomotion.Player.Instance.rightHandRotOffset = quaternion2 * Quaternion.Inverse(rotation2);
		}
		bodyVector = new Vector3(0f, bodyCollider.height / 2f - bodyCollider.radius, 0f);
		if (SteamManager.Initialized)
		{
			gameOverlayActivatedCb = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		isGameOverlayActive = pCallback.m_bActive != 0;
	}

	protected void LateUpdate()
	{
		if (OpenVR.Overlay != null && OpenVR.Overlay.IsDashboardVisible())
		{
			if (leftHandTriggerCollider.activeSelf)
			{
				leftHandTriggerCollider.SetActive(value: false);
				rightHandTriggerCollider.SetActive(value: true);
			}
			GorillaLocomotion.Player.Instance.inOverlay = true;
		}
		else
		{
			if (!leftHandTriggerCollider.activeSelf)
			{
				leftHandTriggerCollider.SetActive(value: true);
				rightHandTriggerCollider.SetActive(value: true);
			}
			GorillaLocomotion.Player.Instance.inOverlay = false;
		}
		if (loadedDeviceName == "Oculus")
		{
			if (OVRManager.hasInputFocus && !overrideNotInFocus)
			{
				if (!leftHandTriggerCollider.activeSelf)
				{
					leftHandTriggerCollider.SetActive(value: true);
					rightHandTriggerCollider.SetActive(value: true);
				}
				GorillaLocomotion.Player.Instance.inOverlay = false;
				if (wasInOverlay && CosmeticsController.instance != null)
				{
					CosmeticsController.instance.LeaveSystemMenu();
				}
				wasInOverlay = false;
			}
			else
			{
				if (leftHandTriggerCollider.activeSelf)
				{
					leftHandTriggerCollider.SetActive(value: false);
					rightHandTriggerCollider.SetActive(value: true);
				}
				GorillaLocomotion.Player.Instance.inOverlay = true;
				wasInOverlay = true;
			}
			if ((bool)OVRManager.instance)
			{
				GorillaLocomotion.Player.Instance.isUserPresent = OVRManager.instance.isUserPresent;
			}
		}
		if (xrSubsystemIsActive && Application.platform != RuntimePlatform.Android)
		{
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / XRDevice.refreshRate) > 0.0001f)
			{
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime);
				Debug.Log(" refresh rate         :\t" + XRDevice.refreshRate);
				Time.fixedDeltaTime = 1f / XRDevice.refreshRate;
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime);
				Debug.Log(" history size before  :\t" + GorillaLocomotion.Player.Instance.velocityHistorySize);
				GorillaLocomotion.Player.Instance.velocityHistorySize = Mathf.Max(Mathf.Min(Mathf.FloorToInt(XRDevice.refreshRate * (1f / 12f)), 10), 6);
				if (GorillaLocomotion.Player.Instance.velocityHistorySize > 9)
				{
					GorillaLocomotion.Player.Instance.velocityHistorySize--;
				}
				Debug.Log("new history size: " + GorillaLocomotion.Player.Instance.velocityHistorySize);
				Debug.Log(" ============================================");
				GorillaLocomotion.Player.Instance.slideControl = 1f - CalcSlideControl(XRDevice.refreshRate);
				GorillaLocomotion.Player.Instance.InitializeValues();
			}
		}
		else if (Application.platform != RuntimePlatform.Android && OVRManager.instance != null && OVRManager.OVRManagerinitialized && OVRManager.instance.gameObject != null && OVRManager.instance.gameObject.activeSelf)
		{
			UnityEngine.Object.Destroy(OVRManager.instance.gameObject);
		}
		if (!frameRateUpdated && Application.platform == RuntimePlatform.Android && OVRManager.instance.gameObject.activeSelf)
		{
			int num = OVRManager.display.displayFrequenciesAvailable.Length - 1;
			float num2 = OVRManager.display.displayFrequenciesAvailable[num];
			_ = OVRPlugin.systemDisplayFrequency;
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
				Debug.Log("!!!!Time.fixedDeltaTime - (1f / newRefreshRate) * .98f)" + num3);
				Debug.Log("Old Refresh rate: " + num2);
				Debug.Log("New Refresh rate: " + num2);
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime);
				Debug.Log(" refresh rate         :\t" + num2);
				Time.fixedDeltaTime = 1f / num2 * 0.98f;
				OVRPlugin.systemDisplayFrequency = num2;
				GorillaLocomotion.Player.Instance.velocityHistorySize = Mathf.FloorToInt(num2 * (1f / 12f));
				if (GorillaLocomotion.Player.Instance.velocityHistorySize > 9)
				{
					GorillaLocomotion.Player.Instance.velocityHistorySize--;
				}
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime);
				Debug.Log(" history size before  :\t" + GorillaLocomotion.Player.Instance.velocityHistorySize);
				Debug.Log("new history size: " + GorillaLocomotion.Player.Instance.velocityHistorySize);
				Debug.Log(" ============================================");
				GorillaLocomotion.Player.Instance.slideControl = 1f - CalcSlideControl(XRDevice.refreshRate);
				GorillaLocomotion.Player.Instance.InitializeValues();
				OVRManager.instance.gameObject.SetActive(value: false);
				frameRateUpdated = true;
			}
		}
		if (!xrSubsystemIsActive && Application.platform != RuntimePlatform.Android && Mathf.Abs(Time.fixedDeltaTime - 1f / 144f) > 0.0001f)
		{
			Debug.Log("updating delta time. was: " + Time.fixedDeltaTime + ". now it's " + 1f / 144f);
			Application.targetFrameRate = 144;
			Time.fixedDeltaTime = 1f / 144f;
			GorillaLocomotion.Player.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt(12f), 10);
			if (GorillaLocomotion.Player.Instance.velocityHistorySize > 9)
			{
				GorillaLocomotion.Player.Instance.velocityHistorySize--;
			}
			Debug.Log("new history size: " + GorillaLocomotion.Player.Instance.velocityHistorySize);
			GorillaLocomotion.Player.Instance.slideControl = 1f - CalcSlideControl(144f);
			GorillaLocomotion.Player.Instance.InitializeValues();
		}
		leftRaycastSweep = leftHandTransform.position - lastLeftHandPositionForTag;
		leftHeadRaycastSweep = leftHandTransform.position - headCollider.transform.position;
		rightRaycastSweep = rightHandTransform.position - lastRightHandPositionForTag;
		rightHeadRaycastSweep = rightHandTransform.position - headCollider.transform.position;
		headRaycastSweep = headCollider.transform.position - lastHeadPositionForTag;
		bodyRaycastSweep = bodyCollider.transform.position - lastBodyPositionForTag;
		otherPlayer = null;
		float num4 = sphereCastRadius * GorillaLocomotion.Player.Instance.scale;
		nonAllocHits = Physics.SphereCastNonAlloc(lastLeftHandPositionForTag, num4, leftRaycastSweep.normalized, nonAllocRaycastHits, Mathf.Max(leftRaycastSweep.magnitude, num4), gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		TryTaggingAllHits(isBodyTag: false);
		nonAllocHits = Physics.SphereCastNonAlloc(headCollider.transform.position, num4, leftHeadRaycastSweep.normalized, nonAllocRaycastHits, Mathf.Max(leftHeadRaycastSweep.magnitude, num4), gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		TryTaggingAllHits(isBodyTag: false);
		nonAllocHits = Physics.SphereCastNonAlloc(lastRightHandPositionForTag, num4, rightRaycastSweep.normalized, nonAllocRaycastHits, Mathf.Max(rightRaycastSweep.magnitude, num4), gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		TryTaggingAllHits(isBodyTag: false);
		nonAllocHits = Physics.SphereCastNonAlloc(headCollider.transform.position, num4, rightHeadRaycastSweep.normalized, nonAllocRaycastHits, Mathf.Max(rightHeadRaycastSweep.magnitude, num4), gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		TryTaggingAllHits(isBodyTag: false);
		nonAllocHits = Physics.SphereCastNonAlloc(headCollider.transform.position, headCollider.radius * headCollider.transform.localScale.x * GorillaLocomotion.Player.Instance.scale, headRaycastSweep.normalized, nonAllocRaycastHits, Mathf.Max(headRaycastSweep.magnitude, num4), gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		TryTaggingAllHits(isBodyTag: true);
		topVector = lastBodyPositionForTag + bodyVector;
		bottomVector = lastBodyPositionForTag - bodyVector;
		nonAllocHits = Physics.CapsuleCastNonAlloc(topVector, bottomVector, bodyCollider.radius * 2f * GorillaLocomotion.Player.Instance.scale, bodyRaycastSweep.normalized, nonAllocRaycastHits, Mathf.Max(bodyRaycastSweep.magnitude, num4), gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		TryTaggingAllHits(isBodyTag: true);
		if (otherPlayer != null && GorillaGameManager.instance != null)
		{
			Debug.Log("tagging someone yeet");
			PhotonView.Get(GorillaGameManager.instance.GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", RpcTarget.MasterClient, otherPlayer);
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(forLeftHand: true) && GorillaLocomotion.Player.Instance.IsHandTouching(forLeftHand: true) && !leftHandTouching && Time.time > lastLeftTap + tapCoolDown && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			StartVibration(forLeftController: true, tapHapticStrength, tapHapticDuration);
			tempInt = ((GorillaLocomotion.Player.Instance.leftHandSurfaceOverride != null) ? GorillaLocomotion.Player.Instance.leftHandSurfaceOverride.overrideIndex : GorillaLocomotion.Player.Instance.leftHandMaterialTouchIndex);
			if (PhotonNetwork.InRoom && myVRRig != null)
			{
				PhotonView.Get(myVRRig).RPC("PlayHandTap", RpcTarget.Others, tempInt, true, handTapVolume);
			}
			offlineVRRig.PlayHandTapLocal(tempInt, isLeftHand: true, handTapVolume);
			lastLeftTap = Time.time;
			if (GorillaLocomotion.Player.Instance.leftHandSurfaceOverride != null && GorillaLocomotion.Player.Instance.leftHandSurfaceOverride.sendOnTapEvent)
			{
				Tappable component = GorillaLocomotion.Player.Instance.leftHandSurfaceOverride.gameObject.GetComponent<Tappable>();
				if (component != null)
				{
					component.OnTap(handTapVolume, lastLeftTap);
				}
			}
		}
		else if (GorillaLocomotion.Player.Instance.IsHandSliding(forLeftHand: true) && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			StartVibration(forLeftController: true, tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!leftHandSlideSource.isPlaying)
			{
				leftHandSlideSource.Play();
			}
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(forLeftHand: true))
		{
			leftHandSlideSource.Stop();
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(forLeftHand: false) && GorillaLocomotion.Player.Instance.IsHandTouching(forLeftHand: false) && !rightHandTouching && Time.time > lastRightTap + tapCoolDown && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			StartVibration(forLeftController: false, tapHapticStrength, tapHapticDuration);
			tempInt = ((GorillaLocomotion.Player.Instance.rightHandSurfaceOverride != null) ? GorillaLocomotion.Player.Instance.rightHandSurfaceOverride.overrideIndex : GorillaLocomotion.Player.Instance.rightHandMaterialTouchIndex);
			if (PhotonNetwork.InRoom && myVRRig != null)
			{
				PhotonView.Get(myVRRig).RPC("PlayHandTap", RpcTarget.Others, tempInt, false, handTapVolume);
			}
			offlineVRRig.PlayHandTapLocal(tempInt, isLeftHand: false, handTapVolume);
			lastRightTap = Time.time;
			if (GorillaLocomotion.Player.Instance.rightHandSurfaceOverride != null && GorillaLocomotion.Player.Instance.rightHandSurfaceOverride.sendOnTapEvent)
			{
				Tappable component2 = GorillaLocomotion.Player.Instance.rightHandSurfaceOverride.gameObject.GetComponent<Tappable>();
				if (component2 != null)
				{
					component2.OnTap(handTapVolume, lastRightTap);
				}
			}
		}
		else if (GorillaLocomotion.Player.Instance.IsHandSliding(forLeftHand: false) && !GorillaLocomotion.Player.Instance.inOverlay)
		{
			StartVibration(forLeftController: false, tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!rightHandSlideSource.isPlaying)
			{
				rightHandSlideSource.Play();
			}
		}
		if (!GorillaLocomotion.Player.Instance.IsHandSliding(forLeftHand: false))
		{
			rightHandSlideSource.Stop();
		}
		CheckEndStatusEffect();
		leftHandTouching = GorillaLocomotion.Player.Instance.IsHandTouching(forLeftHand: true);
		rightHandTouching = GorillaLocomotion.Player.Instance.IsHandTouching(forLeftHand: false);
		lastLeftHandPositionForTag = leftHandTransform.position;
		lastRightHandPositionForTag = rightHandTransform.position;
		lastBodyPositionForTag = bodyCollider.transform.position;
		lastHeadPositionForTag = headCollider.transform.position;
		if (GorillaComputer.instance.voiceChatOn == "TRUE")
		{
			myRecorder = PhotonNetworkController.Instance.GetComponent<Recorder>();
			if (GorillaComputer.instance.pttType != "ALL CHAT")
			{
				primaryButtonPressRight = false;
				secondaryButtonPressRight = false;
				primaryButtonPressLeft = false;
				secondaryButtonPressLeft = false;
				primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				secondaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				if (primaryButtonPressRight || secondaryButtonPressRight || primaryButtonPressLeft || secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						myRecorder.TransmitEnabled = false;
					}
					else if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						myRecorder.TransmitEnabled = true;
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
				{
					myRecorder.TransmitEnabled = true;
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO TALK")
				{
					myRecorder.TransmitEnabled = false;
				}
			}
			else if (!myRecorder.TransmitEnabled)
			{
				myRecorder.TransmitEnabled = true;
			}
		}
		else if (PhotonNetworkController.Instance.GetComponent<Recorder>().TransmitEnabled)
		{
			PhotonNetworkController.Instance.GetComponent<Recorder>().TransmitEnabled = false;
		}
		void TryTaggingAllHits(bool isBodyTag)
		{
			for (int i = 0; i < nonAllocHits; i++)
			{
				if (nonAllocRaycastHits[i].collider.gameObject.activeSelf && TryToTag(nonAllocRaycastHits[i], isBodyTag, out tryPlayer))
				{
					otherPlayer = tryPlayer;
					break;
				}
			}
		}
	}

	private bool TryToTag(RaycastHit hitInfo, bool isBodyTag, out Photon.Realtime.Player taggedPlayer)
	{
		if (PhotonNetwork.InRoom)
		{
			tempCreator = hitInfo.collider.GetComponentInParent<VRRig>()?.creator;
			if (tempCreator != null && PhotonNetwork.LocalPlayer != tempCreator && GorillaGameManager.instance != null && GorillaGameManager.instance.LocalCanTag(PhotonNetwork.LocalPlayer, tempCreator) && Time.time > taggedTime + tagCooldown)
			{
				if (!isBodyTag)
				{
					StartVibration(((leftHandTransform.position - hitInfo.collider.transform.position).magnitude < (rightHandTransform.position - hitInfo.collider.transform.position).magnitude) ? true : false, tagHapticStrength, tagHapticDuration);
				}
				else
				{
					StartVibration(forLeftController: true, tagHapticStrength, tagHapticDuration);
					StartVibration(forLeftController: false, tagHapticStrength, tagHapticDuration);
				}
				taggedPlayer = tempCreator;
				return true;
			}
		}
		taggedPlayer = null;
		return false;
	}

	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		StartCoroutine(HapticPulses(forLeftController, amplitude, duration));
	}

	private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
	{
		float startTime = Time.time;
		uint channel = 0u;
		InputDevice device = ((!forLeftController) ? InputDevices.GetDeviceAtXRNode(XRNode.RightHand) : InputDevices.GetDeviceAtXRNode(XRNode.LeftHand));
		while (Time.time < startTime + duration)
		{
			device.SendHapticImpulse(channel, amplitude, hapticWaitSeconds);
			yield return new WaitForSeconds(hapticWaitSeconds * 0.9f);
		}
	}

	public void DoVibration(XRNode node, float amplitude, float duration)
	{
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(node);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.SendHapticImpulse(0u, amplitude, duration);
		}
	}

	public void UpdateColor(float red, float green, float blue)
	{
		if (GorillaComputer.instance != null)
		{
			offlineVRRig.InitializeNoobMaterialLocal(red, green, blue, GorillaComputer.instance.leftHanded);
		}
		else
		{
			offlineVRRig.InitializeNoobMaterialLocal(red, green, blue, leftHanded: false);
		}
		if (!PhotonNetwork.InRoom)
		{
			offlineVRRig.mainSkin.material = offlineVRRig.materialsToChangeTo[0];
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (PhotonNetwork.InRoom && other.gameObject.layer == 15 && other.gameObject != null && other.gameObject.GetComponent<GorillaTriggerBox>() != null)
		{
			other.gameObject.GetComponent<GorillaTriggerBox>().OnBoxTriggered();
		}
		if ((bool)other.GetComponentInChildren<GorillaTriggerBox>())
		{
			other.GetComponentInChildren<GorillaTriggerBox>().OnBoxTriggered();
		}
		else if ((bool)other.GetComponentInParent<GorillaTrigger>())
		{
			other.GetComponentInParent<GorillaTrigger>().OnTriggered();
		}
	}

	public void ShowCosmeticParticles(bool showParticles)
	{
		if (showParticles)
		{
			mainCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("GorillaCosmeticParticle");
			mirrorCamera.cullingMask |= 1 << LayerMask.NameToLayer("GorillaCosmeticParticle");
		}
		else
		{
			mainCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("GorillaCosmeticParticle"));
			mirrorCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("GorillaCosmeticParticle"));
		}
	}

	public void ApplyStatusEffect(StatusEffect newStatus, float duration)
	{
		EndStatusEffect(currentStatus);
		currentStatus = newStatus;
		statusEndTime = Time.time + duration;
		switch (newStatus)
		{
		case StatusEffect.Frozen:
			GorillaLocomotion.Player.Instance.disableMovement = true;
			break;
		case StatusEffect.None:
		case StatusEffect.Slowed:
			break;
		}
	}

	private void CheckEndStatusEffect()
	{
		if (Time.time > statusEndTime)
		{
			EndStatusEffect(currentStatus);
		}
	}

	private void EndStatusEffect(StatusEffect effectToEnd)
	{
		switch (effectToEnd)
		{
		case StatusEffect.Frozen:
			GorillaLocomotion.Player.Instance.disableMovement = false;
			currentStatus = StatusEffect.None;
			break;
		case StatusEffect.Slowed:
			currentStatus = StatusEffect.None;
			break;
		case StatusEffect.None:
			break;
		}
	}

	private float CalcSlideControl(float fps)
	{
		return Mathf.Pow(Mathf.Pow(1f - baseSlideControl, 120f), 1f / fps);
	}

	public static void OnPlayerSpawned(Action action)
	{
		if ((bool)_instance)
		{
			action();
		}
		else
		{
			onPlayerSpawnedRootCallback = (Action)Delegate.Combine(onPlayerSpawnedRootCallback, action);
		}
	}
}
