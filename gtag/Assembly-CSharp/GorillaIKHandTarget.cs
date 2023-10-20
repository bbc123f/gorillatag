using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x0200013E RID: 318
public class GorillaIKHandTarget : MonoBehaviour
{
	// Token: 0x06000824 RID: 2084 RVA: 0x00032F21 File Offset: 0x00031121
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x00032F34 File Offset: 0x00031134
	private void FixedUpdate()
	{
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x00032F6C File Offset: 0x0003116C
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
