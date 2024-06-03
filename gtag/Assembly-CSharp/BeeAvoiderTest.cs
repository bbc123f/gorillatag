using System;
using GorillaExtensions;
using UnityEngine;

public class BeeAvoiderTest : MonoBehaviour
{
	public void Update()
	{
		Vector3 position = this.patrolPoints[this.nextPatrolPoint].transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 target = (position - position2).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * this.drag, target, this.acceleration);
		if ((position2 - position).IsLongerThan(this.instabilityOffRadius))
		{
			this.velocity += Random.insideUnitSphere * this.instability * Time.deltaTime;
		}
		Vector3 vector = position2 + this.velocity * Time.deltaTime;
		GameObject[] array = this.avoidancePoints;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 position3 = array[i].transform.position;
			if ((vector - position3).IsShorterThan(this.avoidRadius))
			{
				Vector3 normalized = Vector3.Cross(position3 - vector, position - vector).normalized;
				Vector3 normalized2 = (position - position3).normalized;
				float num = Vector3.Dot(vector - position3, normalized);
				Vector3 b = (this.avoidRadius - num) * normalized;
				vector += b;
				this.velocity += b;
			}
		}
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(position - vector);
		if ((vector - position).IsShorterThan(this.patrolArrivedRadius))
		{
			this.nextPatrolPoint = (this.nextPatrolPoint + 1) % this.patrolPoints.Length;
		}
	}

	public BeeAvoiderTest()
	{
	}

	public GameObject[] patrolPoints;

	public GameObject[] avoidancePoints;

	public float speed;

	public float acceleration;

	public float instability;

	public float instabilityOffRadius;

	public float drag;

	public float avoidRadius;

	public float patrolArrivedRadius;

	private int nextPatrolPoint;

	private Vector3 velocity;
}
