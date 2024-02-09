using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	public class GorillaHandClimber : MonoBehaviour
	{
		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
		}

		private void OnEnable()
		{
		}

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
			GorillaClimbable gorillaClimbable = null;
			foreach (GorillaClimbable gorillaClimbable2 in this.potentialClimbables)
			{
				float num2;
				if (gorillaClimbable2.colliderCache)
				{
					if (!this.col.bounds.Intersects(gorillaClimbable2.colliderCache.bounds))
					{
						continue;
					}
					Vector3 vector = gorillaClimbable2.colliderCache.ClosestPoint(position);
					num2 = Vector3.Distance(position, vector);
				}
				else
				{
					num2 = Vector3.Distance(position, gorillaClimbable2.transform.position);
				}
				if (num2 < num)
				{
					gorillaClimbable = gorillaClimbable2;
					num = num2;
				}
			}
			return gorillaClimbable;
		}

		private void OnTriggerEnter(Collider other)
		{
			GorillaClimbable gorillaClimbable;
			if (other.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
			{
				this.potentialClimbables.Add(gorillaClimbable);
				return;
			}
			GorillaClimbableRef gorillaClimbableRef;
			if (other.TryGetComponent<GorillaClimbableRef>(out gorillaClimbableRef))
			{
				this.potentialClimbables.Add(gorillaClimbableRef);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			GorillaClimbable gorillaClimbable;
			if (other.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
			{
				this.potentialClimbables.Remove(gorillaClimbable);
				return;
			}
			GorillaClimbableRef gorillaClimbableRef;
			if (other.TryGetComponent<GorillaClimbableRef>(out gorillaClimbableRef))
			{
				this.potentialClimbables.Remove(gorillaClimbableRef);
			}
		}

		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		[SerializeField]
		private Player player;

		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		public XRNode xrNode = XRNode.LeftHand;

		[NonSerialized]
		public bool isClimbing;

		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		public Transform handRoot;

		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		private Collider col;
	}
}
