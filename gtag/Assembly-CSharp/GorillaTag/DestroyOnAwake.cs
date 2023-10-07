using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020002FD RID: 765
	public class DestroyOnAwake : MonoBehaviour
	{
		// Token: 0x06001588 RID: 5512 RVA: 0x000774D8 File Offset: 0x000756D8
		protected void Awake()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
