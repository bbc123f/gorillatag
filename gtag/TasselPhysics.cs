using System;
using UnityEngine;

public class TasselPhysics : MonoBehaviour
{
	private void Awake()
	{
		this.centerOfMassLength = this.localCenterOfMass.magnitude;
	}

	private void Update()
	{
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector2 = position + (vector - position).normalized * this.centerOfMassLength;
		this.velocity = (vector2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = vector2;
		foreach (GameObject gameObject in this.tasselInstances)
		{
			gameObject.transform.rotation = Quaternion.LookRotation(vector2 - position, gameObject.transform.position - position);
		}
	}

	public TasselPhysics()
	{
	}

	[SerializeField]
	private GameObject[] tasselInstances;

	[SerializeField]
	private Vector3 localCenterOfMass;

	[SerializeField]
	private float gravityStrength;

	[SerializeField]
	private float drag;

	private Vector3 lastCenterPos;

	private Vector3 velocity;

	private float centerOfMassLength;
}
