using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x020002D2 RID: 722
	public abstract class Interactable : MonoBehaviour
	{
		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06001382 RID: 4994 RVA: 0x00070152 File Offset: 0x0006E352
		public ColliderZone ProximityCollider
		{
			get
			{
				return this._proximityZoneCollider;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06001383 RID: 4995 RVA: 0x0007015A File Offset: 0x0006E35A
		public ColliderZone ContactCollider
		{
			get
			{
				return this._contactZoneCollider;
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06001384 RID: 4996 RVA: 0x00070162 File Offset: 0x0006E362
		public ColliderZone ActionCollider
		{
			get
			{
				return this._actionZoneCollider;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06001385 RID: 4997 RVA: 0x0007016A File Offset: 0x0006E36A
		public virtual int ValidToolTagsMask
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06001386 RID: 4998 RVA: 0x00070170 File Offset: 0x0006E370
		// (remove) Token: 0x06001387 RID: 4999 RVA: 0x000701A8 File Offset: 0x0006E3A8
		public event Action<ColliderZoneArgs> ProximityZoneEvent;

		// Token: 0x06001388 RID: 5000 RVA: 0x000701DD File Offset: 0x0006E3DD
		protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
		{
			if (this.ProximityZoneEvent != null)
			{
				this.ProximityZoneEvent(args);
			}
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06001389 RID: 5001 RVA: 0x000701F4 File Offset: 0x0006E3F4
		// (remove) Token: 0x0600138A RID: 5002 RVA: 0x0007022C File Offset: 0x0006E42C
		public event Action<ColliderZoneArgs> ContactZoneEvent;

		// Token: 0x0600138B RID: 5003 RVA: 0x00070261 File Offset: 0x0006E461
		protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
		{
			if (this.ContactZoneEvent != null)
			{
				this.ContactZoneEvent(args);
			}
		}

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x0600138C RID: 5004 RVA: 0x00070278 File Offset: 0x0006E478
		// (remove) Token: 0x0600138D RID: 5005 RVA: 0x000702B0 File Offset: 0x0006E4B0
		public event Action<ColliderZoneArgs> ActionZoneEvent;

		// Token: 0x0600138E RID: 5006 RVA: 0x000702E5 File Offset: 0x0006E4E5
		protected virtual void OnActionZoneEvent(ColliderZoneArgs args)
		{
			if (this.ActionZoneEvent != null)
			{
				this.ActionZoneEvent(args);
			}
		}

		// Token: 0x0600138F RID: 5007
		public abstract void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth);

		// Token: 0x06001390 RID: 5008 RVA: 0x000702FB File Offset: 0x0006E4FB
		protected virtual void Awake()
		{
			InteractableRegistry.RegisterInteractable(this);
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x00070303 File Offset: 0x0006E503
		protected virtual void OnDestroy()
		{
			InteractableRegistry.UnregisterInteractable(this);
		}

		// Token: 0x0400164C RID: 5708
		protected ColliderZone _proximityZoneCollider;

		// Token: 0x0400164D RID: 5709
		protected ColliderZone _contactZoneCollider;

		// Token: 0x0400164E RID: 5710
		protected ColliderZone _actionZoneCollider;

		// Token: 0x04001652 RID: 5714
		public Interactable.InteractableStateArgsEvent InteractableStateChanged;

		// Token: 0x020004E8 RID: 1256
		[Serializable]
		public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
		{
		}
	}
}
