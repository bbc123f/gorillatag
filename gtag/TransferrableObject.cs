using System;
using GorillaExtensions;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR;

public class TransferrableObject : HoldableObject, ISelfValidator, IRequestableOwnershipGuardCallbacks, IPreDisable
{
	public enum ItemStates
	{
		State0 = 1,
		State1 = 2,
		State2 = 4,
		State3 = 8,
		State4 = 0x10
	}

	[Flags]
	public enum PositionState
	{
		OnLeftArm = 1,
		OnRightArm = 2,
		InLeftHand = 4,
		InRightHand = 8,
		OnChest = 0x10,
		OnLeftShoulder = 0x20,
		OnRightShoulder = 0x40,
		Dropped = 0x80,
		None = 0
	}

	public enum InterpolateState
	{
		None,
		Interpolating
	}

	public VRRig myRig;

	public VRRig myOnlineRig;

	public bool latched;

	private float indexTrigger;

	public bool testActivate;

	public bool testDeactivate;

	public float myThreshold = 0.8f;

	public float hysterisis = 0.05f;

	public bool flipOnXForLeftHand;

	public bool flipOnYForLeftHand;

	public bool flipOnXForLeftArm;

	public bool disableStealing;

	private PositionState initState;

	public ItemStates itemState;

	[DevInspectorShow]
	public BodyDockPositions.DropPositions storedZone;

	protected PositionState previousState;

	[DevInspectorYellow]
	[DevInspectorShow]
	public PositionState currentState;

	public BodyDockPositions.DropPositions dockPositions;

	[DevInspectorCyan]
	[DevInspectorShow]
	public AdvancedItemState advancedGrabState;

	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig targetRig;

	public bool targetRigSet;

	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig ownerRig;

	[DevInspectorShow]
	[DevInspectorCyan]
	public BodyDockPositions targetDock;

	private VRRigAnchorOverrides anchorOverrides;

	public bool canAutoGrabLeft;

	public bool canAutoGrabRight;

	[DevInspectorShow]
	public int objectIndex;

	[Tooltip("In Holdables.prefab, assign to the parent of this transform.\nExample: 'Holdables/YellowHandBootsRight' is the anchor of 'Holdables/YellowHandBootsRight/YELLOW HAND BOOTS'")]
	public Transform anchor;

	[Tooltip("In core prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	public bool isGrabAnchorSet;

	public TransferrableItemSlotTransformOverride transferrableItemSlotTransformOverride;

	public int myIndex;

	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	[SerializeField]
	protected internal WorldShareableItem worldShareableInstance;

	private float interpTime = 0.2f;

	private float interpDt;

	private Vector3 interpStartPos;

	private Quaternion interpStartRot;

	protected int enabledOnFrame = -1;

	protected Vector3 initOffset;

	protected Quaternion initRotation;

	private Matrix4x4 initMatrix = Matrix4x4.identity;

	private bool positionInitialized;

	public bool isSceneObject;

	public Rigidbody rigidbodyInstance;

	public bool isRigidbodySet;

	public bool canDrop;

	public bool shareable;

	public bool detatchOnGrab;

	public bool allowWorldSharableInstance;

	[ItemCanBeNull]
	public Transform originPoint;

	[ItemCanBeNull]
	public float maxDistanceFromOriginBeforeRespawn;

	public AudioClip resetPositionAudioClip;

	public float maxDistanceFromTargetPlayerBeforeRespawn;

	private bool wasHover;

	private bool isHover;

	private bool disableItem;

	protected bool loaded;

	public const int kPositionStateCount = 8;

	[DevInspectorShow]
	public InterpolateState interpState;

	public bool startInterpolation;

	public Transform InitialDockObject;

	private AudioSource audioSrc;

	protected Transform _defaultAnchor;

	protected bool _isDefaultAnchorSet;

	private Matrix4x4? transferrableItemSlotTransformOverrideCachedMatrix;

	private bool transferrableItemSlotTransformOverrideApplicable;

	protected bool appIsQuitting { get; private set; }

	public void FixTransformOverride()
	{
		transferrableItemSlotTransformOverride = GetComponent<TransferrableItemSlotTransformOverride>();
	}

	public void Validate(SelfValidationResult result)
	{
	}

	public void SetTargetRig(VRRig rig)
	{
		if (rig == null)
		{
			targetRigSet = false;
			if (isSceneObject)
			{
				targetRig = rig;
				targetDock = null;
				anchorOverrides = null;
				return;
			}
			if ((bool)myRig)
			{
				SetTargetRig(myRig);
			}
			if ((bool)myOnlineRig)
			{
				SetTargetRig(myOnlineRig);
			}
			return;
		}
		targetRigSet = true;
		targetRig = rig;
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
		anchorOverrides = component2;
		targetDock = component;
		if (interpState == InterpolateState.Interpolating)
		{
			interpState = InterpolateState.None;
		}
	}

	protected virtual void Awake()
	{
		if (rigidbodyInstance == null)
		{
			rigidbodyInstance = GetComponent<Rigidbody>();
		}
		if (rigidbodyInstance != null)
		{
			isRigidbodySet = true;
		}
		audioSrc = GetComponent<AudioSource>();
		latched = false;
		if (!positionInitialized)
		{
			initMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
			positionInitialized = true;
		}
		if (anchor == null)
		{
			InitialDockObject = base.transform.parent;
		}
		else
		{
			InitialDockObject = anchor.parent;
		}
		isGrabAnchorSet = grabAnchor != null;
		Application.quitting += delegate
		{
			appIsQuitting = true;
		};
	}

	private void SetInitMatrix()
	{
		initMatrix = Matrix4x4.Inverse(base.transform.parent.localToWorldMatrix) * base.transform.localToWorldMatrix;
		positionInitialized = true;
	}

	protected virtual void Start()
	{
	}

	public override void OnEnable()
	{
		base.OnEnable();
		transferrableItemSlotTransformOverride = GetComponent<TransferrableItemSlotTransformOverride>();
		if (!positionInitialized)
		{
			initMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
			positionInitialized = true;
		}
		if (isSceneObject)
		{
			if (!worldShareableInstance)
			{
				Debug.LogError("Missing Sharable Instance on Scene enabled object");
				return;
			}
			worldShareableInstance.SyncToSceneObject(this);
			worldShareableInstance.GetComponent<RequestableOwnershipGuard>().AddCallbackTarget(this);
			return;
		}
		if (!myRig && (bool)myOnlineRig)
		{
			ownerRig = myOnlineRig;
			SetTargetRig(myOnlineRig);
		}
		if (myRig == null && myOnlineRig == null)
		{
			if (!isSceneObject)
			{
				base.gameObject.SetActive(value: false);
			}
			return;
		}
		objectIndex = targetDock.ReturnTransferrableItemIndex(myIndex);
		if (currentState == PositionState.OnLeftArm)
		{
			storedZone = BodyDockPositions.DropPositions.LeftArm;
		}
		else if (currentState == PositionState.OnRightArm)
		{
			storedZone = BodyDockPositions.DropPositions.RightArm;
		}
		else if (currentState == PositionState.OnLeftShoulder)
		{
			storedZone = BodyDockPositions.DropPositions.LeftBack;
		}
		else if (currentState == PositionState.OnRightShoulder)
		{
			storedZone = BodyDockPositions.DropPositions.RightBack;
		}
		else if (currentState == PositionState.OnChest)
		{
			storedZone = BodyDockPositions.DropPositions.Chest;
		}
		if (IsLocalObject())
		{
			ownerRig = GorillaTagger.Instance.offlineVRRig;
			SetTargetRig(GorillaTagger.Instance.offlineVRRig);
		}
		if (objectIndex == -1)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		if (currentState == PositionState.OnLeftArm && flipOnXForLeftArm)
		{
			Transform transform = GetAnchor(currentState);
			transform.localScale = new Vector3(0f - transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		initState = currentState;
		enabledOnFrame = Time.frameCount;
		startInterpolation = true;
		if (PhotonNetwork.InRoom && (canDrop || shareable))
		{
			SpawnTransferableObjectViews();
			if ((bool)myRig && myRig != null && worldShareableInstance != null)
			{
				OnWorldShareableItemSpawn();
			}
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (appIsQuitting)
		{
			return;
		}
		enabledOnFrame = -1;
		if (!isSceneObject && IsLocalObject() && (bool)worldShareableInstance && !IsMyItem())
		{
			worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
			{
			});
		}
		if ((bool)worldShareableInstance)
		{
			worldShareableInstance.Invalidate();
			worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if ((bool)targetDock)
			{
				targetDock.DeallocateSharableInstance(worldShareableInstance);
			}
			worldShareableInstance = null;
		}
		PlayDestroyedOrDisabledEffect();
	}

	public void CleanupDisable()
	{
		currentState = PositionState.None;
		enabledOnFrame = -1;
		if ((bool)anchor)
		{
			anchor.parent = InitialDockObject;
			if (anchor != base.transform)
			{
				base.transform.parent = anchor;
			}
		}
		else
		{
			base.transform.parent = anchor;
		}
		interpState = InterpolateState.None;
		Transform obj = base.transform;
		Matrix4x4 matrix4X = GetDefaultTransformationMatrix();
		obj.SetLocalMatrixRelativeToParentWithXParity(in matrix4X);
	}

	public virtual void PreDisable()
	{
		itemState = ItemStates.State0;
		currentState = PositionState.None;
		interpState = InterpolateState.None;
		ResetToDefaultState();
	}

	public virtual Matrix4x4 GetDefaultTransformationMatrix()
	{
		return initMatrix;
	}

	public virtual bool ShouldBeKinematic()
	{
		if (detatchOnGrab)
		{
			if (currentState == PositionState.Dropped || currentState == PositionState.InLeftHand || currentState == PositionState.InRightHand)
			{
				return false;
			}
			return true;
		}
		if (currentState == PositionState.Dropped)
		{
			return false;
		}
		return true;
	}

	private void SpawnShareableObject()
	{
		if (isSceneObject)
		{
			if (!(worldShareableInstance == null))
			{
				worldShareableInstance.GetComponent<WorldShareableItem>().SetupSceneObjectOnNetwork(PhotonNetwork.MasterClient);
			}
		}
		else if (PhotonNetwork.InRoom)
		{
			SpawnTransferableObjectViews();
			if ((bool)myRig && (canDrop || shareable) && myRig != null && worldShareableInstance != null)
			{
				OnWorldShareableItemSpawn();
			}
		}
	}

	public void SpawnTransferableObjectViews()
	{
		Debug.Log("SpawnTransferableObjectViews on gameobject " + base.gameObject.name);
		Player owner = PhotonNetwork.LocalPlayer;
		if (!ownerRig.isOfflineVRRig)
		{
			owner = ownerRig.creator;
		}
		if ((object)worldShareableInstance == null)
		{
			worldShareableInstance = targetDock.AllocateSharableInstance(storedZone, owner);
		}
		GorillaTagger.OnPlayerSpawned(delegate
		{
			worldShareableInstance.SetupSharableObject(myIndex, owner, base.transform);
		});
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (isSceneObject)
		{
			_ = worldShareableInstance == null;
		}
		else if (PhotonNetwork.InRoom && (canDrop || shareable))
		{
			SpawnTransferableObjectViews();
			if ((bool)myRig && myRig != null && worldShareableInstance != null)
			{
				OnWorldShareableItemSpawn();
			}
		}
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		if (isSceneObject || (!shareable && !allowWorldSharableInstance && !canDrop))
		{
			return;
		}
		if (base.gameObject.activeSelf && (bool)worldShareableInstance)
		{
			worldShareableInstance.Invalidate();
			worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if ((bool)targetDock)
			{
				targetDock.DeallocateSharableInstance(worldShareableInstance);
			}
			else
			{
				worldShareableInstance.ResetViews();
				ObjectPools.instance.Destroy(worldShareableInstance.gameObject);
			}
			worldShareableInstance = null;
		}
		if (!IsLocalObject())
		{
			OnItemDestroyedOrDisabled();
			base.gameObject.Disable();
		}
	}

	public bool IsLocalObject()
	{
		if ((object)myRig != null)
		{
			return myRig.isOfflineVRRig;
		}
		return false;
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		_ = isSceneObject;
	}

	public void SetWorldShareableItem(WorldShareableItem item)
	{
		worldShareableInstance = item;
		OnWorldShareableItemSpawn();
	}

	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	protected virtual void PlayDestroyedOrDisabledEffect()
	{
	}

	protected virtual void OnItemDestroyedOrDisabled()
	{
		if ((bool)worldShareableInstance)
		{
			worldShareableInstance.Invalidate();
			worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if ((bool)targetDock)
			{
				targetDock.DeallocateSharableInstance(worldShareableInstance);
			}
			worldShareableInstance = null;
		}
		PlayDestroyedOrDisabledEffect();
		enabledOnFrame = -1;
		currentState = PositionState.None;
	}

	protected virtual void LateUpdate()
	{
		if (IsLocalObject() && canDrop)
		{
			LocalMyObjectValidation();
		}
		if (IsMyItem())
		{
			LateUpdateLocal();
		}
		else
		{
			LateUpdateReplicated();
		}
		LateUpdateShared();
		previousState = currentState;
	}

	protected Transform DefaultAnchor()
	{
		if (_isDefaultAnchorSet)
		{
			return _defaultAnchor;
		}
		_isDefaultAnchorSet = true;
		_defaultAnchor = ((anchor == null) ? base.transform : anchor);
		return _defaultAnchor;
	}

	private Transform GetAnchor(PositionState pos)
	{
		if (grabAnchor == null)
		{
			return DefaultAnchor();
		}
		if (InHand())
		{
			return grabAnchor;
		}
		return DefaultAnchor();
	}

	protected bool Attached()
	{
		bool flag = InHand() && detatchOnGrab;
		if (!Dropped())
		{
			return !flag;
		}
		return false;
	}

	private Transform GetTargetStorageZone(BodyDockPositions.DropPositions state)
	{
		return state switch
		{
			BodyDockPositions.DropPositions.LeftArm => targetDock.leftArmTransform, 
			BodyDockPositions.DropPositions.RightArm => targetDock.rightArmTransform, 
			BodyDockPositions.DropPositions.Chest => targetDock.chestTransform, 
			BodyDockPositions.DropPositions.LeftBack => targetDock.leftBackTransform, 
			BodyDockPositions.DropPositions.RightBack => targetDock.rightBackTransform, 
			BodyDockPositions.DropPositions.None => null, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public Transform GetTargetDock(PositionState state, VRRig rig)
	{
		BodyDockPositions myBodyDockPositions = rig.myBodyDockPositions;
		VRRigAnchorOverrides component = rig.GetComponent<VRRigAnchorOverrides>();
		return state switch
		{
			PositionState.OnLeftArm => component.AnchorOverride(currentState, myBodyDockPositions.leftArmTransform), 
			PositionState.OnRightArm => component.AnchorOverride(currentState, myBodyDockPositions.rightArmTransform), 
			PositionState.InLeftHand => component.AnchorOverride(currentState, myBodyDockPositions.leftHandTransform), 
			PositionState.InRightHand => component.AnchorOverride(currentState, myBodyDockPositions.rightHandTransform), 
			PositionState.OnChest => component.AnchorOverride(currentState, myBodyDockPositions.chestTransform), 
			PositionState.OnLeftShoulder => component.AnchorOverride(currentState, myBodyDockPositions.leftBackTransform), 
			PositionState.OnRightShoulder => component.AnchorOverride(currentState, myBodyDockPositions.rightBackTransform), 
			_ => null, 
		};
	}

	private void UpdateFollowXform()
	{
		switch (currentState)
		{
		case PositionState.Dropped:
			return;
		case PositionState.None:
			if (previousState != 0)
			{
				ResetToHome();
			}
			return;
		}
		if (previousState != currentState)
		{
			transferrableItemSlotTransformOverrideCachedMatrix = null;
			if (interpState == InterpolateState.Interpolating)
			{
				interpState = InterpolateState.None;
			}
		}
		if (!targetRigSet)
		{
			return;
		}
		Transform transform = GetAnchor(currentState);
		Transform transform2 = transform;
		try
		{
			switch (currentState)
			{
			case PositionState.OnLeftArm:
				transform2 = anchorOverrides.AnchorOverride(currentState, targetDock.leftArmTransform);
				break;
			case PositionState.OnRightArm:
				transform2 = anchorOverrides.AnchorOverride(currentState, targetDock.rightArmTransform);
				break;
			case PositionState.InLeftHand:
				transform2 = anchorOverrides.AnchorOverride(currentState, targetDock.leftHandTransform);
				break;
			case PositionState.InRightHand:
				transform2 = anchorOverrides.AnchorOverride(currentState, targetDock.rightHandTransform);
				break;
			case PositionState.OnChest:
				transform2 = anchorOverrides.AnchorOverride(currentState, targetDock.chestTransform);
				break;
			case PositionState.OnLeftShoulder:
				transform2 = anchorOverrides.AnchorOverride(currentState, targetDock.leftBackTransform);
				break;
			case PositionState.OnRightShoulder:
				transform2 = anchorOverrides.AnchorOverride(currentState, targetDock.rightBackTransform);
				break;
			}
		}
		catch
		{
			Debug.LogError("anchorOverrides or targetDock has been destroyed", this);
			SetTargetRig(null);
		}
		if (currentState != PositionState.Dropped && (bool)rigidbodyInstance && ShouldBeKinematic() && !rigidbodyInstance.isKinematic)
		{
			rigidbodyInstance.isKinematic = true;
		}
		if (detatchOnGrab && (currentState == PositionState.InLeftHand || currentState == PositionState.InRightHand))
		{
			base.transform.parent = null;
		}
		if (interpState == InterpolateState.None)
		{
			try
			{
				if ((object)transform == null)
				{
					return;
				}
				startInterpolation |= transform2 != transform.parent;
			}
			catch
			{
			}
			if (!startInterpolation && !isGrabAnchorSet && base.transform.parent != transform && transform != base.transform)
			{
				startInterpolation = true;
			}
			if (startInterpolation)
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
				if (currentState == PositionState.InLeftHand)
				{
					if (flipOnXForLeftHand)
					{
						transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					else if (flipOnYForLeftHand)
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
				if (Time.frameCount == enabledOnFrame || Time.frameCount == enabledOnFrame + 1)
				{
					Matrix4x4 matrix4x = GetDefaultTransformationMatrix();
					if ((bool)transferrableItemSlotTransformOverride && transferrableItemSlotTransformOverride.GetTransformFromPositionState(currentState, advancedGrabState, transform2, out var matrix4X))
					{
						matrix4x = matrix4X;
					}
					Matrix4x4 matrix = transform.localToWorldMatrix * matrix4x;
					base.transform.SetLocalToWorldMatrixNoScale(matrix);
				}
				else
				{
					interpState = InterpolateState.Interpolating;
					interpDt = interpTime;
					interpStartRot = rotation;
					interpStartPos = position;
					base.transform.position = position;
					base.transform.rotation = rotation;
				}
				startInterpolation = false;
			}
		}
		if (interpState != InterpolateState.Interpolating)
		{
			return;
		}
		Matrix4x4 matrix4x2 = GetDefaultTransformationMatrix();
		if ((object)transferrableItemSlotTransformOverride != null)
		{
			if (!transferrableItemSlotTransformOverrideCachedMatrix.HasValue)
			{
				transferrableItemSlotTransformOverrideApplicable = transferrableItemSlotTransformOverride.GetTransformFromPositionState(currentState, advancedGrabState, transform2, out var matrix4X2);
				transferrableItemSlotTransformOverrideCachedMatrix = matrix4X2;
			}
			if (transferrableItemSlotTransformOverrideApplicable)
			{
				matrix4x2 = transferrableItemSlotTransformOverrideCachedMatrix.Value;
			}
		}
		float t = Mathf.Clamp((interpTime - interpDt) / interpTime, 0f, 1f);
		Mathf.SmoothStep(0f, 1f, t);
		Matrix4x4 m = transform.localToWorldMatrix * matrix4x2;
		Transform obj3 = base.transform;
		ref Vector3 a = ref interpStartPos;
		Vector3 b = m.Position();
		obj3.position = a.LerpToUnclamped(in b, t);
		base.transform.rotation = Quaternion.Slerp(interpStartRot, m.Rotation(), t);
		base.transform.localScale = matrix4x2.lossyScale;
		interpDt -= Time.deltaTime;
		if (interpDt <= 0f)
		{
			transform.parent = transform2;
			interpState = InterpolateState.None;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			if (flipOnXForLeftHand && currentState == PositionState.InLeftHand)
			{
				transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			if (flipOnYForLeftHand && currentState == PositionState.InLeftHand)
			{
				transform.localScale = new Vector3(1f, -1f, 1f);
			}
			m = transform.localToWorldMatrix * matrix4x2;
			base.transform.SetLocalToWorldMatrixNoScale(m);
			base.transform.localScale = matrix4x2.lossyScale;
		}
	}

	public virtual void DropItem()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand: true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand: false);
		}
		currentState = PositionState.Dropped;
		if ((bool)worldShareableInstance)
		{
			worldShareableInstance.transferableObjectState = currentState;
		}
		if (canDrop)
		{
			base.transform.parent = null;
			if ((bool)anchor)
			{
				anchor.parent = InitialDockObject;
			}
			if ((bool)rigidbodyInstance && ShouldBeKinematic() && !rigidbodyInstance.isKinematic)
			{
				rigidbodyInstance.isKinematic = true;
			}
		}
	}

	protected virtual void LateUpdateShared()
	{
		disableItem = true;
		if (isSceneObject)
		{
			disableItem = false;
		}
		else
		{
			for (int i = 0; i < ownerRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (ownerRig.ActiveTransferrableObjectIndex(i) == myIndex)
				{
					disableItem = false;
					break;
				}
			}
			if (disableItem)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
		}
		if (previousState != currentState && detatchOnGrab && InHand())
		{
			base.transform.parent = null;
			if (!ShouldBeKinematic() && rigidbodyInstance.isKinematic)
			{
				rigidbodyInstance.isKinematic = false;
			}
		}
		if (currentState != PositionState.Dropped)
		{
			UpdateFollowXform();
		}
		else if (canDrop)
		{
			if ((object)base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			try
			{
				if ((object)anchor != null && anchor.parent != InitialDockObject)
				{
					anchor.parent = InitialDockObject;
				}
			}
			catch
			{
			}
		}
		if (!isRigidbodySet || rigidbodyInstance.isKinematic == ShouldBeKinematic())
		{
			return;
		}
		rigidbodyInstance.isKinematic = ShouldBeKinematic();
		if ((bool)worldShareableInstance)
		{
			if (currentState == PositionState.Dropped)
			{
				worldShareableInstance.EnableRemoteSync = true;
			}
			else
			{
				worldShareableInstance.EnableRemoteSync = !ShouldBeKinematic();
			}
		}
	}

	public virtual void ResetToHome()
	{
		if (isSceneObject)
		{
			currentState = PositionState.None;
		}
		ResetXf();
		if (isRigidbodySet && ShouldBeKinematic() && !rigidbodyInstance.isKinematic)
		{
			rigidbodyInstance.isKinematic = true;
		}
	}

	protected void ResetXf()
	{
		if (!positionInitialized)
		{
			initOffset = base.transform.localPosition;
			initRotation = base.transform.localRotation;
		}
		if (!canDrop && !allowWorldSharableInstance)
		{
			return;
		}
		Transform transform = DefaultAnchor();
		if (base.transform != transform && base.transform.parent != transform)
		{
			base.transform.parent = transform;
		}
		if ((bool)InitialDockObject)
		{
			anchor.localPosition = Vector3.zero;
			anchor.localRotation = Quaternion.identity;
			anchor.localScale = Vector3.one;
		}
		if ((bool)grabAnchor)
		{
			if (grabAnchor.parent != base.transform)
			{
				grabAnchor.parent = base.transform;
			}
			grabAnchor.localPosition = Vector3.zero;
			grabAnchor.localRotation = Quaternion.identity;
			grabAnchor.localScale = Vector3.one;
		}
		if ((bool)transferrableItemSlotTransformOverride)
		{
			Transform transformFromPositionState = transferrableItemSlotTransformOverride.GetTransformFromPositionState(currentState);
			if ((bool)transformFromPositionState)
			{
				base.transform.position = transformFromPositionState.position;
				base.transform.rotation = transformFromPositionState.rotation;
			}
		}
		else
		{
			base.transform.SetLocalMatrixRelativeToParent(GetDefaultTransformationMatrix());
		}
	}

	protected void ReDock()
	{
		if (IsMyItem())
		{
			currentState = initState;
		}
		if ((bool)rigidbodyInstance && ShouldBeKinematic() && !rigidbodyInstance.isKinematic)
		{
			rigidbodyInstance.isKinematic = true;
		}
		ResetXf();
	}

	private void HandleLocalInput()
	{
		GameObject[] array;
		if (!InHand())
		{
			array = gameObjectsActiveOnlyWhileHeld;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			return;
		}
		array = gameObjectsActiveOnlyWhileHeld;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		XRNode node = ((currentState == PositionState.InLeftHand) ? XRNode.LeftHand : XRNode.RightHand);
		indexTrigger = ControllerInputPoller.TriggerFloat(node);
		bool num = !latched && indexTrigger >= myThreshold;
		bool flag = latched && indexTrigger < myThreshold - hysterisis;
		if (num || testActivate)
		{
			testActivate = false;
			if (CanActivate())
			{
				OnActivate();
			}
		}
		else if (flag || testDeactivate)
		{
			testDeactivate = false;
			if (CanDeactivate())
			{
				OnDeactivate();
			}
		}
	}

	protected virtual void LocalMyObjectValidation()
	{
	}

	protected virtual void LocalPersistanceValidation()
	{
		if (maxDistanceFromOriginBeforeRespawn != 0f && Vector3.Distance(base.transform.position, originPoint.position) > maxDistanceFromOriginBeforeRespawn)
		{
			if (audioSrc != null && resetPositionAudioClip != null)
			{
				audioSrc.PlayOneShot(resetPositionAudioClip);
			}
			Debug.Log("Too far from origin, Resetting position", this);
			if (currentState != PositionState.Dropped)
			{
				DropItem();
				currentState = PositionState.Dropped;
				UpdateFollowXform();
			}
			base.transform.position = originPoint.position;
			rigidbodyInstance.velocity = Vector3.zero;
		}
		if ((bool)rigidbodyInstance && rigidbodyInstance.velocity.sqrMagnitude > 10000f)
		{
			Debug.Log("Moving too fast, Assuming ive fallen out of the map. Ressetting position", this);
			ResetToHome();
		}
	}

	public void ObjectBeingTaken()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand: true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand: false);
		}
	}

	protected virtual void LateUpdateLocal()
	{
		wasHover = isHover;
		isHover = false;
		LocalPersistanceValidation();
		if (PhotonNetwork.InRoom)
		{
			if (!isSceneObject && IsLocalObject())
			{
				myRig.SetTransferrablePosStates(objectIndex, currentState);
				myRig.SetTransferrableItemStates(objectIndex, itemState);
				myRig.SetTransferrableDockPosition(objectIndex, storedZone);
			}
			if ((object)worldShareableInstance != null)
			{
				worldShareableInstance.transferableObjectState = currentState;
				worldShareableInstance.transferableObjectItemState = itemState;
			}
		}
		HandleLocalInput();
	}

	protected void LateUpdateReplicatedSceneObject()
	{
		if ((object)myOnlineRig != null)
		{
			storedZone = myOnlineRig.TransferrableDockPosition(objectIndex);
		}
		if ((object)worldShareableInstance != null)
		{
			currentState = worldShareableInstance.transferableObjectState;
			itemState = worldShareableInstance.transferableObjectItemState;
			worldShareableInstance.EnableRemoteSync = !ShouldBeKinematic() || currentState == PositionState.Dropped;
		}
		if (isRigidbodySet && ShouldBeKinematic() && !rigidbodyInstance.isKinematic)
		{
			rigidbodyInstance.isKinematic = true;
		}
	}

	protected virtual void LateUpdateReplicated()
	{
		if (isSceneObject || shareable)
		{
			LateUpdateReplicatedSceneObject();
		}
		else
		{
			if ((object)myOnlineRig == null)
			{
				return;
			}
			currentState = myOnlineRig.TransferrablePosStates(objectIndex);
			if (currentState == PositionState.Dropped && !canDrop && !shareable)
			{
				if (previousState == PositionState.None)
				{
					base.gameObject.SetActive(value: false);
				}
				currentState = previousState;
			}
			if (isRigidbodySet)
			{
				rigidbodyInstance.isKinematic = ShouldBeKinematic();
			}
			bool flag = true;
			itemState = myOnlineRig.TransferrableItemStates(objectIndex);
			storedZone = myOnlineRig.TransferrableDockPosition(objectIndex);
			int num = myOnlineRig.ActiveTransferrableObjectIndexLength();
			for (int i = 0; i < num; i++)
			{
				if (myOnlineRig.ActiveTransferrableObjectIndex(i) == myIndex)
				{
					flag = false;
					GameObject[] array = gameObjectsActiveOnlyWhileHeld;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].SetActive(InHand());
					}
				}
			}
			if (flag)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public virtual void ResetToDefaultState()
	{
		canAutoGrabLeft = true;
		canAutoGrabRight = true;
		wasHover = false;
		isHover = false;
		if (!IsLocalObject() && (bool)worldShareableInstance && !isSceneObject)
		{
			if (IsMyItem())
			{
				return;
			}
			worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
			{
			});
		}
		ResetXf();
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(worldShareableInstance == null) && !worldShareableInstance.guard.isTrulyMine)
		{
			if (!IsGrabbable())
			{
				return;
			}
			worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
		if (grabbingHand == EquipmentInteractor.instance.leftHand && currentState != PositionState.OnLeftArm)
		{
			if (currentState == PositionState.InRightHand && disableStealing)
			{
				return;
			}
			canAutoGrabLeft = false;
			interpState = InterpolateState.None;
			currentState = PositionState.InLeftHand;
			if ((bool)transferrableItemSlotTransformOverride)
			{
				advancedGrabState = transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(PositionState.InLeftHand, EquipmentInteractor.instance.leftHand.transform, GetTargetDock(currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, forLeftHand: true);
			GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		else if (grabbingHand == EquipmentInteractor.instance.rightHand && currentState != PositionState.OnRightArm)
		{
			if (currentState == PositionState.InLeftHand && disableStealing)
			{
				return;
			}
			canAutoGrabRight = false;
			interpState = InterpolateState.None;
			currentState = PositionState.InRightHand;
			if ((bool)transferrableItemSlotTransformOverride)
			{
				advancedGrabState = transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(PositionState.InRightHand, EquipmentInteractor.instance.rightHand.transform, GetTargetDock(currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, forLeftHand: false);
			GorillaTagger.Instance.StartVibration(forLeftController: false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		if ((bool)rigidbodyInstance && !rigidbodyInstance.isKinematic && ShouldBeKinematic())
		{
			rigidbodyInstance.isKinematic = true;
		}
	}

	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!IsMyItem() || !CanDeactivate() || !IsHeld() || ((!(releasingHand == EquipmentInteractor.instance.rightHand) || !(this == EquipmentInteractor.instance.rightHandHeldEquipment)) && (!(releasingHand == EquipmentInteractor.instance.leftHand) || !(this == EquipmentInteractor.instance.leftHandHeldEquipment))))
		{
			return;
		}
		if (releasingHand == EquipmentInteractor.instance.leftHand)
		{
			canAutoGrabLeft = true;
		}
		else
		{
			canAutoGrabRight = true;
		}
		if ((object)zoneReleased != null)
		{
			bool num = currentState == PositionState.InLeftHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.LeftArm;
			bool flag = currentState == PositionState.InRightHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.RightArm;
			if (num || flag)
			{
				return;
			}
			if (targetDock.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == targetDock && (zoneReleased.dropPosition & dockPositions) != 0)
			{
				storedZone = zoneReleased.dropPosition;
			}
		}
		bool flag2 = false;
		interpState = InterpolateState.None;
		if (isSceneObject || canDrop || allowWorldSharableInstance)
		{
			if (!rigidbodyInstance)
			{
				return;
			}
			if ((bool)worldShareableInstance)
			{
				worldShareableInstance.EnableRemoteSync = true;
			}
			if (!flag2)
			{
				currentState = PositionState.Dropped;
			}
			if (rigidbodyInstance.isKinematic && !ShouldBeKinematic())
			{
				rigidbodyInstance.isKinematic = false;
			}
			GorillaVelocityEstimator component = GetComponent<GorillaVelocityEstimator>();
			if (component != null && rigidbodyInstance != null)
			{
				rigidbodyInstance.velocity = component.linearVelocity;
				rigidbodyInstance.angularVelocity = component.angularVelocity;
			}
		}
		else
		{
			_ = allowWorldSharableInstance;
		}
		DropItemCleanup();
		EquipmentInteractor.instance.ForceDropEquipment(this);
	}

	public override void DropItemCleanup()
	{
		if (currentState != PositionState.Dropped)
		{
			switch (storedZone)
			{
			case BodyDockPositions.DropPositions.LeftArm:
				currentState = PositionState.OnLeftArm;
				break;
			case BodyDockPositions.DropPositions.RightArm:
				currentState = PositionState.OnRightArm;
				break;
			case BodyDockPositions.DropPositions.Chest:
				currentState = PositionState.OnChest;
				break;
			case BodyDockPositions.DropPositions.LeftBack:
				currentState = PositionState.OnLeftShoulder;
				break;
			case BodyDockPositions.DropPositions.RightBack:
				currentState = PositionState.OnRightShoulder;
				break;
			}
		}
	}

	public virtual void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (IsGrabbable())
		{
			if (!wasHover)
			{
				GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			}
			isHover = true;
		}
	}

	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = currentState == PositionState.InLeftHand;
		if (myRig.photonView != null)
		{
			myRig.photonView.RPC("PlayHandTap", RpcTarget.Others, soundIndex, flag, 0.1f);
		}
		myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	public virtual void PlayNote(int note, float volume)
	{
	}

	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return canAutoGrabRight;
		}
		return canAutoGrabLeft;
	}

	public virtual bool CanActivate()
	{
		return true;
	}

	public virtual bool CanDeactivate()
	{
		return true;
	}

	public virtual void OnActivate()
	{
		latched = true;
	}

	public virtual void OnDeactivate()
	{
		latched = false;
	}

	public virtual bool IsMyItem()
	{
		if ((object)GorillaTagger.Instance == null)
		{
			return true;
		}
		if ((object)targetRig == null)
		{
			return false;
		}
		return targetRig == GorillaTagger.Instance.offlineVRRig;
	}

	protected virtual bool IsHeld()
	{
		if ((object)EquipmentInteractor.instance == null)
		{
			return false;
		}
		if (!(EquipmentInteractor.instance.leftHandHeldEquipment == this))
		{
			return EquipmentInteractor.instance.rightHandHeldEquipment == this;
		}
		return true;
	}

	public virtual bool IsGrabbable()
	{
		if (IsMyItem())
		{
			return true;
		}
		if (isSceneObject || shareable)
		{
			if (!isSceneObject && !shareable)
			{
				return false;
			}
			if (currentState == PositionState.Dropped)
			{
				return true;
			}
			if (currentState == PositionState.None)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool InHand()
	{
		if (currentState != PositionState.InLeftHand)
		{
			return currentState == PositionState.InRightHand;
		}
		return true;
	}

	public bool Dropped()
	{
		return currentState == PositionState.Dropped;
	}

	public bool InLeftHand()
	{
		return currentState == PositionState.InLeftHand;
	}

	public bool InRightHand()
	{
		return currentState == PositionState.InRightHand;
	}

	public bool OnChest()
	{
		return currentState == PositionState.OnChest;
	}

	public bool OnShoulder()
	{
		if (currentState != PositionState.OnLeftShoulder)
		{
			return currentState == PositionState.OnRightShoulder;
		}
		return true;
	}

	protected Player OwningPlayer()
	{
		if (myRig == null)
		{
			return myOnlineRig.photonView.Owner;
		}
		return PhotonNetwork.LocalPlayer;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		if (originPoint != null)
		{
			Gizmos.DrawWireSphere(originPoint.transform.position, maxDistanceFromOriginBeforeRespawn);
		}
	}

	public virtual void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		if (toPlayer != null && toPlayer.Equals(fromPlayer))
		{
			return;
		}
		if (object.Equals(fromPlayer, PhotonNetwork.LocalPlayer) && IsHeld())
		{
			DropItem();
		}
		if (toPlayer == null)
		{
			SetTargetRig(null);
		}
		else
		{
			if (!shareable && !isSceneObject)
			{
				return;
			}
			if (object.Equals(toPlayer, PhotonNetwork.LocalPlayer))
			{
				if (GorillaTagger.Instance == null)
				{
					Debug.LogError("OnOwnershipTransferred has been initiated too quickly, The local player is not ready");
				}
				else
				{
					SetTargetRig(GorillaTagger.Instance.offlineVRRig);
				}
				return;
			}
			VRRig vRRig = GorillaGameManager.StaticFindRigForPlayer(toPlayer);
			if (!vRRig)
			{
				Debug.LogError("failed to find target rig for ownershiptransfer");
			}
			else
			{
				SetTargetRig(vRRig);
			}
		}
	}

	public bool OnOwnershipRequest(Player fromPlayer)
	{
		if ((bool)GorillaGameManager.instance)
		{
			VRRig vRRig = GorillaGameManager.StaticFindRigForPlayer(fromPlayer);
			if ((bool)vRRig)
			{
				return true;
			}
			if (Vector3.SqrMagnitude(base.transform.position - vRRig.transform.position) > 16f)
			{
				Debug.Log("Player whos trying to get is too far, Denying takeover");
				return false;
			}
		}
		if (currentState == PositionState.Dropped)
		{
			return true;
		}
		if (currentState == PositionState.None)
		{
			return true;
		}
		if (isSceneObject)
		{
			return false;
		}
		if (canDrop)
		{
			if (ownerRig == null || ownerRig.creator == null)
			{
				return true;
			}
			if (ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	public bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer)
	{
		VRRig vRRig = GorillaGameManager.StaticFindRigForPlayer(fromPlayer);
		if (vRRig == null)
		{
			return true;
		}
		if (Vector3.SqrMagnitude(base.transform.position - vRRig.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (currentState == PositionState.Dropped)
		{
			return true;
		}
		if (currentState == PositionState.None)
		{
			return true;
		}
		if (canDrop)
		{
			if (ownerRig == null || ownerRig.creator == null)
			{
				return true;
			}
			if (ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	public void OnMyOwnerLeft()
	{
		if (currentState != 0 && currentState != PositionState.Dropped)
		{
			DropItem();
			if ((bool)anchor)
			{
				anchor.parent = InitialDockObject;
				anchor.localPosition = Vector3.zero;
				anchor.localRotation = Quaternion.identity;
			}
		}
	}

	public void OnMyCreatorLeft()
	{
		OnItemDestroyedOrDisabled();
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
