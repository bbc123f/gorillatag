using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework;

public class RayTool : InteractableTool
{
	private const float MINIMUM_RAY_CAST_DISTANCE = 0.8f;

	private const float COLLIDER_RADIUS = 0.01f;

	private const int NUM_MAX_PRIMARY_HITS = 10;

	private const int NUM_MAX_SECONDARY_HITS = 25;

	private const int NUM_COLLIDERS_TO_TEST = 20;

	[SerializeField]
	private RayToolView _rayToolView;

	[Range(0f, 45f)]
	[SerializeField]
	private float _coneAngleDegrees = 20f;

	[SerializeField]
	private float _farFieldMaxDistance = 5f;

	private PinchStateModule _pinchStateModule = new PinchStateModule();

	private Interactable _focusedInteractable;

	private Collider[] _collidersOverlapped = new Collider[20];

	private Interactable _currInteractableCastedAgainst;

	private float _coneAngleReleaseDegrees;

	private RaycastHit[] _primaryHits = new RaycastHit[10];

	private Collider[] _secondaryOverlapResults = new Collider[25];

	private bool _initialized;

	public override InteractableToolTags ToolTags => InteractableToolTags.Ray;

	public override ToolInputState ToolInputState
	{
		get
		{
			if (_pinchStateModule.PinchDownOnFocusedObject)
			{
				return ToolInputState.PrimaryInputDown;
			}
			if (_pinchStateModule.PinchSteadyOnFocusedObject)
			{
				return ToolInputState.PrimaryInputDownStay;
			}
			if (_pinchStateModule.PinchUpAndDownOnFocusedObject)
			{
				return ToolInputState.PrimaryInputUp;
			}
			return ToolInputState.Inactive;
		}
	}

	public override bool IsFarFieldTool => true;

	public override bool EnableState
	{
		get
		{
			return _rayToolView.EnableState;
		}
		set
		{
			_rayToolView.EnableState = value;
		}
	}

	public override void Initialize()
	{
		InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
		_rayToolView.InteractableTool = this;
		_coneAngleReleaseDegrees = _coneAngleDegrees * 1.2f;
		_initialized = true;
	}

	private void OnDestroy()
	{
		if (InteractableToolsInputRouter.Instance != null)
		{
			InteractableToolsInputRouter.Instance.UnregisterInteractableTool(this);
		}
	}

	private void Update()
	{
		if ((bool)HandsManager.Instance && HandsManager.Instance.IsInitialized() && _initialized)
		{
			OVRHand oVRHand = (base.IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand);
			Transform pointerPose = oVRHand.PointerPose;
			base.transform.position = pointerPose.position;
			base.transform.rotation = pointerPose.rotation;
			Vector3 interactionPosition = base.InteractionPosition;
			Vector3 position = base.transform.position;
			base.Velocity = (position - interactionPosition) / Time.deltaTime;
			base.InteractionPosition = position;
			_pinchStateModule.UpdateState(oVRHand, _focusedInteractable);
			_rayToolView.ToolActivateState = _pinchStateModule.PinchSteadyOnFocusedObject || _pinchStateModule.PinchDownOnFocusedObject;
		}
	}

	private Vector3 GetRayCastOrigin()
	{
		return base.transform.position + 0.8f * base.transform.forward;
	}

	public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
	{
		if (!_initialized)
		{
			return _currentIntersectingObjects;
		}
		if (_currInteractableCastedAgainst != null && HasRayReleasedInteractable(_currInteractableCastedAgainst))
		{
			_currInteractableCastedAgainst = null;
		}
		if (_currInteractableCastedAgainst == null)
		{
			_currentIntersectingObjects.Clear();
			_currInteractableCastedAgainst = FindTargetInteractable();
			if (_currInteractableCastedAgainst != null)
			{
				int num = Physics.OverlapSphereNonAlloc(_currInteractableCastedAgainst.transform.position, 0.01f, _collidersOverlapped);
				for (int i = 0; i < num; i++)
				{
					ColliderZone component = _collidersOverlapped[i].GetComponent<ColliderZone>();
					if (component != null)
					{
						Interactable parentInteractable = component.ParentInteractable;
						if (!(parentInteractable == null) && !(parentInteractable != _currInteractableCastedAgainst))
						{
							InteractableCollisionInfo item = new InteractableCollisionInfo(component, component.CollisionDepth, this);
							_currentIntersectingObjects.Add(item);
						}
					}
				}
				if (_currentIntersectingObjects.Count == 0)
				{
					_currInteractableCastedAgainst = null;
				}
			}
		}
		return _currentIntersectingObjects;
	}

	private bool HasRayReleasedInteractable(Interactable focusedInteractable)
	{
		Vector3 position = base.transform.position;
		Vector3 forward = base.transform.forward;
		float num = Mathf.Cos(_coneAngleReleaseDegrees * ((float)Math.PI / 180f));
		Vector3 lhs = focusedInteractable.transform.position - position;
		lhs.Normalize();
		return Vector3.Dot(lhs, forward) < num;
	}

	private Interactable FindTargetInteractable()
	{
		Vector3 rayCastOrigin = GetRayCastOrigin();
		Vector3 forward = base.transform.forward;
		Interactable interactable = null;
		interactable = FindPrimaryRaycastHit(rayCastOrigin, forward);
		if (interactable == null)
		{
			interactable = FindInteractableViaConeTest(rayCastOrigin, forward);
		}
		return interactable;
	}

	private Interactable FindPrimaryRaycastHit(Vector3 rayOrigin, Vector3 rayDirection)
	{
		Interactable interactable = null;
		int num = Physics.RaycastNonAlloc(new Ray(rayOrigin, rayDirection), _primaryHits, float.PositiveInfinity);
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = _primaryHits[i];
			ColliderZone component = raycastHit.transform.GetComponent<ColliderZone>();
			if (component == null)
			{
				continue;
			}
			Interactable parentInteractable = component.ParentInteractable;
			if (!(parentInteractable == null) && ((uint)parentInteractable.ValidToolTagsMask & (uint)ToolTags) != 0)
			{
				float magnitude = (parentInteractable.transform.position - rayOrigin).magnitude;
				if (interactable == null || magnitude < num2)
				{
					interactable = parentInteractable;
					num2 = magnitude;
				}
			}
		}
		return interactable;
	}

	private Interactable FindInteractableViaConeTest(Vector3 rayOrigin, Vector3 rayDirection)
	{
		Interactable interactable = null;
		float num = 0f;
		float num2 = Mathf.Cos(_coneAngleDegrees * ((float)Math.PI / 180f));
		float num3 = Mathf.Tan((float)Math.PI / 180f * _coneAngleDegrees * 0.5f) * _farFieldMaxDistance;
		int num4 = Physics.OverlapBoxNonAlloc(rayOrigin + rayDirection * _farFieldMaxDistance * 0.5f, new Vector3(num3, num3, _farFieldMaxDistance * 0.5f), _secondaryOverlapResults, base.transform.rotation);
		for (int i = 0; i < num4; i++)
		{
			ColliderZone component = _secondaryOverlapResults[i].GetComponent<ColliderZone>();
			if (component == null)
			{
				continue;
			}
			Interactable parentInteractable = component.ParentInteractable;
			if (!(parentInteractable == null) && ((uint)parentInteractable.ValidToolTagsMask & (uint)ToolTags) != 0)
			{
				Vector3 lhs = parentInteractable.transform.position - rayOrigin;
				float magnitude = lhs.magnitude;
				lhs /= magnitude;
				if (!(Vector3.Dot(lhs, rayDirection) < num2) && (interactable == null || magnitude < num))
				{
					interactable = parentInteractable;
					num = magnitude;
				}
			}
		}
		return interactable;
	}

	public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
	{
		_rayToolView.SetFocusedInteractable(focusedInteractable);
		_focusedInteractable = focusedInteractable;
	}

	public override void DeFocus()
	{
		_rayToolView.SetFocusedInteractable(null);
		_focusedInteractable = null;
	}
}
