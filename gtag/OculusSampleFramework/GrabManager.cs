using UnityEngine;

namespace OculusSampleFramework;

public class GrabManager : MonoBehaviour
{
	private Collider m_grabVolume;

	public Color OutlineColorInRange;

	public Color OutlineColorHighlighted;

	public Color OutlineColorOutOfRange;

	private void OnTriggerEnter(Collider otherCollider)
	{
		DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
		if ((bool)componentInChildren)
		{
			componentInChildren.InRange = true;
		}
	}

	private void OnTriggerExit(Collider otherCollider)
	{
		DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
		if ((bool)componentInChildren)
		{
			componentInChildren.InRange = false;
		}
	}
}
