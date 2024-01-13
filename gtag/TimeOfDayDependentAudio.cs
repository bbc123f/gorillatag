using System.Collections;
using UnityEngine;

public class TimeOfDayDependentAudio : MonoBehaviour
{
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

	private void Awake()
	{
		stepTime = 1f;
		if (myParticleSystem != null)
		{
			myEmissionModule = myParticleSystem.emission;
			startingEmissionRate = myEmissionModule.rateOverTime.constant;
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateTimeOfDay());
	}

	private void FixedUpdate()
	{
		isModified = false;
	}

	private IEnumerator UpdateTimeOfDay()
	{
		yield return 0;
		while (true)
		{
			if (BetterDayNightManager.instance != null)
			{
				if (isModified)
				{
					positionMultiplier = positionMultiplierSet;
				}
				else
				{
					positionMultiplier = 1f;
				}
				if (myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == myWeather || BetterDayNightManager.instance.NextWeather() == myWeather)
				{
					if (!dependentStuff.activeSelf)
					{
						dependentStuff.SetActive(value: true);
					}
					if (includesAudio)
					{
						if (timeOfDayDependent != null)
						{
							if (volumes[BetterDayNightManager.instance.currentTimeIndex] == 0f)
							{
								if (timeOfDayDependent.activeSelf)
								{
									timeOfDayDependent.SetActive(value: false);
								}
							}
							else if (!timeOfDayDependent.activeSelf)
							{
								timeOfDayDependent.SetActive(value: true);
							}
						}
						if (volumes[BetterDayNightManager.instance.currentTimeIndex] != audioSources[0].volume)
						{
							if (BetterDayNightManager.instance.currentLerp < 0.05f)
							{
								currentVolume = Mathf.Lerp(currentVolume, volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
							}
							else
							{
								currentVolume = volumes[BetterDayNightManager.instance.currentTimeIndex];
							}
						}
					}
					if (myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == myWeather)
					{
						if (myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.NextWeather() == myWeather)
						{
							if (myParticleSystem != null)
							{
								newRate = startingEmissionRate;
							}
							if (includesAudio && myParticleSystem != null)
							{
								currentVolume = Mathf.Lerp(volumes[BetterDayNightManager.instance.currentTimeIndex], volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % volumes.Length], BetterDayNightManager.instance.currentLerp);
							}
							else if (includesAudio)
							{
								if (BetterDayNightManager.instance.currentLerp < 0.05f)
								{
									currentVolume = Mathf.Lerp(currentVolume, volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
								}
								else
								{
									currentVolume = volumes[BetterDayNightManager.instance.currentTimeIndex];
								}
							}
						}
						else
						{
							if (myParticleSystem != null)
							{
								newRate = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(startingEmissionRate, 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
							if (includesAudio)
							{
								currentVolume = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(volumes[BetterDayNightManager.instance.currentTimeIndex], 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
						}
					}
					else
					{
						if (myParticleSystem != null)
						{
							newRate = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, startingEmissionRate, (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
						if (includesAudio)
						{
							currentVolume = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % volumes.Length], (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
					}
					if (myParticleSystem != null)
					{
						myEmissionModule = myParticleSystem.emission;
						myEmissionModule.rateOverTime = newRate;
					}
					if (includesAudio)
					{
						for (int i = 0; i < audioSources.Length; i++)
						{
							audioSources[i].volume = currentVolume * positionMultiplier;
							audioSources[i].enabled = currentVolume != 0f;
						}
					}
				}
				else if (dependentStuff.activeSelf)
				{
					dependentStuff.SetActive(value: false);
				}
			}
			yield return new WaitForSeconds(stepTime);
		}
	}
}
