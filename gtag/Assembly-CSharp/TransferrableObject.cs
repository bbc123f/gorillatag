using System;
using GorillaExtensions;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200011D RID: 285
public class TransferrableObject : HoldableObject, ISelfValidator, IRequestableOwnershipGuardCallbacks, IPreDisable
{
	// Token: 0x06000744 RID: 1860 RVA: 0x0002E038 File Offset: 0x0002C238
	public void FixTransformOverride()
	{
		this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0002E046 File Offset: 0x0002C246
	public void Validate(SelfValidationResult result)
	{
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0002E048 File Offset: 0x0002C248
	public void SetTargetRig(VRRig rig)
	{
		if (rig == null)
		{
			this.targetRigSet = false;
			if (this.isSceneObject)
			{
				this.targetRig = rig;
				this.targetDock = null;
				this.anchorOverrides = null;
				return;
			}
			if (this.myRig)
			{
				this.SetTargetRig(this.myRig);
			}
			if (this.myOnlineRig)
			{
				this.SetTargetRig(this.myOnlineRig);
			}
			return;
		}
		else
		{
			this.targetRigSet = true;
			this.targetRig = rig;
			BodyDockPositions component = rig.GetComponent<BodyDockPositions>();
			VRRigAnchorOverrides component2 = rig.GetComponent<VRRigAnchorOverrides>();
			if (!component)
			{
				Debug.LogError("There is no dock attached to this rig", this);
				return;
			}
			if (!component2)
			{
				Debug.LogError("There is no overrides attached to this rig", this);
				return;
			}
			this.anchorOverrides = component2;
			this.targetDock = component;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			return;
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000747 RID: 1863 RVA: 0x0002E118 File Offset: 0x0002C318
	public bool IsLocalOwnedWorldShareable
	{
		get
		{
			return this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine;
		}
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0002E139 File Offset: 0x0002C339
	public void WorldShareableRequestOwnership()
	{
		if (this.worldShareableInstance != null)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x06000749 RID: 1865 RVA: 0x0002E178 File Offset: 0x0002C378
	// (set) Token: 0x0600074A RID: 1866 RVA: 0x0002E180 File Offset: 0x0002C380
	private protected bool appIsQuitting { protected get; private set; }

	// Token: 0x0600074B RID: 1867 RVA: 0x0002E18C File Offset: 0x0002C38C
	protected virtual void Awake()
	{
		if (this.rigidbodyInstance == null)
		{
			this.rigidbodyInstance = base.GetComponent<Rigidbody>();
		}
		if (this.rigidbodyInstance != null)
		{
			this.isRigidbodySet = true;
		}
		this.audioSrc = base.GetComponent<AudioSource>();
		this.latched = false;
		if (!this.positionInitialized)
		{
			this.SetInitMatrix();
			this.positionInitialized = true;
		}
		if (this.anchor == null)
		{
			this.InitialDockObject = base.transform.parent;
		}
		else
		{
			this.InitialDockObject = this.anchor.parent;
		}
		this.isGrabAnchorSet = (this.grabAnchor != null);
		Application.quitting += delegate()
		{
			this.appIsQuitting = true;
		};
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x0002E248 File Offset: 0x0002C448
	private void SetInitMatrix()
	{
		this.initMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		if (this.handPoseLeft != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseLeftReferenceRotation * Quaternion.Inverse(this.handPoseLeft.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseLeftReferencePoint) - this.handPoseLeft.transform.position;
			this.leftHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.leftHandMatrix = this.initMatrix;
		}
		if (this.handPoseRight != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseRightReferenceRotation * Quaternion.Inverse(this.handPoseRight.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseRightReferencePoint) - this.handPoseRight.transform.position;
			this.rightHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.rightHandMatrix = this.initMatrix;
		}
		base.transform.localPosition = this.initMatrix.Position();
		base.transform.localRotation = this.initMatrix.Rotation();
		this.positionInitialized = true;
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x0002E3BD File Offset: 0x0002C5BD
	protected virtual void Start()
	{
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0002E3C0 File Offset: 0x0002C5C0
	public override void OnEnable()
	{
		base.OnEnable();
		this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
		if (!this.positionInitialized)
		{
			this.SetInitMatrix();
			this.positionInitialized = true;
		}
		if (this.isSceneObject)
		{
			if (!this.worldShareableInstance)
			{
				Debug.LogError("Missing Sharable Instance on Scene enabled object");
				return;
			}
			this.worldShareableInstance.SyncToSceneObject(this);
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().AddCallbackTarget(this);
			return;
		}
		else
		{
			if (!this.myRig && this.myOnlineRig)
			{
				this.ownerRig = this.myOnlineRig;
				this.SetTargetRig(this.myOnlineRig);
			}
			if (this.myRig == null && this.myOnlineRig == null)
			{
				if (!this.isSceneObject)
				{
					base.gameObject.SetActive(false);
				}
				return;
			}
			this.objectIndex = this.targetDock.ReturnTransferrableItemIndex(this.myIndex);
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
			else if (this.currentState == TransferrableObject.PositionState.OnChest)
			{
				this.storedZone = BodyDockPositions.DropPositions.Chest;
			}
			if (this.IsLocalObject())
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
				this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
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
			this.startInterpolation = true;
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			if (!this.canDrop && !this.shareable)
			{
				return;
			}
			this.SpawnTransferableObjectViews();
			if (!this.myRig)
			{
				return;
			}
			if (this.myRig != null && this.worldShareableInstance != null)
			{
				this.OnWorldShareableItemSpawn();
			}
			return;
		}
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0002E600 File Offset: 0x0002C800
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.appIsQuitting)
		{
			return;
		}
		this.enabledOnFrame = -1;
		if (!this.isSceneObject && this.IsLocalObject() && this.worldShareableInstance && !this.IsMyItem())
		{
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
			{
			});
		}
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDock)
			{
				this.targetDock.DeallocateSharableInstance(this.worldShareableInstance);
			}
			this.worldShareableInstance = null;
		}
		this.PlayDestroyedOrDisabledEffect();
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0002E6CC File Offset: 0x0002C8CC
	public void CleanupDisable()
	{
		this.currentState = TransferrableObject.PositionState.None;
		this.enabledOnFrame = -1;
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			if (this.anchor != base.transform)
			{
				base.transform.parent = this.anchor;
			}
		}
		else
		{
			base.transform.parent = this.anchor;
		}
		this.interpState = TransferrableObject.InterpolateState.None;
		Transform transform = base.transform;
		Matrix4x4 defaultTransformationMatrix = this.GetDefaultTransformationMatrix();
		transform.SetLocalMatrixRelativeToParentWithXParity(defaultTransformationMatrix);
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0002E757 File Offset: 0x0002C957
	public virtual void PreDisable()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		this.currentState = TransferrableObject.PositionState.None;
		this.interpState = TransferrableObject.InterpolateState.None;
		this.ResetToDefaultState();
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0002E774 File Offset: 0x0002C974
	public virtual Matrix4x4 GetDefaultTransformationMatrix()
	{
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState == TransferrableObject.PositionState.InLeftHand)
		{
			return this.leftHandMatrix;
		}
		if (positionState != TransferrableObject.PositionState.InRightHand)
		{
			return this.initMatrix;
		}
		return this.rightHandMatrix;
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0002E7A6 File Offset: 0x0002C9A6
	public virtual bool ShouldBeKinematic()
	{
		if (this.detatchOnGrab)
		{
			return this.currentState != TransferrableObject.PositionState.Dropped && this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand;
		}
		return this.currentState != TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x0002E7E4 File Offset: 0x0002C9E4
	private void SpawnShareableObject()
	{
		if (this.isSceneObject)
		{
			if (this.worldShareableInstance == null)
			{
				return;
			}
			this.worldShareableInstance.GetComponent<WorldShareableItem>().SetupSceneObjectOnNetwork(PhotonNetwork.MasterClient);
			return;
		}
		else
		{
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			this.SpawnTransferableObjectViews();
			if (!this.myRig)
			{
				return;
			}
			if (!this.canDrop && !this.shareable)
			{
				return;
			}
			if (this.myRig != null && this.worldShareableInstance != null)
			{
				this.OnWorldShareableItemSpawn();
			}
			return;
		}
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x0002E870 File Offset: 0x0002CA70
	public void SpawnTransferableObjectViews()
	{
		Debug.Log("SpawnTransferableObjectViews on gameobject " + base.gameObject.name);
		Player owner = PhotonNetwork.LocalPlayer;
		if (!this.ownerRig.isOfflineVRRig)
		{
			owner = this.ownerRig.creator;
		}
		if (this.worldShareableInstance == null)
		{
			this.worldShareableInstance = this.targetDock.AllocateSharableInstance(this.storedZone, owner);
		}
		GorillaTagger.OnPlayerSpawned(delegate
		{
			this.worldShareableInstance.SetupSharableObject(this.myIndex, owner, this.transform);
		});
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0002E904 File Offset: 0x0002CB04
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.isSceneObject)
		{
			this.worldShareableInstance == null;
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		this.SpawnTransferableObjectViews();
		if (!this.myRig)
		{
			return;
		}
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0002E97C File Offset: 0x0002CB7C
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		if (this.isSceneObject)
		{
			return;
		}
		if (!this.shareable && !this.allowWorldSharableInstance && !this.canDrop)
		{
			return;
		}
		if (base.gameObject.activeSelf && this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDock)
			{
				this.targetDock.DeallocateSharableInstance(this.worldShareableInstance);
			}
			else
			{
				this.worldShareableInstance.ResetViews();
				ObjectPools.instance.Destroy(this.worldShareableInstance.gameObject);
			}
			this.worldShareableInstance = null;
		}
		if (!this.IsLocalObject())
		{
			this.OnItemDestroyedOrDisabled();
			base.gameObject.Disable();
			return;
		}
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x0002EA48 File Offset: 0x0002CC48
	public bool IsLocalObject()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x0002EA5F File Offset: 0x0002CC5F
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		bool flag = this.isSceneObject;
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0002EA6F File Offset: 0x0002CC6F
	public void SetWorldShareableItem(WorldShareableItem item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0002EA7E File Offset: 0x0002CC7E
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x0002EA80 File Offset: 0x0002CC80
	protected virtual void PlayDestroyedOrDisabledEffect()
	{
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x0002EA84 File Offset: 0x0002CC84
	protected virtual void OnItemDestroyedOrDisabled()
	{
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDock)
			{
				this.targetDock.DeallocateSharableInstance(this.worldShareableInstance);
			}
			this.worldShareableInstance = null;
		}
		this.PlayDestroyedOrDisabledEffect();
		this.enabledOnFrame = -1;
		this.currentState = TransferrableObject.PositionState.None;
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x0002EAF3 File Offset: 0x0002CCF3
	protected virtual void LateUpdate()
	{
		if (this.IsLocalObject() && this.canDrop)
		{
			this.LocalMyObjectValidation();
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

	// Token: 0x0600075F RID: 1887 RVA: 0x0002EB33 File Offset: 0x0002CD33
	protected Transform DefaultAnchor()
	{
		if (this._isDefaultAnchorSet)
		{
			return this._defaultAnchor;
		}
		this._isDefaultAnchorSet = true;
		this._defaultAnchor = ((this.anchor == null) ? base.transform : this.anchor);
		return this._defaultAnchor;
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x0002EB73 File Offset: 0x0002CD73
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

	// Token: 0x06000761 RID: 1889 RVA: 0x0002EBA0 File Offset: 0x0002CDA0
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x0002EBD0 File Offset: 0x0002CDD0
	private Transform GetTargetStorageZone(BodyDockPositions.DropPositions state)
	{
		switch (state)
		{
		case BodyDockPositions.DropPositions.None:
			return null;
		case BodyDockPositions.DropPositions.LeftArm:
			return this.targetDock.leftArmTransform;
		case BodyDockPositions.DropPositions.RightArm:
			return this.targetDock.rightArmTransform;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.Chest:
			return this.targetDock.chestTransform;
		case BodyDockPositions.DropPositions.LeftBack:
			return this.targetDock.leftBackTransform;
		default:
			if (state == BodyDockPositions.DropPositions.RightBack)
			{
				return this.targetDock.rightBackTransform;
			}
			break;
		}
		throw new ArgumentOutOfRangeException();
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x0002EC54 File Offset: 0x0002CE54
	public Transform GetTargetDock(TransferrableObject.PositionState state, VRRig rig)
	{
		BodyDockPositions myBodyDockPositions = rig.myBodyDockPositions;
		VRRigAnchorOverrides component = rig.GetComponent<VRRigAnchorOverrides>();
		if (state <= TransferrableObject.PositionState.InRightHand)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				return component.AnchorOverride(this.currentState, myBodyDockPositions.leftArmTransform);
			case TransferrableObject.PositionState.OnRightArm:
				return component.AnchorOverride(this.currentState, myBodyDockPositions.rightArmTransform);
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				return component.AnchorOverride(this.currentState, myBodyDockPositions.leftHandTransform);
			default:
				if (state == TransferrableObject.PositionState.InRightHand)
				{
					return component.AnchorOverride(this.currentState, myBodyDockPositions.rightHandTransform);
				}
				break;
			}
		}
		else
		{
			if (state == TransferrableObject.PositionState.OnChest)
			{
				return component.AnchorOverride(this.currentState, myBodyDockPositions.chestTransform);
			}
			if (state == TransferrableObject.PositionState.OnLeftShoulder)
			{
				return component.AnchorOverride(this.currentState, myBodyDockPositions.leftBackTransform);
			}
			if (state == TransferrableObject.PositionState.OnRightShoulder)
			{
				return component.AnchorOverride(this.currentState, myBodyDockPositions.rightBackTransform);
			}
		}
		return null;
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0002ED30 File Offset: 0x0002CF30
	private void UpdateFollowXform()
	{
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState == TransferrableObject.PositionState.None)
		{
			if (this.previousState != TransferrableObject.PositionState.None)
			{
				this.ResetToHome();
			}
			return;
		}
		if (positionState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		if (this.previousState != this.currentState)
		{
			this.transferrableItemSlotTransformOverrideCachedMatrix = null;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
		}
		if (!this.targetRigSet)
		{
			return;
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		try
		{
			positionState = this.currentState;
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
		}
		catch
		{
			Debug.LogError("anchorOverrides or targetDock has been destroyed", this);
			this.SetTargetRig(null);
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped && this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		if (this.detatchOnGrab && (this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand))
		{
			base.transform.parent = null;
		}
		if (this.interpState == TransferrableObject.InterpolateState.None)
		{
			try
			{
				if (transform == null)
				{
					return;
				}
				this.startInterpolation |= (transform2 != transform.parent);
			}
			catch
			{
			}
			if (!this.startInterpolation && !this.isGrabAnchorSet && base.transform.parent != transform && transform != base.transform)
			{
				this.startInterpolation = true;
			}
			if (this.startInterpolation)
			{
				Vector3 position = base.transform.position;
				Quaternion rotation = base.transform.rotation;
				if (base.transform.parent != transform && transform != base.transform)
				{
					base.transform.parent = transform;
				}
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				if (this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					if (this.flipOnXForLeftHand)
					{
						transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					else if (this.flipOnYForLeftHand)
					{
						transform.localScale = new Vector3(1f, -1f, 1f);
					}
					else
					{
						transform.localScale = Vector3.one;
					}
				}
				else
				{
					transform.localScale = Vector3.one;
				}
				if (Time.frameCount == this.enabledOnFrame || Time.frameCount == this.enabledOnFrame + 1)
				{
					Matrix4x4 rhs = this.GetDefaultTransformationMatrix();
					if ((this.currentState != TransferrableObject.PositionState.InLeftHand || !(this.handPoseLeft != null)) && this.currentState == TransferrableObject.PositionState.InRightHand)
					{
						this.handPoseRight != null;
					}
					Matrix4x4 matrix4x;
					if (this.transferrableItemSlotTransformOverride && this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x))
					{
						rhs = matrix4x;
					}
					Matrix4x4 matrix = transform.localToWorldMatrix * rhs;
					base.transform.SetLocalToWorldMatrixNoScale(matrix);
				}
				else
				{
					this.interpState = TransferrableObject.InterpolateState.Interpolating;
					this.interpDt = this.interpTime;
					this.interpStartRot = rotation;
					this.interpStartPos = position;
					base.transform.position = position;
					base.transform.rotation = rotation;
				}
				this.startInterpolation = false;
			}
		}
		if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
		{
			Matrix4x4 rhs2 = this.GetDefaultTransformationMatrix();
			if (this.transferrableItemSlotTransformOverride != null)
			{
				if (this.transferrableItemSlotTransformOverrideCachedMatrix == null)
				{
					Matrix4x4 value;
					this.transferrableItemSlotTransformOverrideApplicable = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out value);
					this.transferrableItemSlotTransformOverrideCachedMatrix = new Matrix4x4?(value);
				}
				if (this.transferrableItemSlotTransformOverrideApplicable)
				{
					rhs2 = this.transferrableItemSlotTransformOverrideCachedMatrix.Value;
				}
			}
			float t = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			Mathf.SmoothStep(0f, 1f, t);
			Matrix4x4 matrix2 = transform.localToWorldMatrix * rhs2;
			Transform transform3 = base.transform;
			Vector3 vector = matrix2.Position();
			transform3.position = this.interpStartPos.LerpToUnclamped(vector, t);
			base.transform.rotation = Quaternion.Slerp(this.interpStartRot, matrix2.Rotation(), t);
			base.transform.localScale = rhs2.lossyScale;
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = TransferrableObject.InterpolateState.None;
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
				matrix2 = transform.localToWorldMatrix * rhs2;
				base.transform.SetLocalToWorldMatrixNoScale(matrix2);
				base.transform.localScale = rhs2.lossyScale;
			}
		}
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x0002F374 File Offset: 0x0002D574
	public virtual void DropItem()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
		this.currentState = TransferrableObject.PositionState.Dropped;
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.transferableObjectState = this.currentState;
		}
		if (this.canDrop)
		{
			base.transform.parent = null;
			if (this.anchor)
			{
				this.anchor.parent = this.InitialDockObject;
			}
			if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.isKinematic = true;
			}
		}
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x0002F4A4 File Offset: 0x0002D6A4
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		if (this.isSceneObject)
		{
			this.disableItem = false;
		}
		else
		{
			for (int i = 0; i < this.ownerRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.ownerRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
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
		}
		if (this.previousState != this.currentState && this.detatchOnGrab && this.InHand())
		{
			base.transform.parent = null;
			if (!this.ShouldBeKinematic() && this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.isKinematic = false;
			}
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped)
		{
			this.UpdateFollowXform();
		}
		else if (this.canDrop)
		{
			if (base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			try
			{
				if (this.anchor != null && this.anchor.parent != this.InitialDockObject)
				{
					this.anchor.parent = this.InitialDockObject;
				}
			}
			catch
			{
			}
		}
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.rigidbodyInstance.isKinematic != this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
			if (this.worldShareableInstance)
			{
				if (this.currentState == TransferrableObject.PositionState.Dropped)
				{
					this.worldShareableInstance.EnableRemoteSync = true;
					return;
				}
				this.worldShareableInstance.EnableRemoteSync = !this.ShouldBeKinematic();
			}
		}
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x0002F644 File Offset: 0x0002D844
	public virtual void ResetToHome()
	{
		if (this.isSceneObject)
		{
			this.currentState = TransferrableObject.PositionState.None;
		}
		this.ResetXf();
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x0002F690 File Offset: 0x0002D890
	protected void ResetXf()
	{
		if (!this.positionInitialized)
		{
			this.initOffset = base.transform.localPosition;
			this.initRotation = base.transform.localRotation;
		}
		if (this.canDrop || this.allowWorldSharableInstance)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			if (this.InitialDockObject)
			{
				this.anchor.localPosition = Vector3.zero;
				this.anchor.localRotation = Quaternion.identity;
				this.anchor.localScale = Vector3.one;
			}
			if (this.grabAnchor)
			{
				if (this.grabAnchor.parent != base.transform)
				{
					this.grabAnchor.parent = base.transform;
				}
				this.grabAnchor.localPosition = Vector3.zero;
				this.grabAnchor.localRotation = Quaternion.identity;
				this.grabAnchor.localScale = Vector3.one;
			}
			if (this.transferrableItemSlotTransformOverride)
			{
				Transform transformFromPositionState = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState);
				if (transformFromPositionState)
				{
					base.transform.position = transformFromPositionState.position;
					base.transform.rotation = transformFromPositionState.rotation;
					return;
				}
			}
			else
			{
				base.transform.SetLocalMatrixRelativeToParent(this.GetDefaultTransformationMatrix());
			}
		}
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x0002F80C File Offset: 0x0002DA0C
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		this.ResetXf();
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x0002F864 File Offset: 0x0002DA64
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
			array = this.gameObjectsActiveOnlyWhileDocked;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			return;
		}
		array = this.gameObjectsActiveOnlyWhileHeld;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		array = this.gameObjectsActiveOnlyWhileDocked;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
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

	// Token: 0x0600076B RID: 1899 RVA: 0x0002F98B File Offset: 0x0002DB8B
	protected virtual void LocalMyObjectValidation()
	{
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x0002F990 File Offset: 0x0002DB90
	protected virtual void LocalPersistanceValidation()
	{
		if (this.maxDistanceFromOriginBeforeRespawn != 0f && Vector3.Distance(base.transform.position, this.originPoint.position) > this.maxDistanceFromOriginBeforeRespawn)
		{
			if (this.audioSrc != null && this.resetPositionAudioClip != null)
			{
				this.audioSrc.PlayOneShot(this.resetPositionAudioClip);
			}
			Debug.Log("Too far from origin, Resetting position", this);
			if (this.currentState != TransferrableObject.PositionState.Dropped)
			{
				this.DropItem();
				this.currentState = TransferrableObject.PositionState.Dropped;
				this.UpdateFollowXform();
			}
			base.transform.position = this.originPoint.position;
			this.rigidbodyInstance.velocity = Vector3.zero;
		}
		if (this.rigidbodyInstance && this.rigidbodyInstance.velocity.sqrMagnitude > 10000f)
		{
			Debug.Log("Moving too fast, Assuming ive fallen out of the map. Ressetting position", this);
			this.ResetToHome();
		}
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x0002FA90 File Offset: 0x0002DC90
	public void ObjectBeingTaken()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x0002FB38 File Offset: 0x0002DD38
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		this.LocalPersistanceValidation();
		if (PhotonNetwork.InRoom)
		{
			if (!this.isSceneObject && this.IsLocalObject())
			{
				this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
				this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
				this.myRig.SetTransferrableDockPosition(this.objectIndex, this.storedZone);
			}
			if (this.worldShareableInstance != null)
			{
				this.worldShareableInstance.transferableObjectState = this.currentState;
				this.worldShareableInstance.transferableObjectItemState = this.itemState;
			}
		}
		this.HandleLocalInput();
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x0002FBEC File Offset: 0x0002DDEC
	protected void LateUpdateReplicatedSceneObject()
	{
		if (this.myOnlineRig != null)
		{
			this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		}
		if (this.worldShareableInstance != null)
		{
			this.currentState = this.worldShareableInstance.transferableObjectState;
			this.itemState = this.worldShareableInstance.transferableObjectItemState;
			this.worldShareableInstance.EnableRemoteSync = (!this.ShouldBeKinematic() || this.currentState == TransferrableObject.PositionState.Dropped);
		}
		if (this.isRigidbodySet && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0002FC90 File Offset: 0x0002DE90
	protected virtual void LateUpdateReplicated()
	{
		if (this.isSceneObject || this.shareable)
		{
			this.LateUpdateReplicatedSceneObject();
			return;
		}
		if (this.myOnlineRig == null)
		{
			return;
		}
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (this.currentState == TransferrableObject.PositionState.Dropped && !this.canDrop && !this.shareable)
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.SetActive(false);
			}
			this.currentState = this.previousState;
		}
		if (this.isRigidbodySet)
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
		}
		bool flag = true;
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		int num = this.myOnlineRig.ActiveTransferrableObjectIndexLength();
		for (int i = 0; i < num; i++)
		{
			if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				flag = false;
				GameObject[] array = this.gameObjectsActiveOnlyWhileHeld;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].SetActive(this.InHand());
				}
				array = this.gameObjectsActiveOnlyWhileDocked;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].SetActive(!this.InHand());
				}
			}
		}
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0002FDEC File Offset: 0x0002DFEC
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		if (!this.IsLocalObject() && this.worldShareableInstance && !this.isSceneObject)
		{
			if (this.IsMyItem())
			{
				return;
			}
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
			{
			});
		}
		this.ResetXf();
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x0002FE70 File Offset: 0x0002E070
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.worldShareableInstance == null) && !this.worldShareableInstance.guard.isTrulyMine)
		{
			if (!this.IsGrabbable())
			{
				return;
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
		if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState != TransferrableObject.PositionState.OnLeftArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabLeft = false;
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InLeftHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InLeftHand, EquipmentInteractor.instance.leftHand.transform, this.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, true);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabRight = false;
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InRightHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InRightHand, EquipmentInteractor.instance.rightHand.transform, this.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, false);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		if (this.rigidbodyInstance && !this.rigidbodyInstance.isKinematic && this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x000300A0 File Offset: 0x0002E2A0
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
			bool flag3 = false;
			this.interpState = TransferrableObject.InterpolateState.None;
			if (this.isSceneObject || this.canDrop || this.allowWorldSharableInstance)
			{
				if (!this.rigidbodyInstance)
				{
					return;
				}
				if (this.worldShareableInstance)
				{
					this.worldShareableInstance.EnableRemoteSync = true;
				}
				if (!flag3)
				{
					this.currentState = TransferrableObject.PositionState.Dropped;
				}
				if (this.rigidbodyInstance.isKinematic && !this.ShouldBeKinematic())
				{
					this.rigidbodyInstance.isKinematic = false;
				}
				GorillaVelocityEstimator component = base.GetComponent<GorillaVelocityEstimator>();
				if (component != null && this.rigidbodyInstance != null)
				{
					this.rigidbodyInstance.velocity = component.linearVelocity;
					this.rigidbodyInstance.angularVelocity = component.angularVelocity;
				}
			}
			else
			{
				bool flag4 = this.allowWorldSharableInstance;
			}
			this.DropItemCleanup();
			EquipmentInteractor.instance.ForceDropEquipment(this);
		}
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00030290 File Offset: 0x0002E490
	public override void DropItemCleanup()
	{
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
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

	// Token: 0x06000775 RID: 1909 RVA: 0x00030300 File Offset: 0x0002E500
	public virtual void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsGrabbable())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00030364 File Offset: 0x0002E564
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		if (this.myRig.photonView != null)
		{
			this.myRig.photonView.RPC("PlayHandTap", RpcTarget.Others, new object[]
			{
				soundIndex,
				flag,
				0.1f
			});
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x000303E5 File Offset: 0x0002E5E5
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x000303E7 File Offset: 0x0002E5E7
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x000303F9 File Offset: 0x0002E5F9
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x000303FC File Offset: 0x0002E5FC
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x000303FF File Offset: 0x0002E5FF
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00030408 File Offset: 0x0002E608
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00030411 File Offset: 0x0002E611
	public virtual bool IsMyItem()
	{
		return GorillaTagger.Instance == null || (this.targetRig != null && this.targetRig == GorillaTagger.Instance.offlineVRRig);
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0003043B File Offset: 0x0002E63B
	protected virtual bool IsHeld()
	{
		return EquipmentInteractor.instance != null && (EquipmentInteractor.instance.leftHandHeldEquipment == this || EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00030470 File Offset: 0x0002E670
	public virtual bool IsGrabbable()
	{
		return this.IsMyItem() || ((this.isSceneObject || this.shareable) && (this.isSceneObject || this.shareable) && (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None));
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x000304CF File Offset: 0x0002E6CF
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x000304E5 File Offset: 0x0002E6E5
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x000304F4 File Offset: 0x0002E6F4
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x000304FF File Offset: 0x0002E6FF
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x0003050A File Offset: 0x0002E70A
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00030516 File Offset: 0x0002E716
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0003052E File Offset: 0x0002E72E
	protected Player OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.photonView.Owner;
		}
		return PhotonNetwork.LocalPlayer;
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x00030554 File Offset: 0x0002E754
	public virtual void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		if (toPlayer != null && toPlayer.Equals(fromPlayer))
		{
			return;
		}
		if (object.Equals(fromPlayer, PhotonNetwork.LocalPlayer) && this.IsHeld())
		{
			this.DropItem();
		}
		if (toPlayer == null)
		{
			this.SetTargetRig(null);
			return;
		}
		if (!this.shareable && !this.isSceneObject)
		{
			return;
		}
		if (object.Equals(toPlayer, PhotonNetwork.LocalPlayer))
		{
			if (GorillaTagger.Instance == null)
			{
				Debug.LogError("OnOwnershipTransferred has been initiated too quickly, The local player is not ready");
				return;
			}
			this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
			return;
		}
		else
		{
			VRRig exists = GorillaGameManager.StaticFindRigForPlayer(toPlayer);
			if (!exists)
			{
				Debug.LogError("failed to find target rig for ownershiptransfer");
				return;
			}
			this.SetTargetRig(exists);
			return;
		}
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x000305FC File Offset: 0x0002E7FC
	public bool OnOwnershipRequest(Player fromPlayer)
	{
		if (GorillaGameManager.instance)
		{
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(fromPlayer);
			if (vrrig)
			{
				return true;
			}
			if (Vector3.SqrMagnitude(base.transform.position - vrrig.transform.position) > 16f)
			{
				Debug.Log("Player whos trying to get is too far, Denying takeover");
				return false;
			}
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return true;
		}
		if (this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.allowPlayerStealing)
		{
			return true;
		}
		if (this.isSceneObject)
		{
			return false;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x000306C4 File Offset: 0x0002E8C4
	public bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer)
	{
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(fromPlayer);
		if (vrrig == null)
		{
			return true;
		}
		if (Vector3.SqrMagnitude(base.transform.position - vrrig.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return true;
		}
		if (this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x0003076C File Offset: 0x0002E96C
	public void OnMyOwnerLeft()
	{
		if (this.currentState == TransferrableObject.PositionState.None || this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		this.DropItem();
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			this.anchor.localPosition = Vector3.zero;
			this.anchor.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x000307D3 File Offset: 0x0002E9D3
	public void OnMyCreatorLeft()
	{
		this.OnItemDestroyedOrDisabled();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x040008E2 RID: 2274
	public VRRig myRig;

	// Token: 0x040008E3 RID: 2275
	public VRRig myOnlineRig;

	// Token: 0x040008E4 RID: 2276
	public bool latched;

	// Token: 0x040008E5 RID: 2277
	private float indexTrigger;

	// Token: 0x040008E6 RID: 2278
	public bool testActivate;

	// Token: 0x040008E7 RID: 2279
	public bool testDeactivate;

	// Token: 0x040008E8 RID: 2280
	public float myThreshold = 0.8f;

	// Token: 0x040008E9 RID: 2281
	public float hysterisis = 0.05f;

	// Token: 0x040008EA RID: 2282
	public bool flipOnXForLeftHand;

	// Token: 0x040008EB RID: 2283
	public bool flipOnYForLeftHand;

	// Token: 0x040008EC RID: 2284
	public bool flipOnXForLeftArm;

	// Token: 0x040008ED RID: 2285
	public bool disableStealing;

	// Token: 0x040008EE RID: 2286
	public bool allowPlayerStealing;

	// Token: 0x040008EF RID: 2287
	private TransferrableObject.PositionState initState;

	// Token: 0x040008F0 RID: 2288
	public TransferrableObject.ItemStates itemState;

	// Token: 0x040008F1 RID: 2289
	[DevInspectorShow]
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x040008F2 RID: 2290
	protected TransferrableObject.PositionState previousState;

	// Token: 0x040008F3 RID: 2291
	[DevInspectorYellow]
	[DevInspectorShow]
	public TransferrableObject.PositionState currentState;

	// Token: 0x040008F4 RID: 2292
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x040008F5 RID: 2293
	[DevInspectorCyan]
	[DevInspectorShow]
	public AdvancedItemState advancedGrabState;

	// Token: 0x040008F6 RID: 2294
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig targetRig;

	// Token: 0x040008F7 RID: 2295
	public bool targetRigSet;

	// Token: 0x040008F8 RID: 2296
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig ownerRig;

	// Token: 0x040008F9 RID: 2297
	[DevInspectorShow]
	[DevInspectorCyan]
	public BodyDockPositions targetDock;

	// Token: 0x040008FA RID: 2298
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x040008FB RID: 2299
	public bool canAutoGrabLeft;

	// Token: 0x040008FC RID: 2300
	public bool canAutoGrabRight;

	// Token: 0x040008FD RID: 2301
	[DevInspectorShow]
	public int objectIndex;

	// Token: 0x040008FE RID: 2302
	[Tooltip("In Holdables.prefab, assign to the parent of this transform.\nExample: 'Holdables/YellowHandBootsRight' is the anchor of 'Holdables/YellowHandBootsRight/YELLOW HAND BOOTS'")]
	public Transform anchor;

	// Token: 0x040008FF RID: 2303
	[Tooltip("In core prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x04000900 RID: 2304
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x04000901 RID: 2305
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Left mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseLeft;

	// Token: 0x04000902 RID: 2306
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Right mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseRight;

	// Token: 0x04000903 RID: 2307
	public bool isGrabAnchorSet;

	// Token: 0x04000904 RID: 2308
	private static Vector3 handPoseRightReferencePoint = new Vector3(-0.0141f, 0.0065f, -0.278f);

	// Token: 0x04000905 RID: 2309
	private static Quaternion handPoseRightReferenceRotation = Quaternion.Euler(-2.058f, -17.2f, 65.05f);

	// Token: 0x04000906 RID: 2310
	private static Vector3 handPoseLeftReferencePoint = new Vector3(0.0136f, 0.0045f, -0.2809f);

	// Token: 0x04000907 RID: 2311
	private static Quaternion handPoseLeftReferenceRotation = Quaternion.Euler(-0.58f, 21.356f, -63.965f);

	// Token: 0x04000908 RID: 2312
	public TransferrableItemSlotTransformOverride transferrableItemSlotTransformOverride;

	// Token: 0x04000909 RID: 2313
	public int myIndex;

	// Token: 0x0400090A RID: 2314
	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x0400090B RID: 2315
	public GameObject[] gameObjectsActiveOnlyWhileDocked;

	// Token: 0x0400090C RID: 2316
	[SerializeField]
	protected internal WorldShareableItem worldShareableInstance;

	// Token: 0x0400090D RID: 2317
	private float interpTime = 0.2f;

	// Token: 0x0400090E RID: 2318
	private float interpDt;

	// Token: 0x0400090F RID: 2319
	private Vector3 interpStartPos;

	// Token: 0x04000910 RID: 2320
	private Quaternion interpStartRot;

	// Token: 0x04000911 RID: 2321
	protected int enabledOnFrame = -1;

	// Token: 0x04000912 RID: 2322
	protected Vector3 initOffset;

	// Token: 0x04000913 RID: 2323
	protected Quaternion initRotation;

	// Token: 0x04000914 RID: 2324
	private Matrix4x4 initMatrix = Matrix4x4.identity;

	// Token: 0x04000915 RID: 2325
	private Matrix4x4 leftHandMatrix = Matrix4x4.identity;

	// Token: 0x04000916 RID: 2326
	private Matrix4x4 rightHandMatrix = Matrix4x4.identity;

	// Token: 0x04000917 RID: 2327
	private bool positionInitialized;

	// Token: 0x04000918 RID: 2328
	public bool isSceneObject;

	// Token: 0x04000919 RID: 2329
	public Rigidbody rigidbodyInstance;

	// Token: 0x0400091A RID: 2330
	public bool isRigidbodySet;

	// Token: 0x0400091B RID: 2331
	public bool canDrop;

	// Token: 0x0400091C RID: 2332
	public bool shareable;

	// Token: 0x0400091D RID: 2333
	public bool detatchOnGrab;

	// Token: 0x0400091E RID: 2334
	public bool allowWorldSharableInstance;

	// Token: 0x0400091F RID: 2335
	[ItemCanBeNull]
	public Transform originPoint;

	// Token: 0x04000920 RID: 2336
	[ItemCanBeNull]
	public float maxDistanceFromOriginBeforeRespawn;

	// Token: 0x04000921 RID: 2337
	public AudioClip resetPositionAudioClip;

	// Token: 0x04000922 RID: 2338
	public float maxDistanceFromTargetPlayerBeforeRespawn;

	// Token: 0x04000923 RID: 2339
	private bool wasHover;

	// Token: 0x04000924 RID: 2340
	private bool isHover;

	// Token: 0x04000925 RID: 2341
	private bool disableItem;

	// Token: 0x04000926 RID: 2342
	protected bool loaded;

	// Token: 0x04000927 RID: 2343
	public const int kPositionStateCount = 8;

	// Token: 0x04000928 RID: 2344
	[DevInspectorShow]
	public TransferrableObject.InterpolateState interpState;

	// Token: 0x04000929 RID: 2345
	public bool startInterpolation;

	// Token: 0x0400092A RID: 2346
	public Transform InitialDockObject;

	// Token: 0x0400092B RID: 2347
	private AudioSource audioSrc;

	// Token: 0x0400092D RID: 2349
	protected Transform _defaultAnchor;

	// Token: 0x0400092E RID: 2350
	protected bool _isDefaultAnchorSet;

	// Token: 0x0400092F RID: 2351
	private Matrix4x4? transferrableItemSlotTransformOverrideCachedMatrix;

	// Token: 0x04000930 RID: 2352
	private bool transferrableItemSlotTransformOverrideApplicable;

	// Token: 0x02000404 RID: 1028
	public enum ItemStates
	{
		// Token: 0x04001CB0 RID: 7344
		State0 = 1,
		// Token: 0x04001CB1 RID: 7345
		State1,
		// Token: 0x04001CB2 RID: 7346
		State2 = 4,
		// Token: 0x04001CB3 RID: 7347
		State3 = 8,
		// Token: 0x04001CB4 RID: 7348
		State4 = 16
	}

	// Token: 0x02000405 RID: 1029
	[Flags]
	public enum PositionState
	{
		// Token: 0x04001CB6 RID: 7350
		OnLeftArm = 1,
		// Token: 0x04001CB7 RID: 7351
		OnRightArm = 2,
		// Token: 0x04001CB8 RID: 7352
		InLeftHand = 4,
		// Token: 0x04001CB9 RID: 7353
		InRightHand = 8,
		// Token: 0x04001CBA RID: 7354
		OnChest = 16,
		// Token: 0x04001CBB RID: 7355
		OnLeftShoulder = 32,
		// Token: 0x04001CBC RID: 7356
		OnRightShoulder = 64,
		// Token: 0x04001CBD RID: 7357
		Dropped = 128,
		// Token: 0x04001CBE RID: 7358
		None = 0
	}

	// Token: 0x02000406 RID: 1030
	public enum InterpolateState
	{
		// Token: 0x04001CC0 RID: 7360
		None,
		// Token: 0x04001CC1 RID: 7361
		Interpolating
	}
}
