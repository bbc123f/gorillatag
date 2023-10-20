using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class RotationSoundPlayer : MonoBehaviour
{
	// Token: 0x06000151 RID: 337 RVA: 0x0000B374 File Offset: 0x00009574
	private void Awake()
	{
		List<Transform> list = new List<Transform>(this.transforms);
		list.RemoveAll((Transform xform) => xform == null);
		this.transforms = list.ToArray();
		this.initialUpAxis = new Vector3[this.transforms.Length];
		this.lastUpAxis = new Vector3[this.transforms.Length];
		this.lastRotationSpeeds = new float[this.transforms.Length];
		for (int i = 0; i < this.transforms.Length; i++)
		{
			this.initialUpAxis[i] = this.transforms[i].localRotation * Vector3.up;
			this.lastUpAxis[i] = this.initialUpAxis[i];
			this.lastRotationSpeeds[i] = 0f;
		}
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000B454 File Offset: 0x00009654
	private void Update()
	{
		this.cooldownTimer -= Time.deltaTime;
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Vector3 vector = this.transforms[i].localRotation * Vector3.up;
			float num = Vector3.Angle(vector, this.initialUpAxis[i]);
			float num2 = Vector3.Angle(vector, this.lastUpAxis[i]);
			float deltaTime = Time.deltaTime;
			float num3 = num2 / deltaTime;
			if (this.cooldownTimer <= 0f && num > this.rotationAmountThreshold && num3 > this.rotationSpeedThreshold && !this.soundBankPlayer.isPlaying)
			{
				this.cooldownTimer = this.cooldown;
				this.soundBankPlayer.Play(null, null);
			}
			this.lastUpAxis[i] = vector;
			this.lastRotationSpeeds[i] = num3;
		}
	}

	// Token: 0x040001DC RID: 476
	[Tooltip("Transforms that will make a noise when they rotate.")]
	[SerializeField]
	private Transform[] transforms;

	// Token: 0x040001DD RID: 477
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x040001DE RID: 478
	[Tooltip("How much the transform must rotate from it's initial rotation before a sound is played.")]
	private float rotationAmountThreshold = 30f;

	// Token: 0x040001DF RID: 479
	[Tooltip("How fast the transform must rotate before a sound is played.")]
	private float rotationSpeedThreshold = 45f;

	// Token: 0x040001E0 RID: 480
	private float cooldown = 0.6f;

	// Token: 0x040001E1 RID: 481
	private float cooldownTimer;

	// Token: 0x040001E2 RID: 482
	private Vector3[] initialUpAxis;

	// Token: 0x040001E3 RID: 483
	private Vector3[] lastUpAxis;

	// Token: 0x040001E4 RID: 484
	private float[] lastRotationSpeeds;
}
