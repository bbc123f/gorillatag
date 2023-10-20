using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200030A RID: 778
	[Serializable]
	public struct GuidedRefBasicTargetInfo
	{
		// Token: 0x040017BA RID: 6074
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040017BB RID: 6075
		[Tooltip("Used to filter down which relay the target can belong to. If null or empty then all parents with a GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefRelayHubIdSO[] hubIds;
	}
}
