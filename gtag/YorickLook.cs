using System;
using System.Collections.Generic;
using UnityEngine;

public class YorickLook : MonoBehaviour
{
	public Transform leftEye;

	public Transform rightEye;

	public Transform lookTarget;

	public float lookRadius = 0.5f;

	public Collider[] overlapColliders;

	public List<VRRig> rigs = new List<VRRig>();

	public LayerMask layerMask;

	public float rotSpeed = 1f;

	public float lookAtAngleDegrees = 60f;

	private void Awake()
	{
		layerMask = LayerMask.GetMask("Gorilla Tag Collider");
		overlapColliders = new Collider[10];
	}

	private void LateUpdate()
	{
		rigs.Clear();
		for (int i = 0; i < overlapColliders.Length; i++)
		{
			overlapColliders[i] = null;
		}
		float num = -1f;
		float num2 = Mathf.Cos(lookAtAngleDegrees / 180f * (float)Math.PI);
		Physics.OverlapSphereNonAlloc(base.transform.position, lookRadius, overlapColliders, layerMask);
		Collider[] array = overlapColliders;
		foreach (Collider collider in array)
		{
			if (!(collider != null))
			{
				continue;
			}
			VRRig componentInParent = collider.GetComponentInParent<VRRig>();
			if (componentInParent != null && !rigs.Contains(componentInParent))
			{
				Vector3 normalized = (componentInParent.tagSound.transform.position - base.transform.position).normalized;
				float num3 = Vector3.Dot(-base.transform.up, normalized);
				if (num3 > num2)
				{
					rigs.Add(componentInParent);
				}
			}
		}
		lookTarget = null;
		foreach (VRRig rig in rigs)
		{
			Vector3 normalized = (rig.tagSound.transform.position - base.transform.position).normalized;
			float num3 = Vector3.Dot(base.transform.forward, normalized);
			if (num3 > num)
			{
				num = num3;
				lookTarget = rig.tagSound.transform;
			}
		}
		Vector3 target = -base.transform.up;
		Vector3 target2 = -base.transform.up;
		if (lookTarget != null)
		{
			target = (lookTarget.position - leftEye.position).normalized;
			target2 = (lookTarget.position - rightEye.position).normalized;
		}
		Vector3 forward = Vector3.RotateTowards(leftEye.rotation * Vector3.forward, target, rotSpeed * (float)Math.PI, 0f);
		Vector3 forward2 = Vector3.RotateTowards(rightEye.rotation * Vector3.forward, target2, rotSpeed * (float)Math.PI, 0f);
		leftEye.rotation = Quaternion.LookRotation(forward);
		rightEye.rotation = Quaternion.LookRotation(forward2);
	}
}
