using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking;

public class CosmeticItemInstance
{
	public List<GameObject> leftObjects = new List<GameObject>();

	public List<GameObject> rightObjects = new List<GameObject>();

	public List<GameObject> objects = new List<GameObject>();

	private void EnableItem(GameObject obj, bool enable)
	{
		CosmeticAnchors component = obj.GetComponent<CosmeticAnchors>();
		if ((bool)component && !enable)
		{
			component.EnableAnchor(enable: false);
		}
		obj.SetActive(enable);
		if ((bool)component && enable)
		{
			component.EnableAnchor(enable: true);
		}
	}

	public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
	{
		bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
		bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
		foreach (GameObject @object in objects)
		{
			EnableItem(@object, enable: false);
		}
		if (flag)
		{
			foreach (GameObject leftObject in leftObjects)
			{
				EnableItem(leftObject, enable: false);
			}
		}
		if (!flag2)
		{
			return;
		}
		foreach (GameObject rightObject in rightObjects)
		{
			EnableItem(rightObject, enable: false);
		}
	}

	public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
	{
		bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
		bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
		foreach (GameObject @object in objects)
		{
			EnableItem(@object, enable: true);
		}
		if (flag)
		{
			foreach (GameObject leftObject in leftObjects)
			{
				EnableItem(leftObject, enable: true);
			}
		}
		if (!flag2)
		{
			return;
		}
		foreach (GameObject rightObject in rightObjects)
		{
			EnableItem(rightObject, enable: true);
		}
	}
}
