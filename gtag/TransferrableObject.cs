﻿using System;
using GorillaExtensions;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR;

public class TransferrableObject : HoldableObject, ISelfValidator, IRequestableOwnershipGuardCallbacks, IPreDisable
{
	public void FixTransformOverride()
	{
		this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
	}

	public void Validate(SelfValidationResult result)
	{
	}

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

	public bool IsLocalOwnedWorldShareable
	{
		get
		{
			return this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine;
		}
	}

	public void WorldShareableRequestOwnership()
	{
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
	}

	private protected bool appIsQuitting { protected get; private set; }

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
		this.isGrabAnchorSet = this.grabAnchor != null;
		Application.quitting += delegate
		{
			this.appIsQuitting = true;
		};
	}

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
		base.transform.localRotation = (this.initMatrix).Rotation();
		this.positionInitialized = true;
	}

	protected virtual void Start()
	{
	}

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

	protected virtual void OnDestroy()
	{
	}

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

	public virtual void PreDisable()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		this.currentState = TransferrableObject.PositionState.None;
		this.interpState = TransferrableObject.InterpolateState.None;
		this.ResetToDefaultState();
	}

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

	public virtual bool ShouldBeKinematic()
	{
		if (this.detatchOnGrab)
		{
			return this.currentState != TransferrableObject.PositionState.Dropped && this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand;
		}
		return this.currentState != TransferrableObject.PositionState.Dropped;
	}

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

	public bool IsLocalObject()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		bool flag = this.isSceneObject;
	}

	public void SetWorldShareableItem(WorldShareableItem item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	protected virtual void PlayDestroyedOrDisabledEffect()
	{
	}

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

	public virtual void LateUpdate()
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

	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

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
				this.startInterpolation |= transform2 != transform.parent;
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
					Matrix4x4 matrix4x = this.GetDefaultTransformationMatrix();
					if ((this.currentState != TransferrableObject.PositionState.InLeftHand || !(this.handPoseLeft != null)) && this.currentState == TransferrableObject.PositionState.InRightHand)
					{
						this.handPoseRight != null;
					}
					Matrix4x4 matrix4x2;
					if (this.transferrableItemSlotTransformOverride && this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x2))
					{
						matrix4x = matrix4x2;
					}
					Matrix4x4 matrix4x3 = transform.localToWorldMatrix * matrix4x;
					base.transform.SetLocalToWorldMatrixNoScale(matrix4x3);
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
			Matrix4x4 matrix4x4 = this.GetDefaultTransformationMatrix();
			if (this.transferrableItemSlotTransformOverride != null)
			{
				if (this.transferrableItemSlotTransformOverrideCachedMatrix == null)
				{
					Matrix4x4 matrix4x5;
					this.transferrableItemSlotTransformOverrideApplicable = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x5);
					this.transferrableItemSlotTransformOverrideCachedMatrix = new Matrix4x4?(matrix4x5);
				}
				if (this.transferrableItemSlotTransformOverrideApplicable)
				{
					matrix4x4 = this.transferrableItemSlotTransformOverrideCachedMatrix.Value;
				}
			}
			float num = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			Mathf.SmoothStep(0f, 1f, num);
			Matrix4x4 matrix4x6 = transform.localToWorldMatrix * matrix4x4;
			Transform transform3 = base.transform;
			Vector3 vector = matrix4x6.Position();
			transform3.position = (this.interpStartPos).LerpToUnclamped(vector, num);
			base.transform.rotation = Quaternion.Slerp(this.interpStartRot, (matrix4x6).Rotation(), num);
			base.transform.localScale = matrix4x4.lossyScale;
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
				matrix4x6 = transform.localToWorldMatrix * matrix4x4;
				base.transform.SetLocalToWorldMatrixNoScale(matrix4x6);
				base.transform.localScale = matrix4x4.lossyScale;
			}
		}
	}

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
		else if (this.canDrop && !this.allowReparenting)
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

	private void HandleLocalInput()
	{
		GameObject[] array;
		Behaviour[] array2;
		if (!this.InHand())
		{
			array = this.gameObjectsActiveOnlyWhileHeld;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			array = this.gameObjectsActiveOnlyWhileDocked;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = true;
			}
			return;
		}
		array = this.gameObjectsActiveOnlyWhileHeld;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		array2 = this.behavioursEnabledOnlyWhileHeld;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = true;
		}
		array = this.gameObjectsActiveOnlyWhileDocked;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		array2 = this.behavioursEnabledOnlyWhileDocked;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
		XRNode xrnode = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? XRNode.LeftHand : XRNode.RightHand);
		this.indexTrigger = ControllerInputPoller.TriggerFloat(xrnode);
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

	protected virtual void LocalMyObjectValidation()
	{
	}

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
			if (!this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.velocity = Vector3.zero;
			}
		}
		if (this.rigidbodyInstance && this.rigidbodyInstance.velocity.sqrMagnitude > 10000f)
		{
			Debug.Log("Moving too fast, Assuming ive fallen out of the map. Ressetting position", this);
			this.ResetToHome();
		}
	}

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
			this.worldShareableInstance.EnableRemoteSync = !this.ShouldBeKinematic() || this.currentState == TransferrableObject.PositionState.Dropped;
		}
		if (this.isRigidbodySet && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

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
				Behaviour[] array2 = this.behavioursEnabledOnlyWhileHeld;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = this.InHand();
				}
				array = this.gameObjectsActiveOnlyWhileDocked;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].SetActive(!this.InHand());
				}
				array2 = this.behavioursEnabledOnlyWhileDocked;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = !this.InHand();
				}
			}
		}
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
	}

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

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.IsMyItem())
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (!this.IsHeld())
		{
			return false;
		}
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
				return false;
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
				return false;
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
		return true;
	}

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

	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		if (this.myRig.photonView != null)
		{
			this.myRig.photonView.RPC("PlayHandTap", RpcTarget.Others, new object[] { soundIndex, flag, 0.1f });
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	public virtual void PlayNote(int note, float volume)
	{
	}

	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
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
		this.latched = true;
	}

	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	public virtual bool IsMyItem()
	{
		return GorillaTagger.Instance == null || (this.targetRig != null && this.targetRig == GorillaTagger.Instance.offlineVRRig);
	}

	protected virtual bool IsHeld()
	{
		return EquipmentInteractor.instance != null && (EquipmentInteractor.instance.leftHandHeldEquipment == this || EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	public virtual bool IsGrabbable()
	{
		return this.IsMyItem() || ((this.isSceneObject || this.shareable) && (this.isSceneObject || this.shareable) && (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None));
	}

	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	protected Player OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.photonView.Owner;
		}
		return PhotonNetwork.LocalPlayer;
	}

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
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(toPlayer);
			if (!vrrig)
			{
				Debug.LogError("failed to find target rig for ownershiptransfer");
				return;
			}
			this.SetTargetRig(vrrig);
			return;
		}
	}

	public bool OnOwnershipRequest(Player fromPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return false;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
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

	public bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return true;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
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

	public void OnMyCreatorLeft()
	{
		this.OnItemDestroyedOrDisabled();
		Object.Destroy(base.gameObject);
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

	public bool allowPlayerStealing;

	private TransferrableObject.PositionState initState;

	public TransferrableObject.ItemStates itemState;

	[DevInspectorShow]
	public BodyDockPositions.DropPositions storedZone;

	protected TransferrableObject.PositionState previousState;

	[DevInspectorYellow]
	[DevInspectorShow]
	public TransferrableObject.PositionState currentState;

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

	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Left mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseLeft;

	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Right mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseRight;

	public bool isGrabAnchorSet;

	private static Vector3 handPoseRightReferencePoint = new Vector3(-0.0141f, 0.0065f, -0.278f);

	private static Quaternion handPoseRightReferenceRotation = Quaternion.Euler(-2.058f, -17.2f, 65.05f);

	private static Vector3 handPoseLeftReferencePoint = new Vector3(0.0136f, 0.0045f, -0.2809f);

	private static Quaternion handPoseLeftReferenceRotation = Quaternion.Euler(-0.58f, 21.356f, -63.965f);

	public TransferrableItemSlotTransformOverride transferrableItemSlotTransformOverride;

	public int myIndex;

	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileDocked;

	[Tooltip("(Optional)")]
	public Behaviour[] behavioursEnabledOnlyWhileHeld;

	[Tooltip("(Optional)")]
	public Behaviour[] behavioursEnabledOnlyWhileDocked;

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

	private Matrix4x4 leftHandMatrix = Matrix4x4.identity;

	private Matrix4x4 rightHandMatrix = Matrix4x4.identity;

	private bool positionInitialized;

	public bool isSceneObject;

	public Rigidbody rigidbodyInstance;

	public bool isRigidbodySet;

	public bool canDrop;

	public bool allowReparenting;

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
	public TransferrableObject.InterpolateState interpState;

	public bool startInterpolation;

	public Transform InitialDockObject;

	private AudioSource audioSrc;

	protected Transform _defaultAnchor;

	protected bool _isDefaultAnchorSet;

	private Matrix4x4? transferrableItemSlotTransformOverrideCachedMatrix;

	private bool transferrableItemSlotTransformOverrideApplicable;

	public enum ItemStates
	{
		State0 = 1,
		State1,
		State2 = 4,
		State3 = 8,
		State4 = 16
	}

	[Flags]
	public enum PositionState
	{
		OnLeftArm = 1,
		OnRightArm = 2,
		InLeftHand = 4,
		InRightHand = 8,
		OnChest = 16,
		OnLeftShoulder = 32,
		OnRightShoulder = 64,
		Dropped = 128,
		None = 0
	}

	public enum InterpolateState
	{
		None,
		Interpolating
	}
}
