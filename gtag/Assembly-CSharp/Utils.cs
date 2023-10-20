using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000230 RID: 560
public static class Utils
{
	// Token: 0x06000DDE RID: 3550 RVA: 0x000509F4 File Offset: 0x0004EBF4
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

	// Token: 0x06000DDF RID: 3551 RVA: 0x00050A4B File Offset: 0x0004EC4B
	public static bool InRoom(this Player player)
	{
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.ContainsKey(player.ActorNumber);
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x00050A6B File Offset: 0x0004EC6B
	public static bool PlayerInRoom(int actorNumber)
	{
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.ContainsKey(actorNumber);
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00050A86 File Offset: 0x0004EC86
	public static bool PlayerInRoom(int actorNumer, out Player photonPlayer)
	{
		photonPlayer = null;
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumer, out photonPlayer);
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00050AA8 File Offset: 0x0004ECA8
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

	// Token: 0x040010CA RID: 4298
	private static List<IPreDisable> tempDisableCallbacks = new List<IPreDisable>();

	// Token: 0x040010CB RID: 4299
	private static int tempListcount = 0;

	// Token: 0x040010CC RID: 4300
	private static StringBuilder reusableSB = new StringBuilder();
}
