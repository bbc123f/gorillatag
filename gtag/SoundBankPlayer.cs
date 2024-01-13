using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundBankPlayer : MonoBehaviour
{
	private struct PlaylistEntry
	{
		public int index;

		public float volume;

		public float pitch;
	}

	[Tooltip("Optional. AudioSource Settings will be used if this is not defined.")]
	public AudioSource audioSource;

	public bool playOnEnable = true;

	public bool shuffleOrder = true;

	public SoundBankSO soundBank;

	public AudioMixerGroup outputAudioMixerGroup;

	public bool spatialize = true;

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

	private PlaylistEntry[] playlist;

	public bool isPlaying => Time.time < playEndTime;

	protected void Awake()
	{
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = outputAudioMixerGroup;
			audioSource.spatialize = spatialize;
			audioSource.spatializePostEffects = spatializePostEffects;
			audioSource.bypassEffects = bypassEffects;
			audioSource.bypassListenerEffects = bypassListenerEffects;
			audioSource.bypassReverbZones = bypassReverbZones;
			audioSource.priority = priority;
			audioSource.spatialBlend = spatialBlend;
			audioSource.dopplerLevel = dopplerLevel;
			audioSource.spread = spread;
			audioSource.rolloffMode = rolloffMode;
			audioSource.minDistance = minDistance;
			audioSource.maxDistance = maxDistance;
			audioSource.reverbZoneMix = reverbZoneMix;
		}
		audioSource.volume = 1f;
		audioSource.playOnAwake = false;
		if (shuffleOrder)
		{
			int[] array = new int[soundBank.sounds.Length / 2];
			playlist = new PlaylistEntry[soundBank.sounds.Length * 8];
			for (int i = 0; i < playlist.Length; i++)
			{
				int num = 0;
				for (int j = 0; j < 100; j++)
				{
					num = UnityEngine.Random.Range(0, soundBank.sounds.Length);
					if (Array.IndexOf(array, num) == -1)
					{
						break;
					}
				}
				if (array.Length != 0)
				{
					array[i % array.Length] = num;
				}
				playlist[i] = new PlaylistEntry
				{
					index = num,
					volume = UnityEngine.Random.Range(soundBank.volumeRange.x, soundBank.volumeRange.y),
					pitch = UnityEngine.Random.Range(soundBank.pitchRange.x, soundBank.pitchRange.y)
				};
			}
		}
		else
		{
			playlist = new PlaylistEntry[soundBank.sounds.Length * 8];
			for (int k = 0; k < playlist.Length; k++)
			{
				playlist[k] = new PlaylistEntry
				{
					index = k % soundBank.sounds.Length,
					volume = UnityEngine.Random.Range(soundBank.volumeRange.x, soundBank.volumeRange.y),
					pitch = UnityEngine.Random.Range(soundBank.pitchRange.x, soundBank.pitchRange.y)
				};
			}
		}
	}

	protected void OnEnable()
	{
		if (playOnEnable)
		{
			Play();
		}
	}

	public void Play(float? volumeOverride = null, float? pitchOverride = null)
	{
		if (soundBank.sounds.Length != 0)
		{
			PlaylistEntry playlistEntry = playlist[nextIndex];
			audioSource.pitch = (pitchOverride.HasValue ? pitchOverride.Value : playlistEntry.pitch);
			AudioClip audioClip = soundBank.sounds[playlistEntry.index];
			audioSource.PlayOneShot(audioClip, volumeOverride.HasValue ? volumeOverride.Value : playlistEntry.volume);
			playEndTime = Mathf.Max(playEndTime, Time.time + audioClip.length);
			nextIndex = (nextIndex + 1) % playlist.Length;
		}
	}
}
