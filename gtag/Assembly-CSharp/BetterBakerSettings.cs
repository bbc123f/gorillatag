using System;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class BetterBakerSettings : MonoBehaviour
{
	// Token: 0x0400105E RID: 4190
	[SerializeField]
	public GameObject[] lightMapMaps = new GameObject[9];

	// Token: 0x0400105F RID: 4191
	[SerializeField]
	public string testString;

	// Token: 0x02000475 RID: 1141
	[Serializable]
	public struct LightMapMap
	{
		// Token: 0x04001E8F RID: 7823
		[SerializeField]
		public string timeOfDayName;

		// Token: 0x04001E90 RID: 7824
		[SerializeField]
		public GameObject sceneLightObject;
	}
}
