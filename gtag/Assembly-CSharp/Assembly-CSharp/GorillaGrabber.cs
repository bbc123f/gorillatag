using System;
using System.Collections.Generic;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.XR;

public class GorillaGrabber : MonoBehaviour
{
	private void Awake()
	{
		this.col = base.GetComponent<Collider>();
	}

	private void Update()
	{
		bool grab = ControllerInputPoller.GetGrab(this.xrNode);
		bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
		if (grab && this.currentGrabbed == null)
		{
			this.currentGrabbed = this.GetGrabable();
			if (this.currentGrabbed != null)
			{
				this.currentGrabbed.OnGrabbed();
			}
		}
		if (grabRelease && this.currentGrabbed != null)
		{
			this.currentGrabbed.OnGrabReleased();
			this.currentGrabbed = null;
		}
	}

	private IGorillaGrabable GetGrabable()
	{
		if (this.overlapGrabables.Count > 0)
		{
			return this.overlapGrabables[0];
		}
		return null;
	}

	private void OnTriggerEnter(Collider other)
	{
		IGorillaGrabable item;
		if (other.TryGetComponent<IGorillaGrabable>(out item))
		{
			this.overlapGrabables.Add(item);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		IGorillaGrabable item;
		if (other.TryGetComponent<IGorillaGrabable>(out item))
		{
			this.overlapGrabables.Remove(item);
		}
	}

	private List<IGorillaGrabable> overlapGrabables = new List<IGorillaGrabable>();

	[SerializeField]
	private XRNode xrNode = XRNode.LeftHand;

	private Collider col;

	private IGorillaGrabable currentGrabbed;
}
