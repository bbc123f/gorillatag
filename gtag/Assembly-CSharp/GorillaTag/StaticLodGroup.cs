using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000313 RID: 787
	public class StaticLodGroup : MonoBehaviour
	{
		// Token: 0x060015BA RID: 5562 RVA: 0x00077DC2 File Offset: 0x00075FC2
		protected void Awake()
		{
			this.index = StaticLodManager.Register(this);
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x00077DD0 File Offset: 0x00075FD0
		protected void OnEnable()
		{
			StaticLodManager.SetEnabled(this.index, true);
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x00077DDE File Offset: 0x00075FDE
		protected void OnDisable()
		{
			StaticLodManager.SetEnabled(this.index, false);
		}

		// Token: 0x040017BE RID: 6078
		private int index;

		// Token: 0x040017BF RID: 6079
		public float collisionEnableDistance = 3f;

		// Token: 0x040017C0 RID: 6080
		public float uiFadeDistanceMin = 1f;

		// Token: 0x040017C1 RID: 6081
		public float uiFadeDistanceMax = 10f;
	}
}
