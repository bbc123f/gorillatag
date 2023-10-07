using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000309 RID: 777
	[Serializable]
	public struct GuidedRefBasicReceiverFieldInfo
	{
		// Token: 0x040017AF RID: 6063
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040017B0 RID: 6064
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefRelayHubIdSO hubId;

		// Token: 0x040017B1 RID: 6065
		[NonSerialized]
		public int fieldId;
	}
}
