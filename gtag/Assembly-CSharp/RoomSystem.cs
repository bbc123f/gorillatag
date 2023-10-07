using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001F8 RID: 504
internal class RoomSystem : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x06000CE7 RID: 3303 RVA: 0x0004CB29 File Offset: 0x0004AD29
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		PhotonNetwork.AddCallbackTarget(this);
		RoomSystem.callbackInstance = this;
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0004CB40 File Offset: 0x0004AD40
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

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0004CBB0 File Offset: 0x0004ADB0
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

	// Token: 0x06000CEA RID: 3306 RVA: 0x0004CC48 File Offset: 0x0004AE48
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
		RoomSystem.playersInRoom.Add(newPlayer);
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(newPlayer);
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x0004CC5C File Offset: 0x0004AE5C
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

	// Token: 0x06000CEC RID: 3308 RVA: 0x0004CCBF File Offset: 0x0004AEBF
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
		RoomSystem.playersInRoom.Remove(otherPlayer);
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000CED RID: 3309 RVA: 0x0004CCCD File Offset: 0x0004AECD
	public static List<Player> PlayersInRoom
	{
		get
		{
			return RoomSystem.playersInRoom;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000CEE RID: 3310 RVA: 0x0004CCD4 File Offset: 0x0004AED4
	public static string RoomGameMode
	{
		get
		{
			return RoomSystem.roomGameMode;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000CEF RID: 3311 RVA: 0x0004CCDB File Offset: 0x0004AEDB
	public static bool JoinedRoom
	{
		get
		{
			return PhotonNetwork.InRoom && RoomSystem.joinedRoom;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000CF0 RID: 3312 RVA: 0x0004CCEB File Offset: 0x0004AEEB
	public static bool AmITheHost
	{
		get
		{
			return PhotonNetwork.IsMasterClient || !PhotonNetwork.InRoom;
		}
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x0004CCFE File Offset: 0x0004AEFE
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x0004CD00 File Offset: 0x0004AF00
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x0004CD02 File Offset: 0x0004AF02
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x0004CD04 File Offset: 0x0004AF04
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x0004CD06 File Offset: 0x0004AF06
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x0004CD08 File Offset: 0x0004AF08
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x0004CD0A File Offset: 0x0004AF0A
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x0004CD0C File Offset: 0x0004AF0C
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0004CD0E File Offset: 0x0004AF0E
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x0400102A RID: 4138
	private static RoomSystem callbackInstance;

	// Token: 0x0400102B RID: 4139
	private static List<Player> playersInRoom = new List<Player>(10);

	// Token: 0x0400102C RID: 4140
	private static string roomGameMode = "";

	// Token: 0x0400102D RID: 4141
	private static bool joinedRoom = false;

	// Token: 0x0400102E RID: 4142
	private static PhotonView[] sceneViews;
}
