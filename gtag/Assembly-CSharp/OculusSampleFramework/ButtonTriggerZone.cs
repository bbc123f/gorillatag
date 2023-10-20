using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CF RID: 719
	public class ButtonTriggerZone : MonoBehaviour, ColliderZone
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06001360 RID: 4960 RVA: 0x00070072 File Offset: 0x0006E272
		// (set) Token: 0x06001361 RID: 4961 RVA: 0x0007007A File Offset: 0x0006E27A
		public Collider Collider { get; private set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06001362 RID: 4962 RVA: 0x00070083 File Offset: 0x0006E283
		// (set) Token: 0x06001363 RID: 4963 RVA: 0x0007008B File Offset: 0x0006E28B
		public Interactable ParentInteractable { get; private set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x00070094 File Offset: 0x0006E294
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

		// Token: 0x06001365 RID: 4965 RVA: 0x000700D4 File Offset: 0x0006E2D4
		private void Awake()
		{
			this.Collider = base.GetComponent<Collider>();
			this.ParentInteractable = this._parentInteractableObj.GetComponent<Interactable>();
		}

		// Token: 0x0400163E RID: 5694
		[SerializeField]
		private GameObject _parentInteractableObj;
	}
}
