using GorillaLocomotion;
using UnityEngine;

public class ForceVolume : MonoBehaviour
{
	private enum AudioState
	{
		None,
		Enter,
		Crescendo,
		Loop,
		Exit
	}

	[SerializeField]
	public bool scaleWithSize = true;

	[SerializeField]
	private float accel;

	[SerializeField]
	private float maxDepth = -1f;

	[SerializeField]
	private float maxSpeed;

	[SerializeField]
	private bool disableGrip;

	[SerializeField]
	private float dampenXVelPerc;

	[SerializeField]
	private float dampenZVelPerc;

	[SerializeField]
	private float pullToCenterAccel;

	[SerializeField]
	private float pullToCenterMaxSpeed;

	[SerializeField]
	private float pullTOCenterMinDistance = 0.1f;

	private Collider volume;

	public AudioClip enterClip;

	public AudioClip exitClip;

	public AudioClip loopClip;

	public AudioClip loopCresendoClip;

	public AudioSource audioSource;

	private Vector3 enterPos;

	private AudioState audioState;

	private void Awake()
	{
		volume = GetComponent<Collider>();
		audioState = AudioState.None;
	}

	private void LateUpdate()
	{
		if ((bool)audioSource && audioSource != null && !audioSource.isPlaying && audioSource.enabled)
		{
			audioSource.enabled = false;
		}
	}

	private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
	{
		rb = null;
		xf = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
			xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
		}
		if (rb != null)
		{
			return xf != null;
		}
		return false;
	}

	public void OnTriggerEnter(Collider other)
	{
		Rigidbody rb = null;
		Transform xf = null;
		if (TriggerFilter(other, out rb, out xf) && !(enterClip == null))
		{
			if ((bool)audioSource)
			{
				audioSource.enabled = true;
				audioSource.PlayOneShot(enterClip);
				audioState = AudioState.Enter;
			}
			enterPos = xf.position;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Rigidbody rb = null;
		Transform xf = null;
		if (TriggerFilter(other, out rb, out xf) && (bool)audioSource)
		{
			audioSource.enabled = true;
			audioSource.PlayOneShot(exitClip);
			audioState = AudioState.None;
		}
	}

	public void OnTriggerStay(Collider other)
	{
		Rigidbody rb = null;
		Transform xf = null;
		if (!TriggerFilter(other, out rb, out xf))
		{
			return;
		}
		if ((bool)audioSource && !audioSource.isPlaying)
		{
			switch (audioState)
			{
			case AudioState.Enter:
				if (loopCresendoClip != null)
				{
					audioSource.enabled = true;
					audioSource.PlayOneShot(loopCresendoClip);
				}
				audioState = AudioState.Crescendo;
				break;
			case AudioState.Loop:
				if (loopClip != null)
				{
					audioSource.enabled = true;
					audioSource.PlayOneShot(loopClip);
				}
				audioState = AudioState.Loop;
				break;
			}
		}
		if (disableGrip)
		{
			Player.Instance.SetMaximumSlipThisFrame();
		}
		SizeManager sizeManager = null;
		if (scaleWithSize)
		{
			sizeManager = rb.GetComponent<SizeManager>();
		}
		Vector3 velocity = rb.velocity;
		if (scaleWithSize && (bool)sizeManager)
		{
			velocity /= sizeManager.currentScale;
		}
		Vector3 vector = Vector3.Dot(xf.position - base.transform.position, base.transform.up) * base.transform.up;
		Vector3 vector2 = base.transform.position + vector - xf.position;
		float num = vector2.magnitude + 0.0001f;
		Vector3 vector3 = vector2 / num;
		float num2 = Vector3.Dot(velocity, vector3);
		if (maxDepth > -1f)
		{
			float num3 = Vector3.Dot(xf.position - enterPos, vector3);
			float num4 = maxDepth - num3;
			float b = 0f;
			if (num4 > 0.0001f)
			{
				b = num2 * num2 / num4;
			}
			accel = Mathf.Max(accel, b);
		}
		float deltaTime = Time.deltaTime;
		Vector3 vector4 = base.transform.up * accel * deltaTime;
		velocity += vector4;
		Vector3 vector5 = Mathf.Min(Vector3.Dot(velocity, base.transform.up), maxSpeed) * base.transform.up;
		Vector3 vector6 = Vector3.Dot(velocity, base.transform.right) * base.transform.right;
		Vector3 vector7 = Vector3.Dot(velocity, base.transform.forward) * base.transform.forward;
		float num5 = 1f - dampenXVelPerc * 0.01f * deltaTime;
		float num6 = 1f - dampenZVelPerc * 0.01f * deltaTime;
		velocity = vector5 + num5 * vector6 + num6 * vector7;
		if (pullToCenterAccel > 0f && pullToCenterMaxSpeed > 0f)
		{
			velocity -= num2 * vector3;
			if (num > pullTOCenterMinDistance)
			{
				num2 += pullToCenterAccel * deltaTime;
				float b2 = Mathf.Min(pullToCenterMaxSpeed, num / deltaTime);
				num2 = Mathf.Min(num2, b2);
			}
			else
			{
				num2 = 0f;
			}
			velocity += num2 * vector3;
			if (velocity.magnitude > 0.0001f)
			{
				Vector3 vector8 = Vector3.Cross(base.transform.up, vector3);
				float magnitude = vector8.magnitude;
				if (magnitude > 0.0001f)
				{
					vector8 /= magnitude;
					num2 = Vector3.Dot(velocity, vector8);
					velocity -= num2 * vector8;
					num2 -= pullToCenterAccel * deltaTime;
					num2 = Mathf.Max(0f, num2);
					velocity += num2 * vector8;
				}
			}
		}
		if (scaleWithSize && (bool)sizeManager)
		{
			velocity *= sizeManager.currentScale;
		}
		rb.velocity = velocity;
	}

	public void OnDrawGizmosSelected()
	{
		GetComponents<Collider>();
		Gizmos.color = Color.magenta;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, pullTOCenterMinDistance / base.transform.lossyScale.z));
	}
}
