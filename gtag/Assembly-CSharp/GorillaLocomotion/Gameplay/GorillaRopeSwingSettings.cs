using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200029B RID: 667
	[CreateAssetMenu(fileName = "GorillaRopeSwingSettings", menuName = "ScriptableObjects/GorillaRopeSwingSettings", order = 0)]
	public class GorillaRopeSwingSettings : ScriptableObject
	{
		// Token: 0x040013F3 RID: 5107
		public float inheritVelocityMultiplier = 1f;

		// Token: 0x040013F4 RID: 5108
		public float frictionWhenNotHeld = 0.25f;
	}
}
