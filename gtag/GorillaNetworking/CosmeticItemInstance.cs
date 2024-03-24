using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	public class CosmeticItemInstance
	{
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

		public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, false);
			}
			if (flag)
			{
				foreach (GameObject gameObject2 in this.leftObjects)
				{
					this.EnableItem(gameObject2, false);
				}
			}
			if (flag2)
			{
				foreach (GameObject gameObject3 in this.rightObjects)
				{
					this.EnableItem(gameObject3, false);
				}
			}
		}

		public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, true);
			}
			if (flag)
			{
				foreach (GameObject gameObject2 in this.leftObjects)
				{
					this.EnableItem(gameObject2, true);
				}
			}
			if (flag2)
			{
				foreach (GameObject gameObject3 in this.rightObjects)
				{
					this.EnableItem(gameObject3, true);
				}
			}
		}

		public CosmeticItemInstance()
		{
		}

		public List<GameObject> leftObjects = new List<GameObject>();

		public List<GameObject> rightObjects = new List<GameObject>();

		public List<GameObject> objects = new List<GameObject>();
	}
}
