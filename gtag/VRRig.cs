using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class VRRig : MonoBehaviour, IGorillaSerializeable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback, IPreDisable
{
	public enum WearablePackedStateSlots
	{
		Hat,
		LeftHand,
		RightHand
	}

	public struct VelocityTime
	{
		public Vector3 vel;

		public double time;

		public VelocityTime(Vector3 velocity, double velTime)
		{
			vel = velocity;
			time = velTime;
		}
	}

	public VRMap head;

	public VRMap rightHand;

	public VRMap leftHand;

	public VRMapThumb leftThumb;

	public VRMapIndex leftIndex;

	public VRMapMiddle leftMiddle;

	public VRMapThumb rightThumb;

	public VRMapIndex rightIndex;

	public VRMapMiddle rightMiddle;

	private int previousGrabbedRope = -1;

	private int previousGrabbedRopeBoneIndex;

	private bool previousGrabbedRopeWasLeft;

	private GorillaRopeSwing currentRopeSwing;

	private Transform currentRopeSwingTarget;

	private float lastRopeGrabTimer;

	private bool shouldLerpToRope;

	[NonSerialized]
	public int grabbedRopeIndex = -1;

	[NonSerialized]
	public int grabbedRopeBoneIndex;

	[NonSerialized]
	public bool grabbedRopeIsLeft;

	[NonSerialized]
	public Vector3 grabbedRopeOffset = Vector3.zero;

	[Tooltip("- False in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool isOfflineVRRig;

	public GameObject mainCamera;

	public Transform playerOffsetTransform;

	public int SDKIndex;

	public bool isMyPlayer;

	public AudioSource leftHandPlayer;

	public AudioSource rightHandPlayer;

	public AudioSource tagSound;

	[SerializeField]
	private float ratio;

	public Transform headConstraint;

	public Vector3 headBodyOffset = Vector3.zero;

	public GameObject headMesh;

	public Vector3 syncPos;

	public Quaternion syncRotation;

	public AudioClip[] clipToPlay;

	public AudioClip[] handTapSound;

	public int currentMatIndex;

	public int setMatIndex;

	private int tempMatIndex;

	public float lerpValueFingers;

	public float lerpValueBody;

	public GameObject backpack;

	public Transform leftHandTransform;

	public Transform rightHandTransform;

	public SkinnedMeshRenderer mainSkin;

	public Photon.Realtime.Player myPlayer;

	public GameObject spectatorSkin;

	public int handSync;

	public Material[] materialsToChangeTo;

	public float red;

	public float green;

	public float blue;

	public string playerName;

	public Text playerText;

	[Tooltip("- True in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool showName;

	public CosmeticItemRegistry cosmeticsObjectRegistry = new CosmeticItemRegistry();

	public GameObject[] cosmetics;

	public GameObject[] overrideCosmetics;

	public string concatStringOfCosmeticsAllowed = "";

	public bool initializedCosmetics;

	public CosmeticsController.CosmeticSet cosmeticSet;

	public CosmeticsController.CosmeticSet tryOnSet;

	public CosmeticsController.CosmeticSet mergedSet;

	public CosmeticsController.CosmeticSet prevSet;

	public SizeManager sizeManager;

	public float pitchScale = 0.3f;

	public float pitchOffset = 1f;

	public VRRigReliableState reliableState;

	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[3]
	{
		new GTBitOps.BitWriteInfo(0, 1),
		new GTBitOps.BitWriteInfo(1, 2),
		new GTBitOps.BitWriteInfo(3, 2)
	};

	public bool inTryOnRoom;

	public bool muted;

	public float scaleFactor;

	private float timeSpawned;

	public float doNotLerpConstant = 1f;

	public string tempString;

	private Photon.Realtime.Player tempPlayer;

	internal Photon.Realtime.Player creator;

	private VRRig tempRig;

	private float[] speedArray;

	private double handLerpValues;

	private bool initialized;

	public BattleBalloons battleBalloons;

	private int tempInt;

	public BodyDockPositions myBodyDockPositions;

	public ParticleSystem lavaParticleSystem;

	public ParticleSystem rockParticleSystem;

	public ParticleSystem iceParticleSystem;

	public string tempItemName;

	public CosmeticsController.CosmeticItem tempItem;

	public string tempItemId;

	public int tempItemCost;

	public int leftHandHoldableStatus;

	public int rightHandHoldableStatus;

	[Tooltip("This has to match the drumsAS array in DrumsItem.cs.")]
	[SerializeReference]
	public AudioSource[] musicDrums;

	public TransferrableObject[] instrumentSelfOnly;

	public AudioSource geodeCrackingSound;

	public float bonkTime;

	public float bonkCooldown = 2f;

	public bool isQuitting;

	private VRRig tempVRRig;

	public GameObject huntComputer;

	public Slingshot slingshot;

	public bool playerLeftHanded;

	public Slingshot.SlingshotState slingshotState;

	private PhotonVoiceView myPhotonVoiceView;

	private VRRig senderRig;

	public TransferrableObject.PositionState currentState;

	private bool isInitialized;

	private List<VelocityTime> velocityHistoryList = new List<VelocityTime>();

	public int velocityHistoryMaxLength = 200;

	private Vector3 lastPosition;

	public const int splashLimitCount = 4;

	public const float splashLimitCooldown = 0.5f;

	private float[] splashEffectTimes = new float[4];

	internal AudioSource voiceAudio;

	[SerializeField]
	internal PhotonView photonView;

	[SerializeField]
	private FXSystemSettings sharedFXSettings;

	[NonSerialized]
	public FXSystemSettings fxSettings;

	public static Action newPlayerJoined;

	public Color playerColor;

	public bool colorInitialized;

	private Action<Color> onColorInitialized;

	public int WearablePackedStates
	{
		get
		{
			return reliableState.wearablesPackedStates;
		}
		set
		{
			if (reliableState.wearablesPackedStates != value)
			{
				reliableState.wearablesPackedStates = value;
				reliableState.SetIsDirty();
			}
		}
	}

	public int LeftThrowableProjectileIndex
	{
		get
		{
			return reliableState.lThrowableProjectileIndex;
		}
		set
		{
			if (reliableState.lThrowableProjectileIndex != value)
			{
				reliableState.lThrowableProjectileIndex = value;
				reliableState.SetIsDirty();
			}
		}
	}

	public int RightThrowableProjectileIndex
	{
		get
		{
			return reliableState.rThrowableProjectileIndex;
		}
		set
		{
			if (reliableState.rThrowableProjectileIndex != value)
			{
				reliableState.rThrowableProjectileIndex = value;
				reliableState.SetIsDirty();
			}
		}
	}

	public Color LeftThrowableProjectileColor
	{
		get
		{
			return reliableState.lThrowableProjectileColor;
		}
		set
		{
			if (!reliableState.lThrowableProjectileColor.CompareAs255Unclamped(value))
			{
				reliableState.lThrowableProjectileColor = value;
				reliableState.SetIsDirty();
			}
		}
	}

	public Color RightThrowableProjectileColor
	{
		get
		{
			return reliableState.rThrowableProjectileColor;
		}
		set
		{
			if (!reliableState.rThrowableProjectileColor.CompareAs255Unclamped(value))
			{
				reliableState.rThrowableProjectileColor = value;
				reliableState.SetIsDirty();
			}
		}
	}

	public int SizeLayerMask
	{
		get
		{
			return reliableState.sizeLayerMask;
		}
		set
		{
			if (reliableState.sizeLayerMask != value)
			{
				reliableState.sizeLayerMask = value;
				reliableState.SetIsDirty();
			}
		}
	}

	public int ActiveTransferrableObjectIndex(int idx)
	{
		return reliableState.activeTransferrableObjectIndex[idx];
	}

	public int ActiveTransferrableObjectIndexLength()
	{
		return reliableState.activeTransferrableObjectIndex.Length;
	}

	public void SetActiveTransferrableObjectIndex(int idx, int v)
	{
		if (reliableState.activeTransferrableObjectIndex[idx] != v)
		{
			reliableState.activeTransferrableObjectIndex[idx] = v;
			reliableState.SetIsDirty();
		}
	}

	public TransferrableObject.PositionState TransferrablePosStates(int idx)
	{
		return reliableState.transferrablePosStates[idx];
	}

	public void SetTransferrablePosStates(int idx, TransferrableObject.PositionState v)
	{
		if (reliableState.transferrablePosStates[idx] != v)
		{
			reliableState.transferrablePosStates[idx] = v;
			reliableState.SetIsDirty();
		}
	}

	public TransferrableObject.ItemStates TransferrableItemStates(int idx)
	{
		return reliableState.transferrableItemStates[idx];
	}

	public void SetTransferrableItemStates(int idx, TransferrableObject.ItemStates v)
	{
		if (reliableState.transferrableItemStates[idx] != v)
		{
			reliableState.transferrableItemStates[idx] = v;
			reliableState.SetIsDirty();
		}
	}

	public void SetTransferrableDockPosition(int idx, BodyDockPositions.DropPositions v)
	{
		if (reliableState.transferableDockPositions[idx] != v)
		{
			reliableState.transferableDockPositions[idx] = v;
			reliableState.SetIsDirty();
		}
	}

	public BodyDockPositions.DropPositions TransferrableDockPosition(int idx)
	{
		return reliableState.transferableDockPositions[idx];
	}

	public Color GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return RightThrowableProjectileColor;
		}
		return LeftThrowableProjectileColor;
	}

	public void SetThrowableProjectileColor(bool isLeftHand, Color color)
	{
		if (isLeftHand)
		{
			LeftThrowableProjectileColor = color;
		}
		else
		{
			RightThrowableProjectileColor = color;
		}
	}

	private void Awake()
	{
		fxSettings = UnityEngine.Object.Instantiate(sharedFXSettings);
		fxSettings.forLocalRig = isOfflineVRRig;
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		GameObject[] array = cosmetics;
		GameObject value;
		foreach (GameObject gameObject in array)
		{
			if (!dictionary.TryGetValue(gameObject.name, out value))
			{
				dictionary.Add(gameObject.name, gameObject);
			}
		}
		array = overrideCosmetics;
		foreach (GameObject gameObject2 in array)
		{
			if (dictionary.TryGetValue(gameObject2.name, out value) && value.name == gameObject2.name)
			{
				value.name = "OVERRIDDEN";
			}
		}
		cosmetics = cosmetics.Concat(overrideCosmetics).ToArray();
		cosmeticsObjectRegistry.Initialize(cosmetics);
		lastPosition = base.transform.position;
		SharedStart();
	}

	private void Start()
	{
	}

	private void SharedStart()
	{
		if (isInitialized)
		{
			return;
		}
		isInitialized = true;
		myBodyDockPositions = GetComponent<BodyDockPositions>();
		reliableState.SharedStart(isOfflineVRRig, myBodyDockPositions);
		Application.quitting += Quitting;
		concatStringOfCosmeticsAllowed = "";
		playerText.transform.parent.GetComponent<Canvas>().worldCamera = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		materialsToChangeTo[0] = UnityEngine.Object.Instantiate(materialsToChangeTo[0]);
		initialized = false;
		currentState = TransferrableObject.PositionState.OnChest;
		if (setMatIndex > -1 && setMatIndex < materialsToChangeTo.Length)
		{
			mainSkin.material = materialsToChangeTo[setMatIndex];
		}
		if (isOfflineVRRig)
		{
			CosmeticsController.instance.currentWornSet.LoadFromPlayerPreferences(CosmeticsController.instance);
			if (Application.platform == RuntimePlatform.Android && spectatorSkin != null)
			{
				UnityEngine.Object.Destroy(spectatorSkin);
			}
		}
		else if (!isOfflineVRRig)
		{
			if (spectatorSkin != null)
			{
				UnityEngine.Object.Destroy(spectatorSkin);
			}
			head.syncPos = -headBodyOffset;
		}
		if (base.transform.parent == null)
		{
			base.transform.parent = GorillaParent.instance.transform;
		}
		StartCoroutine(OccasionalUpdate());
	}

	private IEnumerator OccasionalUpdate()
	{
		while (true)
		{
			try
			{
				if (!isOfflineVRRig)
				{
					if (PhotonNetwork.IsMasterClient && photonView != null && photonView.IsRoomView && photonView.IsMine)
					{
						Debug.Log("network deleting vrrig");
						PhotonNetwork.Destroy(photonView.gameObject);
					}
					if (photonView.IsRoomView)
					{
						Debug.Log("local disabling vrrig");
						photonView.gameObject.SetActive(value: false);
					}
					if (PhotonNetwork.IsMasterClient && GorillaGameManager.instance == null)
					{
						PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out var value);
						if (value.ToString().Contains("CASUAL") || value.ToString().Contains("INFECTION"))
						{
							PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Tag Manager", base.transform.position, base.transform.rotation, 0);
						}
						else if (value.ToString().Contains("HUNT"))
						{
							PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Hunt Manager", base.transform.position, base.transform.rotation, 0);
						}
						else if (value.ToString().Contains("BATTLE"))
						{
							PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Battle Manager", base.transform.position, base.transform.rotation, 0);
						}
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public bool IsItemAllowed(string itemName)
	{
		if (itemName == "Slingshot" && PhotonNetwork.InRoom && GorillaGameManager.instance is GorillaBattleManager)
		{
			return true;
		}
		if (concatStringOfCosmeticsAllowed == null)
		{
			return false;
		}
		if (concatStringOfCosmeticsAllowed.Contains(itemName))
		{
			return true;
		}
		bool canTryOn = CosmeticsController.instance.GetItemFromDict(itemName).canTryOn;
		if (inTryOnRoom && canTryOn)
		{
			return true;
		}
		return false;
	}

	private void LateUpdate()
	{
		base.transform.localScale = Vector3.one * scaleFactor;
		if (isOfflineVRRig)
		{
			if (GorillaGameManager.instance != null)
			{
				speedArray = GorillaGameManager.instance.LocalPlayerSpeed();
				GorillaLocomotion.Player.Instance.jumpMultiplier = speedArray[1];
				GorillaLocomotion.Player.Instance.maxJumpSpeed = speedArray[0];
			}
			else
			{
				GorillaLocomotion.Player.Instance.jumpMultiplier = 1.1f;
				GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
			}
			scaleFactor = GorillaLocomotion.Player.Instance.scale;
			base.transform.localScale = Vector3.one * scaleFactor;
			base.transform.eulerAngles = new Vector3(0f, mainCamera.transform.rotation.eulerAngles.y, 0f);
			base.transform.position = mainCamera.transform.position + headConstraint.rotation * head.trackingPositionOffset * scaleFactor + base.transform.rotation * headBodyOffset * scaleFactor;
			head.MapMine(scaleFactor, playerOffsetTransform);
			rightHand.MapMine(scaleFactor, playerOffsetTransform);
			leftHand.MapMine(scaleFactor, playerOffsetTransform);
			rightIndex.MapMyFinger(lerpValueFingers);
			rightMiddle.MapMyFinger(lerpValueFingers);
			rightThumb.MapMyFinger(lerpValueFingers);
			leftIndex.MapMyFinger(lerpValueFingers);
			leftMiddle.MapMyFinger(lerpValueFingers);
			leftThumb.MapMyFinger(lerpValueFingers);
			if (GorillaTagger.Instance.loadedDeviceName == "Oculus")
			{
				mainSkin.enabled = (OVRManager.hasInputFocus ? true : false);
			}
			mainSkin.enabled = !GorillaLocomotion.Player.Instance.inOverlay;
		}
		else
		{
			if (voiceAudio != null)
			{
				float num = (GorillaTagger.Instance.offlineVRRig.transform.localScale.x - base.transform.localScale.x) / pitchScale + pitchOffset;
				if (!Mathf.Approximately(voiceAudio.pitch, num))
				{
					voiceAudio.pitch = num;
				}
			}
			if (Time.time > timeSpawned + doNotLerpConstant)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, syncPos, lerpValueBody * 0.66f);
				if ((bool)currentRopeSwing && (bool)currentRopeSwingTarget)
				{
					Vector3 vector = ((!grabbedRopeIsLeft) ? (currentRopeSwingTarget.position - rightHandTransform.position) : (currentRopeSwingTarget.position - leftHandTransform.position));
					if (shouldLerpToRope)
					{
						base.transform.position += Vector3.Lerp(Vector3.zero, vector, lastRopeGrabTimer * 4f);
						if (lastRopeGrabTimer < 1f)
						{
							lastRopeGrabTimer += Time.deltaTime;
						}
					}
					else
					{
						base.transform.position += vector;
					}
				}
			}
			else
			{
				base.transform.position = syncPos;
			}
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, syncRotation, lerpValueBody);
			base.transform.position = SanitizeVector3(base.transform.position);
			base.transform.rotation = SanitizeQuaternion(base.transform.rotation);
			head.syncPos = base.transform.rotation * -headBodyOffset * scaleFactor;
			head.MapOther(lerpValueBody);
			rightHand.MapOther(lerpValueBody);
			leftHand.MapOther(lerpValueBody);
			rightIndex.MapOtherFinger((float)(handSync % 10) / 10f, lerpValueFingers);
			rightMiddle.MapOtherFinger((float)(handSync % 100) / 100f, lerpValueFingers);
			rightThumb.MapOtherFinger((float)(handSync % 1000) / 1000f, lerpValueFingers);
			leftIndex.MapOtherFinger((float)(handSync % 10000) / 10000f, lerpValueFingers);
			leftMiddle.MapOtherFinger((float)(handSync % 100000) / 100000f, lerpValueFingers);
			leftThumb.MapOtherFinger((float)(handSync % 1000000) / 1000000f, lerpValueFingers);
			leftHandHoldableStatus = handSync % 10000000 / 1000000;
			rightHandHoldableStatus = handSync % 100000000 / 10000000;
			if (!initializedCosmetics && GorillaGameManager.instance != null && photonView != null && GorillaGameManager.instance.playerCosmeticsLookup.TryGetValue(photonView.Owner.ActorNumber, out tempString))
			{
				initializedCosmetics = true;
				concatStringOfCosmeticsAllowed = tempString;
				CheckForEarlyAccess();
				SetCosmeticsActive();
				myBodyDockPositions.RefreshTransferrableItems();
			}
		}
		if (creator != null)
		{
			tempMatIndex = ((GorillaGameManager.instance != null) ? GorillaGameManager.instance.MyMatIndex(creator) : 0);
			if (setMatIndex != tempMatIndex)
			{
				setMatIndex = tempMatIndex;
				ChangeMaterialLocal(setMatIndex);
			}
		}
	}

	public void SetHeadBodyOffset()
	{
	}

	public void VRRigResize(float ratioVar)
	{
		ratio *= ratioVar;
	}

	public int ReturnHandPosition()
	{
		return 0 + Mathf.FloorToInt(rightIndex.calcT * 9.99f) + Mathf.FloorToInt(rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(leftThumb.calcT * 9.99f) * 100000 + leftHandHoldableStatus * 1000000 + rightHandHoldableStatus * 10000000;
	}

	public void OnDestroy()
	{
		if ((bool)currentRopeSwingTarget && (bool)currentRopeSwingTarget.gameObject)
		{
			UnityEngine.Object.Destroy(currentRopeSwingTarget.gameObject);
		}
		if (!isQuitting)
		{
			if (GorillaParent.instance != null && GorillaParent.instance.vrrigDict != null && creator != null && GorillaParent.instance.vrrigDict.TryGetValue(creator, out var value) && value == this)
			{
				GorillaParent.instance.vrrigDict[creator] = null;
				GorillaParent.instance.vrrigDict.Remove(creator);
			}
			if (photonView != null && photonView.IsMine && PhotonNetwork.InRoom && !photonView.IsRoomView)
			{
				isQuitting = true;
				PhotonNetwork.Destroy(photonView);
			}
			ClearRopeData();
		}
	}

	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		if (isQuitting || !PhotonNetwork.InRoom || creator == null)
		{
			return;
		}
		Dictionary<Photon.Realtime.Player, VRRig> dictionary = GorillaParent.instance?.vrrigDict;
		if (dictionary != null && dictionary.TryGetValue(creator, out var value) && value == this)
		{
			isQuitting = true;
			dictionary.Remove(creator);
			if (rootView.IsMine)
			{
				PhotonNetwork.Destroy(rootView);
			}
		}
	}

	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		creator = info.Sender;
		if (creator != null && creator != info.photonView.Owner)
		{
			GorillaNot.instance.SendReport("creating rigs for someone else", creator.UserId, creator.NickName);
			isQuitting = true;
		}
		if (photonView.IsRoomView)
		{
			isQuitting = true;
			if (PhotonNetwork.IsMasterClient && photonView.IsMine)
			{
				PhotonNetwork.Destroy(base.gameObject);
				Debug.Log("network deleting vrrig");
			}
			else
			{
				base.gameObject.SetActive(value: false);
				Debug.Log("local setting vrrig false");
			}
			if (creator != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", creator.UserId, creator.NickName);
			}
			return;
		}
		if (isQuitting)
		{
			if (info.photonView.IsMine)
			{
				PhotonNetwork.Destroy(info.photonView);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
			return;
		}
		timeSpawned = Time.time;
		base.transform.parent = GorillaParent.instance.GetComponent<GorillaParent>().vrrigParent.transform;
		GorillaParent.instance.vrrigs.Add(this);
		if (object.Equals(creator, PhotonNetwork.LocalPlayer))
		{
			GorillaParent.ReplicatedClientReady();
		}
		if (creator != null && GorillaParent.instance.vrrigDict.TryGetValue(creator, out tempVRRig) && tempVRRig != null && tempVRRig != this)
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent creating multiple vrrigs", creator.UserId, creator.NickName);
			tempVRRig.isQuitting = true;
			UnityEngine.Object.Destroy(tempVRRig.gameObject);
		}
		if (GorillaParent.instance.vrrigDict.ContainsKey(creator))
		{
			GorillaParent.instance.vrrigDict[creator] = this;
		}
		else
		{
			GorillaParent.instance.vrrigDict.Add(creator, this);
		}
		info.photonView.AddCallbackTarget(this);
		if (GorillaGameManager.instance != null && GorillaGameManager.instance.GetComponent<PhotonView>().IsMine)
		{
			object value;
			bool didTutorial = photonView.Owner.CustomProperties.TryGetValue("didTutorial", out value) && !(bool)value;
			Debug.Log("guy who just joined didnt do the tutorial already: " + didTutorial);
			GorillaGameManager.instance.NewVRRig(photonView.Owner, photonView.ViewID, didTutorial);
		}
		Debug.Log(info.Sender.UserId, this);
		SharedStart();
		try
		{
			newPlayerJoined?.Invoke();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	void IGorillaSerializeable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(head.rigTarget.localRotation);
		stream.SendNext(rightHand.rigTarget.localPosition);
		stream.SendNext(rightHand.rigTarget.localRotation);
		stream.SendNext(leftHand.rigTarget.localPosition);
		stream.SendNext(leftHand.rigTarget.localRotation);
		stream.SendNext(base.transform.position);
		stream.SendNext(Mathf.RoundToInt(base.transform.rotation.eulerAngles.y));
		stream.SendNext(ReturnHandPosition());
		stream.SendNext(currentState);
		stream.SendNext(grabbedRopeIndex);
		if (grabbedRopeIndex > 0)
		{
			stream.SendNext(grabbedRopeBoneIndex);
			stream.SendNext(grabbedRopeIsLeft);
			stream.SendNext(grabbedRopeOffset);
		}
	}

	void IGorillaSerializeable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		head.syncRotation = SanitizeQuaternion((Quaternion)stream.ReceiveNext());
		rightHand.syncPos = SanitizeVector3((Vector3)stream.ReceiveNext());
		rightHand.syncRotation = SanitizeQuaternion((Quaternion)stream.ReceiveNext());
		leftHand.syncPos = SanitizeVector3((Vector3)stream.ReceiveNext());
		leftHand.syncRotation = SanitizeQuaternion((Quaternion)stream.ReceiveNext());
		syncPos = SanitizeVector3((Vector3)stream.ReceiveNext());
		syncRotation.eulerAngles = SanitizeVector3(new Vector3(0f, (int)stream.ReceiveNext(), 0f));
		handSync = (int)stream.ReceiveNext();
		currentState = (TransferrableObject.PositionState)stream.ReceiveNext();
		lastPosition = syncPos;
		grabbedRopeIndex = (int)stream.ReceiveNext();
		if (grabbedRopeIndex > 0)
		{
			grabbedRopeBoneIndex = (int)stream.ReceiveNext();
			grabbedRopeIsLeft = (bool)stream.ReceiveNext();
			grabbedRopeOffset = SanitizeVector3((Vector3)stream.ReceiveNext());
		}
		UpdateRopeData();
		AddVelocityToQueue(syncPos, info);
	}

	private void UpdateRopeData()
	{
		if (previousGrabbedRope == grabbedRopeIndex && previousGrabbedRopeBoneIndex == grabbedRopeBoneIndex && previousGrabbedRopeWasLeft == grabbedRopeIsLeft)
		{
			return;
		}
		ClearRopeData();
		if (grabbedRopeIndex > 0)
		{
			PhotonView photonView = PhotonView.Find(grabbedRopeIndex);
			if ((bool)photonView && photonView.TryGetComponent<GorillaRopeSwing>(out var component))
			{
				if (currentRopeSwingTarget == null || currentRopeSwingTarget.gameObject == null)
				{
					currentRopeSwingTarget = new GameObject("RopeSwingTarget").transform;
				}
				if (component.AttachRemotePlayer(creator.ActorNumber, grabbedRopeBoneIndex, currentRopeSwingTarget, grabbedRopeOffset))
				{
					currentRopeSwing = component;
				}
				lastRopeGrabTimer = 0f;
			}
		}
		shouldLerpToRope = true;
		previousGrabbedRope = grabbedRopeIndex;
		previousGrabbedRopeBoneIndex = grabbedRopeBoneIndex;
		previousGrabbedRopeWasLeft = grabbedRopeIsLeft;
	}

	private void ClearRopeData()
	{
		if ((bool)currentRopeSwing)
		{
			currentRopeSwing.DetachRemotePlayer(creator.ActorNumber);
		}
		if ((bool)currentRopeSwingTarget)
		{
			currentRopeSwingTarget.SetParent(null);
		}
		currentRopeSwing = null;
	}

	public void ChangeMaterial(int materialIndex, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			ChangeMaterialLocal(materialIndex);
		}
	}

	public void ChangeMaterialLocal(int materialIndex)
	{
		setMatIndex = materialIndex;
		if (setMatIndex > -1 && setMatIndex < materialsToChangeTo.Length)
		{
			mainSkin.material = materialsToChangeTo[setMatIndex];
		}
		if (lavaParticleSystem != null)
		{
			if (!isOfflineVRRig && materialIndex == 2 && lavaParticleSystem.isStopped)
			{
				lavaParticleSystem.Play();
			}
			else if (!isOfflineVRRig && lavaParticleSystem.isPlaying)
			{
				lavaParticleSystem.Stop();
			}
		}
		if (rockParticleSystem != null)
		{
			if (!isOfflineVRRig && materialIndex == 1 && rockParticleSystem.isStopped)
			{
				rockParticleSystem.Play();
			}
			else if (!isOfflineVRRig && rockParticleSystem.isPlaying)
			{
				rockParticleSystem.Stop();
			}
		}
		if (iceParticleSystem != null)
		{
			if (!isOfflineVRRig && materialIndex == 3 && rockParticleSystem.isStopped)
			{
				iceParticleSystem.Play();
			}
			else if (!isOfflineVRRig && iceParticleSystem.isPlaying)
			{
				iceParticleSystem.Stop();
			}
		}
	}

	[PunRPC]
	public void InitializeNoobMaterial(float red, float green, float blue, bool leftHanded, PhotonMessageInfo info)
	{
		IncrementRPC(info, "InitializeNoobMaterial");
		if (info.Sender == photonView.Owner && (!initialized || (initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(info.Sender.UserId))))
		{
			initialized = true;
			red = Mathf.Clamp(red, 0f, 1f);
			green = Mathf.Clamp(green, 0f, 1f);
			blue = Mathf.Clamp(blue, 0f, 1f);
			InitializeNoobMaterialLocal(red, green, blue, leftHanded);
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent init noob", info.Sender.UserId, info.Sender.NickName);
		}
		playerLeftHanded = leftHanded;
	}

	public void InitializeNoobMaterialLocal(float red, float green, float blue, bool leftHanded)
	{
		Color color = new Color(red, green, blue);
		materialsToChangeTo[0].color = color;
		if (photonView != null)
		{
			playerText.text = NormalizeName(doIt: true, photonView.Owner.NickName);
		}
		else if (showName)
		{
			playerText.text = PlayerPrefs.GetString("playerName");
		}
		SetColor(color);
	}

	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				text = new string(Array.FindAll(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
			}
		}
		return text;
	}

	public void SetJumpLimitLocal(float maxJumpSpeed)
	{
		GorillaLocomotion.Player.Instance.maxJumpSpeed = maxJumpSpeed;
	}

	public void SetJumpMultiplierLocal(float jumpMultiplier)
	{
		GorillaLocomotion.Player.Instance.jumpMultiplier = jumpMultiplier;
	}

	[PunRPC]
	public void SetTaggedTime(PhotonMessageInfo info)
	{
		IncrementRPC(info, "SetTaggedTime");
		if (GorillaGameManager.instance != null)
		{
			if (info.Sender == PhotonNetwork.MasterClient)
			{
				GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
				GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.StartVibration(forLeftController: false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			}
			else
			{
				GorillaNot.instance.SendReport("inappropriate tag data being sent set tagged time", info.Sender.UserId, info.Sender.NickName);
			}
		}
	}

	[PunRPC]
	public void SetSlowedTime(PhotonMessageInfo info)
	{
		IncrementRPC(info, "SetSlowedTime");
		if (!(GorillaGameManager.instance != null))
		{
			return;
		}
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Slowed)
			{
				GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.StartVibration(forLeftController: false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			}
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, GorillaTagger.Instance.slowCooldown);
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent set slowed time", info.Sender.UserId, info.Sender.NickName);
		}
	}

	[PunRPC]
	public void SetJoinTaggedTime(PhotonMessageInfo info)
	{
		IncrementRPC(info, "SetJoinTaggedTime");
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(forLeftController: false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent set join tagged time", info.Sender.UserId, info.Sender.NickName);
		}
	}

	[PunRPC]
	public void RequestMaterialColor(Photon.Realtime.Player askingPlayer, bool noneBool, PhotonMessageInfo info)
	{
		IncrementRPC(info, "RequestMaterialColor");
		if (photonView.IsMine)
		{
			photonView.RPC("InitializeNoobMaterial", info.Sender, materialsToChangeTo[0].color.r, materialsToChangeTo[0].color.g, materialsToChangeTo[0].color.b, GorillaComputer.instance.leftHanded);
		}
	}

	[PunRPC]
	public void RequestCosmetics(PhotonMessageInfo info)
	{
		IncrementRPC(info, "RequestCosmetics");
		if (photonView.IsMine && CosmeticsController.instance != null)
		{
			string[] array = CosmeticsController.instance.currentWornSet.ToDisplayNameArray();
			string[] array2 = CosmeticsController.instance.tryOnSet.ToDisplayNameArray();
			photonView.RPC("UpdateCosmeticsWithTryon", info.Sender, array, array2);
		}
	}

	[PunRPC]
	public void PlayTagSound(int soundIndex, float soundVolume, PhotonMessageInfo info)
	{
		IncrementRPC(info, "PlayTagSound");
		if (info.Sender.IsMasterClient)
		{
			tagSound.volume = Mathf.Max(0.25f, soundVolume);
			tagSound.PlayOneShot(clipToPlay[soundIndex]);
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent play tag sound", info.Sender.UserId, info.Sender.NickName);
		}
	}

	public void Bonk(int soundIndex, float bonkPercent, PhotonMessageInfo info)
	{
		if (info.Sender == photonView.Owner)
		{
			if (bonkTime + bonkCooldown < Time.time)
			{
				bonkTime = Time.time;
				tagSound.volume = bonkPercent * 0.25f;
				tagSound.PlayOneShot(clipToPlay[soundIndex]);
				if (photonView.IsMine)
				{
					GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
					GorillaTagger.Instance.StartVibration(forLeftController: false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				}
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent bonk", info.Sender.UserId, info.Sender.NickName);
		}
	}

	[PunRPC]
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		IncrementRPC(info, "PlayDrum");
		senderRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
		if (senderRig == null || senderRig.muted)
		{
			return;
		}
		if (drumIndex < 0 || drumIndex >= musicDrums.Length || (senderRig.transform.position - base.transform.position).sqrMagnitude > 9f)
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent drum", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		AudioSource audioSource = (photonView.IsMine ? GorillaTagger.Instance.offlineVRRig.musicDrums[drumIndex] : musicDrums[drumIndex]);
		if (audioSource.gameObject.activeSelf)
		{
			float instrumentVolume = GorillaComputer.instance.instrumentVolume;
			audioSource.time = 0f;
			audioSource.volume = Mathf.Max(Mathf.Min(instrumentVolume, drumVolume * instrumentVolume), 0f);
			audioSource.Play();
		}
	}

	[PunRPC]
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		IncrementRPC(info, "PlaySelfOnlyInstrument");
		if (info.Sender != photonView.Owner || muted)
		{
			return;
		}
		if (selfOnlyIndex >= 0 && selfOnlyIndex < instrumentSelfOnly.Length && info.Sender == photonView.Owner)
		{
			if (instrumentSelfOnly[selfOnlyIndex].gameObject.activeSelf)
			{
				instrumentSelfOnly[selfOnlyIndex].PlayNote(noteIndex, Mathf.Max(Mathf.Min(GorillaComputer.instance.instrumentVolume, instrumentVol * GorillaComputer.instance.instrumentVolume), 0f) / 2f);
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent self only instrument", info.Sender.UserId, info.Sender.NickName);
		}
	}

	[PunRPC]
	public void PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info)
	{
		IncrementRPC(info, "PlayHandTap");
		if (info.Sender == photonView.Owner)
		{
			PlayHandTapLocal(soundIndex, isLeftHand, Mathf.Max(tapVolume, 0.1f));
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent hand tap", info.Sender.UserId, info.Sender.NickName);
		}
	}

	public void PlayHandTapLocal(int soundIndex, bool isLeftHand, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < GorillaLocomotion.Player.Instance.materialData.Count)
		{
			if (isLeftHand)
			{
				leftHandPlayer.volume = tapVolume;
				leftHandPlayer.clip = (GorillaLocomotion.Player.Instance.materialData[soundIndex].overrideAudio ? GorillaLocomotion.Player.Instance.materialData[soundIndex].audio : GorillaLocomotion.Player.Instance.materialData[0].audio);
				leftHandPlayer.PlayOneShot(leftHandPlayer.clip);
			}
			else
			{
				rightHandPlayer.volume = tapVolume;
				rightHandPlayer.clip = (GorillaLocomotion.Player.Instance.materialData[soundIndex].overrideAudio ? GorillaLocomotion.Player.Instance.materialData[soundIndex].audio : GorillaLocomotion.Player.Instance.materialData[0].audio);
				rightHandPlayer.PlayOneShot(rightHandPlayer.clip);
			}
		}
	}

	[PunRPC]
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		IncrementRPC(info, "PlaySplashEffect");
		if (info.Sender == photonView.Owner)
		{
			if (!((base.transform.position - splashPosition).sqrMagnitude < 9f))
			{
				return;
			}
			float time = Time.time;
			int num = -1;
			float num2 = time + 10f;
			for (int i = 0; i < splashEffectTimes.Length; i++)
			{
				if (splashEffectTimes[i] < num2)
				{
					num2 = splashEffectTimes[i];
					num = i;
				}
			}
			if (time - 0.5f > num2)
			{
				splashEffectTimes[num] = time;
				boundingRadius = Mathf.Clamp(boundingRadius, 0.01f, 0.5f);
				ObjectPools.instance.Instantiate(GorillaLocomotion.Player.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GorillaLocomotion.Player.Instance.waterParams.rippleEffectScale * boundingRadius * 2f);
				splashScale = Mathf.Clamp(splashScale, 0.01f, 1f);
				ObjectPools.instance.Instantiate(GorillaLocomotion.Player.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater);
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent splash effect", info.Sender.UserId, info.Sender.NickName);
		}
	}

	[PunRPC]
	public void PlaySlamEffects(Vector3 slamPosition, Quaternion slamRotation, PhotonMessageInfo info)
	{
		IncrementRPC(info, "PlaySlamEffects");
		if (info.Sender == photonView.Owner)
		{
			GameObject fxEffects = ObjectPools.instance.Instantiate(GorillaLocomotion.Player.Instance.wizardStaffSlamEffects, slamPosition, slamRotation);
			PlaySlamEffectsLocal(fxEffects);
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent slam effect", info.Sender.UserId, info.Sender.NickName);
		}
	}

	public void PlaySlamEffectsLocal(GameObject fxEffects)
	{
		if ((bool)fxEffects)
		{
			fxEffects.SetActive(value: true);
			if (fxEffects.TryGetComponent<SoundBankPlayer>(out var component))
			{
				component.Play();
			}
		}
	}

	[PunRPC]
	public void PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		IncrementRPC(info, "PlayGeodeEffect");
		if (info.Sender == photonView.Owner)
		{
			if ((base.transform.position - hitPosition).sqrMagnitude < 9f && (bool)geodeCrackingSound)
			{
				geodeCrackingSound.Play();
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent geode effect", info.Sender.UserId, info.Sender.NickName);
		}
	}

	public void PlayClimbSound(AudioClip clip, bool isLeftHand)
	{
		if (isLeftHand)
		{
			leftHandPlayer.volume = 0.1f;
			leftHandPlayer.clip = clip;
			leftHandPlayer.PlayOneShot(leftHandPlayer.clip);
		}
		else
		{
			rightHandPlayer.volume = 0.1f;
			rightHandPlayer.clip = clip;
			rightHandPlayer.PlayOneShot(rightHandPlayer.clip);
		}
	}

	[PunRPC]
	public void UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
		IncrementRPC(info, "UpdateCosmetics");
		if (info.Sender == photonView.Owner)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			LocalUpdateCosmetics(newSet);
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
		}
	}

	[PunRPC]
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
		IncrementRPC(info, "UpdateCosmeticsWithTryon");
		if (info.Sender == photonView.Owner)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet);
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", info.Sender.UserId, info.Sender.NickName);
		}
	}

	public void UpdateAllowedCosmetics()
	{
		if (GorillaGameManager.instance != null && GorillaGameManager.instance.playerCosmeticsLookup.TryGetValue(photonView.Owner.ActorNumber, out tempString))
		{
			concatStringOfCosmeticsAllowed = tempString;
			CheckForEarlyAccess();
		}
	}

	public void LocalUpdateCosmetics(CosmeticsController.CosmeticSet newSet)
	{
		cosmeticSet = newSet;
		if (initializedCosmetics)
		{
			SetCosmeticsActive();
		}
	}

	public void LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet newSet, CosmeticsController.CosmeticSet newTryOnSet)
	{
		cosmeticSet = newSet;
		tryOnSet = newTryOnSet;
		if (initializedCosmetics)
		{
			SetCosmeticsActive();
		}
	}

	private void CheckForEarlyAccess()
	{
		if (IsItemAllowed("Early Access Supporter Pack"))
		{
			concatStringOfCosmeticsAllowed += "LBAAE.LFAAM.LFAAN.LHAAA.LHAAK.LHAAL.LHAAM.LHAAN.LHAAO.LHAAP.LHABA.LHABB.";
		}
		initializedCosmetics = true;
	}

	public void SetCosmeticsActive()
	{
		if (!(CosmeticsController.instance == null))
		{
			prevSet.CopyItems(mergedSet);
			mergedSet.MergeSets(inTryOnRoom ? tryOnSet : null, cosmeticSet);
			BodyDockPositions component = GetComponent<BodyDockPositions>();
			mergedSet.ActivateCosmetics(prevSet, this, component, CosmeticsController.instance.nullItem, cosmeticsObjectRegistry);
		}
	}

	public void GetUserCosmeticsAllowed()
	{
		if (CosmeticsController.instance != null)
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				foreach (ItemInstance item in result.Inventory)
				{
					if (item.CatalogVersion == CosmeticsController.instance.catalog)
					{
						concatStringOfCosmeticsAllowed += item.ItemId;
					}
				}
				Debug.Log("successful result. allowed cosmetics are: " + concatStringOfCosmeticsAllowed);
				CheckForEarlyAccess();
				SetCosmeticsActive();
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				initializedCosmetics = true;
				SetCosmeticsActive();
			});
		}
		concatStringOfCosmeticsAllowed += "Slingshot";
	}

	private void Quitting()
	{
		isQuitting = true;
	}

	public void GenerateFingerAngleLookupTables()
	{
		GenerateTableIndex(ref leftIndex);
		GenerateTableIndex(ref rightIndex);
		GenerateTableMiddle(ref leftMiddle);
		GenerateTableMiddle(ref rightMiddle);
		GenerateTableThumb(ref leftThumb);
		GenerateTableThumb(ref rightThumb);
	}

	private void GenerateTableThumb(ref VRMapThumb thumb)
	{
		thumb.angle1Table = new Quaternion[11];
		thumb.angle2Table = new Quaternion[11];
		for (int i = 0; i < thumb.angle1Table.Length; i++)
		{
			Debug.Log((float)i / 10f);
			thumb.angle1Table[i] = Quaternion.Lerp(Quaternion.Euler(thumb.startingAngle1), Quaternion.Euler(thumb.closedAngle1), (float)i / 10f);
			thumb.angle2Table[i] = Quaternion.Lerp(Quaternion.Euler(thumb.startingAngle2), Quaternion.Euler(thumb.closedAngle2), (float)i / 10f);
		}
	}

	private void GenerateTableIndex(ref VRMapIndex index)
	{
		index.angle1Table = new Quaternion[11];
		index.angle2Table = new Quaternion[11];
		index.angle3Table = new Quaternion[11];
		for (int i = 0; i < index.angle1Table.Length; i++)
		{
			index.angle1Table[i] = Quaternion.Lerp(Quaternion.Euler(index.startingAngle1), Quaternion.Euler(index.closedAngle1), (float)i / 10f);
			index.angle2Table[i] = Quaternion.Lerp(Quaternion.Euler(index.startingAngle2), Quaternion.Euler(index.closedAngle2), (float)i / 10f);
			index.angle3Table[i] = Quaternion.Lerp(Quaternion.Euler(index.startingAngle3), Quaternion.Euler(index.closedAngle3), (float)i / 10f);
		}
	}

	private void GenerateTableMiddle(ref VRMapMiddle middle)
	{
		middle.angle1Table = new Quaternion[11];
		middle.angle2Table = new Quaternion[11];
		middle.angle3Table = new Quaternion[11];
		for (int i = 0; i < middle.angle1Table.Length; i++)
		{
			middle.angle1Table[i] = Quaternion.Lerp(Quaternion.Euler(middle.startingAngle1), Quaternion.Euler(middle.closedAngle1), (float)i / 10f);
			middle.angle2Table[i] = Quaternion.Lerp(Quaternion.Euler(middle.startingAngle2), Quaternion.Euler(middle.closedAngle2), (float)i / 10f);
			middle.angle3Table[i] = Quaternion.Lerp(Quaternion.Euler(middle.startingAngle3), Quaternion.Euler(middle.closedAngle3), (float)i / 10f);
		}
	}

	private Quaternion SanitizeQuaternion(Quaternion quat)
	{
		if (float.IsNaN(quat.w) || float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsInfinity(quat.w) || float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z))
		{
			return Quaternion.identity;
		}
		return quat;
	}

	private Vector3 SanitizeVector3(Vector3 vec)
	{
		if (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z) || float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z))
		{
			return Vector3.zero;
		}
		return Vector3.ClampMagnitude(vec, 1000f);
	}

	private void IncrementRPC(PhotonMessageInfo info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	private void AddVelocityToQueue(Vector3 position, PhotonMessageInfo info)
	{
		Vector3 velocity;
		if (velocityHistoryList.Count == 0)
		{
			velocity = Vector3.zero;
			lastPosition = position;
		}
		else
		{
			velocity = (position - lastPosition) / (float)(info.SentServerTime - velocityHistoryList[0].time);
		}
		velocityHistoryList.Insert(0, new VelocityTime(velocity, info.SentServerTime));
		if (velocityHistoryList.Count > velocityHistoryMaxLength)
		{
			velocityHistoryList.RemoveRange(velocityHistoryMaxLength, velocityHistoryList.Count - velocityHistoryMaxLength);
		}
	}

	private Vector3 ReturnVelocityAtTime(double timeToReturn)
	{
		if (velocityHistoryList.Count <= 1)
		{
			return Vector3.zero;
		}
		int num = 0;
		int num2 = velocityHistoryList.Count - 1;
		int num3 = 0;
		if (num2 == num)
		{
			return velocityHistoryList[num].vel;
		}
		while (num2 - num > 1 && num3 < 1000)
		{
			num3++;
			int num4 = (num2 - num) / 2;
			if (velocityHistoryList[num4].time > timeToReturn)
			{
				num2 = num4;
			}
			else
			{
				num = num4;
			}
		}
		float num5 = (float)(velocityHistoryList[num].time - timeToReturn);
		double num6 = velocityHistoryList[num].time - velocityHistoryList[num2].time;
		if (num6 == 0.0)
		{
			num6 = 0.001;
		}
		num5 /= (float)num6;
		num5 = Mathf.Clamp(num5, 0f, 1f);
		return Vector3.Lerp(velocityHistoryList[num].vel, velocityHistoryList[num2].vel, num5);
	}

	public void SetColor(Color color)
	{
		onColorInitialized?.Invoke(color);
		onColorInitialized = delegate
		{
		};
		colorInitialized = true;
		playerColor = color;
	}

	public void OnColorInitialized(Action<Color> action)
	{
		if (colorInitialized)
		{
			action(playerColor);
		}
		else
		{
			onColorInitialized = (Action<Color>)Delegate.Combine(onColorInitialized, action);
		}
	}

	private void OnEnable()
	{
		if (currentRopeSwingTarget != null)
		{
			currentRopeSwingTarget.SetParent(null);
		}
	}

	void IPreDisable.PreDisable()
	{
		ClearRopeData();
		if ((bool)currentRopeSwingTarget)
		{
			currentRopeSwingTarget.SetParent(base.transform);
		}
		EnableHuntWatch(on: false);
		EnableBattleCosmetics(on: false);
		concatStringOfCosmeticsAllowed = "";
		if (cosmeticSet != null)
		{
			mergedSet.DeactivateAllCosmetcs(myBodyDockPositions, CosmeticsController.instance.nullItem, cosmeticsObjectRegistry);
			mergedSet.ClearSet(CosmeticsController.instance.nullItem);
			prevSet.ClearSet(CosmeticsController.instance.nullItem);
			tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			cosmeticSet.ClearSet(CosmeticsController.instance.nullItem);
		}
	}

	private void OnDisable()
	{
		initialized = false;
		muted = false;
		photonView = null;
		voiceAudio = null;
		tempRig = null;
		timeSpawned = 0f;
		initializedCosmetics = false;
		velocityHistoryList.Clear();
		tempMatIndex = 0;
		setMatIndex = 0;
		ChangeMaterialLocal(setMatIndex);
		creator = null;
	}

	public void NetInitialize()
	{
		timeSpawned = Time.time;
		if (PhotonNetwork.InRoom)
		{
			GorillaGameManager instance = GorillaGameManager.instance;
			object value;
			if (instance != null)
			{
				if (instance is GorillaHuntManager || instance.GameMode() == "HUNT")
				{
					EnableHuntWatch(on: true);
				}
				else if (instance is GorillaBattleManager || instance.GameMode() == "BATTLE")
				{
					EnableBattleCosmetics(on: true);
				}
			}
			else if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out value))
			{
				string text = value.ToString();
				if (text.Contains("HUNT"))
				{
					EnableHuntWatch(on: true);
				}
				else if (text.Contains("BATTLE"))
				{
					EnableBattleCosmetics(on: true);
				}
			}
		}
		if (photonView != null)
		{
			base.transform.position = photonView.gameObject.transform.position;
			base.transform.rotation = photonView.gameObject.transform.rotation;
		}
		try
		{
			newPlayerJoined?.Invoke();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	public void EnableHuntWatch(bool on)
	{
		huntComputer.SetActive(on);
	}

	public void EnableBattleCosmetics(bool on)
	{
		battleBalloons.gameObject.SetActive(on);
	}
}
