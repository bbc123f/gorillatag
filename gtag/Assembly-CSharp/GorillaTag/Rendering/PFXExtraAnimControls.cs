using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000329 RID: 809
	public class PFXExtraAnimControls : MonoBehaviour
	{
		// Token: 0x0600167D RID: 5757 RVA: 0x0007D360 File Offset: 0x0007B560
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

		// Token: 0x0600167E RID: 5758 RVA: 0x0007D440 File Offset: 0x0007B640
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

		// Token: 0x0400188D RID: 6285
		public float emitRateMult = 1f;

		// Token: 0x0400188E RID: 6286
		public float emitBurstProbabilityMult = 1f;

		// Token: 0x0400188F RID: 6287
		[SerializeField]
		private ParticleSystem[] particleSystems;

		// Token: 0x04001890 RID: 6288
		private ParticleSystem.EmissionModule[] emissionModules;

		// Token: 0x04001891 RID: 6289
		private ParticleSystem.Burst[][] cachedEmitBursts;

		// Token: 0x04001892 RID: 6290
		private ParticleSystem.Burst[][] adjustedEmitBursts;
	}
}
