using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002DE RID: 734
	public abstract class InteractableTool : MonoBehaviour
	{
		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060013BF RID: 5055 RVA: 0x00070BA6 File Offset: 0x0006EDA6
		public Transform ToolTransform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x00070BAE File Offset: 0x0006EDAE
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x00070BB6 File Offset: 0x0006EDB6
		public bool IsRightHandedTool { get; set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060013C2 RID: 5058
		public abstract InteractableToolTags ToolTags { get; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060013C3 RID: 5059
		public abstract ToolInputState ToolInputState { get; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060013C4 RID: 5060
		public abstract bool IsFarFieldTool { get; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060013C5 RID: 5061 RVA: 0x00070BBF File Offset: 0x0006EDBF
		// (set) Token: 0x060013C6 RID: 5062 RVA: 0x00070BC7 File Offset: 0x0006EDC7
		public Vector3 Velocity { get; protected set; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060013C7 RID: 5063 RVA: 0x00070BD0 File Offset: 0x0006EDD0
		// (set) Token: 0x060013C8 RID: 5064 RVA: 0x00070BD8 File Offset: 0x0006EDD8
		public Vector3 InteractionPosition { get; protected set; }

		// Token: 0x060013C9 RID: 5065 RVA: 0x00070BE1 File Offset: 0x0006EDE1
		public List<InteractableCollisionInfo> GetCurrentIntersectingObjects()
		{
			return this._currentIntersectingObjects;
		}

		// Token: 0x060013CA RID: 5066
		public abstract List<InteractableCollisionInfo> GetNextIntersectingObjects();

		// Token: 0x060013CB RID: 5067
		public abstract void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone);

		// Token: 0x060013CC RID: 5068
		public abstract void DeFocus();

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060013CD RID: 5069
		// (set) Token: 0x060013CE RID: 5070
		public abstract bool EnableState { get; set; }

		// Token: 0x060013CF RID: 5071
		public abstract void Initialize();

		// Token: 0x060013D0 RID: 5072 RVA: 0x00070BE9 File Offset: 0x0006EDE9
		public KeyValuePair<Interactable, InteractableCollisionInfo> GetFirstCurrentCollisionInfo()
		{
			return this._currInteractableToCollisionInfos.First<KeyValuePair<Interactable, InteractableCollisionInfo>>();
		}

		// Token: 0x060013D1 RID: 5073 RVA: 0x00070BF6 File Offset: 0x0006EDF6
		public void ClearAllCurrentCollisionInfos()
		{
			this._currInteractableToCollisionInfos.Clear();
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x00070C04 File Offset: 0x0006EE04
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

		// Token: 0x060013D3 RID: 5075 RVA: 0x00070CB0 File Offset: 0x0006EEB0
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

		// Token: 0x0400168B RID: 5771
		protected List<InteractableCollisionInfo> _currentIntersectingObjects = new List<InteractableCollisionInfo>();

		// Token: 0x0400168C RID: 5772
		private List<Interactable> _addedInteractables = new List<Interactable>();

		// Token: 0x0400168D RID: 5773
		private List<Interactable> _removedInteractables = new List<Interactable>();

		// Token: 0x0400168E RID: 5774
		private List<Interactable> _remainingInteractables = new List<Interactable>();

		// Token: 0x0400168F RID: 5775
		private Dictionary<Interactable, InteractableCollisionInfo> _currInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();

		// Token: 0x04001690 RID: 5776
		private Dictionary<Interactable, InteractableCollisionInfo> _prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();
	}
}
