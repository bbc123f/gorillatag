﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.Audio
{
	internal static class GTAudioOneShot
	{
		[OnEnterPlay_Set(false)]
		internal static bool isInitialized
		{
			[CompilerGenerated]
			get
			{
				return GTAudioOneShot.<isInitialized>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				GTAudioOneShot.<isInitialized>k__BackingField = value;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (GTAudioOneShot.isInitialized)
			{
				return;
			}
			AudioSource audioSource = Resources.Load<AudioSource>("AudioSourceSingleton_Prefab");
			if (audioSource == null)
			{
				Debug.LogError("GTAudioOneShot: Failed to load AudioSourceSingleton_Prefab from resources!!!");
				return;
			}
			GTAudioOneShot.audioSource = Object.Instantiate<AudioSource>(audioSource);
			GTAudioOneShot.defaultCurve = GTAudioOneShot.audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
			Object.DontDestroyOnLoad(GTAudioOneShot.audioSource);
			GTAudioOneShot.isInitialized = true;
		}

		internal static void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.pitch = pitch;
			GTAudioOneShot.audioSource.transform.position = position;
			GTAudioOneShot.audioSource.PlayOneShot(clip, volume);
		}

		internal static void Play(AudioClip clip, Vector3 position, AnimationCurve curve, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
			GTAudioOneShot.Play(clip, position, volume, pitch);
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GTAudioOneShot.defaultCurve);
		}

		[CompilerGenerated]
		private static bool <isInitialized>k__BackingField;

		[OnEnterPlay_SetNull]
		internal static AudioSource audioSource;

		[OnEnterPlay_SetNull]
		internal static AnimationCurve defaultCurve;
	}
}
