using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OVR;

public class AudioManager : MonoBehaviour
{
	public enum Fade
	{
		In,
		Out
	}

	[Tooltip("Make the audio manager persistent across all scene loads")]
	public bool makePersistent = true;

	[Tooltip("Enable the OSP audio plugin features")]
	public bool enableSpatializedAudio = true;

	[Tooltip("Always play spatialized sounds with no reflections (Default)")]
	public bool enableSpatializedFastOverride;

	[Tooltip("The audio mixer asset used for snapshot blends, etc.")]
	public AudioMixer audioMixer;

	[Tooltip("The audio mixer group used for the pooled emitters")]
	public AudioMixerGroup defaultMixerGroup;

	[Tooltip("The audio mixer group used for the reserved pool emitter")]
	public AudioMixerGroup reservedMixerGroup;

	[Tooltip("The audio mixer group used for voice chat")]
	public AudioMixerGroup voiceChatMixerGroup;

	[Tooltip("Log all PlaySound calls to the Unity console")]
	public bool verboseLogging;

	[Tooltip("Maximum sound emitters")]
	public int maxSoundEmitters = 32;

	[Tooltip("Default volume for all sounds modulated by individual sound FX volumes")]
	public float volumeSoundFX = 1f;

	[Tooltip("Sound FX fade time")]
	public float soundFxFadeSecs = 1f;

	public float audioMinFallOffDistance = 1f;

	public float audioMaxFallOffDistance = 25f;

	public SoundGroup[] soundGroupings = new SoundGroup[0];

	private Dictionary<string, SoundFX> soundFXCache;

	private static AudioManager theAudioManager = null;

	private static FastList<string> names = new FastList<string>();

	private static string[] defaultSound = new string[1] { "Default Sound" };

	private static SoundFX nullSound = new SoundFX();

	private static bool hideWarnings = false;

	private float audioMaxFallOffDistanceSqr = 625f;

	private SoundEmitter[] soundEmitters;

	private FastList<SoundEmitter> playingEmitters = new FastList<SoundEmitter>();

	private FastList<SoundEmitter> nextFreeEmitters = new FastList<SoundEmitter>();

	private MixerSnapshot currentSnapshot;

	private static GameObject soundEmitterParent = null;

	private static Transform staticListenerPosition = null;

	private static bool showPlayingEmitterCount = false;

	private static bool forceShowEmitterCount = false;

	private static bool soundEnabled = true;

	private static readonly AnimationCurve defaultReverbZoneMix = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	public static bool enableSpatialization
	{
		get
		{
			if (!(theAudioManager != null))
			{
				return false;
			}
			return theAudioManager.enableSpatializedAudio;
		}
	}

	public static AudioManager Instance => theAudioManager;

	public static float NearFallOff => theAudioManager.audioMinFallOffDistance;

	public static float FarFallOff => theAudioManager.audioMaxFallOffDistance;

	public static AudioMixerGroup EmitterGroup => theAudioManager.defaultMixerGroup;

	public static AudioMixerGroup ReservedGroup => theAudioManager.reservedMixerGroup;

	public static AudioMixerGroup VoipGroup => theAudioManager.voiceChatMixerGroup;

	public static bool SoundEnabled => soundEnabled;

	private void Awake()
	{
		Init();
	}

	private void OnDestroy()
	{
		if (theAudioManager == this && soundEmitterParent != null)
		{
			UnityEngine.Object.Destroy(soundEmitterParent);
		}
	}

	private void Init()
	{
		if (theAudioManager != null)
		{
			if (Application.isPlaying && theAudioManager != this)
			{
				base.enabled = false;
			}
			return;
		}
		theAudioManager = this;
		nullSound.name = "Default Sound";
		RebuildSoundFXCache();
		if (Application.isPlaying)
		{
			InitializeSoundSystem();
			if (makePersistent && base.transform.parent == null)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}
	}

	private void Update()
	{
		UpdateFreeEmitters();
	}

	private void RebuildSoundFXCache()
	{
		int num = 0;
		for (int i = 0; i < soundGroupings.Length; i++)
		{
			num += soundGroupings[i].soundList.Length;
		}
		soundFXCache = new Dictionary<string, SoundFX>(num + 1);
		soundFXCache.Add(nullSound.name, nullSound);
		for (int j = 0; j < soundGroupings.Length; j++)
		{
			for (int k = 0; k < soundGroupings[j].soundList.Length; k++)
			{
				if (soundFXCache.ContainsKey(soundGroupings[j].soundList[k].name))
				{
					Debug.LogError("ERROR: Duplicate Sound FX name in the audio manager: '" + soundGroupings[j].name + "' > '" + soundGroupings[j].soundList[k].name + "'");
				}
				else
				{
					soundGroupings[j].soundList[k].Group = soundGroupings[j];
					soundFXCache.Add(soundGroupings[j].soundList[k].name, soundGroupings[j].soundList[k]);
				}
			}
			soundGroupings[j].playingSoundCount = 0;
		}
	}

	public static SoundFX FindSoundFX(string name, bool rebuildCache = false)
	{
		if (string.IsNullOrEmpty(name))
		{
			return nullSound;
		}
		if (rebuildCache)
		{
			theAudioManager.RebuildSoundFXCache();
		}
		if (!theAudioManager.soundFXCache.ContainsKey(name))
		{
			return nullSound;
		}
		return theAudioManager.soundFXCache[name];
	}

	private static bool FindAudioManager()
	{
		GameObject gameObject = GameObject.Find("AudioManager");
		if (gameObject == null || gameObject.GetComponent<AudioManager>() == null)
		{
			if (!hideWarnings)
			{
				Debug.LogError("[ERROR] AudioManager object missing from hierarchy!");
				hideWarnings = true;
			}
			return false;
		}
		gameObject.GetComponent<AudioManager>().Init();
		return true;
	}

	public static GameObject GetGameObject()
	{
		if (theAudioManager == null && !FindAudioManager())
		{
			return null;
		}
		return theAudioManager.gameObject;
	}

	public static string NameMinusGroup(string name)
	{
		if (name.IndexOf("/") > -1)
		{
			return name.Substring(name.IndexOf("/") + 1);
		}
		return name;
	}

	public static string[] GetSoundFXNames(string currentValue, out int currentIdx)
	{
		currentIdx = 0;
		names.Clear();
		if (theAudioManager == null && !FindAudioManager())
		{
			return defaultSound;
		}
		names.Add(nullSound.name);
		for (int i = 0; i < theAudioManager.soundGroupings.Length; i++)
		{
			for (int j = 0; j < theAudioManager.soundGroupings[i].soundList.Length; j++)
			{
				if (string.Compare(currentValue, theAudioManager.soundGroupings[i].soundList[j].name, ignoreCase: true) == 0)
				{
					currentIdx = names.Count;
				}
				names.Add(theAudioManager.soundGroupings[i].name + "/" + theAudioManager.soundGroupings[i].soundList[j].name);
			}
		}
		return names.ToArray();
	}

	private static int CalculateMaxEmittersSize()
	{
		return theAudioManager.maxSoundEmitters + 1;
	}

	private static bool ValidateEmitterIndex(int index)
	{
		if (index > -1)
		{
			return index < CalculateMaxEmittersSize();
		}
		return false;
	}

	private void InitializeSoundSystem()
	{
		int bufferLength = 960;
		int numBuffers = 4;
		AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
		if (Application.isPlaying)
		{
			Debug.Log("[AudioManager] Audio Sample Rate: " + AudioSettings.outputSampleRate);
			Debug.Log("[AudioManager] Audio Buffer Length: " + bufferLength + " Size: " + numBuffers);
		}
		AudioListener audioListener = UnityEngine.Object.FindObjectOfType<AudioListener>();
		if (audioListener == null)
		{
			Debug.LogError("[AudioManager] Missing AudioListener object!  Add one to the scene.");
		}
		else
		{
			staticListenerPosition = audioListener.transform;
		}
		soundEmitters = new SoundEmitter[CalculateMaxEmittersSize()];
		soundEmitterParent = GameObject.Find("__SoundEmitters__");
		if (soundEmitterParent != null)
		{
			UnityEngine.Object.Destroy(soundEmitterParent);
		}
		soundEmitterParent = new GameObject("__SoundEmitters__");
		for (int i = 0; i < CalculateMaxEmittersSize(); i++)
		{
			GameObject gameObject = new GameObject("SoundEmitter_" + i);
			gameObject.transform.parent = soundEmitterParent.transform;
			gameObject.transform.position = Vector3.zero;
			gameObject.hideFlags = HideFlags.DontSaveInEditor;
			soundEmitters[i] = gameObject.AddComponent<SoundEmitter>();
			soundEmitters[i].SetDefaultParent(soundEmitterParent.transform);
			soundEmitters[i].SetChannel(i);
			soundEmitters[i].Stop();
			soundEmitters[i].originalIdx = i;
		}
		ResetFreeEmitters();
		soundEmitterParent.hideFlags = HideFlags.DontSaveInEditor;
		audioMaxFallOffDistanceSqr = audioMaxFallOffDistance * audioMaxFallOffDistance;
	}

	private void UpdateFreeEmitters()
	{
		if (verboseLogging)
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				forceShowEmitterCount = !forceShowEmitterCount;
			}
			if (forceShowEmitterCount)
			{
				showPlayingEmitterCount = true;
			}
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		while (num7 < playingEmitters.size)
		{
			if (playingEmitters[num7] == null)
			{
				Debug.LogError("[AudioManager] ERROR: playingEmitters list had a null emitter! Something nuked a sound emitter!!!");
				playingEmitters.RemoveAtFast(num7);
				return;
			}
			if (!playingEmitters[num7].IsPlaying())
			{
				if (verboseLogging && nextFreeEmitters.Contains(playingEmitters[num7]))
				{
					Debug.LogError("[AudioManager] ERROR: playing sound emitter already in the free emitters list!");
				}
				playingEmitters[num7].Stop();
				nextFreeEmitters.Add(playingEmitters[num7]);
				playingEmitters.RemoveAtFast(num7);
				continue;
			}
			if (verboseLogging && showPlayingEmitterCount)
			{
				num++;
				switch (playingEmitters[num7].priority)
				{
				case SoundPriority.VeryLow:
					num2++;
					break;
				case SoundPriority.Low:
					num3++;
					break;
				case SoundPriority.Default:
					num4++;
					break;
				case SoundPriority.High:
					num5++;
					break;
				case SoundPriority.VeryHigh:
					num6++;
					break;
				}
			}
			num7++;
		}
		if (verboseLogging && showPlayingEmitterCount)
		{
			Debug.LogWarning($"[AudioManager] Playing sounds: Total {Fmt(num)} | VeryLow {Fmt(num2)} | Low {Fmt(num3)} | Default {Fmt(num4)} | High {Fmt(num5)} | VeryHigh {Fmt(num6)} | Free {FmtFree(nextFreeEmitters.Count)}");
			showPlayingEmitterCount = false;
		}
	}

	private string Fmt(int count)
	{
		float num = (float)count / (float)theAudioManager.maxSoundEmitters;
		if (num < 0.5f)
		{
			return "<color=green>" + count + "</color>";
		}
		if ((double)num < 0.7)
		{
			return "<color=yellow>" + count + "</color>";
		}
		return "<color=red>" + count + "</color>";
	}

	private string FmtFree(int count)
	{
		float num = (float)count / (float)theAudioManager.maxSoundEmitters;
		if (num < 0.2f)
		{
			return "<color=red>" + count + "</color>";
		}
		if ((double)num < 0.3)
		{
			return "<color=yellow>" + count + "</color>";
		}
		return "<color=green>" + count + "</color>";
	}

	private void OnPreSceneLoad()
	{
		Debug.Log("[AudioManager] OnPreSceneLoad cleanup");
		for (int i = 0; i < soundEmitters.Length; i++)
		{
			soundEmitters[i].Stop();
			soundEmitters[i].ResetParent(soundEmitterParent.transform);
		}
		ResetFreeEmitters();
	}

	private void ResetFreeEmitters()
	{
		nextFreeEmitters.Clear();
		playingEmitters.Clear();
		for (int i = 1; i < soundEmitters.Length; i++)
		{
			nextFreeEmitters.Add(soundEmitters[i]);
		}
	}

	public static void FadeOutSoundChannel(int channel, float delaySecs, float fadeTime)
	{
		theAudioManager.soundEmitters[channel].FadeOutDelayed(delaySecs, fadeTime);
	}

	public static bool StopSound(int idx, bool fadeOut = true, bool stopReserved = false)
	{
		if (!stopReserved && idx == 0)
		{
			return false;
		}
		if (!fadeOut)
		{
			theAudioManager.soundEmitters[idx].Stop();
		}
		else
		{
			theAudioManager.soundEmitters[idx].FadeOut(theAudioManager.soundFxFadeSecs);
		}
		return true;
	}

	public static void FadeInSound(int idx, float fadeTime, float volume)
	{
		theAudioManager.soundEmitters[idx].FadeIn(fadeTime, volume);
	}

	public static void FadeInSound(int idx, float fadeTime)
	{
		theAudioManager.soundEmitters[idx].FadeIn(fadeTime);
	}

	public static void FadeOutSound(int idx, float fadeTime)
	{
		theAudioManager.soundEmitters[idx].FadeOut(fadeTime);
	}

	public static void StopAllSounds(bool fadeOut, bool stopReserved = false)
	{
		for (int i = 0; i < theAudioManager.soundEmitters.Length; i++)
		{
			StopSound(i, fadeOut, stopReserved);
		}
	}

	public void MuteAllSounds(bool mute, bool muteReserved = false)
	{
		for (int i = 0; i < soundEmitters.Length; i++)
		{
			if (muteReserved || i != 0)
			{
				soundEmitters[i].audioSource.mute = true;
			}
		}
	}

	public void UnMuteAllSounds(bool unmute, bool unmuteReserved = false)
	{
		for (int i = 0; i < soundEmitters.Length; i++)
		{
			if ((unmuteReserved || i != 0) && soundEmitters[i].audioSource.isPlaying)
			{
				soundEmitters[i].audioSource.mute = false;
			}
		}
	}

	public static float GetEmitterEndTime(int idx)
	{
		return theAudioManager.soundEmitters[idx].endPlayTime;
	}

	public static float SetEmitterTime(int idx, float time)
	{
		return theAudioManager.soundEmitters[idx].time = time;
	}

	public static int PlaySound(AudioClip clip, float volume, EmitterChannel src = EmitterChannel.Any, float delay = 0f, float pitchVariance = 1f, bool loop = false)
	{
		if (!SoundEnabled)
		{
			return -1;
		}
		return PlaySoundAt((staticListenerPosition != null) ? staticListenerPosition.position : Vector3.zero, clip, volume, src, delay, pitchVariance, loop);
	}

	private static int FindFreeEmitter(EmitterChannel src, SoundPriority priority)
	{
		SoundEmitter soundEmitter = theAudioManager.soundEmitters[0];
		if (src == EmitterChannel.Any)
		{
			if (theAudioManager.nextFreeEmitters.size > 0)
			{
				soundEmitter = theAudioManager.nextFreeEmitters[0];
				theAudioManager.nextFreeEmitters.RemoveAtFast(0);
			}
			else
			{
				if (priority == SoundPriority.VeryLow)
				{
					return -1;
				}
				soundEmitter = theAudioManager.playingEmitters.Find((SoundEmitter item) => item != null && item.priority < priority);
				if (soundEmitter == null)
				{
					if (priority < SoundPriority.Default)
					{
						if (theAudioManager.verboseLogging)
						{
							Debug.LogWarning("[AudioManager] skipping sound " + priority);
						}
						return -1;
					}
					soundEmitter = theAudioManager.playingEmitters.Find((SoundEmitter item) => item != null && item.priority <= SoundPriority.Default);
				}
				if (soundEmitter != null)
				{
					if (theAudioManager.verboseLogging)
					{
						Debug.LogWarning("[AudioManager] cannabalizing " + soundEmitter.originalIdx + " Time: " + Time.time);
					}
					soundEmitter.Stop();
					theAudioManager.playingEmitters.RemoveFast(soundEmitter);
				}
			}
		}
		if (soundEmitter == null)
		{
			Debug.LogError(string.Concat("[AudioManager] ERROR - absolutely couldn't find a free emitter! Priority = ", priority, " TOO MANY PlaySound* calls!"));
			showPlayingEmitterCount = true;
			return -1;
		}
		return soundEmitter.originalIdx;
	}

	public static int PlaySound(SoundFX soundFX, EmitterChannel src = EmitterChannel.Any, float delay = 0f)
	{
		if (!SoundEnabled)
		{
			return -1;
		}
		return PlaySoundAt((staticListenerPosition != null) ? staticListenerPosition.position : Vector3.zero, soundFX, src, delay);
	}

	public static int PlaySoundAt(Vector3 position, SoundFX soundFX, EmitterChannel src = EmitterChannel.Any, float delay = 0f, float volumeOverride = 1f, float pitchMultiplier = 1f)
	{
		if (!SoundEnabled)
		{
			return -1;
		}
		AudioClip clip = soundFX.GetClip();
		if (clip == null)
		{
			return -1;
		}
		if (staticListenerPosition != null)
		{
			float sqrMagnitude = (staticListenerPosition.position - position).sqrMagnitude;
			if (sqrMagnitude > theAudioManager.audioMaxFallOffDistanceSqr)
			{
				return -1;
			}
			if (sqrMagnitude > soundFX.MaxFalloffDistSquared)
			{
				return -1;
			}
		}
		if (soundFX.ReachedGroupPlayLimit())
		{
			if (theAudioManager.verboseLogging)
			{
				Debug.Log("[AudioManager] PlaySoundAt() with " + soundFX.name + " skipped due to group play limit");
			}
			return -1;
		}
		int num = FindFreeEmitter(src, soundFX.priority);
		if (num == -1)
		{
			return -1;
		}
		SoundEmitter soundEmitter = theAudioManager.soundEmitters[num];
		soundEmitter.ResetParent(soundEmitterParent.transform);
		soundEmitter.gameObject.SetActive(value: true);
		AudioSource source = soundEmitter.audioSource;
		ONSPAudioSource osp = soundEmitter.osp;
		source.enabled = true;
		source.volume = Mathf.Clamp01(Mathf.Clamp01(theAudioManager.volumeSoundFX * soundFX.volume) * volumeOverride * soundFX.GroupVolumeOverride);
		source.pitch = soundFX.GetPitch() * pitchMultiplier;
		source.time = 0f;
		source.spatialBlend = 1f;
		source.rolloffMode = soundFX.falloffCurve;
		if (soundFX.falloffCurve == AudioRolloffMode.Custom)
		{
			source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, soundFX.volumeFalloffCurve);
		}
		source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, soundFX.reverbZoneMix);
		source.dopplerLevel = 0f;
		source.clip = clip;
		source.spread = soundFX.spread;
		source.loop = soundFX.looping;
		source.mute = false;
		source.minDistance = soundFX.falloffDistance.x;
		source.maxDistance = soundFX.falloffDistance.y;
		source.outputAudioMixerGroup = soundFX.GetMixerGroup(EmitterGroup);
		soundEmitter.endPlayTime = Time.time + clip.length + delay;
		soundEmitter.defaultVolume = source.volume;
		soundEmitter.priority = soundFX.priority;
		soundEmitter.onFinished = null;
		soundEmitter.SetPlayingSoundGroup(soundFX.Group);
		if (src == EmitterChannel.Any)
		{
			theAudioManager.playingEmitters.AddUnique(soundEmitter);
		}
		if (osp != null)
		{
			osp.EnableSpatialization = soundFX.ospProps.enableSpatialization;
			osp.EnableRfl = ((theAudioManager.enableSpatializedFastOverride || soundFX.ospProps.useFastOverride) ? true : false);
			osp.Gain = soundFX.ospProps.gain;
			osp.UseInvSqr = soundFX.ospProps.enableInvSquare;
			osp.Near = soundFX.ospProps.invSquareFalloff.x;
			osp.Far = soundFX.ospProps.invSquareFalloff.y;
			source.spatialBlend = (soundFX.ospProps.enableSpatialization ? 1f : 0.8f);
			osp.SetParameters(ref source);
		}
		source.transform.position = position;
		if (theAudioManager.verboseLogging)
		{
			Debug.Log("[AudioManager] PlaySoundAt() channel = " + num + " soundFX = " + soundFX.name + " volume = " + soundEmitter.volume + " Delay = " + delay + " time = " + Time.time + "\n");
		}
		if (delay > 0f)
		{
			source.PlayDelayed(delay);
		}
		else
		{
			source.Play();
		}
		return num;
	}

	public static int PlayRandomSoundAt(Vector3 position, AudioClip[] clips, float volume, EmitterChannel src = EmitterChannel.Any, float delay = 0f, float pitch = 1f, bool loop = false)
	{
		if (clips == null || clips.Length == 0)
		{
			return -1;
		}
		int num = UnityEngine.Random.Range(0, clips.Length);
		return PlaySoundAt(position, clips[num], volume, src, delay, pitch, loop);
	}

	public static int PlaySoundAt(Vector3 position, AudioClip clip, float volume = 1f, EmitterChannel src = EmitterChannel.Any, float delay = 0f, float pitch = 1f, bool loop = false)
	{
		if (!SoundEnabled)
		{
			return -1;
		}
		if (clip == null)
		{
			return -1;
		}
		if (staticListenerPosition != null && (staticListenerPosition.position - position).sqrMagnitude > theAudioManager.audioMaxFallOffDistanceSqr)
		{
			return -1;
		}
		int num = FindFreeEmitter(src, SoundPriority.Default);
		if (num == -1)
		{
			return -1;
		}
		SoundEmitter soundEmitter = theAudioManager.soundEmitters[num];
		soundEmitter.ResetParent(soundEmitterParent.transform);
		soundEmitter.gameObject.SetActive(value: true);
		AudioSource audioSource = soundEmitter.audioSource;
		ONSPAudioSource osp = soundEmitter.osp;
		audioSource.enabled = true;
		audioSource.volume = Mathf.Clamp01(theAudioManager.volumeSoundFX * volume);
		audioSource.pitch = pitch;
		audioSource.spatialBlend = 0.8f;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, defaultReverbZoneMix);
		audioSource.dopplerLevel = 0f;
		audioSource.clip = clip;
		audioSource.spread = 0f;
		audioSource.loop = loop;
		audioSource.mute = false;
		audioSource.minDistance = theAudioManager.audioMinFallOffDistance;
		audioSource.maxDistance = theAudioManager.audioMaxFallOffDistance;
		audioSource.outputAudioMixerGroup = EmitterGroup;
		soundEmitter.endPlayTime = Time.time + clip.length + delay;
		soundEmitter.defaultVolume = audioSource.volume;
		soundEmitter.priority = SoundPriority.Default;
		soundEmitter.onFinished = null;
		soundEmitter.SetPlayingSoundGroup(null);
		if (src == EmitterChannel.Any)
		{
			theAudioManager.playingEmitters.AddUnique(soundEmitter);
		}
		if (osp != null)
		{
			osp.EnableSpatialization = false;
		}
		audioSource.transform.position = position;
		if (theAudioManager.verboseLogging)
		{
			Debug.Log("[AudioManager] PlaySoundAt() channel = " + num + " clip = " + clip.name + " volume = " + soundEmitter.volume + " Delay = " + delay + " time = " + Time.time + "\n");
		}
		if (delay > 0f)
		{
			audioSource.PlayDelayed(delay);
		}
		else
		{
			audioSource.Play();
		}
		return num;
	}

	public static void SetOnFinished(int emitterIdx, Action onFinished)
	{
		if (ValidateEmitterIndex(emitterIdx))
		{
			theAudioManager.soundEmitters[emitterIdx].SetOnFinished(onFinished);
		}
	}

	public static void SetOnFinished(int emitterIdx, Action<object> onFinished, object obj)
	{
		if (ValidateEmitterIndex(emitterIdx))
		{
			theAudioManager.soundEmitters[emitterIdx].SetOnFinished(onFinished, obj);
		}
	}

	public static void AttachSoundToParent(int idx, Transform parent)
	{
		if (theAudioManager.verboseLogging)
		{
			string text = parent.name;
			if (parent.parent != null)
			{
				text = parent.parent.name + "/" + text;
			}
			Debug.Log("[AudioManager] ATTACHING INDEX " + idx + " to " + text);
		}
		theAudioManager.soundEmitters[idx].ParentTo(parent);
	}

	public static void DetachSoundFromParent(int idx)
	{
		if (theAudioManager.verboseLogging)
		{
			Debug.Log("[AudioManager] DETACHING INDEX " + idx);
		}
		theAudioManager.soundEmitters[idx].DetachFromParent();
	}

	public static void DetachSoundsFromParent(SoundEmitter[] emitters, bool stopSounds = true)
	{
		if (emitters == null)
		{
			return;
		}
		foreach (SoundEmitter soundEmitter in emitters)
		{
			if (soundEmitter.defaultParent != null)
			{
				if (stopSounds)
				{
					soundEmitter.Stop();
				}
				soundEmitter.DetachFromParent();
				soundEmitter.gameObject.SetActive(value: true);
			}
			else if (stopSounds)
			{
				soundEmitter.Stop();
			}
		}
	}

	public static void SetEmitterMixerGroup(int idx, AudioMixerGroup mixerGroup)
	{
		if (theAudioManager != null && idx > -1)
		{
			theAudioManager.soundEmitters[idx].SetAudioMixer(mixerGroup);
		}
	}

	public static MixerSnapshot GetActiveSnapshot()
	{
		if (!(theAudioManager != null))
		{
			return null;
		}
		return theAudioManager.currentSnapshot;
	}

	public static void SetCurrentSnapshot(MixerSnapshot mixerSnapshot)
	{
		if (theAudioManager != null)
		{
			if (mixerSnapshot != null && mixerSnapshot.snapshot != null)
			{
				mixerSnapshot.snapshot.TransitionTo(mixerSnapshot.transitionTime);
			}
			else
			{
				mixerSnapshot = null;
			}
			theAudioManager.currentSnapshot = mixerSnapshot;
		}
	}

	public static void BlendWithCurrentSnapshot(MixerSnapshot blendSnapshot, float weight, float blendTime = 0f)
	{
		if (!(theAudioManager != null))
		{
			return;
		}
		if (theAudioManager.audioMixer == null)
		{
			Debug.LogWarning("[AudioManager] can't call BlendWithCurrentSnapshot if the audio mixer is not set!");
			return;
		}
		if (blendTime == 0f)
		{
			blendTime = Time.deltaTime;
		}
		if (theAudioManager.currentSnapshot != null && theAudioManager.currentSnapshot.snapshot != null && blendSnapshot != null && blendSnapshot.snapshot != null)
		{
			weight = Mathf.Clamp01(weight);
			if (weight == 0f)
			{
				theAudioManager.currentSnapshot.snapshot.TransitionTo(blendTime);
				return;
			}
			AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[2]
			{
				theAudioManager.currentSnapshot.snapshot,
				blendSnapshot.snapshot
			};
			float[] weights = new float[2]
			{
				1f - weight,
				weight
			};
			theAudioManager.audioMixer.TransitionToSnapshots(snapshots, weights, blendTime);
		}
	}
}
