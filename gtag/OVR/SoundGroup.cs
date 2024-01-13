using System;
using UnityEngine;
using UnityEngine.Audio;

namespace OVR;

[Serializable]
public class SoundGroup
{
	public string name = string.Empty;

	public SoundFX[] soundList = new SoundFX[0];

	public AudioMixerGroup mixerGroup;

	[Range(0f, 64f)]
	public int maxPlayingSounds;

	public PreloadSounds preloadAudio;

	public float volumeOverride = 1f;

	[HideInInspector]
	public int playingSoundCount;

	public SoundGroup(string name)
	{
		this.name = name;
	}

	public SoundGroup()
	{
		mixerGroup = null;
		maxPlayingSounds = 0;
		preloadAudio = PreloadSounds.Default;
		volumeOverride = 1f;
	}

	public void IncrementPlayCount()
	{
		playingSoundCount = Mathf.Clamp(++playingSoundCount, 0, maxPlayingSounds);
	}

	public void DecrementPlayCount()
	{
		playingSoundCount = Mathf.Clamp(--playingSoundCount, 0, maxPlayingSounds);
	}

	public bool CanPlaySound()
	{
		if (maxPlayingSounds != 0)
		{
			return playingSoundCount < maxPlayingSounds;
		}
		return true;
	}
}
