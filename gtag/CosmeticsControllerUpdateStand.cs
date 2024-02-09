using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

public class CosmeticsControllerUpdateStand : MonoBehaviour
{
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

	public CosmeticsController cosmeticsController;

	public bool FailEntitlement;

	public bool PlayerUnlocked;

	public bool ItemNotGrantedYet;

	public bool ItemSuccessfullyGranted;

	public bool AttemptToConsumeEntitlement;

	public bool EntitlementSuccessfullyConsumed;

	public bool LockSuccessfullyCleared;

	public bool RunDebug;

	public Transform textParent;

	private CosmeticsController.CosmeticItem outItem;

	public HeadModel[] inventoryHeadModels;
}
