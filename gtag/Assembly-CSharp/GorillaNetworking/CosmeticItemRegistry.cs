using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002AC RID: 684
	public class CosmeticItemRegistry
	{
		// Token: 0x060011E0 RID: 4576 RVA: 0x00065C74 File Offset: 0x00063E74
		public void Initialize(GameObject[] cosmetics)
		{
			foreach (GameObject gameObject in cosmetics)
			{
				string key = gameObject.name.Replace("LEFT.", "").Replace("RIGHT.", "");
				CosmeticItemInstance cosmeticItemInstance;
				if (this.nameToCosmeticMap.ContainsKey(key))
				{
					cosmeticItemInstance = this.nameToCosmeticMap[key];
				}
				else
				{
					cosmeticItemInstance = new CosmeticItemInstance();
					this.nameToCosmeticMap.Add(key, cosmeticItemInstance);
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

		// Token: 0x060011E1 RID: 4577 RVA: 0x00065D46 File Offset: 0x00063F46
		public CosmeticItemInstance Cosmetic(string itemName)
		{
			if (itemName.Length == 0 || itemName == "NOTHING")
			{
				return null;
			}
			return this.nameToCosmeticMap[itemName];
		}

		// Token: 0x040014C0 RID: 5312
		private Dictionary<string, CosmeticItemInstance> nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		// Token: 0x040014C1 RID: 5313
		private GameObject nullItem;
	}
}
