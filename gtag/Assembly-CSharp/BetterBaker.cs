using System;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class BetterBaker : MonoBehaviour
{
	// Token: 0x0400105F RID: 4191
	public string bakeryLightmapDirectory;

	// Token: 0x04001060 RID: 4192
	public string dayNightLightmapsDirectory;

	// Token: 0x04001061 RID: 4193
	public GameObject[] allLights;

	// Token: 0x02000475 RID: 1141
	public struct LightMapMap
	{
		// Token: 0x04001E97 RID: 7831
		public string timeOfDayName;

		// Token: 0x04001E98 RID: 7832
		public GameObject lightObject;
	}
}
