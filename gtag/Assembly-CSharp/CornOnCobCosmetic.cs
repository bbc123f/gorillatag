using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class CornOnCobCosmetic : MonoBehaviour
{
	// Token: 0x06000101 RID: 257 RVA: 0x00009870 File Offset: 0x00007A70
	protected void Awake()
	{
		this.emissionModule = this.particleSys.emission;
		this.maxBurstProbability = ((this.emissionModule.burstCount > 0) ? this.emissionModule.GetBurst(0).probability : 0.2f);
	}

	// Token: 0x06000102 RID: 258 RVA: 0x000098C0 File Offset: 0x00007AC0
	protected void LateUpdate()
	{
		for (int i = 0; i < this.emissionModule.burstCount; i++)
		{
			ParticleSystem.Burst burst = this.emissionModule.GetBurst(i);
			burst.probability = this.maxBurstProbability * this.particleEmissionCurve.Evaluate(this.thermalReceiver.celsius);
			this.emissionModule.SetBurst(i, burst);
		}
		int particleCount = this.particleSys.particleCount;
		if (particleCount > this.previousParticleCount)
		{
			this.soundBankPlayer.Play(null, null);
		}
		this.previousParticleCount = particleCount;
	}

	// Token: 0x0400015F RID: 351
	[Tooltip("The corn will start popping based on the temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x04000160 RID: 352
	[Tooltip("The particle system that will be emitted when the heat source is hot enough.")]
	public ParticleSystem particleSys;

	// Token: 0x04000161 RID: 353
	[Tooltip("The curve that determines how many particles will be emitted based on the heat source's temperature.\n\nThe x-axis is the heat source's temperature and the y-axis is the number of particles to emit.")]
	public AnimationCurve particleEmissionCurve;

	// Token: 0x04000162 RID: 354
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000163 RID: 355
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x04000164 RID: 356
	private float maxBurstProbability;

	// Token: 0x04000165 RID: 357
	private int previousParticleCount;
}
