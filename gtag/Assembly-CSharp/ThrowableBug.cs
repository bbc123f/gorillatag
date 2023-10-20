using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000237 RID: 567
public class ThrowableBug : TransferrableObject
{
	// Token: 0x06000E06 RID: 3590 RVA: 0x0005159C File Offset: 0x0004F79C
	public new void Start()
	{
		float f = Random.Range(0f, 6.2831855f);
		this.targetVelocity = new Vector3(Mathf.Sin(f) * this.maxNaturalSpeed, 0f, Mathf.Cos(f) * this.maxNaturalSpeed);
		this.currentState = TransferrableObject.PositionState.Dropped;
		this.rayCastNonAllocColliders = new RaycastHit[5];
		this.rayCastNonAllocColliders2 = new RaycastHit[5];
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x00051606 File Offset: 0x0004F806
	public override bool ShouldBeKinematic()
	{
		return true;
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x0005160C File Offset: 0x0004F80C
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

	// Token: 0x06000E09 RID: 3593 RVA: 0x000517BC File Offset: 0x0004F9BC
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

	// Token: 0x06000E0A RID: 3594 RVA: 0x00051CE0 File Offset: 0x0004FEE0
	private float RandomizeBobingFrequency()
	{
		return Random.Range(this.minRandFrequency, this.maxRandFrequency);
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00051CF4 File Offset: 0x0004FEF4
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

	// Token: 0x06000E0C RID: 3596 RVA: 0x00051D7E File Offset: 0x0004FF7E
	public void OnCollisionEnter(Collision collision)
	{
		this.reliableState.travelingDirection *= -1f;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x00051D9C File Offset: 0x0004FF9C
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

	// Token: 0x040010F9 RID: 4345
	public ThrowableBugReliableState reliableState;

	// Token: 0x040010FA RID: 4346
	public float slowingDownProgress;

	// Token: 0x040010FB RID: 4347
	public float startingSpeed;

	// Token: 0x040010FC RID: 4348
	public float bobingSpeed = 1f;

	// Token: 0x040010FD RID: 4349
	public float bobMagnintude = 0.1f;

	// Token: 0x040010FE RID: 4350
	public bool shouldRandomizeFrequency;

	// Token: 0x040010FF RID: 4351
	public float minRandFrequency = 0.008f;

	// Token: 0x04001100 RID: 4352
	public float maxRandFrequency = 1f;

	// Token: 0x04001101 RID: 4353
	public float bobingFrequency = 1f;

	// Token: 0x04001102 RID: 4354
	public float bobingState;

	// Token: 0x04001103 RID: 4355
	public float thrownYVelocity;

	// Token: 0x04001104 RID: 4356
	public float collisionHitRadius;

	// Token: 0x04001105 RID: 4357
	public LayerMask collisionCheckMask;

	// Token: 0x04001106 RID: 4358
	public Vector3 thrownVeloicity;

	// Token: 0x04001107 RID: 4359
	public Vector3 targetVelocity;

	// Token: 0x04001108 RID: 4360
	public Quaternion bugRotationalVelocity;

	// Token: 0x04001109 RID: 4361
	private RaycastHit[] rayCastNonAllocColliders;

	// Token: 0x0400110A RID: 4362
	private RaycastHit[] rayCastNonAllocColliders2;

	// Token: 0x0400110B RID: 4363
	public VRRig followingRig;

	// Token: 0x0400110C RID: 4364
	public bool isTooHighTravelingDown;

	// Token: 0x0400110D RID: 4365
	public float descentSlerp;

	// Token: 0x0400110E RID: 4366
	public float ascentSlerp;

	// Token: 0x0400110F RID: 4367
	public float maxNaturalSpeed;

	// Token: 0x04001110 RID: 4368
	public float slowdownAcceleration;

	// Token: 0x04001111 RID: 4369
	public float maximumHeightOffOfTheGroundBeforeStartingDescent = 5f;

	// Token: 0x04001112 RID: 4370
	public float minimumHeightOffOfTheGroundBeforeStoppingDescent = 3f;

	// Token: 0x04001113 RID: 4371
	public float descentRate = 0.2f;

	// Token: 0x04001114 RID: 4372
	public float descentSlerpRate = 0.2f;

	// Token: 0x04001115 RID: 4373
	public float minimumHeightOffOfTheGroundBeforeStartingAscent = 0.5f;

	// Token: 0x04001116 RID: 4374
	public float maximumHeightOffOfTheGroundBeforeStoppingAscent = 0.75f;

	// Token: 0x04001117 RID: 4375
	public float ascentRate = 0.4f;

	// Token: 0x04001118 RID: 4376
	public float ascentSlerpRate = 1f;

	// Token: 0x04001119 RID: 4377
	private bool isTooLowTravelingUp;

	// Token: 0x0400111A RID: 4378
	public Animator animator;

	// Token: 0x0400111B RID: 4379
	[FormerlySerializedAs("grabBugAudioSource")]
	public AudioClip grabBugAudioClip;

	// Token: 0x0400111C RID: 4380
	[FormerlySerializedAs("releaseBugAudioSource")]
	public AudioClip releaseBugAudioClip;

	// Token: 0x0400111D RID: 4381
	[FormerlySerializedAs("flyingBugAudioSource")]
	public AudioClip flyingBugAudioClip;

	// Token: 0x0400111E RID: 4382
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400111F RID: 4383
	private float bobbingDefaultFrequency = 1f;

	// Token: 0x04001120 RID: 4384
	public int updateMultiplier;

	// Token: 0x04001121 RID: 4385
	private float velocity;

	// Token: 0x04001122 RID: 4386
	private bool grabAudioPlayed;

	// Token: 0x04001123 RID: 4387
	private ThrowableBug.AudioState currentAudioState;

	// Token: 0x02000482 RID: 1154
	private enum AudioState
	{
		// Token: 0x04001EC6 RID: 7878
		JustGrabbed,
		// Token: 0x04001EC7 RID: 7879
		ContinuallyGrabbed,
		// Token: 0x04001EC8 RID: 7880
		JustReleased,
		// Token: 0x04001EC9 RID: 7881
		NotHeld
	}
}
