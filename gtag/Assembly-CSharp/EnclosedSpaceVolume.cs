using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class EnclosedSpaceVolume : GorillaTriggerBox
{
	// Token: 0x0600088F RID: 2191 RVA: 0x00034D0E File Offset: 0x00032F0E
	private void Awake()
	{
		this.audioSourceInside.volume = this.quietVolume;
		this.audioSourceOutside.volume = this.loudVolume;
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x00034D32 File Offset: 0x00032F32
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<Player>() != null)
		{
			this.audioSourceInside.volume = this.loudVolume;
			this.audioSourceOutside.volume = this.quietVolume;
		}
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x00034D69 File Offset: 0x00032F69
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<Player>() != null)
		{
			this.audioSourceInside.volume = this.quietVolume;
			this.audioSourceOutside.volume = this.loudVolume;
		}
	}

	// Token: 0x04000AC4 RID: 2756
	public AudioSource audioSourceInside;

	// Token: 0x04000AC5 RID: 2757
	public AudioSource audioSourceOutside;

	// Token: 0x04000AC6 RID: 2758
	public float loudVolume;

	// Token: 0x04000AC7 RID: 2759
	public float quietVolume;
}
