using System;
using BoingKit;
using UnityEngine;

public class UFOCamera : MonoBehaviour
{
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_targetOffset = base.transform.position - this.Target.position;
		this.m_spring.Reset(base.transform.position);
	}

	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	public Transform Target;

	private Vector3 m_targetOffset;

	private Vector3Spring m_spring;
}
