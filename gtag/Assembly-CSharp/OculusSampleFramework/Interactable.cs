using System;
using System.Runtime.CompilerServices;
using System.Threading;
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

		public event Action<ColliderZoneArgs> ProximityZoneEvent
		{
			[CompilerGenerated]
			add
			{
				Action<ColliderZoneArgs> action = this.ProximityZoneEvent;
				Action<ColliderZoneArgs> action2;
				do
				{
					action2 = action;
					Action<ColliderZoneArgs> value2 = (Action<ColliderZoneArgs>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange<Action<ColliderZoneArgs>>(ref this.ProximityZoneEvent, value2, action2);
				}
				while (action != action2);
			}
			[CompilerGenerated]
			remove
			{
				Action<ColliderZoneArgs> action = this.ProximityZoneEvent;
				Action<ColliderZoneArgs> action2;
				do
				{
					action2 = action;
					Action<ColliderZoneArgs> value2 = (Action<ColliderZoneArgs>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange<Action<ColliderZoneArgs>>(ref this.ProximityZoneEvent, value2, action2);
				}
				while (action != action2);
			}
		}

		protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
		{
			if (this.ProximityZoneEvent != null)
			{
				this.ProximityZoneEvent(args);
			}
		}

		public event Action<ColliderZoneArgs> ContactZoneEvent
		{
			[CompilerGenerated]
			add
			{
				Action<ColliderZoneArgs> action = this.ContactZoneEvent;
				Action<ColliderZoneArgs> action2;
				do
				{
					action2 = action;
					Action<ColliderZoneArgs> value2 = (Action<ColliderZoneArgs>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange<Action<ColliderZoneArgs>>(ref this.ContactZoneEvent, value2, action2);
				}
				while (action != action2);
			}
			[CompilerGenerated]
			remove
			{
				Action<ColliderZoneArgs> action = this.ContactZoneEvent;
				Action<ColliderZoneArgs> action2;
				do
				{
					action2 = action;
					Action<ColliderZoneArgs> value2 = (Action<ColliderZoneArgs>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange<Action<ColliderZoneArgs>>(ref this.ContactZoneEvent, value2, action2);
				}
				while (action != action2);
			}
		}

		protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
		{
			if (this.ContactZoneEvent != null)
			{
				this.ContactZoneEvent(args);
			}
		}

		public event Action<ColliderZoneArgs> ActionZoneEvent
		{
			[CompilerGenerated]
			add
			{
				Action<ColliderZoneArgs> action = this.ActionZoneEvent;
				Action<ColliderZoneArgs> action2;
				do
				{
					action2 = action;
					Action<ColliderZoneArgs> value2 = (Action<ColliderZoneArgs>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange<Action<ColliderZoneArgs>>(ref this.ActionZoneEvent, value2, action2);
				}
				while (action != action2);
			}
			[CompilerGenerated]
			remove
			{
				Action<ColliderZoneArgs> action = this.ActionZoneEvent;
				Action<ColliderZoneArgs> action2;
				do
				{
					action2 = action;
					Action<ColliderZoneArgs> value2 = (Action<ColliderZoneArgs>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange<Action<ColliderZoneArgs>>(ref this.ActionZoneEvent, value2, action2);
				}
				while (action != action2);
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

		protected Interactable()
		{
		}

		protected ColliderZone _proximityZoneCollider;

		protected ColliderZone _contactZoneCollider;

		protected ColliderZone _actionZoneCollider;

		[CompilerGenerated]
		private Action<ColliderZoneArgs> ProximityZoneEvent;

		[CompilerGenerated]
		private Action<ColliderZoneArgs> ContactZoneEvent;

		[CompilerGenerated]
		private Action<ColliderZoneArgs> ActionZoneEvent;

		public Interactable.InteractableStateArgsEvent InteractableStateChanged;

		[Serializable]
		public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
		{
			public InteractableStateArgsEvent()
			{
			}
		}
	}
}
