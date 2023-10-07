using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CC RID: 716
	public class ButtonController : Interactable
	{
		// Token: 0x17000128 RID: 296
		// (get) Token: 0x0600134C RID: 4940 RVA: 0x0006F6AA File Offset: 0x0006D8AA
		public override int ValidToolTagsMask
		{
			get
			{
				return this._toolTagsMask;
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x0600134D RID: 4941 RVA: 0x0006F6B2 File Offset: 0x0006D8B2
		public Vector3 LocalButtonDirection
		{
			get
			{
				return this._localButtonDirection;
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600134E RID: 4942 RVA: 0x0006F6BA File Offset: 0x0006D8BA
		// (set) Token: 0x0600134F RID: 4943 RVA: 0x0006F6C2 File Offset: 0x0006D8C2
		public InteractableState CurrentButtonState { get; private set; }

		// Token: 0x06001350 RID: 4944 RVA: 0x0006F6CC File Offset: 0x0006D8CC
		protected override void Awake()
		{
			base.Awake();
			foreach (InteractableToolTags interactableToolTags in this._allValidToolsTags)
			{
				this._toolTagsMask |= (int)interactableToolTags;
			}
			this._proximityZoneCollider = this._proximityZone.GetComponent<ColliderZone>();
			this._contactZoneCollider = this._contactZone.GetComponent<ColliderZone>();
			this._actionZoneCollider = this._actionZone.GetComponent<ColliderZone>();
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x0006F73C File Offset: 0x0006D93C
		private void FireInteractionEventsOnDepth(InteractableCollisionDepth oldDepth, InteractableTool collidingTool, InteractionType interactionType)
		{
			switch (oldDepth)
			{
			case InteractableCollisionDepth.Proximity:
				this.OnProximityZoneEvent(new ColliderZoneArgs(base.ProximityCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			case InteractableCollisionDepth.Contact:
				this.OnContactZoneEvent(new ColliderZoneArgs(base.ContactCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			case InteractableCollisionDepth.Action:
				this.OnActionZoneEvent(new ColliderZoneArgs(base.ActionCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			default:
				return;
			}
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x0006F7AC File Offset: 0x0006D9AC
		public override void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth)
		{
			bool isFarFieldTool = interactableTool.IsFarFieldTool;
			if (!isFarFieldTool && !this._allowMultipleNearFieldInteraction && this._toolToState.Keys.Count > 0 && !this._toolToState.ContainsKey(interactableTool))
			{
				return;
			}
			InteractableState currentButtonState = this.CurrentButtonState;
			Vector3 vector = base.transform.TransformDirection(this._localButtonDirection);
			bool validContact = this.IsValidContact(interactableTool, vector) || interactableTool.IsFarFieldTool;
			bool toolIsInProximity = newCollisionDepth >= InteractableCollisionDepth.Proximity;
			bool flag = newCollisionDepth == InteractableCollisionDepth.Contact;
			bool flag2 = newCollisionDepth == InteractableCollisionDepth.Action;
			bool flag3 = oldCollisionDepth != newCollisionDepth;
			if (flag3)
			{
				this.FireInteractionEventsOnDepth(oldCollisionDepth, interactableTool, InteractionType.Exit);
				this.FireInteractionEventsOnDepth(newCollisionDepth, interactableTool, InteractionType.Enter);
			}
			else
			{
				this.FireInteractionEventsOnDepth(newCollisionDepth, interactableTool, InteractionType.Stay);
			}
			InteractableState interactableState = currentButtonState;
			if (interactableTool.IsFarFieldTool)
			{
				interactableState = (flag ? InteractableState.ContactState : (flag2 ? InteractableState.ActionState : InteractableState.Default));
			}
			else
			{
				Plane plane = new Plane(-vector, this._buttonPlaneCenter.position);
				bool onPositiveSideOfInteractable = !this._makeSureToolIsOnPositiveSide || plane.GetSide(interactableTool.InteractionPosition);
				interactableState = this.GetUpcomingStateNearField(currentButtonState, newCollisionDepth, flag2, flag, toolIsInProximity, validContact, onPositiveSideOfInteractable);
			}
			if (interactableState != InteractableState.Default)
			{
				this._toolToState[interactableTool] = interactableState;
			}
			else
			{
				this._toolToState.Remove(interactableTool);
			}
			if (isFarFieldTool || this._allowMultipleNearFieldInteraction)
			{
				foreach (InteractableState interactableState2 in this._toolToState.Values)
				{
					if (interactableState < interactableState2)
					{
						interactableState = interactableState2;
					}
				}
			}
			if (currentButtonState != interactableState)
			{
				this.CurrentButtonState = interactableState;
				InteractionType interactionType = (!flag3) ? InteractionType.Stay : ((newCollisionDepth == InteractableCollisionDepth.None) ? InteractionType.Exit : InteractionType.Enter);
				ColliderZone collider;
				switch (this.CurrentButtonState)
				{
				case InteractableState.ProximityState:
					collider = base.ProximityCollider;
					break;
				case InteractableState.ContactState:
					collider = base.ContactCollider;
					break;
				case InteractableState.ActionState:
					collider = base.ActionCollider;
					break;
				default:
					collider = null;
					break;
				}
				Interactable.InteractableStateArgsEvent interactableStateChanged = this.InteractableStateChanged;
				if (interactableStateChanged == null)
				{
					return;
				}
				interactableStateChanged.Invoke(new InteractableStateArgs(this, interactableTool, this.CurrentButtonState, currentButtonState, new ColliderZoneArgs(collider, (float)Time.frameCount, interactableTool, interactionType)));
			}
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x0006F9D4 File Offset: 0x0006DBD4
		private InteractableState GetUpcomingStateNearField(InteractableState oldState, InteractableCollisionDepth newCollisionDepth, bool toolIsInActionZone, bool toolIsInContactZone, bool toolIsInProximity, bool validContact, bool onPositiveSideOfInteractable)
		{
			InteractableState result = oldState;
			switch (oldState)
			{
			case InteractableState.Default:
				if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
				{
					result = ((newCollisionDepth == InteractableCollisionDepth.Action) ? InteractableState.ActionState : InteractableState.ContactState);
				}
				else if (toolIsInProximity)
				{
					result = InteractableState.ProximityState;
				}
				break;
			case InteractableState.ProximityState:
				if (newCollisionDepth < InteractableCollisionDepth.Proximity)
				{
					result = InteractableState.Default;
				}
				else if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
				{
					result = ((newCollisionDepth == InteractableCollisionDepth.Action) ? InteractableState.ActionState : InteractableState.ContactState);
				}
				break;
			case InteractableState.ContactState:
				if (newCollisionDepth < InteractableCollisionDepth.Contact)
				{
					result = (toolIsInProximity ? InteractableState.ProximityState : InteractableState.Default);
				}
				else if (toolIsInActionZone && validContact && onPositiveSideOfInteractable)
				{
					result = InteractableState.ActionState;
				}
				break;
			case InteractableState.ActionState:
				if (!toolIsInActionZone)
				{
					if (toolIsInContactZone)
					{
						result = InteractableState.ContactState;
					}
					else if (toolIsInProximity)
					{
						result = InteractableState.ProximityState;
					}
					else
					{
						result = InteractableState.Default;
					}
				}
				break;
			}
			return result;
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x0006FA6C File Offset: 0x0006DC6C
		public void ForceResetButton()
		{
			InteractableState currentButtonState = this.CurrentButtonState;
			this.CurrentButtonState = InteractableState.Default;
			Interactable.InteractableStateArgsEvent interactableStateChanged = this.InteractableStateChanged;
			if (interactableStateChanged == null)
			{
				return;
			}
			interactableStateChanged.Invoke(new InteractableStateArgs(this, null, this.CurrentButtonState, currentButtonState, new ColliderZoneArgs(base.ContactCollider, (float)Time.frameCount, null, InteractionType.Exit)));
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x0006FAB8 File Offset: 0x0006DCB8
		private bool IsValidContact(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			if (this._contactTests == null || collidingTool.IsFarFieldTool)
			{
				return true;
			}
			ButtonController.ContactTest[] contactTests = this._contactTests;
			for (int i = 0; i < contactTests.Length; i++)
			{
				if (contactTests[i] == ButtonController.ContactTest.BackwardsPress)
				{
					if (!this.PassEntryTest(collidingTool, buttonDirection))
					{
						return false;
					}
				}
				else if (!this.PassPerpTest(collidingTool, buttonDirection))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x0006FB0C File Offset: 0x0006DD0C
		private bool PassEntryTest(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			return Vector3.Dot(collidingTool.Velocity.normalized, buttonDirection) >= 0.8f;
		}

		// Token: 0x06001357 RID: 4951 RVA: 0x0006FB38 File Offset: 0x0006DD38
		private bool PassPerpTest(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			Vector3 vector = collidingTool.ToolTransform.right;
			if (collidingTool.IsRightHandedTool)
			{
				vector = -vector;
			}
			return Vector3.Dot(vector, buttonDirection) >= 0.5f;
		}

		// Token: 0x04001623 RID: 5667
		private const float ENTRY_DOT_THRESHOLD = 0.8f;

		// Token: 0x04001624 RID: 5668
		private const float PERP_DOT_THRESHOLD = 0.5f;

		// Token: 0x04001625 RID: 5669
		[SerializeField]
		private GameObject _proximityZone;

		// Token: 0x04001626 RID: 5670
		[SerializeField]
		private GameObject _contactZone;

		// Token: 0x04001627 RID: 5671
		[SerializeField]
		private GameObject _actionZone;

		// Token: 0x04001628 RID: 5672
		[SerializeField]
		private ButtonController.ContactTest[] _contactTests;

		// Token: 0x04001629 RID: 5673
		[SerializeField]
		private Transform _buttonPlaneCenter;

		// Token: 0x0400162A RID: 5674
		[SerializeField]
		private bool _makeSureToolIsOnPositiveSide = true;

		// Token: 0x0400162B RID: 5675
		[SerializeField]
		private Vector3 _localButtonDirection = Vector3.down;

		// Token: 0x0400162C RID: 5676
		[SerializeField]
		private InteractableToolTags[] _allValidToolsTags = new InteractableToolTags[]
		{
			InteractableToolTags.All
		};

		// Token: 0x0400162D RID: 5677
		private int _toolTagsMask;

		// Token: 0x0400162E RID: 5678
		[SerializeField]
		private bool _allowMultipleNearFieldInteraction;

		// Token: 0x04001630 RID: 5680
		private Dictionary<InteractableTool, InteractableState> _toolToState = new Dictionary<InteractableTool, InteractableState>();

		// Token: 0x020004E5 RID: 1253
		public enum ContactTest
		{
			// Token: 0x04002060 RID: 8288
			PerpenTest,
			// Token: 0x04002061 RID: 8289
			BackwardsPress
		}
	}
}
