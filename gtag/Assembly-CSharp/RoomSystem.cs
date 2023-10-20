using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001F9 RID: 505
internal class RoomSystem : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x06000CED RID: 3309 RVA: 0x0004CD89 File Offset: 0x0004AF89
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		PhotonNetwork.AddCallbackTarget(this);
		RoomSystem.callbackInstance = this;
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x0004CDA0 File Offset: 0x0004AFA0
	private void Start()
	{
		List<PhotonView> list = new List<PhotonView>(20);
		foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
		{
			if (photonView.IsRoomView)
			{
				list.Add(photonView);
			}
		}
		RoomSystem.sceneViews = list.ToArray();
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x0004CE10 File Offset: 0x0004B010
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
		RoomSystem.joinedRoom = true;
		foreach (Player item in PhotonNetwork.CurrentRoom.Players.Values)
		{
			RoomSystem.playersInRoom.Add(item);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(RoomSystem.playersInRoom);
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj))
		{
			string text = obj as string;
			if (text != null)
			{
				RoomSystem.roomGameMode = text;
			}
		}
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x0004CEA8 File Offset: 0x0004B0A8
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
		RoomSystem.playersInRoom.Add(newPlayer);
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(newPlayer);
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x0004CEBC File Offset: 0x0004B0BC
	void IMatchmakingCallbacks.OnLeftRoom()
	{
		RoomSystem.joinedRoom = false;
		RoomSystem.playersInRoom.Clear();
		RoomSystem.roomGameMode = "";
		PlayerCosmeticsSystem.StaticReset();
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		for (int i = 0; i < RoomSystem.sceneViews.Length; i++)
		{
			RoomSystem.sceneViews[i].ControllerActorNr = actorNumber;
			RoomSystem.sceneViews[i].OwnerActorNr = actorNumber;
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x0004CF1F File Offset: 0x0004B11F
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
		RoomSystem.playersInRoom.Remove(otherPlayer);
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x0004CF2D File Offset: 0x0004B12D
	public static List<Player> PlayersInRoom
	{
		get
		{
			return RoomSystem.playersInRoom;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000CF4 RID: 3316 RVA: 0x0004CF34 File Offset: 0x0004B134
	public static string RoomGameMode
	{
		get
		{
			return RoomSystem.roomGameMode;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000CF5 RID: 3317 RVA: 0x0004CF3B File Offset: 0x0004B13B
	public static bool JoinedRoom
	{
		get
		{
			return PhotonNetwork.InRoom && RoomSystem.joinedRoom;
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000CF6 RID: 3318 RVA: 0x0004CF4B File Offset: 0x0004B14B
	public static bool AmITheHost
	{
		get
		{
			return PhotonNetwork.IsMasterClient || !PhotonNetwork.InRoom;
		}
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x0004CF5E File Offset: 0x0004B15E
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x0004CF60 File Offset: 0x0004B160
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0004CF62 File Offset: 0x0004B162
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x0004CF64 File Offset: 0x0004B164
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x0004CF66 File Offset: 0x0004B166
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x0004CF68 File Offset: 0x0004B168
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x0004CF6A File Offset: 0x0004B16A
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x0004CF6C File Offset: 0x0004B16C
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x0004CF6E File Offset: 0x0004B16E
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x0400102F RID: 4143
	private static RoomSystem callbackInstance;

	// Token: 0x04001030 RID: 4144
	private static List<Player> playersInRoom = new List<Player>(10);

	// Token: 0x04001031 RID: 4145
	private static string roomGameMode = "";

	// Token: 0x04001032 RID: 4146
	private static bool joinedRoom = false;

	// Token: 0x04001033 RID: 4147
	private static PhotonView[] sceneViews;
}
