using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x0200032A RID: 810
	public sealed class ZoneLiquidEffectable : MonoBehaviour
	{
		// Token: 0x06001680 RID: 5760 RVA: 0x0007D517 File Offset: 0x0007B717
		private void Awake()
		{
			this.childRenderers = base.GetComponentsInChildren<Renderer>(false);
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x0007D526 File Offset: 0x0007B726
		private void OnEnable()
		{
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x0007D528 File Offset: 0x0007B728
		private void OnDisable()
		{
		}

		// Token: 0x04001893 RID: 6291
		public float radius = 1f;

		// Token: 0x04001894 RID: 6292
		[NonSerialized]
		public bool inLiquidVolume;

		// Token: 0x04001895 RID: 6293
		[NonSerialized]
		public bool wasInLiquidVolume;

		// Token: 0x04001896 RID: 6294
		[NonSerialized]
		public Renderer[] childRenderers;
	}
}
