using System;
using UnityEngine;

namespace GorillaTag
{
	[Serializable]
	public struct GuidedRefBasicReceiverFieldInfo
	{
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefRelayHubIdSO hubId;

		[NonSerialized]
		public int fieldId;
	}
}
