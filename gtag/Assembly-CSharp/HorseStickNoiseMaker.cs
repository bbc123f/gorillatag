using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class HorseStickNoiseMaker : MonoBehaviour
{
	// Token: 0x0600015D RID: 349 RVA: 0x0000B794 File Offset: 0x00009994
	protected void OnEnable()
	{
		if (this.gorillaPlayerXform == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.oldPos = this.gorillaPlayerXform.position;
		this.distElapsed = 0f;
		this.timeSincePlay = 0f;
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000B7E4 File Offset: 0x000099E4
	protected void LateUpdate()
	{
		Vector3 position = this.gorillaPlayerXform.position;
		Vector3 vector = position - this.oldPos;
		this.distElapsed += vector.magnitude;
		this.timeSincePlay += Time.deltaTime;
		this.oldPos = position;
		if (this.distElapsed >= this.metersPerClip && this.timeSincePlay >= this.minSecBetweenClips)
		{
			this.soundBankPlayer.Play(null, null);
			this.distElapsed = 0f;
			this.timeSincePlay = 0f;
			if (this.particleFX != null)
			{
				this.particleFX.Play();
			}
		}
	}

	// Token: 0x040001EC RID: 492
	[Tooltip("Meters the object should traverse between playing a provided audio clip.")]
	public float metersPerClip = 4f;

	// Token: 0x040001ED RID: 493
	[Tooltip("Number of seconds that must elapse before playing another audio clip.")]
	public float minSecBetweenClips = 1.5f;

	// Token: 0x040001EE RID: 494
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x040001EF RID: 495
	[Tooltip("Transform assigned in Gorilla Player Networked Prefab to the Gorilla Player Networked parent to keep track of distance traveled.")]
	public Transform gorillaPlayerXform;

	// Token: 0x040001F0 RID: 496
	[Tooltip("Optional particle FX to spawn when sound plays")]
	public ParticleSystem particleFX;

	// Token: 0x040001F1 RID: 497
	private Vector3 oldPos;

	// Token: 0x040001F2 RID: 498
	private float timeSincePlay;

	// Token: 0x040001F3 RID: 499
	private float distElapsed;
}
