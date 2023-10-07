using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaLocomotion;
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
	// Token: 0x020002B7 RID: 695
	public class PhotonNetworkController : MonoBehaviourPunCallbacks, IConnectionCallbacks
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x0600128F RID: 4751 RVA: 0x0006BB0F File Offset: 0x00069D0F
		public string GameVersionType
		{
			get
			{
				return this.gameVersionType;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06001290 RID: 4752 RVA: 0x0006BB17 File Offset: 0x00069D17
		public int GameMajorVersion
		{
			get
			{
				return this.majorVersion;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06001291 RID: 4753 RVA: 0x0006BB1F File Offset: 0x00069D1F
		public int GameMinorVersion
		{
			get
			{
				return this.minorVersion;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06001292 RID: 4754 RVA: 0x0006BB27 File Offset: 0x00069D27
		public int GameMinorVersion2
		{
			get
			{
				return this.minorVersion2;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x0006BB2F File Offset: 0x00069D2F
		// (set) Token: 0x06001294 RID: 4756 RVA: 0x0006BB37 File Offset: 0x00069D37
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

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06001295 RID: 4757 RVA: 0x0006BB40 File Offset: 0x00069D40
		// (set) Token: 0x06001296 RID: 4758 RVA: 0x0006BB48 File Offset: 0x00069D48
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

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06001297 RID: 4759 RVA: 0x0006BB51 File Offset: 0x00069D51
		// (set) Token: 0x06001298 RID: 4760 RVA: 0x0006BB59 File Offset: 0x00069D59
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

		// Token: 0x06001299 RID: 4761 RVA: 0x0006BB66 File Offset: 0x00069D66
		public void FullDisconnect()
		{
			this.currentState = PhotonNetworkController.ConnectionState.Initialization;
			PhotonNetwork.Disconnect();
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x0006BB74 File Offset: 0x00069D74
		public void InitiateConnection()
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.InitialConnection);
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x0006BB80 File Offset: 0x00069D80
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
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x0006BC54 File Offset: 0x00069E54
		public override void OnCustomAuthenticationFailed(string debugMessage)
		{
			this.retry = true;
			if (this.timeToRetryWithBackoff < 1f)
			{
				this.timeToRetryWithBackoff = 1f;
			}
			Debug.Log("auth failed, backing off connecting, with message: " + debugMessage);
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x0006BC85 File Offset: 0x00069E85
		public void Start()
		{
			base.StartCoroutine(this.DisableOnStart());
			this.currentState = PhotonNetworkController.ConnectionState.Initialization;
			PhotonNetwork.EnableCloseConnection = false;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x0006BCB1 File Offset: 0x00069EB1
		private IEnumerator DisableOnStart()
		{
			ZoneManagement.SetActiveZone(this.StartZone);
			yield break;
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x0006BCC0 File Offset: 0x00069EC0
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

		// Token: 0x060012A0 RID: 4768 RVA: 0x0006BE48 File Offset: 0x0006A048
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

		// Token: 0x060012A1 RID: 4769 RVA: 0x0006BEFC File Offset: 0x0006A0FC
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

		// Token: 0x060012A2 RID: 4770 RVA: 0x0006C038 File Offset: 0x0006A238
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

		// Token: 0x060012A3 RID: 4771 RVA: 0x0006C0B0 File Offset: 0x0006A2B0
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

		// Token: 0x060012A4 RID: 4772 RVA: 0x0006C10C File Offset: 0x0006A30C
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

		// Token: 0x060012A5 RID: 4773 RVA: 0x0006C270 File Offset: 0x0006A470
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

		// Token: 0x060012A6 RID: 4774 RVA: 0x0006C674 File Offset: 0x0006A874
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

		// Token: 0x060012A7 RID: 4775 RVA: 0x0006C948 File Offset: 0x0006AB48
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

		// Token: 0x060012A8 RID: 4776 RVA: 0x0006C9F4 File Offset: 0x0006ABF4
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

		// Token: 0x060012A9 RID: 4777 RVA: 0x0006CAAF File Offset: 0x0006ACAF
		private void ProcessWrongVersionState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
			this.InvalidState(connectionEvent);
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x0006CAB8 File Offset: 0x0006ACB8
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

		// Token: 0x060012AB RID: 4779 RVA: 0x0006CB6F File Offset: 0x0006AD6F
		private void InvalidState(PhotonNetworkController.ConnectionEvent connectionEvent)
		{
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x0006CB71 File Offset: 0x0006AD71
		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger)
		{
			this.currentJoinTrigger = triggeredTrigger;
			this.joiningWithFriend = false;
			this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom);
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x0006CB88 File Offset: 0x0006AD88
		public void AttemptToJoinSpecificRoom(string roomID)
		{
			this.customRoomID = roomID;
			this.joiningWithFriend = false;
			this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinSpecificRoom);
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x0006CBA0 File Offset: 0x0006ADA0
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
				hashtable.Add("gameMode", this.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + GorillaComputer.instance.currentGameMode);
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

		// Token: 0x060012AF RID: 4783 RVA: 0x0006CCD8 File Offset: 0x0006AED8
		private void JoinSpecificRoom()
		{
			PhotonNetwork.LocalPlayer.NickName = GorillaComputer.instance.savedName;
			if (PlayFabClientAPI.IsClientLoggedIn())
			{
				this.playFabAuthenticator.SetDisplayName(PhotonNetwork.LocalPlayer.NickName);
			}
			PhotonNetwork.JoinRoom(this.customRoomID, null);
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x0006CD24 File Offset: 0x0006AF24
		private void DisconnectCleanup()
		{
			if (GorillaParent.instance != null)
			{
				GorillaScoreboardSpawner[] componentsInChildren = GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnLeftRoom();
				}
			}
			this.attemptingToConnect = true;
			if (GorillaComputer.instance != null)
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

		// Token: 0x060012B1 RID: 4785 RVA: 0x0006CDEE File Offset: 0x0006AFEE
		public override void OnConnectedToMaster()
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnConnectedToMaster);
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x0006CDF8 File Offset: 0x0006AFF8
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
				if (this.initialGameMode.Contains("CASUAL") || this.initialGameMode.Contains("INFECTION"))
				{
					PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Tag Manager", this.playerOffset.transform.position, this.playerOffset.transform.rotation, 0, null);
				}
				else if (this.initialGameMode.Contains("HUNT"))
				{
					PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Hunt Manager", this.playerOffset.transform.position, this.playerOffset.transform.rotation, 0, null);
				}
				else if (this.initialGameMode.Contains("BATTLE"))
				{
					PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Battle Manager", this.playerOffset.transform.position, this.playerOffset.transform.rotation, 0, null);
				}
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

		// Token: 0x060012B3 RID: 4787 RVA: 0x0006D0CC File Offset: 0x0006B2CC
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

		// Token: 0x060012B4 RID: 4788 RVA: 0x0006D140 File Offset: 0x0006B340
		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnJoinRandomFailed);
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x0006D14C File Offset: 0x0006B34C
		private void CreatePublicRoom(bool joinWithFriends)
		{
			Hashtable customRoomProperties;
			if (this.currentJoinTrigger.gameModeName != "city" && this.currentJoinTrigger.gameModeName != "basement")
			{
				customRoomProperties = new Hashtable
				{
					{
						"gameMode",
						this.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + GorillaComputer.instance.currentGameMode
					}
				};
			}
			else
			{
				customRoomProperties = new Hashtable
				{
					{
						"gameMode",
						this.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + "CASUAL"
					}
				};
			}
			Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
			roomOptions.IsVisible = true;
			roomOptions.IsOpen = true;
			roomOptions.MaxPlayers = this.GetRoomSize(this.currentJoinTrigger.gameModeName);
			roomOptions.CustomRoomProperties = customRoomProperties;
			roomOptions.PublishUserId = true;
			roomOptions.CustomRoomPropertiesForLobby = new string[]
			{
				"gameMode"
			};
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

		// Token: 0x060012B6 RID: 4790 RVA: 0x0006D2A4 File Offset: 0x0006B4A4
		private void CreatePrivateRoom()
		{
			this.currentJoinTrigger = this.privateTrigger;
			Hashtable customRoomProperties = new Hashtable
			{
				{
					"gameMode",
					this.currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + GorillaComputer.instance.currentGameMode
				}
			};
			Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
			roomOptions.IsVisible = false;
			roomOptions.IsOpen = true;
			roomOptions.MaxPlayers = this.GetRoomSize(this.currentJoinTrigger.gameModeName);
			roomOptions.CustomRoomProperties = customRoomProperties;
			roomOptions.PublishUserId = true;
			roomOptions.CustomRoomPropertiesForLobby = new string[]
			{
				"gameMode"
			};
			PhotonNetwork.CreateRoom(this.customRoomID, roomOptions, null, null);
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x0006D352 File Offset: 0x0006B552
		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnCreateRoomFailed);
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x0006D35C File Offset: 0x0006B55C
		public void ConnectToRegion(string region)
		{
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
			base.StartCoroutine(this.ConnectUsingSettingsWithBackoff());
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x0006D37B File Offset: 0x0006B57B
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

		// Token: 0x060012BA RID: 4794 RVA: 0x0006D38C File Offset: 0x0006B58C
		public void AttemptJoinPublicWithFriends(GorillaNetworkJoinTrigger triggeredTrigger)
		{
			this.currentJoinTrigger = triggeredTrigger;
			this.joiningWithFriend = true;
			this.keyToFollow = PhotonNetwork.LocalPlayer.ActorNumber.ToString() + this.keyStr;
			this.BroadcastMyRoom(false);
			this.ProcessState(PhotonNetworkController.ConnectionEvent.AttemptJoinPublicRoom);
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x0006D3D8 File Offset: 0x0006B5D8
		public void AttemptToFollowFriendIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr)
		{
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = actorNumberToFollow.ToString() + newKeyStr;
			this.actorIdToFollow = actorNumberToFollow;
			this.shuffler = shufflerStr;
			this.ProcessState(PhotonNetworkController.ConnectionEvent.FollowFriendToPub);
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0006D40B File Offset: 0x0006B60B
		public void AttemptDisconnect()
		{
			this.ProcessState(PhotonNetworkController.ConnectionEvent.Disconnect);
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x0006D414 File Offset: 0x0006B614
		private void DisconnectFromRoom(PhotonNetworkController.ConnectionState newState)
		{
			this.currentState = newState;
			PhotonNetwork.Disconnect();
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0006D422 File Offset: 0x0006B622
		public override void OnDisconnected(DisconnectCause cause)
		{
			this.DisconnectCleanup();
			this.ProcessState(PhotonNetworkController.ConnectionEvent.OnDisconnected);
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x0006D434 File Offset: 0x0006B634
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

		// Token: 0x060012C0 RID: 4800 RVA: 0x0006D49E File Offset: 0x0006B69E
		private IEnumerator ReGetNonce()
		{
			yield return new WaitForSeconds(3f);
			Users.GetUserProof().OnComplete(new Message<UserProof>.Callback(this.GetOculusNonceCallback));
			yield return null;
			yield break;
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x0006D4AD File Offset: 0x0006B6AD
		public void WrongVersion()
		{
			this.wrongVersion = true;
			this.currentState = PhotonNetworkController.ConnectionState.WrongVersion;
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x0006D4BD File Offset: 0x0006B6BD
		public void OnApplicationQuit()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev";
			}
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0006D4E0 File Offset: 0x0006B6E0
		private string ReturnRoomName()
		{
			if (this.isPrivate)
			{
				return this.customRoomID;
			}
			return this.RandomRoomName();
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0006D4F8 File Offset: 0x0006B6F8
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

		// Token: 0x060012C5 RID: 4805 RVA: 0x0006D554 File Offset: 0x0006B754
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
				string value = room.Substring(i, 1);
				int num2 = this.roomCharacters.IndexOf(value);
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

		// Token: 0x060012C6 RID: 4806 RVA: 0x0006D629 File Offset: 0x0006B829
		public byte GetRoomSize(string gameModeName)
		{
			if (gameModeName.Contains("ball"))
			{
				return 5;
			}
			return 10;
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x0006D63C File Offset: 0x0006B83C
		public void StartSearchingForFriend()
		{
			this.startingToLookForFriend = Time.time;
			this.successfullyFoundFriend = false;
			base.StartCoroutine(this.SearchForFriendToJoin(this.friendToFollow, this.keyToFollow, this.shuffler));
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x0006D66F File Offset: 0x0006B86F
		private IEnumerator SearchForFriendToJoin(string userID, string keyToFollow, string shufflerToFollow)
		{
			while (!this.successfullyFoundFriend && this.startingToLookForFriend + this.timeToSpendLookingForFriend > Time.time)
			{
				try
				{
					GetSharedGroupDataRequest getSharedGroupDataRequest = new GetSharedGroupDataRequest();
					getSharedGroupDataRequest.Keys = new List<string>
					{
						keyToFollow
					};
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

		// Token: 0x060012C9 RID: 4809 RVA: 0x0006D694 File Offset: 0x0006B894
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

		// Token: 0x060012CA RID: 4810 RVA: 0x0006D714 File Offset: 0x0006B914
		public int TotalUsers()
		{
			int num = 0;
			foreach (int num2 in this.playersInRegion)
			{
				num += num2;
			}
			return num;
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x0006D741 File Offset: 0x0006B941
		public string CurrentState()
		{
			return this.currentState.ToString();
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x0006D754 File Offset: 0x0006B954
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

		// Token: 0x060012CD RID: 4813 RVA: 0x0006D7E9 File Offset: 0x0006B9E9
		public static int mod(int x, int m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04001596 RID: 5526
		public static volatile PhotonNetworkController Instance;

		// Token: 0x04001597 RID: 5527
		public int incrementCounter;

		// Token: 0x04001598 RID: 5528
		private PhotonNetworkController.ConnectionState currentState;

		// Token: 0x04001599 RID: 5529
		public PlayFabAuthenticator playFabAuthenticator;

		// Token: 0x0400159A RID: 5530
		public string[] serverRegions;

		// Token: 0x0400159B RID: 5531
		private string gameVersionType = "live1";

		// Token: 0x0400159C RID: 5532
		private int majorVersion = 1;

		// Token: 0x0400159D RID: 5533
		private int minorVersion = 1;

		// Token: 0x0400159E RID: 5534
		private int minorVersion2 = 59;

		// Token: 0x0400159F RID: 5535
		private string _gameVersionString = "";

		// Token: 0x040015A0 RID: 5536
		public bool isPrivate;

		// Token: 0x040015A1 RID: 5537
		public string customRoomID;

		// Token: 0x040015A2 RID: 5538
		public GameObject playerOffset;

		// Token: 0x040015A3 RID: 5539
		public SkinnedMeshRenderer[] offlineVRRig;

		// Token: 0x040015A4 RID: 5540
		public bool attemptingToConnect;

		// Token: 0x040015A5 RID: 5541
		private int currentRegionIndex;

		// Token: 0x040015A6 RID: 5542
		public string currentGameType;

		// Token: 0x040015A7 RID: 5543
		public bool wrongVersion;

		// Token: 0x040015A8 RID: 5544
		public bool roomCosmeticsInitialized;

		// Token: 0x040015A9 RID: 5545
		public GameObject photonVoiceObjectPrefab;

		// Token: 0x040015AA RID: 5546
		public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

		// Token: 0x040015AB RID: 5547
		private bool pastFirstConnection;

		// Token: 0x040015AC RID: 5548
		private float timeToRetryWithBackoff = 0.1f;

		// Token: 0x040015AD RID: 5549
		private bool retry;

		// Token: 0x040015AE RID: 5550
		private float lastTimeConnected;

		// Token: 0x040015AF RID: 5551
		private float lastHeadRightHandDistance;

		// Token: 0x040015B0 RID: 5552
		private float lastHeadLeftHandDistance;

		// Token: 0x040015B1 RID: 5553
		private float pauseTime;

		// Token: 0x040015B2 RID: 5554
		private float disconnectTime = 120f;

		// Token: 0x040015B3 RID: 5555
		public bool disableAFKKick;

		// Token: 0x040015B4 RID: 5556
		private float headRightHandDistance;

		// Token: 0x040015B5 RID: 5557
		private float headLeftHandDistance;

		// Token: 0x040015B6 RID: 5558
		private Quaternion headQuat;

		// Token: 0x040015B7 RID: 5559
		private Quaternion lastHeadQuat;

		// Token: 0x040015B8 RID: 5560
		public GameObject[] disableOnStartup;

		// Token: 0x040015B9 RID: 5561
		public GameObject[] enableOnStartup;

		// Token: 0x040015BA RID: 5562
		public string roomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";

		// Token: 0x040015BB RID: 5563
		public bool updatedName;

		// Token: 0x040015BC RID: 5564
		private int[] playersInRegion;

		// Token: 0x040015BD RID: 5565
		private int[] pingInRegion;

		// Token: 0x040015BE RID: 5566
		public List<string> friendIDList = new List<string>();

		// Token: 0x040015BF RID: 5567
		private bool successfullyFoundFriend;

		// Token: 0x040015C0 RID: 5568
		private float startingToLookForFriend;

		// Token: 0x040015C1 RID: 5569
		private float timeToSpendLookingForFriend = 15f;

		// Token: 0x040015C2 RID: 5570
		private bool joiningWithFriend;

		// Token: 0x040015C3 RID: 5571
		private string friendToFollow;

		// Token: 0x040015C4 RID: 5572
		private string keyToFollow;

		// Token: 0x040015C5 RID: 5573
		private int actorIdToFollow;

		// Token: 0x040015C6 RID: 5574
		public string shuffler;

		// Token: 0x040015C7 RID: 5575
		public string keyStr;

		// Token: 0x040015C8 RID: 5576
		private Photon.Realtime.Player tempPlayer;

		// Token: 0x040015C9 RID: 5577
		private bool isRoomFull;

		// Token: 0x040015CA RID: 5578
		private bool doesRoomExist;

		// Token: 0x040015CB RID: 5579
		private bool createRoom;

		// Token: 0x040015CC RID: 5580
		private string startLevel;

		// Token: 0x040015CD RID: 5581
		private GTZone startZone;

		// Token: 0x040015CE RID: 5582
		public GorillaNetworkJoinTrigger privateTrigger;

		// Token: 0x040015CF RID: 5583
		internal string initialGameMode = "";

		// Token: 0x040015D0 RID: 5584
		public GorillaNetworkJoinTrigger currentJoinTrigger;

		// Token: 0x040015D1 RID: 5585
		public bool allowedInPubRoom;

		// Token: 0x020004CD RID: 1229
		public enum ConnectionState
		{
			// Token: 0x04001FF2 RID: 8178
			Initialization,
			// Token: 0x04001FF3 RID: 8179
			WrongVersion,
			// Token: 0x04001FF4 RID: 8180
			DeterminingPingsAndPlayerCount,
			// Token: 0x04001FF5 RID: 8181
			ConnectedAndWaiting,
			// Token: 0x04001FF6 RID: 8182
			DisconnectingFromRoom,
			// Token: 0x04001FF7 RID: 8183
			JoiningPublicRoom,
			// Token: 0x04001FF8 RID: 8184
			JoiningSpecificRoom,
			// Token: 0x04001FF9 RID: 8185
			JoiningFriend,
			// Token: 0x04001FFA RID: 8186
			InPrivateRoom,
			// Token: 0x04001FFB RID: 8187
			InPublicRoom
		}

		// Token: 0x020004CE RID: 1230
		public enum ConnectionEvent
		{
			// Token: 0x04001FFD RID: 8189
			InitialConnection,
			// Token: 0x04001FFE RID: 8190
			OnConnectedToMaster,
			// Token: 0x04001FFF RID: 8191
			AttemptJoinPublicRoom,
			// Token: 0x04002000 RID: 8192
			AttemptJoinSpecificRoom,
			// Token: 0x04002001 RID: 8193
			AttemptToCreateRoom,
			// Token: 0x04002002 RID: 8194
			Disconnect,
			// Token: 0x04002003 RID: 8195
			OnJoinedRoom,
			// Token: 0x04002004 RID: 8196
			OnJoinRoomFailed,
			// Token: 0x04002005 RID: 8197
			OnJoinRandomFailed,
			// Token: 0x04002006 RID: 8198
			OnCreateRoomFailed,
			// Token: 0x04002007 RID: 8199
			OnDisconnected,
			// Token: 0x04002008 RID: 8200
			FoundFriendToJoin,
			// Token: 0x04002009 RID: 8201
			FollowFriendToPub
		}
	}
}
