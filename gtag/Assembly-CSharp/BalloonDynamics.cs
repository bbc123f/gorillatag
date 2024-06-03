using System;
using UnityEngine;

public class BalloonDynamics : MonoBehaviour, ITetheredObjectBehavior
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
		float y = this.bouyancyForce * num2 * this.balloonScale;
		this.rb.AddForce(new Vector3(0f, y, 0f), ForceMode.Acceleration);
	}

	private void ApplyUpRightForce()
	{
		Vector3 torque = Vector3.Cross(base.transform.up, Vector3.up) * this.upRightTorque * this.balloonScale;
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
		float num = this.stringLength * this.balloonScale;
		if (magnitude > num)
		{
			Vector3 vector2 = Vector3.Dot(this.knotRb.velocity, normalized) * normalized;
			float num2 = magnitude - num;
			float num3 = num2 / Time.fixedDeltaTime;
			if (vector2.magnitude < num3)
			{
				float b = num3 - vector2.magnitude;
				float num4 = Mathf.Clamp01(num2 / this.stringStretch);
				Vector3 force = Mathf.Lerp(0f, b, num4 * num4) * normalized * this.stringStrength;
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

	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.enableDistanceConstraints = enable;
		this.balloonScale = scale;
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
		if (this.enableDynamics && !this.rb.isKinematic)
		{
			this.ApplyBouyancyForce();
			this.ApplyUpRightForce();
			this.ApplyAirResistance();
			if (this.enableDistanceConstraints)
			{
				this.ApplyDistanceConstraint();
			}
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			this.rb.velocity = velocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
		}
	}

	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		if (other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
		{
			transferOwnership = true;
			TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
			if (component)
			{
				Vector3 a = (component.transform.position - component.prevPos) / Time.deltaTime;
				if (this.rb)
				{
					force = a * this.bopSpeed;
					force = Mathf.Min(this.maximumVelocity, force.magnitude) * force.normalized * this.balloonScale;
					collisionPt = other.ClosestPointOnBounds(base.transform.position);
					this.rb.AddForceAtPosition(force, collisionPt, ForceMode.VelocityChange);
					GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
					if (component2 != null)
					{
						float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
						float fixedDeltaTime = Time.fixedDeltaTime;
						GorillaTagger.Instance.StartVibration(component2.isLeftHand, amplitude, fixedDeltaTime);
					}
				}
			}
		}
	}

	public bool ReturnStep()
	{
		return true;
	}

	public BalloonDynamics()
	{
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

	public float balloonScale = 1f;

	public float bopSpeed = 1f;
}
