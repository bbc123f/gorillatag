using System;
using System.Collections.Generic;
using UnityEngine;

public class OwlLook : MonoBehaviour
{
	private void Awake()
	{
		this.layerMask = LayerMask.GetMask(new string[] { "Gorilla Tag Collider" });
		this.overlapColliders = new Collider[10];
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
	}

	private void LateUpdate()
	{
		this.rigs.Clear();
		for (int i = 0; i < this.overlapColliders.Length; i++)
		{
			this.overlapColliders[i] = null;
		}
		float num = -1f;
		float num2 = Mathf.Cos(this.lookAtAngleDegrees / 180f * 3.1415927f);
		Physics.OverlapSphereNonAlloc(base.transform.position, this.lookRadius, this.overlapColliders, this.layerMask);
		foreach (Collider collider in this.overlapColliders)
		{
			if (collider != null)
			{
				VRRig componentInParent = collider.GetComponentInParent<VRRig>();
				if (componentInParent != null && componentInParent != this.myRig && !this.rigs.Contains(componentInParent))
				{
					Vector3 vector = (componentInParent.tagSound.transform.position - base.transform.position).normalized;
					float num3 = Vector3.Dot(-base.transform.up, vector);
					if (num3 > num2)
					{
						this.rigs.Add(componentInParent);
					}
				}
			}
		}
		this.lookTarget = null;
		foreach (VRRig vrrig in this.rigs)
		{
			Vector3 vector = (vrrig.tagSound.transform.position - base.transform.position).normalized;
			float num3 = Vector3.Dot(base.transform.forward, vector);
			if (num3 > num)
			{
				num = num3;
				this.lookTarget = vrrig.tagSound.transform;
			}
		}
		Vector3 vector2 = this.neck.forward;
		if (this.lookTarget != null)
		{
			vector2 = (this.lookTarget.position - this.head.position).normalized;
		}
		Vector3 vector3 = this.neck.InverseTransformDirection(vector2);
		vector3.y = Mathf.Clamp(vector3.y, this.minNeckY, this.maxNeckY);
		vector2 = this.neck.TransformDirection(vector3.normalized);
		Vector3 vector4 = Vector3.RotateTowards(this.head.forward, vector2, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
		this.head.rotation = Quaternion.LookRotation(vector4, this.neck.up);
	}

	public Transform head;

	public Transform lookTarget;

	public Transform neck;

	public float lookRadius = 0.5f;

	public Collider[] overlapColliders;

	public List<VRRig> rigs = new List<VRRig>();

	public LayerMask layerMask;

	public float rotSpeed = 1f;

	public float lookAtAngleDegrees = 60f;

	public float maxNeckY;

	public float minNeckY;

	public VRRig myRig;
}
