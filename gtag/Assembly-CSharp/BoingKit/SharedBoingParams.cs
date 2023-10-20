using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000375 RID: 885
	[CreateAssetMenu(fileName = "BoingParams", menuName = "Boing Kit/Shared Boing Params", order = 550)]
	public class SharedBoingParams : ScriptableObject
	{
		// Token: 0x06001A21 RID: 6689 RVA: 0x00091F98 File Offset: 0x00090198
		public SharedBoingParams()
		{
			this.Params.Init();
		}

		// Token: 0x04001AA8 RID: 6824
		public BoingWork.Params Params;
	}
}
