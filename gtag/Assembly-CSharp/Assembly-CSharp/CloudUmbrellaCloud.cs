using System;
using UnityEngine;

public class CloudUmbrellaCloud : MonoBehaviour
{
	protected void Awake()
	{
		this.umbrellaXform = this.umbrella.transform;
		this.cloudScaleXform = this.cloudRenderer.transform;
	}

	protected void LateUpdate()
	{
		float time = Vector3.Dot(this.umbrellaXform.up, Vector3.up);
		float num = Mathf.Clamp01(this.scaleCurve.Evaluate(time));
		this.rendererOn = ((num > 0.09f && num < 0.1f) ? this.rendererOn : (num > 0.1f));
		this.cloudRenderer.enabled = this.rendererOn;
		this.cloudScaleXform.localScale = new Vector3(num, num, num);
		this.cloudRotateXform.up = Vector3.up;
	}

	public UmbrellaItem umbrella;

	public Transform cloudRotateXform;

	public Renderer cloudRenderer;

	public AnimationCurve scaleCurve;

	private const float kHideAtScale = 0.1f;

	private const float kHideAtScaleTolerance = 0.01f;

	private bool rendererOn;

	private Transform umbrellaXform;

	private Transform cloudScaleXform;
}
