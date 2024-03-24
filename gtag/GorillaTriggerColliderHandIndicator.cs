using System;
using UnityEngine;

public class GorillaTriggerColliderHandIndicator : MonoBehaviour
{
	private void LateUpdate()
	{
		this.currentVelocity = (this.lastPosition - base.transform.position) / Time.fixedDeltaTime;
		this.lastPosition = base.transform.position;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	public GorillaTriggerColliderHandIndicator()
	{
	}

	public Vector3 currentVelocity;

	public Vector3 lastPosition = Vector3.zero;

	public bool isLeftHand;

	public GorillaThrowableController throwableController;
}
