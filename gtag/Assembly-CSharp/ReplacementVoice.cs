using System;
using UnityEngine;

public class ReplacementVoice : MonoBehaviour
{
	public void InvokeUpdate()
	{
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.ShouldPlayReplacementVoice())
		{
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < this.loudReplacementVoiceThreshold)
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClips[Random.Range(0, this.replacementVoiceClips.Length - 1)];
				this.replacementVoiceSource.volume = this.normalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClipsLoud[Random.Range(0, this.replacementVoiceClipsLoud.Length - 1)];
				this.replacementVoiceSource.volume = this.loudVolume;
			}
			this.replacementVoiceSource.Play();
		}
	}

	public ReplacementVoice()
	{
	}

	public AudioSource replacementVoiceSource;

	public AudioClip[] replacementVoiceClips;

	public AudioClip[] replacementVoiceClipsLoud;

	public float loudReplacementVoiceThreshold = 0.1f;

	public VRRig myVRRig;

	public float normalVolume = 0.5f;

	public float loudVolume = 0.8f;
}
