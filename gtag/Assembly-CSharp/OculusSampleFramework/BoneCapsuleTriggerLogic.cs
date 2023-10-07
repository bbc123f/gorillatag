using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CB RID: 715
	public class BoneCapsuleTriggerLogic : MonoBehaviour
	{
		// Token: 0x06001346 RID: 4934 RVA: 0x0006F537 File Offset: 0x0006D737
		private void OnDisable()
		{
			this.CollidersTouchingUs.Clear();
		}

		// Token: 0x06001347 RID: 4935 RVA: 0x0006F544 File Offset: 0x0006D744
		private void Update()
		{
			this.CleanUpDeadColliders();
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x0006F54C File Offset: 0x0006D74C
		private void OnTriggerEnter(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Add(component);
			}
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x0006F58C File Offset: 0x0006D78C
		private void OnTriggerExit(Collider other)
		{
			ButtonTriggerZone component = other.GetComponent<ButtonTriggerZone>();
			if (component != null && (component.ParentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
			{
				this.CollidersTouchingUs.Remove(component);
			}
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x0006F5CC File Offset: 0x0006D7CC
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

		// Token: 0x04001620 RID: 5664
		public InteractableToolTags ToolTags;

		// Token: 0x04001621 RID: 5665
		public HashSet<ColliderZone> CollidersTouchingUs = new HashSet<ColliderZone>();

		// Token: 0x04001622 RID: 5666
		private List<ColliderZone> _elementsToCleanUp = new List<ColliderZone>();
	}
}
