using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework;

public class FingerTipPokeTool : InteractableTool
{
	private const int NUM_VELOCITY_FRAMES = 10;

	[SerializeField]
	private FingerTipPokeToolView _fingerTipPokeToolView;

	[SerializeField]
	private OVRPlugin.HandFinger _fingerToFollow = OVRPlugin.HandFinger.Index;

	private Vector3[] _velocityFrames;

	private int _currVelocityFrame;

	private bool _sampledMaxFramesAlready;

	private Vector3 _position;

	private BoneCapsuleTriggerLogic[] _boneCapsuleTriggerLogic;

	private float _lastScale = 1f;

	private bool _isInitialized;

	private OVRBoneCapsule _capsuleToTrack;

	public override InteractableToolTags ToolTags => InteractableToolTags.Poke;

	public override ToolInputState ToolInputState => ToolInputState.Inactive;

	public override bool IsFarFieldTool => false;

	public override bool EnableState
	{
		get
		{
			return _fingerTipPokeToolView.gameObject.activeSelf;
		}
		set
		{
			_fingerTipPokeToolView.gameObject.SetActive(value);
		}
	}

	public override void Initialize()
	{
		InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
		_fingerTipPokeToolView.InteractableTool = this;
		_velocityFrames = new Vector3[10];
		Array.Clear(_velocityFrames, 0, 10);
		StartCoroutine(AttachTriggerLogic());
	}

	private IEnumerator AttachTriggerLogic()
	{
		while (!HandsManager.Instance || !HandsManager.Instance.IsInitialized())
		{
			yield return null;
		}
		OVRSkeleton skeleton = (base.IsRightHandedTool ? HandsManager.Instance.RightHandSkeleton : HandsManager.Instance.LeftHandSkeleton);
		OVRSkeleton.BoneId boneId = _fingerToFollow switch
		{
			OVRPlugin.HandFinger.Thumb => OVRSkeleton.BoneId.Hand_Thumb3, 
			OVRPlugin.HandFinger.Index => OVRSkeleton.BoneId.Hand_Index3, 
			OVRPlugin.HandFinger.Middle => OVRSkeleton.BoneId.Hand_Middle3, 
			OVRPlugin.HandFinger.Ring => OVRSkeleton.BoneId.Hand_Ring3, 
			_ => OVRSkeleton.BoneId.Hand_Pinky3, 
		};
		List<BoneCapsuleTriggerLogic> list = new List<BoneCapsuleTriggerLogic>();
		List<OVRBoneCapsule> capsulesPerBone = HandsManager.GetCapsulesPerBone(skeleton, boneId);
		foreach (OVRBoneCapsule item in capsulesPerBone)
		{
			BoneCapsuleTriggerLogic boneCapsuleTriggerLogic = item.CapsuleRigidbody.gameObject.AddComponent<BoneCapsuleTriggerLogic>();
			item.CapsuleCollider.isTrigger = true;
			boneCapsuleTriggerLogic.ToolTags = ToolTags;
			list.Add(boneCapsuleTriggerLogic);
		}
		_boneCapsuleTriggerLogic = list.ToArray();
		if (capsulesPerBone.Count > 0)
		{
			_capsuleToTrack = capsulesPerBone[0];
		}
		_isInitialized = true;
	}

	private void Update()
	{
		if ((bool)HandsManager.Instance && HandsManager.Instance.IsInitialized() && _isInitialized && _capsuleToTrack != null)
		{
			float handScale = (base.IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand).HandScale;
			Transform transform = _capsuleToTrack.CapsuleCollider.transform;
			Vector3 right = transform.right;
			Vector3 vector = transform.position + _capsuleToTrack.CapsuleCollider.height * 0.5f * right;
			Vector3 vector2 = handScale * _fingerTipPokeToolView.SphereRadius * right;
			Vector3 position = vector + vector2;
			base.transform.position = position;
			base.transform.rotation = transform.rotation;
			base.InteractionPosition = vector;
			UpdateAverageVelocity();
			CheckAndUpdateScale();
		}
	}

	private void UpdateAverageVelocity()
	{
		Vector3 position = _position;
		Vector3 position2 = base.transform.position;
		Vector3 vector = (position2 - position) / Time.deltaTime;
		_position = position2;
		_velocityFrames[_currVelocityFrame] = vector;
		_currVelocityFrame = (_currVelocityFrame + 1) % 10;
		base.Velocity = Vector3.zero;
		if (!_sampledMaxFramesAlready && _currVelocityFrame == 9)
		{
			_sampledMaxFramesAlready = true;
		}
		int num = (_sampledMaxFramesAlready ? 10 : (_currVelocityFrame + 1));
		for (int i = 0; i < num; i++)
		{
			base.Velocity += _velocityFrames[i];
		}
		base.Velocity /= (float)num;
	}

	private void CheckAndUpdateScale()
	{
		float num = (base.IsRightHandedTool ? HandsManager.Instance.RightHand.HandScale : HandsManager.Instance.LeftHand.HandScale);
		if (Mathf.Abs(num - _lastScale) > Mathf.Epsilon)
		{
			base.transform.localScale = new Vector3(num, num, num);
			_lastScale = num;
		}
	}

	public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
	{
		_currentIntersectingObjects.Clear();
		BoneCapsuleTriggerLogic[] boneCapsuleTriggerLogic = _boneCapsuleTriggerLogic;
		for (int i = 0; i < boneCapsuleTriggerLogic.Length; i++)
		{
			foreach (ColliderZone collidersTouchingU in boneCapsuleTriggerLogic[i].CollidersTouchingUs)
			{
				_currentIntersectingObjects.Add(new InteractableCollisionInfo(collidersTouchingU, collidersTouchingU.CollisionDepth, this));
			}
		}
		return _currentIntersectingObjects;
	}

	public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
	{
	}

	public override void DeFocus()
	{
	}
}
