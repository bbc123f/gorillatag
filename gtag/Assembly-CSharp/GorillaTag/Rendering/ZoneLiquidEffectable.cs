using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x0200032C RID: 812
	public sealed class ZoneLiquidEffectable : MonoBehaviour
	{
		// Token: 0x06001689 RID: 5769 RVA: 0x0007D9FF File Offset: 0x0007BBFF
		private void Awake()
		{
			this.childRenderers = base.GetComponentsInChildren<Renderer>(false);
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x0007DA0E File Offset: 0x0007BC0E
		private void OnEnable()
		{
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x0007DA10 File Offset: 0x0007BC10
		private void OnDisable()
		{
		}

		// Token: 0x040018A0 RID: 6304
		public float radius = 1f;

		// Token: 0x040018A1 RID: 6305
		[NonSerialized]
		public bool inLiquidVolume;

		// Token: 0x040018A2 RID: 6306
		[NonSerialized]
		public bool wasInLiquidVolume;

		// Token: 0x040018A3 RID: 6307
		[NonSerialized]
		public Renderer[] childRenderers;
	}
}
