using System;
using UnityEngine;

// Token: 0x0200000E RID: 14
public class PlatformerCollectiblesMain : MonoBehaviour
{
	// Token: 0x0600002B RID: 43 RVA: 0x00002F70 File Offset: 0x00001170
	public void Start()
	{
		int num = 0;
		while ((float)num < this.CoinGridCount)
		{
			float x = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num / (this.CoinGridCount - 1f);
			int num2 = 0;
			while ((float)num2 < this.CoinGridCount)
			{
				float z = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num2 / (this.CoinGridCount - 1f);
				Object.Instantiate<GameObject>(this.Coin).transform.position = new Vector3(x, 0.2f, z);
				num2++;
			}
			num++;
		}
	}

	// Token: 0x0400002E RID: 46
	public GameObject Coin;

	// Token: 0x0400002F RID: 47
	public float CoinGridCount = 5f;

	// Token: 0x04000030 RID: 48
	public float CoinGridSize = 7f;
}
