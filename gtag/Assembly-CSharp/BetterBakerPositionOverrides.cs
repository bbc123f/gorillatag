using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class BetterBakerPositionOverrides : MonoBehaviour
{
	// Token: 0x0400105D RID: 4189
	public List<BetterBakerPositionOverrides.OverridePosition> overridePositions;

	// Token: 0x02000474 RID: 1140
	[Serializable]
	public struct OverridePosition
	{
		// Token: 0x04001E8C RID: 7820
		public GameObject go;

		// Token: 0x04001E8D RID: 7821
		public Transform bakingTransform;

		// Token: 0x04001E8E RID: 7822
		public Transform gameTransform;
	}
}
