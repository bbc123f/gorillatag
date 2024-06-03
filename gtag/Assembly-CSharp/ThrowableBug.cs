using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

public class ThrowableBug : TransferrableObject
{
	protected override void Start()
	{
		float f = Random.Range(0f, 6.2831855f);
		this.targetVelocity = new Vector3(Mathf.Sin(f) * this.maxNaturalSpeed, 0f, Mathf.Cos(f) * this.maxNaturalSpeed);
		this.currentState = TransferrableObject.PositionState.Dropped;
		this.rayCastNonAllocColliders = new RaycastHit[5];
		this.rayCastNonAllocColliders2 = new RaycastHit[5];
	}

	public override void OnEnable()
	{
		base.OnEnable();
		ThrowableBugBeacon.OnCall += this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss += this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock += this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock += this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier += this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		ThrowableBugBeacon.OnCall -= this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss -= this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock -= this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock -= this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier -= this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
	}

	private bool isValid(ThrowableBugBeacon tbb)
	{
		return tbb.BugName == this.bugName && (tbb.Range <= 0f || Vector3.Distance(tbb.transform.position, base.transform.position) <= tbb.Range);
	}

	private void ThrowableBugBeacon_OnCall(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
		}
	}

	private void ThrowableBugBeacon_OnLock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
			this.lockedTarget = tbb.transform;
			this.locked = true;
		}
	}

	private void ThrowableBugBeacon_OnDismiss(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = base.transform.position - tbb.transform.position;
			this.locked = false;
		}
	}

	private void ThrowableBugBeacon_OnUnlock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.locked = false;
		}
	}

	private void ThrowableBugBeacon_OnChangeSpeedMultiplier(ThrowableBugBeacon tbb, float f)
	{
		if (this.isValid(tbb))
		{
			this.speedMultiplier = f;
		}
	}

	public override bool ShouldBeKinematic()
	{
		return true;
	}

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
			if (this.locked && Vector3.Distance(this.lockedTarget.position, base.transform.position) > 0.1f)
			{
				this.reliableState.travelingDirection = this.lockedTarget.position - base.transform.position;
			}
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
			vector *= this.speedMultiplier;
			if (this.speedMultiplier != 1f)
			{
				this.speedMultiplier = Mathf.MoveTowards(this.speedMultiplier, 1f, Time.deltaTime);
			}
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

	private float RandomizeBobingFrequency()
	{
		return Random.Range(this.minRandFrequency, this.maxRandFrequency);
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.slowingDownProgress = 0f;
		GorillaVelocityEstimator component = base.GetComponent<GorillaVelocityEstimator>();
		Vector3 linearVelocity = component.linearVelocity;
		this.thrownVeloicity = linearVelocity;
		this.reliableState.travelingDirection = linearVelocity;
		this.bugRotationalVelocity = Quaternion.Euler(component.angularVelocity);
		this.startingSpeed = linearVelocity.magnitude;
		Vector3 normalized = this.reliableState.travelingDirection.x0z().normalized;
		this.targetVelocity = normalized * this.maxNaturalSpeed;
		return true;
	}

	public void OnCollisionEnter(Collision collision)
	{
		this.reliableState.travelingDirection *= -1f;
	}

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

	public ThrowableBug()
	{
	}

	public ThrowableBugReliableState reliableState;

	public float slowingDownProgress;

	public float startingSpeed;

	public float bobingSpeed = 1f;

	public float bobMagnintude = 0.1f;

	public bool shouldRandomizeFrequency;

	public float minRandFrequency = 0.008f;

	public float maxRandFrequency = 1f;

	public float bobingFrequency = 1f;

	public float bobingState;

	public float thrownYVelocity;

	public float collisionHitRadius;

	public LayerMask collisionCheckMask;

	public Vector3 thrownVeloicity;

	public Vector3 targetVelocity;

	public Quaternion bugRotationalVelocity;

	private RaycastHit[] rayCastNonAllocColliders;

	private RaycastHit[] rayCastNonAllocColliders2;

	public VRRig followingRig;

	public bool isTooHighTravelingDown;

	public float descentSlerp;

	public float ascentSlerp;

	public float maxNaturalSpeed;

	public float slowdownAcceleration;

	public float maximumHeightOffOfTheGroundBeforeStartingDescent = 5f;

	public float minimumHeightOffOfTheGroundBeforeStoppingDescent = 3f;

	public float descentRate = 0.2f;

	public float descentSlerpRate = 0.2f;

	public float minimumHeightOffOfTheGroundBeforeStartingAscent = 0.5f;

	public float maximumHeightOffOfTheGroundBeforeStoppingAscent = 0.75f;

	public float ascentRate = 0.4f;

	public float ascentSlerpRate = 1f;

	private bool isTooLowTravelingUp;

	public Animator animator;

	[FormerlySerializedAs("grabBugAudioSource")]
	public AudioClip grabBugAudioClip;

	[FormerlySerializedAs("releaseBugAudioSource")]
	public AudioClip releaseBugAudioClip;

	[FormerlySerializedAs("flyingBugAudioSource")]
	public AudioClip flyingBugAudioClip;

	[SerializeField]
	private AudioSource audioSource;

	private float bobbingDefaultFrequency = 1f;

	public int updateMultiplier;

	private ThrowableBug.AudioState currentAudioState;

	private float speedMultiplier = 1f;

	[SerializeField]
	private ThrowableBug.BugName bugName;

	private Transform lockedTarget;

	private bool locked;

	public enum BugName
	{
		NONE,
		DougTheBug,
		MattTheBat
	}

	private enum AudioState
	{
		JustGrabbed,
		ContinuallyGrabbed,
		JustReleased,
		NotHeld
	}
}
