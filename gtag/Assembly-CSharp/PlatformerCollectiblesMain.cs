using System;
using UnityEngine;

public class PlatformerCollectiblesMain : MonoBehaviour
{
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

	public GameObject Coin;

	public float CoinGridCount = 5f;

	public float CoinGridSize = 7f;
}
