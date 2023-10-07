using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200029C RID: 668
	[CreateAssetMenu(fileName = "GorillaZiplineSettings", menuName = "ScriptableObjects/GorillaZiplineSettings", order = 0)]
	public class GorillaZiplineSettings : ScriptableObject
	{
		// Token: 0x040013F8 RID: 5112
		public float minSlidePitch = 0.5f;

		// Token: 0x040013F9 RID: 5113
		public float maxSlidePitch = 1f;

		// Token: 0x040013FA RID: 5114
		public float minSlideVolume;

		// Token: 0x040013FB RID: 5115
		public float maxSlideVolume = 0.2f;

		// Token: 0x040013FC RID: 5116
		public float maxSpeed = 10f;

		// Token: 0x040013FD RID: 5117
		public float gravityMulti = 1.1f;

		// Token: 0x040013FE RID: 5118
		[Header("Friction")]
		public float friction = 0.25f;

		// Token: 0x040013FF RID: 5119
		public float maxFriction = 1f;

		// Token: 0x04001400 RID: 5120
		public float maxFrictionSpeed = 15f;
	}
}
