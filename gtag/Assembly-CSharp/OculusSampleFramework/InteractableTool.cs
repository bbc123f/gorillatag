using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E0 RID: 736
	public abstract class InteractableTool : MonoBehaviour
	{
		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060013C6 RID: 5062 RVA: 0x00071072 File Offset: 0x0006F272
		public Transform ToolTransform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060013C7 RID: 5063 RVA: 0x0007107A File Offset: 0x0006F27A
		// (set) Token: 0x060013C8 RID: 5064 RVA: 0x00071082 File Offset: 0x0006F282
		public bool IsRightHandedTool { get; set; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060013C9 RID: 5065
		public abstract InteractableToolTags ToolTags { get; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060013CA RID: 5066
		public abstract ToolInputState ToolInputState { get; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060013CB RID: 5067
		public abstract bool IsFarFieldTool { get; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060013CC RID: 5068 RVA: 0x0007108B File Offset: 0x0006F28B
		// (set) Token: 0x060013CD RID: 5069 RVA: 0x00071093 File Offset: 0x0006F293
		public Vector3 Velocity { get; protected set; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060013CE RID: 5070 RVA: 0x0007109C File Offset: 0x0006F29C
		// (set) Token: 0x060013CF RID: 5071 RVA: 0x000710A4 File Offset: 0x0006F2A4
		public Vector3 InteractionPosition { get; protected set; }

		// Token: 0x060013D0 RID: 5072 RVA: 0x000710AD File Offset: 0x0006F2AD
		public List<InteractableCollisionInfo> GetCurrentIntersectingObjects()
		{
			return this._currentIntersectingObjects;
		}

		// Token: 0x060013D1 RID: 5073
		public abstract List<InteractableCollisionInfo> GetNextIntersectingObjects();

		// Token: 0x060013D2 RID: 5074
		public abstract void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone);

		// Token: 0x060013D3 RID: 5075
		public abstract void DeFocus();

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060013D4 RID: 5076
		// (set) Token: 0x060013D5 RID: 5077
		public abstract bool EnableState { get; set; }

		// Token: 0x060013D6 RID: 5078
		public abstract void Initialize();

		// Token: 0x060013D7 RID: 5079 RVA: 0x000710B5 File Offset: 0x0006F2B5
		public KeyValuePair<Interactable, InteractableCollisionInfo> GetFirstCurrentCollisionInfo()
		{
			return this._currInteractableToCollisionInfos.First<KeyValuePair<Interactable, InteractableCollisionInfo>>();
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x000710C2 File Offset: 0x0006F2C2
		public void ClearAllCurrentCollisionInfos()
		{
			this._currInteractableToCollisionInfos.Clear();
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x000710D0 File Offset: 0x0006F2D0
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

		// Token: 0x060013DA RID: 5082 RVA: 0x0007117C File Offset: 0x0006F37C
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

		// Token: 0x04001698 RID: 5784
		protected List<InteractableCollisionInfo> _currentIntersectingObjects = new List<InteractableCollisionInfo>();

		// Token: 0x04001699 RID: 5785
		private List<Interactable> _addedInteractables = new List<Interactable>();

		// Token: 0x0400169A RID: 5786
		private List<Interactable> _removedInteractables = new List<Interactable>();

		// Token: 0x0400169B RID: 5787
		private List<Interactable> _remainingInteractables = new List<Interactable>();

		// Token: 0x0400169C RID: 5788
		private Dictionary<Interactable, InteractableCollisionInfo> _currInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();

		// Token: 0x0400169D RID: 5789
		private Dictionary<Interactable, InteractableCollisionInfo> _prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();
	}
}
