using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class TimeOfDayDependentAudio : MonoBehaviour
{
	// Token: 0x0600006B RID: 107 RVA: 0x00004AC0 File Offset: 0x00002CC0
	private void Awake()
	{
		this.stepTime = 1f;
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.startingEmissionRate = this.myEmissionModule.rateOverTime.constant;
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00004B10 File Offset: 0x00002D10
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00004B18 File Offset: 0x00002D18
	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateTimeOfDay());
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00004B27 File Offset: 0x00002D27
	private void FixedUpdate()
	{
		this.isModified = false;
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00004B30 File Offset: 0x00002D30
	private IEnumerator UpdateTimeOfDay()
	{
		yield return 0;
		for (;;)
		{
			if (BetterDayNightManager.instance != null)
			{
				if (this.isModified)
				{
					this.positionMultiplier = this.positionMultiplierSet;
				}
				else
				{
					this.positionMultiplier = 1f;
				}
				if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == this.myWeather || BetterDayNightManager.instance.NextWeather() == this.myWeather)
				{
					if (!this.dependentStuff.activeSelf)
					{
						this.dependentStuff.SetActive(true);
					}
					if (this.includesAudio)
					{
						if (this.timeOfDayDependent != null)
						{
							if (this.volumes[BetterDayNightManager.instance.currentTimeIndex] == 0f)
							{
								if (this.timeOfDayDependent.activeSelf)
								{
									this.timeOfDayDependent.SetActive(false);
								}
							}
							else if (!this.timeOfDayDependent.activeSelf)
							{
								this.timeOfDayDependent.SetActive(true);
							}
						}
						if (this.volumes[BetterDayNightManager.instance.currentTimeIndex] != this.audioSources[0].volume)
						{
							if (BetterDayNightManager.instance.currentLerp < 0.05f)
							{
								this.currentVolume = Mathf.Lerp(this.currentVolume, this.volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
							}
							else
							{
								this.currentVolume = this.volumes[BetterDayNightManager.instance.currentTimeIndex];
							}
						}
					}
					if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == this.myWeather)
					{
						if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.NextWeather() == this.myWeather)
						{
							if (this.myParticleSystem != null)
							{
								this.newRate = this.startingEmissionRate;
							}
							if (this.includesAudio && this.myParticleSystem != null)
							{
								this.currentVolume = Mathf.Lerp(this.volumes[BetterDayNightManager.instance.currentTimeIndex], this.volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % this.volumes.Length], BetterDayNightManager.instance.currentLerp);
							}
							else if (this.includesAudio)
							{
								if (BetterDayNightManager.instance.currentLerp < 0.05f)
								{
									this.currentVolume = Mathf.Lerp(this.currentVolume, this.volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
								}
								else
								{
									this.currentVolume = this.volumes[BetterDayNightManager.instance.currentTimeIndex];
								}
							}
						}
						else
						{
							if (this.myParticleSystem != null)
							{
								this.newRate = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(this.startingEmissionRate, 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
							if (this.includesAudio)
							{
								this.currentVolume = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(this.volumes[BetterDayNightManager.instance.currentTimeIndex], 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
						}
					}
					else
					{
						if (this.myParticleSystem != null)
						{
							this.newRate = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, this.startingEmissionRate, (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
						if (this.includesAudio)
						{
							this.currentVolume = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, this.volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % this.volumes.Length], (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
					}
					if (this.myParticleSystem != null)
					{
						this.myEmissionModule = this.myParticleSystem.emission;
						this.myEmissionModule.rateOverTime = this.newRate;
					}
					if (this.includesAudio)
					{
						for (int i = 0; i < this.audioSources.Length; i++)
						{
							this.audioSources[i].volume = this.currentVolume * this.positionMultiplier;
							this.audioSources[i].enabled = (this.currentVolume != 0f);
						}
					}
				}
				else if (this.dependentStuff.activeSelf)
				{
					this.dependentStuff.SetActive(false);
				}
			}
			yield return new WaitForSeconds(this.stepTime);
		}
		yield break;
	}

	// Token: 0x04000095 RID: 149
	public AudioSource[] audioSources;

	// Token: 0x04000096 RID: 150
	public float[] volumes;

	// Token: 0x04000097 RID: 151
	public float currentVolume;

	// Token: 0x04000098 RID: 152
	public float stepTime;

	// Token: 0x04000099 RID: 153
	public BetterDayNightManager.WeatherType myWeather;

	// Token: 0x0400009A RID: 154
	public GameObject dependentStuff;

	// Token: 0x0400009B RID: 155
	public GameObject timeOfDayDependent;

	// Token: 0x0400009C RID: 156
	public bool includesAudio;

	// Token: 0x0400009D RID: 157
	public ParticleSystem myParticleSystem;

	// Token: 0x0400009E RID: 158
	private float startingEmissionRate;

	// Token: 0x0400009F RID: 159
	private int lastEmission;

	// Token: 0x040000A0 RID: 160
	private int nextEmission;

	// Token: 0x040000A1 RID: 161
	private ParticleSystem.MinMaxCurve newCurve;

	// Token: 0x040000A2 RID: 162
	private ParticleSystem.EmissionModule myEmissionModule;

	// Token: 0x040000A3 RID: 163
	private float newRate;

	// Token: 0x040000A4 RID: 164
	public float positionMultiplierSet;

	// Token: 0x040000A5 RID: 165
	public float positionMultiplier = 1f;

	// Token: 0x040000A6 RID: 166
	public bool isModified;
}
