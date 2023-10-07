using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020001A1 RID: 417
public class PlayerAudioManager : MonoBehaviour
{
	// Token: 0x06000ABF RID: 2751 RVA: 0x0004230C File Offset: 0x0004050C
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.1f)
	{
		snapshot.TransitionTo(transitionTime);
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00042315 File Offset: 0x00040515
	public void UnsetMixerSnapshot(float transitionTime = 0.1f)
	{
		this.defaultSnapshot.TransitionTo(transitionTime);
	}

	// Token: 0x04000D87 RID: 3463
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04000D88 RID: 3464
	public AudioMixerSnapshot underwaterSnapshot;
}
