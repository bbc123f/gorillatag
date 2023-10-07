using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class WaterSplashEffect : MonoBehaviour
{
	// Token: 0x0600025B RID: 603 RVA: 0x0000FD77 File Offset: 0x0000DF77
	private void OnEnable()
	{
		this.startTime = Time.time;
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0000FD84 File Offset: 0x0000DF84
	public void Destroy()
	{
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.DeactivateParticleSystems(this.smallSplashParticleSystems);
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x0600025D RID: 605 RVA: 0x0000FDB8 File Offset: 0x0000DFB8
	public void PlayEffect(bool isBigSplash, bool isEntry, float scale, WaterVolume volume = null)
	{
		this.waterVolume = volume;
		if (isBigSplash)
		{
			this.DeactivateParticleSystems(this.smallSplashParticleSystems);
			this.SetParticleEffectParameters(this.bigSplashParticleSystems, scale, this.bigSplashBaseGravityMultiplier, this.bigSplashBaseStartSpeed, this.bigSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.bigSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.bigSplashAudioClips, ref WaterSplashEffect.lastPlayedBigSplashAudioClipIndex);
			return;
		}
		if (isEntry)
		{
			this.DeactivateParticleSystems(this.bigSplashParticleSystems);
			this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.smallSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.smallSplashEntryAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashEntryAudioClipIndex);
			return;
		}
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
		this.PlayParticleEffects(this.smallSplashParticleSystems);
		this.PlayRandomAudioClipWithoutRepeats(this.smallSplashExitAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashExitAudioClipIndex);
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0000FEC0 File Offset: 0x0000E0C0
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 b = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - b;
		}
		if ((Time.time - this.startTime) / this.lifeTime >= 1f)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x0600025F RID: 607 RVA: 0x0000FF88 File Offset: 0x0000E188
	private void DeactivateParticleSystems(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0000FFB4 File Offset: 0x0000E1B4
	private void PlayParticleEffects(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(true);
				particleSystems[i].Play();
			}
		}
	}

	// Token: 0x06000261 RID: 609 RVA: 0x0000FFE8 File Offset: 0x0000E1E8
	private void SetParticleEffectParameters(ParticleSystem[] particleSystems, float scale, float baseGravMultiplier, float baseStartSpeed, float baseSimulationSpeed, WaterVolume waterVolume = null)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem.MainModule main = particleSystems[i].main;
				main.startSpeed = baseStartSpeed;
				main.gravityModifier = baseGravMultiplier;
				if (scale < 0.99f)
				{
					main.startSpeed = baseStartSpeed * scale * 2f;
					main.gravityModifier = baseGravMultiplier * scale * 0.5f;
				}
				if (waterVolume != null && waterVolume.Parameters != null)
				{
					particleSystems[i].colorBySpeed.color = waterVolume.Parameters.splashColorBySpeedGradient;
				}
			}
		}
	}

	// Token: 0x06000262 RID: 610 RVA: 0x000100A0 File Offset: 0x0000E2A0
	private void PlayRandomAudioClipWithoutRepeats(AudioClip[] audioClips, ref int lastPlayedAudioClipIndex)
	{
		if (this.audioSource != null && audioClips != null && audioClips.Length != 0)
		{
			int num = 0;
			if (audioClips.Length > 1)
			{
				int num2 = Random.Range(0, audioClips.Length);
				if (num2 == lastPlayedAudioClipIndex)
				{
					num2 = ((Random.Range(0f, 1f) > 0.5f) ? ((num2 + 1) % audioClips.Length) : (num2 - 1));
					if (num2 < 0)
					{
						num2 = audioClips.Length - 1;
					}
				}
				num = num2;
			}
			lastPlayedAudioClipIndex = num;
			this.audioSource.clip = audioClips[num];
			this.audioSource.Play();
		}
	}

	// Token: 0x04000319 RID: 793
	private static int lastPlayedBigSplashAudioClipIndex = -1;

	// Token: 0x0400031A RID: 794
	private static int lastPlayedSmallSplashEntryAudioClipIndex = -1;

	// Token: 0x0400031B RID: 795
	private static int lastPlayedSmallSplashExitAudioClipIndex = -1;

	// Token: 0x0400031C RID: 796
	public ParticleSystem[] bigSplashParticleSystems;

	// Token: 0x0400031D RID: 797
	public ParticleSystem[] smallSplashParticleSystems;

	// Token: 0x0400031E RID: 798
	public float bigSplashBaseGravityMultiplier = 0.9f;

	// Token: 0x0400031F RID: 799
	public float bigSplashBaseStartSpeed = 1.9f;

	// Token: 0x04000320 RID: 800
	public float bigSplashBaseSimulationSpeed = 0.9f;

	// Token: 0x04000321 RID: 801
	public float smallSplashBaseGravityMultiplier = 0.6f;

	// Token: 0x04000322 RID: 802
	public float smallSplashBaseStartSpeed = 0.6f;

	// Token: 0x04000323 RID: 803
	public float smallSplashBaseSimulationSpeed = 0.6f;

	// Token: 0x04000324 RID: 804
	public float lifeTime = 1f;

	// Token: 0x04000325 RID: 805
	private float startTime = -1f;

	// Token: 0x04000326 RID: 806
	public AudioSource audioSource;

	// Token: 0x04000327 RID: 807
	public AudioClip[] bigSplashAudioClips;

	// Token: 0x04000328 RID: 808
	public AudioClip[] smallSplashEntryAudioClips;

	// Token: 0x04000329 RID: 809
	public AudioClip[] smallSplashExitAudioClips;

	// Token: 0x0400032A RID: 810
	private WaterVolume waterVolume;
}
