using System;
using Cinemachine.Utility;
using GorillaExtensions;
using UnityEngine;

public class GragerHoldable : MonoBehaviour
{
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.lastWorldPosition = base.transform.TransformPoint(this.LocalCenterOfMass);
		this.lastClackParentLocalPosition = base.transform.parent.InverseTransformPoint(this.lastWorldPosition);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	private void Update()
	{
		Vector3 target = base.transform.TransformPoint(this.LocalCenterOfMass);
		Vector3 a = this.lastWorldPosition + this.velocity * Time.deltaTime * this.drag;
		Vector3 vector = base.transform.parent.TransformDirection(this.LocalRotationAxis);
		Vector3 vector2 = base.transform.position + (a - base.transform.position).ProjectOntoPlane(vector).normalized * this.centerOfMassRadius;
		vector2 = Vector3.MoveTowards(vector2, target, this.localFriction * Time.deltaTime);
		this.velocity = (vector2 - this.lastWorldPosition) / Time.deltaTime;
		this.velocity += Vector3.down * this.gravity * Time.deltaTime;
		this.lastWorldPosition = vector2;
		base.transform.rotation = Quaternion.LookRotation(vector2 - base.transform.position, vector) * this.RotationCorrection;
		Vector3 a2 = base.transform.parent.InverseTransformPoint(base.transform.TransformPoint(this.LocalCenterOfMass));
		if ((a2 - this.lastClackParentLocalPosition).IsLongerThan(this.distancePerClack))
		{
			this.clackAudio.PlayOneShot(this.allClacks[Random.Range(0, this.allClacks.Length)]);
			this.lastClackParentLocalPosition = a2;
		}
	}

	public GragerHoldable()
	{
	}

	[SerializeField]
	private Vector3 LocalCenterOfMass;

	[SerializeField]
	private Vector3 LocalRotationAxis;

	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	[SerializeField]
	private float drag;

	[SerializeField]
	private float gravity;

	[SerializeField]
	private float localFriction;

	[SerializeField]
	private float distancePerClack;

	[SerializeField]
	private AudioSource clackAudio;

	[SerializeField]
	private AudioClip[] allClacks;

	private float centerOfMassRadius;

	private Vector3 velocity;

	private Vector3 lastWorldPosition;

	private Vector3 lastClackParentLocalPosition;

	private Quaternion RotationCorrection;
}
