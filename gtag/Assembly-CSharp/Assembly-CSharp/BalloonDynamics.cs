using System;
using UnityEngine;

public class BalloonDynamics : MonoBehaviour
{
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	private void ApplyBouyancyForce()
	{
		float num = this.bouyancyActualHeight + Mathf.Sin(Time.time) * this.varianceMaxheight;
		float num2 = (num - base.transform.position.y) / num;
		float y = this.bouyancyForce * num2;
		this.rb.AddForce(new Vector3(0f, y, 0f), ForceMode.Acceleration);
	}

	private void ApplyUpRightForce()
	{
		Vector3 torque = Vector3.Cross(base.transform.up, Vector3.up) * this.upRightTorque;
		this.rb.AddTorque(torque);
	}

	private void ApplyAirResistance()
	{
		this.rb.velocity *= 1f - this.airResistance;
	}

	private void ApplyDistanceConstraint()
	{
		this.knot.transform.position - base.transform.position;
		Vector3 vector = this.grabPt.transform.position - this.knot.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		if (magnitude > this.stringLength)
		{
			Vector3 vector2 = Vector3.Dot(this.knotRb.velocity, normalized) * normalized;
			float num = magnitude - this.stringLength;
			float num2 = num / Time.fixedDeltaTime;
			if (vector2.magnitude < num2)
			{
				float b = num2 - vector2.magnitude;
				float num3 = Mathf.Clamp01(num / this.stringStretch);
				Vector3 force = Mathf.Lerp(0f, b, num3 * num3) * normalized * this.stringStrength;
				this.rb.AddForceAtPosition(force, this.knot.transform.position, ForceMode.VelocityChange);
			}
		}
	}

	public void EnableDynamics(bool enable, bool kinematic)
	{
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = enable;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!enable)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	public void EnableDistanceConstraints(bool enable)
	{
		this.enableDistanceConstraints = enable;
	}

	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	private void FixedUpdate()
	{
		if (this.enableDynamics)
		{
			this.ApplyBouyancyForce();
			this.ApplyUpRightForce();
			this.ApplyAirResistance();
			if (this.enableDistanceConstraints)
			{
				this.ApplyDistanceConstraint();
			}
			float magnitude = this.rb.velocity.magnitude;
			this.rb.velocity = this.rb.velocity.normalized * Mathf.Min(magnitude, this.maximumVelocity);
		}
	}

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
}
