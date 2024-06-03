using System;
using UnityEngine;

public class WingsWearable : MonoBehaviour
{
	private void Awake()
	{
		this.xform = this.animator.transform;
	}

	private void OnEnable()
	{
		this.oldPos = this.xform.localPosition;
	}

	private void Update()
	{
		Vector3 position = this.xform.position;
		float f = (position - this.oldPos).magnitude / Time.deltaTime;
		float value = this.flapSpeedCurve.Evaluate(Mathf.Abs(f));
		this.animator.SetFloat(this.flapSpeedParamID, value);
		this.oldPos = position;
	}

	public WingsWearable()
	{
	}

	[Tooltip("This animator must have a parameter called 'FlapSpeed'")]
	public Animator animator;

	[Tooltip("X axis is move speed, Y axis is flap speed")]
	public AnimationCurve flapSpeedCurve;

	private Transform xform;

	private Vector3 oldPos;

	private readonly int flapSpeedParamID = Animator.StringToHash("FlapSpeed");
}
