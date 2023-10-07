using System;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class ShoppingCart : MonoBehaviour
{
	// Token: 0x060007B2 RID: 1970 RVA: 0x00031103 File Offset: 0x0002F303
	public void Awake()
	{
		if (ShoppingCart.instance == null)
		{
			ShoppingCart.instance = this;
			return;
		}
		if (ShoppingCart.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00031137 File Offset: 0x0002F337
	private void Start()
	{
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00031139 File Offset: 0x0002F339
	private void Update()
	{
	}

	// Token: 0x04000951 RID: 2385
	public static volatile ShoppingCart instance;
}
