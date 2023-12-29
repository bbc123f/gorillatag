using System;
using UnityEngine;

public class ChestObjectHysteresis : MonoBehaviour
{
	private void Start()
	{
		this.lastAngleQuat = base.transform.rotation;
		this.currentAngleQuat = base.transform.rotation;
	}

	private void OnEnable()
	{
		ChestObjectHysteresisManager.RegisterCH(this);
	}

	private void OnDisable()
	{
		ChestObjectHysteresisManager.UnregisterCH(this);
	}

	public void InvokeUpdate()
	{
		this.currentAngleQuat = this.angleFollower.rotation;
		this.angleBetween = Quaternion.Angle(this.currentAngleQuat, this.lastAngleQuat);
		if (this.angleBetween > this.angleHysteresis)
		{
			base.transform.rotation = Quaternion.Slerp(this.currentAngleQuat, this.lastAngleQuat, this.angleHysteresis / this.angleBetween);
			this.lastAngleQuat = base.transform.rotation;
		}
		base.transform.rotation = this.lastAngleQuat;
	}

	public float angleHysteresis;

	public float angleBetween;

	public Transform angleFollower;

	private Quaternion lastAngleQuat;

	private Quaternion currentAngleQuat;
}
