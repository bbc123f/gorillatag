using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using UnityEngine;

public class GorillaVelocityEstimator : MonoBehaviour
{
	public Vector3 linearVelocity
	{
		[CompilerGenerated]
		get
		{
			return this.<linearVelocity>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<linearVelocity>k__BackingField = value;
		}
	}

	public Vector3 angularVelocity
	{
		[CompilerGenerated]
		get
		{
			return this.<angularVelocity>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<angularVelocity>k__BackingField = value;
		}
	}

	public Vector3 handPos
	{
		[CompilerGenerated]
		get
		{
			return this.<handPos>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<handPos>k__BackingField = value;
		}
	}

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
		Vector3 vector = Player.Instance.currentVelocity;
		if (this.useGlobalSpace)
		{
			vector = Vector3.zero;
		}
		Vector3 vector2 = (position - this.lastPos) / Time.deltaTime - vector;
		Quaternion rotation = base.transform.rotation;
		Vector3 vector3 = (rotation * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector3.x > 180f)
		{
			vector3.x -= 360f;
		}
		if (vector3.y > 180f)
		{
			vector3.y -= 360f;
		}
		if (vector3.z > 180f)
		{
			vector3.z -= 360f;
		}
		vector3 *= 0.017453292f / Time.fixedDeltaTime;
		this.history[this.currentFrame % this.numFrames] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = vector2,
			angular = vector3
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

	public GorillaVelocityEstimator()
	{
	}

	private int numFrames = 8;

	[CompilerGenerated]
	private Vector3 <linearVelocity>k__BackingField;

	[CompilerGenerated]
	private Vector3 <angularVelocity>k__BackingField;

	[CompilerGenerated]
	private Vector3 <handPos>k__BackingField;

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
