using System;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class BalloonDynamics : MonoBehaviour
{
	// Token: 0x060004C3 RID: 1219 RVA: 0x0001E6A4 File Offset: 0x0001C8A4
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x0001E6F0 File Offset: 0x0001C8F0
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x0001E71C File Offset: 0x0001C91C
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0001E76C File Offset: 0x0001C96C
	private void ApplyBouyancyForce()
	{
		float num = this.bouyancyActualHeight + Mathf.Sin(Time.time) * this.varianceMaxheight;
		float num2 = (num - base.transform.position.y) / num;
		float y = this.bouyancyForce * num2;
		this.rb.AddForce(new Vector3(0f, y, 0f), ForceMode.Acceleration);
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0001E7CC File Offset: 0x0001C9CC
	private void ApplyUpRightForce()
	{
		Vector3 torque = Vector3.Cross(base.transform.up, Vector3.up) * this.upRightTorque;
		this.rb.AddTorque(torque);
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001E806 File Offset: 0x0001CA06
	private void ApplyAirResistance()
	{
		this.rb.velocity *= 1f - this.airResistance;
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0001E82C File Offset: 0x0001CA2C
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

	// Token: 0x060004CA RID: 1226 RVA: 0x0001E934 File Offset: 0x0001CB34
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

	// Token: 0x060004CB RID: 1227 RVA: 0x0001E99E File Offset: 0x0001CB9E
	public void EnableDistanceConstraints(bool enable)
	{
		this.enableDistanceConstraints = enable;
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060004CC RID: 1228 RVA: 0x0001E9A7 File Offset: 0x0001CBA7
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0001E9C4 File Offset: 0x0001CBC4
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

	// Token: 0x04000586 RID: 1414
	private Rigidbody rb;

	// Token: 0x04000587 RID: 1415
	private Collider balloonCollider;

	// Token: 0x04000588 RID: 1416
	private Bounds bounds;

	// Token: 0x04000589 RID: 1417
	public float bouyancyForce = 1f;

	// Token: 0x0400058A RID: 1418
	public float bouyancyMinHeight = 10f;

	// Token: 0x0400058B RID: 1419
	public float bouyancyMaxHeight = 20f;

	// Token: 0x0400058C RID: 1420
	private float bouyancyActualHeight = 20f;

	// Token: 0x0400058D RID: 1421
	public float varianceMaxheight = 5f;

	// Token: 0x0400058E RID: 1422
	public float airResistance = 0.01f;

	// Token: 0x0400058F RID: 1423
	public GameObject knot;

	// Token: 0x04000590 RID: 1424
	private Rigidbody knotRb;

	// Token: 0x04000591 RID: 1425
	public Transform grabPt;

	// Token: 0x04000592 RID: 1426
	private Transform grabPtInitParent;

	// Token: 0x04000593 RID: 1427
	public float stringLength = 2f;

	// Token: 0x04000594 RID: 1428
	public float stringStrength = 0.9f;

	// Token: 0x04000595 RID: 1429
	public float stringStretch = 0.1f;

	// Token: 0x04000596 RID: 1430
	public float maximumVelocity = 2f;

	// Token: 0x04000597 RID: 1431
	public float upRightTorque = 1f;

	// Token: 0x04000598 RID: 1432
	private bool enableDynamics;

	// Token: 0x04000599 RID: 1433
	private bool enableDistanceConstraints;
}
