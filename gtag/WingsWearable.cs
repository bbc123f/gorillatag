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
		float num = (position - this.oldPos).magnitude / Time.deltaTime;
		float num2 = this.flapSpeedCurve.Evaluate(Mathf.Abs(num));
		this.animator.SetFloat(this.flapSpeedParamID, num2);
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
