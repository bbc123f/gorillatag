using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	public class BoneCapsuleTriggerLogic : MonoBehaviour
	{
		private void OnDisable()
		{
			this.CollidersTouchingUs.Clear();
		}

		private void Update()
		{
			this.CleanUpDeadColliders();
		}

		private void OnTriggerEnter(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Add(component);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Remove(component);
			}
		}

		private void CleanUpDeadColliders()
		{
			this._elementsToCleanUp.Clear();
			foreach (ColliderZone colliderZone in this.CollidersTouchingUs)
			{
				if (!colliderZone.Collider.gameObject.activeInHierarchy)
				{
					this._elementsToCleanUp.Add(colliderZone);
				}
			}
			foreach (ColliderZone colliderZone2 in this._elementsToCleanUp)
			{
				this.CollidersTouchingUs.Remove(colliderZone2);
			}
		}

		public InteractableToolTags ToolTags;

		public HashSet<ColliderZone> CollidersTouchingUs = new HashSet<ColliderZone>();

		private List<ColliderZone> _elementsToCleanUp = new List<ColliderZone>();
	}
}
