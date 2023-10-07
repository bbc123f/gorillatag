using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F8 RID: 248
public class VRRig : MonoBehaviour, IGorillaSerializeable, IPreDisable, IUserCosmeticsCallback, IGuidedRefTarget, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x060005BE RID: 1470 RVA: 0x00023F87 File Offset: 0x00022187
	public int ActiveTransferrableObjectIndex(int idx)
	{
		return this.reliableState.activeTransferrableObjectIndex[idx];
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00023F96 File Offset: 0x00022196
	public int ActiveTransferrableObjectIndexLength()
	{
		return this.reliableState.activeTransferrableObjectIndex.Length;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00023FA5 File Offset: 0x000221A5
	public void SetActiveTransferrableObjectIndex(int idx, int v)
	{
		if (this.reliableState.activeTransferrableObjectIndex[idx] != v)
		{
			this.reliableState.activeTransferrableObjectIndex[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00023FD0 File Offset: 0x000221D0
	public TransferrableObject.PositionState TransferrablePosStates(int idx)
	{
		return this.reliableState.transferrablePosStates[idx];
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00023FDF File Offset: 0x000221DF
	public void SetTransferrablePosStates(int idx, TransferrableObject.PositionState v)
	{
		if (this.reliableState.transferrablePosStates[idx] != v)
		{
			this.reliableState.transferrablePosStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x0002400A File Offset: 0x0002220A
	public TransferrableObject.ItemStates TransferrableItemStates(int idx)
	{
		return this.reliableState.transferrableItemStates[idx];
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00024019 File Offset: 0x00022219
	public void SetTransferrableItemStates(int idx, TransferrableObject.ItemStates v)
	{
		if (this.reliableState.transferrableItemStates[idx] != v)
		{
			this.reliableState.transferrableItemStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00024044 File Offset: 0x00022244
	public void SetTransferrableDockPosition(int idx, BodyDockPositions.DropPositions v)
	{
		if (this.reliableState.transferableDockPositions[idx] != v)
		{
			this.reliableState.transferableDockPositions[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x0002406F File Offset: 0x0002226F
	public BodyDockPositions.DropPositions TransferrableDockPosition(int idx)
	{
		return this.reliableState.transferableDockPositions[idx];
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060005C7 RID: 1479 RVA: 0x0002407E File Offset: 0x0002227E
	// (set) Token: 0x060005C8 RID: 1480 RVA: 0x0002408B File Offset: 0x0002228B
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

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060005C9 RID: 1481 RVA: 0x000240B2 File Offset: 0x000222B2
	// (set) Token: 0x060005CA RID: 1482 RVA: 0x000240BF File Offset: 0x000222BF
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

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060005CB RID: 1483 RVA: 0x000240E6 File Offset: 0x000222E6
	// (set) Token: 0x060005CC RID: 1484 RVA: 0x000240F3 File Offset: 0x000222F3
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

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060005CD RID: 1485 RVA: 0x0002411A File Offset: 0x0002231A
	// (set) Token: 0x060005CE RID: 1486 RVA: 0x00024127 File Offset: 0x00022327
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

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x060005CF RID: 1487 RVA: 0x00024153 File Offset: 0x00022353
	// (set) Token: 0x060005D0 RID: 1488 RVA: 0x00024160 File Offset: 0x00022360
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

	// Token: 0x060005D1 RID: 1489 RVA: 0x0002418C File Offset: 0x0002238C
	public Color GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightThrowableProjectileColor;
		}
		return this.LeftThrowableProjectileColor;
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x0002419E File Offset: 0x0002239E
	public void SetThrowableProjectileColor(bool isLeftHand, Color color)
	{
		if (isLeftHand)
		{
			this.LeftThrowableProjectileColor = color;
			return;
		}
		this.RightThrowableProjectileColor = color;
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060005D3 RID: 1491 RVA: 0x000241B2 File Offset: 0x000223B2
	// (set) Token: 0x060005D4 RID: 1492 RVA: 0x000241BF File Offset: 0x000223BF
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

	// Token: 0x060005D5 RID: 1493 RVA: 0x000241E8 File Offset: 0x000223E8
	private void Awake()
	{
		this.GuidedRefInitialize();
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

	// Token: 0x060005D6 RID: 1494 RVA: 0x000242EB File Offset: 0x000224EB
	private void Start()
	{
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x000242ED File Offset: 0x000224ED
	private void EnsureInstantiatedMaterial()
	{
		if (this.didInstantiateMaterial)
		{
			return;
		}
		this.materialsToChangeTo[0] = Object.Instantiate<Material>(this.materialsToChangeTo[0]);
		this.didInstantiateMaterial = true;
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00024314 File Offset: 0x00022514
	private void SharedStart()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.isInitialized = true;
		this.myBodyDockPositions = base.GetComponent<BodyDockPositions>();
		this.reliableState.SharedStart(this.isOfflineVRRig, this.myBodyDockPositions);
		Application.quitting += this.Quitting;
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
		base.StartCoroutine(this.OccasionalUpdate());
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00024491 File Offset: 0x00022691
	private IEnumerator OccasionalUpdate()
	{
		for (;;)
		{
			try
			{
				if (!this.isOfflineVRRig)
				{
					if (PhotonNetwork.IsMasterClient && this.photonView != null && this.photonView.IsRoomView && this.photonView.IsMine)
					{
						Debug.Log("network deleting vrrig");
						PhotonNetwork.Destroy(this.photonView.gameObject);
					}
					if (this.photonView.IsRoomView)
					{
						Debug.Log("local disabling vrrig");
						this.photonView.gameObject.SetActive(false);
					}
					if (RoomSystem.JoinedRoom && PhotonNetwork.IsMasterClient && GorillaGameManager.instance == null)
					{
						object obj;
						PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
						if (obj.ToString().Contains("CASUAL") || obj.ToString().Contains("INFECTION"))
						{
							PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Tag Manager", base.transform.position, base.transform.rotation, 0, null);
						}
						else if (obj.ToString().Contains("HUNT"))
						{
							PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Hunt Manager", base.transform.position, base.transform.rotation, 0, null);
						}
						else if (obj.ToString().Contains("BATTLE"))
						{
							PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Battle Manager", base.transform.position, base.transform.rotation, 0, null);
						}
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x000244A0 File Offset: 0x000226A0
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

	// Token: 0x060005DB RID: 1499 RVA: 0x00024510 File Offset: 0x00022710
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
			base.transform.position = this.mainCamera.transform.position + this.headConstraint.rotation * this.head.trackingPositionOffset * this.scaleFactor + base.transform.rotation * this.headBodyOffset * this.scaleFactor;
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
		}
		else
		{
			if (this.voiceAudio != null)
			{
				float num = (GorillaTagger.Instance.offlineVRRig.transform.localScale.x - base.transform.localScale.x) / this.pitchScale + this.pitchOffset;
				float num2 = this.UsingHauntedRing ? this.HauntedVoicePitch : num;
				num2 = (this.IsHaunted ? this.HauntedVoicePitch : num2);
				if (!Mathf.Approximately(this.voiceAudio.pitch, num2))
				{
					this.voiceAudio.pitch = num2;
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
						base.transform.position += Vector3.Lerp(Vector3.zero, b, this.lastRopeGrabTimer * 4f);
						if (this.lastRopeGrabTimer < 1f)
						{
							this.lastRopeGrabTimer += Time.deltaTime;
						}
					}
					else
					{
						base.transform.position += b;
					}
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
			RisingLavaManager instance = RisingLavaManager.instance;
			int num3;
			if (instance != null && instance.GetMaterialIfPlayerInGame(this.creator.ActorNumber, out num3))
			{
				this.tempMatIndex = num3;
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

	// Token: 0x060005DC RID: 1500 RVA: 0x00024BCC File Offset: 0x00022DCC
	public void SetHeadBodyOffset()
	{
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00024BCE File Offset: 0x00022DCE
	public void VRRigResize(float ratioVar)
	{
		this.ratio *= ratioVar;
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00024BE0 File Offset: 0x00022DE0
	public int ReturnHandPosition()
	{
		return 0 + Mathf.FloorToInt(this.rightIndex.calcT * 9.99f) + Mathf.FloorToInt(this.rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(this.rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(this.leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(this.leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(this.leftThumb.calcT * 9.99f) * 100000 + this.leftHandHoldableStatus * 1000000 + this.rightHandHoldableStatus * 10000000;
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00024CAA File Offset: 0x00022EAA
	public void OnDestroy()
	{
		if (this.currentRopeSwingTarget && this.currentRopeSwingTarget.gameObject)
		{
			Object.Destroy(this.currentRopeSwingTarget.gameObject);
		}
		if (this.isQuitting)
		{
			return;
		}
		this.ClearRopeData();
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00024CEC File Offset: 0x00022EEC
	void IGorillaSerializeable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.head.rigTarget.localRotation);
		stream.SendNext(this.rightHand.rigTarget.localPosition);
		stream.SendNext(this.rightHand.rigTarget.localRotation);
		stream.SendNext(this.leftHand.rigTarget.localPosition);
		stream.SendNext(this.leftHand.rigTarget.localRotation);
		stream.SendNext(base.transform.position);
		stream.SendNext(Mathf.RoundToInt(base.transform.rotation.eulerAngles.y));
		stream.SendNext(this.ReturnHandPosition());
		stream.SendNext(this.currentState);
		stream.SendNext(this.grabbedRopeIndex);
		if (this.grabbedRopeIndex > 0)
		{
			stream.SendNext(this.grabbedRopeBoneIndex);
			stream.SendNext(this.grabbedRopeIsLeft);
			stream.SendNext(this.grabbedRopeOffset);
		}
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00024E30 File Offset: 0x00023030
	void IGorillaSerializeable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		this.head.syncRotation = this.SanitizeQuaternion((Quaternion)stream.ReceiveNext());
		this.rightHand.syncPos = this.SanitizeVector3((Vector3)stream.ReceiveNext());
		this.rightHand.syncRotation = this.SanitizeQuaternion((Quaternion)stream.ReceiveNext());
		this.leftHand.syncPos = this.SanitizeVector3((Vector3)stream.ReceiveNext());
		this.leftHand.syncRotation = this.SanitizeQuaternion((Quaternion)stream.ReceiveNext());
		this.syncPos = this.SanitizeVector3((Vector3)stream.ReceiveNext());
		this.syncRotation.eulerAngles = this.SanitizeVector3(new Vector3(0f, (float)((int)stream.ReceiveNext()), 0f));
		this.handSync = (int)stream.ReceiveNext();
		this.currentState = (TransferrableObject.PositionState)stream.ReceiveNext();
		this.lastPosition = this.syncPos;
		this.grabbedRopeIndex = (int)stream.ReceiveNext();
		if (this.grabbedRopeIndex > 0)
		{
			this.grabbedRopeBoneIndex = (int)stream.ReceiveNext();
			this.grabbedRopeIsLeft = (bool)stream.ReceiveNext();
			this.grabbedRopeOffset = this.SanitizeVector3((Vector3)stream.ReceiveNext());
		}
		this.UpdateRopeData();
		this.AddVelocityToQueue(this.syncPos, info);
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00024FA0 File Offset: 0x000231A0
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
		}
		this.shouldLerpToRope = true;
		this.previousGrabbedRope = this.grabbedRopeIndex;
		this.previousGrabbedRopeBoneIndex = this.grabbedRopeBoneIndex;
		this.previousGrabbedRopeWasLeft = this.grabbedRopeIsLeft;
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000250A0 File Offset: 0x000232A0
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
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x000250F0 File Offset: 0x000232F0
	public void ChangeMaterial(int materialIndex, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			this.ChangeMaterialLocal(materialIndex);
		}
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00025108 File Offset: 0x00023308
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

	// Token: 0x060005E6 RID: 1510 RVA: 0x00025248 File Offset: 0x00023448
	[PunRPC]
	public void InitializeNoobMaterial(float red, float green, float blue, bool leftHanded, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "InitializeNoobMaterial");
		if (info.Sender == this.photonView.Owner && (!this.initialized || (this.initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(info.Sender.UserId))))
		{
			this.initialized = true;
			red = Mathf.Clamp(red, 0f, 1f);
			green = Mathf.Clamp(green, 0f, 1f);
			blue = Mathf.Clamp(blue, 0f, 1f);
			this.InitializeNoobMaterialLocal(red, green, blue, leftHanded);
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent init noob", info.Sender.UserId, info.Sender.NickName);
		}
		this.playerLeftHanded = leftHanded;
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00025328 File Offset: 0x00023528
	public void InitializeNoobMaterialLocal(float red, float green, float blue, bool leftHanded)
	{
		Color color = new Color(red, green, blue);
		this.EnsureInstantiatedMaterial();
		this.materialsToChangeTo[0].color = color;
		if (this.photonView != null)
		{
			this.playerText.text = this.NormalizeName(true, this.photonView.Owner.NickName);
		}
		else if (this.showName)
		{
			this.playerText.text = PlayerPrefs.GetString("playerName");
		}
		this.SetColor(color);
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x000253AC File Offset: 0x000235AC
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

	// Token: 0x060005E9 RID: 1513 RVA: 0x00025423 File Offset: 0x00023623
	public void SetJumpLimitLocal(float maxJumpSpeed)
	{
		GorillaLocomotion.Player.Instance.maxJumpSpeed = maxJumpSpeed;
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00025430 File Offset: 0x00023630
	public void SetJumpMultiplierLocal(float jumpMultiplier)
	{
		GorillaLocomotion.Player.Instance.jumpMultiplier = jumpMultiplier;
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00025440 File Offset: 0x00023640
	[PunRPC]
	public void SetTaggedTime(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "SetTaggedTime");
		if (GorillaGameManager.instance != null)
		{
			if (info.Sender == PhotonNetwork.MasterClient)
			{
				GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				return;
			}
			GorillaNot.instance.SendReport("inappropriate tag data being sent set tagged time", info.Sender.UserId, info.Sender.NickName);
		}
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x000254F4 File Offset: 0x000236F4
	[PunRPC]
	public void SetSlowedTime(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "SetSlowedTime");
		if (GorillaGameManager.instance != null)
		{
			if (info.Sender == PhotonNetwork.MasterClient)
			{
				if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Slowed)
				{
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				}
				GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, GorillaTagger.Instance.slowCooldown);
				return;
			}
			GorillaNot.instance.SendReport("inappropriate tag data being sent set slowed time", info.Sender.UserId, info.Sender.NickName);
		}
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x000255B4 File Offset: 0x000237B4
	[PunRPC]
	public void SetJoinTaggedTime(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "SetJoinTaggedTime");
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent set join tagged time", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00025640 File Offset: 0x00023840
	[PunRPC]
	public void RequestMaterialColor(Photon.Realtime.Player askingPlayer, bool noneBool, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "RequestMaterialColor");
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("InitializeNoobMaterial", info.Sender, new object[]
			{
				this.materialsToChangeTo[0].color.r,
				this.materialsToChangeTo[0].color.g,
				this.materialsToChangeTo[0].color.b,
				GorillaComputer.instance.leftHanded
			});
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x000256E4 File Offset: 0x000238E4
	[PunRPC]
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

	// Token: 0x060005F0 RID: 1520 RVA: 0x00025764 File Offset: 0x00023964
	[PunRPC]
	public void PlayTagSound(int soundIndex, float soundVolume, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlayTagSound");
		if (info.Sender.IsMasterClient)
		{
			this.tagSound.volume = Mathf.Max(0.25f, soundVolume);
			this.tagSound.PlayOneShot(this.clipToPlay[soundIndex]);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent play tag sound", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x000257DC File Offset: 0x000239DC
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

	// Token: 0x060005F2 RID: 1522 RVA: 0x000258BC File Offset: 0x00023ABC
	[PunRPC]
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlayDrum");
		this.senderRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
		if (this.senderRig == null || this.senderRig.muted)
		{
			return;
		}
		if (drumIndex < 0 || drumIndex >= this.musicDrums.Length || (this.senderRig.transform.position - base.transform.position).sqrMagnitude > 9f)
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

	// Token: 0x060005F3 RID: 1523 RVA: 0x000259D8 File Offset: 0x00023BD8
	[PunRPC]
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlaySelfOnlyInstrument");
		if (info.Sender == this.photonView.Owner && !this.muted)
		{
			if (selfOnlyIndex >= 0 && selfOnlyIndex < this.instrumentSelfOnly.Length && info.Sender == this.photonView.Owner)
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

	// Token: 0x060005F4 RID: 1524 RVA: 0x00025AB4 File Offset: 0x00023CB4
	[PunRPC]
	public void PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlayHandTap");
		if (info.Sender == this.photonView.Owner)
		{
			this.PlayHandTapLocal(soundIndex, isLeftHand, Mathf.Max(tapVolume, 0.1f));
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent hand tap", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00025B20 File Offset: 0x00023D20
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

	// Token: 0x060005F6 RID: 1526 RVA: 0x00025C30 File Offset: 0x00023E30
	[PunRPC]
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlaySplashEffect");
		if (info.Sender == this.photonView.Owner)
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

	// Token: 0x060005F7 RID: 1527 RVA: 0x00025D8C File Offset: 0x00023F8C
	[PunRPC]
	public void PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "PlayGeodeEffect");
		if (info.Sender == this.photonView.Owner)
		{
			if ((base.transform.position - hitPosition).sqrMagnitude < 9f && this.geodeCrackingSound)
			{
				this.geodeCrackingSound.Play();
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent geode effect", info.Sender.UserId, info.Sender.NickName);
		}
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00025E18 File Offset: 0x00024018
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

	// Token: 0x060005F9 RID: 1529 RVA: 0x00025E90 File Offset: 0x00024090
	[PunRPC]
	public void UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "UpdateCosmetics");
		if (info.Sender == this.photonView.Owner)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			this.LocalUpdateCosmetics(newSet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x00025EFC File Offset: 0x000240FC
	[PunRPC]
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "UpdateCosmeticsWithTryon");
		if (info.Sender == this.photonView.Owner)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00025F74 File Offset: 0x00024174
	public void UpdateAllowedCosmetics()
	{
		if (GorillaGameManager.instance != null && GorillaGameManager.instance.playerCosmeticsLookup.TryGetValue(this.photonView.Owner.ActorNumber, out this.tempString))
		{
			this.concatStringOfCosmeticsAllowed = this.tempString;
			this.CheckForEarlyAccess();
		}
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00025FCB File Offset: 0x000241CB
	public void LocalUpdateCosmetics(CosmeticsController.CosmeticSet newSet)
	{
		this.cosmeticSet = newSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive();
		}
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00025FE2 File Offset: 0x000241E2
	public void LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet newSet, CosmeticsController.CosmeticSet newTryOnSet)
	{
		this.cosmeticSet = newSet;
		this.tryOnSet = newTryOnSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive();
		}
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00026000 File Offset: 0x00024200
	private void CheckForEarlyAccess()
	{
		if (this.concatStringOfCosmeticsAllowed.Contains("Early Access Supporter Pack"))
		{
			this.concatStringOfCosmeticsAllowed += "LBAAE.LFAAM.LFAAN.LHAAA.LHAAK.LHAAL.LHAAM.LHAAN.LHAAO.LHAAP.LHABA.LHABB.";
		}
		this.initializedCosmetics = true;
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00026034 File Offset: 0x00024234
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

	// Token: 0x06000600 RID: 1536 RVA: 0x000260B0 File Offset: 0x000242B0
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

	// Token: 0x06000601 RID: 1537 RVA: 0x00026106 File Offset: 0x00024306
	private void Quitting()
	{
		this.isQuitting = true;
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00026110 File Offset: 0x00024310
	public void GenerateFingerAngleLookupTables()
	{
		this.GenerateTableIndex(ref this.leftIndex);
		this.GenerateTableIndex(ref this.rightIndex);
		this.GenerateTableMiddle(ref this.leftMiddle);
		this.GenerateTableMiddle(ref this.rightMiddle);
		this.GenerateTableThumb(ref this.leftThumb);
		this.GenerateTableThumb(ref this.rightThumb);
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00026168 File Offset: 0x00024368
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

	// Token: 0x06000604 RID: 1540 RVA: 0x00026220 File Offset: 0x00024420
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

	// Token: 0x06000605 RID: 1541 RVA: 0x00026308 File Offset: 0x00024508
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

	// Token: 0x06000606 RID: 1542 RVA: 0x000263F0 File Offset: 0x000245F0
	private Quaternion SanitizeQuaternion(Quaternion quat)
	{
		if (float.IsNaN(quat.w) || float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsInfinity(quat.w) || float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z))
		{
			return Quaternion.identity;
		}
		return quat;
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x0002646C File Offset: 0x0002466C
	private Vector3 SanitizeVector3(Vector3 vec)
	{
		if (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z) || float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z))
		{
			return Vector3.zero;
		}
		return Vector3.ClampMagnitude(vec, 1000f);
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000264D8 File Offset: 0x000246D8
	private void IncrementRPC(PhotonMessageInfo info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x000264F0 File Offset: 0x000246F0
	private void AddVelocityToQueue(Vector3 position, PhotonMessageInfo info)
	{
		Vector3 velocity;
		if (this.velocityHistoryList.Count == 0)
		{
			velocity = Vector3.zero;
			this.lastPosition = position;
		}
		else
		{
			velocity = (position - this.lastPosition) / (float)(info.SentServerTime - this.velocityHistoryList[0].time);
		}
		this.velocityHistoryList.Insert(0, new VRRig.VelocityTime(velocity, info.SentServerTime));
		if (this.velocityHistoryList.Count > this.velocityHistoryMaxLength)
		{
			this.velocityHistoryList.RemoveRange(this.velocityHistoryMaxLength, this.velocityHistoryList.Count - this.velocityHistoryMaxLength);
		}
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x00026594 File Offset: 0x00024794
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

	// Token: 0x0600060B RID: 1547 RVA: 0x000266A8 File Offset: 0x000248A8
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

	// Token: 0x0600060C RID: 1548 RVA: 0x000266FA File Offset: 0x000248FA
	public void OnColorInitialized(Action<Color> action)
	{
		if (this.colorInitialized)
		{
			action(this.playerColor);
			return;
		}
		this.onColorInitialized = (Action<Color>)Delegate.Combine(this.onColorInitialized, action);
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00026728 File Offset: 0x00024928
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

	// Token: 0x0600060E RID: 1550 RVA: 0x00026760 File Offset: 0x00024960
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

	// Token: 0x0600060F RID: 1551 RVA: 0x0002685C File Offset: 0x00024A5C
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
			this.fxSettings.callSettings[1].CallLimitSettings.Reset();
		}
		catch
		{
			Debug.LogError("fxtype missing in fxSettings, please fix or remove this");
		}
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00026908 File Offset: 0x00024B08
	public void NetInitialize()
	{
		this.timeSpawned = Time.time;
		if (PhotonNetwork.InRoom)
		{
			GorillaGameManager instance = GorillaGameManager.instance;
			object obj;
			if (instance != null)
			{
				if (instance is GorillaHuntManager || instance.GameMode() == "HUNT")
				{
					this.EnableHuntWatch(true);
				}
				else if (instance is GorillaBattleManager || instance.GameMode() == "BATTLE")
				{
					this.EnableBattleCosmetics(true);
				}
			}
			else if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj))
			{
				string text = obj.ToString();
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

	// Token: 0x06000611 RID: 1553 RVA: 0x00026A44 File Offset: 0x00024C44
	public void EnableHuntWatch(bool on)
	{
		this.huntComputer.SetActive(on);
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00026A52 File Offset: 0x00024C52
	public void EnableBattleCosmetics(bool on)
	{
		this.battleBalloons.gameObject.SetActive(on);
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000613 RID: 1555 RVA: 0x00026A65 File Offset: 0x00024C65
	// (set) Token: 0x06000614 RID: 1556 RVA: 0x00026A6D File Offset: 0x00024C6D
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

	// Token: 0x06000615 RID: 1557 RVA: 0x00026A78 File Offset: 0x00024C78
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

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000616 RID: 1558 RVA: 0x00026B19 File Offset: 0x00024D19
	GuidedRefTargetIdSO IGuidedRefTarget.GuidedRefTargetId
	{
		get
		{
			this.guidedRefTargetInfo.targetId == null;
			return this.guidedRefTargetInfo.targetId;
		}
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000617 RID: 1559 RVA: 0x00026B38 File Offset: 0x00024D38
	Object IGuidedRefTarget.GuidedRefTargetObject
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x00026B3B File Offset: 0x00024D3B
	public void GuidedRefInitialize()
	{
		GuidedRefRelayHub.RegisterTargetWithParentRelays(this, this.guidedRefTargetInfo.hubIds, this);
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x00026C51 File Offset: 0x00024E51
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x00026C59 File Offset: 0x00024E59
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x040006EE RID: 1774
	public static Action newPlayerJoined;

	// Token: 0x040006EF RID: 1775
	public GuidedRefBasicTargetInfo guidedRefTargetInfo;

	// Token: 0x040006F0 RID: 1776
	public VRMap head;

	// Token: 0x040006F1 RID: 1777
	public VRMap rightHand;

	// Token: 0x040006F2 RID: 1778
	public VRMap leftHand;

	// Token: 0x040006F3 RID: 1779
	public VRMapThumb leftThumb;

	// Token: 0x040006F4 RID: 1780
	public VRMapIndex leftIndex;

	// Token: 0x040006F5 RID: 1781
	public VRMapMiddle leftMiddle;

	// Token: 0x040006F6 RID: 1782
	public VRMapThumb rightThumb;

	// Token: 0x040006F7 RID: 1783
	public VRMapIndex rightIndex;

	// Token: 0x040006F8 RID: 1784
	public VRMapMiddle rightMiddle;

	// Token: 0x040006F9 RID: 1785
	private int previousGrabbedRope = -1;

	// Token: 0x040006FA RID: 1786
	private int previousGrabbedRopeBoneIndex;

	// Token: 0x040006FB RID: 1787
	private bool previousGrabbedRopeWasLeft;

	// Token: 0x040006FC RID: 1788
	private GorillaRopeSwing currentRopeSwing;

	// Token: 0x040006FD RID: 1789
	private Transform currentRopeSwingTarget;

	// Token: 0x040006FE RID: 1790
	private float lastRopeGrabTimer;

	// Token: 0x040006FF RID: 1791
	private bool shouldLerpToRope;

	// Token: 0x04000700 RID: 1792
	[NonSerialized]
	public int grabbedRopeIndex = -1;

	// Token: 0x04000701 RID: 1793
	[NonSerialized]
	public int grabbedRopeBoneIndex;

	// Token: 0x04000702 RID: 1794
	[NonSerialized]
	public bool grabbedRopeIsLeft;

	// Token: 0x04000703 RID: 1795
	[NonSerialized]
	public Vector3 grabbedRopeOffset = Vector3.zero;

	// Token: 0x04000704 RID: 1796
	[Tooltip("- False in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool isOfflineVRRig;

	// Token: 0x04000705 RID: 1797
	public GameObject mainCamera;

	// Token: 0x04000706 RID: 1798
	public Transform playerOffsetTransform;

	// Token: 0x04000707 RID: 1799
	public int SDKIndex;

	// Token: 0x04000708 RID: 1800
	public bool isMyPlayer;

	// Token: 0x04000709 RID: 1801
	public AudioSource leftHandPlayer;

	// Token: 0x0400070A RID: 1802
	public AudioSource rightHandPlayer;

	// Token: 0x0400070B RID: 1803
	public AudioSource tagSound;

	// Token: 0x0400070C RID: 1804
	[SerializeField]
	private float ratio;

	// Token: 0x0400070D RID: 1805
	public Transform headConstraint;

	// Token: 0x0400070E RID: 1806
	public Vector3 headBodyOffset = Vector3.zero;

	// Token: 0x0400070F RID: 1807
	public GameObject headMesh;

	// Token: 0x04000710 RID: 1808
	public Vector3 syncPos;

	// Token: 0x04000711 RID: 1809
	public Quaternion syncRotation;

	// Token: 0x04000712 RID: 1810
	public AudioClip[] clipToPlay;

	// Token: 0x04000713 RID: 1811
	public AudioClip[] handTapSound;

	// Token: 0x04000714 RID: 1812
	public int currentMatIndex;

	// Token: 0x04000715 RID: 1813
	public int setMatIndex;

	// Token: 0x04000716 RID: 1814
	private int tempMatIndex;

	// Token: 0x04000717 RID: 1815
	public float lerpValueFingers;

	// Token: 0x04000718 RID: 1816
	public float lerpValueBody;

	// Token: 0x04000719 RID: 1817
	public GameObject backpack;

	// Token: 0x0400071A RID: 1818
	public Transform leftHandTransform;

	// Token: 0x0400071B RID: 1819
	public Transform rightHandTransform;

	// Token: 0x0400071C RID: 1820
	public SkinnedMeshRenderer mainSkin;

	// Token: 0x0400071D RID: 1821
	public Photon.Realtime.Player myPlayer;

	// Token: 0x0400071E RID: 1822
	public GameObject spectatorSkin;

	// Token: 0x0400071F RID: 1823
	public int handSync;

	// Token: 0x04000720 RID: 1824
	public Material[] materialsToChangeTo;

	// Token: 0x04000721 RID: 1825
	public float red;

	// Token: 0x04000722 RID: 1826
	public float green;

	// Token: 0x04000723 RID: 1827
	public float blue;

	// Token: 0x04000724 RID: 1828
	public string playerName;

	// Token: 0x04000725 RID: 1829
	public Text playerText;

	// Token: 0x04000726 RID: 1830
	[Tooltip("- True in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool showName;

	// Token: 0x04000727 RID: 1831
	public CosmeticItemRegistry cosmeticsObjectRegistry = new CosmeticItemRegistry();

	// Token: 0x04000728 RID: 1832
	public GameObject[] cosmetics;

	// Token: 0x04000729 RID: 1833
	public GameObject[] overrideCosmetics;

	// Token: 0x0400072A RID: 1834
	public string concatStringOfCosmeticsAllowed = "";

	// Token: 0x0400072B RID: 1835
	public bool initializedCosmetics;

	// Token: 0x0400072C RID: 1836
	public CosmeticsController.CosmeticSet cosmeticSet;

	// Token: 0x0400072D RID: 1837
	public CosmeticsController.CosmeticSet tryOnSet;

	// Token: 0x0400072E RID: 1838
	public CosmeticsController.CosmeticSet mergedSet;

	// Token: 0x0400072F RID: 1839
	public CosmeticsController.CosmeticSet prevSet;

	// Token: 0x04000730 RID: 1840
	private int cosmeticRetries = 2;

	// Token: 0x04000731 RID: 1841
	private int currentCosmeticTries;

	// Token: 0x04000732 RID: 1842
	public SizeManager sizeManager;

	// Token: 0x04000733 RID: 1843
	public float pitchScale = 0.3f;

	// Token: 0x04000734 RID: 1844
	public float pitchOffset = 1f;

	// Token: 0x04000735 RID: 1845
	[NonSerialized]
	public bool IsHaunted;

	// Token: 0x04000736 RID: 1846
	public float HauntedVoicePitch = 0.5f;

	// Token: 0x04000737 RID: 1847
	public float HauntedHearingVolume = 0.15f;

	// Token: 0x04000738 RID: 1848
	[NonSerialized]
	public bool UsingHauntedRing;

	// Token: 0x04000739 RID: 1849
	[NonSerialized]
	public float HauntedRingVoicePitch;

	// Token: 0x0400073A RID: 1850
	public VRRigReliableState reliableState;

	// Token: 0x0400073B RID: 1851
	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[]
	{
		new GTBitOps.BitWriteInfo(0, 1),
		new GTBitOps.BitWriteInfo(1, 2),
		new GTBitOps.BitWriteInfo(3, 2)
	};

	// Token: 0x0400073C RID: 1852
	public bool inTryOnRoom;

	// Token: 0x0400073D RID: 1853
	public bool muted;

	// Token: 0x0400073E RID: 1854
	public float scaleFactor;

	// Token: 0x0400073F RID: 1855
	private float timeSpawned;

	// Token: 0x04000740 RID: 1856
	public float doNotLerpConstant = 1f;

	// Token: 0x04000741 RID: 1857
	public string tempString;

	// Token: 0x04000742 RID: 1858
	private Photon.Realtime.Player tempPlayer;

	// Token: 0x04000743 RID: 1859
	internal Photon.Realtime.Player creator;

	// Token: 0x04000744 RID: 1860
	private VRRig tempRig;

	// Token: 0x04000745 RID: 1861
	private float[] speedArray;

	// Token: 0x04000746 RID: 1862
	private double handLerpValues;

	// Token: 0x04000747 RID: 1863
	private bool initialized;

	// Token: 0x04000748 RID: 1864
	public BattleBalloons battleBalloons;

	// Token: 0x04000749 RID: 1865
	private int tempInt;

	// Token: 0x0400074A RID: 1866
	public BodyDockPositions myBodyDockPositions;

	// Token: 0x0400074B RID: 1867
	public ParticleSystem lavaParticleSystem;

	// Token: 0x0400074C RID: 1868
	public ParticleSystem rockParticleSystem;

	// Token: 0x0400074D RID: 1869
	public ParticleSystem iceParticleSystem;

	// Token: 0x0400074E RID: 1870
	public string tempItemName;

	// Token: 0x0400074F RID: 1871
	public CosmeticsController.CosmeticItem tempItem;

	// Token: 0x04000750 RID: 1872
	public string tempItemId;

	// Token: 0x04000751 RID: 1873
	public int tempItemCost;

	// Token: 0x04000752 RID: 1874
	public int leftHandHoldableStatus;

	// Token: 0x04000753 RID: 1875
	public int rightHandHoldableStatus;

	// Token: 0x04000754 RID: 1876
	[Tooltip("This has to match the drumsAS array in DrumsItem.cs.")]
	[SerializeReference]
	public AudioSource[] musicDrums;

	// Token: 0x04000755 RID: 1877
	public TransferrableObject[] instrumentSelfOnly;

	// Token: 0x04000756 RID: 1878
	public AudioSource geodeCrackingSound;

	// Token: 0x04000757 RID: 1879
	public float bonkTime;

	// Token: 0x04000758 RID: 1880
	public float bonkCooldown = 2f;

	// Token: 0x04000759 RID: 1881
	public bool isQuitting;

	// Token: 0x0400075A RID: 1882
	private VRRig tempVRRig;

	// Token: 0x0400075B RID: 1883
	public GameObject huntComputer;

	// Token: 0x0400075C RID: 1884
	public Slingshot slingshot;

	// Token: 0x0400075D RID: 1885
	public bool playerLeftHanded;

	// Token: 0x0400075E RID: 1886
	public Slingshot.SlingshotState slingshotState;

	// Token: 0x0400075F RID: 1887
	private PhotonVoiceView myPhotonVoiceView;

	// Token: 0x04000760 RID: 1888
	private VRRig senderRig;

	// Token: 0x04000761 RID: 1889
	public TransferrableObject.PositionState currentState;

	// Token: 0x04000762 RID: 1890
	private bool isInitialized;

	// Token: 0x04000763 RID: 1891
	private List<VRRig.VelocityTime> velocityHistoryList = new List<VRRig.VelocityTime>();

	// Token: 0x04000764 RID: 1892
	public int velocityHistoryMaxLength = 200;

	// Token: 0x04000765 RID: 1893
	private Vector3 lastPosition;

	// Token: 0x04000766 RID: 1894
	public const int splashLimitCount = 4;

	// Token: 0x04000767 RID: 1895
	public const float splashLimitCooldown = 0.5f;

	// Token: 0x04000768 RID: 1896
	private float[] splashEffectTimes = new float[4];

	// Token: 0x04000769 RID: 1897
	internal AudioSource voiceAudio;

	// Token: 0x0400076A RID: 1898
	[SerializeField]
	internal PhotonView photonView;

	// Token: 0x0400076B RID: 1899
	[SerializeField]
	private FXSystemSettings sharedFXSettings;

	// Token: 0x0400076C RID: 1900
	[NonSerialized]
	public FXSystemSettings fxSettings;

	// Token: 0x0400076D RID: 1901
	private bool didInstantiateMaterial;

	// Token: 0x0400076E RID: 1902
	private bool playerWasHaunted;

	// Token: 0x0400076F RID: 1903
	private float nonHauntedVolume;

	// Token: 0x04000770 RID: 1904
	public Color playerColor;

	// Token: 0x04000771 RID: 1905
	public bool colorInitialized;

	// Token: 0x04000772 RID: 1906
	private Action<Color> onColorInitialized;

	// Token: 0x04000773 RID: 1907
	private bool pendingCosmeticUpdate = true;

	// Token: 0x04000774 RID: 1908
	private string rawCosmeticString = "";

	// Token: 0x020003F1 RID: 1009
	public enum WearablePackedStateSlots
	{
		// Token: 0x04001C75 RID: 7285
		Hat,
		// Token: 0x04001C76 RID: 7286
		LeftHand,
		// Token: 0x04001C77 RID: 7287
		RightHand
	}

	// Token: 0x020003F2 RID: 1010
	public struct VelocityTime
	{
		// Token: 0x06001BDD RID: 7133 RVA: 0x00096568 File Offset: 0x00094768
		public VelocityTime(Vector3 velocity, double velTime)
		{
			this.vel = velocity;
			this.time = velTime;
		}

		// Token: 0x04001C78 RID: 7288
		public Vector3 vel;

		// Token: 0x04001C79 RID: 7289
		public double time;
	}
}
