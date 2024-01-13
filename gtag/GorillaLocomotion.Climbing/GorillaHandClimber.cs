using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing;

public class GorillaHandClimber : MonoBehaviour
{
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

	private void Awake()
	{
		col = GetComponent<Collider>();
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
		for (int num = potentialClimbables.Count - 1; num >= 0; num--)
		{
			if (potentialClimbables[num] == null || !potentialClimbables[num].isActiveAndEnabled)
			{
				potentialClimbables.RemoveAt(num);
			}
		}
		bool grab = ControllerInputPoller.GetGrab(xrNode);
		bool grabRelease = ControllerInputPoller.GetGrabRelease(xrNode);
		if (!isClimbing)
		{
			if (queuedToBecomeValidToGrabAgain && Vector3.Distance(lastAutoReleasePos, handRoot.localPosition) >= 0.35f)
			{
				queuedToBecomeValidToGrabAgain = false;
			}
			if (grabRelease)
			{
				queuedToBecomeValidToGrabAgain = false;
				dontReclimbLast = null;
			}
			GorillaClimbable closestClimbable = GetClosestClimbable();
			if (!queuedToBecomeValidToGrabAgain && (bool)closestClimbable && grab && !equipmentInteractor.GetIsHolding(xrNode) && closestClimbable != dontReclimbLast && !player.inOverlay)
			{
				if (closestClimbable is GorillaClimbableRef gorillaClimbableRef)
				{
					player.BeginClimbing(gorillaClimbableRef.climb, this, gorillaClimbableRef);
				}
				else
				{
					player.BeginClimbing(closestClimbable, this);
				}
			}
		}
		else if (grabRelease)
		{
			player.EndClimbing(this, startingNewClimb: false);
		}
	}

	public GorillaClimbable GetClosestClimbable()
	{
		if (potentialClimbables.Count == 0)
		{
			return null;
		}
		if (potentialClimbables.Count == 1)
		{
			return potentialClimbables[0];
		}
		Vector3 position = base.transform.position;
		float num = float.MaxValue;
		GorillaClimbable result = null;
		foreach (GorillaClimbable potentialClimbable in potentialClimbables)
		{
			float num2 = 0f;
			if ((bool)potentialClimbable.colliderCache)
			{
				if (!col.bounds.Intersects(potentialClimbable.colliderCache.bounds))
				{
					continue;
				}
				Vector3 b = potentialClimbable.colliderCache.ClosestPoint(position);
				num2 = Vector3.Distance(position, b);
			}
			else
			{
				num2 = Vector3.Distance(position, potentialClimbable.transform.position);
			}
			if (num2 < num)
			{
				result = potentialClimbable;
				num = num2;
			}
		}
		return result;
	}

	private void OnTriggerEnter(Collider other)
	{
		GorillaClimbableRef component2;
		if (other.TryGetComponent<GorillaClimbable>(out var component))
		{
			potentialClimbables.Add(component);
		}
		else if (other.TryGetComponent<GorillaClimbableRef>(out component2))
		{
			potentialClimbables.Add(component2);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		GorillaClimbableRef component2;
		if (other.TryGetComponent<GorillaClimbable>(out var component))
		{
			potentialClimbables.Remove(component);
		}
		else if (other.TryGetComponent<GorillaClimbableRef>(out component2))
		{
			potentialClimbables.Remove(component2);
		}
	}

	public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
	{
		player.EndClimbing(this, startingNewClimb, doDontReclimb);
	}
}
