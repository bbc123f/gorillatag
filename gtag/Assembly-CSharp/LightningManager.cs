using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class LightningManager : MonoBehaviour
{
	// Token: 0x06000C45 RID: 3141 RVA: 0x0004A7DC File Offset: 0x000489DC
	private void Start()
	{
		this.lightningAudio = base.GetComponent<AudioSource>();
		GorillaComputer instance = GorillaComputer.instance;
		instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x0004A812 File Offset: 0x00048A12
	private void OnTimeChanged()
	{
		this.InitializeRng();
		if (this.lightningRunner != null)
		{
			base.StopCoroutine(this.lightningRunner);
		}
		this.lightningRunner = base.StartCoroutine(this.LightningEffectRunner());
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x0004A840 File Offset: 0x00048A40
	private void GetHourStart(out long seed, out float timestampRealtime)
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime d = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, 0, 0);
		timestampRealtime = Time.realtimeSinceStartup - (float)(serverTime - d).TotalSeconds;
		seed = d.Ticks;
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x0004A8A0 File Offset: 0x00048AA0
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

	// Token: 0x06000C49 RID: 3145 RVA: 0x0004A964 File Offset: 0x00048B64
	private void DoLightningStrike()
	{
		BetterDayNightManager.instance.AnimateLightFlash(this.lightMapIndex, this.flashFadeInDuration, this.flashHoldDuration, this.flashFadeOutDuration);
		this.lightningAudio.clip = (ZoneManagement.IsInZone(GTZone.cave) ? this.muffledLightning : this.regularLightning);
		this.lightningAudio.Play();
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x0004A9C1 File Offset: 0x00048BC1
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

	// Token: 0x04000FA3 RID: 4003
	public int lightMapIndex;

	// Token: 0x04000FA4 RID: 4004
	public float minTimeBetweenFlashes;

	// Token: 0x04000FA5 RID: 4005
	public float maxTimeBetweenFlashes;

	// Token: 0x04000FA6 RID: 4006
	public float flashFadeInDuration;

	// Token: 0x04000FA7 RID: 4007
	public float flashHoldDuration;

	// Token: 0x04000FA8 RID: 4008
	public float flashFadeOutDuration;

	// Token: 0x04000FA9 RID: 4009
	private AudioSource lightningAudio;

	// Token: 0x04000FAA RID: 4010
	private SRand rng;

	// Token: 0x04000FAB RID: 4011
	private long currentHourlySeed;

	// Token: 0x04000FAC RID: 4012
	private List<float> lightningTimestampsRealtime = new List<float>();

	// Token: 0x04000FAD RID: 4013
	private int nextLightningTimestampIndex;

	// Token: 0x04000FAE RID: 4014
	public AudioClip regularLightning;

	// Token: 0x04000FAF RID: 4015
	public AudioClip muffledLightning;

	// Token: 0x04000FB0 RID: 4016
	private Coroutine lightningRunner;
}
