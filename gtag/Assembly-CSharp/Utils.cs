using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200022F RID: 559
public static class Utils
{
	// Token: 0x06000DD8 RID: 3544 RVA: 0x00050794 File Offset: 0x0004E994
	public static void Disable(this GameObject target)
	{
		if (!target.activeSelf)
		{
			return;
		}
		target.GetComponents<IPreDisable>(Utils.tempDisableCallbacks);
		Utils.tempListcount = Utils.tempDisableCallbacks.Count;
		for (int i = 0; i < Utils.tempListcount; i++)
		{
			Utils.tempDisableCallbacks[i].PreDisable();
		}
		target.SetActive(false);
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x000507EB File Offset: 0x0004E9EB
	public static bool InRoom(this Player player)
	{
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.ContainsKey(player.ActorNumber);
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0005080B File Offset: 0x0004EA0B
	public static bool PlayerInRoom(int actorNumber)
	{
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.ContainsKey(actorNumber);
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00050826 File Offset: 0x0004EA26
	public static bool PlayerInRoom(int actorNumer, out Player photonPlayer)
	{
		photonPlayer = null;
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumer, out photonPlayer);
	}

	// Token: 0x040010C5 RID: 4293
	private static List<IPreDisable> tempDisableCallbacks = new List<IPreDisable>();

	// Token: 0x040010C6 RID: 4294
	private static int tempListcount = 0;
}
