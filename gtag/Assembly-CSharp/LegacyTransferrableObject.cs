using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000110 RID: 272
public class LegacyTransferrableObject : HoldableObject
{
	// Token: 0x060006AE RID: 1710 RVA: 0x00029B75 File Offset: 0x00027D75
	protected virtual void Awake()
	{
		this.latched = false;
		this.initOffset = base.transform.localPosition;
		this.initRotation = base.transform.localRotation;
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x00029BA0 File Offset: 0x00027DA0
	protected virtual void Start()
	{
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00029BA4 File Offset: 0x00027DA4
	public override void OnEnable()
	{
		base.OnEnable();
		if (this.myRig == null && this.myOnlineRig != null && this.myOnlineRig.photonView != null && this.myOnlineRig.photonView.IsMine)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.myRig == null && this.myOnlineRig == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.objectIndex = this.targetDock.ReturnTransferrableItemIndex(this.myIndex);
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
			{
				this.storedZone = BodyDockPositions.DropPositions.LeftArm;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
			{
				this.storedZone = BodyDockPositions.DropPositions.RightArm;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
			{
				this.storedZone = BodyDockPositions.DropPositions.LeftBack;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
			{
				this.storedZone = BodyDockPositions.DropPositions.RightBack;
			}
			else
			{
				this.storedZone = BodyDockPositions.DropPositions.Chest;
			}
		}
		if (this.objectIndex == -1)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.currentState == TransferrableObject.PositionState.OnLeftArm && this.flipOnXForLeftArm)
		{
			Transform transform = this.GetAnchor(this.currentState);
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		this.initState = this.currentState;
		this.enabledOnFrame = Time.frameCount;
		this.SpawnShareableObject();
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x00029D30 File Offset: 0x00027F30
	public override void OnDisable()
	{
		base.OnDisable();
		this.enabledOnFrame = -1;
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x00029D40 File Offset: 0x00027F40
	private void SpawnShareableObject()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		if (this.worldShareableInstance != null)
		{
			return;
		}
		object[] data = new object[]
		{
			this.myIndex,
			PhotonNetwork.LocalPlayer
		};
		this.worldShareableInstance = PhotonNetwork.Instantiate("Objects/equipment/WorldShareableItem", base.transform.position, base.transform.rotation, 0, data);
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x00029DDC File Offset: 0x00027FDC
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.SpawnShareableObject();
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x00029DEA File Offset: 0x00027FEA
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		if (this.worldShareableInstance != null)
		{
			PhotonNetwork.Destroy(this.worldShareableInstance);
		}
		this.OnWorldShareableItemDeallocated(PhotonNetwork.LocalPlayer);
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00029E16 File Offset: 0x00028016
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		this.OnWorldShareableItemDeallocated(otherPlayer);
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00029E26 File Offset: 0x00028026
	public void SetWorldShareableItem(GameObject item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x00029E35 File Offset: 0x00028035
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x00029E37 File Offset: 0x00028037
	protected virtual void OnWorldShareableItemDeallocated(Player player)
	{
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x00029E3C File Offset: 0x0002803C
	public virtual void LateUpdate()
	{
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
		}
		if (this.IsMyItem())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
		this.previousState = this.currentState;
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x00029E8C File Offset: 0x0002808C
	protected Transform DefaultAnchor()
	{
		if (!(this.anchor == null))
		{
			return this.anchor;
		}
		return base.transform;
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00029EA9 File Offset: 0x000280A9
	private Transform GetAnchor(TransferrableObject.PositionState pos)
	{
		if (this.grabAnchor == null)
		{
			return this.DefaultAnchor();
		}
		if (this.InHand())
		{
			return this.grabAnchor;
		}
		return this.DefaultAnchor();
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00029ED8 File Offset: 0x000280D8
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00029F08 File Offset: 0x00028108
	private void UpdateFollowXform()
	{
		if (this.targetRig == null)
		{
			return;
		}
		if (this.targetDock == null)
		{
			this.targetDock = this.targetRig.GetComponent<BodyDockPositions>();
		}
		if (this.anchorOverrides == null)
		{
			this.anchorOverrides = this.targetRig.GetComponent<VRRigAnchorOverrides>();
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState <= TransferrableObject.PositionState.InRightHand)
		{
			switch (positionState)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftArmTransform);
				break;
			case TransferrableObject.PositionState.OnRightArm:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightArmTransform);
				break;
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftHandTransform);
				break;
			default:
				if (positionState == TransferrableObject.PositionState.InRightHand)
				{
					transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightHandTransform);
				}
				break;
			}
		}
		else if (positionState != TransferrableObject.PositionState.OnChest)
		{
			if (positionState != TransferrableObject.PositionState.OnLeftShoulder)
			{
				if (positionState == TransferrableObject.PositionState.OnRightShoulder)
				{
					transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightBackTransform);
				}
			}
			else
			{
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftBackTransform);
			}
		}
		else
		{
			transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.chestTransform);
		}
		LegacyTransferrableObject.InterpolateState interpolateState = this.interpState;
		if (interpolateState != LegacyTransferrableObject.InterpolateState.None)
		{
			if (interpolateState != LegacyTransferrableObject.InterpolateState.Interpolating)
			{
				return;
			}
			float t = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			transform.transform.position = Vector3.Lerp(this.interpStartPos, transform2.transform.position, t);
			transform.transform.rotation = Quaternion.Slerp(this.interpStartRot, transform2.transform.rotation, t);
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = LegacyTransferrableObject.InterpolateState.None;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				if (this.flipOnXForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				if (this.flipOnYForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(1f, -1f, 1f);
				}
			}
		}
		else if (transform2 != transform.parent)
		{
			if (Time.frameCount == this.enabledOnFrame)
			{
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				return;
			}
			this.interpState = LegacyTransferrableObject.InterpolateState.Interpolating;
			this.interpDt = this.interpTime;
			this.interpStartPos = transform.transform.position;
			this.interpStartRot = transform.transform.rotation;
			return;
		}
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x0002A22D File Offset: 0x0002842D
	public void DropItem()
	{
		base.transform.parent = null;
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x0002A23C File Offset: 0x0002843C
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		for (int i = 0; i < this.targetRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.targetRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				this.disableItem = false;
				break;
			}
		}
		if (this.disableItem)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.previousState != this.currentState && this.detatchOnGrab && this.InHand())
		{
			base.transform.parent = null;
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped)
		{
			this.UpdateFollowXform();
			return;
		}
		if (this.canDrop)
		{
			this.DropItem();
		}
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x0002A2E4 File Offset: 0x000284E4
	protected void ResetXf()
	{
		if (this.canDrop)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			base.transform.localPosition = this.initOffset;
			base.transform.localRotation = this.initRotation;
		}
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x0002A34F File Offset: 0x0002854F
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		this.ResetXf();
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x0002A36C File Offset: 0x0002856C
	private void HandleLocalInput()
	{
		GameObject[] array;
		if (!this.InHand())
		{
			array = this.gameObjectsActiveOnlyWhileHeld;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			return;
		}
		array = this.gameObjectsActiveOnlyWhileHeld;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		XRNode node = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? XRNode.LeftHand : XRNode.RightHand;
		this.indexTrigger = ControllerInputPoller.TriggerFloat(node);
		bool flag = !this.latched && this.indexTrigger >= this.myThreshold;
		bool flag2 = this.latched && this.indexTrigger < this.myThreshold - this.hysterisis;
		if (flag || this.testActivate)
		{
			this.testActivate = false;
			if (this.CanActivate())
			{
				this.OnActivate();
				return;
			}
		}
		else if (flag2 || this.testDeactivate)
		{
			this.testDeactivate = false;
			if (this.CanDeactivate())
			{
				this.OnDeactivate();
			}
		}
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x0002A458 File Offset: 0x00028658
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		if (PhotonNetwork.InRoom)
		{
			this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
			this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
		}
		this.targetRig = this.myRig;
		this.HandleLocalInput();
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x0002A4C0 File Offset: 0x000286C0
	protected virtual void LateUpdateReplicated()
	{
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (this.currentState == TransferrableObject.PositionState.Dropped && !this.canDrop && !this.shareable)
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.SetActive(false);
			}
			this.currentState = this.previousState;
		}
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.targetRig = this.myOnlineRig;
		if (this.myOnlineRig != null)
		{
			bool flag = true;
			for (int i = 0; i < this.myOnlineRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
				{
					flag = false;
					GameObject[] array = this.gameObjectsActiveOnlyWhileHeld;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].SetActive(this.InHand());
					}
				}
			}
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x0002A5B2 File Offset: 0x000287B2
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		this.ResetXf();
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x0002A5D8 File Offset: 0x000287D8
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!(grabbingHand == this.interactor.leftHand) || this.currentState == TransferrableObject.PositionState.OnLeftArm)
		{
			if (grabbingHand == this.interactor.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
			{
				if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
				{
					return;
				}
				this.canAutoGrabRight = false;
				this.currentState = TransferrableObject.PositionState.InRightHand;
				EquipmentInteractor.instance.UpdateHandEquipment(this, false);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			}
			return;
		}
		if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
		{
			return;
		}
		this.canAutoGrabLeft = false;
		this.currentState = TransferrableObject.PositionState.InLeftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, true);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x0002A6E0 File Offset: 0x000288E0
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!this.CanDeactivate())
		{
			return;
		}
		if (this.IsHeld() && ((releasingHand == EquipmentInteractor.instance.rightHand && this == EquipmentInteractor.instance.rightHandHeldEquipment) || (releasingHand == EquipmentInteractor.instance.leftHand && this == EquipmentInteractor.instance.leftHandHeldEquipment)))
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				this.canAutoGrabLeft = true;
			}
			else
			{
				this.canAutoGrabRight = true;
			}
			if (zoneReleased != null)
			{
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.LeftArm;
				bool flag2 = this.currentState == TransferrableObject.PositionState.InRightHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.RightArm;
				if (flag || flag2)
				{
					return;
				}
				if (this.targetDock.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == this.targetDock && (zoneReleased.dropPosition & this.dockPositions) != BodyDockPositions.DropPositions.None)
				{
					this.storedZone = zoneReleased.dropPosition;
				}
			}
			this.DropItemCleanup();
			EquipmentInteractor.instance.UpdateHandEquipment(null, releasingHand == EquipmentInteractor.instance.leftHand);
		}
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x0002A824 File Offset: 0x00028A24
	public override void DropItemCleanup()
	{
		if (this.canDrop)
		{
			this.currentState = TransferrableObject.PositionState.Dropped;
			return;
		}
		BodyDockPositions.DropPositions dropPositions = this.storedZone;
		switch (dropPositions)
		{
		case BodyDockPositions.DropPositions.LeftArm:
			this.currentState = TransferrableObject.PositionState.OnLeftArm;
			return;
		case BodyDockPositions.DropPositions.RightArm:
			this.currentState = TransferrableObject.PositionState.OnRightArm;
			return;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
			break;
		case BodyDockPositions.DropPositions.Chest:
			this.currentState = TransferrableObject.PositionState.OnChest;
			return;
		default:
			if (dropPositions == BodyDockPositions.DropPositions.LeftBack)
			{
				this.currentState = TransferrableObject.PositionState.OnLeftShoulder;
				return;
			}
			if (dropPositions != BodyDockPositions.DropPositions.RightBack)
			{
				return;
			}
			this.currentState = TransferrableObject.PositionState.OnRightShoulder;
			break;
		}
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x0002A898 File Offset: 0x00028A98
	public virtual void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x0002A8FC File Offset: 0x00028AFC
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		VRRig vrrig = this.targetRig;
		if ((vrrig != null) ? vrrig.photonView : null)
		{
			this.targetRig.photonView.RPC("PlayHandTap", RpcTarget.Others, new object[]
			{
				soundIndex,
				flag,
				0.1f
			});
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x0002A983 File Offset: 0x00028B83
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x0002A985 File Offset: 0x00028B85
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x0002A997 File Offset: 0x00028B97
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x0002A99A File Offset: 0x00028B9A
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x0002A99D File Offset: 0x00028B9D
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x0002A9A6 File Offset: 0x00028BA6
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x0002A9AF File Offset: 0x00028BAF
	public virtual bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x0002A9CC File Offset: 0x00028BCC
	protected virtual bool IsHeld()
	{
		return EquipmentInteractor.instance.leftHandHeldEquipment == this || EquipmentInteractor.instance.rightHandHeldEquipment == this;
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x0002A9F6 File Offset: 0x00028BF6
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x0002AA0C File Offset: 0x00028C0C
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x0002AA1B File Offset: 0x00028C1B
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x0002AA26 File Offset: 0x00028C26
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x0002AA31 File Offset: 0x00028C31
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x0002AA3D File Offset: 0x00028C3D
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x0002AA55 File Offset: 0x00028C55
	protected Player OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.photonView.Owner;
		}
		return PhotonNetwork.LocalPlayer;
	}

	// Token: 0x0400080E RID: 2062
	protected EquipmentInteractor interactor;

	// Token: 0x0400080F RID: 2063
	public VRRig myRig;

	// Token: 0x04000810 RID: 2064
	public VRRig myOnlineRig;

	// Token: 0x04000811 RID: 2065
	public bool latched;

	// Token: 0x04000812 RID: 2066
	private float indexTrigger;

	// Token: 0x04000813 RID: 2067
	public bool testActivate;

	// Token: 0x04000814 RID: 2068
	public bool testDeactivate;

	// Token: 0x04000815 RID: 2069
	public float myThreshold = 0.8f;

	// Token: 0x04000816 RID: 2070
	public float hysterisis = 0.05f;

	// Token: 0x04000817 RID: 2071
	public bool flipOnXForLeftHand;

	// Token: 0x04000818 RID: 2072
	public bool flipOnYForLeftHand;

	// Token: 0x04000819 RID: 2073
	public bool flipOnXForLeftArm;

	// Token: 0x0400081A RID: 2074
	public bool disableStealing;

	// Token: 0x0400081B RID: 2075
	private TransferrableObject.PositionState initState;

	// Token: 0x0400081C RID: 2076
	public TransferrableObject.ItemStates itemState;

	// Token: 0x0400081D RID: 2077
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x0400081E RID: 2078
	protected TransferrableObject.PositionState previousState;

	// Token: 0x0400081F RID: 2079
	public TransferrableObject.PositionState currentState;

	// Token: 0x04000820 RID: 2080
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x04000821 RID: 2081
	public VRRig targetRig;

	// Token: 0x04000822 RID: 2082
	public BodyDockPositions targetDock;

	// Token: 0x04000823 RID: 2083
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04000824 RID: 2084
	public bool canAutoGrabLeft;

	// Token: 0x04000825 RID: 2085
	public bool canAutoGrabRight;

	// Token: 0x04000826 RID: 2086
	public int objectIndex;

	// Token: 0x04000827 RID: 2087
	[Tooltip("In Holdables.prefab, assign to the parent of this transform.\nExample: 'Holdables/YellowHandBootsRight' is the anchor of 'Holdables/YellowHandBootsRight/YELLOW HAND BOOTS'")]
	public Transform anchor;

	// Token: 0x04000828 RID: 2088
	[Tooltip("In Holdables.prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x04000829 RID: 2089
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x0400082A RID: 2090
	public int myIndex;

	// Token: 0x0400082B RID: 2091
	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x0400082C RID: 2092
	protected GameObject worldShareableInstance;

	// Token: 0x0400082D RID: 2093
	private float interpTime = 0.1f;

	// Token: 0x0400082E RID: 2094
	private float interpDt;

	// Token: 0x0400082F RID: 2095
	private Vector3 interpStartPos;

	// Token: 0x04000830 RID: 2096
	private Quaternion interpStartRot;

	// Token: 0x04000831 RID: 2097
	protected int enabledOnFrame = -1;

	// Token: 0x04000832 RID: 2098
	private Vector3 initOffset;

	// Token: 0x04000833 RID: 2099
	private Quaternion initRotation;

	// Token: 0x04000834 RID: 2100
	public bool canDrop;

	// Token: 0x04000835 RID: 2101
	public bool shareable;

	// Token: 0x04000836 RID: 2102
	public bool detatchOnGrab;

	// Token: 0x04000837 RID: 2103
	private bool wasHover;

	// Token: 0x04000838 RID: 2104
	private bool isHover;

	// Token: 0x04000839 RID: 2105
	private bool disableItem;

	// Token: 0x0400083A RID: 2106
	public const int kPositionStateCount = 8;

	// Token: 0x0400083B RID: 2107
	public LegacyTransferrableObject.InterpolateState interpState;

	// Token: 0x020003FF RID: 1023
	public enum InterpolateState
	{
		// Token: 0x04001CA6 RID: 7334
		None,
		// Token: 0x04001CA7 RID: 7335
		Interpolating
	}
}
