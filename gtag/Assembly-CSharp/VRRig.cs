using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
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
	public bool HasBracelet
	{
		get
		{
			return this.reliableState.HasBracelet;
		}
	}

	public Vector3 GetMouthPosition()
	{
		return this.MouthPosition.position;
	}

	public VRRig.PartyMemberStatus GetPartyMemberStatus()
	{
		if (this.partyMemberStatus == VRRig.PartyMemberStatus.NeedsUpdate)
		{
			this.partyMemberStatus = (FriendshipGroupDetection.Instance.IsInMyGroup(this.creator.UserId) ? VRRig.PartyMemberStatus.InLocalParty : VRRig.PartyMemberStatus.NotInLocalParty);
		}
		return this.partyMemberStatus;
	}

	public bool IsLocalPartyMember
	{
		get
		{
			return this.GetPartyMemberStatus() != VRRig.PartyMemberStatus.NotInLocalParty;
		}
	}

	public void ClearPartyMemberStatus()
	{
		this.partyMemberStatus = VRRig.PartyMemberStatus.NeedsUpdate;
	}

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

	public Color32 LeftThrowableProjectileColor
	{
		get
		{
			return this.reliableState.lThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.lThrowableProjectileColor.Equals(value))
			{
				this.reliableState.lThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public Color32 RightThrowableProjectileColor
	{
		get
		{
			return this.reliableState.rThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.rThrowableProjectileColor.Equals(value))
			{
				this.reliableState.rThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	public Color32 GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightThrowableProjectileColor;
		}
		return this.LeftThrowableProjectileColor;
	}

	public void SetThrowableProjectileColor(bool isLeftHand, Color32 color)
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

	public void BuildInitialize()
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
		if (!this.isOfflineVRRig)
		{
			base.transform.parent = null;
		}
		SizeManager component = base.GetComponent<SizeManager>();
		if (component != null)
		{
			component.BuildInitialize();
		}
		this.myMouthFlap = base.GetComponent<GorillaMouthFlap>();
		this.mySpeakerLoudness = base.GetComponent<GorillaSpeakerLoudness>();
		if (this.myReplacementVoice == null)
		{
			this.myReplacementVoice = base.GetComponentInChildren<ReplacementVoice>();
		}
		this.myEyeExpressions = base.GetComponent<GorillaEyeExpressions>();
	}

	private void Awake()
	{
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.UpdateName));
		if (this.isOfflineVRRig)
		{
			this.BuildInitialize();
		}
		this.SharedStart();
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
		float defaultValue = 0f;
		float @float = PlayerPrefs.GetFloat("redValue", defaultValue);
		float float2 = PlayerPrefs.GetFloat("greenValue", defaultValue);
		float float3 = PlayerPrefs.GetFloat("blueValue", defaultValue);
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
		if (!Application.isBatchMode)
		{
			this.playerText.transform.parent.GetComponent<Canvas>().worldCamera = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		}
		this.EnsureInstantiatedMaterial();
		this.initialized = false;
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
			this.initialized = true;
		}
		else if (!this.isOfflineVRRig)
		{
			if (this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.head.syncPos = -this.headBodyOffset;
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

	public void RemoteRigUpdate()
	{
		if (this.scaleFactor != this.lastScaleFactor)
		{
			base.transform.localScale = Vector3.one * this.scaleFactor;
		}
		this.lastScaleFactor = this.scaleFactor;
		if (this.voiceAudio != null)
		{
			float num = (GorillaTagger.Instance.offlineVRRig.scaleFactor - this.scaleFactor) / this.pitchScale + this.pitchOffset;
			float num2 = this.UsingHauntedRing ? this.HauntedRingVoicePitch : num;
			num2 = (this.IsHaunted ? this.HauntedVoicePitch : num2);
			if (!Mathf.Approximately(this.voiceAudio.pitch, num2))
			{
				this.voiceAudio.pitch = num2;
			}
		}
		this.jobPos = base.transform.position;
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobPos = Vector3.Lerp(base.transform.position, this.SanitizeVector3(this.syncPos), this.lerpValueBody * 0.66f);
			if (this.currentRopeSwing && this.currentRopeSwingTarget)
			{
				Vector3 b;
				if (this.grabbedRopeIsLeft)
				{
					b = this.currentRopeSwingTarget.position - this.leftHandTransform.position;
				}
				else
				{
					b = this.currentRopeSwingTarget.position - this.rightHandTransform.position;
				}
				if (this.shouldLerpToRope)
				{
					this.jobPos += Vector3.Lerp(Vector3.zero, b, this.lastRopeGrabTimer * 4f);
					if (this.lastRopeGrabTimer < 1f)
					{
						this.lastRopeGrabTimer += Time.deltaTime;
					}
				}
				else
				{
					this.jobPos += b;
				}
			}
			else if (this.currentHoldParent != null)
			{
				this.jobPos += this.currentHoldParent.TransformPoint(this.grabbedRopeOffset) - (this.grabbedRopeIsLeft ? this.leftHandTransform : this.rightHandTransform).position;
			}
		}
		else
		{
			this.jobPos = this.SanitizeVector3(this.syncPos);
		}
		this.jobRotation = Quaternion.Lerp(base.transform.rotation, this.SanitizeQuaternion(this.syncRotation), this.lerpValueBody);
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

	private void LateUpdate()
	{
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
						this.currentMicWrapper = (this.audioDesc as MicWrapper);
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
		if (this.creator != null)
		{
			ScienceExperimentManager instance = ScienceExperimentManager.instance;
			int num4;
			if (instance != null && instance.GetMaterialIfPlayerInGame(this.creator.ActorNumber, out num4))
			{
				this.tempMatIndex = num4;
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
		GorillaMouthFlap gorillaMouthFlap = this.myMouthFlap;
		if (gorillaMouthFlap != null)
		{
			gorillaMouthFlap.InvokeUpdate();
		}
		GorillaSpeakerLoudness gorillaSpeakerLoudness = this.mySpeakerLoudness;
		if (gorillaSpeakerLoudness != null)
		{
			gorillaSpeakerLoudness.InvokeUpdate();
		}
		ReplacementVoice replacementVoice = this.myReplacementVoice;
		if (replacementVoice != null)
		{
			replacementVoice.InvokeUpdate();
		}
		GorillaEyeExpressions gorillaEyeExpressions = this.myEyeExpressions;
		if (gorillaEyeExpressions == null)
		{
			return;
		}
		gorillaEyeExpressions.InvokeUpdate();
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
		inputStruct.remoteUseReplacementVoice = this.remoteUseReplacementVoice;
		inputStruct.speakingLoudness = this.speakingLoudness;
		inputStruct.grabbedRopeIndex = this.grabbedRopeIndex;
		if (this.grabbedRopeIndex > 0)
		{
			inputStruct.ropeBoneIndex = this.grabbedRopeBoneIndex;
			inputStruct.ropeGrabIsLeft = this.grabbedRopeIsLeft;
			inputStruct.ropeGrabOffset = this.grabbedRopeOffset;
		}
		double serverTimeStamp = NetworkSystem.Instance.SimTick / 1000.0;
		inputStruct.serverTimeStamp = serverTimeStamp;
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

	public static int PackQuaternionForNetwork(Quaternion q)
	{
		q.Normalize();
		float num = Mathf.Abs(q.x);
		float num2 = Mathf.Abs(q.y);
		float num3 = Mathf.Abs(q.z);
		float num4 = Mathf.Abs(q.w);
		float num5 = num;
		VRRig.QAxis qaxis = VRRig.QAxis.X;
		if (num2 > num5)
		{
			num5 = num2;
			qaxis = VRRig.QAxis.Y;
		}
		if (num3 > num5)
		{
			num5 = num3;
			qaxis = VRRig.QAxis.Z;
		}
		if (num4 > num5)
		{
			qaxis = VRRig.QAxis.W;
		}
		bool flag;
		float num6;
		float num7;
		float num8;
		switch (qaxis)
		{
		case VRRig.QAxis.X:
			flag = (q.x < 0f);
			num6 = q.y;
			num7 = q.z;
			num8 = q.w;
			goto IL_11A;
		case VRRig.QAxis.Y:
			flag = (q.y < 0f);
			num6 = q.x;
			num7 = q.z;
			num8 = q.w;
			goto IL_11A;
		case VRRig.QAxis.Z:
			flag = (q.z < 0f);
			num6 = q.x;
			num7 = q.y;
			num8 = q.w;
			goto IL_11A;
		}
		flag = (q.w < 0f);
		num6 = q.x;
		num7 = q.y;
		num8 = q.z;
		IL_11A:
		if (flag)
		{
			num6 = -num6;
			num7 = -num7;
			num8 = -num8;
		}
		int num9 = Mathf.Clamp(Mathf.RoundToInt((num6 + 0.707107f) * 361.33145f), 0, 511);
		int num10 = Mathf.Clamp(Mathf.RoundToInt((num7 + 0.707107f) * 361.33145f), 0, 511);
		int num11 = Mathf.Clamp(Mathf.RoundToInt((num8 + 0.707107f) * 361.33145f), 0, 511);
		return (int)(num9 + (num10 << 9) + (num11 << 18) + ((int)qaxis << 27));
	}

	public static Quaternion UnpackQuaterionFromNetwork(int data)
	{
		float num = (float)(data & 511) * 0.0027675421f - 0.707107f;
		float num2 = (float)(data >> 9 & 511) * 0.0027675421f - 0.707107f;
		float num3 = (float)(data >> 18 & 511) * 0.0027675421f - 0.707107f;
		float num4 = Mathf.Sqrt(1f - (num * num + num2 * num2 + num3 * num3));
		switch (data >> 27 & 3)
		{
		case 0:
			return new Quaternion(num4, num, num2, num3);
		case 1:
			return new Quaternion(num, num4, num2, num3);
		case 2:
			return new Quaternion(num, num2, num4, num3);
		}
		return new Quaternion(num, num2, num3, num4);
	}

	public static long PackHandPosRotForNetwork(Vector3 localPos, Quaternion rot)
	{
		long num = (long)Mathf.Clamp(Mathf.RoundToInt(localPos.x * 512f) + 1024, 0, 2047);
		long num2 = (long)Mathf.Clamp(Mathf.RoundToInt(localPos.y * 512f) + 1024, 0, 2047);
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(localPos.z * 512f) + 1024, 0, 2047);
		long num4 = (long)VRRig.PackQuaternionForNetwork(rot);
		return num + (num2 << 11) + (num3 << 22) + (num4 << 33);
	}

	public static void UnpackHandPosRotFromNetwork(long data, out Vector3 localPos, out Quaternion handRot)
	{
		long num = data & 2047L;
		long num2 = data >> 11 & 2047L;
		long num3 = data >> 22 & 2047L;
		localPos = new Vector3((float)(num - 1024L) * 0.001953125f, (float)(num2 - 1024L) * 0.001953125f, (float)(num3 - 1024L) * 0.001953125f);
		int data2 = (int)(data >> 33);
		handRot = VRRig.UnpackQuaterionFromNetwork(data2);
	}

	public static long PackWorldPosForNetwork(Vector3 worldPos)
	{
		long num = (long)Mathf.Clamp(Mathf.RoundToInt(worldPos.x * 1024f) + 1048576, 0, 2097151);
		long num2 = (long)Mathf.Clamp(Mathf.RoundToInt(worldPos.y * 1024f) + 1048576, 0, 2097151);
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(worldPos.z * 1024f) + 1048576, 0, 2097151);
		return num + (num2 << 21) + (num3 << 42);
	}

	public static Vector3 UnpackWorldPosFromNetwork(long data)
	{
		float num = (float)(data & 2097151L);
		long num2 = data >> 21 & 2097151L;
		long num3 = data >> 42 & 2097151L;
		return new Vector3((float)((long)num - 1048576L) * 0.0009765625f, (float)(num2 - 1048576L) * 0.0009765625f, (float)(num3 - 1048576L) * 0.0009765625f);
	}

	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(VRRig.PackQuaternionForNetwork(this.head.rigTarget.localRotation));
		stream.SendNext(VRRig.PackHandPosRotForNetwork(this.rightHand.rigTarget.localPosition, this.rightHand.rigTarget.localRotation));
		stream.SendNext(VRRig.PackHandPosRotForNetwork(this.leftHand.rigTarget.localPosition, this.leftHand.rigTarget.localRotation));
		stream.SendNext(VRRig.PackWorldPosForNetwork(base.transform.position));
		stream.SendNext(this.ReturnHandPosition());
		int num = Mathf.Clamp(Mathf.RoundToInt(base.transform.rotation.eulerAngles.y + 360f) % 360, 0, 360);
		int num2 = Mathf.RoundToInt(Mathf.Clamp01(this.speakingLoudness) * 255f);
		int num3 = num + (this.remoteUseReplacementVoice ? 512 : 0) + ((this.grabbedRopeIndex > 0) ? 1024 : 0) + (num2 << 16);
		stream.SendNext(num3);
		if (this.grabbedRopeIndex > 0)
		{
			stream.SendNext(this.grabbedRopeIndex);
			stream.SendNext(this.grabbedRopeBoneIndex);
			stream.SendNext(this.grabbedRopeIsLeft);
			stream.SendNext(this.grabbedRopeOffset);
		}
	}

	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		this.head.syncRotation = VRRig.UnpackQuaterionFromNetwork((int)stream.ReceiveNext());
		VRRig.UnpackHandPosRotFromNetwork((long)stream.ReceiveNext(), out this.rightHand.syncPos, out this.rightHand.syncRotation);
		VRRig.UnpackHandPosRotFromNetwork((long)stream.ReceiveNext(), out this.leftHand.syncPos, out this.leftHand.syncRotation);
		this.syncPos = VRRig.UnpackWorldPosFromNetwork((long)stream.ReceiveNext());
		this.handSync = (int)stream.ReceiveNext();
		this.lastPosition = this.syncPos;
		int num = (int)stream.ReceiveNext();
		int num2 = num & 511;
		this.syncRotation.eulerAngles = this.SanitizeVector3(new Vector3(0f, (float)num2, 0f));
		this.remoteUseReplacementVoice = ((num & 512) != 0);
		int num3 = num >> 16 & 255;
		this.speakingLoudness = (float)num3 / 255f;
		this.UpdateReplacementVoice();
		if ((num & 1024) != 0)
		{
			this.grabbedRopeIndex = (int)stream.ReceiveNext();
			this.grabbedRopeBoneIndex = (int)stream.ReceiveNext();
			this.grabbedRopeIsLeft = (bool)stream.ReceiveNext();
			this.grabbedRopeOffset = this.SanitizeVector3((Vector3)stream.ReceiveNext());
		}
		else
		{
			this.grabbedRopeIndex = 0;
		}
		this.UpdateRopeData();
		this.AddVelocityToQueue(this.syncPos, info.timestamp);
	}

	private void UpdateExtrapolationTarget()
	{
		float num = (float)(PhotonNetwork.Time - this.remoteLatestTimestamp);
		num -= 0.15f;
		num = Mathf.Clamp(num, -0.5f, 0.5f);
		this.syncPos += this.remoteVelocity * num;
		this.remoteCorrectionNeeded = this.syncPos - base.transform.position;
		if (this.remoteCorrectionNeeded.magnitude > 1.5f && this.grabbedRopeIndex <= 0)
		{
			base.transform.position = this.syncPos;
			this.remoteCorrectionNeeded = Vector3.zero;
		}
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
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
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
		NetworkSystem.Instance.GetPlayer(info.senderID);
		string userID = NetworkSystem.Instance.GetUserID(info.senderID);
		if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject) && (!this.initialized || (this.initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(userID))))
		{
			this.initialized = true;
			blue = blue.ClampSafe(0f, 1f);
			red = red.ClampSafe(0f, 1f);
			green = green.ClampSafe(0f, 1f);
			this.InitializeNoobMaterialLocal(red, green, blue);
		}
	}

	public void InitializeNoobMaterialLocal(float red, float green, float blue)
	{
		Color color = new Color(red, green, blue);
		this.EnsureInstantiatedMaterial();
		if (this.myDefaultSkinMaterialInstance != null)
		{
			color.r = Mathf.Clamp(color.r, 0f, 1f);
			color.g = Mathf.Clamp(color.g, 0f, 1f);
			color.b = Mathf.Clamp(color.b, 0f, 1f);
			this.myDefaultSkinMaterialInstance.color = color;
		}
		this.SetColor(color);
		this.UpdateName(PlayFabAuthenticator.instance.GetSafety());
	}

	public void UpdateName(bool isSafety)
	{
		if (this.rigSerializer != null)
		{
			string text = isSafety ? this.OwningNetPlayer.DefaultName : this.OwningNetPlayer.NickName;
			this.playerNameVisible = this.NormalizeName(true, text);
		}
		else if (this.showName && NetworkSystem.Instance != null)
		{
			this.playerNameVisible = (isSafety ? NetworkSystem.Instance.GetMyDefaultName() : NetworkSystem.Instance.GetMyNickName());
		}
		this.playerText.text = this.playerNameVisible;
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
			this.photonView.RPC("UpdateCosmeticsWithTryon", info.Sender, new object[]
			{
				array,
				array2
			});
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
		AudioSource audioSource = this.photonView.IsMine ? GorillaTagger.Instance.offlineVRRig.musicDrums[drumIndex] : this.musicDrums[drumIndex];
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
			GorillaLocomotion.Player.MaterialData materialData = GorillaLocomotion.Player.Instance.materialData[soundIndex];
			AudioSource audioSource = isLeftHand ? this.leftHandPlayer : this.rightHandPlayer;
			audioSource.volume = tapVolume;
			AudioClip clip = materialData.overrideAudio ? materialData.audio : GorillaLocomotion.Player.Instance.materialData[0].audio;
			audioSource.PlayOneShot(clip);
		}
	}

	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlaySplashEffect");
		if (info.Sender == this.photonView.Owner && splashPosition.IsValid() && splashRotation.IsValid() && float.IsFinite(splashScale) && float.IsFinite(boundingRadius))
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

	public bool IsMakingFistLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) > 0.25f;
		}
		return this.leftIndex.calcT > 0.25f && this.leftMiddle.calcT > 0.25f;
	}

	public bool IsMakingFistRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.25f;
		}
		return this.rightIndex.calcT > 0.25f && this.rightMiddle.calcT > 0.25f;
	}

	public VRMap GetMakingFist(bool debug, out bool isLeftHand)
	{
		if (this.IsMakingFistRight())
		{
			isLeftHand = false;
			return this.rightHand;
		}
		if (this.IsMakingFistLeft())
		{
			isLeftHand = true;
			return this.leftHand;
		}
		isLeftHand = false;
		return null;
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
		if (info.Sender == this.photonView.Owner && currentItems.Length <= 16)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			this.LocalUpdateCosmetics(newSet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
	}

	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "UpdateCosmeticsWithTryon");
		if (info.Sender == this.photonView.Owner && currentItems.Length <= 16 && tryOnItems.Length <= 16)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet);
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
				this.CheckForEarlyAccess();
				this.SetCosmeticsActive();
			}, delegate(PlayFabError error)
			{
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
		Vector3 velocity;
		if (this.velocityHistoryList.Count == 0)
		{
			velocity = Vector3.zero;
			this.lastPosition = position;
		}
		else
		{
			velocity = (position - this.lastPosition) / (float)(serverTime - this.velocityHistoryList[0].time);
		}
		this.velocityHistoryList.Insert(0, new VRRig.VelocityTime(velocity, serverTime));
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

	public Vector3 LatestVelocity()
	{
		if (this.velocityHistoryList.Count > 0)
		{
			return this.velocityHistoryList[0].vel;
		}
		return Vector3.zero;
	}

	public bool CheckDistance(Vector3 position, float max)
	{
		max = max * max * this.scaleFactor;
		return Vector3.SqrMagnitude(this.syncPos - position) < max;
	}

	public bool CheckTagDistanceRollback(VRRig otherRig, float max, float timeInterval)
	{
		Vector3 a;
		Vector3 b;
		GorillaMath.LineSegClosestPoints(this.syncPos, -this.LatestVelocity() * timeInterval, otherRig.syncPos, -otherRig.LatestVelocity() * timeInterval, out a, out b);
		return Vector3.SqrMagnitude(a - b) < max * max * this.scaleFactor;
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
		if (!this.isOfflineVRRig)
		{
			VRRigJobManager.Instance.RegisterVRRig(this);
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
		this.ClearPartyMemberStatus();
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
		if (!this.isOfflineVRRig)
		{
			VRRigJobManager.Instance.DeregisterVRRig(this);
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
			this.UpdateFriendshipBracelet();
			if (this.IsLocalPartyMember && !this.isOfflineVRRig)
			{
				FriendshipGroupDetection.Instance.SendVerifyPartyMember(this.creator);
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
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	public void UpdateFriendshipBracelet()
	{
		bool flag = false;
		if (this.isOfflineVRRig)
		{
			bool flag2 = false;
			VRRig.PartyMemberStatus partyMemberStatus = this.GetPartyMemberStatus();
			if (partyMemberStatus != VRRig.PartyMemberStatus.InLocalParty)
			{
				if (partyMemberStatus == VRRig.PartyMemberStatus.NotInLocalParty)
				{
					flag2 = false;
					this.reliableState.isBraceletLeftHanded = false;
				}
			}
			else
			{
				flag2 = true;
				this.reliableState.isBraceletLeftHanded = (FriendshipGroupDetection.Instance.DidJoinLeftHanded && !this.huntComputer.activeSelf);
			}
			if (this.reliableState.HasBracelet != flag2 || this.reliableState.braceletBeadColors.Count != FriendshipGroupDetection.Instance.myBeadColors.Count)
			{
				this.reliableState.SetIsDirty();
				flag = (this.reliableState.HasBracelet == flag2);
			}
			this.reliableState.braceletBeadColors.Clear();
			if (flag2)
			{
				this.reliableState.braceletBeadColors.AddRange(FriendshipGroupDetection.Instance.myBeadColors);
			}
			this.reliableState.braceletSelfIndex = FriendshipGroupDetection.Instance.MyBraceletSelfIndex;
		}
		if (this.nonCosmeticLeftHandItem != null)
		{
			bool flag3 = this.reliableState.HasBracelet && this.reliableState.isBraceletLeftHanded;
			this.nonCosmeticLeftHandItem.EnableItem(flag3);
			if (flag3)
			{
				this.friendshipBraceletLeftHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletLeftHand.PlayAppearEffects();
				}
			}
		}
		if (this.nonCosmeticRightHandItem != null)
		{
			bool flag4 = this.reliableState.HasBracelet && !this.reliableState.isBraceletLeftHanded;
			this.nonCosmeticRightHandItem.EnableItem(flag4);
			if (flag4)
			{
				this.friendshipBraceletRightHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletRightHand.PlayAppearEffects();
				}
			}
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
		this.rawCosmeticString = (cosmetics ?? "");
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
		return false;
	}

	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
	}

	public void OnGuidedRefTargetDestroyed(int fieldId)
	{
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void CacheLocalRig()
	{
		if (VRRig.gLocalRig != null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Local Gorilla Player");
		VRRig.gLocalRig = ((gameObject != null) ? gameObject.GetComponentInChildren<VRRig>() : null);
		VRRig.bCachedLocalRig = true;
	}

	public bool isLocal
	{
		get
		{
			return VRRig.gLocalRig == this;
		}
	}

	public VRRig()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static VRRig()
	{
	}

	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	[CompilerGenerated]
	private void <GetUserCosmeticsAllowed>b__274_0(GetUserInventoryResult result)
	{
		foreach (ItemInstance itemInstance in result.Inventory)
		{
			if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog)
			{
				this.concatStringOfCosmeticsAllowed += itemInstance.ItemId;
			}
		}
		this.CheckForEarlyAccess();
		this.SetCosmeticsActive();
	}

	[CompilerGenerated]
	private void <GetUserCosmeticsAllowed>b__274_1(PlayFabError error)
	{
		this.initializedCosmetics = true;
		this.SetCosmeticsActive();
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

	public Vector3 jobPos;

	public Quaternion syncRotation;

	public Quaternion jobRotation;

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

	public ZoneEntity zoneEntity;

	public Material myDefaultSkinMaterialInstance;

	public Material scoreboardMaterial;

	public GameObject spectatorSkin;

	public int handSync;

	public Material[] materialsToChangeTo;

	public float red;

	public float green;

	public float blue;

	public string playerName;

	public Text playerText;

	public string playerNameVisible;

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

	public FriendshipBracelet friendshipBraceletLeftHand;

	public NonCosmeticHandItem nonCosmeticLeftHandItem;

	public FriendshipBracelet friendshipBraceletRightHand;

	public NonCosmeticHandItem nonCosmeticRightHandItem;

	public VRRigReliableState reliableState;

	[SerializeField]
	private Transform MouthPosition;

	internal RigContainer rigContainer;

	private Vector3 remoteVelocity;

	private double remoteLatestTimestamp;

	private Vector3 remoteCorrectionNeeded;

	private const float REMOTE_CORRECTION_RATE = 5f;

	private const bool USE_NEW_NETCODE = false;

	private VRRig.PartyMemberStatus partyMemberStatus;

	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[]
	{
		new GTBitOps.BitWriteInfo(0, 1),
		new GTBitOps.BitWriteInfo(1, 2),
		new GTBitOps.BitWriteInfo(3, 2)
	};

	public bool inTryOnRoom;

	public bool muted;

	public float scaleFactor;

	public float lastScaleFactor;

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

	private GorillaMouthFlap myMouthFlap;

	private GorillaSpeakerLoudness mySpeakerLoudness;

	public ReplacementVoice myReplacementVoice;

	private GorillaEyeExpressions myEyeExpressions;

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

	private const float QPackMax = 0.707107f;

	private const float QPackScale = 361.33145f;

	private const float QPackInvScale = 0.0027675421f;

	public Color playerColor;

	public bool colorInitialized;

	private Action<Color> onColorInitialized;

	private bool pendingCosmeticUpdate = true;

	private string rawCosmeticString = "";

	[SerializeField]
	private GuidedRefBasicTargetInfo guidedRefTargetInfo;

	[CompilerGenerated]
	private int <GorillaTag.GuidedRefs.IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount>k__BackingField;

	[DebugReadOnly]
	private static VRRig gLocalRig;

	private static bool bCachedLocalRig;

	public enum PartyMemberStatus
	{
		NeedsUpdate,
		InLocalParty,
		NotInLocalParty
	}

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

	private enum QAxis
	{
		X,
		Y,
		Z,
		W
	}

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal bool <NormalizeName>b__251_0(char c)
		{
			return char.IsLetterOrDigit(c);
		}

		internal void <SetColor>b__291_0(Color color1)
		{
		}

		public static readonly VRRig.<>c <>9 = new VRRig.<>c();

		public static Predicate<char> <>9__251_0;

		public static Action<Color> <>9__291_0;
	}

	[CompilerGenerated]
	private sealed class <OccasionalUpdate>d__215 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <OccasionalUpdate>d__215(int <>1__state)
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
			}
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
			this.<>2__current = new WaitForSeconds(1f);
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
	}
}
