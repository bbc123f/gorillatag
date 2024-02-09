﻿using System;
using GorillaNetworking;
using GorillaTag.Audio;
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

	public float Louddess
	{
		get
		{
			return this.loudness;
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
		this.UpdateLoudness();
		this.UpdateSmoothedLoudness();
	}

	private void UpdateLoudness()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		if (this.voiceView == null)
		{
			this.voiceView = this.rigContainer.Voice;
		}
		if (this.voiceView != null && this.speaker == null)
		{
			this.speaker = this.voiceView.SpeakerInUse;
		}
		if (this.recorder == null)
		{
			this.recorder = PhotonNetworkController.Instance.GetComponent<Recorder>();
		}
		if (this.voiceView != null && this.voiceView.IsSpeaking)
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
		else if (this.rigContainer.photonView != null && this.rigContainer.photonView.IsMine && this.recorder != null && this.recorder.IsCurrentlyTransmitting)
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

	private RigContainer rigContainer;

	private PhotonVoiceView voiceView;

	private Speaker speaker;

	private SpeakerVoiceToLoudness speakerVoiceToLoudness;

	private Recorder recorder;

	private VoiceToLoudness voiceToLoudness;

	private float smoothedLoudness;

	private float lastLoudness;

	private float timeSinceLoudnessChange;

	private float loudnessUpdateCheckRate = 0.2f;

	private float loudnessBlendStrength = 2f;
}