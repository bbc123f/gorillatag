using System;
using GorillaExtensions;
using UnityEngine;

public class LongScarfSim : MonoBehaviour
{
	private void Start()
	{
		this.clampToPlane.Normalize();
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.baseLocalRotations = new Quaternion[this.gameObjects.Length];
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.baseLocalRotations[i] = this.gameObjects[i].transform.localRotation;
		}
	}

	private void LateUpdate()
	{
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 a = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector = position + (a - position).normalized * this.centerOfMassLength;
		Vector3 vector2 = base.transform.InverseTransformPoint(vector);
		float num = Vector3.Dot(vector2, this.clampToPlane);
		if (num < 0f)
		{
			vector2 -= this.clampToPlane * num;
			vector = base.transform.TransformPoint(vector2);
		}
		Vector3 a2 = vector;
		this.velocity = (a2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = a2;
		float target = (float)(this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold) ? 1 : 0);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, target, this.blendAmountPerSecond * Time.deltaTime);
		Quaternion b = Quaternion.LookRotation(a2 - position);
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			Quaternion a3 = this.gameObjects[i].transform.parent.rotation * this.baseLocalRotations[i];
			this.gameObjects[i].transform.rotation = Quaternion.Lerp(a3, b, this.currentBlend);
		}
	}

	public LongScarfSim()
	{
	}

	[SerializeField]
	private GameObject[] gameObjects;

	[SerializeField]
	private float speedThreshold = 1f;

	[SerializeField]
	private float blendAmountPerSecond = 1f;

	private GorillaVelocityEstimator velocityEstimator;

	private Quaternion[] baseLocalRotations;

	private float currentBlend;

	[SerializeField]
	private float centerOfMassLength;

	[SerializeField]
	private float gravityStrength;

	[SerializeField]
	private float drag;

	[SerializeField]
	private Vector3 clampToPlane;

	private Vector3 lastCenterPos;

	private Vector3 velocity;
}
