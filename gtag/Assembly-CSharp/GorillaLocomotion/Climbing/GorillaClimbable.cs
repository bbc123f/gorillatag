using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020002A5 RID: 677
	public class GorillaClimbable : MonoBehaviour
	{
		// Token: 0x0600119E RID: 4510 RVA: 0x000645F1 File Offset: 0x000627F1
		private void Awake()
		{
			this.colliderCache = base.GetComponent<Collider>();
		}

		// Token: 0x04001453 RID: 5203
		public bool snapX;

		// Token: 0x04001454 RID: 5204
		public bool snapY;

		// Token: 0x04001455 RID: 5205
		public bool snapZ;

		// Token: 0x04001456 RID: 5206
		public float maxDistanceSnap = 0.05f;

		// Token: 0x04001457 RID: 5207
		public AudioClip clip;

		// Token: 0x04001458 RID: 5208
		public AudioClip clipOnFullRelease;

		// Token: 0x04001459 RID: 5209
		public Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb;

		// Token: 0x0400145A RID: 5210
		[NonSerialized]
		public bool isBeingClimbed;

		// Token: 0x0400145B RID: 5211
		[NonSerialized]
		public Collider colliderCache;
	}
}
