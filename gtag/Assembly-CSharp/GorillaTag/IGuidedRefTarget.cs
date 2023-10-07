using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000306 RID: 774
	public interface IGuidedRefTarget : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06001595 RID: 5525
		GuidedRefTargetIdSO GuidedRefTargetId { get; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06001596 RID: 5526
		Object GuidedRefTargetObject { get; }
	}
}
