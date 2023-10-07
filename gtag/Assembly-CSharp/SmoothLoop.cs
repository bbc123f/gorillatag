using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class SmoothLoop : MonoBehaviour
{
	// Token: 0x060007B6 RID: 1974 RVA: 0x00031144 File Offset: 0x0002F344
	private void Start()
	{
		if (this.delay != 0f && !this.randomStart)
		{
			this.source.Stop();
			base.StartCoroutine(this.DelayedStart());
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.Play();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x000311C4 File Offset: 0x0002F3C4
	private void Update()
	{
		if (this.source.time > this.source.clip.length * 0.95f)
		{
			this.source.time = 0.1f;
		}
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x000311FC File Offset: 0x0002F3FC
	private void OnEnable()
	{
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.Play();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0003124E File Offset: 0x0002F44E
	public IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(this.delay);
		this.source.Play();
		yield break;
	}

	// Token: 0x04000952 RID: 2386
	public AudioSource source;

	// Token: 0x04000953 RID: 2387
	public float delay;

	// Token: 0x04000954 RID: 2388
	public bool randomStart;
}
