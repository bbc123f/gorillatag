using System;
using UnityEngine;

namespace OVR;

[Serializable]
public class SoundFXRef
{
	public string soundFXName = string.Empty;

	private bool initialized;

	private SoundFX soundFXCached;

	public SoundFX soundFX
	{
		get
		{
			if (!initialized)
			{
				Init();
			}
			return soundFXCached;
		}
	}

	public string name
	{
		get
		{
			return soundFXName;
		}
		set
		{
			soundFXName = value;
			Init();
		}
	}

	public int Length => soundFX.Length;

	public bool IsValid => soundFX.IsValid;

	private void Init()
	{
		soundFXCached = AudioManager.FindSoundFX(soundFXName);
		if (soundFXCached == null)
		{
			soundFXCached = AudioManager.FindSoundFX(string.Empty);
		}
		initialized = true;
	}

	public AudioClip GetClip()
	{
		return soundFX.GetClip();
	}

	public float GetClipLength(int idx)
	{
		return soundFX.GetClipLength(idx);
	}

	public int PlaySound(float delaySecs = 0f)
	{
		return soundFX.PlaySound(delaySecs);
	}

	public int PlaySoundAt(Vector3 pos, float delaySecs = 0f, float volume = 1f, float pitchMultiplier = 1f)
	{
		return soundFX.PlaySoundAt(pos, delaySecs, volume, pitchMultiplier);
	}

	public void SetOnFinished(Action onFinished)
	{
		soundFX.SetOnFinished(onFinished);
	}

	public void SetOnFinished(Action<object> onFinished, object obj)
	{
		soundFX.SetOnFinished(onFinished, obj);
	}

	public bool StopSound()
	{
		return soundFX.StopSound();
	}

	public void AttachToParent(Transform parent)
	{
		soundFX.AttachToParent(parent);
	}

	public void DetachFromParent()
	{
		soundFX.DetachFromParent();
	}
}
