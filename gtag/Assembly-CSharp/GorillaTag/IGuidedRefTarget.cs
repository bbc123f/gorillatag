using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000308 RID: 776
	public interface IGuidedRefTarget : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000179 RID: 377
		// (get) Token: 0x0600159E RID: 5534
		GuidedRefTargetIdSO GuidedRefTargetId { get; }

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x0600159F RID: 5535
		Object GuidedRefTargetObject { get; }
	}
}
