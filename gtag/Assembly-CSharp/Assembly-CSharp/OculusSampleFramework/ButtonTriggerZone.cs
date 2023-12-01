using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class ButtonTriggerZone : MonoBehaviour, ColliderZone
	{
		public Collider Collider { get; private set; }

		public Interactable ParentInteractable { get; private set; }

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

		[SerializeField]
		private GameObject _parentInteractableObj;
	}
}
