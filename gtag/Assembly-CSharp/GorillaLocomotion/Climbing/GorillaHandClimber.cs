using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020002A7 RID: 679
	public class GorillaHandClimber : MonoBehaviour
	{
		// Token: 0x060011A1 RID: 4513 RVA: 0x0006461A File Offset: 0x0006281A
		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x00064628 File Offset: 0x00062828
		private void OnEnable()
		{
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x0006462C File Offset: 0x0006282C
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

		// Token: 0x060011A4 RID: 4516 RVA: 0x00064770 File Offset: 0x00062970
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

		// Token: 0x060011A5 RID: 4517 RVA: 0x00064870 File Offset: 0x00062A70
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

		// Token: 0x060011A6 RID: 4518 RVA: 0x000648AC File Offset: 0x00062AAC
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

		// Token: 0x060011A7 RID: 4519 RVA: 0x000648E8 File Offset: 0x00062AE8
		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		// Token: 0x0400145D RID: 5213
		[SerializeField]
		private Player player;

		// Token: 0x0400145E RID: 5214
		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		// Token: 0x0400145F RID: 5215
		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		// Token: 0x04001460 RID: 5216
		public XRNode xrNode = XRNode.LeftHand;

		// Token: 0x04001461 RID: 5217
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x04001462 RID: 5218
		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		// Token: 0x04001463 RID: 5219
		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		// Token: 0x04001464 RID: 5220
		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		// Token: 0x04001465 RID: 5221
		public Transform handRoot;

		// Token: 0x04001466 RID: 5222
		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		// Token: 0x04001467 RID: 5223
		private Collider col;
	}
}
