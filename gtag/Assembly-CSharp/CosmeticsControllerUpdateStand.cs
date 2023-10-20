using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class CosmeticsControllerUpdateStand : MonoBehaviour
{
	// Token: 0x06000640 RID: 1600 RVA: 0x000274AC File Offset: 0x000256AC
	public GameObject ReturnChildWithCosmeticNameMatch(Transform parentTransform)
	{
		GameObject gameObject = null;
		using (IEnumerator enumerator = parentTransform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = (Transform)enumerator.Current;
				if (child.gameObject.activeInHierarchy && this.cosmeticsController.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => child.name == x.itemName) > -1)
				{
					return child.gameObject;
				}
				gameObject = this.ReturnChildWithCosmeticNameMatch(child);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
		}
		return gameObject;
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00027568 File Offset: 0x00025768
	public void UpdateInventoryHeadModels()
	{
		foreach (HeadModel headModel in this.inventoryHeadModels)
		{
			for (int j = 0; j < headModel.transform.childCount; j++)
			{
				for (int k = 0; k < headModel.transform.GetChild(j).gameObject.transform.childCount; k++)
				{
					for (int l = 0; l < headModel.transform.GetChild(j).gameObject.transform.GetChild(k).gameObject.transform.childCount; l++)
					{
						if (!headModel.transform.GetChild(j).gameObject.transform.GetChild(k).gameObject.transform.GetChild(l).gameObject.activeInHierarchy)
						{
							headModel.transform.GetChild(j).gameObject.transform.GetChild(k).gameObject.transform.GetChild(l).gameObject.SetActive(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x04000798 RID: 1944
	public CosmeticsController cosmeticsController;

	// Token: 0x04000799 RID: 1945
	public bool FailEntitlement;

	// Token: 0x0400079A RID: 1946
	public bool PlayerUnlocked;

	// Token: 0x0400079B RID: 1947
	public bool ItemNotGrantedYet;

	// Token: 0x0400079C RID: 1948
	public bool ItemSuccessfullyGranted;

	// Token: 0x0400079D RID: 1949
	public bool AttemptToConsumeEntitlement;

	// Token: 0x0400079E RID: 1950
	public bool EntitlementSuccessfullyConsumed;

	// Token: 0x0400079F RID: 1951
	public bool LockSuccessfullyCleared;

	// Token: 0x040007A0 RID: 1952
	public bool RunDebug;

	// Token: 0x040007A1 RID: 1953
	public Transform textParent;

	// Token: 0x040007A2 RID: 1954
	private CosmeticsController.CosmeticItem outItem;

	// Token: 0x040007A3 RID: 1955
	public HeadModel[] inventoryHeadModels;
}
