using System;
using Photon.Pun;
using UnityEngine;

internal class VrrigReliableSerializer : GorillaSerializer
{
	protected override bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (info.Sender != info.photonView.Owner || this.photonView.IsRoomView)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			outTargetObject = rigContainer.gameObject;
			outTargetType = typeof(VRRigReliableState);
			return true;
		}
		return false;
	}
}
