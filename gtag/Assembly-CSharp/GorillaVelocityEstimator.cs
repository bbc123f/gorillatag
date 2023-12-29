using System;
using GorillaLocomotion;
using UnityEngine;

public class GorillaVelocityEstimator : MonoBehaviour
{
	public Vector3 linearVelocity { get; private set; }

	public Vector3 angularVelocity { get; private set; }

	public Vector3 handPos { get; private set; }

	private void Awake()
	{
		this.history = new GorillaVelocityEstimator.VelocityHistorySample[this.numFrames];
	}

	private void OnEnable()
	{
		this.currentFrame = 0;
		for (int i = 0; i < this.history.Length; i++)
		{
			this.history[i] = default(GorillaVelocityEstimator.VelocityHistorySample);
		}
		this.lastPos = base.transform.position;
		this.lastRotation = base.transform.rotation;
		GorillaVelocityEstimatorManager.Register(this);
	}

	private void OnDisable()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	private void OnDestroy()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	public void TriggeredLateUpdate()
	{
		Vector3 position = base.transform.position;
		Vector3 b = Player.Instance.currentVelocity;
		if (this.useGlobalSpace)
		{
			b = Vector3.zero;
		}
		Vector3 linear = (position - this.lastPos) / Time.deltaTime - b;
		Quaternion rotation = base.transform.rotation;
		Vector3 vector = (rotation * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		if (vector.y > 180f)
		{
			vector.y -= 360f;
		}
		if (vector.z > 180f)
		{
			vector.z -= 360f;
		}
		vector *= 0.017453292f / Time.fixedDeltaTime;
		this.history[this.currentFrame % this.numFrames] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = linear,
			angular = vector
		};
		this.linearVelocity = this.history[0].linear;
		this.angularVelocity = this.history[0].angular;
		for (int i = 0; i < this.numFrames; i++)
		{
			this.linearVelocity += this.history[i].linear;
			this.angularVelocity += this.history[i].angular;
		}
		this.linearVelocity /= (float)this.numFrames;
		this.angularVelocity /= (float)this.numFrames;
		this.handPos = position;
		this.currentFrame = (this.currentFrame + 1) % this.numFrames;
		this.lastPos = position;
		this.lastRotation = rotation;
	}

	private int numFrames = 8;

	private GorillaVelocityEstimator.VelocityHistorySample[] history;

	private int currentFrame;

	private Vector3 lastPos;

	private Quaternion lastRotation;

	private Vector3 lastRotationVec;

	public bool useGlobalSpace;

	public struct VelocityHistorySample
	{
		public Vector3 linear;

		public Vector3 angular;
	}
}
