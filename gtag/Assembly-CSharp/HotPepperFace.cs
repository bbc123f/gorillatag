using System;
using UnityEngine;

// Token: 0x0200012B RID: 299
public class HotPepperFace : MonoBehaviour
{
	// Token: 0x060007D7 RID: 2007 RVA: 0x000315E4 File Offset: 0x0002F7E4
	public void PlayFX(float delay)
	{
		if (delay < 0f)
		{
			this.PlayFX();
			return;
		}
		base.Invoke("PlayFX", delay);
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00031601 File Offset: 0x0002F801
	public void PlayFX()
	{
		this._faceMesh.SetActive(true);
		this._fireFX.Play();
		this._flameSpeaker.Play();
		this._breathSpeaker.Play();
		base.Invoke("StopFX", this._effectLength);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x00031641 File Offset: 0x0002F841
	public void StopFX()
	{
		this._faceMesh.SetActive(false);
		this._fireFX.Stop();
		this._flameSpeaker.Stop();
		this._breathSpeaker.Stop();
	}

	// Token: 0x0400096A RID: 2410
	[SerializeField]
	private GameObject _faceMesh;

	// Token: 0x0400096B RID: 2411
	[SerializeField]
	private ParticleSystem _fireFX;

	// Token: 0x0400096C RID: 2412
	[SerializeField]
	private AudioSource _flameSpeaker;

	// Token: 0x0400096D RID: 2413
	[SerializeField]
	private AudioSource _breathSpeaker;

	// Token: 0x0400096E RID: 2414
	[SerializeField]
	private float _effectLength = 1.5f;
}
