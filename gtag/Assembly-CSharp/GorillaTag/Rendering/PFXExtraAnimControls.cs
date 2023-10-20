using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x0200032B RID: 811
	public class PFXExtraAnimControls : MonoBehaviour
	{
		// Token: 0x06001686 RID: 5766 RVA: 0x0007D848 File Offset: 0x0007BA48
		protected void Awake()
		{
			this.emissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			this.cachedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			this.adjustedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.particleSystems[i].emission;
				this.cachedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.adjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					this.cachedEmitBursts[i][j] = emission.GetBurst(j);
					this.adjustedEmitBursts[i][j] = emission.GetBurst(j);
				}
				this.emissionModules[i] = emission;
			}
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x0007D928 File Offset: 0x0007BB28
		protected void LateUpdate()
		{
			for (int i = 0; i < this.emissionModules.Length; i++)
			{
				this.emissionModules[i].rateOverTimeMultiplier = this.emitRateMult;
				Mathf.Min(this.emissionModules[i].burstCount, this.cachedEmitBursts[i].Length);
				for (int j = 0; j < this.cachedEmitBursts[i].Length; j++)
				{
					this.adjustedEmitBursts[i][j].probability = this.cachedEmitBursts[i][j].probability * this.emitBurstProbabilityMult;
				}
				this.emissionModules[i].SetBursts(this.adjustedEmitBursts[i]);
			}
		}

		// Token: 0x0400189A RID: 6298
		public float emitRateMult = 1f;

		// Token: 0x0400189B RID: 6299
		public float emitBurstProbabilityMult = 1f;

		// Token: 0x0400189C RID: 6300
		[SerializeField]
		private ParticleSystem[] particleSystems;

		// Token: 0x0400189D RID: 6301
		private ParticleSystem.EmissionModule[] emissionModules;

		// Token: 0x0400189E RID: 6302
		private ParticleSystem.Burst[][] cachedEmitBursts;

		// Token: 0x0400189F RID: 6303
		private ParticleSystem.Burst[][] adjustedEmitBursts;
	}
}
