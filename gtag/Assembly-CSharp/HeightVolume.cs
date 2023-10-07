using System;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class HeightVolume : MonoBehaviour
{
	// Token: 0x06000A4B RID: 2635 RVA: 0x00040174 File Offset: 0x0003E374
	private void Awake()
	{
		if (this.targetTransform == null)
		{
			this.targetTransform = Camera.main.transform;
		}
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x00040194 File Offset: 0x0003E394
	private void Update()
	{
		if (this.audioSource.gameObject.activeSelf)
		{
			if (this.targetTransform.position.y > this.heightTop.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.baseVolume : this.minVolume);
				return;
			}
			if (this.targetTransform.position.y < this.heightBottom.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.minVolume : this.baseVolume);
				return;
			}
			this.audioSource.volume = ((!this.invertHeightVol) ? ((this.targetTransform.position.y - this.heightBottom.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume) : ((this.heightTop.position.y - this.targetTransform.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume));
		}
	}

	// Token: 0x04000CED RID: 3309
	public Transform heightTop;

	// Token: 0x04000CEE RID: 3310
	public Transform heightBottom;

	// Token: 0x04000CEF RID: 3311
	public AudioSource audioSource;

	// Token: 0x04000CF0 RID: 3312
	public float baseVolume;

	// Token: 0x04000CF1 RID: 3313
	public float minVolume;

	// Token: 0x04000CF2 RID: 3314
	public Transform targetTransform;

	// Token: 0x04000CF3 RID: 3315
	public bool invertHeightVol;
}
