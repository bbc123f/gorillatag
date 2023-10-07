using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class BetterDayNightManager : MonoBehaviour, ITimeOfDaySystem
{
	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000ADD RID: 2781 RVA: 0x000433A9 File Offset: 0x000415A9
	// (set) Token: 0x06000ADE RID: 2782 RVA: 0x000433B1 File Offset: 0x000415B1
	public string currentTimeOfDay { get; private set; }

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000ADF RID: 2783 RVA: 0x000433BA File Offset: 0x000415BA
	double ITimeOfDaySystem.currentTimeInSeconds
	{
		get
		{
			return this.currentTime;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x000433C2 File Offset: 0x000415C2
	double ITimeOfDaySystem.totalTimeInSeconds
	{
		get
		{
			return this.totalSeconds;
		}
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x000433CC File Offset: 0x000415CC
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

	// Token: 0x06000AE2 RID: 2786 RVA: 0x000434B7 File Offset: 0x000416B7
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x000434BF File Offset: 0x000416BF
	protected void OnDestroy()
	{
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x000434C4 File Offset: 0x000416C4
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

	// Token: 0x06000AE5 RID: 2789 RVA: 0x00043546 File Offset: 0x00041746
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

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00043558 File Offset: 0x00041758
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

	// Token: 0x06000AE7 RID: 2791 RVA: 0x00043638 File Offset: 0x00041838
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

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0004381C File Offset: 0x00041A1C
	public BetterDayNightManager.WeatherType CurrentWeather()
	{
		return this.weatherCycle[this.currentWeatherIndex];
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0004382B File Offset: 0x00041A2B
	public BetterDayNightManager.WeatherType NextWeather()
	{
		return this.weatherCycle[(this.currentWeatherIndex + 1) % this.weatherCycle.Length];
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x00043845 File Offset: 0x00041A45
	public BetterDayNightManager.WeatherType LastWeather()
	{
		return this.weatherCycle[(this.currentWeatherIndex - 1) % this.weatherCycle.Length];
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00043860 File Offset: 0x00041A60
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

	// Token: 0x06000AEC RID: 2796 RVA: 0x00043938 File Offset: 0x00041B38
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

	// Token: 0x06000AED RID: 2797 RVA: 0x00043995 File Offset: 0x00041B95
	public static void UnregisterScheduledEvent(int id)
	{
		BetterDayNightManager.scheduledEvents.Remove(id);
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x000439A4 File Offset: 0x00041BA4
	public void SetOverrideIndex(int index)
	{
		this.overrideIndex = index;
		this.currentWeatherIndex = this.overrideIndex;
		this.currentTimeIndex = this.overrideIndex;
		this.currentTimeOfDay = this.dayNightLightmaps[this.currentTimeIndex].name;
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x00043A05 File Offset: 0x00041C05
	public void AnimateLightFlash(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		if (this.animatingLightFlash != null)
		{
			base.StopCoroutine(this.animatingLightFlash);
		}
		this.animatingLightFlash = base.StartCoroutine(this.AnimateLightFlashCo(index, fadeInDuration, holdDuration, fadeOutDuration));
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x00043A32 File Offset: 0x00041C32
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

	// Token: 0x06000AF1 RID: 2801 RVA: 0x00043A58 File Offset: 0x00041C58
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

	// Token: 0x04000DAE RID: 3502
	public static volatile BetterDayNightManager instance;

	// Token: 0x04000DAF RID: 3503
	public Shader standard;

	// Token: 0x04000DB0 RID: 3504
	public Shader standardCutout;

	// Token: 0x04000DB1 RID: 3505
	public Shader gorillaUnlit;

	// Token: 0x04000DB2 RID: 3506
	public Shader gorillaUnlitCutout;

	// Token: 0x04000DB3 RID: 3507
	public Material[] standardMaterialsUnlit;

	// Token: 0x04000DB4 RID: 3508
	public Material[] standardMaterialsUnlitDarker;

	// Token: 0x04000DB5 RID: 3509
	public Material[] dayNightSupportedMaterials;

	// Token: 0x04000DB6 RID: 3510
	public Material[] dayNightSupportedMaterialsCutout;

	// Token: 0x04000DB7 RID: 3511
	public Texture2D[] dayNightLightmaps;

	// Token: 0x04000DB8 RID: 3512
	public Texture2D[] dayNightWeatherLightmaps;

	// Token: 0x04000DB9 RID: 3513
	public Texture2D[] dayNightSkyboxTextures;

	// Token: 0x04000DBA RID: 3514
	public Texture2D[] cloudsDayNightSkyboxTextures;

	// Token: 0x04000DBB RID: 3515
	public Texture2D[] beachDayNightSkyboxTextures;

	// Token: 0x04000DBC RID: 3516
	public Texture2D[] dayNightWeatherSkyboxTextures;

	// Token: 0x04000DBD RID: 3517
	public float[] standardUnlitColor;

	// Token: 0x04000DBE RID: 3518
	public float[] standardUnlitColorWithPremadeColorDarker;

	// Token: 0x04000DBF RID: 3519
	public float currentLerp;

	// Token: 0x04000DC0 RID: 3520
	public float currentTimestep;

	// Token: 0x04000DC1 RID: 3521
	public double[] timeOfDayRange;

	// Token: 0x04000DC2 RID: 3522
	public double timeMultiplier;

	// Token: 0x04000DC3 RID: 3523
	private float lastTime;

	// Token: 0x04000DC4 RID: 3524
	private double currentTime;

	// Token: 0x04000DC5 RID: 3525
	private double totalHours;

	// Token: 0x04000DC6 RID: 3526
	private double totalSeconds;

	// Token: 0x04000DC7 RID: 3527
	private float colorFrom;

	// Token: 0x04000DC8 RID: 3528
	private float colorTo;

	// Token: 0x04000DC9 RID: 3529
	private float colorFromDarker;

	// Token: 0x04000DCA RID: 3530
	private float colorToDarker;

	// Token: 0x04000DCB RID: 3531
	public int currentTimeIndex;

	// Token: 0x04000DCC RID: 3532
	public int currentWeatherIndex;

	// Token: 0x04000DCD RID: 3533
	private int lastIndex;

	// Token: 0x04000DCE RID: 3534
	private double currentIndexSeconds;

	// Token: 0x04000DCF RID: 3535
	private float tempLerp;

	// Token: 0x04000DD0 RID: 3536
	private double baseSeconds;

	// Token: 0x04000DD1 RID: 3537
	private bool computerInit;

	// Token: 0x04000DD2 RID: 3538
	private float h;

	// Token: 0x04000DD3 RID: 3539
	private float s;

	// Token: 0x04000DD4 RID: 3540
	private float v;

	// Token: 0x04000DD5 RID: 3541
	public int mySeed;

	// Token: 0x04000DD6 RID: 3542
	public Random randomNumberGenerator = new Random();

	// Token: 0x04000DD7 RID: 3543
	public BetterDayNightManager.WeatherType[] weatherCycle;

	// Token: 0x04000DD9 RID: 3545
	public float rainChance = 0.3f;

	// Token: 0x04000DDA RID: 3546
	public int maxRainDuration = 5;

	// Token: 0x04000DDB RID: 3547
	private int rainDuration;

	// Token: 0x04000DDC RID: 3548
	private float remainingSeconds;

	// Token: 0x04000DDD RID: 3549
	private long initialDayCycles;

	// Token: 0x04000DDE RID: 3550
	private long gameEpochDay;

	// Token: 0x04000DDF RID: 3551
	private int currentWeatherCycle;

	// Token: 0x04000DE0 RID: 3552
	private int fromWeatherIndex;

	// Token: 0x04000DE1 RID: 3553
	private int toWeatherIndex;

	// Token: 0x04000DE2 RID: 3554
	private Texture2D fromMap;

	// Token: 0x04000DE3 RID: 3555
	private Texture2D fromSky;

	// Token: 0x04000DE4 RID: 3556
	private Texture2D fromSky2;

	// Token: 0x04000DE5 RID: 3557
	private Texture2D fromSky3;

	// Token: 0x04000DE6 RID: 3558
	private Texture2D toMap;

	// Token: 0x04000DE7 RID: 3559
	private Texture2D toSky;

	// Token: 0x04000DE8 RID: 3560
	private Texture2D toSky2;

	// Token: 0x04000DE9 RID: 3561
	private Texture2D toSky3;

	// Token: 0x04000DEA RID: 3562
	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	// Token: 0x04000DEB RID: 3563
	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	// Token: 0x04000DEC RID: 3564
	public int overrideIndex = -1;

	// Token: 0x04000DED RID: 3565
	private static readonly Dictionary<int, BetterDayNightManager.ScheduledEvent> scheduledEvents = new Dictionary<int, BetterDayNightManager.ScheduledEvent>(256);

	// Token: 0x04000DEE RID: 3566
	public TimeSettings currentSetting;

	// Token: 0x04000DEF RID: 3567
	private ShaderHashId _Color = "_Color";

	// Token: 0x04000DF0 RID: 3568
	private ShaderHashId _GlobalDayNightLerpValue = "_GlobalDayNightLerpValue";

	// Token: 0x04000DF1 RID: 3569
	private ShaderHashId _GlobalDayNightLightmap1 = "_GlobalDayNightLightmap1";

	// Token: 0x04000DF2 RID: 3570
	private ShaderHashId _GlobalDayNightLightmap2 = "_GlobalDayNightLightmap2";

	// Token: 0x04000DF3 RID: 3571
	private ShaderHashId _GlobalDayNightSkyTex1 = "_GlobalDayNightSkyTex1";

	// Token: 0x04000DF4 RID: 3572
	private ShaderHashId _GlobalDayNightSkyTex2 = "_GlobalDayNightSkyTex2";

	// Token: 0x04000DF5 RID: 3573
	private ShaderHashId _GlobalDayNightSky2Tex1 = "_GlobalDayNightSky2Tex1";

	// Token: 0x04000DF6 RID: 3574
	private ShaderHashId _GlobalDayNightSky2Tex2 = "_GlobalDayNightSky2Tex2";

	// Token: 0x04000DF7 RID: 3575
	private ShaderHashId _GlobalDayNightSky3Tex1 = "_GlobalDayNightSky3Tex1";

	// Token: 0x04000DF8 RID: 3576
	private ShaderHashId _GlobalDayNightSky3Tex2 = "_GlobalDayNightSky3Tex2";

	// Token: 0x04000DF9 RID: 3577
	private Coroutine animatingLightFlash;

	// Token: 0x02000449 RID: 1097
	public enum WeatherType
	{
		// Token: 0x04001DBA RID: 7610
		None,
		// Token: 0x04001DBB RID: 7611
		Raining,
		// Token: 0x04001DBC RID: 7612
		All
	}

	// Token: 0x0200044A RID: 1098
	private class ScheduledEvent
	{
		// Token: 0x04001DBD RID: 7613
		public long lastDayCalled;

		// Token: 0x04001DBE RID: 7614
		public int hour;

		// Token: 0x04001DBF RID: 7615
		public Action action;
	}
}
