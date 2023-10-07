using System;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class AmbientSoundRandomizer : MonoBehaviour
{
	// Token: 0x06000D28 RID: 3368 RVA: 0x0004D567 File Offset: 0x0004B767
	private void Button_Cache()
	{
		this.audioSources = base.GetComponentsInChildren<AudioSource>();
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x0004D575 File Offset: 0x0004B775
	private void Awake()
	{
		this.SetTarget();
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x0004D580 File Offset: 0x0004B780
	private void Update()
	{
		if (this.timer >= this.timerTarget)
		{
			int num = Random.Range(0, this.audioSources.Length);
			int num2 = Random.Range(0, this.audioClips.Length);
			this.audioSources[num].clip = this.audioClips[num2];
			this.audioSources[num].Play();
			this.SetTarget();
			return;
		}
		this.timer += Time.deltaTime;
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x0004D5F4 File Offset: 0x0004B7F4
	private void SetTarget()
	{
		this.timerTarget = this.baseTime + Random.Range(0f, this.randomModifier);
		this.timer = 0f;
	}

	// Token: 0x04001054 RID: 4180
	[SerializeField]
	private AudioSource[] audioSources;

	// Token: 0x04001055 RID: 4181
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04001056 RID: 4182
	[SerializeField]
	private float baseTime = 15f;

	// Token: 0x04001057 RID: 4183
	[SerializeField]
	private float randomModifier = 5f;

	// Token: 0x04001058 RID: 4184
	private float timer;

	// Token: 0x04001059 RID: 4185
	private float timerTarget;
}
