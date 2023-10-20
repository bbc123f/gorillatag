using System;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class BetterBakerSettings : MonoBehaviour
{
	// Token: 0x04001063 RID: 4195
	[SerializeField]
	public GameObject[] lightMapMaps = new GameObject[9];

	// Token: 0x04001064 RID: 4196
	[SerializeField]
	public string testString;

	// Token: 0x02000477 RID: 1143
	[Serializable]
	public struct LightMapMap
	{
		// Token: 0x04001E9C RID: 7836
		[SerializeField]
		public string timeOfDayName;

		// Token: 0x04001E9D RID: 7837
		[SerializeField]
		public GameObject sceneLightObject;
	}
}
