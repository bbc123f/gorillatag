using System;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class BetterBaker : MonoBehaviour
{
	// Token: 0x0400105A RID: 4186
	public string bakeryLightmapDirectory;

	// Token: 0x0400105B RID: 4187
	public string dayNightLightmapsDirectory;

	// Token: 0x0400105C RID: 4188
	public GameObject[] allLights;

	// Token: 0x02000473 RID: 1139
	public struct LightMapMap
	{
		// Token: 0x04001E8A RID: 7818
		public string timeOfDayName;

		// Token: 0x04001E8B RID: 7819
		public GameObject lightObject;
	}
}
