using UnityEngine;

public class GorillaTriggerColliderHandIndicator : MonoBehaviour
{
	public Vector3 currentVelocity;

	public Vector3 lastPosition = Vector3.zero;

	public bool isLeftHand;

	public GorillaThrowableController throwableController;

	private void LateUpdate()
	{
		currentVelocity = (lastPosition - base.transform.position) / Time.fixedDeltaTime;
		lastPosition = base.transform.position;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (throwableController != null)
		{
			throwableController.GrabbableObjectHover(isLeftHand);
		}
	}
}
