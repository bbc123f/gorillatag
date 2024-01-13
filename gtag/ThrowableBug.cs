using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

public class ThrowableBug : TransferrableObject
{
	private enum AudioState
	{
		JustGrabbed,
		ContinuallyGrabbed,
		JustReleased,
		NotHeld
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

	private float velocity;

	private bool grabAudioPlayed;

	private AudioState currentAudioState;

	public new void Start()
	{
		float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		targetVelocity = new Vector3(Mathf.Sin(f) * maxNaturalSpeed, 0f, Mathf.Cos(f) * maxNaturalSpeed);
		currentState = PositionState.Dropped;
		rayCastNonAllocColliders = new RaycastHit[5];
		rayCastNonAllocColliders2 = new RaycastHit[5];
	}

	public override bool ShouldBeKinematic()
	{
		return true;
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		bool flag = currentState == PositionState.InLeftHand || currentState == PositionState.InRightHand;
		animator.SetBool("isHeld", flag);
		if (!audioSource)
		{
			return;
		}
		switch (currentAudioState)
		{
		case AudioState.NotHeld:
			if (!flag)
			{
				if ((bool)flyingBugAudioClip && !audioSource.isPlaying)
				{
					audioSource.clip = flyingBugAudioClip;
					audioSource.time = 0f;
					audioSource.Play();
				}
			}
			else
			{
				currentAudioState = AudioState.JustGrabbed;
			}
			break;
		case AudioState.JustGrabbed:
			if (flag)
			{
				if ((bool)grabBugAudioClip && audioSource.clip != grabBugAudioClip)
				{
					audioSource.clip = grabBugAudioClip;
					audioSource.time = 0f;
					audioSource.Play();
				}
				else if (!audioSource.isPlaying)
				{
					currentAudioState = AudioState.ContinuallyGrabbed;
				}
			}
			else
			{
				currentAudioState = AudioState.JustReleased;
			}
			break;
		case AudioState.ContinuallyGrabbed:
			if (!flag)
			{
				currentAudioState = AudioState.JustReleased;
			}
			break;
		case AudioState.JustReleased:
			if (!flag)
			{
				if ((bool)releaseBugAudioClip && audioSource.clip != releaseBugAudioClip)
				{
					audioSource.clip = releaseBugAudioClip;
					audioSource.time = 0f;
					audioSource.Play();
				}
				else if (!audioSource.isPlaying)
				{
					currentAudioState = AudioState.NotHeld;
				}
			}
			else
			{
				currentAudioState = AudioState.JustGrabbed;
			}
			break;
		}
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!reliableState)
		{
			return;
		}
		if (currentState == PositionState.InLeftHand)
		{
			_ = 1;
		}
		else
			_ = currentState == PositionState.InRightHand;
		if (currentState != PositionState.Dropped)
		{
			return;
		}
		if (slowingDownProgress < 1f)
		{
			slowingDownProgress += slowdownAcceleration * Time.deltaTime;
			float t = Mathf.SmoothStep(0f, 1f, slowingDownProgress);
			reliableState.travelingDirection = Vector3.Slerp(thrownVeloicity, targetVelocity, t);
		}
		else
		{
			reliableState.travelingDirection = reliableState.travelingDirection.normalized * maxNaturalSpeed;
		}
		bobingFrequency = (shouldRandomizeFrequency ? RandomizeBobingFrequency() : bobbingDefaultFrequency);
		float num = bobingState + bobingSpeed * Time.deltaTime;
		float num2 = Mathf.Sin(num / bobingFrequency) - Mathf.Sin(bobingState / bobingFrequency);
		Vector3 vector = Vector3.up * (num2 * bobMagnintude);
		bobingState = num;
		if (bobingState > (float)Math.PI * 2f)
		{
			bobingState -= (float)Math.PI * 2f;
		}
		vector += reliableState.travelingDirection * Time.deltaTime;
		Debug.DrawLine(base.transform.position, base.transform.position + vector, Color.red, 5f, depthTest: false);
		int num3 = Physics.SphereCastNonAlloc(base.transform.position, collisionHitRadius, vector.normalized, rayCastNonAllocColliders, vector.magnitude, collisionCheckMask);
		float maxDistance = maximumHeightOffOfTheGroundBeforeStartingDescent;
		if (isTooHighTravelingDown)
		{
			maxDistance = minimumHeightOffOfTheGroundBeforeStoppingDescent;
		}
		float num4 = minimumHeightOffOfTheGroundBeforeStartingAscent;
		if (isTooLowTravelingUp)
		{
			num4 = maximumHeightOffOfTheGroundBeforeStoppingAscent;
		}
		if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, rayCastNonAllocColliders2, maxDistance, collisionCheckMask) > 0)
		{
			isTooHighTravelingDown = false;
			if (descentSlerp > 0f)
			{
				descentSlerp = Mathf.Clamp01(descentSlerp - descentSlerpRate * Time.deltaTime);
			}
			RaycastHit raycastHit = rayCastNonAllocColliders2[0];
			isTooLowTravelingUp = raycastHit.distance < num4;
			if (isTooLowTravelingUp)
			{
				if (ascentSlerp < 1f)
				{
					ascentSlerp = Mathf.Clamp01(ascentSlerp + ascentSlerpRate * Time.deltaTime);
				}
			}
			else if (ascentSlerp > 0f)
			{
				ascentSlerp = Mathf.Clamp01(ascentSlerp - ascentSlerpRate * Time.deltaTime);
			}
		}
		else
		{
			isTooHighTravelingDown = true;
			if (descentSlerp < 1f)
			{
				descentSlerp = Mathf.Clamp01(descentSlerp + descentSlerpRate * Time.deltaTime);
			}
		}
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, descentSlerp) * descentRate * Vector3.down;
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, ascentSlerp) * ascentRate * Vector3.up;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.up, Quaternion.identity * Vector3.up).ToAngleAxis(out var angle, out var axis);
		Quaternion quaternion = Quaternion.AngleAxis(angle * 0.02f, axis);
		Quaternion.FromToRotation(base.transform.rotation * Vector3.forward, reliableState.travelingDirection.normalized).ToAngleAxis(out var angle2, out var axis2);
		Quaternion quaternion2 = Quaternion.AngleAxis(angle2 * 0.005f, axis2);
		quaternion = quaternion2 * quaternion;
		vector = quaternion * quaternion * quaternion * quaternion * vector;
		if (num3 > 0)
		{
			Vector3 normal = rayCastNonAllocColliders[0].normal;
			reliableState.travelingDirection = Vector3.Reflect(reliableState.travelingDirection, normal).x0z();
			base.transform.position += Vector3.Reflect(vector, normal);
			thrownVeloicity = Vector3.Reflect(thrownVeloicity, normal);
			targetVelocity = Vector3.Reflect(targetVelocity, normal).x0z();
		}
		else
		{
			base.transform.position += vector;
		}
		bugRotationalVelocity = quaternion * bugRotationalVelocity;
		bugRotationalVelocity.ToAngleAxis(out var angle3, out var axis3);
		bugRotationalVelocity = Quaternion.AngleAxis(angle3 * 0.9f, axis3);
		base.transform.rotation = bugRotationalVelocity * base.transform.rotation;
	}

	private float RandomizeBobingFrequency()
	{
		return UnityEngine.Random.Range(minRandFrequency, maxRandFrequency);
	}

	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		slowingDownProgress = 0f;
		GorillaVelocityEstimator component = GetComponent<GorillaVelocityEstimator>();
		Vector3 travelingDirection = (thrownVeloicity = component.linearVelocity);
		reliableState.travelingDirection = travelingDirection;
		bugRotationalVelocity = Quaternion.Euler(component.angularVelocity);
		startingSpeed = travelingDirection.magnitude;
		Vector3 normalized = reliableState.travelingDirection.x0z().normalized;
		targetVelocity = normalized * maxNaturalSpeed;
	}

	public void OnCollisionEnter(Collision collision)
	{
		reliableState.travelingDirection *= -1f;
	}

	private void Update()
	{
		if (updateMultiplier > 0)
		{
			for (int i = 0; i < updateMultiplier; i++)
			{
				LateUpdateLocal();
			}
		}
	}
}
