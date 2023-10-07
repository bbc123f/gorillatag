using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000312 RID: 786
	public class DeactivateOnAwake : MonoBehaviour
	{
		// Token: 0x060015B8 RID: 5560 RVA: 0x00077DA6 File Offset: 0x00075FA6
		private void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
