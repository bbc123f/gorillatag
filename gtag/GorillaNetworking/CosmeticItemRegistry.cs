using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking;

public class CosmeticItemRegistry
{
	private Dictionary<string, CosmeticItemInstance> nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

	private GameObject nullItem;

	public void Initialize(GameObject[] cosmetics)
	{
		foreach (GameObject gameObject in cosmetics)
		{
			CosmeticItemInstance cosmeticItemInstance = null;
			string key = gameObject.name.Replace("LEFT.", "").Replace("RIGHT.", "");
			if (nameToCosmeticMap.ContainsKey(key))
			{
				cosmeticItemInstance = nameToCosmeticMap[key];
			}
			else
			{
				cosmeticItemInstance = new CosmeticItemInstance();
				nameToCosmeticMap.Add(key, cosmeticItemInstance);
			}
			bool num = gameObject.name.Contains("LEFT.");
			bool flag = gameObject.name.Contains("RIGHT.");
			if (num)
			{
				cosmeticItemInstance.leftObjects.Add(gameObject);
			}
			else if (flag)
			{
				cosmeticItemInstance.rightObjects.Add(gameObject);
			}
			else
			{
				cosmeticItemInstance.objects.Add(gameObject);
			}
		}
	}

	public CosmeticItemInstance Cosmetic(string itemName)
	{
		if (itemName.Length == 0 || itemName == "NOTHING")
		{
			return null;
		}
		return nameToCosmeticMap[itemName];
	}
}
