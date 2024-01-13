using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public static class Utils
{
	private static List<IPreDisable> tempDisableCallbacks = new List<IPreDisable>();

	private static int tempListcount = 0;

	public static void Disable(this GameObject target)
	{
		if (target.activeSelf)
		{
			target.GetComponents(tempDisableCallbacks);
			tempListcount = tempDisableCallbacks.Count;
			for (int i = 0; i < tempListcount; i++)
			{
				tempDisableCallbacks[i].PreDisable();
			}
			target.SetActive(value: false);
		}
	}

	public static bool InRoom(this Player player)
	{
		if (PhotonNetwork.InRoom)
		{
			return PhotonNetwork.CurrentRoom.Players.ContainsKey(player.ActorNumber);
		}
		return false;
	}
}
