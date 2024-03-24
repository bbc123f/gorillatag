using System;
using Photon.Pun;
using UnityEngine;

public class HoldableObject : MonoBehaviourPunCallbacks
{
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (!(this == EquipmentInteractor.instance.rightHandHeldEquipment) || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (!(this == EquipmentInteractor.instance.leftHandHeldEquipment) || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	public virtual void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	public virtual void DropItemCleanup()
	{
	}

	public HoldableObject()
	{
	}
}
