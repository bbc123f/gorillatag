using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020002A3 RID: 675
	public class GorillaClimbable : MonoBehaviour
	{
		// Token: 0x06001197 RID: 4503 RVA: 0x00064189 File Offset: 0x00062389
		private void Awake()
		{
			this.colliderCache = base.GetComponent<Collider>();
		}

		// Token: 0x04001446 RID: 5190
		public bool snapX;

		// Token: 0x04001447 RID: 5191
		public bool snapY;

		// Token: 0x04001448 RID: 5192
		public bool snapZ;

		// Token: 0x04001449 RID: 5193
		public float maxDistanceSnap = 0.05f;

		// Token: 0x0400144A RID: 5194
		public AudioClip clip;

		// Token: 0x0400144B RID: 5195
		public AudioClip clipOnFullRelease;

		// Token: 0x0400144C RID: 5196
		public Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb;

		// Token: 0x0400144D RID: 5197
		[NonSerialized]
		public bool isBeingClimbed;

		// Token: 0x0400144E RID: 5198
		[NonSerialized]
		public Collider colliderCache;
	}
}
