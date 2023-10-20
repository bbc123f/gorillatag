using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200030B RID: 779
	[Serializable]
	public struct GuidedRefBasicReceiverFieldInfo
	{
		// Token: 0x040017BC RID: 6076
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040017BD RID: 6077
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefRelayHubIdSO hubId;

		// Token: 0x040017BE RID: 6078
		[NonSerialized]
		public int fieldId;
	}
}
