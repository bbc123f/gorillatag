using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000308 RID: 776
	[Serializable]
	public struct GuidedRefBasicTargetInfo
	{
		// Token: 0x040017AD RID: 6061
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040017AE RID: 6062
		[Tooltip("Used to filter down which relay the target can belong to. If null or empty then all parents with a GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefRelayHubIdSO[] hubIds;
	}
}
