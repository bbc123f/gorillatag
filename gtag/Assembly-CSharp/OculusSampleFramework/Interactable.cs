using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x020002D4 RID: 724
	public abstract class Interactable : MonoBehaviour
	{
		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06001389 RID: 5001 RVA: 0x0007061E File Offset: 0x0006E81E
		public ColliderZone ProximityCollider
		{
			get
			{
				return this._proximityZoneCollider;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600138A RID: 5002 RVA: 0x00070626 File Offset: 0x0006E826
		public ColliderZone ContactCollider
		{
			get
			{
				return this._contactZoneCollider;
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600138B RID: 5003 RVA: 0x0007062E File Offset: 0x0006E82E
		public ColliderZone ActionCollider
		{
			get
			{
				return this._actionZoneCollider;
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x0600138C RID: 5004 RVA: 0x00070636 File Offset: 0x0006E836
		public virtual int ValidToolTagsMask
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x0600138D RID: 5005 RVA: 0x0007063C File Offset: 0x0006E83C
		// (remove) Token: 0x0600138E RID: 5006 RVA: 0x00070674 File Offset: 0x0006E874
		public event Action<ColliderZoneArgs> ProximityZoneEvent;

		// Token: 0x0600138F RID: 5007 RVA: 0x000706A9 File Offset: 0x0006E8A9
		protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
		{
			if (this.ProximityZoneEvent != null)
			{
				this.ProximityZoneEvent(args);
			}
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06001390 RID: 5008 RVA: 0x000706C0 File Offset: 0x0006E8C0
		// (remove) Token: 0x06001391 RID: 5009 RVA: 0x000706F8 File Offset: 0x0006E8F8
		public event Action<ColliderZoneArgs> ContactZoneEvent;

		// Token: 0x06001392 RID: 5010 RVA: 0x0007072D File Offset: 0x0006E92D
		protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
		{
			if (this.ContactZoneEvent != null)
			{
				this.ContactZoneEvent(args);
			}
		}

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x06001393 RID: 5011 RVA: 0x00070744 File Offset: 0x0006E944
		// (remove) Token: 0x06001394 RID: 5012 RVA: 0x0007077C File Offset: 0x0006E97C
		public event Action<ColliderZoneArgs> ActionZoneEvent;

		// Token: 0x06001395 RID: 5013 RVA: 0x000707B1 File Offset: 0x0006E9B1
		protected virtual void OnActionZoneEvent(ColliderZoneArgs args)
		{
			if (this.ActionZoneEvent != null)
			{
				this.ActionZoneEvent(args);
			}
		}

		// Token: 0x06001396 RID: 5014
		public abstract void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth);

		// Token: 0x06001397 RID: 5015 RVA: 0x000707C7 File Offset: 0x0006E9C7
		protected virtual void Awake()
		{
			InteractableRegistry.RegisterInteractable(this);
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x000707CF File Offset: 0x0006E9CF
		protected virtual void OnDestroy()
		{
			InteractableRegistry.UnregisterInteractable(this);
		}

		// Token: 0x04001659 RID: 5721
		protected ColliderZone _proximityZoneCollider;

		// Token: 0x0400165A RID: 5722
		protected ColliderZone _contactZoneCollider;

		// Token: 0x0400165B RID: 5723
		protected ColliderZone _actionZoneCollider;

		// Token: 0x0400165F RID: 5727
		public Interactable.InteractableStateArgsEvent InteractableStateChanged;

		// Token: 0x020004EA RID: 1258
		[Serializable]
		public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
		{
		}
	}
}
