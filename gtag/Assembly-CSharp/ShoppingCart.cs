using System;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class ShoppingCart : MonoBehaviour
{
	// Token: 0x060007B3 RID: 1971 RVA: 0x00030F43 File Offset: 0x0002F143
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

	// Token: 0x060007B4 RID: 1972 RVA: 0x00030F77 File Offset: 0x0002F177
	private void Start()
	{
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00030F79 File Offset: 0x0002F179
	private void Update()
	{
	}

	// Token: 0x04000951 RID: 2385
	public static volatile ShoppingCart instance;
}
