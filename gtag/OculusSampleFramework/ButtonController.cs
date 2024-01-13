using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework;

public class ButtonController : Interactable
{
	public enum ContactTest
	{
		PerpenTest,
		BackwardsPress
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
	private ContactTest[] _contactTests;

	[SerializeField]
	private Transform _buttonPlaneCenter;

	[SerializeField]
	private bool _makeSureToolIsOnPositiveSide = true;

	[SerializeField]
	private Vector3 _localButtonDirection = Vector3.down;

	[SerializeField]
	private InteractableToolTags[] _allValidToolsTags = new InteractableToolTags[1] { InteractableToolTags.All };

	private int _toolTagsMask;

	private InteractableState _currentButtonState;

	private Dictionary<InteractableTool, InteractableState> _toolToState = new Dictionary<InteractableTool, InteractableState>();

	public override int ValidToolTagsMask => _toolTagsMask;

	public Vector3 LocalButtonDirection => _localButtonDirection;

	protected override void Awake()
	{
		base.Awake();
		InteractableToolTags[] allValidToolsTags = _allValidToolsTags;
		foreach (InteractableToolTags interactableToolTags in allValidToolsTags)
		{
			_toolTagsMask |= (int)interactableToolTags;
		}
		_proximityZoneCollider = _proximityZone.GetComponent<ColliderZone>();
		_contactZoneCollider = _contactZone.GetComponent<ColliderZone>();
		_actionZoneCollider = _actionZone.GetComponent<ColliderZone>();
	}

	private void FireInteractionEventsOnDepth(InteractableCollisionDepth oldDepth, InteractableTool collidingTool, InteractionType interactionType)
	{
		switch (oldDepth)
		{
		case InteractableCollisionDepth.Action:
			OnActionZoneEvent(new ColliderZoneArgs(base.ActionCollider, Time.frameCount, collidingTool, interactionType));
			break;
		case InteractableCollisionDepth.Contact:
			OnContactZoneEvent(new ColliderZoneArgs(base.ContactCollider, Time.frameCount, collidingTool, interactionType));
			break;
		case InteractableCollisionDepth.Proximity:
			OnProximityZoneEvent(new ColliderZoneArgs(base.ProximityCollider, Time.frameCount, collidingTool, interactionType));
			break;
		}
	}

	public override void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth)
	{
		bool isFarFieldTool = interactableTool.IsFarFieldTool;
		if (!isFarFieldTool && _toolToState.Keys.Count > 0 && !_toolToState.ContainsKey(interactableTool))
		{
			return;
		}
		InteractableState currentButtonState = _currentButtonState;
		Vector3 vector = base.transform.TransformDirection(_localButtonDirection);
		bool validContact = IsValidContact(interactableTool, vector) || interactableTool.IsFarFieldTool;
		bool toolIsInProximity = newCollisionDepth >= InteractableCollisionDepth.Proximity;
		bool flag = newCollisionDepth == InteractableCollisionDepth.Contact;
		bool flag2 = newCollisionDepth == InteractableCollisionDepth.Action;
		bool flag3 = oldCollisionDepth != newCollisionDepth;
		if (flag3)
		{
			FireInteractionEventsOnDepth(oldCollisionDepth, interactableTool, InteractionType.Exit);
			FireInteractionEventsOnDepth(newCollisionDepth, interactableTool, InteractionType.Enter);
		}
		else
		{
			FireInteractionEventsOnDepth(newCollisionDepth, interactableTool, InteractionType.Stay);
		}
		InteractableState interactableState = currentButtonState;
		if (interactableTool.IsFarFieldTool)
		{
			interactableState = (flag ? InteractableState.ContactState : (flag2 ? InteractableState.ActionState : InteractableState.Default));
		}
		else
		{
			Plane plane = new Plane(-vector, _buttonPlaneCenter.position);
			bool onPositiveSideOfInteractable = !_makeSureToolIsOnPositiveSide || plane.GetSide(interactableTool.InteractionPosition);
			interactableState = GetUpcomingStateNearField(currentButtonState, newCollisionDepth, flag2, flag, toolIsInProximity, validContact, onPositiveSideOfInteractable);
		}
		if (interactableState != 0)
		{
			_toolToState[interactableTool] = interactableState;
		}
		else
		{
			_toolToState.Remove(interactableTool);
		}
		if (isFarFieldTool)
		{
			foreach (InteractableState value in _toolToState.Values)
			{
				if (interactableState < value)
				{
					interactableState = value;
				}
			}
		}
		if (currentButtonState != interactableState)
		{
			_currentButtonState = interactableState;
			InteractionType interactionType = ((!flag3) ? InteractionType.Stay : ((newCollisionDepth == InteractableCollisionDepth.None) ? InteractionType.Exit : InteractionType.Enter));
			ColliderZone collider = ((_currentButtonState == InteractableState.ProximityState) ? base.ProximityCollider : ((_currentButtonState == InteractableState.ContactState) ? base.ContactCollider : ((_currentButtonState == InteractableState.ActionState) ? base.ActionCollider : null)));
			if (InteractableStateChanged != null)
			{
				InteractableStateChanged.Invoke(new InteractableStateArgs(this, interactableTool, _currentButtonState, currentButtonState, new ColliderZoneArgs(collider, Time.frameCount, interactableTool, interactionType)));
			}
		}
	}

	private InteractableState GetUpcomingStateNearField(InteractableState oldState, InteractableCollisionDepth newCollisionDepth, bool toolIsInActionZone, bool toolIsInContactZone, bool toolIsInProximity, bool validContact, bool onPositiveSideOfInteractable)
	{
		InteractableState result = oldState;
		switch (oldState)
		{
		case InteractableState.ActionState:
			if (!toolIsInActionZone)
			{
				result = ((!toolIsInContactZone) ? (toolIsInProximity ? InteractableState.ProximityState : InteractableState.Default) : InteractableState.ContactState);
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
		}
		return result;
	}

	private bool IsValidContact(InteractableTool collidingTool, Vector3 buttonDirection)
	{
		if (_contactTests == null || collidingTool.IsFarFieldTool)
		{
			return true;
		}
		ContactTest[] contactTests = _contactTests;
		foreach (ContactTest contactTest in contactTests)
		{
			if (contactTest == ContactTest.BackwardsPress)
			{
				if (!PassEntryTest(collidingTool, buttonDirection))
				{
					return false;
				}
			}
			else if (!PassPerpTest(collidingTool, buttonDirection))
			{
				return false;
			}
		}
		return true;
	}

	private bool PassEntryTest(InteractableTool collidingTool, Vector3 buttonDirection)
	{
		if (Vector3.Dot(collidingTool.Velocity.normalized, buttonDirection) < 0.8f)
		{
			return false;
		}
		return true;
	}

	private bool PassPerpTest(InteractableTool collidingTool, Vector3 buttonDirection)
	{
		Vector3 vector = collidingTool.ToolTransform.right;
		if (collidingTool.IsRightHandedTool)
		{
			vector = -vector;
		}
		if (Vector3.Dot(vector, buttonDirection) < 0.5f)
		{
			return false;
		}
		return true;
	}
}
