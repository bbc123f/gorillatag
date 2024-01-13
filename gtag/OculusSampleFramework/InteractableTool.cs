using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OculusSampleFramework;

public abstract class InteractableTool : MonoBehaviour
{
	protected List<InteractableCollisionInfo> _currentIntersectingObjects = new List<InteractableCollisionInfo>();

	private List<Interactable> _addedInteractables = new List<Interactable>();

	private List<Interactable> _removedInteractables = new List<Interactable>();

	private List<Interactable> _remainingInteractables = new List<Interactable>();

	private Dictionary<Interactable, InteractableCollisionInfo> _currInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();

	private Dictionary<Interactable, InteractableCollisionInfo> _prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();

	public Transform ToolTransform => base.transform;

	public bool IsRightHandedTool { get; set; }

	public abstract InteractableToolTags ToolTags { get; }

	public abstract ToolInputState ToolInputState { get; }

	public abstract bool IsFarFieldTool { get; }

	public Vector3 Velocity { get; protected set; }

	public Vector3 InteractionPosition { get; protected set; }

	public abstract bool EnableState { get; set; }

	public List<InteractableCollisionInfo> GetCurrentIntersectingObjects()
	{
		return _currentIntersectingObjects;
	}

	public abstract List<InteractableCollisionInfo> GetNextIntersectingObjects();

	public abstract void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone);

	public abstract void DeFocus();

	public abstract void Initialize();

	public KeyValuePair<Interactable, InteractableCollisionInfo> GetFirstCurrentCollisionInfo()
	{
		return _currInteractableToCollisionInfos.First();
	}

	public void ClearAllCurrentCollisionInfos()
	{
		_currInteractableToCollisionInfos.Clear();
	}

	public virtual void UpdateCurrentCollisionsBasedOnDepth()
	{
		_currInteractableToCollisionInfos.Clear();
		foreach (InteractableCollisionInfo currentIntersectingObject in _currentIntersectingObjects)
		{
			Interactable parentInteractable = currentIntersectingObject.InteractableCollider.ParentInteractable;
			InteractableCollisionDepth collisionDepth = currentIntersectingObject.CollisionDepth;
			InteractableCollisionInfo value = null;
			if (!_currInteractableToCollisionInfos.TryGetValue(parentInteractable, out value))
			{
				_currInteractableToCollisionInfos[parentInteractable] = currentIntersectingObject;
			}
			else if (value.CollisionDepth < collisionDepth)
			{
				value.InteractableCollider = currentIntersectingObject.InteractableCollider;
				value.CollisionDepth = collisionDepth;
			}
		}
	}

	public virtual void UpdateLatestCollisionData()
	{
		_addedInteractables.Clear();
		_removedInteractables.Clear();
		_remainingInteractables.Clear();
		foreach (Interactable key in _currInteractableToCollisionInfos.Keys)
		{
			if (!_prevInteractableToCollisionInfos.ContainsKey(key))
			{
				_addedInteractables.Add(key);
			}
			else
			{
				_remainingInteractables.Add(key);
			}
		}
		foreach (Interactable key2 in _prevInteractableToCollisionInfos.Keys)
		{
			if (!_currInteractableToCollisionInfos.ContainsKey(key2))
			{
				_removedInteractables.Add(key2);
			}
		}
		foreach (Interactable removedInteractable in _removedInteractables)
		{
			removedInteractable.UpdateCollisionDepth(this, _prevInteractableToCollisionInfos[removedInteractable].CollisionDepth, InteractableCollisionDepth.None);
		}
		foreach (Interactable addedInteractable in _addedInteractables)
		{
			InteractableCollisionDepth collisionDepth = _currInteractableToCollisionInfos[addedInteractable].CollisionDepth;
			addedInteractable.UpdateCollisionDepth(this, InteractableCollisionDepth.None, collisionDepth);
		}
		foreach (Interactable remainingInteractable in _remainingInteractables)
		{
			InteractableCollisionDepth collisionDepth2 = _currInteractableToCollisionInfos[remainingInteractable].CollisionDepth;
			InteractableCollisionDepth collisionDepth3 = _prevInteractableToCollisionInfos[remainingInteractable].CollisionDepth;
			remainingInteractable.UpdateCollisionDepth(this, collisionDepth3, collisionDepth2);
		}
		_prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>(_currInteractableToCollisionInfos);
	}
}
