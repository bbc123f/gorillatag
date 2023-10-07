using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class GorillaBodyPhysics : MonoBehaviour
{
	// Token: 0x06000812 RID: 2066 RVA: 0x00032E25 File Offset: 0x00031025
	private void FixedUpdate()
	{
		this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
	}

	// Token: 0x040009C7 RID: 2503
	public GameObject bodyCollider;

	// Token: 0x040009C8 RID: 2504
	public Vector3 bodyColliderOffset;

	// Token: 0x040009C9 RID: 2505
	public Transform headsetTransform;
}
