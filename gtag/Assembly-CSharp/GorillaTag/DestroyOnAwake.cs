using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020002FF RID: 767
	public class DestroyOnAwake : MonoBehaviour
	{
		// Token: 0x06001591 RID: 5521 RVA: 0x000779C0 File Offset: 0x00075BC0
		protected void Awake()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
