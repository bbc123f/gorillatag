using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000373 RID: 883
	[CreateAssetMenu(fileName = "BoingParams", menuName = "Boing Kit/Shared Boing Params", order = 550)]
	public class SharedBoingParams : ScriptableObject
	{
		// Token: 0x06001A18 RID: 6680 RVA: 0x00091AB0 File Offset: 0x0008FCB0
		public SharedBoingParams()
		{
			this.Params.Init();
		}

		// Token: 0x04001A9B RID: 6811
		public BoingWork.Params Params;
	}
}
