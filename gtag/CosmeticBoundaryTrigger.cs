using GorillaNetworking;
using UnityEngine;

public class CosmeticBoundaryTrigger : GorillaTriggerBox
{
	public VRRig rigRef;

	public void OnTriggerEnter(Collider other)
	{
		if (!(other.attachedRigidbody == null))
		{
			rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (!(rigRef == null))
			{
				rigRef.inTryOnRoom = true;
				rigRef.LocalUpdateCosmeticsWithTryon(rigRef.cosmeticSet, rigRef.tryOnSet);
				rigRef.myBodyDockPositions.RefreshTransferrableItems();
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (!(rigRef == null))
		{
			rigRef.inTryOnRoom = false;
			if (rigRef.isOfflineVRRig)
			{
				rigRef.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
				CosmeticsController.instance.ClearCheckout();
				CosmeticsController.instance.UpdateShoppingCart();
			}
			rigRef.LocalUpdateCosmeticsWithTryon(rigRef.cosmeticSet, rigRef.tryOnSet);
			rigRef.myBodyDockPositions.RefreshTransferrableItems();
		}
	}
}
