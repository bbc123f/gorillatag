using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class SpitballEvents : SubEmitterListener
{
	// Token: 0x060007DA RID: 2010 RVA: 0x00031843 File Offset: 0x0002FA43
	protected override void OnSubEmit()
	{
		base.OnSubEmit();
		if (this._audioSource && this._sfxHit)
		{
			this._audioSource.PlayOneShot(this._sfxHit);
		}
	}

	// Token: 0x0400096F RID: 2415
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x04000970 RID: 2416
	[SerializeField]
	private AudioClip _sfxHit;
}
