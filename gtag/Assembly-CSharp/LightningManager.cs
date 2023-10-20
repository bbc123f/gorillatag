using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class LightningManager : MonoBehaviour
{
	// Token: 0x06000C4B RID: 3147 RVA: 0x0004AA44 File Offset: 0x00048C44
	private void Start()
	{
		this.lightningAudio = base.GetComponent<AudioSource>();
		GorillaComputer instance = GorillaComputer.instance;
		instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x0004AA7A File Offset: 0x00048C7A
	private void OnTimeChanged()
	{
		this.InitializeRng();
		if (this.lightningRunner != null)
		{
			base.StopCoroutine(this.lightningRunner);
		}
		this.lightningRunner = base.StartCoroutine(this.LightningEffectRunner());
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x0004AAA8 File Offset: 0x00048CA8
	private void GetHourStart(out long seed, out float timestampRealtime)
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime d = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, 0, 0);
		timestampRealtime = Time.realtimeSinceStartup - (float)(serverTime - d).TotalSeconds;
		seed = d.Ticks;
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x0004AB08 File Offset: 0x00048D08
	private void InitializeRng()
	{
		long seed;
		float num;
		this.GetHourStart(out seed, out num);
		this.currentHourlySeed = seed;
		this.rng = new SRand(seed);
		this.lightningTimestampsRealtime.Clear();
		this.nextLightningTimestampIndex = -1;
		float num2 = num;
		float num3 = 0f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (num3 < 3600f)
		{
			float num4 = this.rng.NextFloat(this.minTimeBetweenFlashes, this.maxTimeBetweenFlashes);
			num3 += num4;
			num2 += num4;
			if (this.nextLightningTimestampIndex == -1 && num2 > realtimeSinceStartup)
			{
				this.nextLightningTimestampIndex = this.lightningTimestampsRealtime.Count;
			}
			this.lightningTimestampsRealtime.Add(num2);
		}
		this.lightningTimestampsRealtime[this.lightningTimestampsRealtime.Count - 1] = num + 3605f;
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x0004ABCC File Offset: 0x00048DCC
	private void DoLightningStrike()
	{
		BetterDayNightManager.instance.AnimateLightFlash(this.lightMapIndex, this.flashFadeInDuration, this.flashHoldDuration, this.flashFadeOutDuration);
		this.lightningAudio.clip = (ZoneManagement.IsInZone(GTZone.cave) ? this.muffledLightning : this.regularLightning);
		this.lightningAudio.Play();
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x0004AC29 File Offset: 0x00048E29
	private IEnumerator LightningEffectRunner()
	{
		for (;;)
		{
			if (this.lightningTimestampsRealtime.Count <= this.nextLightningTimestampIndex)
			{
				this.InitializeRng();
			}
			if (this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
			{
				yield return new WaitForSecondsRealtime(this.lightningTimestampsRealtime[this.nextLightningTimestampIndex] - Time.realtimeSinceStartup);
				float num = this.lightningTimestampsRealtime[this.nextLightningTimestampIndex];
				this.nextLightningTimestampIndex++;
				if (Time.realtimeSinceStartup - num < 1f && this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
				{
					this.DoLightningStrike();
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000FA7 RID: 4007
	public int lightMapIndex;

	// Token: 0x04000FA8 RID: 4008
	public float minTimeBetweenFlashes;

	// Token: 0x04000FA9 RID: 4009
	public float maxTimeBetweenFlashes;

	// Token: 0x04000FAA RID: 4010
	public float flashFadeInDuration;

	// Token: 0x04000FAB RID: 4011
	public float flashHoldDuration;

	// Token: 0x04000FAC RID: 4012
	public float flashFadeOutDuration;

	// Token: 0x04000FAD RID: 4013
	private AudioSource lightningAudio;

	// Token: 0x04000FAE RID: 4014
	private SRand rng;

	// Token: 0x04000FAF RID: 4015
	private long currentHourlySeed;

	// Token: 0x04000FB0 RID: 4016
	private List<float> lightningTimestampsRealtime = new List<float>();

	// Token: 0x04000FB1 RID: 4017
	private int nextLightningTimestampIndex;

	// Token: 0x04000FB2 RID: 4018
	public AudioClip regularLightning;

	// Token: 0x04000FB3 RID: 4019
	public AudioClip muffledLightning;

	// Token: 0x04000FB4 RID: 4020
	private Coroutine lightningRunner;
}
