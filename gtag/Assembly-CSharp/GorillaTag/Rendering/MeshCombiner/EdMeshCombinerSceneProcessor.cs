using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering.MeshCombiner
{
	// Token: 0x0200032F RID: 815
	public class EdMeshCombinerSceneProcessor : MonoBehaviour
	{
		// Token: 0x060016AA RID: 5802 RVA: 0x0007E1E3 File Offset: 0x0007C3E3
		protected void Awake()
		{
			if (Application.isPlaying)
			{
				Object.Destroy(this);
			}
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x0007E1F2 File Offset: 0x0007C3F2
		protected void OnEnable()
		{
		}

		// Token: 0x040018CD RID: 6349
		public const bool kDebugAllowMeshCombining = true;

		// Token: 0x040018CE RID: 6350
		[NonSerialized]
		private static string _dummyProp = "";

		// Token: 0x040018CF RID: 6351
		private static Dictionary<Hash128, Mesh> meshCache;

		// Token: 0x040018D0 RID: 6352
		private static int aliveInstanceCount;
	}
}
