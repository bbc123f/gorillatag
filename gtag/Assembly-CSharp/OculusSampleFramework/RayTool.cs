using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E3 RID: 739
	public class RayTool : InteractableTool
	{
		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060013E7 RID: 5095 RVA: 0x00071525 File Offset: 0x0006F725
		public override InteractableToolTags ToolTags
		{
			get
			{
				return InteractableToolTags.Ray;
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060013E8 RID: 5096 RVA: 0x00071528 File Offset: 0x0006F728
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

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060013E9 RID: 5097 RVA: 0x00071558 File Offset: 0x0006F758
		public override bool IsFarFieldTool
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060013EA RID: 5098 RVA: 0x0007155B File Offset: 0x0006F75B
		// (set) Token: 0x060013EB RID: 5099 RVA: 0x00071568 File Offset: 0x0006F768
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

		// Token: 0x060013EC RID: 5100 RVA: 0x00071576 File Offset: 0x0006F776
		public override void Initialize()
		{
			InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
			this._rayToolView.InteractableTool = this;
			this._coneAngleReleaseDegrees = this._coneAngleDegrees * 1.2f;
			this._initialized = true;
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x000715A8 File Offset: 0x0006F7A8
		private void OnDestroy()
		{
			if (InteractableToolsInputRouter.Instance != null)
			{
				InteractableToolsInputRouter.Instance.UnregisterInteractableTool(this);
			}
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x000715C4 File Offset: 0x0006F7C4
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

		// Token: 0x060013EF RID: 5103 RVA: 0x000716A3 File Offset: 0x0006F8A3
		private Vector3 GetRayCastOrigin()
		{
			return base.transform.position + 0.8f * base.transform.forward;
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x000716CC File Offset: 0x0006F8CC
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

		// Token: 0x060013F1 RID: 5105 RVA: 0x000717D8 File Offset: 0x0006F9D8
		private bool HasRayReleasedInteractable(Interactable focusedInteractable)
		{
			Vector3 position = base.transform.position;
			Vector3 forward = base.transform.forward;
			float num = Mathf.Cos(this._coneAngleReleaseDegrees * 0.017453292f);
			Vector3 lhs = focusedInteractable.transform.position - position;
			lhs.Normalize();
			return Vector3.Dot(lhs, forward) < num;
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x00071834 File Offset: 0x0006FA34
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

		// Token: 0x060013F3 RID: 5107 RVA: 0x00071874 File Offset: 0x0006FA74
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

		// Token: 0x060013F4 RID: 5108 RVA: 0x00071924 File Offset: 0x0006FB24
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

		// Token: 0x060013F5 RID: 5109 RVA: 0x00071A47 File Offset: 0x0006FC47
		public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
		{
			this._rayToolView.SetFocusedInteractable(focusedInteractable);
			this._focusedInteractable = focusedInteractable;
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x00071A5C File Offset: 0x0006FC5C
		public override void DeFocus()
		{
			this._rayToolView.SetFocusedInteractable(null);
			this._focusedInteractable = null;
		}

		// Token: 0x040016A1 RID: 5793
		private const float MINIMUM_RAY_CAST_DISTANCE = 0.8f;

		// Token: 0x040016A2 RID: 5794
		private const float COLLIDER_RADIUS = 0.01f;

		// Token: 0x040016A3 RID: 5795
		private const int NUM_MAX_PRIMARY_HITS = 10;

		// Token: 0x040016A4 RID: 5796
		private const int NUM_MAX_SECONDARY_HITS = 25;

		// Token: 0x040016A5 RID: 5797
		private const int NUM_COLLIDERS_TO_TEST = 20;

		// Token: 0x040016A6 RID: 5798
		[SerializeField]
		private RayToolView _rayToolView;

		// Token: 0x040016A7 RID: 5799
		[Range(0f, 45f)]
		[SerializeField]
		private float _coneAngleDegrees = 20f;

		// Token: 0x040016A8 RID: 5800
		[SerializeField]
		private float _farFieldMaxDistance = 5f;

		// Token: 0x040016A9 RID: 5801
		private PinchStateModule _pinchStateModule = new PinchStateModule();

		// Token: 0x040016AA RID: 5802
		private Interactable _focusedInteractable;

		// Token: 0x040016AB RID: 5803
		private Collider[] _collidersOverlapped = new Collider[20];

		// Token: 0x040016AC RID: 5804
		private Interactable _currInteractableCastedAgainst;

		// Token: 0x040016AD RID: 5805
		private float _coneAngleReleaseDegrees;

		// Token: 0x040016AE RID: 5806
		private RaycastHit[] _primaryHits = new RaycastHit[10];

		// Token: 0x040016AF RID: 5807
		private Collider[] _secondaryOverlapResults = new Collider[25];

		// Token: 0x040016B0 RID: 5808
		private bool _initialized;
	}
}
