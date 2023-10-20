using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CD RID: 717
	public class BoneCapsuleTriggerLogic : MonoBehaviour
	{
		// Token: 0x0600134D RID: 4941 RVA: 0x0006FA03 File Offset: 0x0006DC03
		private void OnDisable()
		{
			this.CollidersTouchingUs.Clear();
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x0006FA10 File Offset: 0x0006DC10
		private void Update()
		{
			this.CleanUpDeadColliders();
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x0006FA18 File Offset: 0x0006DC18
		private void OnTriggerEnter(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Add(component);
			}
		}

		// Token: 0x06001350 RID: 4944 RVA: 0x0006FA58 File Offset: 0x0006DC58
		private void OnTriggerExit(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Remove(component);
			}
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x0006FA98 File Offset: 0x0006DC98
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
			foreach (ColliderZone item in this._elementsToCleanUp)
			{
				this.CollidersTouchingUs.Remove(item);
			}
		}

		// Token: 0x0400162D RID: 5677
		public InteractableToolTags ToolTags;

		// Token: 0x0400162E RID: 5678
		public HashSet<ColliderZone> CollidersTouchingUs = new HashSet<ColliderZone>();

		// Token: 0x0400162F RID: 5679
		private List<ColliderZone> _elementsToCleanUp = new List<ColliderZone>();
	}
}
