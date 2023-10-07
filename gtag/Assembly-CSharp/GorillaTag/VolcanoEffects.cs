using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x0200031D RID: 797
	public class VolcanoEffects : MonoBehaviour
	{
		// Token: 0x06001647 RID: 5703 RVA: 0x0007BB3C File Offset: 0x00079D3C
		protected void Awake()
		{
			if (this.RemoveNullsFromArray<Renderer>(ref this.renderers))
			{
				this.LogNullsFoundInArray("renderers");
			}
			if (this.RemoveNullsFromArray<ParticleSystem>(ref this.lavaSpewParticleSystems))
			{
				this.LogNullsFoundInArray("lavaSpewParticleSystems");
			}
			if (this.RemoveNullsFromArray<ParticleSystem>(ref this.smokeParticleSystems))
			{
				this.LogNullsFoundInArray("smokeParticleSystems");
			}
			this.hasVolcanoAudioSrc = (this.volcanoAudioSource != null);
			this.hasForestSpeakerAudioSrc = (this.forestSpeakerAudioSrc != null);
			this.lavaSpewEmissionModules = new ParticleSystem.EmissionModule[this.lavaSpewParticleSystems.Length];
			this.lavaSpewEmissionDefaultRateMultipliers = new float[this.lavaSpewParticleSystems.Length];
			this.lavaSpewDefaultEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
			this.lavaSpewAdjustedEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
			for (int i = 0; i < this.lavaSpewParticleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.lavaSpewParticleSystems[i].emission;
				this.lavaSpewEmissionDefaultRateMultipliers[i] = emission.rateOverTimeMultiplier;
				this.lavaSpewDefaultEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.lavaSpewAdjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					ParticleSystem.Burst burst = emission.GetBurst(j);
					this.lavaSpewDefaultEmitBursts[i][j] = burst;
					this.lavaSpewAdjustedEmitBursts[i][j] = new ParticleSystem.Burst(burst.time, burst.minCount, burst.maxCount, burst.cycleCount, burst.repeatInterval);
					this.lavaSpewAdjustedEmitBursts[i][j].count = burst.count;
				}
				this.lavaSpewEmissionModules[i] = emission;
			}
			this.smokeMainModules = new ParticleSystem.MainModule[this.smokeParticleSystems.Length];
			this.smokeEmissionModules = new ParticleSystem.EmissionModule[this.smokeParticleSystems.Length];
			this.smokeEmissionDefaultRateMultipliers = new float[this.smokeParticleSystems.Length];
			for (int k = 0; k < this.smokeParticleSystems.Length; k++)
			{
				this.smokeMainModules[k] = this.smokeParticleSystems[k].main;
				this.smokeEmissionModules[k] = this.smokeParticleSystems[k].emission;
				this.smokeEmissionDefaultRateMultipliers[k] = this.smokeEmissionModules[k].rateOverTimeMultiplier;
			}
			this.InitState(this.drainedStateFX);
			this.InitState(this.eruptingStateFX);
			this.InitState(this.risingStateFX);
			this.InitState(this.fullStateFX);
			this.InitState(this.drainingStateFX);
			this.UpdateDrainedState(0f);
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x0007BDD4 File Offset: 0x00079FD4
		public void OnVolcanoBellyEmpty()
		{
			if (!this.hasForestSpeakerAudioSrc)
			{
				return;
			}
			if (Time.time - this.timeVolcanoBellyWasLastEmpty < this.warnVolcanoBellyEmptied.length)
			{
				return;
			}
			this.forestSpeakerAudioSrc.gameObject.SetActive(true);
			this.forestSpeakerAudioSrc.PlayOneShot(this.warnVolcanoBellyEmptied, 1f);
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x0007BE2C File Offset: 0x0007A02C
		public void OnStoneAccepted(double activationProgress)
		{
			if (!this.hasVolcanoAudioSrc)
			{
				return;
			}
			this.volcanoAudioSource.gameObject.SetActive(true);
			if (activationProgress > 1.0)
			{
				this.volcanoAudioSource.PlayOneShot(this.volcanoAcceptLastStone, 1f);
				return;
			}
			this.volcanoAudioSource.PlayOneShot(this.volcanoAcceptStone, 1f);
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x0007BE8C File Offset: 0x0007A08C
		private void InitState(VolcanoEffects.LavaStateFX fx)
		{
			fx.startSoundExists = (fx.startSound != null);
			fx.endSoundExists = (fx.endSound != null);
			fx.loop1Exists = (fx.loop1AudioSrc != null);
			fx.loop2Exists = (fx.loop2AudioSrc != null);
			if (fx.loop1Exists)
			{
				fx.loop1DefaultVolume = fx.loop1AudioSrc.volume;
				fx.loop1AudioSrc.volume = 0f;
			}
			if (fx.loop2Exists)
			{
				fx.loop2DefaultVolume = fx.loop2AudioSrc.volume;
				fx.loop2AudioSrc.volume = 0f;
			}
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x0007BF33 File Offset: 0x0007A133
		private void ResetState(VolcanoEffects.LavaStateFX fx)
		{
			fx.startSoundPlayed = false;
			fx.endSoundPlayed = false;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x0007BF44 File Offset: 0x0007A144
		private void UpdateState(VolcanoEffects.LavaStateFX fx, float time, float timeRemaining, float progress)
		{
			if (fx.startSoundExists && !fx.startSoundPlayed && time >= fx.startSoundDelay)
			{
				fx.startSoundPlayed = true;
				fx.startSoundAudioSrc.gameObject.SetActive(true);
				fx.startSoundAudioSrc.PlayOneShot(fx.startSound, fx.startSoundVol);
			}
			if (fx.endSoundExists && !fx.endSoundPlayed && timeRemaining <= fx.endSound.length + fx.endSoundPadTime)
			{
				fx.endSoundPlayed = true;
				fx.endSoundAudioSrc.gameObject.SetActive(true);
				fx.endSoundAudioSrc.PlayOneShot(fx.endSound, fx.endSoundVol);
			}
			if (fx.loop1Exists)
			{
				fx.loop1AudioSrc.volume = fx.loop1VolAnim.Evaluate(progress) * fx.loop1DefaultVolume;
				if (!fx.loop1AudioSrc.isPlaying)
				{
					fx.loop1AudioSrc.gameObject.SetActive(true);
					fx.loop1AudioSrc.Play();
				}
			}
			if (fx.loop2Exists)
			{
				fx.loop2AudioSrc.volume = fx.loop2VolAnim.Evaluate(progress) * fx.loop2DefaultVolume;
				if (!fx.loop2AudioSrc.isPlaying)
				{
					fx.loop2AudioSrc.gameObject.SetActive(true);
					fx.loop2AudioSrc.Play();
				}
			}
			for (int i = 0; i < this.smokeMainModules.Length; i++)
			{
				this.smokeMainModules[i].startColor = fx.smokeStartColorAnim.Evaluate(progress);
				this.smokeEmissionModules[i].rateOverTimeMultiplier = fx.smokeEmissionAnim.Evaluate(progress) * this.smokeEmissionDefaultRateMultipliers[i];
			}
			this.SetParticleEmissionRateAndBurst(fx.lavaSpewEmissionAnim.Evaluate(progress), this.lavaSpewEmissionModules, this.lavaSpewEmissionDefaultRateMultipliers, this.lavaSpewDefaultEmitBursts, this.lavaSpewAdjustedEmitBursts);
			if (this.applyShaderGlobals)
			{
				Shader.SetGlobalColor(this.shaderProp_ZoneLiquidLightColor, fx.lavaLightColor.Evaluate(progress) * fx.lavaLightIntensityAnim.Evaluate(progress));
				Shader.SetGlobalFloat(this.shaderProp_ZoneLiquidLightDistScale, fx.lavaLightAttenuationAnim.Evaluate(progress));
			}
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x0007C160 File Offset: 0x0007A360
		public void UpdateDrainedState(float time)
		{
			this.ResetState(this.drainingStateFX);
			this.UpdateState(this.drainedStateFX, time, float.MaxValue, float.MinValue);
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x0007C1B4 File Offset: 0x0007A3B4
		public void UpdateEruptingState(float time, float timeRemaining, float progress)
		{
			this.ResetState(this.drainedStateFX);
			this.UpdateState(this.eruptingStateFX, time, timeRemaining, progress);
			foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
			{
				audioSource.volume = 0f;
				audioSource.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x0007C20C File Offset: 0x0007A40C
		public void UpdateRisingState(float time, float timeRemaining, float progress)
		{
			this.ResetState(this.eruptingStateFX);
			this.UpdateState(this.risingStateFX, time, timeRemaining, progress);
			foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
			{
				audioSource.volume = Mathf.Lerp(0f, 1f, Mathf.Clamp01(time));
				audioSource.gameObject.SetActive(true);
			}
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x0007C274 File Offset: 0x0007A474
		public void UpdateFullState(float time, float timeRemaining, float progress)
		{
			this.ResetState(this.risingStateFX);
			this.UpdateState(this.fullStateFX, time, timeRemaining, progress);
			foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
			{
				audioSource.volume = 1f;
				audioSource.gameObject.SetActive(true);
			}
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x0007C2CC File Offset: 0x0007A4CC
		public void UpdateDrainingState(float time, float timeRemaining, float progress)
		{
			this.ResetState(this.fullStateFX);
			this.UpdateState(this.drainingStateFX, time, timeRemaining, progress);
			foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
			{
				audioSource.volume = Mathf.Lerp(1f, 0f, progress);
				audioSource.gameObject.SetActive(true);
			}
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x0007C330 File Offset: 0x0007A530
		private void SetParticleEmissionRateAndBurst(float multiplier, ParticleSystem.EmissionModule[] emissionModules, float[] defaultRateMultipliers, ParticleSystem.Burst[][] defaultEmitBursts, ParticleSystem.Burst[][] adjustedEmitBursts)
		{
			for (int i = 0; i < emissionModules.Length; i++)
			{
				emissionModules[i].rateOverTimeMultiplier = multiplier * defaultRateMultipliers[i];
				int num = Mathf.Min(emissionModules[i].burstCount, defaultEmitBursts[i].Length);
				for (int j = 0; j < num; j++)
				{
					adjustedEmitBursts[i][j].probability = defaultEmitBursts[i][j].probability * multiplier;
				}
				emissionModules[i].SetBursts(adjustedEmitBursts[i]);
			}
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x0007C3B0 File Offset: 0x0007A5B0
		private bool RemoveNullsFromArray<T>(ref T[] array) where T : Object
		{
			List<T> list = new List<T>(array.Length);
			foreach (T t in array)
			{
				if (t != null)
				{
					list.Add(t);
				}
			}
			int num = array.Length;
			array = list.ToArray();
			return num != array.Length;
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x0007C40A File Offset: 0x0007A60A
		private void LogNullsFoundInArray(string nameOfArray)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Null reference found in ",
				nameOfArray,
				" array of component: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
		}

		// Token: 0x0400184F RID: 6223
		[Tooltip("Only one VolcanoEffects should change shader globals in the scene (lava color, lava light) at a time.")]
		[SerializeField]
		private bool applyShaderGlobals = true;

		// Token: 0x04001850 RID: 6224
		[Tooltip("Game trigger notification sounds will play through this.")]
		[SerializeField]
		private AudioSource forestSpeakerAudioSrc;

		// Token: 0x04001851 RID: 6225
		[Tooltip("The accumulator value of rocks being thrown into the volcano has been reset.")]
		[SerializeField]
		private AudioClip warnVolcanoBellyEmptied;

		// Token: 0x04001852 RID: 6226
		[Tooltip("Accept stone sounds will play through here.")]
		[SerializeField]
		private AudioSource volcanoAudioSource;

		// Token: 0x04001853 RID: 6227
		[Tooltip("volcano ate rock but needs more.")]
		[SerializeField]
		private AudioClip volcanoAcceptStone;

		// Token: 0x04001854 RID: 6228
		[Tooltip("volcano ate last needed rock.")]
		[SerializeField]
		private AudioClip volcanoAcceptLastStone;

		// Token: 0x04001855 RID: 6229
		[Tooltip("This will be faded in while lava is rising.")]
		[SerializeField]
		private AudioSource[] lavaSurfaceAudioSrcs;

		// Token: 0x04001856 RID: 6230
		[Tooltip("The renderers that will have shader properties changed.")]
		[SerializeField]
		private Renderer[] renderers;

		// Token: 0x04001857 RID: 6231
		[Tooltip("Emission will be adjusted for these particles during eruption.")]
		[SerializeField]
		private ParticleSystem[] lavaSpewParticleSystems;

		// Token: 0x04001858 RID: 6232
		[Tooltip("Smoke emits during all states but it's intensity and color will change when erupting/idling.")]
		[SerializeField]
		private ParticleSystem[] smokeParticleSystems;

		// Token: 0x04001859 RID: 6233
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainedStateFX;

		// Token: 0x0400185A RID: 6234
		[SerializeField]
		private VolcanoEffects.LavaStateFX eruptingStateFX;

		// Token: 0x0400185B RID: 6235
		[SerializeField]
		private VolcanoEffects.LavaStateFX risingStateFX;

		// Token: 0x0400185C RID: 6236
		[SerializeField]
		private VolcanoEffects.LavaStateFX fullStateFX;

		// Token: 0x0400185D RID: 6237
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainingStateFX;

		// Token: 0x0400185E RID: 6238
		private ParticleSystem.EmissionModule[] lavaSpewEmissionModules;

		// Token: 0x0400185F RID: 6239
		private float[] lavaSpewEmissionDefaultRateMultipliers;

		// Token: 0x04001860 RID: 6240
		private ParticleSystem.Burst[][] lavaSpewDefaultEmitBursts;

		// Token: 0x04001861 RID: 6241
		private ParticleSystem.Burst[][] lavaSpewAdjustedEmitBursts;

		// Token: 0x04001862 RID: 6242
		private ParticleSystem.MainModule[] smokeMainModules;

		// Token: 0x04001863 RID: 6243
		private ParticleSystem.EmissionModule[] smokeEmissionModules;

		// Token: 0x04001864 RID: 6244
		private float[] smokeEmissionDefaultRateMultipliers;

		// Token: 0x04001865 RID: 6245
		private int shaderProp_ZoneLiquidLightColor = Shader.PropertyToID("_ZoneLiquidLightColor");

		// Token: 0x04001866 RID: 6246
		private int shaderProp_ZoneLiquidLightDistScale = Shader.PropertyToID("_ZoneLiquidLightDistScale");

		// Token: 0x04001867 RID: 6247
		private float timeVolcanoBellyWasLastEmpty;

		// Token: 0x04001868 RID: 6248
		private bool hasVolcanoAudioSrc;

		// Token: 0x04001869 RID: 6249
		private bool hasForestSpeakerAudioSrc;

		// Token: 0x0200050A RID: 1290
		[Serializable]
		public class LavaStateFX
		{
			// Token: 0x04002100 RID: 8448
			public AudioClip startSound;

			// Token: 0x04002101 RID: 8449
			public AudioSource startSoundAudioSrc;

			// Token: 0x04002102 RID: 8450
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float startSoundVol = 1f;

			// Token: 0x04002103 RID: 8451
			[FormerlySerializedAs("startSoundPad")]
			public float startSoundDelay;

			// Token: 0x04002104 RID: 8452
			public AudioClip endSound;

			// Token: 0x04002105 RID: 8453
			public AudioSource endSoundAudioSrc;

			// Token: 0x04002106 RID: 8454
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float endSoundVol = 1f;

			// Token: 0x04002107 RID: 8455
			[Tooltip("How much time should there be between the end of the clip playing and the end of the state.")]
			public float endSoundPadTime;

			// Token: 0x04002108 RID: 8456
			public AudioSource loop1AudioSrc;

			// Token: 0x04002109 RID: 8457
			public AnimationCurve loop1VolAnim;

			// Token: 0x0400210A RID: 8458
			public AudioSource loop2AudioSrc;

			// Token: 0x0400210B RID: 8459
			public AnimationCurve loop2VolAnim;

			// Token: 0x0400210C RID: 8460
			public AnimationCurve lavaSpewEmissionAnim;

			// Token: 0x0400210D RID: 8461
			public AnimationCurve smokeEmissionAnim;

			// Token: 0x0400210E RID: 8462
			public Gradient smokeStartColorAnim;

			// Token: 0x0400210F RID: 8463
			public Gradient lavaLightColor;

			// Token: 0x04002110 RID: 8464
			public AnimationCurve lavaLightIntensityAnim = AnimationCurve.Constant(0f, 1f, 60f);

			// Token: 0x04002111 RID: 8465
			public AnimationCurve lavaLightAttenuationAnim = AnimationCurve.Constant(0f, 1f, 0.1f);

			// Token: 0x04002112 RID: 8466
			[NonSerialized]
			public bool startSoundExists;

			// Token: 0x04002113 RID: 8467
			[NonSerialized]
			public bool startSoundPlayed;

			// Token: 0x04002114 RID: 8468
			[NonSerialized]
			public bool endSoundExists;

			// Token: 0x04002115 RID: 8469
			[NonSerialized]
			public bool endSoundPlayed;

			// Token: 0x04002116 RID: 8470
			[NonSerialized]
			public bool loop1Exists;

			// Token: 0x04002117 RID: 8471
			[NonSerialized]
			public float loop1DefaultVolume;

			// Token: 0x04002118 RID: 8472
			[NonSerialized]
			public bool loop2Exists;

			// Token: 0x04002119 RID: 8473
			[NonSerialized]
			public float loop2DefaultVolume;
		}
	}
}
