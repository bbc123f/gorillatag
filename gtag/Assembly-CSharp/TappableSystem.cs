using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class TappableSystem : GTSystem<Tappable>
{
	// Token: 0x06000AB8 RID: 2744 RVA: 0x0004223C File Offset: 0x0004043C
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
