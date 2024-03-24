using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TimeOfDayDependentAudio : MonoBehaviour
{
	private void Awake()
	{
		this.stepTime = 1f;
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.startingEmissionRate = this.myEmissionModule.rateOverTime.constant;
		}
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateTimeOfDay());
	}

	private void FixedUpdate()
	{
		this.isModified = false;
	}

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
							this.audioSources[i].enabled = this.currentVolume != 0f;
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

	public TimeOfDayDependentAudio()
	{
	}

	public AudioSource[] audioSources;

	public float[] volumes;

	public float currentVolume;

	public float stepTime;

	public BetterDayNightManager.WeatherType myWeather;

	public GameObject dependentStuff;

	public GameObject timeOfDayDependent;

	public bool includesAudio;

	public ParticleSystem myParticleSystem;

	private float startingEmissionRate;

	private int lastEmission;

	private int nextEmission;

	private ParticleSystem.MinMaxCurve newCurve;

	private ParticleSystem.EmissionModule myEmissionModule;

	private float newRate;

	public float positionMultiplierSet;

	public float positionMultiplier = 1f;

	public bool isModified;

	[CompilerGenerated]
	private sealed class <UpdateTimeOfDay>d__22 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateTimeOfDay>d__22(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			TimeOfDayDependentAudio timeOfDayDependentAudio = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				this.<>2__current = 0;
				this.<>1__state = 1;
				return true;
			case 1:
				this.<>1__state = -1;
				break;
			case 2:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			if (BetterDayNightManager.instance != null)
			{
				if (timeOfDayDependentAudio.isModified)
				{
					timeOfDayDependentAudio.positionMultiplier = timeOfDayDependentAudio.positionMultiplierSet;
				}
				else
				{
					timeOfDayDependentAudio.positionMultiplier = 1f;
				}
				if (timeOfDayDependentAudio.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == timeOfDayDependentAudio.myWeather || BetterDayNightManager.instance.NextWeather() == timeOfDayDependentAudio.myWeather)
				{
					if (!timeOfDayDependentAudio.dependentStuff.activeSelf)
					{
						timeOfDayDependentAudio.dependentStuff.SetActive(true);
					}
					if (timeOfDayDependentAudio.includesAudio)
					{
						if (timeOfDayDependentAudio.timeOfDayDependent != null)
						{
							if (timeOfDayDependentAudio.volumes[BetterDayNightManager.instance.currentTimeIndex] == 0f)
							{
								if (timeOfDayDependentAudio.timeOfDayDependent.activeSelf)
								{
									timeOfDayDependentAudio.timeOfDayDependent.SetActive(false);
								}
							}
							else if (!timeOfDayDependentAudio.timeOfDayDependent.activeSelf)
							{
								timeOfDayDependentAudio.timeOfDayDependent.SetActive(true);
							}
						}
						if (timeOfDayDependentAudio.volumes[BetterDayNightManager.instance.currentTimeIndex] != timeOfDayDependentAudio.audioSources[0].volume)
						{
							if (BetterDayNightManager.instance.currentLerp < 0.05f)
							{
								timeOfDayDependentAudio.currentVolume = Mathf.Lerp(timeOfDayDependentAudio.currentVolume, timeOfDayDependentAudio.volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
							}
							else
							{
								timeOfDayDependentAudio.currentVolume = timeOfDayDependentAudio.volumes[BetterDayNightManager.instance.currentTimeIndex];
							}
						}
					}
					if (timeOfDayDependentAudio.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == timeOfDayDependentAudio.myWeather)
					{
						if (timeOfDayDependentAudio.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.NextWeather() == timeOfDayDependentAudio.myWeather)
						{
							if (timeOfDayDependentAudio.myParticleSystem != null)
							{
								timeOfDayDependentAudio.newRate = timeOfDayDependentAudio.startingEmissionRate;
							}
							if (timeOfDayDependentAudio.includesAudio && timeOfDayDependentAudio.myParticleSystem != null)
							{
								timeOfDayDependentAudio.currentVolume = Mathf.Lerp(timeOfDayDependentAudio.volumes[BetterDayNightManager.instance.currentTimeIndex], timeOfDayDependentAudio.volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % timeOfDayDependentAudio.volumes.Length], BetterDayNightManager.instance.currentLerp);
							}
							else if (timeOfDayDependentAudio.includesAudio)
							{
								if (BetterDayNightManager.instance.currentLerp < 0.05f)
								{
									timeOfDayDependentAudio.currentVolume = Mathf.Lerp(timeOfDayDependentAudio.currentVolume, timeOfDayDependentAudio.volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
								}
								else
								{
									timeOfDayDependentAudio.currentVolume = timeOfDayDependentAudio.volumes[BetterDayNightManager.instance.currentTimeIndex];
								}
							}
						}
						else
						{
							if (timeOfDayDependentAudio.myParticleSystem != null)
							{
								timeOfDayDependentAudio.newRate = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(timeOfDayDependentAudio.startingEmissionRate, 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
							if (timeOfDayDependentAudio.includesAudio)
							{
								timeOfDayDependentAudio.currentVolume = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(timeOfDayDependentAudio.volumes[BetterDayNightManager.instance.currentTimeIndex], 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
						}
					}
					else
					{
						if (timeOfDayDependentAudio.myParticleSystem != null)
						{
							timeOfDayDependentAudio.newRate = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, timeOfDayDependentAudio.startingEmissionRate, (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
						if (timeOfDayDependentAudio.includesAudio)
						{
							timeOfDayDependentAudio.currentVolume = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, timeOfDayDependentAudio.volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % timeOfDayDependentAudio.volumes.Length], (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
					}
					if (timeOfDayDependentAudio.myParticleSystem != null)
					{
						timeOfDayDependentAudio.myEmissionModule = timeOfDayDependentAudio.myParticleSystem.emission;
						timeOfDayDependentAudio.myEmissionModule.rateOverTime = timeOfDayDependentAudio.newRate;
					}
					if (timeOfDayDependentAudio.includesAudio)
					{
						for (int i = 0; i < timeOfDayDependentAudio.audioSources.Length; i++)
						{
							timeOfDayDependentAudio.audioSources[i].volume = timeOfDayDependentAudio.currentVolume * timeOfDayDependentAudio.positionMultiplier;
							timeOfDayDependentAudio.audioSources[i].enabled = timeOfDayDependentAudio.currentVolume != 0f;
						}
					}
				}
				else if (timeOfDayDependentAudio.dependentStuff.activeSelf)
				{
					timeOfDayDependentAudio.dependentStuff.SetActive(false);
				}
			}
			this.<>2__current = new WaitForSeconds(timeOfDayDependentAudio.stepTime);
			this.<>1__state = 2;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public TimeOfDayDependentAudio <>4__this;
	}
}
