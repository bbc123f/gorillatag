using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	public class ButtonController : Interactable
	{
		public override int ValidToolTagsMask
		{
			get
			{
				return this._toolTagsMask;
			}
		}

		public Vector3 LocalButtonDirection
		{
			get
			{
				return this._localButtonDirection;
			}
		}

		public InteractableState CurrentButtonState { get; private set; }

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

		public override void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth)
		{
			bool isFarFieldTool = interactableTool.IsFarFieldTool;
			if (!isFarFieldTool && !this._allowMultipleNearFieldInteraction && this._toolToState.Keys.Count > 0 && !this._toolToState.ContainsKey(interactableTool))
			{
				return;
			}
			InteractableState currentButtonState = this.CurrentButtonState;
			Vector3 vector = base.transform.TransformDirection(this._localButtonDirection);
			bool flag = this.IsValidContact(interactableTool, vector) || interactableTool.IsFarFieldTool;
			bool flag2 = newCollisionDepth >= InteractableCollisionDepth.Proximity;
			bool flag3 = newCollisionDepth == InteractableCollisionDepth.Contact;
			bool flag4 = newCollisionDepth == InteractableCollisionDepth.Action;
			bool flag5 = oldCollisionDepth != newCollisionDepth;
			if (flag5)
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
				interactableState = (flag3 ? InteractableState.ContactState : (flag4 ? InteractableState.ActionState : InteractableState.Default));
			}
			else
			{
				Plane plane = new Plane(-vector, this._buttonPlaneCenter.position);
				bool flag6 = !this._makeSureToolIsOnPositiveSide || plane.GetSide(interactableTool.InteractionPosition);
				interactableState = this.GetUpcomingStateNearField(currentButtonState, newCollisionDepth, flag4, flag3, flag2, flag, flag6);
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
				InteractionType interactionType = ((!flag5) ? InteractionType.Stay : ((newCollisionDepth == InteractableCollisionDepth.None) ? InteractionType.Exit : InteractionType.Enter));
				ColliderZone colliderZone;
				switch (this.CurrentButtonState)
				{
				case InteractableState.ProximityState:
					colliderZone = base.ProximityCollider;
					break;
				case InteractableState.ContactState:
					colliderZone = base.ContactCollider;
					break;
				case InteractableState.ActionState:
					colliderZone = base.ActionCollider;
					break;
				default:
					colliderZone = null;
					break;
				}
				Interactable.InteractableStateArgsEvent interactableStateChanged = this.InteractableStateChanged;
				if (interactableStateChanged == null)
				{
					return;
				}
				interactableStateChanged.Invoke(new InteractableStateArgs(this, interactableTool, this.CurrentButtonState, currentButtonState, new ColliderZoneArgs(colliderZone, (float)Time.frameCount, interactableTool, interactionType)));
			}
		}

		private InteractableState GetUpcomingStateNearField(InteractableState oldState, InteractableCollisionDepth newCollisionDepth, bool toolIsInActionZone, bool toolIsInContactZone, bool toolIsInProximity, bool validContact, bool onPositiveSideOfInteractable)
		{
			InteractableState interactableState = oldState;
			switch (oldState)
			{
			case InteractableState.Default:
				if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
				{
					interactableState = ((newCollisionDepth == InteractableCollisionDepth.Action) ? InteractableState.ActionState : InteractableState.ContactState);
				}
				else if (toolIsInProximity)
				{
					interactableState = InteractableState.ProximityState;
				}
				break;
			case InteractableState.ProximityState:
				if (newCollisionDepth < InteractableCollisionDepth.Proximity)
				{
					interactableState = InteractableState.Default;
				}
				else if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
				{
					interactableState = ((newCollisionDepth == InteractableCollisionDepth.Action) ? InteractableState.ActionState : InteractableState.ContactState);
				}
				break;
			case InteractableState.ContactState:
				if (newCollisionDepth < InteractableCollisionDepth.Contact)
				{
					interactableState = (toolIsInProximity ? InteractableState.ProximityState : InteractableState.Default);
				}
				else if (toolIsInActionZone && validContact && onPositiveSideOfInteractable)
				{
					interactableState = InteractableState.ActionState;
				}
				break;
			case InteractableState.ActionState:
				if (!toolIsInActionZone)
				{
					if (toolIsInContactZone)
					{
						interactableState = InteractableState.ContactState;
					}
					else if (toolIsInProximity)
					{
						interactableState = InteractableState.ProximityState;
					}
					else
					{
						interactableState = InteractableState.Default;
					}
				}
				break;
			}
			return interactableState;
		}

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

		private bool PassEntryTest(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			return Vector3.Dot(collidingTool.Velocity.normalized, buttonDirection) >= 0.8f;
		}

		private bool PassPerpTest(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			Vector3 vector = collidingTool.ToolTransform.right;
			if (collidingTool.IsRightHandedTool)
			{
				vector = -vector;
			}
			return Vector3.Dot(vector, buttonDirection) >= 0.5f;
		}

		private const float ENTRY_DOT_THRESHOLD = 0.8f;

		private const float PERP_DOT_THRESHOLD = 0.5f;

		[SerializeField]
		private GameObject _proximityZone;

		[SerializeField]
		private GameObject _contactZone;

		[SerializeField]
		private GameObject _actionZone;

		[SerializeField]
		private ButtonController.ContactTest[] _contactTests;

		[SerializeField]
		private Transform _buttonPlaneCenter;

		[SerializeField]
		private bool _makeSureToolIsOnPositiveSide = true;

		[SerializeField]
		private Vector3 _localButtonDirection = Vector3.down;

		[SerializeField]
		private InteractableToolTags[] _allValidToolsTags = new InteractableToolTags[] { InteractableToolTags.All };

		private int _toolTagsMask;

		[SerializeField]
		private bool _allowMultipleNearFieldInteraction;

		private Dictionary<InteractableTool, InteractableState> _toolToState = new Dictionary<InteractableTool, InteractableState>();

		public enum ContactTest
		{
			PerpenTest,
			BackwardsPress
		}
	}
}
