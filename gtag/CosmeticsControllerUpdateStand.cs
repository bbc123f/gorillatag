using GorillaNetworking;
using UnityEngine;

public class CosmeticsControllerUpdateStand : MonoBehaviour
{
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

	public GameObject ReturnChildWithCosmeticNameMatch(Transform parentTransform)
	{
		GameObject gameObject = null;
		foreach (Transform child in parentTransform)
		{
			if (child.gameObject.activeInHierarchy && cosmeticsController.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => child.name == x.itemName) > -1)
			{
				return child.gameObject;
			}
			gameObject = ReturnChildWithCosmeticNameMatch(child);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return gameObject;
	}

	public void UpdateInventoryHeadModels()
	{
		HeadModel[] array = inventoryHeadModels;
		foreach (HeadModel headModel in array)
		{
			for (int j = 0; j < headModel.transform.childCount; j++)
			{
				for (int k = 0; k < headModel.transform.GetChild(j).gameObject.transform.childCount; k++)
				{
					for (int l = 0; l < headModel.transform.GetChild(j).gameObject.transform.GetChild(k).gameObject.transform.childCount; l++)
					{
						if (!headModel.transform.GetChild(j).gameObject.transform.GetChild(k).gameObject.transform.GetChild(l).gameObject.activeInHierarchy)
						{
							headModel.transform.GetChild(j).gameObject.transform.GetChild(k).gameObject.transform.GetChild(l).gameObject.SetActive(value: true);
						}
					}
				}
			}
		}
	}
}
