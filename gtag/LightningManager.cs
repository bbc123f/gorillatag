using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

public class LightningManager : MonoBehaviour
{
	private void Start()
	{
		this.lightningAudio = base.GetComponent<AudioSource>();
		GorillaComputer instance = GorillaComputer.instance;
		instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
	}

	private void OnTimeChanged()
	{
		this.InitializeRng();
		if (this.lightningRunner != null)
		{
			base.StopCoroutine(this.lightningRunner);
		}
		this.lightningRunner = base.StartCoroutine(this.LightningEffectRunner());
	}

	private void GetHourStart(out long seed, out float timestampRealtime)
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, 0, 0);
		timestampRealtime = Time.realtimeSinceStartup - (float)(serverTime - dateTime).TotalSeconds;
		seed = dateTime.Ticks;
	}

	private void InitializeRng()
	{
		long num;
		float num2;
		this.GetHourStart(out num, out num2);
		this.currentHourlySeed = num;
		this.rng = new SRand(num);
		this.lightningTimestampsRealtime.Clear();
		this.nextLightningTimestampIndex = -1;
		float num3 = num2;
		float num4 = 0f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (num4 < 3600f)
		{
			float num5 = this.rng.NextFloat(this.minTimeBetweenFlashes, this.maxTimeBetweenFlashes);
			num4 += num5;
			num3 += num5;
			if (this.nextLightningTimestampIndex == -1 && num3 > realtimeSinceStartup)
			{
				this.nextLightningTimestampIndex = this.lightningTimestampsRealtime.Count;
			}
			this.lightningTimestampsRealtime.Add(num3);
		}
		this.lightningTimestampsRealtime[this.lightningTimestampsRealtime.Count - 1] = num2 + 3605f;
	}

	private void DoLightningStrike()
	{
		BetterDayNightManager.instance.AnimateLightFlash(this.lightMapIndex, this.flashFadeInDuration, this.flashHoldDuration, this.flashFadeOutDuration);
		this.lightningAudio.clip = (ZoneManagement.IsInZone(GTZone.cave) ? this.muffledLightning : this.regularLightning);
		this.lightningAudio.Play();
	}

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

	public LightningManager()
	{
	}

	public int lightMapIndex;

	public float minTimeBetweenFlashes;

	public float maxTimeBetweenFlashes;

	public float flashFadeInDuration;

	public float flashHoldDuration;

	public float flashFadeOutDuration;

	private AudioSource lightningAudio;

	private SRand rng;

	private long currentHourlySeed;

	private List<float> lightningTimestampsRealtime = new List<float>();

	private int nextLightningTimestampIndex;

	public AudioClip regularLightning;

	public AudioClip muffledLightning;

	private Coroutine lightningRunner;

	[CompilerGenerated]
	private sealed class <LightningEffectRunner>d__19 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <LightningEffectRunner>d__19(int <>1__state)
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
			LightningManager lightningManager = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				break;
			case 1:
			{
				this.<>1__state = -1;
				float num2 = lightningManager.lightningTimestampsRealtime[lightningManager.nextLightningTimestampIndex];
				lightningManager.nextLightningTimestampIndex++;
				if (Time.realtimeSinceStartup - num2 < 1f && lightningManager.lightningTimestampsRealtime.Count > lightningManager.nextLightningTimestampIndex)
				{
					lightningManager.DoLightningStrike();
					goto IL_CE;
				}
				goto IL_CE;
			}
			case 2:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			if (lightningManager.lightningTimestampsRealtime.Count <= lightningManager.nextLightningTimestampIndex)
			{
				lightningManager.InitializeRng();
			}
			if (lightningManager.lightningTimestampsRealtime.Count > lightningManager.nextLightningTimestampIndex)
			{
				this.<>2__current = new WaitForSecondsRealtime(lightningManager.lightningTimestampsRealtime[lightningManager.nextLightningTimestampIndex] - Time.realtimeSinceStartup);
				this.<>1__state = 1;
				return true;
			}
			IL_CE:
			this.<>2__current = null;
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

		public LightningManager <>4__this;
	}
}
