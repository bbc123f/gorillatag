using System;
using System.Collections.Generic;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class HandHold : MonoBehaviour, IGorillaGrabable
{
	Transform IGorillaGrabable.OnGrabbed(GorillaGrabber g)
	{
		Transform transform = new GameObject().transform;
		transform.position = g.transform.position;
		transform.parent = base.transform;
		this.attached[g.transform] = transform;
		Vector3 vector;
		g.Player.AddHandHold(transform, g.transform, g.XrNode == XRNode.RightHand, this.rotatePlayerWhenHeld, out vector);
		this.OnGrab.Invoke(vector);
		return transform;
	}

	Transform IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		g.Player.RemoveHandHold(this.attached[g.transform], g.XrNode == XRNode.RightHand);
		Object.Destroy(this.attached[g.transform].gameObject);
		this.attached.Remove(g.transform);
		this.OnRelease.Invoke();
		return null;
	}

	public HandHold()
	{
	}

	private Dictionary<Transform, Transform> attached = new Dictionary<Transform, Transform>();

	[SerializeField]
	private UnityEvent<Vector3> OnGrab;

	[SerializeField]
	private UnityEvent OnRelease;

	[SerializeField]
	private bool rotatePlayerWhenHeld;
}
