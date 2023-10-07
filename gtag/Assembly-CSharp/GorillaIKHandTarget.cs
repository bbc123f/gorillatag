using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x0200013E RID: 318
public class GorillaIKHandTarget : MonoBehaviour
{
	// Token: 0x06000823 RID: 2083 RVA: 0x000330E1 File Offset: 0x000312E1
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x000330F4 File Offset: 0x000312F4
	private void FixedUpdate()
	{
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0003312C File Offset: 0x0003132C
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x040009DC RID: 2524
	public GameObject handToStickTo;

	// Token: 0x040009DD RID: 2525
	public bool isLeftHand;

	// Token: 0x040009DE RID: 2526
	public float hapticStrength;

	// Token: 0x040009DF RID: 2527
	private Rigidbody thisRigidbody;

	// Token: 0x040009E0 RID: 2528
	private XRController controllerReference;
}
