using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OculusSampleFramework
{
	public abstract class InteractableTool : MonoBehaviour
	{
		public Transform ToolTransform
		{
			get
			{
				return base.transform;
			}
		}

		public bool IsRightHandedTool
		{
			[CompilerGenerated]
			get
			{
				return this.<IsRightHandedTool>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<IsRightHandedTool>k__BackingField = value;
			}
		}

		public abstract InteractableToolTags ToolTags { get; }

		public abstract ToolInputState ToolInputState { get; }

		public abstract bool IsFarFieldTool { get; }

		public Vector3 Velocity
		{
			[CompilerGenerated]
			get
			{
				return this.<Velocity>k__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				this.<Velocity>k__BackingField = value;
			}
		}

		public Vector3 InteractionPosition
		{
			[CompilerGenerated]
			get
			{
				return this.<InteractionPosition>k__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				this.<InteractionPosition>k__BackingField = value;
			}
		}

		public List<InteractableCollisionInfo> GetCurrentIntersectingObjects()
		{
			return this._currentIntersectingObjects;
		}

		public abstract List<InteractableCollisionInfo> GetNextIntersectingObjects();

		public abstract void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone);

		public abstract void DeFocus();

		public abstract bool EnableState { get; set; }

		public abstract void Initialize();

		public KeyValuePair<Interactable, InteractableCollisionInfo> GetFirstCurrentCollisionInfo()
		{
			return this._currInteractableToCollisionInfos.First<KeyValuePair<Interactable, InteractableCollisionInfo>>();
		}

		public void ClearAllCurrentCollisionInfos()
		{
			this._currInteractableToCollisionInfos.Clear();
		}

		public virtual void UpdateCurrentCollisionsBasedOnDepth()
		{
			this._currInteractableToCollisionInfos.Clear();
			foreach (InteractableCollisionInfo interactableCollisionInfo in this._currentIntersectingObjects)
			{
				Interactable parentInteractable = interactableCollisionInfo.InteractableCollider.ParentInteractable;
				InteractableCollisionDepth collisionDepth = interactableCollisionInfo.CollisionDepth;
				InteractableCollisionInfo interactableCollisionInfo2 = null;
				if (!this._currInteractableToCollisionInfos.TryGetValue(parentInteractable, out interactableCollisionInfo2))
				{
					this._currInteractableToCollisionInfos[parentInteractable] = interactableCollisionInfo;
				}
				else if (interactableCollisionInfo2.CollisionDepth < collisionDepth)
				{
					interactableCollisionInfo2.InteractableCollider = interactableCollisionInfo.InteractableCollider;
					interactableCollisionInfo2.CollisionDepth = collisionDepth;
				}
			}
		}

		public virtual void UpdateLatestCollisionData()
		{
			this._addedInteractables.Clear();
			this._removedInteractables.Clear();
			this._remainingInteractables.Clear();
			foreach (Interactable interactable in this._currInteractableToCollisionInfos.Keys)
			{
				if (!this._prevInteractableToCollisionInfos.ContainsKey(interactable))
				{
					this._addedInteractables.Add(interactable);
				}
				else
				{
					this._remainingInteractables.Add(interactable);
				}
			}
			foreach (Interactable interactable2 in this._prevInteractableToCollisionInfos.Keys)
			{
				if (!this._currInteractableToCollisionInfos.ContainsKey(interactable2))
				{
					this._removedInteractables.Add(interactable2);
				}
			}
			foreach (Interactable interactable3 in this._removedInteractables)
			{
				interactable3.UpdateCollisionDepth(this, this._prevInteractableToCollisionInfos[interactable3].CollisionDepth, InteractableCollisionDepth.None);
			}
			foreach (Interactable interactable4 in this._addedInteractables)
			{
				InteractableCollisionDepth collisionDepth = this._currInteractableToCollisionInfos[interactable4].CollisionDepth;
				interactable4.UpdateCollisionDepth(this, InteractableCollisionDepth.None, collisionDepth);
			}
			foreach (Interactable interactable5 in this._remainingInteractables)
			{
				InteractableCollisionDepth collisionDepth2 = this._currInteractableToCollisionInfos[interactable5].CollisionDepth;
				InteractableCollisionDepth collisionDepth3 = this._prevInteractableToCollisionInfos[interactable5].CollisionDepth;
				interactable5.UpdateCollisionDepth(this, collisionDepth3, collisionDepth2);
			}
			this._prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>(this._currInteractableToCollisionInfos);
		}

		protected InteractableTool()
		{
		}

		[CompilerGenerated]
		private bool <IsRightHandedTool>k__BackingField;

		[CompilerGenerated]
		private Vector3 <Velocity>k__BackingField;

		[CompilerGenerated]
		private Vector3 <InteractionPosition>k__BackingField;

		protected List<InteractableCollisionInfo> _currentIntersectingObjects = new List<InteractableCollisionInfo>();

		private List<Interactable> _addedInteractables = new List<Interactable>();

		private List<Interactable> _removedInteractables = new List<Interactable>();

		private List<Interactable> _remainingInteractables = new List<Interactable>();

		private Dictionary<Interactable, InteractableCollisionInfo> _currInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();

		private Dictionary<Interactable, InteractableCollisionInfo> _prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();
	}
}
