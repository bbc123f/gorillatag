using System;
using GorillaNetworking;
using GorillaTag.Audio;
using Oculus.VoiceSDK.Dictation.Utilities;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class GorillaSpeakerLoudness : MonoBehaviour
{
	public bool IsSpeaking
	{
		get
		{
			return this.isSpeaking;
		}
	}

	public float Loudness
	{
		get
		{
			return this.loudness;
		}
	}

	public bool IsMicEnabled
	{
		get
		{
			return this.isMicEnabled;
		}
	}

	public float SmoothedLoudness
	{
		get
		{
			return this.smoothedLoudness;
		}
	}

	private void Start()
	{
		this.rigContainer = base.GetComponent<RigContainer>();
	}

	private void Update()
	{
		this.UpdateMicEnabled();
		this.UpdateLoudness();
		this.UpdateSmoothedLoudness();
	}

	private void UpdateMicEnabled()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig)
		{
			this.permission = this.permission || MicPermissionsManager.HasMicPermission();
			if (this.permission && !this.micConnected && Microphone.devices != null)
			{
				this.micConnected = Microphone.devices.Length != 0;
			}
			this.isMicEnabled = this.permission && this.micConnected;
			rig.IsMicEnabled = this.isMicEnabled;
			return;
		}
		this.isMicEnabled = rig.IsMicEnabled;
	}

	private void UpdateLoudness()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		PhotonVoiceView voice = this.rigContainer.Voice;
		if (voice != null && this.speaker == null)
		{
			this.speaker = voice.SpeakerInUse;
		}
		if (this.recorder == null)
		{
			this.recorder = ((voice != null) ? voice.RecorderInUse : null);
		}
		VRRig rig = this.rigContainer.Rig;
		if ((rig.remoteUseReplacementVoice || rig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && rig.SpeakingLoudness > 0f)
		{
			this.isSpeaking = true;
			this.loudness = rig.SpeakingLoudness;
			return;
		}
		if (voice != null && voice.IsSpeaking)
		{
			this.isSpeaking = true;
			if (!(this.speaker != null))
			{
				this.loudness = 0f;
				return;
			}
			if (this.speakerVoiceToLoudness == null)
			{
				this.speakerVoiceToLoudness = this.speaker.GetComponent<SpeakerVoiceToLoudness>();
			}
			if (this.speakerVoiceToLoudness != null)
			{
				this.loudness = this.speakerVoiceToLoudness.loudness;
				return;
			}
		}
		else if (voice != null && this.recorder != null && NetworkSystem.Instance.IsObjectLocallyOwned(voice.gameObject) && this.recorder.IsCurrentlyTransmitting)
		{
			if (this.voiceToLoudness == null)
			{
				this.voiceToLoudness = this.recorder.GetComponent<VoiceToLoudness>();
			}
			this.isSpeaking = true;
			if (this.voiceToLoudness != null)
			{
				this.loudness = this.voiceToLoudness.loudness;
				return;
			}
			this.loudness = 0f;
			return;
		}
		else
		{
			this.isSpeaking = false;
			this.loudness = 0f;
		}
	}

	private void UpdateSmoothedLoudness()
	{
		if (!this.isSpeaking)
		{
			this.smoothedLoudness = 0f;
			return;
		}
		if (!Mathf.Approximately(this.loudness, this.lastLoudness))
		{
			this.timeSinceLoudnessChange = 0f;
			this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * Time.deltaTime));
			this.lastLoudness = this.loudness;
			return;
		}
		if (this.timeSinceLoudnessChange > this.loudnessUpdateCheckRate)
		{
			this.smoothedLoudness = 0.001f;
			return;
		}
		this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * Time.deltaTime));
		this.timeSinceLoudnessChange += Time.deltaTime;
	}

	private bool isSpeaking;

	private float loudness;

	private bool isMicEnabled;

	private RigContainer rigContainer;

	private Speaker speaker;

	private SpeakerVoiceToLoudness speakerVoiceToLoudness;

	private Recorder recorder;

	private VoiceToLoudness voiceToLoudness;

	private float smoothedLoudness;

	private float lastLoudness;

	private float timeSinceLoudnessChange;

	private float loudnessUpdateCheckRate = 0.2f;

	private float loudnessBlendStrength = 2f;

	private bool permission;

	private bool micConnected;
}
