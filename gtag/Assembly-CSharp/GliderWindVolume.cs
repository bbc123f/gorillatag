using System;
using UnityEngine;

public class GliderWindVolume : MonoBehaviour
{
	public Vector3 WindDirection
	{
		get
		{
			return base.transform.TransformDirection(this.localWindDirection);
		}
	}

	public Vector3 GetAccelFromVelocity(Vector3 velocity)
	{
		Vector3 windDirection = this.WindDirection;
		float time = Mathf.Clamp(Vector3.Dot(velocity, windDirection), -this.maxSpeed, this.maxSpeed) / this.maxSpeed;
		float d = this.speedVsAccelCurve.Evaluate(time) * this.maxAccel;
		return windDirection * d;
	}

	public GliderWindVolume()
	{
	}

	[SerializeField]
	private float maxSpeed = 30f;

	[SerializeField]
	private float maxAccel = 15f;

	[SerializeField]
	private AnimationCurve speedVsAccelCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	[SerializeField]
	private Vector3 localWindDirection = Vector3.up;
}
