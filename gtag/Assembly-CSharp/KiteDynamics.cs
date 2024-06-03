using System;
using UnityEngine;

public class KiteDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtPosition = this.grabPt.position;
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
		this.held = enable;
		this.rb.useGravity = !enable;
		this.balloonScale = scale;
		this.grabPtPosition = this.grabPt.position;
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
		if (this.rb.isKinematic || this.rb.useGravity)
		{
			return;
		}
		if (this.enableDynamics)
		{
			Vector3 vector = (this.grabPt.position - this.grabPtPosition) * 100f;
			vector = Matrix4x4.Rotate(this.ctrlRotation).MultiplyVector(vector);
			this.rb.AddForce(vector, ForceMode.Force);
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			this.rb.velocity = velocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
			base.transform.LookAt(base.transform.position - this.rb.velocity);
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
		transferOwnership = false;
	}

	public bool ReturnStep()
	{
		this.rb.isKinematic = true;
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.grabPt.position, Time.deltaTime * this.returnSpeed);
		return base.transform.position == this.grabPt.position;
	}

	public KiteDynamics()
	{
	}

	private Rigidbody rb;

	private Collider balloonCollider;

	private Bounds bounds;

	[SerializeField]
	private float bouyancyForce = 1f;

	[SerializeField]
	private float bouyancyMinHeight = 10f;

	[SerializeField]
	private float bouyancyMaxHeight = 20f;

	private float bouyancyActualHeight = 20f;

	[SerializeField]
	private float varianceMaxheight = 5f;

	[SerializeField]
	private float airResistance = 0.01f;

	public GameObject knot;

	private Rigidbody knotRb;

	public Transform grabPt;

	private Transform grabPtInitParent;

	[SerializeField]
	private float stringLength = 2f;

	[SerializeField]
	private float stringStrength = 0.9f;

	[SerializeField]
	private float stringStretch = 0.1f;

	[SerializeField]
	private float maximumVelocity = 2f;

	[SerializeField]
	private float upRightTorque = 1f;

	private bool enableDynamics;

	private bool held;

	[SerializeField]
	private float balloonScale = 1f;

	[SerializeField]
	private float bopSpeed = 1f;

	private Vector3 grabPtPosition;

	[SerializeField]
	private Quaternion ctrlRotation;

	[SerializeField]
	private float returnSpeed = 50f;
}
