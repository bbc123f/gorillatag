using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002AD RID: 685
	public class CosmeticItemInstance
	{
		// Token: 0x060011E3 RID: 4579 RVA: 0x00065D80 File Offset: 0x00063F80
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

		// Token: 0x060011E4 RID: 4580 RVA: 0x00065DC0 File Offset: 0x00063FC0
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

		// Token: 0x060011E5 RID: 4581 RVA: 0x00065EB4 File Offset: 0x000640B4
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

		// Token: 0x040014C2 RID: 5314
		public List<GameObject> leftObjects = new List<GameObject>();

		// Token: 0x040014C3 RID: 5315
		public List<GameObject> rightObjects = new List<GameObject>();

		// Token: 0x040014C4 RID: 5316
		public List<GameObject> objects = new List<GameObject>();
	}
}
