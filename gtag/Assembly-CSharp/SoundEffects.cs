using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class SoundEffects : MonoBehaviour
{
	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000800 RID: 2048 RVA: 0x0003289C File Offset: 0x00030A9C
	public bool isPlaying
	{
		get
		{
			return this._lastClipIndex >= 0 && this._lastClipLength >= 0.0 && this._lastClipElapsedTime < this._lastClipLength;
		}
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x000328CF File Offset: 0x00030ACF
	public void Clear()
	{
		this.audioClips.Clear();
		this._lastClipIndex = -1;
		this._lastClipLength = -1.0;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x000328F2 File Offset: 0x00030AF2
	public void Stop()
	{
		if (this.source)
		{
			this.source.Stop();
		}
		this._lastClipLength = -1.0;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0003291C File Offset: 0x00030B1C
	public void PlayNext(float delayMin, float delayMax, float volMin, float volMax)
	{
		float delay = this._rnd.NextFloat(delayMin, delayMax);
		float volume = this._rnd.NextFloat(volMin, volMax);
		this.PlayNext(delay, volume);
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x00032950 File Offset: 0x00030B50
	public void PlayNext(float delay = 0f, float volume = 1f)
	{
		if (!this.source)
		{
			return;
		}
		if (this.audioClips == null || this.audioClips.Count == 0)
		{
			return;
		}
		if (this.source.isPlaying)
		{
			this.source.Stop();
		}
		int num = this._rnd.NextInt(this.audioClips.Count);
		while (this.distinct && this._lastClipIndex == num)
		{
			num = this._rnd.NextInt(this.audioClips.Count);
		}
		AudioClip audioClip = this.audioClips[num];
		this._lastClipIndex = num;
		this._lastClipLength = (double)audioClip.length;
		float num2 = delay;
		if (num2 < this._minDelay)
		{
			num2 = this._minDelay;
		}
		if (num2 < 0.0001f)
		{
			this.source.PlayOneShot(audioClip, volume);
			this._lastClipElapsedTime = 0f;
			return;
		}
		this.source.clip = audioClip;
		this.source.volume = volume;
		this.source.PlayDelayed(num2);
		this._lastClipElapsedTime = -num2;
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x00032A64 File Offset: 0x00030C64
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(this.seed))
		{
			this.seed = "0x1337C0D3";
		}
		this._rnd = new SRand(this.seed);
		if (this.audioClips == null)
		{
			this.audioClips = new List<AudioClip>();
		}
	}

	// Token: 0x040009A7 RID: 2471
	public AudioSource source;

	// Token: 0x040009A8 RID: 2472
	[Space]
	public List<AudioClip> audioClips = new List<AudioClip>();

	// Token: 0x040009A9 RID: 2473
	public string seed = "0x1337C0D3";

	// Token: 0x040009AA RID: 2474
	[Space]
	public bool distinct = true;

	// Token: 0x040009AB RID: 2475
	[SerializeField]
	private float _minDelay;

	// Token: 0x040009AC RID: 2476
	[Space]
	[SerializeField]
	private SRand _rnd;

	// Token: 0x040009AD RID: 2477
	[NonSerialized]
	private int _lastClipIndex = -1;

	// Token: 0x040009AE RID: 2478
	[NonSerialized]
	private double _lastClipLength = -1.0;

	// Token: 0x040009AF RID: 2479
	[NonSerialized]
	private TimeSince _lastClipElapsedTime;
}
