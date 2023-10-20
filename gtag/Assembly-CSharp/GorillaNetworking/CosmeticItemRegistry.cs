using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002AE RID: 686
	public class CosmeticItemRegistry
	{
		// Token: 0x060011E7 RID: 4583 RVA: 0x00066140 File Offset: 0x00064340
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

		// Token: 0x060011E8 RID: 4584 RVA: 0x00066212 File Offset: 0x00064412
		public CosmeticItemInstance Cosmetic(string itemName)
		{
			if (itemName.Length == 0 || itemName == "NOTHING")
			{
				return null;
			}
			return this.nameToCosmeticMap[itemName];
		}

		// Token: 0x040014CD RID: 5325
		private Dictionary<string, CosmeticItemInstance> nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		// Token: 0x040014CE RID: 5326
		private GameObject nullItem;
	}
}
