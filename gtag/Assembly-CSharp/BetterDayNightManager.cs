using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class BetterDayNightManager : MonoBehaviour, ITimeOfDaySystem
{
	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000AE2 RID: 2786 RVA: 0x000434E1 File Offset: 0x000416E1
	// (set) Token: 0x06000AE3 RID: 2787 RVA: 0x000434E9 File Offset: 0x000416E9
	public string currentTimeOfDay { get; private set; }

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000AE4 RID: 2788 RVA: 0x000434F2 File Offset: 0x000416F2
	double ITimeOfDaySystem.currentTimeInSeconds
	{
		get
		{
			return this.currentTime;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x000434FA File Offset: 0x000416FA
	double ITimeOfDaySystem.totalTimeInSeconds
	{
		get
		{
			return this.totalSeconds;
		}
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00043504 File Offset: 0x00041704
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

	// Token: 0x06000AE7 RID: 2791 RVA: 0x000435EF File Offset: 0x000417EF
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x000435F7 File Offset: 0x000417F7
	protected void OnDestroy()
	{
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x000435FC File Offset: 0x000417FC
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

	// Token: 0x06000AEA RID: 2794 RVA: 0x0004367E File Offset: 0x0004187E
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
						this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmaps.Length) % this.weatherCycle.Length;
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
						this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmaps.Length) % this.weatherCycle.Length;
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
					this.currentTimeOfDay = this.dayNightLightmaps[this.currentTimeIndex].name;
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

	// Token: 0x06000AEB RID: 2795 RVA: 0x00043690 File Offset: 0x00041890
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

	// Token: 0x06000AEC RID: 2796 RVA: 0x00043770 File Offset: 0x00041970
	private void ChangeMaps(int fromIndex, int toIndex)
	{
		this.fromWeatherIndex = this.currentWeatherIndex;
		this.toWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.fromMap = this.dayNightWeatherLightmaps[fromIndex];
			this.fromSky = this.dayNightWeatherSkyboxTextures[fromIndex];
		}
		else
		{
			this.fromMap = this.dayNightLightmaps[fromIndex];
			this.fromSky = this.dayNightSkyboxTextures[fromIndex];
		}
		this.fromSky2 = this.cloudsDayNightSkyboxTextures[fromIndex];
		this.fromSky3 = this.beachDayNightSkyboxTextures[fromIndex];
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.toMap = this.dayNightWeatherLightmaps[toIndex];
			this.toSky = this.dayNightWeatherSkyboxTextures[toIndex];
		}
		else
		{
			this.toMap = this.dayNightLightmaps[toIndex];
			this.toSky = this.dayNightSkyboxTextures[toIndex];
		}
		this.toSky2 = this.cloudsDayNightSkyboxTextures[toIndex];
		this.toSky3 = this.beachDayNightSkyboxTextures[toIndex];
		Shader.SetGlobalTexture(this._GlobalDayNightLightmap1, this.fromMap);
		Shader.SetGlobalTexture(this._GlobalDayNightLightmap2, this.toMap);
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

	// Token: 0x06000AED RID: 2797 RVA: 0x00043954 File Offset: 0x00041B54
	public BetterDayNightManager.WeatherType CurrentWeather()
	{
		return this.weatherCycle[this.currentWeatherIndex];
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x00043963 File Offset: 0x00041B63
	public BetterDayNightManager.WeatherType NextWeather()
	{
		return this.weatherCycle[(this.currentWeatherIndex + 1) % this.weatherCycle.Length];
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0004397D File Offset: 0x00041B7D
	public BetterDayNightManager.WeatherType LastWeather()
	{
		return this.weatherCycle[(this.currentWeatherIndex - 1) % this.weatherCycle.Length];
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x00043998 File Offset: 0x00041B98
	private void GenerateWeatherEventTimes()
	{
		this.weatherCycle = new BetterDayNightManager.WeatherType[100 * this.dayNightLightmaps.Length];
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

	// Token: 0x06000AF1 RID: 2801 RVA: 0x00043A70 File Offset: 0x00041C70
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

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00043ACD File Offset: 0x00041CCD
	public static void UnregisterScheduledEvent(int id)
	{
		BetterDayNightManager.scheduledEvents.Remove(id);
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x00043ADC File Offset: 0x00041CDC
	public void SetOverrideIndex(int index)
	{
		this.overrideIndex = index;
		this.currentWeatherIndex = this.overrideIndex;
		this.currentTimeIndex = this.overrideIndex;
		this.currentTimeOfDay = this.dayNightLightmaps[this.currentTimeIndex].name;
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00043B3D File Offset: 0x00041D3D
	public void AnimateLightFlash(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		if (this.animatingLightFlash != null)
		{
			base.StopCoroutine(this.animatingLightFlash);
		}
		this.animatingLightFlash = base.StartCoroutine(this.AnimateLightFlashCo(index, fadeInDuration, holdDuration, fadeOutDuration));
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00043B6A File Offset: 0x00041D6A
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

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00043B90 File Offset: 0x00041D90
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

	// Token: 0x04000DB2 RID: 3506
	public static volatile BetterDayNightManager instance;

	// Token: 0x04000DB3 RID: 3507
	public Shader standard;

	// Token: 0x04000DB4 RID: 3508
	public Shader standardCutout;

	// Token: 0x04000DB5 RID: 3509
	public Shader gorillaUnlit;

	// Token: 0x04000DB6 RID: 3510
	public Shader gorillaUnlitCutout;

	// Token: 0x04000DB7 RID: 3511
	public Material[] standardMaterialsUnlit;

	// Token: 0x04000DB8 RID: 3512
	public Material[] standardMaterialsUnlitDarker;

	// Token: 0x04000DB9 RID: 3513
	public Material[] dayNightSupportedMaterials;

	// Token: 0x04000DBA RID: 3514
	public Material[] dayNightSupportedMaterialsCutout;

	// Token: 0x04000DBB RID: 3515
	public Texture2D[] dayNightLightmaps;

	// Token: 0x04000DBC RID: 3516
	public Texture2D[] dayNightWeatherLightmaps;

	// Token: 0x04000DBD RID: 3517
	public Texture2D[] dayNightSkyboxTextures;

	// Token: 0x04000DBE RID: 3518
	public Texture2D[] cloudsDayNightSkyboxTextures;

	// Token: 0x04000DBF RID: 3519
	public Texture2D[] beachDayNightSkyboxTextures;

	// Token: 0x04000DC0 RID: 3520
	public Texture2D[] dayNightWeatherSkyboxTextures;

	// Token: 0x04000DC1 RID: 3521
	public float[] standardUnlitColor;

	// Token: 0x04000DC2 RID: 3522
	public float[] standardUnlitColorWithPremadeColorDarker;

	// Token: 0x04000DC3 RID: 3523
	public float currentLerp;

	// Token: 0x04000DC4 RID: 3524
	public float currentTimestep;

	// Token: 0x04000DC5 RID: 3525
	public double[] timeOfDayRange;

	// Token: 0x04000DC6 RID: 3526
	public double timeMultiplier;

	// Token: 0x04000DC7 RID: 3527
	private float lastTime;

	// Token: 0x04000DC8 RID: 3528
	private double currentTime;

	// Token: 0x04000DC9 RID: 3529
	private double totalHours;

	// Token: 0x04000DCA RID: 3530
	private double totalSeconds;

	// Token: 0x04000DCB RID: 3531
	private float colorFrom;

	// Token: 0x04000DCC RID: 3532
	private float colorTo;

	// Token: 0x04000DCD RID: 3533
	private float colorFromDarker;

	// Token: 0x04000DCE RID: 3534
	private float colorToDarker;

	// Token: 0x04000DCF RID: 3535
	public int currentTimeIndex;

	// Token: 0x04000DD0 RID: 3536
	public int currentWeatherIndex;

	// Token: 0x04000DD1 RID: 3537
	private int lastIndex;

	// Token: 0x04000DD2 RID: 3538
	private double currentIndexSeconds;

	// Token: 0x04000DD3 RID: 3539
	private float tempLerp;

	// Token: 0x04000DD4 RID: 3540
	private double baseSeconds;

	// Token: 0x04000DD5 RID: 3541
	private bool computerInit;

	// Token: 0x04000DD6 RID: 3542
	private float h;

	// Token: 0x04000DD7 RID: 3543
	private float s;

	// Token: 0x04000DD8 RID: 3544
	private float v;

	// Token: 0x04000DD9 RID: 3545
	public int mySeed;

	// Token: 0x04000DDA RID: 3546
	public Random randomNumberGenerator = new Random();

	// Token: 0x04000DDB RID: 3547
	public BetterDayNightManager.WeatherType[] weatherCycle;

	// Token: 0x04000DDD RID: 3549
	public float rainChance = 0.3f;

	// Token: 0x04000DDE RID: 3550
	public int maxRainDuration = 5;

	// Token: 0x04000DDF RID: 3551
	private int rainDuration;

	// Token: 0x04000DE0 RID: 3552
	private float remainingSeconds;

	// Token: 0x04000DE1 RID: 3553
	private long initialDayCycles;

	// Token: 0x04000DE2 RID: 3554
	private long gameEpochDay;

	// Token: 0x04000DE3 RID: 3555
	private int currentWeatherCycle;

	// Token: 0x04000DE4 RID: 3556
	private int fromWeatherIndex;

	// Token: 0x04000DE5 RID: 3557
	private int toWeatherIndex;

	// Token: 0x04000DE6 RID: 3558
	private Texture2D fromMap;

	// Token: 0x04000DE7 RID: 3559
	private Texture2D fromSky;

	// Token: 0x04000DE8 RID: 3560
	private Texture2D fromSky2;

	// Token: 0x04000DE9 RID: 3561
	private Texture2D fromSky3;

	// Token: 0x04000DEA RID: 3562
	private Texture2D toMap;

	// Token: 0x04000DEB RID: 3563
	private Texture2D toSky;

	// Token: 0x04000DEC RID: 3564
	private Texture2D toSky2;

	// Token: 0x04000DED RID: 3565
	private Texture2D toSky3;

	// Token: 0x04000DEE RID: 3566
	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	// Token: 0x04000DEF RID: 3567
	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	// Token: 0x04000DF0 RID: 3568
	public int overrideIndex = -1;

	// Token: 0x04000DF1 RID: 3569
	private static readonly Dictionary<int, BetterDayNightManager.ScheduledEvent> scheduledEvents = new Dictionary<int, BetterDayNightManager.ScheduledEvent>(256);

	// Token: 0x04000DF2 RID: 3570
	public TimeSettings currentSetting;

	// Token: 0x04000DF3 RID: 3571
	private ShaderHashId _Color = "_Color";

	// Token: 0x04000DF4 RID: 3572
	private ShaderHashId _GlobalDayNightLerpValue = "_GlobalDayNightLerpValue";

	// Token: 0x04000DF5 RID: 3573
	private ShaderHashId _GlobalDayNightLightmap1 = "_GlobalDayNightLightmap1";

	// Token: 0x04000DF6 RID: 3574
	private ShaderHashId _GlobalDayNightLightmap2 = "_GlobalDayNightLightmap2";

	// Token: 0x04000DF7 RID: 3575
	private ShaderHashId _GlobalDayNightSkyTex1 = "_GlobalDayNightSkyTex1";

	// Token: 0x04000DF8 RID: 3576
	private ShaderHashId _GlobalDayNightSkyTex2 = "_GlobalDayNightSkyTex2";

	// Token: 0x04000DF9 RID: 3577
	private ShaderHashId _GlobalDayNightSky2Tex1 = "_GlobalDayNightSky2Tex1";

	// Token: 0x04000DFA RID: 3578
	private ShaderHashId _GlobalDayNightSky2Tex2 = "_GlobalDayNightSky2Tex2";

	// Token: 0x04000DFB RID: 3579
	private ShaderHashId _GlobalDayNightSky3Tex1 = "_GlobalDayNightSky3Tex1";

	// Token: 0x04000DFC RID: 3580
	private ShaderHashId _GlobalDayNightSky3Tex2 = "_GlobalDayNightSky3Tex2";

	// Token: 0x04000DFD RID: 3581
	private Coroutine animatingLightFlash;

	// Token: 0x0200044B RID: 1099
	public enum WeatherType
	{
		// Token: 0x04001DC7 RID: 7623
		None,
		// Token: 0x04001DC8 RID: 7624
		Raining,
		// Token: 0x04001DC9 RID: 7625
		All
	}

	// Token: 0x0200044C RID: 1100
	private class ScheduledEvent
	{
		// Token: 0x04001DCA RID: 7626
		public long lastDayCalled;

		// Token: 0x04001DCB RID: 7627
		public int hour;

		// Token: 0x04001DCC RID: 7628
		public Action action;
	}
}
