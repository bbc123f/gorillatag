using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class YorickLook : MonoBehaviour
{
	// Token: 0x060007AA RID: 1962 RVA: 0x00030B98 File Offset: 0x0002ED98
	private void Awake()
	{
		this.layerMask = LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		});
		this.overlapColliders = new Collider[10];
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x00030BC8 File Offset: 0x0002EDC8
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

	// Token: 0x04000945 RID: 2373
	public Transform leftEye;

	// Token: 0x04000946 RID: 2374
	public Transform rightEye;

	// Token: 0x04000947 RID: 2375
	public Transform lookTarget;

	// Token: 0x04000948 RID: 2376
	public float lookRadius = 0.5f;

	// Token: 0x04000949 RID: 2377
	public Collider[] overlapColliders;

	// Token: 0x0400094A RID: 2378
	public List<VRRig> rigs = new List<VRRig>();

	// Token: 0x0400094B RID: 2379
	public LayerMask layerMask;

	// Token: 0x0400094C RID: 2380
	public float rotSpeed = 1f;

	// Token: 0x0400094D RID: 2381
	public float lookAtAngleDegrees = 60f;
}
