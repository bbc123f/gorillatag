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

		public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, true);
				if (cosmeticSlot == CosmeticsController.CosmeticSlots.Badge)
				{
					GorillaTagger.Instance.offlineVRRig.GetComponent<VRRigAnchorOverrides>().CurrentBadgeTransform = gameObject.transform;
				}
			}
			if (flag)
			{
				foreach (GameObject obj in this.leftObjects)
				{
					this.EnableItem(obj, true);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj2 in this.rightObjects)
				{
					this.EnableItem(obj2, true);
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
