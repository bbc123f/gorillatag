using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200029E RID: 670
	[CreateAssetMenu(fileName = "GorillaZiplineSettings", menuName = "ScriptableObjects/GorillaZiplineSettings", order = 0)]
	public class GorillaZiplineSettings : ScriptableObject
	{
		// Token: 0x04001405 RID: 5125
		public float minSlidePitch = 0.5f;

		// Token: 0x04001406 RID: 5126
		public float maxSlidePitch = 1f;

		// Token: 0x04001407 RID: 5127
		public float minSlideVolume;

		// Token: 0x04001408 RID: 5128
		public float maxSlideVolume = 0.2f;

		// Token: 0x04001409 RID: 5129
		public float maxSpeed = 10f;

		// Token: 0x0400140A RID: 5130
		public float gravityMulti = 1.1f;

		// Token: 0x0400140B RID: 5131
		[Header("Friction")]
		public float friction = 0.25f;

		// Token: 0x0400140C RID: 5132
		public float maxFriction = 1f;

		// Token: 0x0400140D RID: 5133
		public float maxFrictionSpeed = 15f;
	}
}
