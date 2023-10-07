using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class BetaChecker : MonoBehaviour
{
	// Token: 0x0600062D RID: 1581 RVA: 0x00027152 File Offset: 0x00025352
	private void Start()
	{
		if (PlayerPrefs.GetString("CheckedBox2") == "true")
		{
			this.doNotEnable = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00027180 File Offset: 0x00025380
	private void Update()
	{
		if (!this.doNotEnable)
		{
			if (CosmeticsController.instance.confirmedDidntPlayInBeta)
			{
				PlayerPrefs.SetString("CheckedBox2", "true");
				PlayerPrefs.Save();
				base.gameObject.SetActive(false);
				return;
			}
			if (CosmeticsController.instance.playedInBeta)
			{
				GameObject[] array = this.objectsToEnable;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				this.doNotEnable = true;
			}
		}
	}

	// Token: 0x04000786 RID: 1926
	public GameObject[] objectsToEnable;

	// Token: 0x04000787 RID: 1927
	public bool doNotEnable;
}
