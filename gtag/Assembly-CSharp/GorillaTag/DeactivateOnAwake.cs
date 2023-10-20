using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000314 RID: 788
	public class DeactivateOnAwake : MonoBehaviour
	{
		// Token: 0x060015C1 RID: 5569 RVA: 0x0007828E File Offset: 0x0007648E
		private void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
