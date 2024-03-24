using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public static class Utils
{
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

	public static bool InRoom(this Player player)
	{
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.ContainsKey(player.ActorNumber);
	}

	public static bool InRoom(this NetPlayer player)
	{
		return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.AllNetPlayers.Contains(player);
	}

	public static bool PlayerInRoom(int actorNumber)
	{
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.ContainsKey(actorNumber);
	}

	public static bool PlayerInRoom(int actorNumer, out Player photonPlayer)
	{
		photonPlayer = null;
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumer, out photonPlayer);
	}

	internal static string ToStringStripped(this Room room)
	{
		Utils.reusableSB.Clear();
		Utils.reusableSB.AppendFormat("Room: '{0}' ", (room.Name.Length < 20) ? room.Name : room.Name.Remove(20));
		Utils.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			room.IsVisible ? "visible" : "hidden",
			room.IsOpen ? "open" : "closed",
			room.MaxPlayers,
			room.PlayerCount
		});
		Utils.reusableSB.Append("\ncustomProps: {");
		Utils.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary customProperties = room.CustomProperties;
		if (customProperties.Contains("gameMode"))
		{
			object obj = customProperties["gameMode"];
			if (obj == null)
			{
				Utils.reusableSB.AppendFormat("gameMode=null}", Array.Empty<object>());
			}
			else
			{
				string text = obj as string;
				if (text != null)
				{
					Utils.reusableSB.AppendFormat("gameMode={0}", (text.Length < 50) ? text : text.Remove(50));
				}
			}
		}
		Utils.reusableSB.Append("}");
		return Utils.reusableSB.ToString();
	}

	// Note: this type is marked as 'beforefieldinit'.
	static Utils()
	{
	}

	private static List<IPreDisable> tempDisableCallbacks = new List<IPreDisable>();

	private static int tempListcount = 0;

	private static StringBuilder reusableSB = new StringBuilder();
}
