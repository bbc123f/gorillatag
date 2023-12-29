using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	public abstract class Interactable : MonoBehaviour
	{
		public ColliderZone ProximityCollider
		{
			get
			{
				return this._proximityZoneCollider;
			}
		}

		public ColliderZone ContactCollider
		{
			get
			{
				return this._contactZoneCollider;
			}
		}

		public ColliderZone ActionCollider
		{
			get
			{
				return this._actionZoneCollider;
			}
		}

		public virtual int ValidToolTagsMask
		{
			get
			{
				return -1;
			}
		}

		public event Action<ColliderZoneArgs> ProximityZoneEvent;

		protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
		{
			if (this.ProximityZoneEvent != null)
			{
				this.ProximityZoneEvent(args);
			}
		}

		public event Action<ColliderZoneArgs> ContactZoneEvent;

		protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
		{
			if (this.ContactZoneEvent != null)
			{
				this.ContactZoneEvent(args);
			}
		}

		public event Action<ColliderZoneArgs> ActionZoneEvent;

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

		protected ColliderZone _proximityZoneCollider;

		protected ColliderZone _contactZoneCollider;

		protected ColliderZone _actionZoneCollider;

		public Interactable.InteractableStateArgsEvent InteractableStateChanged;

		[Serializable]
		public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
		{
		}
	}
}
