using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class TappableSystem : GTSystem<Tappable>
{
	// Token: 0x06000ABD RID: 2749 RVA: 0x00042374 File Offset: 0x00040574
	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SendOnTapRPC");
		if (key < 0 || key >= this._instances.Count)
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		this._instances[key].OnTapLocal(tapStrength, Time.time);
	}
}
