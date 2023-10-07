using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000299 RID: 665
	[CreateAssetMenu(fileName = "GorillaRopeSwingSettings", menuName = "ScriptableObjects/GorillaRopeSwingSettings", order = 0)]
	public class GorillaRopeSwingSettings : ScriptableObject
	{
		// Token: 0x040013E6 RID: 5094
		public float inheritVelocityMultiplier = 1f;

		// Token: 0x040013E7 RID: 5095
		public float frictionWhenNotHeld = 0.25f;
	}
}
