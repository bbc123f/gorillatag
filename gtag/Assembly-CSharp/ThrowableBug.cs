using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000236 RID: 566
public class ThrowableBug : TransferrableObject
{
	// Token: 0x06000DFF RID: 3583 RVA: 0x000511C0 File Offset: 0x0004F3C0
	public new void Start()
	{
		float f = Random.Range(0f, 6.2831855f);
		this.targetVelocity = new Vector3(Mathf.Sin(f) * this.maxNaturalSpeed, 0f, Mathf.Cos(f) * this.maxNaturalSpeed);
		this.currentState = TransferrableObject.PositionState.Dropped;
		this.rayCastNonAllocColliders = new RaycastHit[5];
		this.rayCastNonAllocColliders2 = new RaycastHit[5];
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x0005122A File Offset: 0x0004F42A
	public override bool ShouldBeKinematic()
	{
		return true;
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00051230 File Offset: 0x0004F430
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
		this.animator.SetBool("isHeld", flag);
		if (!this.audioSource)
		{
			return;
		}
		switch (this.currentAudioState)
		{
		case ThrowableBug.AudioState.JustGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			if (this.grabBugAudioClip && this.audioSource.clip != this.grabBugAudioClip)
			{
				this.audioSource.clip = this.grabBugAudioClip;
				this.audioSource.time = 0f;
				this.audioSource.Play();
				return;
			}
			if (!this.audioSource.isPlaying)
			{
				this.currentAudioState = ThrowableBug.AudioState.ContinuallyGrabbed;
				return;
			}
			break;
		case ThrowableBug.AudioState.ContinuallyGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			break;
		case ThrowableBug.AudioState.JustReleased:
			if (!flag)
			{
				if (this.releaseBugAudioClip && this.audioSource.clip != this.releaseBugAudioClip)
				{
					this.audioSource.clip = this.releaseBugAudioClip;
					this.audioSource.time = 0f;
					this.audioSource.Play();
					return;
				}
				if (!this.audioSource.isPlaying)
				{
					this.currentAudioState = ThrowableBug.AudioState.NotHeld;
					return;
				}
			}
			else
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
			}
			break;
		case ThrowableBug.AudioState.NotHeld:
			if (flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
				return;
			}
			if (this.flyingBugAudioClip && !this.audioSource.isPlaying)
			{
				this.audioSource.clip = this.flyingBugAudioClip;
				this.audioSource.time = 0f;
				this.audioSource.Play();
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x000513E0 File Offset: 0x0004F5E0
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this.reliableState)
		{
			return;
		}
		if (this.currentState != TransferrableObject.PositionState.InLeftHand)
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InRightHand;
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			if (this.slowingDownProgress < 1f)
			{
				this.slowingDownProgress += this.slowdownAcceleration * Time.deltaTime;
				float t = Mathf.SmoothStep(0f, 1f, this.slowingDownProgress);
				this.reliableState.travelingDirection = Vector3.Slerp(this.thrownVeloicity, this.targetVelocity, t);
			}
			else
			{
				this.reliableState.travelingDirection = this.reliableState.travelingDirection.normalized * this.maxNaturalSpeed;
			}
			this.bobingFrequency = (this.shouldRandomizeFrequency ? this.RandomizeBobingFrequency() : this.bobbingDefaultFrequency);
			float num = this.bobingState + this.bobingSpeed * Time.deltaTime;
			float num2 = Mathf.Sin(num / this.bobingFrequency) - Mathf.Sin(this.bobingState / this.bobingFrequency);
			Vector3 vector = Vector3.up * (num2 * this.bobMagnintude);
			this.bobingState = num;
			if (this.bobingState > 6.2831855f)
			{
				this.bobingState -= 6.2831855f;
			}
			vector += this.reliableState.travelingDirection * Time.deltaTime;
			Debug.DrawLine(base.transform.position, base.transform.position + vector, Color.red, 5f, false);
			int num3 = Physics.SphereCastNonAlloc(base.transform.position, this.collisionHitRadius, vector.normalized, this.rayCastNonAllocColliders, vector.magnitude, this.collisionCheckMask);
			float maxDistance = this.maximumHeightOffOfTheGroundBeforeStartingDescent;
			if (this.isTooHighTravelingDown)
			{
				maxDistance = this.minimumHeightOffOfTheGroundBeforeStoppingDescent;
			}
			float num4 = this.minimumHeightOffOfTheGroundBeforeStartingAscent;
			if (this.isTooLowTravelingUp)
			{
				num4 = this.maximumHeightOffOfTheGroundBeforeStoppingAscent;
			}
			if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, this.rayCastNonAllocColliders2, maxDistance, this.collisionCheckMask) > 0)
			{
				this.isTooHighTravelingDown = false;
				if (this.descentSlerp > 0f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp - this.descentSlerpRate * Time.deltaTime);
				}
				RaycastHit raycastHit = this.rayCastNonAllocColliders2[0];
				this.isTooLowTravelingUp = (raycastHit.distance < num4);
				if (this.isTooLowTravelingUp)
				{
					if (this.ascentSlerp < 1f)
					{
						this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp + this.ascentSlerpRate * Time.deltaTime);
					}
				}
				else if (this.ascentSlerp > 0f)
				{
					this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp - this.ascentSlerpRate * Time.deltaTime);
				}
			}
			else
			{
				this.isTooHighTravelingDown = true;
				if (this.descentSlerp < 1f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp + this.descentSlerpRate * Time.deltaTime);
				}
			}
			vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.descentSlerp) * this.descentRate * Vector3.down;
			vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.ascentSlerp) * this.ascentRate * Vector3.up;
			float num5;
			Vector3 axis;
			Quaternion.FromToRotation(base.transform.rotation * Vector3.up, Quaternion.identity * Vector3.up).ToAngleAxis(out num5, out axis);
			Quaternion quaternion = Quaternion.AngleAxis(num5 * 0.02f, axis);
			float num6;
			Vector3 axis2;
			Quaternion.FromToRotation(base.transform.rotation * Vector3.forward, this.reliableState.travelingDirection.normalized).ToAngleAxis(out num6, out axis2);
			Quaternion lhs = Quaternion.AngleAxis(num6 * 0.005f, axis2);
			quaternion = lhs * quaternion;
			vector = quaternion * quaternion * quaternion * quaternion * vector;
			if (num3 > 0)
			{
				Vector3 normal = this.rayCastNonAllocColliders[0].normal;
				this.reliableState.travelingDirection = Vector3.Reflect(this.reliableState.travelingDirection, normal).x0z();
				base.transform.position += Vector3.Reflect(vector, normal);
				this.thrownVeloicity = Vector3.Reflect(this.thrownVeloicity, normal);
				this.targetVelocity = Vector3.Reflect(this.targetVelocity, normal).x0z();
			}
			else
			{
				base.transform.position += vector;
			}
			this.bugRotationalVelocity = quaternion * this.bugRotationalVelocity;
			float num7;
			Vector3 axis3;
			this.bugRotationalVelocity.ToAngleAxis(out num7, out axis3);
			this.bugRotationalVelocity = Quaternion.AngleAxis(num7 * 0.9f, axis3);
			base.transform.rotation = this.bugRotationalVelocity * base.transform.rotation;
		}
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00051904 File Offset: 0x0004FB04
	private float RandomizeBobingFrequency()
	{
		return Random.Range(this.minRandFrequency, this.maxRandFrequency);
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x00051918 File Offset: 0x0004FB18
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		this.slowingDownProgress = 0f;
		GorillaVelocityEstimator component = base.GetComponent<GorillaVelocityEstimator>();
		Vector3 linearVelocity = component.linearVelocity;
		this.thrownVeloicity = linearVelocity;
		this.reliableState.travelingDirection = linearVelocity;
		this.bugRotationalVelocity = Quaternion.Euler(component.angularVelocity);
		this.startingSpeed = linearVelocity.magnitude;
		Vector3 normalized = this.reliableState.travelingDirection.x0z().normalized;
		this.targetVelocity = normalized * this.maxNaturalSpeed;
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x000519A2 File Offset: 0x0004FBA2
	public void OnCollisionEnter(Collision collision)
	{
		this.reliableState.travelingDirection *= -1f;
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x000519C0 File Offset: 0x0004FBC0
	private void Update()
	{
		if (this.updateMultiplier > 0)
		{
			for (int i = 0; i < this.updateMultiplier; i++)
			{
				this.LateUpdateLocal();
			}
		}
	}

	// Token: 0x040010F3 RID: 4339
	public ThrowableBugReliableState reliableState;

	// Token: 0x040010F4 RID: 4340
	public float slowingDownProgress;

	// Token: 0x040010F5 RID: 4341
	public float startingSpeed;

	// Token: 0x040010F6 RID: 4342
	public float bobingSpeed = 1f;

	// Token: 0x040010F7 RID: 4343
	public float bobMagnintude = 0.1f;

	// Token: 0x040010F8 RID: 4344
	public bool shouldRandomizeFrequency;

	// Token: 0x040010F9 RID: 4345
	public float minRandFrequency = 0.008f;

	// Token: 0x040010FA RID: 4346
	public float maxRandFrequency = 1f;

	// Token: 0x040010FB RID: 4347
	public float bobingFrequency = 1f;

	// Token: 0x040010FC RID: 4348
	public float bobingState;

	// Token: 0x040010FD RID: 4349
	public float thrownYVelocity;

	// Token: 0x040010FE RID: 4350
	public float collisionHitRadius;

	// Token: 0x040010FF RID: 4351
	public LayerMask collisionCheckMask;

	// Token: 0x04001100 RID: 4352
	public Vector3 thrownVeloicity;

	// Token: 0x04001101 RID: 4353
	public Vector3 targetVelocity;

	// Token: 0x04001102 RID: 4354
	public Quaternion bugRotationalVelocity;

	// Token: 0x04001103 RID: 4355
	private RaycastHit[] rayCastNonAllocColliders;

	// Token: 0x04001104 RID: 4356
	private RaycastHit[] rayCastNonAllocColliders2;

	// Token: 0x04001105 RID: 4357
	public VRRig followingRig;

	// Token: 0x04001106 RID: 4358
	public bool isTooHighTravelingDown;

	// Token: 0x04001107 RID: 4359
	public float descentSlerp;

	// Token: 0x04001108 RID: 4360
	public float ascentSlerp;

	// Token: 0x04001109 RID: 4361
	public float maxNaturalSpeed;

	// Token: 0x0400110A RID: 4362
	public float slowdownAcceleration;

	// Token: 0x0400110B RID: 4363
	public float maximumHeightOffOfTheGroundBeforeStartingDescent = 5f;

	// Token: 0x0400110C RID: 4364
	public float minimumHeightOffOfTheGroundBeforeStoppingDescent = 3f;

	// Token: 0x0400110D RID: 4365
	public float descentRate = 0.2f;

	// Token: 0x0400110E RID: 4366
	public float descentSlerpRate = 0.2f;

	// Token: 0x0400110F RID: 4367
	public float minimumHeightOffOfTheGroundBeforeStartingAscent = 0.5f;

	// Token: 0x04001110 RID: 4368
	public float maximumHeightOffOfTheGroundBeforeStoppingAscent = 0.75f;

	// Token: 0x04001111 RID: 4369
	public float ascentRate = 0.4f;

	// Token: 0x04001112 RID: 4370
	public float ascentSlerpRate = 1f;

	// Token: 0x04001113 RID: 4371
	private bool isTooLowTravelingUp;

	// Token: 0x04001114 RID: 4372
	public Animator animator;

	// Token: 0x04001115 RID: 4373
	[FormerlySerializedAs("grabBugAudioSource")]
	public AudioClip grabBugAudioClip;

	// Token: 0x04001116 RID: 4374
	[FormerlySerializedAs("releaseBugAudioSource")]
	public AudioClip releaseBugAudioClip;

	// Token: 0x04001117 RID: 4375
	[FormerlySerializedAs("flyingBugAudioSource")]
	public AudioClip flyingBugAudioClip;

	// Token: 0x04001118 RID: 4376
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001119 RID: 4377
	private float bobbingDefaultFrequency = 1f;

	// Token: 0x0400111A RID: 4378
	public int updateMultiplier;

	// Token: 0x0400111B RID: 4379
	private float velocity;

	// Token: 0x0400111C RID: 4380
	private bool grabAudioPlayed;

	// Token: 0x0400111D RID: 4381
	private ThrowableBug.AudioState currentAudioState;

	// Token: 0x02000480 RID: 1152
	private enum AudioState
	{
		// Token: 0x04001EB9 RID: 7865
		JustGrabbed,
		// Token: 0x04001EBA RID: 7866
		ContinuallyGrabbed,
		// Token: 0x04001EBB RID: 7867
		JustReleased,
		// Token: 0x04001EBC RID: 7868
		NotHeld
	}
}
