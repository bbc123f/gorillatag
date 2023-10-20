using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002AF RID: 687
	public class CosmeticItemInstance
	{
		// Token: 0x060011EA RID: 4586 RVA: 0x0006624C File Offset: 0x0006444C
		private void EnableItem(GameObject obj, bool enable)
		{
			CosmeticAnchors component = obj.GetComponent<CosmeticAnchors>();
			if (component && !enable)
			{
				component.EnableAnchor(false);
			}
			obj.SetActive(enable);
			if (component && enable)
			{
				component.EnableAnchor(true);
			}
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x0006628C File Offset: 0x0006448C
		public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject obj in this.objects)
			{
				this.EnableItem(obj, false);
			}
			if (flag)
			{
				foreach (GameObject obj2 in this.leftObjects)
				{
					this.EnableItem(obj2, false);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj3 in this.rightObjects)
				{
					this.EnableItem(obj3, false);
				}
			}
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x00066380 File Offset: 0x00064580
		public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject obj in this.objects)
			{
				this.EnableItem(obj, true);
			}
			if (flag)
			{
				foreach (GameObject obj2 in this.leftObjects)
				{
					this.EnableItem(obj2, true);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj3 in this.rightObjects)
				{
					this.EnableItem(obj3, true);
				}
			}
		}

		// Token: 0x040014CF RID: 5327
		public List<GameObject> leftObjects = new List<GameObject>();

		// Token: 0x040014D0 RID: 5328
		public List<GameObject> rightObjects = new List<GameObject>();

		// Token: 0x040014D1 RID: 5329
		public List<GameObject> objects = new List<GameObject>();
	}
}
