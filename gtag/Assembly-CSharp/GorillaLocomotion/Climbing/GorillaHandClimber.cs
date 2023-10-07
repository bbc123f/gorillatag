using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020002A5 RID: 677
	public class GorillaHandClimber : MonoBehaviour
	{
		// Token: 0x0600119A RID: 4506 RVA: 0x000641B2 File Offset: 0x000623B2
		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x000641C0 File Offset: 0x000623C0
		private void OnEnable()
		{
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x000641C4 File Offset: 0x000623C4
		private void Update()
		{
			for (int i = this.potentialClimbables.Count - 1; i >= 0; i--)
			{
				if (this.potentialClimbables[i] == null || !this.potentialClimbables[i].isActiveAndEnabled)
				{
					this.potentialClimbables.RemoveAt(i);
				}
			}
			bool grab = ControllerInputPoller.GetGrab(this.xrNode);
			bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
			if (!this.isClimbing)
			{
				if (this.queuedToBecomeValidToGrabAgain && Vector3.Distance(this.lastAutoReleasePos, this.handRoot.localPosition) >= 0.35f)
				{
					this.queuedToBecomeValidToGrabAgain = false;
				}
				if (grabRelease)
				{
					this.queuedToBecomeValidToGrabAgain = false;
					this.dontReclimbLast = null;
				}
				GorillaClimbable closestClimbable = this.GetClosestClimbable();
				if (!this.queuedToBecomeValidToGrabAgain && closestClimbable && grab && !this.equipmentInteractor.GetIsHolding(this.xrNode) && closestClimbable != this.dontReclimbLast && !this.player.inOverlay)
				{
					GorillaClimbableRef gorillaClimbableRef = closestClimbable as GorillaClimbableRef;
					if (gorillaClimbableRef != null)
					{
						this.player.BeginClimbing(gorillaClimbableRef.climb, this, gorillaClimbableRef);
						return;
					}
					this.player.BeginClimbing(closestClimbable, this, null);
					return;
				}
			}
			else if (grabRelease)
			{
				this.player.EndClimbing(this, false, false);
			}
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00064308 File Offset: 0x00062508
		public GorillaClimbable GetClosestClimbable()
		{
			if (this.potentialClimbables.Count == 0)
			{
				return null;
			}
			if (this.potentialClimbables.Count == 1)
			{
				return this.potentialClimbables[0];
			}
			Vector3 position = base.transform.position;
			float num = float.MaxValue;
			GorillaClimbable result = null;
			foreach (GorillaClimbable gorillaClimbable in this.potentialClimbables)
			{
				float num2;
				if (gorillaClimbable.colliderCache)
				{
					if (!this.col.bounds.Intersects(gorillaClimbable.colliderCache.bounds))
					{
						continue;
					}
					Vector3 b = gorillaClimbable.colliderCache.ClosestPoint(position);
					num2 = Vector3.Distance(position, b);
				}
				else
				{
					num2 = Vector3.Distance(position, gorillaClimbable.transform.position);
				}
				if (num2 < num)
				{
					result = gorillaClimbable;
					num = num2;
				}
			}
			return result;
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00064408 File Offset: 0x00062608
		private void OnTriggerEnter(Collider other)
		{
			GorillaClimbable item;
			if (other.TryGetComponent<GorillaClimbable>(out item))
			{
				this.potentialClimbables.Add(item);
				return;
			}
			GorillaClimbableRef item2;
			if (other.TryGetComponent<GorillaClimbableRef>(out item2))
			{
				this.potentialClimbables.Add(item2);
			}
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00064444 File Offset: 0x00062644
		private void OnTriggerExit(Collider other)
		{
			GorillaClimbable item;
			if (other.TryGetComponent<GorillaClimbable>(out item))
			{
				this.potentialClimbables.Remove(item);
				return;
			}
			GorillaClimbableRef item2;
			if (other.TryGetComponent<GorillaClimbableRef>(out item2))
			{
				this.potentialClimbables.Remove(item2);
			}
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00064480 File Offset: 0x00062680
		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		// Token: 0x04001450 RID: 5200
		[SerializeField]
		private Player player;

		// Token: 0x04001451 RID: 5201
		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		// Token: 0x04001452 RID: 5202
		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		// Token: 0x04001453 RID: 5203
		public XRNode xrNode = XRNode.LeftHand;

		// Token: 0x04001454 RID: 5204
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x04001455 RID: 5205
		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		// Token: 0x04001456 RID: 5206
		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		// Token: 0x04001457 RID: 5207
		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		// Token: 0x04001458 RID: 5208
		public Transform handRoot;

		// Token: 0x04001459 RID: 5209
		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		// Token: 0x0400145A RID: 5210
		private Collider col;
	}
}
