using System;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class AmbientSoundRandomizer : MonoBehaviour
{
	// Token: 0x06000D2E RID: 3374 RVA: 0x0004D7C7 File Offset: 0x0004B9C7
	private void Button_Cache()
	{
		this.audioSources = base.GetComponentsInChildren<AudioSource>();
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x0004D7D5 File Offset: 0x0004B9D5
	private void Awake()
	{
		this.SetTarget();
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x0004D7E0 File Offset: 0x0004B9E0
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

	// Token: 0x06000D31 RID: 3377 RVA: 0x0004D854 File Offset: 0x0004BA54
	private void SetTarget()
	{
		this.timerTarget = this.baseTime + Random.Range(0f, this.randomModifier);
		this.timer = 0f;
	}

	// Token: 0x04001059 RID: 4185
	[SerializeField]
	private AudioSource[] audioSources;

	// Token: 0x0400105A RID: 4186
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x0400105B RID: 4187
	[SerializeField]
	private float baseTime = 15f;

	// Token: 0x0400105C RID: 4188
	[SerializeField]
	private float randomModifier = 5f;

	// Token: 0x0400105D RID: 4189
	private float timer;

	// Token: 0x0400105E RID: 4190
	private float timerTarget;
}
