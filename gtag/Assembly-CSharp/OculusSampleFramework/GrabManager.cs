using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CB RID: 715
	public class GrabManager : MonoBehaviour
	{
		// Token: 0x06001346 RID: 4934 RVA: 0x0006F970 File Offset: 0x0006DB70
		private void OnTriggerEnter(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = true;
			}
		}

		// Token: 0x06001347 RID: 4935 RVA: 0x0006F994 File Offset: 0x0006DB94
		private void OnTriggerExit(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = false;
			}
		}

		// Token: 0x04001629 RID: 5673
		private Collider m_grabVolume;

		// Token: 0x0400162A RID: 5674
		public Color OutlineColorInRange;

		// Token: 0x0400162B RID: 5675
		public Color OutlineColorHighlighted;

		// Token: 0x0400162C RID: 5676
		public Color OutlineColorOutOfRange;
	}
}
