using System;
using Photon.Pun;
using UnityEngine;

internal class VrrigReliableSerializer : GorillaSerializer
{
	protected override bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (info.Sender != info.photonView.Owner || photonView.IsRoomView)
		{
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out var playerRig))
		{
			outTargetObject = playerRig.gameObject;
			outTargetType = typeof(VRRigReliableState);
			return true;
		}
		return false;
	}
}
