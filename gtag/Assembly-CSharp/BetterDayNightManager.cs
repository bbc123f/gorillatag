using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

public class BetterDayNightManager : MonoBehaviour, ITimeOfDaySystem
{
	public static void Register(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Add(data);
	}

	public static void Unregister(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Remove(data);
	}

	public string currentTimeOfDay
	{
		[CompilerGenerated]
		get
		{
			return this.<currentTimeOfDay>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<currentTimeOfDay>k__BackingField = value;
		}
	}

	double ITimeOfDaySystem.currentTimeInSeconds
	{
		get
		{
			return this.currentTime;
		}
	}

	double ITimeOfDaySystem.totalTimeInSeconds
	{
		get
		{
			return this.totalSeconds;
		}
	}

	private void OnEnable()
	{
		if (BetterDayNightManager.instance == null)
		{
			BetterDayNightManager.instance = this;
		}
		else if (BetterDayNightManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.currentLerp = 0f;
		this.totalHours = 0.0;
		for (int i = 0; i < this.timeOfDayRange.Length; i++)
		{
			this.totalHours += this.timeOfDayRange[i];
		}
		this.totalSeconds = this.totalHours * 60.0 * 60.0;
		this.currentTimeIndex = 0;
		this.baseSeconds = 0.0;
		this.computerInit = false;
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateWeatherEventTimes();
		this.ChangeMaps(0, 1);
		base.StartCoroutine(this.UpdateTimeOfDay());
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	protected void OnDestroy()
	{
	}

	private Vector4 MaterialColorCorrection(Vector4 color)
	{
		if (color.x < 0.5f)
		{
			color.x += 3E-08f;
		}
		if (color.y < 0.5f)
		{
			color.y += 3E-08f;
		}
		if (color.z < 0.5f)
		{
			color.z += 3E-08f;
		}
		if (color.w < 0.5f)
		{
			color.w += 3E-08f;
		}
		return color;
	}

	private IEnumerator UpdateTimeOfDay()
	{
		yield return 0;
		for (;;)
		{
			if (this.animatingLightFlash != null)
			{
				yield return new WaitForSeconds(this.currentTimestep);
			}
			else
			{
				try
				{
					if (!this.computerInit && GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
					{
						this.computerInit = true;
						this.initialDayCycles = (long)(TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier / this.totalSeconds);
						this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
						this.baseSeconds = TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier % this.totalSeconds;
						this.currentTime = (this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds;
						this.currentIndexSeconds = 0.0;
						for (int i = 0; i < this.timeOfDayRange.Length; i++)
						{
							this.currentIndexSeconds += this.timeOfDayRange[i] * 3600.0;
							if (this.currentIndexSeconds > this.currentTime)
							{
								this.currentTimeIndex = i;
								break;
							}
						}
						this.currentWeatherIndex += this.currentTimeIndex;
					}
					else if (!this.computerInit && this.baseSeconds == 0.0)
					{
						this.initialDayCycles = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier / this.totalSeconds);
						this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
						this.baseSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier % this.totalSeconds;
						this.currentTime = this.baseSeconds % this.totalSeconds;
						this.currentIndexSeconds = 0.0;
						for (int j = 0; j < this.timeOfDayRange.Length; j++)
						{
							this.currentIndexSeconds += this.timeOfDayRange[j] * 3600.0;
							if (this.currentIndexSeconds > this.currentTime)
							{
								this.currentTimeIndex = j;
								break;
							}
						}
						this.currentWeatherIndex += this.currentTimeIndex - 1;
						if (this.currentWeatherIndex < 0)
						{
							this.currentWeatherIndex = this.weatherCycle.Length - 1;
						}
					}
					this.currentTime = ((this.currentSetting == TimeSettings.Normal) ? ((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds) : this.currentTime);
					this.currentIndexSeconds = 0.0;
					for (int k = 0; k < this.timeOfDayRange.Length; k++)
					{
						this.currentIndexSeconds += this.timeOfDayRange[k] * 3600.0;
						if (this.currentIndexSeconds > this.currentTime)
						{
							this.currentTimeIndex = k;
							break;
						}
					}
					if (this.currentTimeIndex != this.lastIndex)
					{
						this.currentWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
						this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
					}
					this.currentLerp = (float)(1.0 - (this.currentIndexSeconds - this.currentTime) / (this.timeOfDayRange[this.currentTimeIndex] * 3600.0));
					this.ChangeLerps(this.currentLerp);
					this.lastIndex = this.currentTimeIndex;
					this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
				}
				catch (Exception ex)
				{
					string str = "Error in BetterDayNightManager: ";
					Exception ex2 = ex;
					Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null), this);
				}
				this.gameEpochDay = (long)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) / this.totalSeconds + (double)this.initialDayCycles);
				foreach (BetterDayNightManager.ScheduledEvent scheduledEvent in BetterDayNightManager.scheduledEvents.Values)
				{
					if (scheduledEvent.lastDayCalled != this.gameEpochDay && scheduledEvent.hour == this.currentTimeIndex)
					{
						scheduledEvent.lastDayCalled = this.gameEpochDay;
						scheduledEvent.action();
					}
				}
				yield return new WaitForSeconds(this.currentTimestep);
			}
		}
		yield break;
	}

	private void ChangeLerps(float newLerp)
	{
		Shader.SetGlobalFloat(this._GlobalDayNightLerpValue, newLerp);
		for (int i = 0; i < this.standardMaterialsUnlit.Length; i++)
		{
			this.tempLerp = Mathf.Lerp(this.colorFrom, this.colorTo, newLerp);
			this.standardMaterialsUnlit[i].color = new Color(this.tempLerp, this.tempLerp, this.tempLerp);
		}
		for (int j = 0; j < this.standardMaterialsUnlitDarker.Length; j++)
		{
			this.tempLerp = Mathf.Lerp(this.colorFromDarker, this.colorToDarker, newLerp);
			Color.RGBToHSV(this.standardMaterialsUnlitDarker[j].color, out this.h, out this.s, out this.v);
			this.standardMaterialsUnlitDarker[j].color = Color.HSVToRGB(this.h, this.s, this.tempLerp);
		}
	}

	private void ChangeMaps(int fromIndex, int toIndex)
	{
		this.fromWeatherIndex = this.currentWeatherIndex;
		this.toWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.fromSky = this.dayNightWeatherSkyboxTextures[fromIndex];
		}
		else
		{
			this.fromSky = this.dayNightSkyboxTextures[fromIndex];
		}
		this.fromSky2 = this.cloudsDayNightSkyboxTextures[fromIndex];
		this.fromSky3 = this.beachDayNightSkyboxTextures[fromIndex];
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.toSky = this.dayNightWeatherSkyboxTextures[toIndex];
		}
		else
		{
			this.toSky = this.dayNightSkyboxTextures[toIndex];
		}
		this.toSky2 = this.cloudsDayNightSkyboxTextures[toIndex];
		this.toSky3 = this.beachDayNightSkyboxTextures[toIndex];
		this.PopulateAllLightmaps(fromIndex, toIndex);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex1, this.fromSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex2, this.toSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex1, this.fromSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex2, this.toSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex1, this.fromSky3);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex2, this.toSky3);
		this.colorFrom = this.standardUnlitColor[fromIndex];
		this.colorTo = this.standardUnlitColor[toIndex];
		this.colorFromDarker = this.standardUnlitColorWithPremadeColorDarker[fromIndex];
		this.colorToDarker = this.standardUnlitColorWithPremadeColorDarker[toIndex];
	}

	private void Update()
	{
		if (!this.shouldRepopulate)
		{
			using (List<PerSceneRenderData>.Enumerator enumerator = BetterDayNightManager.allScenesRenderData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CheckShouldRepopulate())
					{
						this.shouldRepopulate = true;
					}
				}
			}
		}
		if (this.shouldRepopulate)
		{
			this.PopulateAllLightmaps();
			this.shouldRepopulate = false;
		}
	}

	public void RequestRepopulateLightmaps()
	{
		this.shouldRepopulate = true;
	}

	public void PopulateAllLightmaps()
	{
		this.PopulateAllLightmaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	public void PopulateAllLightmaps(int fromIndex, int toIndex)
	{
		string fromTimeOfDay;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			fromTimeOfDay = this.dayNightWeatherLightmapNames[fromIndex];
		}
		else
		{
			fromTimeOfDay = this.dayNightLightmapNames[fromIndex];
		}
		string toTimeOfDay;
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			toTimeOfDay = this.dayNightWeatherLightmapNames[toIndex];
		}
		else
		{
			toTimeOfDay = this.dayNightLightmapNames[toIndex];
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		foreach (PerSceneRenderData perSceneRenderData in BetterDayNightManager.allScenesRenderData)
		{
			perSceneRenderData.PopulateLightmaps(fromTimeOfDay, toTimeOfDay, lightmaps);
		}
		LightmapSettings.lightmaps = lightmaps;
	}

	public BetterDayNightManager.WeatherType CurrentWeather()
	{
		return this.weatherCycle[this.currentWeatherIndex];
	}

	public BetterDayNightManager.WeatherType NextWeather()
	{
		return this.weatherCycle[(this.currentWeatherIndex + 1) % this.weatherCycle.Length];
	}

	public BetterDayNightManager.WeatherType LastWeather()
	{
		return this.weatherCycle[(this.currentWeatherIndex - 1) % this.weatherCycle.Length];
	}

	private void GenerateWeatherEventTimes()
	{
		this.weatherCycle = new BetterDayNightManager.WeatherType[100 * this.dayNightLightmapNames.Length];
		this.rainChance = this.rainChance * 2f / (float)this.maxRainDuration;
		for (int i = 1; i < this.weatherCycle.Length; i++)
		{
			this.weatherCycle[i] = (((float)this.randomNumberGenerator.Next(100) < this.rainChance * 100f) ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None);
			if (this.weatherCycle[i] == BetterDayNightManager.WeatherType.Raining)
			{
				this.rainDuration = this.randomNumberGenerator.Next(1, this.maxRainDuration + 1);
				for (int j = 1; j < this.rainDuration; j++)
				{
					if (i + j < this.weatherCycle.Length)
					{
						this.weatherCycle[i + j] = BetterDayNightManager.WeatherType.Raining;
					}
				}
				i += this.rainDuration - 1;
			}
		}
	}

	public static int RegisterScheduledEvent(int hour, Action action)
	{
		int num = (int)(DateTime.Now.Ticks % 2147483647L);
		while (BetterDayNightManager.scheduledEvents.ContainsKey(num))
		{
			num++;
		}
		BetterDayNightManager.scheduledEvents.Add(num, new BetterDayNightManager.ScheduledEvent
		{
			lastDayCalled = -1L,
			hour = hour,
			action = action
		});
		return num;
	}

	public static void UnregisterScheduledEvent(int id)
	{
		BetterDayNightManager.scheduledEvents.Remove(id);
	}

	public void SetOverrideIndex(int index)
	{
		this.overrideIndex = index;
		this.currentWeatherIndex = this.overrideIndex;
		this.currentTimeIndex = this.overrideIndex;
		this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	public void AnimateLightFlash(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		if (this.animatingLightFlash != null)
		{
			base.StopCoroutine(this.animatingLightFlash);
		}
		this.animatingLightFlash = base.StartCoroutine(this.AnimateLightFlashCo(index, fadeInDuration, holdDuration, fadeOutDuration));
	}

	private IEnumerator AnimateLightFlashCo(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		int startMap = (this.currentLerp < 0.5f) ? this.currentTimeIndex : ((this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeMaps(startMap, index);
		float endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(index, index);
		this.ChangeLerps(0f);
		endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			yield return null;
		}
		this.ChangeMaps(index, startMap);
		endTimestamp = Time.time + fadeOutDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeLerps(this.currentLerp);
		this.animatingLightFlash = null;
		yield break;
	}

	public void SetTimeOfDay(int timeIndex)
	{
		double num = 0.0;
		for (int i = 0; i < timeIndex; i++)
		{
			num += this.timeOfDayRange[i];
		}
		this.currentTime = num * 3600.0;
		this.currentSetting = TimeSettings.Static;
	}

	public void FastForward(float seconds)
	{
		this.baseSeconds += (double)seconds;
	}

	public BetterDayNightManager()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static BetterDayNightManager()
	{
	}

	[OnEnterPlay_SetNull]
	public static volatile BetterDayNightManager instance;

	[OnEnterPlay_Clear]
	public static List<PerSceneRenderData> allScenesRenderData = new List<PerSceneRenderData>();

	public Shader standard;

	public Shader standardCutout;

	public Shader gorillaUnlit;

	public Shader gorillaUnlitCutout;

	public Material[] standardMaterialsUnlit;

	public Material[] standardMaterialsUnlitDarker;

	public Material[] dayNightSupportedMaterials;

	public Material[] dayNightSupportedMaterialsCutout;

	public string[] dayNightLightmapNames;

	public string[] dayNightWeatherLightmapNames;

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

	public Random randomNumberGenerator = new Random();

	public BetterDayNightManager.WeatherType[] weatherCycle;

	[CompilerGenerated]
	private string <currentTimeOfDay>k__BackingField;

	public float rainChance = 0.3f;

	public int maxRainDuration = 5;

	private int rainDuration;

	private float remainingSeconds;

	private long initialDayCycles;

	private long gameEpochDay;

	private int currentWeatherCycle;

	private int fromWeatherIndex;

	private int toWeatherIndex;

	private Texture2D fromSky;

	private Texture2D fromSky2;

	private Texture2D fromSky3;

	private Texture2D toSky;

	private Texture2D toSky2;

	private Texture2D toSky3;

	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	public int overrideIndex = -1;

	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, BetterDayNightManager.ScheduledEvent> scheduledEvents = new Dictionary<int, BetterDayNightManager.ScheduledEvent>(256);

	public TimeSettings currentSetting;

	private ShaderHashId _Color = "_Color";

	private ShaderHashId _GlobalDayNightLerpValue = "_GlobalDayNightLerpValue";

	private ShaderHashId _GlobalDayNightSkyTex1 = "_GlobalDayNightSkyTex1";

	private ShaderHashId _GlobalDayNightSkyTex2 = "_GlobalDayNightSkyTex2";

	private ShaderHashId _GlobalDayNightSky2Tex1 = "_GlobalDayNightSky2Tex1";

	private ShaderHashId _GlobalDayNightSky2Tex2 = "_GlobalDayNightSky2Tex2";

	private ShaderHashId _GlobalDayNightSky3Tex1 = "_GlobalDayNightSky3Tex1";

	private ShaderHashId _GlobalDayNightSky3Tex2 = "_GlobalDayNightSky3Tex2";

	private bool shouldRepopulate;

	private Coroutine animatingLightFlash;

	public enum WeatherType
	{
		None,
		Raining,
		All
	}

	private class ScheduledEvent
	{
		public ScheduledEvent()
		{
		}

		public long lastDayCalled;

		public int hour;

		public Action action;
	}

	[CompilerGenerated]
	private sealed class <AnimateLightFlashCo>d__104 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <AnimateLightFlashCo>d__104(int <>1__state)
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
			BetterDayNightManager betterDayNightManager = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				startMap = ((betterDayNightManager.currentLerp < 0.5f) ? betterDayNightManager.currentTimeIndex : ((betterDayNightManager.currentTimeIndex + 1) % betterDayNightManager.timeOfDayRange.Length));
				betterDayNightManager.ChangeMaps(startMap, index);
				endTimestamp = Time.time + fadeInDuration;
				break;
			case 1:
				this.<>1__state = -1;
				break;
			case 2:
				this.<>1__state = -1;
				goto IL_10A;
			case 3:
				this.<>1__state = -1;
				goto IL_173;
			default:
				return false;
			}
			if (Time.time < endTimestamp)
			{
				betterDayNightManager.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
				this.<>2__current = null;
				this.<>1__state = 1;
				return true;
			}
			betterDayNightManager.ChangeMaps(index, index);
			betterDayNightManager.ChangeLerps(0f);
			endTimestamp = Time.time + fadeInDuration;
			IL_10A:
			if (Time.time < endTimestamp)
			{
				this.<>2__current = null;
				this.<>1__state = 2;
				return true;
			}
			betterDayNightManager.ChangeMaps(index, startMap);
			endTimestamp = Time.time + fadeOutDuration;
			IL_173:
			if (Time.time >= endTimestamp)
			{
				betterDayNightManager.ChangeMaps(betterDayNightManager.currentTimeIndex, (betterDayNightManager.currentTimeIndex + 1) % betterDayNightManager.timeOfDayRange.Length);
				betterDayNightManager.ChangeLerps(betterDayNightManager.currentLerp);
				betterDayNightManager.animatingLightFlash = null;
				return false;
			}
			betterDayNightManager.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			this.<>2__current = null;
			this.<>1__state = 3;
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

		public BetterDayNightManager <>4__this;

		public int index;

		public float fadeInDuration;

		public float fadeOutDuration;

		private int <startMap>5__2;

		private float <endTimestamp>5__3;
	}

	[CompilerGenerated]
	private sealed class <UpdateTimeOfDay>d__79 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateTimeOfDay>d__79(int <>1__state)
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
			BetterDayNightManager betterDayNightManager = this;
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
			case 3:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			if (betterDayNightManager.animatingLightFlash != null)
			{
				this.<>2__current = new WaitForSeconds(betterDayNightManager.currentTimestep);
				this.<>1__state = 2;
				return true;
			}
			try
			{
				if (!betterDayNightManager.computerInit && GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
				{
					betterDayNightManager.computerInit = true;
					betterDayNightManager.initialDayCycles = (long)(TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * betterDayNightManager.timeMultiplier / betterDayNightManager.totalSeconds);
					betterDayNightManager.currentWeatherIndex = (int)(betterDayNightManager.initialDayCycles * (long)betterDayNightManager.dayNightLightmapNames.Length) % betterDayNightManager.weatherCycle.Length;
					betterDayNightManager.baseSeconds = TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * betterDayNightManager.timeMultiplier % betterDayNightManager.totalSeconds;
					betterDayNightManager.currentTime = (betterDayNightManager.baseSeconds + (double)Time.realtimeSinceStartup * betterDayNightManager.timeMultiplier) % betterDayNightManager.totalSeconds;
					betterDayNightManager.currentIndexSeconds = 0.0;
					for (int i = 0; i < betterDayNightManager.timeOfDayRange.Length; i++)
					{
						betterDayNightManager.currentIndexSeconds += betterDayNightManager.timeOfDayRange[i] * 3600.0;
						if (betterDayNightManager.currentIndexSeconds > betterDayNightManager.currentTime)
						{
							betterDayNightManager.currentTimeIndex = i;
							break;
						}
					}
					betterDayNightManager.currentWeatherIndex += betterDayNightManager.currentTimeIndex;
				}
				else if (!betterDayNightManager.computerInit && betterDayNightManager.baseSeconds == 0.0)
				{
					betterDayNightManager.initialDayCycles = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * betterDayNightManager.timeMultiplier / betterDayNightManager.totalSeconds);
					betterDayNightManager.currentWeatherIndex = (int)(betterDayNightManager.initialDayCycles * (long)betterDayNightManager.dayNightLightmapNames.Length) % betterDayNightManager.weatherCycle.Length;
					betterDayNightManager.baseSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * betterDayNightManager.timeMultiplier % betterDayNightManager.totalSeconds;
					betterDayNightManager.currentTime = betterDayNightManager.baseSeconds % betterDayNightManager.totalSeconds;
					betterDayNightManager.currentIndexSeconds = 0.0;
					for (int j = 0; j < betterDayNightManager.timeOfDayRange.Length; j++)
					{
						betterDayNightManager.currentIndexSeconds += betterDayNightManager.timeOfDayRange[j] * 3600.0;
						if (betterDayNightManager.currentIndexSeconds > betterDayNightManager.currentTime)
						{
							betterDayNightManager.currentTimeIndex = j;
							break;
						}
					}
					betterDayNightManager.currentWeatherIndex += betterDayNightManager.currentTimeIndex - 1;
					if (betterDayNightManager.currentWeatherIndex < 0)
					{
						betterDayNightManager.currentWeatherIndex = betterDayNightManager.weatherCycle.Length - 1;
					}
				}
				betterDayNightManager.currentTime = ((betterDayNightManager.currentSetting == TimeSettings.Normal) ? ((betterDayNightManager.baseSeconds + (double)Time.realtimeSinceStartup * betterDayNightManager.timeMultiplier) % betterDayNightManager.totalSeconds) : betterDayNightManager.currentTime);
				betterDayNightManager.currentIndexSeconds = 0.0;
				for (int k = 0; k < betterDayNightManager.timeOfDayRange.Length; k++)
				{
					betterDayNightManager.currentIndexSeconds += betterDayNightManager.timeOfDayRange[k] * 3600.0;
					if (betterDayNightManager.currentIndexSeconds > betterDayNightManager.currentTime)
					{
						betterDayNightManager.currentTimeIndex = k;
						break;
					}
				}
				if (betterDayNightManager.currentTimeIndex != betterDayNightManager.lastIndex)
				{
					betterDayNightManager.currentWeatherIndex = (betterDayNightManager.currentWeatherIndex + 1) % betterDayNightManager.weatherCycle.Length;
					betterDayNightManager.ChangeMaps(betterDayNightManager.currentTimeIndex, (betterDayNightManager.currentTimeIndex + 1) % betterDayNightManager.timeOfDayRange.Length);
				}
				betterDayNightManager.currentLerp = (float)(1.0 - (betterDayNightManager.currentIndexSeconds - betterDayNightManager.currentTime) / (betterDayNightManager.timeOfDayRange[betterDayNightManager.currentTimeIndex] * 3600.0));
				betterDayNightManager.ChangeLerps(betterDayNightManager.currentLerp);
				betterDayNightManager.lastIndex = betterDayNightManager.currentTimeIndex;
				betterDayNightManager.currentTimeOfDay = betterDayNightManager.dayNightLightmapNames[betterDayNightManager.currentTimeIndex];
			}
			catch (Exception ex)
			{
				string str = "Error in BetterDayNightManager: ";
				Exception ex2 = ex;
				Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null), betterDayNightManager);
			}
			betterDayNightManager.gameEpochDay = (long)((betterDayNightManager.baseSeconds + (double)Time.realtimeSinceStartup * betterDayNightManager.timeMultiplier) / betterDayNightManager.totalSeconds + (double)betterDayNightManager.initialDayCycles);
			foreach (BetterDayNightManager.ScheduledEvent scheduledEvent in BetterDayNightManager.scheduledEvents.Values)
			{
				if (scheduledEvent.lastDayCalled != betterDayNightManager.gameEpochDay && scheduledEvent.hour == betterDayNightManager.currentTimeIndex)
				{
					scheduledEvent.lastDayCalled = betterDayNightManager.gameEpochDay;
					scheduledEvent.action();
				}
			}
			this.<>2__current = new WaitForSeconds(betterDayNightManager.currentTimestep);
			this.<>1__state = 3;
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

		public BetterDayNightManager <>4__this;
	}
}
