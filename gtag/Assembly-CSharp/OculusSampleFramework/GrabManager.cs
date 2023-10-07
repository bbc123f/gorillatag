using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002C9 RID: 713
	public class GrabManager : MonoBehaviour
	{
		// Token: 0x0600133F RID: 4927 RVA: 0x0006F4A4 File Offset: 0x0006D6A4
		private void OnTriggerEnter(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = true;
			}
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x0006F4C8 File Offset: 0x0006D6C8
		private void OnTriggerExit(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = false;
			}
		}

		// Token: 0x0400161C RID: 5660
		private Collider m_grabVolume;

		// Token: 0x0400161D RID: 5661
		public Color OutlineColorInRange;

		// Token: 0x0400161E RID: 5662
		public Color OutlineColorHighlighted;

		// Token: 0x0400161F RID: 5663
		public Color OutlineColorOutOfRange;
	}
}
