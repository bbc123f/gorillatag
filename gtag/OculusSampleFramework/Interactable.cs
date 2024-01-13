using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework;

public abstract class Interactable : MonoBehaviour
{
	[Serializable]
	public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
	{
	}

	protected ColliderZone _proximityZoneCollider;

	protected ColliderZone _contactZoneCollider;

	protected ColliderZone _actionZoneCollider;

	public InteractableStateArgsEvent InteractableStateChanged;

	public ColliderZone ProximityCollider => _proximityZoneCollider;

	public ColliderZone ContactCollider => _contactZoneCollider;

	public ColliderZone ActionCollider => _actionZoneCollider;

	public virtual int ValidToolTagsMask => -1;

	public event Action<ColliderZoneArgs> ProximityZoneEvent;

	public event Action<ColliderZoneArgs> ContactZoneEvent;

	public event Action<ColliderZoneArgs> ActionZoneEvent;

	protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
	{
		if (this.ProximityZoneEvent != null)
		{
			this.ProximityZoneEvent(args);
		}
	}

	protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
	{
		if (this.ContactZoneEvent != null)
		{
			this.ContactZoneEvent(args);
		}
	}

	protected virtual void OnActionZoneEvent(ColliderZoneArgs args)
	{
		if (this.ActionZoneEvent != null)
		{
			this.ActionZoneEvent(args);
		}
	}

	public abstract void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth);

	protected virtual void Awake()
	{
		InteractableRegistry.RegisterInteractable(this);
	}

	protected virtual void OnDestroy()
	{
		InteractableRegistry.UnregisterInteractable(this);
	}
}
