using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class SoundOnCollisionTagSpecific : MonoBehaviour
{
	// Token: 0x06000238 RID: 568 RVA: 0x0000F250 File Offset: 0x0000D450
	private void OnTriggerEnter(Collider collider)
	{
		if (Time.time > this.nextSound && collider.gameObject.CompareTag(this.tagName))
		{
			this.nextSound = Time.time + this.noiseCooldown;
			this.audioSource.PlayOneShot(this.collisionSounds[Random.Range(0, this.collisionSounds.Length)], 0.5f);
		}
	}

	// Token: 0x040002E1 RID: 737
	public string tagName;

	// Token: 0x040002E2 RID: 738
	public float noiseCooldown = 1f;

	// Token: 0x040002E3 RID: 739
	private float nextSound;

	// Token: 0x040002E4 RID: 740
	public AudioSource audioSource;

	// Token: 0x040002E5 RID: 741
	public AudioClip[] collisionSounds;
}
