using System;
using UnityEngine.Audio;

namespace OVR;

[Serializable]
public class MixerSnapshot
{
	public AudioMixerSnapshot snapshot;

	public float transitionTime = 0.25f;
}
