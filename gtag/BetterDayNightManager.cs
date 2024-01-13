using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

public class BetterDayNightManager : MonoBehaviour, ITimeOfDaySystem
{
	public enum WeatherType
	{
		None,
		Raining,
		All
	}

	private class ScheduledEvent
	{
		public long lastDayCalled;

		public int hour;

		public Action action;
	}

	public static volatile BetterDayNightManager instance;

	public Shader standard;

	public Shader standardCutout;

	public Shader gorillaUnlit;

	public Shader gorillaUnlitCutout;

	public Material[] standardMaterialsUnlit;

	public Material[] standardMaterialsUnlitDarker;

	public Material[] dayNightSupportedMaterials;

	public Material[] dayNightSupportedMaterialsCutout;

	public Texture2D[] dayNightLightmaps;

	public Texture2D[] dayNightWeatherLightmaps;

	public Texture2D[] dayNightSkyboxTextures;

	public Texture2D[] cloudsDayNightSkyboxTextures;

	public Texture2D[] beachDayNightSkyboxTextures;

	public Texture2D[] dayNightWeatherSkyboxTextures;

	public float[] standardUnlitColor;

	public float[] standardUnlitColorWithPremadeColorDarker;

	public float currentLerp;

	public float currentTimestep;

	public double[] timeOfDayRange;

	public double timeMultiplier;

	private float lastTime;

	private double currentTime;

	private double totalHours;

	private double totalSeconds;

	private float colorFrom;

	private float colorTo;

	private float colorFromDarker;

	private float colorToDarker;

	public int currentTimeIndex;

	public int currentWeatherIndex;

	private int lastIndex;

	private double currentIndexSeconds;

	private float tempLerp;

	private double baseSeconds;

	private bool computerInit;

	private float h;

	private float s;

	private float v;

	public int mySeed;

	public System.Random randomNumberGenerator = new System.Random();

	public WeatherType[] weatherCycle;

	public float rainChance = 0.3f;

	public int maxRainDuration = 5;

	private int rainDuration;

	private float remainingSeconds;

	private long initialDayCycles;

	private long gameEpochDay;

	private int currentWeatherCycle;

	private int fromWeatherIndex;

	private int toWeatherIndex;

	private Texture2D fromMap;

	private Texture2D fromSky;

	private Texture2D fromSky2;

	private Texture2D fromSky3;

	private Texture2D toMap;

	private Texture2D toSky;

	private Texture2D toSky2;

	private Texture2D toSky3;

	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	public int overrideIndex = -1;

	private static readonly Dictionary<int, ScheduledEvent> scheduledEvents = new Dictionary<int, ScheduledEvent>(256);

	public TimeSettings currentSetting;

	public string currentTimeOfDay { get; private set; }

	double ITimeOfDaySystem.currentTimeInSeconds => currentTime;

	double ITimeOfDaySystem.totalTimeInSeconds => totalSeconds;

	private void OnEnable()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		currentLerp = 0f;
		totalHours = 0.0;
		for (int i = 0; i < timeOfDayRange.Length; i++)
		{
			totalHours += timeOfDayRange[i];
		}
		totalSeconds = totalHours * 60.0 * 60.0;
		currentTimeIndex = 0;
		baseSeconds = 0.0;
		computerInit = false;
		randomNumberGenerator = new System.Random(mySeed);
		GenerateWeatherEventTimes();
		ChangeMaps(0, 1);
		StartCoroutine(UpdateTimeOfDay());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	protected void OnDestroy()
	{
	}

	private IEnumerator UpdateTimeOfDay()
	{
		yield return 0;
		while (true)
		{
			try
			{
				if (!computerInit && GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
				{
					computerInit = true;
					initialDayCycles = (long)(TimeSpan.FromMilliseconds(GorillaComputer.instance.startupMillis).TotalSeconds * timeMultiplier / totalSeconds);
					currentWeatherIndex = (int)(initialDayCycles * dayNightLightmaps.Length) % weatherCycle.Length;
					baseSeconds = TimeSpan.FromMilliseconds(GorillaComputer.instance.startupMillis).TotalSeconds * timeMultiplier % totalSeconds;
					currentTime = (baseSeconds + (double)Time.realtimeSinceStartup * timeMultiplier) % totalSeconds;
					currentIndexSeconds = 0.0;
					for (int i = 0; i < timeOfDayRange.Length; i++)
					{
						currentIndexSeconds += timeOfDayRange[i] * 3600.0;
						if (currentIndexSeconds > currentTime)
						{
							currentTimeIndex = i;
							break;
						}
					}
					currentWeatherIndex += currentTimeIndex;
				}
				else if (!computerInit && baseSeconds == 0.0)
				{
					initialDayCycles = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * timeMultiplier / totalSeconds);
					currentWeatherIndex = (int)(initialDayCycles * dayNightLightmaps.Length) % weatherCycle.Length;
					baseSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * timeMultiplier % totalSeconds;
					currentTime = baseSeconds % totalSeconds;
					currentIndexSeconds = 0.0;
					for (int j = 0; j < timeOfDayRange.Length; j++)
					{
						currentIndexSeconds += timeOfDayRange[j] * 3600.0;
						if (currentIndexSeconds > currentTime)
						{
							currentTimeIndex = j;
							break;
						}
					}
					currentWeatherIndex += currentTimeIndex - 1;
					if (currentWeatherIndex < 0)
					{
						currentWeatherIndex = weatherCycle.Length - 1;
					}
				}
				currentTime = (baseSeconds + (double)Time.realtimeSinceStartup * timeMultiplier) % totalSeconds;
				currentIndexSeconds = 0.0;
				for (int k = 0; k < timeOfDayRange.Length; k++)
				{
					currentIndexSeconds += timeOfDayRange[k] * 3600.0;
					if (currentIndexSeconds > currentTime)
					{
						currentTimeIndex = k;
						break;
					}
				}
				if (currentTimeIndex != lastIndex)
				{
					currentWeatherIndex = (currentWeatherIndex + 1) % weatherCycle.Length;
					ChangeMaps(currentTimeIndex, (currentTimeIndex + 1) % dayNightLightmaps.Length);
				}
				currentLerp = (float)(1.0 - (currentIndexSeconds - currentTime) / (timeOfDayRange[currentTimeIndex] * 3600.0));
				ChangeLerps(currentLerp);
				lastIndex = currentTimeIndex;
				currentTimeOfDay = dayNightLightmaps[currentTimeIndex].name;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error in BetterDayNightManager: " + ex, this);
			}
			gameEpochDay = (long)((baseSeconds + (double)Time.realtimeSinceStartup * timeMultiplier) / totalSeconds + (double)initialDayCycles);
			foreach (ScheduledEvent value in scheduledEvents.Values)
			{
				if (value.lastDayCalled != gameEpochDay && value.hour == currentTimeIndex)
				{
					value.lastDayCalled = gameEpochDay;
					value.action();
				}
			}
			yield return new WaitForSeconds(currentTimestep);
		}
	}

	private void ChangeLerps(float newLerp)
	{
		Shader.SetGlobalFloat("_GlobalDayNightLerpValue", newLerp);
		for (int i = 0; i < standardMaterialsUnlit.Length; i++)
		{
			tempLerp = Mathf.Lerp(colorFrom, colorTo, newLerp);
			standardMaterialsUnlit[i].color = new Color(tempLerp, tempLerp, tempLerp);
		}
		for (int j = 0; j < standardMaterialsUnlitDarker.Length; j++)
		{
			tempLerp = Mathf.Lerp(colorFromDarker, colorToDarker, newLerp);
			Color.RGBToHSV(standardMaterialsUnlitDarker[j].color, out h, out s, out v);
			standardMaterialsUnlitDarker[j].color = Color.HSVToRGB(h, s, tempLerp);
		}
	}

	private void ChangeMaps(int fromIndex, int toIndex)
	{
		fromWeatherIndex = currentWeatherIndex;
		toWeatherIndex = (currentWeatherIndex + 1) % weatherCycle.Length;
		if (weatherCycle[fromWeatherIndex] == WeatherType.Raining)
		{
			fromMap = dayNightWeatherLightmaps[fromIndex];
			fromSky = dayNightWeatherSkyboxTextures[fromIndex];
		}
		else
		{
			fromMap = dayNightLightmaps[fromIndex];
			fromSky = dayNightSkyboxTextures[fromIndex];
		}
		fromSky2 = cloudsDayNightSkyboxTextures[fromIndex];
		fromSky3 = beachDayNightSkyboxTextures[fromIndex];
		if (weatherCycle[toWeatherIndex] == WeatherType.Raining)
		{
			toMap = dayNightWeatherLightmaps[toIndex];
			toSky = dayNightWeatherSkyboxTextures[toIndex];
		}
		else
		{
			toMap = dayNightLightmaps[toIndex];
			toSky = dayNightSkyboxTextures[toIndex];
		}
		toSky2 = cloudsDayNightSkyboxTextures[toIndex];
		toSky3 = beachDayNightSkyboxTextures[toIndex];
		Shader.SetGlobalTexture("_GlobalDayNightLightmap1", fromMap);
		Shader.SetGlobalTexture("_GlobalDayNightLightmap2", toMap);
		Shader.SetGlobalTexture("_GlobalDayNightSkyTex1", fromSky);
		Shader.SetGlobalTexture("_GlobalDayNightSkyTex2", toSky);
		Shader.SetGlobalTexture("_GlobalDayNightSky2Tex1", fromSky2);
		Shader.SetGlobalTexture("_GlobalDayNightSky2Tex2", toSky2);
		Shader.SetGlobalTexture("_GlobalDayNightSky3Tex1", fromSky3);
		Shader.SetGlobalTexture("_GlobalDayNightSky3Tex2", toSky3);
		colorFrom = standardUnlitColor[fromIndex];
		colorTo = standardUnlitColor[toIndex];
		colorFromDarker = standardUnlitColorWithPremadeColorDarker[fromIndex];
		colorToDarker = standardUnlitColorWithPremadeColorDarker[toIndex];
	}

	public WeatherType CurrentWeather()
	{
		return weatherCycle[currentWeatherIndex];
	}

	public WeatherType NextWeather()
	{
		return weatherCycle[(currentWeatherIndex + 1) % weatherCycle.Length];
	}

	public WeatherType LastWeather()
	{
		return weatherCycle[(currentWeatherIndex - 1) % weatherCycle.Length];
	}

	private void GenerateWeatherEventTimes()
	{
		weatherCycle = new WeatherType[100 * dayNightLightmaps.Length];
		rainChance = rainChance * 2f / (float)maxRainDuration;
		for (int i = 1; i < weatherCycle.Length; i++)
		{
			weatherCycle[i] = (((float)randomNumberGenerator.Next(100) < rainChance * 100f) ? WeatherType.Raining : WeatherType.None);
			if (weatherCycle[i] != WeatherType.Raining)
			{
				continue;
			}
			rainDuration = randomNumberGenerator.Next(1, maxRainDuration + 1);
			for (int j = 1; j < rainDuration; j++)
			{
				if (i + j < weatherCycle.Length)
				{
					weatherCycle[i + j] = WeatherType.Raining;
				}
			}
			i += rainDuration - 1;
		}
	}

	public static int RegisterScheduledEvent(int hour, Action action)
	{
		int i;
		for (i = (int)(DateTime.Now.Ticks % int.MaxValue); scheduledEvents.ContainsKey(i); i++)
		{
		}
		scheduledEvents.Add(i, new ScheduledEvent
		{
			lastDayCalled = -1L,
			hour = hour,
			action = action
		});
		return i;
	}

	public static void UnregisterScheduledEvent(int id)
	{
		scheduledEvents.Remove(id);
	}

	public void SetOverrideIndex(int index)
	{
		overrideIndex = index;
		currentWeatherIndex = overrideIndex;
		currentTimeIndex = overrideIndex;
		currentTimeOfDay = dayNightLightmaps[currentTimeIndex].name;
		ChangeMaps(currentTimeIndex, (currentTimeIndex + 1) % dayNightLightmaps.Length);
	}
}
