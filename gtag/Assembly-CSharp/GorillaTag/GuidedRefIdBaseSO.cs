using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000305 RID: 773
	public abstract class GuidedRefIdBaseSO : ScriptableObject, IGuidedRefObject
	{
		// Token: 0x06001598 RID: 5528 RVA: 0x00077A5B File Offset: 0x00075C5B
		public virtual void GuidedRefInitialize()
		{
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x00077A65 File Offset: 0x00075C65
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}
	}
}
