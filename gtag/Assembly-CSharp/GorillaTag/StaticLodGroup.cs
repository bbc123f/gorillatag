using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000315 RID: 789
	public class StaticLodGroup : MonoBehaviour
	{
		// Token: 0x060015C3 RID: 5571 RVA: 0x000782AA File Offset: 0x000764AA
		protected void Awake()
		{
			this.index = StaticLodManager.Register(this);
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x000782B8 File Offset: 0x000764B8
		protected void OnEnable()
		{
			StaticLodManager.SetEnabled(this.index, true);
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x000782C6 File Offset: 0x000764C6
		protected void OnDisable()
		{
			StaticLodManager.SetEnabled(this.index, false);
		}

		// Token: 0x040017CB RID: 6091
		private int index;

		// Token: 0x040017CC RID: 6092
		public float collisionEnableDistance = 3f;

		// Token: 0x040017CD RID: 6093
		public float uiFadeDistanceMin = 1f;

		// Token: 0x040017CE RID: 6094
		public float uiFadeDistanceMax = 10f;
	}
}
