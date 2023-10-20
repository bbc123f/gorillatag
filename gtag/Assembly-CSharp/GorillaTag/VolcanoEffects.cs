using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x0200031F RID: 799
	public class VolcanoEffects : MonoBehaviour
	{
		// Token: 0x06001650 RID: 5712 RVA: 0x0007C024 File Offset: 0x0007A224
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

		// Token: 0x06001651 RID: 5713 RVA: 0x0007C2BC File Offset: 0x0007A4BC
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

		// Token: 0x06001652 RID: 5714 RVA: 0x0007C314 File Offset: 0x0007A514
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

		// Token: 0x06001653 RID: 5715 RVA: 0x0007C374 File Offset: 0x0007A574
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

		// Token: 0x06001654 RID: 5716 RVA: 0x0007C41B File Offset: 0x0007A61B
		private void ResetState(VolcanoEffects.LavaStateFX fx)
		{
			fx.startSoundPlayed = false;
			fx.endSoundPlayed = false;
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x0007C42C File Offset: 0x0007A62C
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

		// Token: 0x06001656 RID: 5718 RVA: 0x0007C648 File Offset: 0x0007A848
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

		// Token: 0x06001657 RID: 5719 RVA: 0x0007C69C File Offset: 0x0007A89C
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

		// Token: 0x06001658 RID: 5720 RVA: 0x0007C6F4 File Offset: 0x0007A8F4
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

		// Token: 0x06001659 RID: 5721 RVA: 0x0007C75C File Offset: 0x0007A95C
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

		// Token: 0x0600165A RID: 5722 RVA: 0x0007C7B4 File Offset: 0x0007A9B4
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

		// Token: 0x0600165B RID: 5723 RVA: 0x0007C818 File Offset: 0x0007AA18
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

		// Token: 0x0600165C RID: 5724 RVA: 0x0007C898 File Offset: 0x0007AA98
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

		// Token: 0x0600165D RID: 5725 RVA: 0x0007C8F2 File Offset: 0x0007AAF2
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

		// Token: 0x0400185C RID: 6236
		[Tooltip("Only one VolcanoEffects should change shader globals in the scene (lava color, lava light) at a time.")]
		[SerializeField]
		private bool applyShaderGlobals = true;

		// Token: 0x0400185D RID: 6237
		[Tooltip("Game trigger notification sounds will play through this.")]
		[SerializeField]
		private AudioSource forestSpeakerAudioSrc;

		// Token: 0x0400185E RID: 6238
		[Tooltip("The accumulator value of rocks being thrown into the volcano has been reset.")]
		[SerializeField]
		private AudioClip warnVolcanoBellyEmptied;

		// Token: 0x0400185F RID: 6239
		[Tooltip("Accept stone sounds will play through here.")]
		[SerializeField]
		private AudioSource volcanoAudioSource;

		// Token: 0x04001860 RID: 6240
		[Tooltip("volcano ate rock but needs more.")]
		[SerializeField]
		private AudioClip volcanoAcceptStone;

		// Token: 0x04001861 RID: 6241
		[Tooltip("volcano ate last needed rock.")]
		[SerializeField]
		private AudioClip volcanoAcceptLastStone;

		// Token: 0x04001862 RID: 6242
		[Tooltip("This will be faded in while lava is rising.")]
		[SerializeField]
		private AudioSource[] lavaSurfaceAudioSrcs;

		// Token: 0x04001863 RID: 6243
		[Tooltip("The renderers that will have shader properties changed.")]
		[SerializeField]
		private Renderer[] renderers;

		// Token: 0x04001864 RID: 6244
		[Tooltip("Emission will be adjusted for these particles during eruption.")]
		[SerializeField]
		private ParticleSystem[] lavaSpewParticleSystems;

		// Token: 0x04001865 RID: 6245
		[Tooltip("Smoke emits during all states but it's intensity and color will change when erupting/idling.")]
		[SerializeField]
		private ParticleSystem[] smokeParticleSystems;

		// Token: 0x04001866 RID: 6246
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainedStateFX;

		// Token: 0x04001867 RID: 6247
		[SerializeField]
		private VolcanoEffects.LavaStateFX eruptingStateFX;

		// Token: 0x04001868 RID: 6248
		[SerializeField]
		private VolcanoEffects.LavaStateFX risingStateFX;

		// Token: 0x04001869 RID: 6249
		[SerializeField]
		private VolcanoEffects.LavaStateFX fullStateFX;

		// Token: 0x0400186A RID: 6250
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainingStateFX;

		// Token: 0x0400186B RID: 6251
		private ParticleSystem.EmissionModule[] lavaSpewEmissionModules;

		// Token: 0x0400186C RID: 6252
		private float[] lavaSpewEmissionDefaultRateMultipliers;

		// Token: 0x0400186D RID: 6253
		private ParticleSystem.Burst[][] lavaSpewDefaultEmitBursts;

		// Token: 0x0400186E RID: 6254
		private ParticleSystem.Burst[][] lavaSpewAdjustedEmitBursts;

		// Token: 0x0400186F RID: 6255
		private ParticleSystem.MainModule[] smokeMainModules;

		// Token: 0x04001870 RID: 6256
		private ParticleSystem.EmissionModule[] smokeEmissionModules;

		// Token: 0x04001871 RID: 6257
		private float[] smokeEmissionDefaultRateMultipliers;

		// Token: 0x04001872 RID: 6258
		private int shaderProp_ZoneLiquidLightColor = Shader.PropertyToID("_ZoneLiquidLightColor");

		// Token: 0x04001873 RID: 6259
		private int shaderProp_ZoneLiquidLightDistScale = Shader.PropertyToID("_ZoneLiquidLightDistScale");

		// Token: 0x04001874 RID: 6260
		private float timeVolcanoBellyWasLastEmpty;

		// Token: 0x04001875 RID: 6261
		private bool hasVolcanoAudioSrc;

		// Token: 0x04001876 RID: 6262
		private bool hasForestSpeakerAudioSrc;

		// Token: 0x0200050C RID: 1292
		[Serializable]
		public class LavaStateFX
		{
			// Token: 0x0400210D RID: 8461
			public AudioClip startSound;

			// Token: 0x0400210E RID: 8462
			public AudioSource startSoundAudioSrc;

			// Token: 0x0400210F RID: 8463
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float startSoundVol = 1f;

			// Token: 0x04002110 RID: 8464
			[FormerlySerializedAs("startSoundPad")]
			public float startSoundDelay;

			// Token: 0x04002111 RID: 8465
			public AudioClip endSound;

			// Token: 0x04002112 RID: 8466
			public AudioSource endSoundAudioSrc;

			// Token: 0x04002113 RID: 8467
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float endSoundVol = 1f;

			// Token: 0x04002114 RID: 8468
			[Tooltip("How much time should there be between the end of the clip playing and the end of the state.")]
			public float endSoundPadTime;

			// Token: 0x04002115 RID: 8469
			public AudioSource loop1AudioSrc;

			// Token: 0x04002116 RID: 8470
			public AnimationCurve loop1VolAnim;

			// Token: 0x04002117 RID: 8471
			public AudioSource loop2AudioSrc;

			// Token: 0x04002118 RID: 8472
			public AnimationCurve loop2VolAnim;

			// Token: 0x04002119 RID: 8473
			public AnimationCurve lavaSpewEmissionAnim;

			// Token: 0x0400211A RID: 8474
			public AnimationCurve smokeEmissionAnim;

			// Token: 0x0400211B RID: 8475
			public Gradient smokeStartColorAnim;

			// Token: 0x0400211C RID: 8476
			public Gradient lavaLightColor;

			// Token: 0x0400211D RID: 8477
			public AnimationCurve lavaLightIntensityAnim = AnimationCurve.Constant(0f, 1f, 60f);

			// Token: 0x0400211E RID: 8478
			public AnimationCurve lavaLightAttenuationAnim = AnimationCurve.Constant(0f, 1f, 0.1f);

			// Token: 0x0400211F RID: 8479
			[NonSerialized]
			public bool startSoundExists;

			// Token: 0x04002120 RID: 8480
			[NonSerialized]
			public bool startSoundPlayed;

			// Token: 0x04002121 RID: 8481
			[NonSerialized]
			public bool endSoundExists;

			// Token: 0x04002122 RID: 8482
			[NonSerialized]
			public bool endSoundPlayed;

			// Token: 0x04002123 RID: 8483
			[NonSerialized]
			public bool loop1Exists;

			// Token: 0x04002124 RID: 8484
			[NonSerialized]
			public float loop1DefaultVolume;

			// Token: 0x04002125 RID: 8485
			[NonSerialized]
			public bool loop2Exists;

			// Token: 0x04002126 RID: 8486
			[NonSerialized]
			public float loop2DefaultVolume;
		}
	}
}
