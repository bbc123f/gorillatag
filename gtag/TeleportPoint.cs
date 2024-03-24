using System;
using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
	private void Start()
	{
	}

	public Transform GetDestTransform()
	{
		return this.destTransform;
	}

	private void Update()
	{
		float num = Mathf.SmoothStep(this.fullIntensity, this.lowIntensity, (Time.time - this.lastLookAtTime) * this.dimmingSpeed);
		base.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", num);
	}

	public void OnLookAt()
	{
		this.lastLookAtTime = Time.time;
	}

	public TeleportPoint()
	{
	}

	public float dimmingSpeed = 1f;

	public float fullIntensity = 1f;

	public float lowIntensity = 0.5f;

	public Transform destTransform;

	private float lastLookAtTime;
}
