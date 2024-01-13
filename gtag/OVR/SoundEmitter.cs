using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace OVR;

public class SoundEmitter : MonoBehaviour
{
	public enum FadeState
	{
		Null,
		FadingIn,
		FadingOut,
		Ducking
	}

	public EmitterChannel channel;

	public bool disableSpatialization;

	private FadeState state;

	[NonSerialized]
	[HideInInspector]
	public AudioSource audioSource;

	[NonSerialized]
	[HideInInspector]
	public SoundPriority priority;

	[NonSerialized]
	[HideInInspector]
	public ONSPAudioSource osp;

	[NonSerialized]
	[HideInInspector]
	public float endPlayTime;

	private Transform lastParentTransform;

	[NonSerialized]
	[HideInInspector]
	public float defaultVolume = 1f;

	[NonSerialized]
	[HideInInspector]
	public Transform defaultParent;

	[NonSerialized]
	[HideInInspector]
	public int originalIdx = -1;

	[NonSerialized]
	[HideInInspector]
	public Action onFinished;

	[NonSerialized]
	[HideInInspector]
	public Action<object> onFinishedObject;

	[NonSerialized]
	[HideInInspector]
	public object onFinishedParam;

	[NonSerialized]
	[HideInInspector]
	public SoundGroup playingSoundGroup;

	public float volume
	{
		get
		{
			return audioSource.volume;
		}
		set
		{
			audioSource.volume = value;
		}
	}

	public float pitch
	{
		get
		{
			return audioSource.pitch;
		}
		set
		{
			audioSource.pitch = value;
		}
	}

	public AudioClip clip
	{
		get
		{
			return audioSource.clip;
		}
		set
		{
			audioSource.clip = value;
		}
	}

	public float time
	{
		get
		{
			return audioSource.time;
		}
		set
		{
			audioSource.time = value;
		}
	}

	public float length
	{
		get
		{
			if (!(audioSource.clip != null))
			{
				return 0f;
			}
			return audioSource.clip.length;
		}
	}

	public bool loop
	{
		get
		{
			return audioSource.loop;
		}
		set
		{
			audioSource.loop = value;
		}
	}

	public bool mute
	{
		get
		{
			return audioSource.mute;
		}
		set
		{
			audioSource.mute = value;
		}
	}

	public AudioVelocityUpdateMode velocityUpdateMode
	{
		get
		{
			return audioSource.velocityUpdateMode;
		}
		set
		{
			audioSource.velocityUpdateMode = value;
		}
	}

	public bool isPlaying => audioSource.isPlaying;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
		if (AudioManager.enableSpatialization && !disableSpatialization)
		{
			osp = GetComponent<ONSPAudioSource>();
			if (osp == null)
			{
				osp = base.gameObject.AddComponent<ONSPAudioSource>();
			}
		}
		audioSource.playOnAwake = false;
		audioSource.Stop();
	}

	public void SetPlayingSoundGroup(SoundGroup soundGroup)
	{
		playingSoundGroup = soundGroup;
		soundGroup?.IncrementPlayCount();
	}

	public void SetOnFinished(Action onFinished)
	{
		this.onFinished = onFinished;
	}

	public void SetOnFinished(Action<object> onFinished, object obj)
	{
		onFinishedObject = onFinished;
		onFinishedParam = obj;
	}

	public void SetChannel(int _channel)
	{
		channel = (EmitterChannel)_channel;
	}

	public void SetDefaultParent(Transform parent)
	{
		defaultParent = parent;
	}

	public void SetAudioMixer(AudioMixerGroup _mixer)
	{
		if (audioSource != null)
		{
			audioSource.outputAudioMixerGroup = _mixer;
		}
	}

	public bool IsPlaying()
	{
		if (loop && audioSource.isPlaying)
		{
			return true;
		}
		return endPlayTime > Time.time;
	}

	public void Play()
	{
		state = FadeState.Null;
		endPlayTime = Time.time + length;
		StopAllCoroutines();
		audioSource.Play();
	}

	public void Pause()
	{
		state = FadeState.Null;
		StopAllCoroutines();
		audioSource.Pause();
	}

	public void Stop()
	{
		state = FadeState.Null;
		StopAllCoroutines();
		if (audioSource != null)
		{
			audioSource.Stop();
		}
		if (onFinished != null)
		{
			onFinished();
			onFinished = null;
		}
		if (onFinishedObject != null)
		{
			onFinishedObject(onFinishedParam);
			onFinishedObject = null;
		}
		if (playingSoundGroup != null)
		{
			playingSoundGroup.DecrementPlayCount();
			playingSoundGroup = null;
		}
	}

	private int GetSampleTime()
	{
		return audioSource.clip.samples - audioSource.timeSamples;
	}

	public void ParentTo(Transform parent)
	{
		if (lastParentTransform != null)
		{
			Debug.LogError("[SoundEmitter] You must detach the sound emitter before parenting to another object!");
			return;
		}
		lastParentTransform = base.transform.parent;
		base.transform.parent = parent;
	}

	public void DetachFromParent()
	{
		if (lastParentTransform == null)
		{
			base.transform.parent = defaultParent;
			return;
		}
		base.transform.parent = lastParentTransform;
		lastParentTransform = null;
	}

	public void ResetParent(Transform parent)
	{
		base.transform.parent = parent;
		lastParentTransform = null;
	}

	public void SyncTo(SoundEmitter other, float fadeTime, float toVolume)
	{
		StartCoroutine(DelayedSyncTo(other, fadeTime, toVolume));
	}

	private IEnumerator DelayedSyncTo(SoundEmitter other, float fadeTime, float toVolume)
	{
		yield return new WaitForEndOfFrame();
		audioSource.time = other.time;
		audioSource.Play();
		FadeTo(fadeTime, toVolume);
	}

	public void FadeTo(float fadeTime, float toVolume)
	{
		if (state != FadeState.FadingOut)
		{
			state = FadeState.Ducking;
			StopAllCoroutines();
			StartCoroutine(FadeSoundChannelTo(fadeTime, toVolume));
		}
	}

	public void FadeIn(float fadeTime, float defaultVolume)
	{
		audioSource.volume = 0f;
		state = FadeState.FadingIn;
		StopAllCoroutines();
		StartCoroutine(FadeSoundChannel(0f, fadeTime, Fade.In, defaultVolume));
	}

	public void FadeIn(float fadeTime)
	{
		audioSource.volume = 0f;
		state = FadeState.FadingIn;
		StopAllCoroutines();
		StartCoroutine(FadeSoundChannel(0f, fadeTime, Fade.In, defaultVolume));
	}

	public void FadeOut(float fadeTime)
	{
		if (audioSource.isPlaying)
		{
			state = FadeState.FadingOut;
			StopAllCoroutines();
			StartCoroutine(FadeSoundChannel(0f, fadeTime, Fade.Out, audioSource.volume));
		}
	}

	public void FadeOutDelayed(float delayedSecs, float fadeTime)
	{
		if (audioSource.isPlaying)
		{
			state = FadeState.FadingOut;
			StopAllCoroutines();
			StartCoroutine(FadeSoundChannel(delayedSecs, fadeTime, Fade.Out, audioSource.volume));
		}
	}

	private IEnumerator FadeSoundChannelTo(float fadeTime, float toVolume)
	{
		float start = audioSource.volume;
		float startTime = Time.realtimeSinceStartup;
		float elapsedTime = 0f;
		while (elapsedTime < fadeTime)
		{
			elapsedTime = Time.realtimeSinceStartup - startTime;
			float t = elapsedTime / fadeTime;
			audioSource.volume = Mathf.Lerp(start, toVolume, t);
			yield return 0;
		}
		state = FadeState.Null;
	}

	private IEnumerator FadeSoundChannel(float delaySecs, float fadeTime, Fade fadeType, float defaultVolume)
	{
		if (delaySecs > 0f)
		{
			yield return new WaitForSeconds(delaySecs);
		}
		float start = ((fadeType == Fade.In) ? 0f : defaultVolume);
		float end = ((fadeType == Fade.In) ? defaultVolume : 0f);
		bool restartPlay = false;
		if (fadeType == Fade.In)
		{
			if (Time.time == 0f)
			{
				restartPlay = true;
			}
			audioSource.volume = 0f;
			audioSource.Play();
		}
		float startTime = Time.realtimeSinceStartup;
		float elapsedTime = 0f;
		while (elapsedTime < fadeTime)
		{
			elapsedTime = Time.realtimeSinceStartup - startTime;
			float t = elapsedTime / fadeTime;
			audioSource.volume = Mathf.Lerp(start, end, t);
			yield return 0;
			if (restartPlay && Time.time > 0f)
			{
				audioSource.Play();
				restartPlay = false;
			}
			if (!audioSource.isPlaying)
			{
				break;
			}
		}
		if (fadeType == Fade.Out)
		{
			Stop();
		}
		state = FadeState.Null;
	}
}
