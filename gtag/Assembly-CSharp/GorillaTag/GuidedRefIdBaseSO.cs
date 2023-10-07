using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000303 RID: 771
	public abstract class GuidedRefIdBaseSO : ScriptableObject, IGuidedRefObject
	{
		// Token: 0x0600158F RID: 5519 RVA: 0x00077573 File Offset: 0x00075773
		public virtual void GuidedRefInitialize()
		{
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x0007757D File Offset: 0x0007577D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}
	}
}
