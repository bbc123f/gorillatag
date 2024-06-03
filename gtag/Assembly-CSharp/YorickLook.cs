using System;
using System.Collections.Generic;
using UnityEngine;

public class YorickLook : MonoBehaviour
{
	private void Awake()
	{
		this.layerMask = LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		});
		this.overlapColliders = new Collider[10];
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
				if (componentInParent != null && !this.rigs.Contains(componentInParent))
				{
					Vector3 normalized = (componentInParent.tagSound.transform.position - base.transform.position).normalized;
					float num3 = Vector3.Dot(-base.transform.up, normalized);
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
			Vector3 normalized = (vrrig.tagSound.transform.position - base.transform.position).normalized;
			float num3 = Vector3.Dot(base.transform.forward, normalized);
			if (num3 > num)
			{
				num = num3;
				this.lookTarget = vrrig.tagSound.transform;
			}
		}
		Vector3 target = -base.transform.up;
		Vector3 target2 = -base.transform.up;
		if (this.lookTarget != null)
		{
			target = (this.lookTarget.position - this.leftEye.position).normalized;
			target2 = (this.lookTarget.position - this.rightEye.position).normalized;
		}
		Vector3 forward = Vector3.RotateTowards(this.leftEye.rotation * Vector3.forward, target, this.rotSpeed * 3.1415927f, 0f);
		Vector3 forward2 = Vector3.RotateTowards(this.rightEye.rotation * Vector3.forward, target2, this.rotSpeed * 3.1415927f, 0f);
		this.leftEye.rotation = Quaternion.LookRotation(forward);
		this.rightEye.rotation = Quaternion.LookRotation(forward2);
	}

	public YorickLook()
	{
	}

	public Transform leftEye;

	public Transform rightEye;

	public Transform lookTarget;

	public float lookRadius = 0.5f;

	public Collider[] overlapColliders;

	public List<VRRig> rigs = new List<VRRig>();

	public LayerMask layerMask;

	public float rotSpeed = 1f;

	public float lookAtAngleDegrees = 60f;
}
