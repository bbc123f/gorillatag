using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering.MeshCombiner
{
	// Token: 0x02000331 RID: 817
	public class EdMeshCombinerSceneProcessor : MonoBehaviour
	{
		// Token: 0x060016B3 RID: 5811 RVA: 0x0007E6CB File Offset: 0x0007C8CB
		protected void Awake()
		{
			if (Application.isPlaying)
			{
				Object.Destroy(this);
			}
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x0007E6DA File Offset: 0x0007C8DA
		protected void OnEnable()
		{
		}

		// Token: 0x040018DA RID: 6362
		public const bool kDebugAllowMeshCombining = true;

		// Token: 0x040018DB RID: 6363
		[NonSerialized]
		private static string _dummyProp = "";

		// Token: 0x040018DC RID: 6364
		private static Dictionary<Hash128, Mesh> meshCache;

		// Token: 0x040018DD RID: 6365
		private static int aliveInstanceCount;
	}
}
