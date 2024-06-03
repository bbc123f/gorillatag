using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OculusSampleFramework
{
	public class ButtonTriggerZone : MonoBehaviour, ColliderZone
	{
		public Collider Collider
		{
			[CompilerGenerated]
			get
			{
				return this.<Collider>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Collider>k__BackingField = value;
			}
		}

		public Interactable ParentInteractable
		{
			[CompilerGenerated]
			get
			{
				return this.<ParentInteractable>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ParentInteractable>k__BackingField = value;
			}
		}

		public InteractableCollisionDepth CollisionDepth
		{
			get
			{
				if (this.ParentInteractable.ProximityCollider == this)
				{
					return InteractableCollisionDepth.Proximity;
				}
				if (this.ParentInteractable.ContactCollider == this)
				{
					return InteractableCollisionDepth.Contact;
				}
				if (this.ParentInteractable.ActionCollider != this)
				{
					return InteractableCollisionDepth.None;
				}
				return InteractableCollisionDepth.Action;
			}
		}

		private void Awake()
		{
			this.Collider = base.GetComponent<Collider>();
			this.ParentInteractable = this._parentInteractableObj.GetComponent<Interactable>();
		}

		public ButtonTriggerZone()
		{
		}

		[SerializeField]
		private GameObject _parentInteractableObj;

		[CompilerGenerated]
		private Collider <Collider>k__BackingField;

		[CompilerGenerated]
		private Interactable <ParentInteractable>k__BackingField;
	}
}
