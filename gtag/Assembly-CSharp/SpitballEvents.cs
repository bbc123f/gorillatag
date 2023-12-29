using System;
using UnityEngine;

public class SpitballEvents : SubEmitterListener
{
	protected override void OnSubEmit()
	{
		base.OnSubEmit();
		if (this._audioSource && this._sfxHit)
		{
			this._audioSource.PlayOneShot(this._sfxHit);
		}
	}

	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private AudioClip _sfxHit;
}
