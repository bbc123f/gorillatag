using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class GrabManager : MonoBehaviour
	{
		private void OnTriggerEnter(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = true;
			}
		}

		private void OnTriggerExit(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = false;
			}
		}

		private Collider m_grabVolume;

		public Color OutlineColorInRange;

		public Color OutlineColorHighlighted;

		public Color OutlineColorOutOfRange;
	}
}
