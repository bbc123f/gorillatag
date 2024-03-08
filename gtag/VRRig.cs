using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using WebSocketSharp;

public class VRRig : MonoBehaviour, IWrappedSerializable, INetworkStruct, IPreDisable, IUserCosmeticsCallback, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject, IGuidedRefReceiverMono
{
	public int ActiveTransferrableObjectIndex(int idx)
	{
		return this.reliableState.activeTransferrableObjectIndex[idx];
	}

	public int ActiveTransferrableObjectIndexLength()
	{
		return this.reliableState.activeTransferrableObjectIndex.Length;
	}

	public void SetActiveTransferrableObjectIndex(int idx, int v)
	{
		if (this.reliableState.activeTransferrableObjectIndex[idx] != v)
		{
			this.reliableState.activeTransferrableObjectIndex[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	public TransferrableObject.PositionState TransferrablePosStates(int idx)
	{
		return this.reliableState.transferrablePosStates[idx];
	}

	public void SetTransferrablePosStates(int idx, TransferrableObject.PositionState v)
	{
		if (this.reliableState.transferrablePosStates[idx] != v)
		{
			this.reliableState.transferrablePosStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	public TransferrableObject.ItemStates TransferrableItemStates(int idx)
	{
		return this.reliableState.transferrableItemStates[idx];
	}

	public void SetTransferrableItemStates(int idx, TransferrableObject.ItemStates v)
	{
		if (this.reliableState.transferrableItemStates[idx] != v)
		{
			this.reliableState.transferrableItemStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	public void SetTransferrableDockPosition(int idx, BodyDockPositions.DropPositions v)
	{
		if (this.reliableState.transferableDockPositions[idx] != v)
		{
			this.reliableState.transferableDockPositions[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	public BodyDockPositions.DropPositions TransferrableDockPosition(int idx)
	{
		return this.reliableState.transferableDockPositions[idx];
	}

	public int WearablePackedStates
	{
		get
		{
			return this.reliableState.wearablesPackedStates;
		}
		set
		{
			if (this.reliableState.wearablesPackedStates != value)
			{
				this.reliableState.wearablesPackedStates = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public int LeftThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.lThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.lThrowableProjectileIndex != value)
			{
				this.reliableState.lThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public int RightThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.rThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.rThrowableProjectileIndex != value)
			{
				this.reliableState.rThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public Color LeftThrowableProjectileColor
	{
		get
		{
			return this.reliableState.lThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.lThrowableProjectileColor.CompareAs255Unclamped(value))
			{
				this.reliableState.lThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public Color RightThrowableProjectileColor
	{
		get
		{
			return this.reliableState.rThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.rThrowableProjectileColor.CompareAs255Unclamped(value))
			{
				this.reliableState.rThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public Color GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightThrowableProjectileColor;
		}
		return this.LeftThrowableProjectileColor;
	}

	public void SetThrowableProjectileColor(bool isLeftHand, Color color)
	{
		if (isLeftHand)
		{
			this.LeftThrowableProjectileColor = color;
			return;
		}
		this.RightThrowableProjectileColor = color;
	}

	public void SetRandomThrowableModelIndex(int randModelIndex)
	{
		this.RandomThrowableIndex = randModelIndex;
	}

	public int GetRandomThrowableModelIndex()
	{
		return this.RandomThrowableIndex;
	}

	private int RandomThrowableIndex
	{
		get
		{
			return this.reliableState.randomThrowableIndex;
		}
		set
		{
			if (this.reliableState.randomThrowableIndex != value)
			{
				this.reliableState.randomThrowableIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public bool IsMicEnabled
	{
		get
		{
			return this.reliableState.isMicEnabled;
		}
		set
		{
			if (this.reliableState.isMicEnabled != value)
			{
				this.reliableState.isMicEnabled = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public int SizeLayerMask
	{
		get
		{
			return this.reliableState.sizeLayerMask;
		}
		set
		{
			if (this.reliableState.sizeLayerMask != value)
			{
				this.reliableState.sizeLayerMask = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public Photon.Realtime.Player Creator
	{
		get
		{
			return this.creator;
		}
	}

	internal bool Initialized
	{
		get
		{
			return this.initialized;
		}
	}

	public float SpeakingLoudness
	{
		get
		{
			return this.speakingLoudness;
		}
	}

	private void Awake()
	{
		this.fxSettings = Object.Instantiate<FXSystemSettings>(this.sharedFXSettings);
		this.fxSettings.forLocalRig = this.isOfflineVRRig;
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		foreach (GameObject gameObject in this.cosmetics)
		{
			GameObject gameObject2;
			if (!dictionary.TryGetValue(gameObject.name, out gameObject2))
			{
				dictionary.Add(gameObject.name, gameObject);
			}
		}
		foreach (GameObject gameObject3 in this.overrideCosmetics)
		{
			GameObject gameObject2;
			if (dictionary.TryGetValue(gameObject3.name, out gameObject2) && gameObject2.name == gameObject3.name)
			{
				gameObject2.name = "OVERRIDDEN";
			}
		}
		this.cosmetics = this.cosmetics.Concat(this.overrideCosmetics).ToArray<GameObject>();
		this.cosmeticsObjectRegistry.Initialize(this.cosmetics);
		this.lastPosition = base.transform.position;
		this.SharedStart();
	}

	private void Start()
	{
	}

	private void EnsureInstantiatedMaterial()
	{
		if (this.myDefaultSkinMaterialInstance == null)
		{
			this.myDefaultSkinMaterialInstance = Object.Instantiate<Material>(this.materialsToChangeTo[0]);
			this.materialsToChangeTo[0] = this.myDefaultSkinMaterialInstance;
		}
	}

	private void ApplyColorCode()
	{
		float @float = PlayerPrefs.GetFloat("redValue", 0.16f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0.16f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0.16f);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
	}

	private void SharedStart()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.isInitialized = true;
		this.myBodyDockPositions = base.GetComponent<BodyDockPositions>();
		this.reliableState.SharedStart(this.isOfflineVRRig, this.myBodyDockPositions);
		this.concatStringOfCosmeticsAllowed = "";
		this.playerText.transform.parent.GetComponent<Canvas>().worldCamera = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		this.EnsureInstantiatedMaterial();
		this.initialized = false;
		this.currentState = TransferrableObject.PositionState.OnChest;
		if (this.setMatIndex > -1 && this.setMatIndex < this.materialsToChangeTo.Length)
		{
			this.mainSkin.material = this.materialsToChangeTo[this.setMatIndex];
		}
		if (this.isOfflineVRRig)
		{
			CosmeticsController.instance.currentWornSet.LoadFromPlayerPreferences(CosmeticsController.instance);
			if (Application.platform == RuntimePlatform.Android && this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			base.StartCoroutine(this.OccasionalUpdate());
		}
		else if (!this.isOfflineVRRig)
		{
			if (this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.head.syncPos = -this.headBodyOffset;
		}
		if (base.transform.parent == null)
		{
			base.transform.parent = GorillaParent.instance.transform;
		}
		GorillaSkin.ApplyToRig(this, this.defaultSkin);
		base.Invoke("ApplyColorCode", 1f);
	}

	private IEnumerator OccasionalUpdate()
	{
		for (;;)
		{
			try
			{
				if (RoomSystem.JoinedRoom && NetworkSystem.Instance.IsMasterClient && GorillaGameModes.GameMode.ActiveNetworkHandler.IsNull())
				{
					GorillaGameModes.GameMode.LoadGameModeFromProperty();
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public bool IsItemAllowed(string itemName)
	{
		if (itemName == "Slingshot")
		{
			return PhotonNetwork.InRoom && GorillaGameManager.instance is GorillaBattleManager;
		}
		if (this.concatStringOfCosmeticsAllowed == null)
		{
			return false;
		}
		if (this.concatStringOfCosmeticsAllowed.Contains(itemName))
		{
			return true;
		}
		bool canTryOn = CosmeticsController.instance.GetItemFromDict(itemName).canTryOn;
		return this.inTryOnRoom && canTryOn;
	}

	private void LateUpdate()
	{
		base.transform.localScale = Vector3.one * this.scaleFactor;
		if (this.isOfflineVRRig)
		{
			if (GorillaGameManager.instance != null)
			{
				this.speedArray = GorillaGameManager.instance.LocalPlayerSpeed();
				GorillaLocomotion.Player.Instance.jumpMultiplier = this.speedArray[1];
				GorillaLocomotion.Player.Instance.maxJumpSpeed = this.speedArray[0];
			}
			else
			{
				GorillaLocomotion.Player.Instance.jumpMultiplier = 1.1f;
				GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
			}
			this.scaleFactor = GorillaLocomotion.Player.Instance.scale;
			base.transform.localScale = Vector3.one * this.scaleFactor;
			base.transform.eulerAngles = new Vector3(0f, this.mainCamera.transform.rotation.eulerAngles.y, 0f);
			this.syncPos = this.mainCamera.transform.position + this.headConstraint.rotation * this.head.trackingPositionOffset * this.scaleFactor + base.transform.rotation * this.headBodyOffset * this.scaleFactor;
			base.transform.position = this.syncPos;
			this.head.MapMine(this.scaleFactor, this.playerOffsetTransform);
			this.rightHand.MapMine(this.scaleFactor, this.playerOffsetTransform);
			this.leftHand.MapMine(this.scaleFactor, this.playerOffsetTransform);
			this.rightIndex.MapMyFinger(this.lerpValueFingers);
			this.rightMiddle.MapMyFinger(this.lerpValueFingers);
			this.rightThumb.MapMyFinger(this.lerpValueFingers);
			this.leftIndex.MapMyFinger(this.lerpValueFingers);
			this.leftMiddle.MapMyFinger(this.lerpValueFingers);
			this.leftThumb.MapMyFinger(this.lerpValueFingers);
			if (GorillaTagger.Instance.loadedDeviceName == "Oculus")
			{
				this.mainSkin.enabled = OVRManager.hasInputFocus;
			}
			this.mainSkin.enabled = !GorillaLocomotion.Player.Instance.inOverlay;
			this.speakingLoudness = 0f;
			if (this.shouldSendSpeakingLoudness && this.photonView)
			{
				PhotonVoiceView component = this.photonView.GetComponent<PhotonVoiceView>();
				if (component && component.RecorderInUse)
				{
					if (this.audioDesc != component.RecorderInUse.InputSource)
					{
						this.audioDesc = component.RecorderInUse.InputSource;
						this.currentMicWrapper = this.audioDesc as MicWrapper;
					}
					if (this.currentMicWrapper != null)
					{
						int num = this.replacementVoiceDetectionDelay;
						float[] array = new float[num];
						if (this.currentMicWrapper.Mic.samples >= num && this.currentMicWrapper.Mic.GetData(array, this.currentMicWrapper.Mic.samples - num))
						{
							float num2 = 0f;
							for (int i = 0; i < num; i++)
							{
								float num3 = Mathf.Sqrt(array[i]);
								if (num3 > num2)
								{
									num2 = num3;
								}
							}
							this.speakingLoudness = num2;
						}
					}
				}
			}
		}
		else
		{
			if (this.voiceAudio != null)
			{
				float num4 = (GorillaTagger.Instance.offlineVRRig.transform.localScale.x - base.transform.localScale.x) / this.pitchScale + this.pitchOffset;
				float num5 = (this.UsingHauntedRing ? this.HauntedRingVoicePitch : num4);
				num5 = (this.IsHaunted ? this.HauntedVoicePitch : num5);
				if (!Mathf.Approximately(this.voiceAudio.pitch, num5))
				{
					this.voiceAudio.pitch = num5;
				}
				bool isHaunted = GorillaTagger.Instance.offlineVRRig.IsHaunted;
				if (isHaunted != this.playerWasHaunted)
				{
					if (isHaunted)
					{
						this.nonHauntedVolume = this.voiceAudio.volume;
						this.voiceAudio.volume = this.HauntedHearingVolume;
					}
					else
					{
						this.voiceAudio.volume = this.nonHauntedVolume;
					}
					this.playerWasHaunted = isHaunted;
				}
			}
			if (Time.time > this.timeSpawned + this.doNotLerpConstant)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.syncPos, this.lerpValueBody * 0.66f);
				if (this.currentRopeSwing && this.currentRopeSwingTarget)
				{
					Vector3 vector;
					if (this.grabbedRopeIsLeft)
					{
						vector = this.currentRopeSwingTarget.position - this.leftHandTransform.position;
					}
					else
					{
						vector = this.currentRopeSwingTarget.position - this.rightHandTransform.position;
					}
					if (this.shouldLerpToRope)
					{
						base.transform.position += Vector3.Lerp(Vector3.zero, vector, this.lastRopeGrabTimer * 4f);
						if (this.lastRopeGrabTimer < 1f)
						{
							this.lastRopeGrabTimer += Time.deltaTime;
						}
					}
					else
					{
						base.transform.position += vector;
					}
				}
				else if (this.currentHoldParent != null)
				{
					base.transform.position += this.currentHoldParent.TransformPoint(this.grabbedRopeOffset) - (this.grabbedRopeIsLeft ? this.leftHandTransform : this.rightHandTransform).position;
				}
			}
			else
			{
				base.transform.position = this.syncPos;
			}
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.syncRotation, this.lerpValueBody);
			base.transform.position = this.SanitizeVector3(base.transform.position);
			base.transform.rotation = this.SanitizeQuaternion(base.transform.rotation);
			this.head.syncPos = base.transform.rotation * -this.headBodyOffset * this.scaleFactor;
			this.head.MapOther(this.lerpValueBody);
			this.rightHand.MapOther(this.lerpValueBody);
			this.leftHand.MapOther(this.lerpValueBody);
			this.rightIndex.MapOtherFinger((float)(this.handSync % 10) / 10f, this.lerpValueFingers);
			this.rightMiddle.MapOtherFinger((float)(this.handSync % 100) / 100f, this.lerpValueFingers);
			this.rightThumb.MapOtherFinger((float)(this.handSync % 1000) / 1000f, this.lerpValueFingers);
			this.leftIndex.MapOtherFinger((float)(this.handSync % 10000) / 10000f, this.lerpValueFingers);
			this.leftMiddle.MapOtherFinger((float)(this.handSync % 100000) / 100000f, this.lerpValueFingers);
			this.leftThumb.MapOtherFinger((float)(this.handSync % 1000000) / 1000000f, this.lerpValueFingers);
			this.leftHandHoldableStatus = this.handSync % 10000000 / 1000000;
			this.rightHandHoldableStatus = this.handSync % 100000000 / 10000000;
		}
		if (this.creator != null)
		{
			ScienceExperimentManager instance = ScienceExperimentManager.instance;
			int num6;
			if (instance != null && instance.GetMaterialIfPlayerInGame(this.creator.ActorNumber, out num6))
			{
				this.tempMatIndex = num6;
			}
			else
			{
				this.tempMatIndex = ((GorillaGameManager.instance != null) ? GorillaGameManager.instance.MyMatIndex(this.creator) : 0);
			}
			if (this.setMatIndex != this.tempMatIndex)
			{
				this.setMatIndex = this.tempMatIndex;
				this.ChangeMaterialLocal(this.setMatIndex);
			}
		}
	}

	public void SetHeadBodyOffset()
	{
	}

	public void VRRigResize(float ratioVar)
	{
		this.ratio *= ratioVar;
	}

	public int ReturnHandPosition()
	{
		return 0 + Mathf.FloorToInt(this.rightIndex.calcT * 9.99f) + Mathf.FloorToInt(this.rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(this.rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(this.leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(this.leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(this.leftThumb.calcT * 9.99f) * 100000 + this.leftHandHoldableStatus * 1000000 + this.rightHandHoldableStatus * 10000000;
	}

	public void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GuidedRefHub.UnregisterTarget<VRRig>(this, true);
		if (this.currentRopeSwingTarget && this.currentRopeSwingTarget.gameObject)
		{
			Object.Destroy(this.currentRopeSwingTarget.gameObject);
		}
		this.ClearRopeData();
	}

	public object OnSerializeWrite()
	{
		InputStruct inputStruct = default(InputStruct);
		inputStruct.headRotation = this.head.rigTarget.localRotation;
		inputStruct.rightHandPosition = this.rightHand.rigTarget.localPosition;
		inputStruct.rightHandRotation = this.rightHand.rigTarget.localRotation;
		inputStruct.leftHandPosition = this.leftHand.rigTarget.localPosition;
		inputStruct.leftHandRotation = this.leftHand.rigTarget.localRotation;
		inputStruct.position = base.transform.position;
		inputStruct.roundedRotation = Mathf.RoundToInt(base.transform.rotation.eulerAngles.y);
		inputStruct.handPosition = this.ReturnHandPosition();
		inputStruct.state = this.currentState;
		inputStruct.remoteUseReplacementVoice = this.remoteUseReplacementVoice;
		inputStruct.speakingLoudness = this.speakingLoudness;
		inputStruct.grabbedRopeIndex = this.grabbedRopeIndex;
		if (this.grabbedRopeIndex > 0)
		{
			inputStruct.ropeBoneIndex = this.grabbedRopeBoneIndex;
			inputStruct.ropeGrabIsLeft = this.grabbedRopeIsLeft;
			inputStruct.ropeGrabOffset = this.grabbedRopeOffset;
		}
		double num = NetworkSystem.Instance.SimTick / 1000.0;
		inputStruct.serverTimeStamp = num;
		return inputStruct;
	}

	public void OnSerializeRead(object objectData)
	{
		InputStruct inputStruct = (InputStruct)objectData;
		this.head.syncRotation = this.SanitizeQuaternion(inputStruct.headRotation);
		this.rightHand.syncPos = this.SanitizeVector3(inputStruct.rightHandPosition);
		this.rightHand.syncRotation = this.SanitizeQuaternion(inputStruct.rightHandRotation);
		this.leftHand.syncPos = this.SanitizeVector3(inputStruct.leftHandPosition);
		this.leftHand.syncRotation = this.SanitizeQuaternion(inputStruct.leftHandRotation);
		this.syncPos = this.SanitizeVector3(inputStruct.position);
		this.syncRotation.eulerAngles = this.SanitizeVector3(new Vector3(0f, (float)inputStruct.roundedRotation, 0f));
		this.handSync = inputStruct.handPosition;
		this.currentState = inputStruct.state;
		this.remoteUseReplacementVoice = inputStruct.remoteUseReplacementVoice;
		this.speakingLoudness = inputStruct.speakingLoudness;
		this.UpdateReplacementVoice();
		this.lastPosition = this.syncPos;
		this.grabbedRopeIndex = inputStruct.grabbedRopeIndex;
		if (this.grabbedRopeIndex > 0)
		{
			this.grabbedRopeBoneIndex = inputStruct.ropeBoneIndex;
			this.grabbedRopeIsLeft = inputStruct.ropeGrabIsLeft;
			this.grabbedRopeOffset = this.SanitizeVector3(inputStruct.ropeGrabOffset);
		}
		this.UpdateRopeData();
		this.AddVelocityToQueue(this.syncPos, inputStruct.serverTimeStamp);
	}

	private void UpdateRopeData()
	{
		if (this.previousGrabbedRope == this.grabbedRopeIndex && this.previousGrabbedRopeBoneIndex == this.grabbedRopeBoneIndex && this.previousGrabbedRopeWasLeft == this.grabbedRopeIsLeft)
		{
			return;
		}
		this.ClearRopeData();
		if (this.grabbedRopeIndex > 0)
		{
			PhotonView photonView = PhotonView.Find(this.grabbedRopeIndex);
			GorillaRopeSwing gorillaRopeSwing;
			GorillaClimbable gorillaClimbable;
			if (photonView && photonView.TryGetComponent<GorillaRopeSwing>(out gorillaRopeSwing))
			{
				if (this.currentRopeSwingTarget == null || this.currentRopeSwingTarget.gameObject == null)
				{
					this.currentRopeSwingTarget = new GameObject("RopeSwingTarget").transform;
				}
				if (gorillaRopeSwing.AttachRemotePlayer(this.creator.ActorNumber, this.grabbedRopeBoneIndex, this.currentRopeSwingTarget, this.grabbedRopeOffset))
				{
					this.currentRopeSwing = gorillaRopeSwing;
				}
				this.lastRopeGrabTimer = 0f;
			}
			else if (photonView && photonView.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
			{
				this.currentHoldParent = photonView.transform;
			}
		}
		this.shouldLerpToRope = true;
		this.previousGrabbedRope = this.grabbedRopeIndex;
		this.previousGrabbedRopeBoneIndex = this.grabbedRopeBoneIndex;
		this.previousGrabbedRopeWasLeft = this.grabbedRopeIsLeft;
	}

	public static void AttachLocalPlayerToPhotonView(PhotonView view, XRNode xrNode, Vector3 offset, Vector3 velocity)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = view.ViewID;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
		}
	}

	public static void DetachLocalPlayerFromPhotonView()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
		}
	}

	private void ClearRopeData()
	{
		if (this.currentRopeSwing)
		{
			this.currentRopeSwing.DetachRemotePlayer(this.creator.ActorNumber);
		}
		if (this.currentRopeSwingTarget)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		this.currentRopeSwing = null;
		this.currentHoldParent = null;
	}

	public void ChangeMaterial(int materialIndex, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			this.ChangeMaterialLocal(materialIndex);
		}
	}

	public void ChangeMaterialLocal(int materialIndex)
	{
		Debug.Log("ChangeMatLocal");
		this.setMatIndex = materialIndex;
		if (this.setMatIndex > -1 && this.setMatIndex < this.materialsToChangeTo.Length)
		{
			this.mainSkin.material = this.materialsToChangeTo[this.setMatIndex];
		}
		if (this.lavaParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 2 && this.lavaParticleSystem.isStopped)
			{
				this.lavaParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.lavaParticleSystem.isPlaying)
			{
				this.lavaParticleSystem.Stop();
			}
		}
		if (this.rockParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 1 && this.rockParticleSystem.isStopped)
			{
				this.rockParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.rockParticleSystem.isPlaying)
			{
				this.rockParticleSystem.Stop();
			}
		}
		if (this.iceParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 3 && this.rockParticleSystem.isStopped)
			{
				this.iceParticleSystem.Play();
				return;
			}
			if (!this.isOfflineVRRig && this.iceParticleSystem.isPlaying)
			{
				this.iceParticleSystem.Stop();
			}
		}
	}

	public void InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "InitializeNoobMaterial");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		Debug.Log("InitNoobMat senderID from info is " + info.senderID.ToString() + ". My ID is " + NetworkSystem.Instance.LocalPlayerID.ToString());
		Debug.Log("Rig ID = " + NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject).ToString());
		string userID = NetworkSystem.Instance.GetUserID(info.senderID);
		Debug.Log(info.senderID == NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject));
		Debug.Log(!this.initialized);
		Debug.Log(GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(userID));
		if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject) && (!this.initialized || (this.initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(userID))))
		{
			this.initialized = true;
			red = Mathf.Clamp(red, 0f, 1f);
			green = Mathf.Clamp(green, 0f, 1f);
			blue = Mathf.Clamp(blue, 0f, 1f);
			Debug.Log(string.Concat(new string[]
			{
				"Setting colour values to: red - ",
				red.ToString(),
				", green - ",
				green.ToString(),
				" blue - ",
				blue.ToString()
			}));
			this.InitializeNoobMaterialLocal(red, green, blue);
			return;
		}
		Debug.Log("inappropriate tag data being sent init noob");
		GorillaNot.instance.SendReport("inappropriate tag data being sent init noob", player.UserId, player.NickName);
	}

	public void InitializeNoobMaterialLocal(float red, float green, float blue)
	{
		Color color = new Color(red, green, blue);
		this.EnsureInstantiatedMaterial();
		if (this.myDefaultSkinMaterialInstance != null)
		{
			color.r = Mathf.Clamp(color.r, 0.16f, 1f);
			color.g = Mathf.Clamp(color.g, 0.16f, 1f);
			color.b = Mathf.Clamp(color.b, 0.16f, 1f);
			this.myDefaultSkinMaterialInstance.color = color;
		}
		if (this.rigSerializer != null)
		{
			string nickName = this.OwningNetPlayer.NickName;
			this.playerText.text = this.NormalizeName(true, nickName);
		}
		else if (this.showName)
		{
			this.playerText.text = NetworkSystem.Instance.GetMyNickName();
		}
		Debug.Log("Set Color");
		this.SetColor(color);
	}

	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
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
	public void RequestMaterialColor(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		Debug.Log("Request Mat Color from rig");
		this.IncrementRPC(info, "RequestMaterialColor");
		Photon.Realtime.Player playerRef = ((PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.senderID)).playerRef;
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("InitializeNoobMaterial", playerRef, new object[]
			{
				this.myDefaultSkinMaterialInstance.color.r,
				this.myDefaultSkinMaterialInstance.color.g,
				this.myDefaultSkinMaterialInstance.color.b
			});
		}
	}

	public void RequestCosmetics(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "RequestCosmetics");
		if (this.photonView.IsMine && CosmeticsController.instance != null)
		{
			string[] array = CosmeticsController.instance.currentWornSet.ToDisplayNameArray();
			string[] array2 = CosmeticsController.instance.tryOnSet.ToDisplayNameArray();
			this.photonView.RPC("UpdateCosmeticsWithTryon", info.Sender, new object[] { array, array2 });
		}
	}

	public void PlayTagSoundLocal(int soundIndex, float soundVolume)
	{
		if (soundIndex < 0 || soundIndex >= this.clipToPlay.Length)
		{
			return;
		}
		this.tagSound.volume = Mathf.Min(0.25f, soundVolume);
		this.tagSound.PlayOneShot(this.clipToPlay[soundIndex]);
	}

	public void Bonk(int soundIndex, float bonkPercent, PhotonMessageInfo info)
	{
		if (info.Sender == this.photonView.Owner)
		{
			if (this.bonkTime + this.bonkCooldown < Time.time)
			{
				this.bonkTime = Time.time;
				this.tagSound.volume = bonkPercent * 0.25f;
				this.tagSound.PlayOneShot(this.clipToPlay[soundIndex]);
				if (this.photonView.IsMine)
				{
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
					return;
				}
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent bonk", info.Sender.UserId, info.Sender.NickName);
		}
	}

	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlayDrum");
		this.senderRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
		if (this.senderRig == null || this.senderRig.muted)
		{
			return;
		}
		if (drumIndex < 0 || drumIndex >= this.musicDrums.Length || (this.senderRig.transform.position - base.transform.position).sqrMagnitude > 9f || !float.IsFinite(drumVolume))
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent drum", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		AudioSource audioSource = (this.photonView.IsMine ? GorillaTagger.Instance.offlineVRRig.musicDrums[drumIndex] : this.musicDrums[drumIndex]);
		if (!audioSource.gameObject.activeSelf)
		{
			return;
		}
		float instrumentVolume = GorillaComputer.instance.instrumentVolume;
		audioSource.time = 0f;
		audioSource.volume = Mathf.Max(Mathf.Min(instrumentVolume, drumVolume * instrumentVolume), 0f);
		audioSource.Play();
	}

	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlaySelfOnlyInstrument");
		if (info.Sender == this.photonView.Owner && !this.muted)
		{
			if (selfOnlyIndex >= 0 && selfOnlyIndex < this.instrumentSelfOnly.Length && info.Sender == this.photonView.Owner && float.IsFinite(instrumentVol))
			{
				if (this.instrumentSelfOnly[selfOnlyIndex].gameObject.activeSelf)
				{
					this.instrumentSelfOnly[selfOnlyIndex].PlayNote(noteIndex, Mathf.Max(Mathf.Min(GorillaComputer.instance.instrumentVolume, instrumentVol * GorillaComputer.instance.instrumentVolume), 0f) / 2f);
					return;
				}
			}
			else
			{
				GorillaNot.instance.SendReport("inappropriate tag data being sent self only instrument", info.Sender.UserId, info.Sender.NickName);
			}
		}
	}

	public void PlayHandTapLocal(int soundIndex, bool isLeftHand, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < GorillaLocomotion.Player.Instance.materialData.Count)
		{
			if (isLeftHand)
			{
				this.leftHandPlayer.volume = tapVolume;
				this.leftHandPlayer.clip = (GorillaLocomotion.Player.Instance.materialData[soundIndex].overrideAudio ? GorillaLocomotion.Player.Instance.materialData[soundIndex].audio : GorillaLocomotion.Player.Instance.materialData[0].audio);
				this.leftHandPlayer.PlayOneShot(this.leftHandPlayer.clip);
				return;
			}
			this.rightHandPlayer.volume = tapVolume;
			this.rightHandPlayer.clip = (GorillaLocomotion.Player.Instance.materialData[soundIndex].overrideAudio ? GorillaLocomotion.Player.Instance.materialData[soundIndex].audio : GorillaLocomotion.Player.Instance.materialData[0].audio);
			this.rightHandPlayer.PlayOneShot(this.rightHandPlayer.clip);
		}
	}

	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlaySplashEffect");
		if (info.Sender == this.photonView.Owner && (splashPosition).IsValid() && (splashRotation).IsValid() && float.IsFinite(splashScale) && float.IsFinite(boundingRadius))
		{
			if ((base.transform.position - splashPosition).sqrMagnitude < 9f)
			{
				float time = Time.time;
				int num = -1;
				float num2 = time + 10f;
				for (int i = 0; i < this.splashEffectTimes.Length; i++)
				{
					if (this.splashEffectTimes[i] < num2)
					{
						num2 = this.splashEffectTimes[i];
						num = i;
					}
				}
				if (time - 0.5f > num2)
				{
					this.splashEffectTimes[num] = time;
					boundingRadius = Mathf.Clamp(boundingRadius, 0.0001f, 0.5f);
					ObjectPools.instance.Instantiate(GorillaLocomotion.Player.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GorillaLocomotion.Player.Instance.waterParams.rippleEffectScale * boundingRadius * 2f);
					splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
					ObjectPools.instance.Instantiate(GorillaLocomotion.Player.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale, null);
					return;
				}
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent splash effect", info.Sender.UserId, info.Sender.NickName);
		}
	}

	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "EnableNonCosmeticHandItem");
		if (info.Sender == this.photonView.Owner)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", info.Sender.UserId, info.Sender.NickName);
		}
	}

	public void PlayGeodeEffect(Vector3 hitPosition)
	{
		if ((base.transform.position - hitPosition).sqrMagnitude < 9f && this.geodeCrackingSound)
		{
			this.geodeCrackingSound.Play();
		}
	}

	public void PlayClimbSound(AudioClip clip, bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.leftHandPlayer.volume = 0.1f;
			this.leftHandPlayer.clip = clip;
			this.leftHandPlayer.PlayOneShot(this.leftHandPlayer.clip);
			return;
		}
		this.rightHandPlayer.volume = 0.1f;
		this.rightHandPlayer.clip = clip;
		this.rightHandPlayer.PlayOneShot(this.rightHandPlayer.clip);
	}

	public void UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "UpdateCosmetics");
		if (info.Sender == this.photonView.Owner)
		{
			CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			this.LocalUpdateCosmetics(cosmeticSet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
	}

	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "UpdateCosmeticsWithTryon");
		if (info.Sender == this.photonView.Owner)
		{
			CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet cosmeticSet2 = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(cosmeticSet, cosmeticSet2);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", info.Sender.UserId, info.Sender.NickName);
	}

	public void LocalUpdateCosmetics(CosmeticsController.CosmeticSet newSet)
	{
		this.cosmeticSet = newSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive();
		}
	}

	public void LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet newSet, CosmeticsController.CosmeticSet newTryOnSet)
	{
		this.cosmeticSet = newSet;
		this.tryOnSet = newTryOnSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive();
		}
	}

	private void CheckForEarlyAccess()
	{
		if (this.concatStringOfCosmeticsAllowed.Contains("Early Access Supporter Pack"))
		{
			this.concatStringOfCosmeticsAllowed += "LBAAE.LFAAM.LFAAN.LHAAA.LHAAK.LHAAL.LHAAM.LHAAN.LHAAO.LHAAP.LHABA.LHABB.";
		}
		this.initializedCosmetics = true;
	}

	public void SetCosmeticsActive()
	{
		if (CosmeticsController.instance == null)
		{
			return;
		}
		this.prevSet.CopyItems(this.mergedSet);
		this.mergedSet.MergeSets(this.inTryOnRoom ? this.tryOnSet : null, this.cosmeticSet);
		BodyDockPositions component = base.GetComponent<BodyDockPositions>();
		this.mergedSet.ActivateCosmetics(this.prevSet, this, component, CosmeticsController.instance.nullItem, this.cosmeticsObjectRegistry);
	}

	public void GetUserCosmeticsAllowed()
	{
		if (CosmeticsController.instance != null)
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				foreach (ItemInstance itemInstance in result.Inventory)
				{
					if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog)
					{
						this.concatStringOfCosmeticsAllowed += itemInstance.ItemId;
					}
				}
				Debug.Log("successful result. allowed cosmetics are: " + this.concatStringOfCosmeticsAllowed);
				this.CheckForEarlyAccess();
				this.SetCosmeticsActive();
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				this.initializedCosmetics = true;
				this.SetCosmeticsActive();
			}, null, null);
		}
		this.concatStringOfCosmeticsAllowed += "Slingshot";
	}

	public void GenerateFingerAngleLookupTables()
	{
		this.GenerateTableIndex(ref this.leftIndex);
		this.GenerateTableIndex(ref this.rightIndex);
		this.GenerateTableMiddle(ref this.leftMiddle);
		this.GenerateTableMiddle(ref this.rightMiddle);
		this.GenerateTableThumb(ref this.leftThumb);
		this.GenerateTableThumb(ref this.rightThumb);
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

	private void IncrementRPC(PhotonMessageInfoWrapped info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	private void IncrementRPC(PhotonMessageInfo info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	private void AddVelocityToQueue(Vector3 position, double serverTime)
	{
		Vector3 vector;
		if (this.velocityHistoryList.Count == 0)
		{
			vector = Vector3.zero;
			this.lastPosition = position;
		}
		else
		{
			vector = (position - this.lastPosition) / (float)(serverTime - this.velocityHistoryList[0].time);
		}
		this.velocityHistoryList.Insert(0, new VRRig.VelocityTime(vector, serverTime));
		if (this.velocityHistoryList.Count > this.velocityHistoryMaxLength)
		{
			this.velocityHistoryList.RemoveRange(this.velocityHistoryMaxLength, this.velocityHistoryList.Count - this.velocityHistoryMaxLength);
		}
	}

	private Vector3 ReturnVelocityAtTime(double timeToReturn)
	{
		if (this.velocityHistoryList.Count <= 1)
		{
			return Vector3.zero;
		}
		int num = 0;
		int num2 = this.velocityHistoryList.Count - 1;
		int num3 = 0;
		if (num2 == num)
		{
			return this.velocityHistoryList[num].vel;
		}
		while (num2 - num > 1 && num3 < 1000)
		{
			num3++;
			int num4 = (num2 - num) / 2;
			if (this.velocityHistoryList[num4].time > timeToReturn)
			{
				num2 = num4;
			}
			else
			{
				num = num4;
			}
		}
		float num5 = (float)(this.velocityHistoryList[num].time - timeToReturn);
		double num6 = this.velocityHistoryList[num].time - this.velocityHistoryList[num2].time;
		if (num6 == 0.0)
		{
			num6 = 0.001;
		}
		num5 /= (float)num6;
		num5 = Mathf.Clamp(num5, 0f, 1f);
		return Vector3.Lerp(this.velocityHistoryList[num].vel, this.velocityHistoryList[num2].vel, num5);
	}

	public bool CheckDistance(Vector3 position, float max)
	{
		max = max * max * this.scaleFactor;
		return Vector3.SqrMagnitude(this.syncPos - position) < max;
	}

	public void SetColor(Color color)
	{
		Action<Color> action = this.onColorInitialized;
		if (action != null)
		{
			action(color);
		}
		this.onColorInitialized = delegate(Color color1)
		{
		};
		this.colorInitialized = true;
		this.playerColor = color;
	}

	public void OnColorInitialized(Action<Color> action)
	{
		if (this.colorInitialized)
		{
			action(this.playerColor);
			return;
		}
		this.onColorInitialized = (Action<Color>)Delegate.Combine(this.onColorInitialized, action);
	}

	private void OnEnable()
	{
		if (this.currentRopeSwingTarget != null)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		if (!this.isOfflineVRRig)
		{
			PlayerCosmeticsSystem.RegisterCosmeticCallback(this.creator.ActorNumber, this);
		}
	}

	void IPreDisable.PreDisable()
	{
		this.ClearRopeData();
		if (this.currentRopeSwingTarget)
		{
			this.currentRopeSwingTarget.SetParent(base.transform);
		}
		this.EnableHuntWatch(false);
		this.EnableBattleCosmetics(false);
		this.concatStringOfCosmeticsAllowed = "";
		this.rawCosmeticString = "";
		if (this.cosmeticSet != null)
		{
			this.mergedSet.DeactivateAllCosmetcs(this.myBodyDockPositions, CosmeticsController.instance.nullItem, this.cosmeticsObjectRegistry);
			this.mergedSet.ClearSet(CosmeticsController.instance.nullItem);
			this.prevSet.ClearSet(CosmeticsController.instance.nullItem);
			this.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			this.cosmeticSet.ClearSet(CosmeticsController.instance.nullItem);
		}
		if (!this.isOfflineVRRig)
		{
			PlayerCosmeticsSystem.RemoveCosmeticCallback(this.creator.ActorNumber);
			this.pendingCosmeticUpdate = true;
		}
	}

	private void OnDisable()
	{
		Debug.Log("ON DISABLE!");
		this.initialized = false;
		this.muted = false;
		this.photonView = null;
		this.voiceAudio = null;
		this.tempRig = null;
		this.timeSpawned = 0f;
		this.initializedCosmetics = false;
		this.velocityHistoryList.Clear();
		this.tempMatIndex = 0;
		this.setMatIndex = 0;
		this.ChangeMaterialLocal(this.setMatIndex);
		this.currentCosmeticTries = 0;
		this.creator = null;
		Debug.Log("ON DISABLE! Finished cleanup" + (this.photonView == null).ToString());
		try
		{
			CallLimitType<CallLimiter>[] callSettings = this.fxSettings.callSettings;
			for (int i = 0; i < callSettings.Length; i++)
			{
				callSettings[i].CallLimitSettings.Reset();
			}
		}
		catch
		{
			Debug.LogError("fxtype missing in fxSettings, please fix or remove this");
		}
	}

	public void NetInitialize()
	{
		this.timeSpawned = Time.time;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null)
			{
				if (instance is GorillaHuntManager || instance.GameModeName() == "HUNT")
				{
					this.EnableHuntWatch(true);
				}
				else if (instance is GorillaBattleManager || instance.GameModeName() == "BATTLE")
				{
					this.EnableBattleCosmetics(true);
				}
			}
			else
			{
				string gameModeString = NetworkSystem.Instance.GameModeString;
				if (!gameModeString.IsNullOrEmpty())
				{
					string text = gameModeString;
					if (text.Contains("HUNT"))
					{
						this.EnableHuntWatch(true);
					}
					else if (text.Contains("BATTLE"))
					{
						this.EnableBattleCosmetics(true);
					}
				}
			}
		}
		if (this.photonView != null)
		{
			base.transform.position = this.photonView.gameObject.transform.position;
			base.transform.rotation = this.photonView.gameObject.transform.rotation;
		}
		try
		{
			Action action = VRRig.newPlayerJoined;
			if (action != null)
			{
				action();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	public void EnableHuntWatch(bool on)
	{
		this.huntComputer.SetActive(on);
	}

	public void EnableBattleCosmetics(bool on)
	{
		this.battleBalloons.gameObject.SetActive(on);
	}

	private void UpdateReplacementVoice()
	{
		if (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn != "TRUE")
		{
			this.voiceAudio.mute = true;
			return;
		}
		this.voiceAudio.mute = false;
	}

	public bool ShouldPlayReplacementVoice()
	{
		return this.photonView && !this.photonView.IsMine && !(GorillaComputer.instance.voiceChatOn == "OFF") && (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && this.speakingLoudness > this.replacementVoiceLoudnessThreshold;
	}

	bool IUserCosmeticsCallback.PendingUpdate
	{
		get
		{
			return this.pendingCosmeticUpdate;
		}
		set
		{
			this.pendingCosmeticUpdate = value;
		}
	}

	bool IUserCosmeticsCallback.OnGetUserCosmetics(string cosmetics)
	{
		if (cosmetics == this.rawCosmeticString && this.currentCosmeticTries < this.cosmeticRetries)
		{
			this.currentCosmeticTries++;
			return false;
		}
		this.rawCosmeticString = cosmetics ?? "";
		this.concatStringOfCosmeticsAllowed = this.rawCosmeticString;
		this.initializedCosmetics = true;
		this.currentCosmeticTries = 0;
		this.CheckForEarlyAccess();
		this.SetCosmeticsActive();
		this.myBodyDockPositions.RefreshTransferrableItems();
		PhotonView photonView = this.photonView;
		if (photonView != null)
		{
			photonView.RPC("RequestCosmetics", this.photonView.Owner, Array.Empty<object>());
		}
		return true;
	}

	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	void IGuidedRefObject.GuidedRefInitialize()
	{
		GuidedRefHub.RegisterTarget<VRRig>(this, this.guidedRefTargetInfo.hubIds, this);
		GuidedRefHub.ReceiverFullyRegistered<VRRig>(this);
	}

	GuidedRefBasicTargetInfo IGuidedRefTargetMono.GRefTargetInfo
	{
		get
		{
			return this.guidedRefTargetInfo;
		}
		set
		{
			this.guidedRefTargetInfo = value;
		}
	}

	Object IGuidedRefTargetMono.GuidedRefTargetObject
	{
		get
		{
			return this;
		}
	}

	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		return false;
	}

	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
	}

	public void OnGuidedRefTargetDestroyed(int fieldId)
	{
	}

	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	public static Action newPlayerJoined;

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

	private Transform currentHoldParent;

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

	public GorillaSkin defaultSkin;

	public Material myDefaultSkinMaterialInstance;

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

	private int cosmeticRetries = 2;

	private int currentCosmeticTries;

	public SizeManager sizeManager;

	public float pitchScale = 0.3f;

	public float pitchOffset = 1f;

	[NonSerialized]
	public bool IsHaunted;

	public float HauntedVoicePitch = 0.5f;

	public float HauntedHearingVolume = 0.15f;

	[NonSerialized]
	public bool UsingHauntedRing;

	[NonSerialized]
	public float HauntedRingVoicePitch;

	public NonCosmeticHandItem nonCosmeticLeftHandItem;

	public NonCosmeticHandItem nonCosmeticRightHandItem;

	public VRRigReliableState reliableState;

	internal RigContainer rigContainer;

	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[]
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

	internal NetPlayer creatorWrapped;

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

	private VRRig tempVRRig;

	public GameObject huntComputer;

	public Slingshot slingshot;

	public Slingshot.SlingshotState slingshotState;

	private PhotonVoiceView myPhotonVoiceView;

	private VRRig senderRig;

	public TransferrableObject.PositionState currentState;

	private bool isInitialized;

	private List<VRRig.VelocityTime> velocityHistoryList = new List<VRRig.VelocityTime>();

	public int velocityHistoryMaxLength = 200;

	private Vector3 lastPosition;

	public const int splashLimitCount = 4;

	public const float splashLimitCooldown = 0.5f;

	private float[] splashEffectTimes = new float[4];

	internal AudioSource voiceAudio;

	public bool remoteUseReplacementVoice;

	public bool localUseReplacementVoice;

	private MicWrapper currentMicWrapper;

	private IAudioDesc audioDesc;

	private float speakingLoudness;

	public bool shouldSendSpeakingLoudness = true;

	public float replacementVoiceLoudnessThreshold = 0.05f;

	public int replacementVoiceDetectionDelay = 128;

	[SerializeField]
	internal PhotonView photonView;

	[SerializeField]
	internal VRRigSerializer rigSerializer;

	public NetPlayer OwningNetPlayer;

	[SerializeField]
	private FXSystemSettings sharedFXSettings;

	[NonSerialized]
	public FXSystemSettings fxSettings;

	private bool playerWasHaunted;

	private float nonHauntedVolume;

	private int count;

	public Color playerColor;

	public bool colorInitialized;

	private Action<Color> onColorInitialized;

	private bool pendingCosmeticUpdate = true;

	private string rawCosmeticString = "";

	[SerializeField]
	private GuidedRefBasicTargetInfo guidedRefTargetInfo;

	public enum WearablePackedStateSlots
	{
		Hat,
		LeftHand,
		RightHand
	}

	public struct VelocityTime
	{
		public VelocityTime(Vector3 velocity, double velTime)
		{
			this.vel = velocity;
			this.time = velTime;
		}

		public Vector3 vel;

		public double time;
	}
}
