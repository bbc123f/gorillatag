using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaTag;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace GorillaNetworking
{
	public class PhotonNetworkController : MonoBehaviourPunCallbacks, IConnectionCallbacks
	{
		public string GameVersionType
		{
			get
			{
				return this.gameVersionType;
			}
		}

		public int GameMajorVersion
		{
			get
			{
				return this.majorVersion;
			}
		}

		public int GameMinorVersion
		{
			get
			{
				return this.minorVersion;
			}
		}

		public int GameMinorVersion2
		{
			get
			{
				return this.minorVersion2;
			}
		}

		public string StartLevel
		{
			get
			{
				return this.startLevel;
			}
			set
			{
				this.startLevel = value;
			}
		}

		public GTZone StartZone
		{
			get
			{
				return this.startZone;
			}
			set
			{
				this.startZone = value;
			}
		}

		public string GameVersionString
		{
			get
			{
				return this._gameVersionString;
			}
			set
			{
				this._gameVersionString = "MODDED";
			}
		}

		public void FullDisconnect()
		{
			this.currentState = PhotonNetworkController.ConnectionState.Initialization;
			PhotonNetwork.Disconnect();
		}

		public void InitiateConnection()
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.InitialConnection);
		}

		public void Awake()
		{
			this._gameVersionString = string.Concat(new string[]
			{
				this.gameVersionType,
				".",
				this.majorVersion.ToString(),
				".",
				this.minorVersion.ToString(),
				".",
				this.minorVersion2.ToString()
			});
			if (PhotonNetworkController.Instance == null)
			{
				PhotonNetworkController.Instance = this;
			}
			else if (PhotonNetworkController.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.updatedName = false;
			this.roomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
			this.playersInRegion = new int[this.serverRegions.Length];
			this.pingInRegion = new int[this.serverRegions.Length];
			this.currentState = PhotonNetworkController.ConnectionState.Initialization;
		}

		public override void OnCustomAuthenticationFailed(string debugMessage)
		{
			this.retry = true;
			if (this.timeToRetryWithBackoff < 1f)
			{
				this.timeToRetryWithBackoff = 1f;
			}
			Debug.Log("auth failed, backing off connecting, with message: " + debugMessage);
		}

		public void Start()
		{
			base.StartCoroutine(this.DisableOnStart());
			PhotonNetwork.EnableCloseConnection = false;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
		}

		private IEnumerator DisableOnStart()
		{
			ZoneManagement.SetActiveZone(this.StartZone);
			yield break;
		}

		public void FixedUpdate()
		{
			this.headRightHandDistance = (GorillaLocomotion.Player.Instance.headCollider.transform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.position).magnitude;
			this.headLeftHandDistance = (GorillaLocomotion.Player.Instance.headCollider.transform.position - GorillaLocomotion.Player.Instance.leftControllerTransform.position).magnitude;
			this.headQuat = GorillaLocomotion.Player.Instance.headCollider.transform.rotation;
			if (!this.disableAFKKick && Quaternion.Angle(this.headQuat, this.lastHeadQuat) <= 0.01f && Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) < 0.001f && Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) < 0.001f && this.pauseTime + this.disconnectTime < Time.realtimeSinceStartup)
			{
				this.pauseTime = Time.realtimeSinceStartup;
				this.ProcessState(PhotonNetworkController.ConnectionEvent.Disconnect);
			}
			else if (Quaternion.Angle(this.headQuat, this.lastHeadQuat) > 0.01f || Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) >= 0.001f || Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) >= 0.001f)
			{
				this.pauseTime = Time.realtimeSinceStartup;
			}
			this.lastHeadRightHandDistance = this.headRightHandDistance;
			this.lastHeadLeftHandDistance = this.headLeftHandDistance;
			this.lastHeadQuat = this.headQuat;
		}

		private void ProcessInitializationState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			if (connectionEvent == PhotonNetworkController.ConnectionEvent.InitialConnection)
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = this.GameVersionString;
				this.currentRegionIndex = 0;
				if (PlayerPrefs.GetString("playerName") != "")
				{
					PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("playerName");
				}
				else
				{
					PhotonNetwork.LocalPlayer.NickName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
				}
				PhotonNetwork.AutomaticallySyncScene = false;
				this.currentState = PhotonNetworkController.ConnectionState.DeterminingPingsAndPlayerCount;
				this.ConnectToRegion(this.serverRegions[this.currentRegionIndex]);
				return;
			}
			this.InvalidState(connectionEvent);
		}

		private void ProcessDeterminingPingsAndPlayerCountState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			if (connectionEvent == PhotonNetworkController.ConnectionEvent.OnConnectedToMaster)
			{
				int ping = PhotonNetwork.GetPing();
				Debug.Log(string.Concat(new string[]
				{
					"current ping is ",
					ping.ToString(),
					" on region ",
					this.serverRegions[this.currentRegionIndex],
					". player count is ",
					PhotonNetwork.CountOfPlayers.ToString()
				}));
				GorillaComputer.instance.screenChanged = true;
				this.playersInRegion[this.currentRegionIndex] = PhotonNetwork.CountOfPlayers;
				this.pingInRegion[this.currentRegionIndex] = ping;
				PhotonNetwork.Disconnect();
				return;
			}
			if (connectionEvent != PhotonNetworkController.ConnectionEvent.OnDisconnected)
			{
				this.InvalidState(connectionEvent);
				return;
			}
			this.currentRegionIndex++;
			if (this.currentRegionIndex >= this.serverRegions.Length)
			{
				Debug.Log("checked all servers. connecting to server with best ping: " + this.GetRegionWithLowestPing());
				this.currentState = PhotonNetworkController.ConnectionState.ConnectedAndWaiting;
				GorillaComputer.instance.screenChanged = true;
				if (this.currentJoinTrigger != null)
				{
					this.AttemptToJoinPublicRoom(this.currentJoinTrigger);
				}
				this.ConnectToRegion(this.GetRegionWithLowestPing());
				return;
			}
			Debug.Log("checking next region");
			this.ConnectToRegion(this.serverRegions[this.currentRegionIndex]);
		}

		private void ProcessConnectedAndWaitingState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			switch (connectionEvent)
			{
			case PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom:
				this.currentState = PhotonNetworkController.ConnectionState.JoiningPublicRoom;
				this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom);
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom:
				this.currentState = PhotonNetworkController.ConnectionState.JoiningSpecificRoom;
				this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom);
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptToCreateRoom:
				break;
			case PhotonNetworkController.ConnectionEvent.Disconnect:
				PhotonNetwork.Disconnect();
				return;
			default:
				if (connectionEvent == PhotonNetworkController.ConnectionEvent.OnDisconnected)
				{
					Debug.Log("not sure what happened, reconnecting to region with best ping");
					this.retry = true;
					this.ConnectToRegion(this.GetRegionWithLowestPing());
					return;
				}
				break;
			}
			this.InvalidState(connectionEvent);
		}

		private void ProcessDisconnectingFromRoomState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			if (connectionEvent == PhotonNetworkController.ConnectionEvent.OnConnectedToMaster)
			{
				Debug.Log("successfully reconnected to master. waiting on what to do next.");
				this.currentState = PhotonNetworkController.ConnectionState.ConnectedAndWaiting;
				return;
			}
			if (connectionEvent == PhotonNetworkController.ConnectionEvent.Disconnect)
			{
				PhotonNetwork.Disconnect();
				return;
			}
			if (connectionEvent != PhotonNetworkController.ConnectionEvent.OnDisconnected)
			{
				this.InvalidState(connectionEvent);
				return;
			}
			Debug.Log("just disconnected while trying to disconnect. attempting to reconnect to best region.");
			this.currentState = PhotonNetworkController.ConnectionState.ConnectedAndWaiting;
			this.ConnectToRegion(this.GetRegionWithLowestPing());
		}

		private void ProcessJoiningPublicRoomState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			switch (connectionEvent)
			{
			case PhotonNetworkController.ConnectionEvent.OnConnectedToMaster:
				this.JoinPublicRoom(this.joiningWithFriend);
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom:
				if (!this.pastFirstConnection)
				{
					this.JoinPublicRoom(this.joiningWithFriend);
					return;
				}
				PhotonNetwork.Disconnect();
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom:
				this.JoinSpecificRoom();
				return;
			case PhotonNetworkController.ConnectionEvent.Disconnect:
				this.DisconnectFromRoom(PhotonNetworkController.ConnectionState.DisconnectingFromRoom);
				return;
			case PhotonNetworkController.ConnectionEvent.OnJoinedRoom:
				this.pastFirstConnection = true;
				this.currentJoinTrigger.UpdateScreens();
				this.currentState = PhotonNetworkController.ConnectionState.InPublicRoom;
				return;
			case PhotonNetworkController.ConnectionEvent.OnJoinRandomFailed:
				this.CreatePublicRoom(this.joiningWithFriend);
				return;
			case PhotonNetworkController.ConnectionEvent.OnCreateRoomFailed:
				this.CreatePublicRoom(this.joiningWithFriend);
				return;
			case PhotonNetworkController.ConnectionEvent.OnDisconnected:
				if (!this.joiningWithFriend)
				{
					if (!this.pastFirstConnection)
					{
						this.pastFirstConnection = true;
						PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.GetRegionWithLowestPing();
					}
					else
					{
						float value = Random.value;
						int num = 0;
						for (int i = 0; i < this.playersInRegion.Length; i++)
						{
							num += this.playersInRegion[i];
						}
						float num2 = 0f;
						int num3 = -1;
						while (num2 < value && num3 < this.playersInRegion.Length - 1)
						{
							num3++;
							num2 += (float)this.playersInRegion[num3] / (float)num;
						}
						PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.serverRegions[num3];
					}
				}
				base.StartCoroutine(this.ConnectUsingSettingsWithBackoff());
				return;
			}
			this.InvalidState(connectionEvent);
		}

		private void ProcessJoiningSpecificRoomState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			switch (connectionEvent)
			{
			case PhotonNetworkController.ConnectionEvent.OnConnectedToMaster:
				if (!this.createRoom)
				{
					Debug.Log("connected to master in the determined region. joining specific room");
					this.JoinSpecificRoom();
					return;
				}
				this.createRoom = false;
				this.CreatePrivateRoom();
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom:
				this.currentState = PhotonNetworkController.ConnectionState.JoiningPublicRoom;
				this.JoinPublicRoom(false);
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom:
				this.isRoomFull = false;
				GorillaComputer.instance.roomNotAllowed = false;
				this.doesRoomExist = false;
				this.createRoom = false;
				this.currentRegionIndex = 0;
				PhotonNetwork.Disconnect();
				return;
			case PhotonNetworkController.ConnectionEvent.Disconnect:
				this.DisconnectFromRoom(PhotonNetworkController.ConnectionState.DisconnectingFromRoom);
				return;
			case PhotonNetworkController.ConnectionEvent.OnJoinedRoom:
				Debug.Log("successfully joined room!");
				this.currentState = (PhotonNetwork.CurrentRoom.IsVisible ? PhotonNetworkController.ConnectionState.InPublicRoom : PhotonNetworkController.ConnectionState.InPrivateRoom);
				if (this.currentState == PhotonNetworkController.ConnectionState.InPublicRoom)
				{
					Debug.Log("game mode of room joined is: " + (string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]);
					if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.forestMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.forestMapTrigger;
					}
					else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.caveMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.caveMapTrigger;
					}
					else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.canyonMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.canyonMapTrigger;
					}
					else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.cityMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.cityMapTrigger;
					}
					else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.mountainMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.mountainMapTrigger;
					}
					else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.skyjungleMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.skyjungleMapTrigger;
					}
					else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.basementMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.basementMapTrigger;
					}
					else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.beachMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.beachMapTrigger;
					}
					else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.rotatingMapTrigger.gameModeName))
					{
						this.currentJoinTrigger = GorillaComputer.instance.rotatingMapTrigger;
					}
					this.currentJoinTrigger.UpdateScreens();
					return;
				}
				return;
			case PhotonNetworkController.ConnectionEvent.OnJoinRoomFailed:
				if (this.doesRoomExist && this.isRoomFull)
				{
					Debug.Log("cant join, the room is full! going back to best region.");
					GorillaComputer.instance.roomFull = true;
					this.DisconnectFromRoom(PhotonNetworkController.ConnectionState.DisconnectingFromRoom);
					return;
				}
				if (this.currentRegionIndex == this.serverRegions.Length - 1)
				{
					this.createRoom = true;
					PhotonNetwork.Disconnect();
					return;
				}
				Debug.Log("room was missing. check the next region");
				this.currentRegionIndex++;
				PhotonNetwork.Disconnect();
				return;
			case PhotonNetworkController.ConnectionEvent.OnCreateRoomFailed:
				Debug.Log("the room probably actually already exists, so maybe it was created just now? either way, give up.");
				this.DisconnectFromRoom(PhotonNetworkController.ConnectionState.ConnectedAndWaiting);
				return;
			case PhotonNetworkController.ConnectionEvent.OnDisconnected:
				if (!this.createRoom)
				{
					this.ConnectToRegion(this.serverRegions[this.currentRegionIndex]);
					return;
				}
				Debug.Log("checked all the rooms, it doesn't exist. lets go back to our fav region and create the room");
				this.ConnectToRegion(this.GetRegionWithLowestPing());
				return;
			}
			this.InvalidState(connectionEvent);
		}

		private void ProcessJoiningFriendState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			switch (connectionEvent)
			{
			case PhotonNetworkController.ConnectionEvent.OnConnectedToMaster:
				this.StartSearchingForFriend();
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom:
				this.currentState = PhotonNetworkController.ConnectionState.JoiningPublicRoom;
				this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom);
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom:
				this.JoinSpecificRoom();
				return;
			case PhotonNetworkController.ConnectionEvent.Disconnect:
				PhotonNetwork.Disconnect();
				return;
			case PhotonNetworkController.ConnectionEvent.OnJoinedRoom:
				this.currentState = PhotonNetworkController.ConnectionState.InPublicRoom;
				if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.forestMapTrigger.gameModeName))
				{
					this.currentJoinTrigger = GorillaComputer.instance.forestMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.caveMapTrigger.gameModeName))
				{
					this.currentJoinTrigger = GorillaComputer.instance.caveMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.canyonMapTrigger.gameModeName))
				{
					this.currentJoinTrigger = GorillaComputer.instance.canyonMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.cityMapTrigger.gameModeName))
				{
					this.currentJoinTrigger = GorillaComputer.instance.cityMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.basementMapTrigger.gameModeName))
				{
					this.currentJoinTrigger = GorillaComputer.instance.basementMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.beachMapTrigger.gameModeName))
				{
					this.currentJoinTrigger = GorillaComputer.instance.beachMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.rotatingMapTrigger.gameModeName))
				{
					this.currentJoinTrigger = GorillaComputer.instance.rotatingMapTrigger;
				}
				this.currentJoinTrigger.UpdateScreens();
				return;
			case PhotonNetworkController.ConnectionEvent.OnJoinRoomFailed:
				this.currentState = PhotonNetworkController.ConnectionState.ConnectedAndWaiting;
				return;
			case PhotonNetworkController.ConnectionEvent.OnDisconnected:
				base.StartCoroutine(this.ConnectUsingSettingsWithBackoff());
				return;
			case PhotonNetworkController.ConnectionEvent.FoundFriendToJoin:
				if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(this.actorIdToFollow, out this.tempPlayer) && this.tempPlayer != null)
				{
					this.currentState = PhotonNetworkController.ConnectionState.InPrivateRoom;
					GorillaNot.instance.SendReport("possible kick attempt", this.tempPlayer.UserId, this.tempPlayer.NickName);
					return;
				}
				if (PhotonNetwork.CurrentRoom.Name != this.customRoomID)
				{
					this.DisconnectCleanup();
					this.currentState = PhotonNetworkController.ConnectionState.JoiningSpecificRoom;
					this.currentRegionIndex = 0;
					PhotonNetwork.Disconnect();
					return;
				}
				return;
			}
			this.InvalidState(connectionEvent);
		}

		private void ProcessInPrivateRoomState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			switch (connectionEvent)
			{
			case PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom:
				if (this.joiningWithFriend)
				{
					this.DisconnectFromRoom(PhotonNetworkController.ConnectionState.JoiningPublicRoom);
					return;
				}
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom:
				if (PhotonNetwork.CurrentRoom.Name != this.customRoomID)
				{
					this.DisconnectCleanup();
					this.currentState = PhotonNetworkController.ConnectionState.JoiningSpecificRoom;
					this.currentRegionIndex = 0;
					PhotonNetwork.Disconnect();
					return;
				}
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptToCreateRoom:
				break;
			case PhotonNetworkController.ConnectionEvent.Disconnect:
				this.DisconnectFromRoom(PhotonNetworkController.ConnectionState.DisconnectingFromRoom);
				return;
			default:
				if (connectionEvent == PhotonNetworkController.ConnectionEvent.OnDisconnected)
				{
					this.DisconnectCleanup();
					this.currentState = PhotonNetworkController.ConnectionState.DisconnectingFromRoom;
					base.StartCoroutine(this.ConnectUsingSettingsWithBackoff());
					return;
				}
				if (connectionEvent == PhotonNetworkController.ConnectionEvent.FollowFriendToPub)
				{
					this.currentState = PhotonNetworkController.ConnectionState.JoiningFriend;
					this.StartSearchingForFriend();
					return;
				}
				break;
			}
			this.InvalidState(connectionEvent);
		}

		private void ProcessInPublicRoomState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			switch (connectionEvent)
			{
			case PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom:
				if (!((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(this.currentJoinTrigger.gameModeName))
				{
					this.DisconnectFromRoom(PhotonNetworkController.ConnectionState.JoiningPublicRoom);
					return;
				}
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom:
				if (PhotonNetwork.CurrentRoom.Name != this.customRoomID)
				{
					this.DisconnectCleanup();
					this.currentState = PhotonNetworkController.ConnectionState.JoiningSpecificRoom;
					this.currentRegionIndex = 0;
					PhotonNetwork.Disconnect();
					return;
				}
				return;
			case PhotonNetworkController.ConnectionEvent.AttemptToCreateRoom:
				break;
			case PhotonNetworkController.ConnectionEvent.Disconnect:
				this.DisconnectFromRoom(PhotonNetworkController.ConnectionState.DisconnectingFromRoom);
				return;
			default:
				if (connectionEvent == PhotonNetworkController.ConnectionEvent.OnDisconnected)
				{
					this.DisconnectCleanup();
					this.currentState = PhotonNetworkController.ConnectionState.DisconnectingFromRoom;
					base.StartCoroutine(this.ConnectUsingSettingsWithBackoff());
					return;
				}
				break;
			}
			this.InvalidState(connectionEvent);
		}

		private void ProcessWrongVersionState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			this.InvalidState(connectionEvent);
		}

		private void ProcessState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			if (this.currentState == PhotonNetworkController.ConnectionState.Initialization)
			{
				this.ProcessInitializationState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.DeterminingPingsAndPlayerCount)
			{
				this.ProcessDeterminingPingsAndPlayerCountState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.ConnectedAndWaiting)
			{
				this.ProcessConnectedAndWaitingState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.DisconnectingFromRoom)
			{
				this.ProcessDisconnectingFromRoomState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.JoiningPublicRoom)
			{
				this.ProcessJoiningPublicRoomState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.JoiningSpecificRoom)
			{
				this.ProcessJoiningSpecificRoomState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.JoiningFriend)
			{
				this.ProcessJoiningFriendState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.InPrivateRoom)
			{
				this.ProcessInPrivateRoomState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.InPublicRoom)
			{
				this.ProcessInPublicRoomState(connectionEvent);
				return;
			}
			if (this.currentState == PhotonNetworkController.ConnectionState.WrongVersion)
			{
				this.ProcessWrongVersionState(connectionEvent);
				return;
			}
		}

		private void InvalidState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
		}

		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger)
		{
			this.currentJoinTrigger = triggeredTrigger;
			this.joiningWithFriend = false;
			this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom);
		}

		public void AttemptToJoinSpecificRoom(string roomID)
		{
			this.customRoomID = roomID;
			this.joiningWithFriend = false;
			this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom);
		}

		private void JoinPublicRoom(bool joinWithFriends)
		{
			PhotonNetwork.LocalPlayer.NickName = GorillaComputer.instance.savedName;
			if (PlayFabClientAPI.IsClientLoggedIn())
			{
				this.playFabAuthenticator.SetDisplayName(PhotonNetwork.LocalPlayer.NickName);
			}
			Hashtable hashtable = new Hashtable();
			if (this.currentJoinTrigger.gameModeName != "city" && this.currentJoinTrigger.gameModeName != "basement")
			{
				Dictionary<object, object> dictionary = hashtable;
				object obj = "gameMode";
				string gameModeName = this.currentJoinTrigger.gameModeName;
				string currentQueue = GorillaComputer.instance.currentQueue;
				WatchableStringSO currentGameMode = GorillaComputer.instance.currentGameMode;
				dictionary.Add(obj, gameModeName + currentQueue + ((currentGameMode != null) ? currentGameMode.ToString() : null));
			}
			else
			{
				hashtable.Add("gameMode", this.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + "CASUAL");
			}
			PhotonNetwork.AutomaticallySyncScene = false;
			if (joinWithFriends)
			{
				this.friendIDList.Remove(PhotonNetwork.LocalPlayer.UserId);
				PhotonNetwork.JoinRandomRoom(hashtable, this.GetRoomSize(this.currentJoinTrigger.gameModeName), MatchmakingMode.RandomMatching, null, null, this.friendIDList.ToArray());
				return;
			}
			PhotonNetwork.JoinRandomRoom(hashtable, this.GetRoomSize(this.currentJoinTrigger.gameModeName), MatchmakingMode.FillRoom, null, null, null);
		}

		private void JoinSpecificRoom()
		{
			PhotonNetwork.LocalPlayer.NickName = GorillaComputer.instance.savedName;
			if (PlayFabClientAPI.IsClientLoggedIn())
			{
				this.playFabAuthenticator.SetDisplayName(PhotonNetwork.LocalPlayer.NickName);
			}
			PhotonNetwork.JoinRoom(this.customRoomID, null);
		}

		private void DisconnectCleanup()
		{
			if (GTAppState.isQuitting)
			{
				return;
			}
			if (GorillaParent.instance != null)
			{
				GorillaScoreboardSpawner[] componentsInChildren = GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnLeftRoom();
				}
			}
			this.attemptingToConnect = true;
			if (GorillaComputer.instance != null && !ApplicationQuittingState.IsQuitting)
			{
				foreach (GorillaLevelScreen gorillaLevelScreen in GorillaComputer.instance.levelScreens)
				{
					gorillaLevelScreen.UpdateText(gorillaLevelScreen.startingText, true);
				}
			}
			GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
			GorillaLocomotion.Player.Instance.jumpMultiplier = 1.1f;
			GorillaNot.instance.currentMasterClient = null;
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
			this.initialGameMode = "";
		}

		public override void OnConnectedToMaster()
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnConnectedToMaster);
		}

		public override void OnJoinedRoom()
		{
			this.timeToRetryWithBackoff = 0.1f;
			object obj;
			if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj))
			{
				this.AttemptDisconnect();
				return;
			}
			this.initialGameMode = obj.ToString();
			if (!PhotonNetwork.CurrentRoom.IsVisible)
			{
				this.currentJoinTrigger = this.privateTrigger;
				GorillaLevelScreen[] levelScreens = GorillaComputer.instance.levelScreens;
				for (int i = 0; i < levelScreens.Length; i++)
				{
					levelScreens[i].UpdateText("YOU'RE IN A PRIVATE ROOM, SO GO WHEREVER YOU WANT. MAKE SURE YOU PLAY WITHIN THE BOUNDARIES SET BY THE PLAYERS IN THIS ROOM!", true);
				}
			}
			else
			{
				this.allowedInPubRoom = false;
				for (int j = 0; j < GorillaComputer.instance.allowedMapsToJoin.Length; j++)
				{
					if (obj.ToString().Contains(GorillaComputer.instance.allowedMapsToJoin[j]))
					{
						this.allowedInPubRoom = true;
						break;
					}
				}
				if (!this.allowedInPubRoom)
				{
					GorillaComputer.instance.roomNotAllowed = true;
					PhotonNetwork.Disconnect();
					return;
				}
			}
			PhotonVoiceNetwork.Instance.PrimaryRecorder.StartRecording();
			if (PhotonNetwork.IsMasterClient)
			{
				GameMode.LoadGameModeFromProperty(this.initialGameMode);
			}
			bool flag = PlayerPrefs.GetString("tutorial", "nope") == "done";
			if (!flag)
			{
				PlayerPrefs.SetString("tutorial", "done");
				PlayerPrefs.Save();
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("didTutorial", flag);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
			PhotonNetwork.Instantiate("GorillaPrefabs/Gorilla Player Networked", this.playerOffset.transform.position, this.playerOffset.transform.rotation, 0, null);
			GorillaComputer.instance.roomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			if (this.joiningWithFriend)
			{
				this.BroadcastMyRoom(true);
			}
			PhotonNetwork.NickName = PlayerPrefs.GetString("playerName", "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0'));
			GorillaNot.instance.currentMasterClient = null;
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnJoinedRoom);
		}

		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.Log("join room failed:" + returnCode.ToString() + " " + message);
			if (returnCode == 32758)
			{
				Debug.Log("room didn't exist!");
				this.doesRoomExist = false;
			}
			else
			{
				this.doesRoomExist = true;
			}
			if (returnCode == 32765)
			{
				Debug.Log("room was full!");
				this.isRoomFull = true;
			}
			else
			{
				this.isRoomFull = false;
			}
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnJoinRoomFailed);
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnJoinRandomFailed);
		}

		private void CreatePublicRoom(bool joinWithFriends)
		{
			Hashtable hashtable2;
			if (this.currentJoinTrigger.gameModeName != "city" && this.currentJoinTrigger.gameModeName != "basement")
			{
				Hashtable hashtable = new Hashtable();
				object obj = "gameMode";
				string gameModeName = this.currentJoinTrigger.gameModeName;
				string currentQueue = GorillaComputer.instance.currentQueue;
				WatchableStringSO currentGameMode = GorillaComputer.instance.currentGameMode;
				hashtable.Add(obj, gameModeName + currentQueue + ((currentGameMode != null) ? currentGameMode.ToString() : null));
				hashtable2 = hashtable;
			}
			else
			{
				hashtable2 = new Hashtable { 
				{
					"gameMode",
					this.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + "CASUAL"
				} };
			}
			Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
			roomOptions.IsVisible = true;
			roomOptions.IsOpen = true;
			roomOptions.MaxPlayers = this.GetRoomSize(this.currentJoinTrigger.gameModeName);
			roomOptions.CustomRoomProperties = hashtable2;
			roomOptions.PublishUserId = true;
			roomOptions.CustomRoomPropertiesForLobby = new string[] { "gameMode" };
			if (joinWithFriends)
			{
				foreach (string text in this.friendIDList.ToArray())
				{
				}
				this.friendIDList.Remove(PhotonNetwork.LocalPlayer.UserId);
				PhotonNetwork.CreateRoom(this.ReturnRoomName(), roomOptions, null, this.friendIDList.ToArray());
				return;
			}
			PhotonNetwork.CreateRoom(this.ReturnRoomName(), roomOptions, null, null);
		}

		private void CreatePrivateRoom()
		{
			this.currentJoinTrigger = this.privateTrigger;
			Hashtable hashtable = new Hashtable();
			object obj = "gameMode";
			string gameModeName = this.currentJoinTrigger.gameModeName;
			string currentQueue = GorillaComputer.instance.currentQueue;
			WatchableStringSO currentGameMode = GorillaComputer.instance.currentGameMode;
			hashtable.Add(obj, gameModeName + currentQueue + ((currentGameMode != null) ? currentGameMode.ToString() : null));
			Hashtable hashtable2 = hashtable;
			Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
			roomOptions.IsVisible = false;
			roomOptions.IsOpen = true;
			roomOptions.MaxPlayers = this.GetRoomSize(this.currentJoinTrigger.gameModeName);
			roomOptions.CustomRoomProperties = hashtable2;
			roomOptions.PublishUserId = true;
			roomOptions.CustomRoomPropertiesForLobby = new string[] { "gameMode" };
			PhotonNetwork.CreateRoom(this.customRoomID, roomOptions, null, null);
		}

		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnCreateRoomFailed);
		}

		public void ConnectToRegion(string region)
		{
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
			base.StartCoroutine(this.ConnectUsingSettingsWithBackoff());
		}

		private IEnumerator ConnectUsingSettingsWithBackoff()
		{
			if (this.retry)
			{
				this.retry = false;
				this.timeToRetryWithBackoff *= 2f;
				Debug.Log("backing off retry, current time to wait is " + this.timeToRetryWithBackoff.ToString());
			}
			yield return new WaitForSeconds(this.timeToRetryWithBackoff);
			PhotonNetwork.ConnectUsingSettings();
			yield break;
		}

		public void AttemptJoinPublicWithFriends(GorillaNetworkJoinTrigger triggeredTrigger)
		{
			this.currentJoinTrigger = triggeredTrigger;
			this.joiningWithFriend = true;
			this.keyToFollow = PhotonNetwork.LocalPlayer.ActorNumber.ToString() + this.keyStr;
			this.BroadcastMyRoom(false);
			this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom);
		}

		public void AttemptToFollowFriendIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr)
		{
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = actorNumberToFollow.ToString() + newKeyStr;
			this.actorIdToFollow = actorNumberToFollow;
			this.shuffler = shufflerStr;
			this.ProcessState(PhotonNetworkController.ConnectionEvent.FollowFriendToPub);
		}

		public void AttemptDisconnect()
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.Disconnect);
		}

		private void DisconnectFromRoom(PhotonNetworkController.ConnectionState newState)
		{
			this.currentState = newState;
			PhotonNetwork.Disconnect();
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			this.DisconnectCleanup();
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnDisconnected);
		}

		private void GetOculusNonceCallback(Message<UserProof> message)
		{
			AuthenticationValues authValues = PhotonNetwork.AuthValues;
			if (authValues != null)
			{
				Dictionary<string, object> dictionary = PhotonNetwork.AuthValues.AuthPostData as Dictionary<string, object>;
				if (dictionary != null)
				{
					if (message.IsError)
					{
						base.StartCoroutine(this.ReGetNonce());
						return;
					}
					dictionary["Nonce"] = message.Data.Value;
					authValues.SetAuthPostData(dictionary);
					PhotonNetwork.AuthValues = authValues;
				}
			}
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnDisconnected);
		}

		private IEnumerator ReGetNonce()
		{
			yield return new WaitForSeconds(3f);
			Users.GetUserProof().OnComplete(new Message<UserProof>.Callback(this.GetOculusNonceCallback));
			yield return null;
			yield break;
		}

		public void WrongVersion()
		{
			this.wrongVersion = true;
			this.currentState = PhotonNetworkController.ConnectionState.WrongVersion;
		}

		public void OnApplicationQuit()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev";
			}
		}

		private string ReturnRoomName()
		{
			if (this.isPrivate)
			{
				return this.customRoomID;
			}
			return this.RandomRoomName();
		}

		private string RandomRoomName()
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				text += this.roomCharacters.Substring(Random.Range(0, this.roomCharacters.Length), 1);
			}
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				return text;
			}
			return this.RandomRoomName();
		}

		public string ShuffleRoomName(string room, string shuffle, bool encode)
		{
			string text = "";
			int num;
			if (!int.TryParse(shuffle, out num) || shuffle.Length != room.Length * 2)
			{
				return "";
			}
			for (int i = 0; i < room.Length; i++)
			{
				string text2 = room.Substring(i, 1);
				int num2 = this.roomCharacters.IndexOf(text2);
				if (encode)
				{
					text += this.roomCharacters.Substring(PhotonNetworkController.mod(num2 + int.Parse(shuffle.Substring(i * 2, 2)), this.roomCharacters.Length), 1);
				}
				else
				{
					text += this.roomCharacters.Substring(PhotonNetworkController.mod(num2 - int.Parse(shuffle.Substring(i * 2, 2)), this.roomCharacters.Length), 1);
				}
			}
			return text;
		}

		public byte GetRoomSize(string gameModeName)
		{
			if (gameModeName.Contains("ball"))
			{
				return 5;
			}
			return 10;
		}

		public void StartSearchingForFriend()
		{
			this.startingToLookForFriend = Time.time;
			this.successfullyFoundFriend = false;
			base.StartCoroutine(this.SearchForFriendToJoin(this.friendToFollow, this.keyToFollow, this.shuffler));
		}

		private IEnumerator SearchForFriendToJoin(string userID, string keyToFollow, string shufflerToFollow)
		{
			while (!this.successfullyFoundFriend && this.startingToLookForFriend + this.timeToSpendLookingForFriend > Time.time)
			{
				try
				{
					GetSharedGroupDataRequest getSharedGroupDataRequest = new GetSharedGroupDataRequest();
					getSharedGroupDataRequest.Keys = new List<string> { keyToFollow };
					getSharedGroupDataRequest.SharedGroupId = userID;
					PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
					{
						foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
						{
							if (keyValuePair.Key == keyToFollow)
							{
								this.customRoomID = this.ShuffleRoomName(keyValuePair.Value.Value, shufflerToFollow, false);
								this.successfullyFoundFriend = true;
								this.ProcessState(PhotonNetworkController.ConnectionEvent.FoundFriendToJoin);
							}
						}
					}, delegate(PlayFabError error)
					{
					}, null, null);
				}
				catch
				{
				}
				yield return new WaitForSeconds(3f);
			}
			yield return null;
			yield break;
		}

		private string GetRegionWithLowestPing()
		{
			int num = 10000;
			int num2 = 0;
			for (int i = 0; i < this.serverRegions.Length; i++)
			{
				Debug.Log("ping in region " + this.serverRegions[i] + " is " + this.pingInRegion[i].ToString());
				if (this.pingInRegion[i] < num && this.pingInRegion[i] > 0)
				{
					num = this.pingInRegion[i];
					num2 = i;
				}
			}
			return this.serverRegions[num2];
		}

		public int TotalUsers()
		{
			int num = 0;
			foreach (int num2 in this.playersInRegion)
			{
				num += num2;
			}
			return num;
		}

		public string CurrentState()
		{
			return this.currentState.ToString();
		}

		private void BroadcastMyRoom(bool create)
		{
			Debug.Log("broadcasting room");
			ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
			executeCloudScriptRequest.FunctionName = "BroadcastMyRoom";
			executeCloudScriptRequest.FunctionParameter = new
			{
				KeyToFollow = this.keyToFollow,
				RoomToJoin = this.ShuffleRoomName(PhotonNetwork.CurrentRoom.Name, this.shuffler, true),
				Set = create
			};
			PlayFabClientAPI.ExecuteCloudScript(executeCloudScriptRequest, delegate(ExecuteCloudScriptResult result)
			{
			}, delegate(PlayFabError error)
			{
			}, null, null);
		}

		public static int mod(int x, int m)
		{
			return (x % m + m) % m;
		}

		public static volatile PhotonNetworkController Instance;

		public int incrementCounter;

		private PhotonNetworkController.ConnectionState currentState;

		public PlayFabAuthenticator playFabAuthenticator;

		public string[] serverRegions;

		private string gameVersionType = "live1";

		private int majorVersion = 1;

		private int minorVersion = 1;

		private int minorVersion2 = 71;

		private string _gameVersionString = "";

		public bool isPrivate;

		public string customRoomID;

		public GameObject playerOffset;

		public SkinnedMeshRenderer[] offlineVRRig;

		public bool attemptingToConnect;

		private int currentRegionIndex;

		public string currentGameType;

		public bool wrongVersion;

		public bool roomCosmeticsInitialized;

		public GameObject photonVoiceObjectPrefab;

		public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

		private bool pastFirstConnection;

		private float timeToRetryWithBackoff = 0.1f;

		private bool retry;

		private float lastTimeConnected;

		private float lastHeadRightHandDistance;

		private float lastHeadLeftHandDistance;

		private float pauseTime;

		private float disconnectTime = 120f;

		public bool disableAFKKick;

		private float headRightHandDistance;

		private float headLeftHandDistance;

		private Quaternion headQuat;

		private Quaternion lastHeadQuat;

		public GameObject[] disableOnStartup;

		public GameObject[] enableOnStartup;

		public string roomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";

		public bool updatedName;

		private int[] playersInRegion;

		private int[] pingInRegion;

		public List<string> friendIDList = new List<string>();

		private bool successfullyFoundFriend;

		private float startingToLookForFriend;

		private float timeToSpendLookingForFriend = 15f;

		private bool joiningWithFriend;

		private string friendToFollow;

		private string keyToFollow;

		private int actorIdToFollow;

		public string shuffler;

		public string keyStr;

		private Photon.Realtime.Player tempPlayer;

		private bool isRoomFull;

		private bool doesRoomExist;

		private bool createRoom;

		private string startLevel;

		private GTZone startZone;

		public GorillaNetworkJoinTrigger privateTrigger;

		internal string initialGameMode = "";

		public GorillaNetworkJoinTrigger currentJoinTrigger;

		public bool allowedInPubRoom;

		public enum ConnectionState
		{
			Initialization,
			WrongVersion,
			DeterminingPingsAndPlayerCount,
			ConnectedAndWaiting,
			DisconnectingFromRoom,
			JoiningPublicRoom,
			JoiningSpecificRoom,
			JoiningFriend,
			InPrivateRoom,
			InPublicRoom
		}

		public enum ConnectionEvent
		{
			InitialConnection,
			OnConnectedToMaster,
			AttemptJoinPublicRoom,
			AttemptJoinSpecificRoom,
			AttemptToCreateRoom,
			Disconnect,
			OnJoinedRoom,
			OnJoinRoomFailed,
			OnJoinRandomFailed,
			OnCreateRoomFailed,
			OnDisconnected,
			FoundFriendToJoin,
			FollowFriendToPub
		}
	}
}
