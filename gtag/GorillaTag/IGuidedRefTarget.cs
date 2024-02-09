using System;
using UnityEngine;

namespace GorillaTag
{
	public interface IGuidedRefTarget : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		GuidedRefTargetIdSO GuidedRefTargetId { get; }

		Object GuidedRefTargetObject { get; }
	}
}
