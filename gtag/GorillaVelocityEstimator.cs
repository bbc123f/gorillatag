using System;
using GorillaLocomotion;
using UnityEngine;

public class GorillaVelocityEstimator : MonoBehaviour
{
	public struct VelocityHistorySample
	{
		public Vector3 linear;

		public Vector3 angular;
	}

	private int numFrames = 8;

	private VelocityHistorySample[] history;

	private int currentFrame;

	private Vector3 lastPos;

	private Quaternion lastRotation;

	private Vector3 lastRotationVec;

	public bool useGlobalSpace;

	public Vector3 linearVelocity { get; private set; }

	public Vector3 angularVelocity { get; private set; }

	public Vector3 handPos { get; private set; }

	private void Awake()
	{
		history = new VelocityHistorySample[numFrames];
	}

	private void OnEnable()
	{
		currentFrame = 0;
		for (int i = 0; i < history.Length; i++)
		{
			history[i] = default(VelocityHistorySample);
		}
		lastPos = base.transform.position;
		lastRotation = base.transform.rotation;
	}

	protected void LateUpdate()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = Player.Instance.currentVelocity;
		if (useGlobalSpace)
		{
			vector = Vector3.zero;
		}
		Vector3 linear = (position - lastPos) / Time.deltaTime - vector;
		Quaternion rotation = base.transform.rotation;
		Vector3 eulerAngles = (rotation * Quaternion.Inverse(lastRotation)).eulerAngles;
		if (eulerAngles.x > 180f)
		{
			eulerAngles.x -= 360f;
		}
		if (eulerAngles.y > 180f)
		{
			eulerAngles.y -= 360f;
		}
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		eulerAngles *= (float)Math.PI / 180f / Time.fixedDeltaTime;
		history[currentFrame % numFrames] = new VelocityHistorySample
		{
			linear = linear,
			angular = eulerAngles
		};
		linearVelocity = history[0].linear;
		angularVelocity = history[0].angular;
		for (int i = 0; i < numFrames; i++)
		{
			linearVelocity += history[i].linear;
			angularVelocity += history[i].angular;
		}
		linearVelocity /= (float)numFrames;
		angularVelocity /= (float)numFrames;
		handPos = position;
		currentFrame = (currentFrame + 1) % numFrames;
		lastPos = position;
		lastRotation = rotation;
	}
}
