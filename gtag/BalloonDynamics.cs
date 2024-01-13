using UnityEngine;

public class BalloonDynamics : MonoBehaviour
{
	private Rigidbody rb;

	private Collider balloonCollider;

	private Bounds bounds;

	public float bouyancyForce = 1f;

	public float bouyancyMinHeight = 10f;

	public float bouyancyMaxHeight = 20f;

	private float bouyancyActualHeight = 20f;

	public float varianceMaxheight = 5f;

	public float airResistance = 0.01f;

	public GameObject knot;

	private Rigidbody knotRb;

	public Transform grabPt;

	private Transform grabPtInitParent;

	public float stringLength = 2f;

	public float stringStrength = 0.9f;

	public float stringStretch = 0.1f;

	public float maximumVelocity = 2f;

	public float upRightTorque = 1f;

	private bool enableDynamics;

	private bool enableDistanceConstraints;

	public bool ColliderEnabled
	{
		get
		{
			if ((bool)balloonCollider)
			{
				return balloonCollider.enabled;
			}
			return false;
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		knotRb = knot.GetComponent<Rigidbody>();
		balloonCollider = GetComponent<Collider>();
		grabPtInitParent = grabPt.transform.parent;
	}

	private void Start()
	{
		airResistance = Mathf.Clamp(airResistance, 0f, 1f);
		balloonCollider.enabled = false;
	}

	public void ReParent()
	{
		if (grabPt != null)
		{
			grabPt.transform.parent = grabPtInitParent.transform;
		}
		bouyancyActualHeight = Random.Range(bouyancyMinHeight, bouyancyMaxHeight);
	}

	private void ApplyBouyancyForce()
	{
		float num = bouyancyActualHeight + Mathf.Sin(Time.time) * varianceMaxheight;
		float num2 = (num - base.transform.position.y) / num;
		float y = bouyancyForce * num2;
		rb.AddForce(new Vector3(0f, y, 0f), ForceMode.Acceleration);
	}

	private void ApplyUpRightForce()
	{
		Vector3 torque = Vector3.Cross(base.transform.up, Vector3.up) * upRightTorque;
		rb.AddTorque(torque);
	}

	private void ApplyAirResistance()
	{
		rb.velocity *= 1f - airResistance;
	}

	private void ApplyDistanceConstraint()
	{
		_ = knot.transform.position - base.transform.position;
		Vector3 vector = grabPt.transform.position - knot.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		if (magnitude > stringLength)
		{
			Vector3 vector2 = Vector3.Dot(knotRb.velocity, normalized) * normalized;
			float num = magnitude - stringLength;
			float num2 = num / Time.fixedDeltaTime;
			if (vector2.magnitude < num2)
			{
				float b = num2 - vector2.magnitude;
				float num3 = Mathf.Clamp01(num / stringStretch);
				Vector3 force = Mathf.Lerp(0f, b, num3 * num3) * normalized * stringStrength;
				rb.AddForceAtPosition(force, knot.transform.position, ForceMode.VelocityChange);
			}
		}
	}

	public void EnableDynamics(bool enable, bool kinematic)
	{
		enableDynamics = enable;
		if ((bool)balloonCollider)
		{
			balloonCollider.enabled = enable;
		}
		if (rb != null)
		{
			rb.isKinematic = kinematic;
			if (!enable)
			{
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
		}
	}

	public void EnableDistanceConstraints(bool enable)
	{
		enableDistanceConstraints = enable;
	}

	private void FixedUpdate()
	{
		if (enableDynamics)
		{
			ApplyBouyancyForce();
			ApplyUpRightForce();
			ApplyAirResistance();
			if (enableDistanceConstraints)
			{
				ApplyDistanceConstraint();
			}
			float magnitude = rb.velocity.magnitude;
			rb.velocity = rb.velocity.normalized * Mathf.Min(magnitude, maximumVelocity);
		}
	}
}
