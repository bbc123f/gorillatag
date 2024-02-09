using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	public class CosmeticItemRegistry
	{
		public void Initialize(GameObject[] cosmetics)
		{
			foreach (GameObject gameObject in cosmetics)
			{
				string text = gameObject.name.Replace("LEFT.", "").Replace("RIGHT.", "");
				CosmeticItemInstance cosmeticItemInstance;
				if (this.nameToCosmeticMap.ContainsKey(text))
				{
					cosmeticItemInstance = this.nameToCosmeticMap[text];
				}
				else
				{
					cosmeticItemInstance = new CosmeticItemInstance();
					this.nameToCosmeticMap.Add(text, cosmeticItemInstance);
				}
				bool flag = gameObject.name.Contains("LEFT.");
				bool flag2 = gameObject.name.Contains("RIGHT.");
				if (flag)
				{
					cosmeticItemInstance.leftObjects.Add(gameObject);
				}
				else if (flag2)
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
			return this.nameToCosmeticMap[itemName];
		}

		private Dictionary<string, CosmeticItemInstance> nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		private GameObject nullItem;
	}
}
