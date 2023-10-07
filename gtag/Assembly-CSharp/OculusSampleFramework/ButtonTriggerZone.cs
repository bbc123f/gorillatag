using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CD RID: 717
	public class ButtonTriggerZone : MonoBehaviour, ColliderZone
	{
		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06001359 RID: 4953 RVA: 0x0006FBA6 File Offset: 0x0006DDA6
		// (set) Token: 0x0600135A RID: 4954 RVA: 0x0006FBAE File Offset: 0x0006DDAE
		public Collider Collider { get; private set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x0600135B RID: 4955 RVA: 0x0006FBB7 File Offset: 0x0006DDB7
		// (set) Token: 0x0600135C RID: 4956 RVA: 0x0006FBBF File Offset: 0x0006DDBF
		public Interactable ParentInteractable { get; private set; }

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x0600135D RID: 4957 RVA: 0x0006FBC8 File Offset: 0x0006DDC8
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

		// Token: 0x0600135E RID: 4958 RVA: 0x0006FC08 File Offset: 0x0006DE08
		private void Awake()
		{
			this.Collider = base.GetComponent<Collider>();
			this.ParentInteractable = this._parentInteractableObj.GetComponent<Interactable>();
		}

		// Token: 0x04001631 RID: 5681
		[SerializeField]
		private GameObject _parentInteractableObj;
	}
}
