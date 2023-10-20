using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class BetterBakerPositionOverrides : MonoBehaviour
{
	// Token: 0x04001062 RID: 4194
	public List<BetterBakerPositionOverrides.OverridePosition> overridePositions;

	// Token: 0x02000476 RID: 1142
	[Serializable]
	public struct OverridePosition
	{
		// Token: 0x04001E99 RID: 7833
		public GameObject go;

		// Token: 0x04001E9A RID: 7834
		public Transform bakingTransform;

		// Token: 0x04001E9B RID: 7835
		public Transform gameTransform;
	}
}
