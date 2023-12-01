using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	public class RayTool : InteractableTool
	{
		public override InteractableToolTags ToolTags
		{
			get
			{
				return InteractableToolTags.Ray;
			}
		}

		public override ToolInputState ToolInputState
		{
			get
			{
				if (this._pinchStateModule.PinchDownOnFocusedObject)
				{
					return ToolInputState.PrimaryInputDown;
				}
				if (this._pinchStateModule.PinchSteadyOnFocusedObject)
				{
					return ToolInputState.PrimaryInputDownStay;
				}
				if (this._pinchStateModule.PinchUpAndDownOnFocusedObject)
				{
					return ToolInputState.PrimaryInputUp;
				}
				return ToolInputState.Inactive;
			}
		}

		public override bool IsFarFieldTool
		{
			get
			{
				return true;
			}
		}

		public override bool EnableState
		{
			get
			{
				return this._rayToolView.EnableState;
			}
			set
			{
				this._rayToolView.EnableState = value;
			}
		}

		public override void Initialize()
		{
			InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
			this._rayToolView.InteractableTool = this;
			this._coneAngleReleaseDegrees = this._coneAngleDegrees * 1.2f;
			this._initialized = true;
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
			if (!HandsManager.Instance || !HandsManager.Instance.IsInitialized() || !this._initialized)
			{
				return;
			}
			OVRHand ovrhand = base.IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand;
			Transform pointerPose = ovrhand.PointerPose;
			base.transform.position = pointerPose.position;
			base.transform.rotation = pointerPose.rotation;
			Vector3 interactionPosition = base.InteractionPosition;
			Vector3 position = base.transform.position;
			base.Velocity = (position - interactionPosition) / Time.deltaTime;
			base.InteractionPosition = position;
			this._pinchStateModule.UpdateState(ovrhand, this._focusedInteractable);
			this._rayToolView.ToolActivateState = (this._pinchStateModule.PinchSteadyOnFocusedObject || this._pinchStateModule.PinchDownOnFocusedObject);
		}

		private Vector3 GetRayCastOrigin()
		{
			return base.transform.position + 0.8f * base.transform.forward;
		}

		public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
		{
			if (!this._initialized)
			{
				return this._currentIntersectingObjects;
			}
			if (this._currInteractableCastedAgainst != null && this.HasRayReleasedInteractable(this._currInteractableCastedAgainst))
			{
				this._currInteractableCastedAgainst = null;
			}
			if (this._currInteractableCastedAgainst == null)
			{
				this._currentIntersectingObjects.Clear();
				this._currInteractableCastedAgainst = this.FindTargetInteractable();
				if (this._currInteractableCastedAgainst != null)
				{
					int num = Physics.OverlapSphereNonAlloc(this._currInteractableCastedAgainst.transform.position, 0.01f, this._collidersOverlapped);
					for (int i = 0; i < num; i++)
					{
						ColliderZone component = this._collidersOverlapped[i].GetComponent<ColliderZone>();
						if (component != null)
						{
							Interactable parentInteractable = component.ParentInteractable;
							if (!(parentInteractable == null) && !(parentInteractable != this._currInteractableCastedAgainst))
							{
								InteractableCollisionInfo item = new InteractableCollisionInfo(component, component.CollisionDepth, this);
								this._currentIntersectingObjects.Add(item);
							}
						}
					}
					if (this._currentIntersectingObjects.Count == 0)
					{
						this._currInteractableCastedAgainst = null;
					}
				}
			}
			return this._currentIntersectingObjects;
		}

		private bool HasRayReleasedInteractable(Interactable focusedInteractable)
		{
			Vector3 position = base.transform.position;
			Vector3 forward = base.transform.forward;
			float num = Mathf.Cos(this._coneAngleReleaseDegrees * 0.017453292f);
			Vector3 lhs = focusedInteractable.transform.position - position;
			lhs.Normalize();
			return Vector3.Dot(lhs, forward) < num;
		}

		private Interactable FindTargetInteractable()
		{
			Vector3 rayCastOrigin = this.GetRayCastOrigin();
			Vector3 forward = base.transform.forward;
			Interactable interactable = this.FindPrimaryRaycastHit(rayCastOrigin, forward);
			if (interactable == null)
			{
				interactable = this.FindInteractableViaConeTest(rayCastOrigin, forward);
			}
			return interactable;
		}

		private Interactable FindPrimaryRaycastHit(Vector3 rayOrigin, Vector3 rayDirection)
		{
			Interactable interactable = null;
			int num = Physics.RaycastNonAlloc(new Ray(rayOrigin, rayDirection), this._primaryHits, float.PositiveInfinity);
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this._primaryHits[i];
				ColliderZone component = raycastHit.transform.GetComponent<ColliderZone>();
				if (component != null)
				{
					Interactable parentInteractable = component.ParentInteractable;
					if (!(parentInteractable == null) && (parentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
					{
						float magnitude = (parentInteractable.transform.position - rayOrigin).magnitude;
						if (interactable == null || magnitude < num2)
						{
							interactable = parentInteractable;
							num2 = magnitude;
						}
					}
				}
			}
			return interactable;
		}

		private Interactable FindInteractableViaConeTest(Vector3 rayOrigin, Vector3 rayDirection)
		{
			Interactable interactable = null;
			float num = 0f;
			float num2 = Mathf.Cos(this._coneAngleDegrees * 0.017453292f);
			float num3 = Mathf.Tan(0.017453292f * this._coneAngleDegrees * 0.5f) * this._farFieldMaxDistance;
			int num4 = Physics.OverlapBoxNonAlloc(rayOrigin + rayDirection * this._farFieldMaxDistance * 0.5f, new Vector3(num3, num3, this._farFieldMaxDistance * 0.5f), this._secondaryOverlapResults, base.transform.rotation);
			for (int i = 0; i < num4; i++)
			{
				ColliderZone component = this._secondaryOverlapResults[i].GetComponent<ColliderZone>();
				if (component != null)
				{
					Interactable parentInteractable = component.ParentInteractable;
					if (!(parentInteractable == null) && (parentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
					{
						Vector3 vector = parentInteractable.transform.position - rayOrigin;
						float magnitude = vector.magnitude;
						vector /= magnitude;
						if (Vector3.Dot(vector, rayDirection) >= num2 && (interactable == null || magnitude < num))
						{
							interactable = parentInteractable;
							num = magnitude;
						}
					}
				}
			}
			return interactable;
		}

		public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
		{
			this._rayToolView.SetFocusedInteractable(focusedInteractable);
			this._focusedInteractable = focusedInteractable;
		}

		public override void DeFocus()
		{
			this._rayToolView.SetFocusedInteractable(null);
			this._focusedInteractable = null;
		}

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
	}
}
