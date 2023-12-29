﻿using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundBankPlayer : MonoBehaviour
{
	public bool isPlaying
	{
		get
		{
			return Time.time < this.playEndTime;
		}
	}

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

	protected void OnEnable()
	{
		if (this.playOnEnable)
		{
			this.Play(null, null);
		}
	}

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

	[Tooltip("Optional. AudioSource Settings will be used if this is not defined.")]
	public AudioSource audioSource;

	public bool playOnEnable = true;

	public bool shuffleOrder = true;

	public SoundBankSO soundBank;

	public AudioMixerGroup outputAudioMixerGroup;

	public bool spatialize;

	public bool spatializePostEffects;

	public bool bypassEffects;

	public bool bypassListenerEffects;

	public bool bypassReverbZones;

	public int priority = 128;

	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	public float reverbZoneMix = 1f;

	public float dopplerLevel = 1f;

	public float spread;

	public AudioRolloffMode rolloffMode;

	public float minDistance = 1f;

	public float maxDistance = 100f;

	public AnimationCurve customRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	private int nextIndex;

	private float playEndTime;

	private SoundBankPlayer.PlaylistEntry[] playlist;

	private struct PlaylistEntry
	{
		public int index;

		public float volume;

		public float pitch;
	}
}
