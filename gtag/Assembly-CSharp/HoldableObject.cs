using System;
using Photon.Pun;
using UnityEngine;

public class HoldableObject : MonoBehaviourPunCallbacks
{
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	public virtual void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	public virtual void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (!(this == EquipmentInteractor.instance.rightHandHeldEquipment) || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (!(this == EquipmentInteractor.instance.leftHandHeldEquipment) || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	public virtual void DropItemCleanup()
	{
	}

	public HoldableObject()
	{
	}
}
