using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000162 RID: 354
internal class VrrigReliableSerializer : GorillaSerializer
{
	// Token: 0x060008C6 RID: 2246 RVA: 0x00035B1C File Offset: 0x00033D1C
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
