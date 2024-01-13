using System;
using UnityEngine;
using UnityEngine.Audio;

namespace OVR;

[Serializable]
public class SoundFX
{
	[Tooltip("Each sound FX should have a unique name")]
	public string name = string.Empty;

	[Tooltip("Sound diversity playback option when multiple audio clips are defined, default = Random")]
	public SoundFXNext playback;

	[Tooltip("Default volume for this sound FX, default = 1.0")]
	[Range(0f, 1f)]
	public float volume = 1f;

	[Tooltip("Random pitch variance each time a sound FX is played, default = 1.0 (none)")]
	[MinMax(1f, 1f, 0f, 2f)]
	public Vector2 pitchVariance = Vector2.one;

	[Tooltip("Falloff distance for the sound FX, default = 1m min to 25m max")]
	[MinMax(1f, 25f, 0f, 250f)]
	public Vector2 falloffDistance = new Vector2(1f, 25f);

	[Tooltip("Volume falloff curve - sets how the sound FX attenuates over distance, default = Linear")]
	public AudioRolloffMode falloffCurve = AudioRolloffMode.Linear;

	[Tooltip("Defines the custom volume falloff curve")]
	public AnimationCurve volumeFalloffCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	[Tooltip("The amount by which the signal from the AudioSource will be mixed into the global reverb associated with the Reverb Zones | Valid range is 0.0 - 1.1, default = 1.0")]
	public AnimationCurve reverbZoneMix = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	[Tooltip("Sets the spread angle (in degrees) of a 3d stereo or multichannel sound in speaker space, default = 0")]
	[Range(0f, 360f)]
	public float spread;

	[Tooltip("The percentage chance that this sound FX will play | 0.0 = none, 1.0 = 100%, default = 1.0")]
	[Range(0f, 1f)]
	public float pctChanceToPlay = 1f;

	[Tooltip("Sets the priority for this sound to play and/or to override a currently playing sound FX, default = Default")]
	public SoundPriority priority;

	[Tooltip("Specifies the default delay when this sound FX is played, default = 0.0 secs")]
	[MinMax(0f, 0f, 0f, 2f)]
	public Vector2 delay = Vector2.zero;

	[Tooltip("Set to true for the sound to loop continuously, default = false")]
	public bool looping;

	public OSPProps ospProps = new OSPProps();

	[Tooltip("List of the audio clips assigned to this sound FX")]
	public AudioClip[] soundClips = new AudioClip[1];

	public bool visibilityToggle;

	[NonSerialized]
	private SoundGroup soundGroup;

	private int lastIdx = -1;

	private int playingIdx = -1;

	public int Length => soundClips.Length;

	public bool IsValid
	{
		get
		{
			if (soundClips.Length != 0)
			{
				return soundClips[0] != null;
			}
			return false;
		}
	}

	public SoundGroup Group
	{
		get
		{
			return soundGroup;
		}
		set
		{
			soundGroup = value;
		}
	}

	public float MaxFalloffDistSquared => falloffDistance.y * falloffDistance.y;

	public float GroupVolumeOverride
	{
		get
		{
			if (soundGroup == null)
			{
				return 1f;
			}
			return soundGroup.volumeOverride;
		}
	}

	public SoundFX()
	{
		playback = SoundFXNext.Random;
		volume = 1f;
		pitchVariance = Vector2.one;
		falloffDistance = new Vector2(1f, 25f);
		falloffCurve = AudioRolloffMode.Linear;
		volumeFalloffCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
		reverbZoneMix = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
		spread = 0f;
		pctChanceToPlay = 1f;
		priority = SoundPriority.Default;
		delay = Vector2.zero;
		looping = false;
		ospProps = new OSPProps();
	}

	public AudioClip GetClip()
	{
		if (soundClips.Length == 0)
		{
			return null;
		}
		if (soundClips.Length == 1)
		{
			return soundClips[0];
		}
		if (playback == SoundFXNext.Random)
		{
			int num;
			for (num = UnityEngine.Random.Range(0, soundClips.Length); num == lastIdx; num = UnityEngine.Random.Range(0, soundClips.Length))
			{
			}
			lastIdx = num;
			return soundClips[num];
		}
		if (++lastIdx >= soundClips.Length)
		{
			lastIdx = 0;
		}
		return soundClips[lastIdx];
	}

	public AudioMixerGroup GetMixerGroup(AudioMixerGroup defaultMixerGroup)
	{
		if (soundGroup != null)
		{
			if (!(soundGroup.mixerGroup != null))
			{
				return defaultMixerGroup;
			}
			return soundGroup.mixerGroup;
		}
		return defaultMixerGroup;
	}

	public bool ReachedGroupPlayLimit()
	{
		if (soundGroup != null)
		{
			return !soundGroup.CanPlaySound();
		}
		return false;
	}

	public float GetClipLength(int idx)
	{
		if (idx == -1 || soundClips.Length == 0 || idx >= soundClips.Length || soundClips[idx] == null)
		{
			return 0f;
		}
		return soundClips[idx].length;
	}

	public float GetPitch()
	{
		return UnityEngine.Random.Range(pitchVariance.x, pitchVariance.y);
	}

	public int PlaySound(float delaySecs = 0f)
	{
		playingIdx = -1;
		if (!IsValid)
		{
			return playingIdx;
		}
		if (pctChanceToPlay > 0.99f || UnityEngine.Random.value < pctChanceToPlay)
		{
			if (delay.y > 0f)
			{
				delaySecs = UnityEngine.Random.Range(delay.x, delay.y);
			}
			playingIdx = AudioManager.PlaySound(this, EmitterChannel.Any, delaySecs);
		}
		return playingIdx;
	}

	public int PlaySoundAt(Vector3 pos, float delaySecs = 0f, float volumeOverride = 1f, float pitchMultiplier = 1f)
	{
		playingIdx = -1;
		if (!IsValid)
		{
			return playingIdx;
		}
		if (pctChanceToPlay > 0.99f || UnityEngine.Random.value < pctChanceToPlay)
		{
			if (delay.y > 0f)
			{
				delaySecs = UnityEngine.Random.Range(delay.x, delay.y);
			}
			playingIdx = AudioManager.PlaySoundAt(pos, this, EmitterChannel.Any, delaySecs, volumeOverride, pitchMultiplier);
		}
		return playingIdx;
	}

	public void SetOnFinished(Action onFinished)
	{
		if (playingIdx > -1)
		{
			AudioManager.SetOnFinished(playingIdx, onFinished);
		}
	}

	public void SetOnFinished(Action<object> onFinished, object obj)
	{
		if (playingIdx > -1)
		{
			AudioManager.SetOnFinished(playingIdx, onFinished, obj);
		}
	}

	public bool StopSound()
	{
		bool result = false;
		if (playingIdx > -1)
		{
			result = AudioManager.StopSound(playingIdx);
			playingIdx = -1;
		}
		return result;
	}

	public void AttachToParent(Transform parent)
	{
		if (playingIdx > -1)
		{
			AudioManager.AttachSoundToParent(playingIdx, parent);
		}
	}

	public void DetachFromParent()
	{
		if (playingIdx > -1)
		{
			AudioManager.DetachSoundFromParent(playingIdx);
		}
	}
}
