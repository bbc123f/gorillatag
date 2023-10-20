using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200021E RID: 542
public class SoundBankPlayer : MonoBehaviour
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000D74 RID: 3444 RVA: 0x0004E65B File Offset: 0x0004C85B
	public bool isPlaying
	{
		get
		{
			return Time.time < this.playEndTime;
		}
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x0004E66C File Offset: 0x0004C86C
	protected void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
			this.audioSource.outputAudioMixerGroup = this.outputAudioMixerGroup;
			this.audioSource.spatialize = this.spatialize;
			this.audioSource.spatializePostEffects = this.spatializePostEffects;
			this.audioSource.bypassEffects = this.bypassEffects;
			this.audioSource.bypassListenerEffects = this.bypassListenerEffects;
			this.audioSource.bypassReverbZones = this.bypassReverbZones;
			this.audioSource.priority = this.priority;
			this.audioSource.spatialBlend = this.spatialBlend;
			this.audioSource.dopplerLevel = this.dopplerLevel;
			this.audioSource.spread = this.spread;
			this.audioSource.rolloffMode = this.rolloffMode;
			this.audioSource.minDistance = this.minDistance;
			this.audioSource.maxDistance = this.maxDistance;
			this.audioSource.reverbZoneMix = this.reverbZoneMix;
		}
		this.audioSource.volume = 1f;
		this.audioSource.playOnAwake = false;
		if (this.shuffleOrder)
		{
			int[] array = new int[this.soundBank.sounds.Length / 2];
			this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
			for (int i = 0; i < this.playlist.Length; i++)
			{
				int num = 0;
				for (int j = 0; j < 100; j++)
				{
					num = Random.Range(0, this.soundBank.sounds.Length);
					if (Array.IndexOf<int>(array, num) == -1)
					{
						break;
					}
				}
				if (array.Length != 0)
				{
					array[i % array.Length] = num;
				}
				this.playlist[i] = new SoundBankPlayer.PlaylistEntry
				{
					index = num,
					volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
					pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
				};
			}
			return;
		}
		this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
		for (int k = 0; k < this.playlist.Length; k++)
		{
			this.playlist[k] = new SoundBankPlayer.PlaylistEntry
			{
				index = k % this.soundBank.sounds.Length,
				volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
				pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
			};
		}
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x0004E968 File Offset: 0x0004CB68
	protected void OnEnable()
	{
		if (this.playOnEnable)
		{
			this.Play(null, null);
		}
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x0004E998 File Offset: 0x0004CB98
	public void Play(float? volumeOverride = null, float? pitchOverride = null)
	{
		if (this.soundBank.sounds.Length == 0)
		{
			return;
		}
		SoundBankPlayer.PlaylistEntry playlistEntry = this.playlist[this.nextIndex];
		this.audioSource.pitch = ((pitchOverride != null) ? pitchOverride.Value : playlistEntry.pitch);
		AudioClip audioClip = this.soundBank.sounds[playlistEntry.index];
		this.audioSource.PlayOneShot(audioClip, (volumeOverride != null) ? volumeOverride.Value : playlistEntry.volume);
		this.playEndTime = Mathf.Max(this.playEndTime, Time.time + audioClip.length);
		this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
	}

	// Token: 0x0400108D RID: 4237
	[Tooltip("Optional. AudioSource Settings will be used if this is not defined.")]
	public AudioSource audioSource;

	// Token: 0x0400108E RID: 4238
	public bool playOnEnable = true;

	// Token: 0x0400108F RID: 4239
	public bool shuffleOrder = true;

	// Token: 0x04001090 RID: 4240
	public SoundBankSO soundBank;

	// Token: 0x04001091 RID: 4241
	public AudioMixerGroup outputAudioMixerGroup;

	// Token: 0x04001092 RID: 4242
	public bool spatialize;

	// Token: 0x04001093 RID: 4243
	public bool spatializePostEffects;

	// Token: 0x04001094 RID: 4244
	public bool bypassEffects;

	// Token: 0x04001095 RID: 4245
	public bool bypassListenerEffects;

	// Token: 0x04001096 RID: 4246
	public bool bypassReverbZones;

	// Token: 0x04001097 RID: 4247
	public int priority = 128;

	// Token: 0x04001098 RID: 4248
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x04001099 RID: 4249
	public float reverbZoneMix = 1f;

	// Token: 0x0400109A RID: 4250
	public float dopplerLevel = 1f;

	// Token: 0x0400109B RID: 4251
	public float spread;

	// Token: 0x0400109C RID: 4252
	public AudioRolloffMode rolloffMode;

	// Token: 0x0400109D RID: 4253
	public float minDistance = 1f;

	// Token: 0x0400109E RID: 4254
	public float maxDistance = 100f;

	// Token: 0x0400109F RID: 4255
	public AnimationCurve customRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x040010A0 RID: 4256
	private int nextIndex;

	// Token: 0x040010A1 RID: 4257
	private float playEndTime;

	// Token: 0x040010A2 RID: 4258
	private SoundBankPlayer.PlaylistEntry[] playlist;

	// Token: 0x0200047C RID: 1148
	private struct PlaylistEntry
	{
		// Token: 0x04001EB3 RID: 7859
		public int index;

		// Token: 0x04001EB4 RID: 7860
		public float volume;

		// Token: 0x04001EB5 RID: 7861
		public float pitch;
	}
}
