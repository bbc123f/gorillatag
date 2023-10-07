using System;
using UnityEngine;

// Token: 0x0200005C RID: 92
public class GTDisableStaticOnAwake : MonoBehaviour
{
	// Token: 0x060001C0 RID: 448 RVA: 0x0000C9CE File Offset: 0x0000ABCE
	private void Awake()
	{
		base.gameObject.isStatic = false;
		Object.Destroy(this);
	}
}
