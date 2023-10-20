using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020001A2 RID: 418
public class PlayerAudioManager : MonoBehaviour
{
	// Token: 0x06000AC4 RID: 2756 RVA: 0x00042444 File Offset: 0x00040644
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.1f)
	{
		snapshot.TransitionTo(transitionTime);
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x0004244D File Offset: 0x0004064D
	public void UnsetMixerSnapshot(float transitionTime = 0.1f)
	{
		this.defaultSnapshot.TransitionTo(transitionTime);
	}

	// Token: 0x04000D8B RID: 3467
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04000D8C RID: 3468
	public AudioMixerSnapshot underwaterSnapshot;
}
