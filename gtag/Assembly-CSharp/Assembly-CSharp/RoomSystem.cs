using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class RoomSystem : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		PhotonNetwork.AddCallbackTarget(this);
		RoomSystem.callbackInstance = this;
	}

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

	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
		RoomSystem.playersInRoom.Add(newPlayer);
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(newPlayer);
	}

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

	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
		RoomSystem.playersInRoom.Remove(otherPlayer);
	}

	public static List<Player> PlayersInRoom
	{
		get
		{
			return RoomSystem.playersInRoom;
		}
	}

	public static string RoomGameMode
	{
		get
		{
			return RoomSystem.roomGameMode;
		}
	}

	public static bool JoinedRoom
	{
		get
		{
			return PhotonNetwork.InRoom && RoomSystem.joinedRoom;
		}
	}

	public static bool AmITheHost
	{
		get
		{
			return PhotonNetwork.IsMasterClient || !PhotonNetwork.InRoom;
		}
	}

	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	private static RoomSystem callbackInstance;

	private static List<Player> playersInRoom = new List<Player>(10);

	private static string roomGameMode = "";

	private static bool joinedRoom = false;

	private static PhotonView[] sceneViews;
}
