using System;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class HeightVolume : MonoBehaviour
{
	// Token: 0x06000A50 RID: 2640 RVA: 0x000402A4 File Offset: 0x0003E4A4
	private void Awake()
	{
		if (this.targetTransform == null)
		{
			this.targetTransform = Camera.main.transform;
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x000402C4 File Offset: 0x0003E4C4
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

	// Token: 0x04000CF1 RID: 3313
	public Transform heightTop;

	// Token: 0x04000CF2 RID: 3314
	public Transform heightBottom;

	// Token: 0x04000CF3 RID: 3315
	public AudioSource audioSource;

	// Token: 0x04000CF4 RID: 3316
	public float baseVolume;

	// Token: 0x04000CF5 RID: 3317
	public float minVolume;

	// Token: 0x04000CF6 RID: 3318
	public Transform targetTransform;

	// Token: 0x04000CF7 RID: 3319
	public bool invertHeightVol;
}
