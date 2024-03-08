using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaTag.Audio;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

[RequireComponent(typeof(PUNCallbackNotifier))]
public class NetworkSystemPUN : NetworkSystem
{
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.punVoice;
		}
	}

	private int lowestPingRegionIndex
	{
		get
		{
			int num = 9999;
			int num2 = -1;
			for (int i = 0; i < this.regionData.Length; i++)
			{
				if (this.regionData[i].pingToRegion < num)
				{
					num = this.regionData[i].pingToRegion;
					num2 = i;
				}
			}
			return num2;
		}
	}

	private NetworkSystemPUN.InternalState internalState
	{
		get
		{
			return this.currentState;
		}
		set
		{
			this.currentState = value;
		}
	}

	public override string CurrentPhotonBackend
	{
		get
		{
			return "PUN";
		}
	}

	public override bool IsOnline
	{
		get
		{
			return this.LocalPlayerID != -1;
		}
	}

	public override bool InRoom
	{
		get
		{
			return PhotonNetwork.InRoom;
		}
	}

	public override string RoomName
	{
		get
		{
			return PhotonNetwork.CurrentRoom.Name;
		}
	}

	public override string GameModeString
	{
		get
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (obj != null)
			{
				return obj.ToString();
			}
			return null;
		}
	}

	public override string CurrentRegion
	{
		get
		{
			return PhotonNetwork.CloudRegion;
		}
	}

	public override bool SessionIsPrivate
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && !currentRoom.IsVisible;
		}
	}

	public override int LocalPlayerID
	{
		get
		{
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	public override int MasterAuthID
	{
		get
		{
			return PhotonNetwork.MasterClient.ActorNumber;
		}
	}

	public override int[] AllPlayerIDs
	{
		get
		{
			return this.playerIDCache;
		}
	}

	public override float SimTime
	{
		get
		{
			return Time.time;
		}
	}

	public override float SimDeltaTime
	{
		get
		{
			return Time.deltaTime;
		}
	}

	public override int SimTick
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	public override int RoomPlayerCount
	{
		get
		{
			return (int)PhotonNetwork.CurrentRoom.PlayerCount;
		}
	}

	public override async void Initialise()
	{
		base.Initialise();
		base.netState = NetSystemState.Initialization;
		PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = NetworkSystemConfig.AppVersion;
		PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
		PhotonNetwork.EnableCloseConnection = false;
		PhotonNetwork.AutomaticallySyncScene = true;
		if (PlayerPrefs.GetString("playerName") != "")
		{
			this.SetMyNickName(PlayerPrefs.GetString("playerName"));
		}
		else
		{
			this.SetMyNickName("gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0'));
		}
		await this.CacheRegionInfo();
	}

	private async Task CacheRegionInfo()
	{
		if (!this.isWrongVersion)
		{
			this.regionData = new NetworkRegionInfo[this.regionNames.Length];
			for (int i = 0; i < this.regionData.Length; i++)
			{
				this.regionData[i] = new NetworkRegionInfo();
			}
			this.currentRegionIndex = 0;
			TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Authenticated }).GetAwaiter();
			TaskAwaiter<bool> taskAwaiter2;
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (taskAwaiter.GetResult())
			{
				base.netState = NetSystemState.PingRecon;
				while (this.currentRegionIndex < this.regionNames.Length)
				{
					this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.currentRegionIndex];
					PhotonNetwork.ConnectUsingSettings();
					taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster }).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						return;
					}
					this.regionData[this.currentRegionIndex].playersInRegion = PhotonNetwork.CountOfPlayers;
					this.regionData[this.currentRegionIndex].pingToRegion = PhotonNetwork.GetPing();
					Debug.Log("Ping for " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion.ToString() + " is " + PhotonNetwork.GetPing().ToString());
					this.internalState = NetworkSystemPUN.InternalState.PingGathering;
					PhotonNetwork.Disconnect();
					taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Internal_Disconnected }).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						return;
					}
					this.currentRegionIndex++;
				}
				this.internalState = NetworkSystemPUN.InternalState.Idle;
				base.netState = NetSystemState.Idle;
			}
		}
	}

	public override void SetAuthenticationValues(Dictionary<string, string> authValues)
	{
		this.internalState = NetworkSystemPUN.InternalState.Authenticated;
	}

	private async Task WaitForState(CancellationToken ct, params NetworkSystemPUN.InternalState[] desiredStates)
	{
		float timeoutTime = Time.time + 10f;
		while (!desiredStates.Contains(this.internalState) && !ct.IsCancellationRequested)
		{
			if (timeoutTime < Time.time)
			{
				string text = "";
				foreach (NetworkSystemPUN.InternalState internalState in desiredStates)
				{
					text += string.Format("- {0}", internalState);
				}
				Debug.LogError("Got stuck waiting for states " + text);
				this.internalState = NetworkSystemPUN.InternalState.StateCheckFailed;
				break;
			}
			await Task.Yield();
		}
	}

	private Task<bool> WaitForStateCheck(params NetworkSystemPUN.InternalState[] desiredStates)
	{
		NetworkSystemPUN.<WaitForStateCheck>d__48 <WaitForStateCheck>d__;
		<WaitForStateCheck>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForStateCheck>d__.<>4__this = this;
		<WaitForStateCheck>d__.desiredStates = desiredStates;
		<WaitForStateCheck>d__.<>1__state = -1;
		<WaitForStateCheck>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForStateCheck>d__48>(ref <WaitForStateCheck>d__);
		return <WaitForStateCheck>d__.<>t__builder.Task;
	}

	private async Task<NetJoinResult> MakeOrFindRoom(string roomName, RoomConfig opts)
	{
		if (this.InRoom)
		{
			await this.InternalDisconnect();
		}
		this.currentRegionIndex = 0;
		bool flag = await this.TryJoinRoom(roomName);
		NetJoinResult netJoinResult;
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_JoinFailed_Full)
		{
			netJoinResult = NetJoinResult.Failed_Full;
		}
		else if (!flag)
		{
			netJoinResult = await this.TryCreateRoom(roomName, opts);
		}
		else
		{
			netJoinResult = NetJoinResult.Success;
		}
		return netJoinResult;
	}

	private async Task<bool> TryJoinRoom(string roomName)
	{
		bool foundRoom = false;
		while (this.currentRegionIndex < this.regionNames.Length && !foundRoom)
		{
			this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.currentRegionIndex];
			PhotonNetwork.ConnectUsingSettings();
			TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster }).GetAwaiter();
			TaskAwaiter<bool> taskAwaiter2;
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			bool flag;
			if (!taskAwaiter.GetResult())
			{
				flag = false;
			}
			else
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_Joining;
				Debug.Log("Looking for room in" + this.regionNames[this.currentRegionIndex]);
				PhotonNetwork.JoinRoom(roomName, null);
				taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Joined,
					NetworkSystemPUN.InternalState.Searching_JoinFailed,
					NetworkSystemPUN.InternalState.Searching_JoinFailed_Full
				}).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					flag = false;
				}
				else
				{
					foundRoom = this.internalState == NetworkSystemPUN.InternalState.Searching_Joined;
					if (foundRoom)
					{
						continue;
					}
					Debug.Log("room not found!");
					PhotonNetwork.Disconnect();
					this.internalState = NetworkSystemPUN.InternalState.Searching_Disconnecting;
					taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Searching_Disconnected }).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (taskAwaiter.GetResult())
					{
						this.currentRegionIndex++;
						continue;
					}
					flag = false;
				}
			}
			return flag;
		}
		return foundRoom;
	}

	private async Task<NetJoinResult> TryCreateRoom(string roomName, RoomConfig opts)
	{
		Debug.Log("returning to best region to create room");
		this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		PhotonNetwork.ConnectUsingSettings();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster }).GetAwaiter();
		TaskAwaiter<bool> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		NetJoinResult netJoinResult;
		if (!taskAwaiter.GetResult())
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
			PhotonNetwork.CreateRoom(roomName, opts.ToPUNOpts(), null, null);
			taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
			{
				NetworkSystemPUN.InternalState.Searching_Created,
				NetworkSystemPUN.InternalState.Searching_CreateFailed
			}).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else if (this.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else
			{
				netJoinResult = NetJoinResult.FallbackCreated;
			}
		}
		return netJoinResult;
	}

	private async Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		if (this.InRoom)
		{
			await this.InternalDisconnect();
		}
		this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
		if (!opts.IsJoiningWithFriends)
		{
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.GetRandomWeightedRegion();
		}
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		PhotonNetwork.ConnectUsingSettings();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster }).GetAwaiter();
		TaskAwaiter<bool> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		NetJoinResult netJoinResult;
		if (!taskAwaiter.GetResult())
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joining;
			if (opts.IsJoiningWithFriends)
			{
				PhotonNetwork.JoinRandomRoom(opts.customProps, opts.MaxPlayers, MatchmakingMode.RandomMatching, null, null, opts.joinFriendIDs.ToArray<string>());
			}
			else
			{
				PhotonNetwork.JoinRandomRoom(opts.customProps, opts.MaxPlayers, MatchmakingMode.FillRoom, null, null, null);
			}
			taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
			{
				NetworkSystemPUN.InternalState.Searching_Joined,
				NetworkSystemPUN.InternalState.Searching_JoinFailed
			}).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else if (this.internalState == NetworkSystemPUN.InternalState.Searching_JoinFailed)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
				if (opts.IsJoiningWithFriends)
				{
					PhotonNetwork.CreateRoom(NetworkSystem.GetRandomRoomName(), opts.ToPUNOpts(), null, opts.joinFriendIDs);
				}
				else
				{
					PhotonNetwork.CreateRoom(NetworkSystem.GetRandomRoomName(), opts.ToPUNOpts(), null, null);
				}
				taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Created,
					NetworkSystemPUN.InternalState.Searching_CreateFailed
				}).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					netJoinResult = NetJoinResult.Failed_Other;
				}
				else if (this.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
				{
					netJoinResult = NetJoinResult.Failed_Other;
				}
				else
				{
					netJoinResult = NetJoinResult.FallbackCreated;
				}
			}
			else
			{
				netJoinResult = NetJoinResult.Success;
			}
		}
		return netJoinResult;
	}

	public override async Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts)
	{
		NetJoinResult netJoinResult;
		if (this.isWrongVersion)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else if (base.netState != NetSystemState.Idle && base.netState != NetSystemState.InGame)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else if (this.InRoom && roomName == this.RoomName)
		{
			netJoinResult = NetJoinResult.AlreadyInRoom;
		}
		else
		{
			base.netState = NetSystemState.Connecting;
			Debug.Log("Connecting to:" + ((roomName == null) ? "random room" : roomName));
			NetJoinResult netJoinResult2;
			if (roomName != null)
			{
				this.roomTask = this.MakeOrFindRoom(roomName, opts);
				netJoinResult2 = await this.roomTask;
				this.roomTask = null;
				Debug.Log(netJoinResult2.ToString());
			}
			else
			{
				this.roomTask = this.JoinRandomPublicRoom(opts);
				netJoinResult2 = await this.roomTask;
				this.roomTask = null;
				Debug.Log(netJoinResult2.ToString());
			}
			if (netJoinResult2 == NetJoinResult.Failed_Full || netJoinResult2 == NetJoinResult.Failed_Other)
			{
				this.ResetSystem();
				netJoinResult = netJoinResult2;
			}
			else if (netJoinResult2 == NetJoinResult.AlreadyInRoom)
			{
				base.netState = NetSystemState.InGame;
				netJoinResult = netJoinResult2;
			}
			else
			{
				this.AddVoice();
				this.UpdatePlayerIDCache();
				this.UpdateNetPlayerList();
				base.netState = NetSystemState.InGame;
				Debug.Log("MULTIPLAYER STARTED!");
				base.MultiplayerStarted();
				base.PlayerJoined(this.LocalPlayerID);
				this.localRecorder.StartRecording();
				Debug.Log("Connect to room result: " + netJoinResult2.ToString());
				netJoinResult = netJoinResult2;
			}
		}
		return netJoinResult;
	}

	public override async Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		bool foundFriend = false;
		float searchStartTime = Time.time;
		float timeToSpendSearching = 15f;
		while (!foundFriend && searchStartTime + timeToSpendSearching > Time.time)
		{
			NetworkSystemPUN.<>c__DisplayClass54_0 CS$<>8__locals1 = new NetworkSystemPUN.<>c__DisplayClass54_0();
			CS$<>8__locals1.data = new Dictionary<string, SharedGroupDataRecord>();
			CS$<>8__locals1.callbackFinished = false;
			PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
			{
				Keys = new List<string> { keyToFollow },
				SharedGroupId = userID
			}, delegate(GetSharedGroupDataResult result)
			{
				CS$<>8__locals1.data = result.Data;
				CS$<>8__locals1.callbackFinished = true;
			}, delegate(PlayFabError error)
			{
				Debug.Log("error in returning!");
				CS$<>8__locals1.callbackFinished = true;
			}, null, null);
			while (!CS$<>8__locals1.callbackFinished)
			{
				await Task.Yield();
			}
			foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in CS$<>8__locals1.data)
			{
				Debug.Log("KEY:" + keyValuePair.Key + " - Key to follow:" + keyToFollow);
				Debug.Log(keyValuePair.Value);
				if (keyValuePair.Key == keyToFollow)
				{
					string roomID = NetworkSystem.ShuffleRoomName(keyValuePair.Value.Value, shufflerToFollow, false);
					foundFriend = true;
					Player player;
					if (this.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorIDToFollow, out player) && player != null)
					{
						GorillaNot.instance.SendReport("possible kick attempt", player.UserId, player.NickName);
					}
					else
					{
						Debug.Log("following friend to " + roomID);
						if (this.RoomName != roomID)
						{
							await this.ReturnToSinglePlayer();
							Task<NetJoinResult> ConnectToRoomTask = this.ConnectToRoom(roomID, new RoomConfig
							{
								createIfMissing = false,
								isPublic = true,
								isJoinable = true
							});
							await ConnectToRoomTask;
							NetJoinResult result2 = ConnectToRoomTask.Result;
							ConnectToRoomTask = null;
						}
					}
				}
			}
			Dictionary<string, SharedGroupDataRecord>.Enumerator enumerator = default(Dictionary<string, SharedGroupDataRecord>.Enumerator);
			CS$<>8__locals1 = null;
		}
	}

	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	public override string GetRandomWeightedRegion()
	{
		float value = Random.value;
		int num = 0;
		for (int i = 0; i < this.regionData.Length; i++)
		{
			num += this.regionData[i].playersInRegion;
		}
		float num2 = 0f;
		int num3 = -1;
		while (num2 < value && num3 < this.regionData.Length - 1)
		{
			num3++;
			num2 += (float)this.regionData[num3].playersInRegion / (float)num;
		}
		return this.regionNames[num3];
	}

	public override async Task ReturnToSinglePlayer()
	{
		if (base.netState == NetSystemState.InGame)
		{
			base.netState = NetSystemState.Disconnecting;
			this._taskCancelTokens.ForEach(delegate(CancellationTokenSource cts)
			{
				cts.Cancel();
			});
			this._taskCancelTokens.Clear();
			await this.InternalDisconnect();
			base.netState = NetSystemState.Idle;
		}
	}

	private async Task InternalDisconnect()
	{
		Debug.Log("InternalDisconnect running");
		this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnecting;
		PhotonNetwork.Disconnect();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Internal_Disconnected }).GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		if (!taskAwaiter.GetResult())
		{
			Debug.LogError("Failed to achieve internal disconnected state");
		}
		Object.Destroy(this.VoiceNetworkObject);
		this.UpdateNetPlayerList();
		this.UpdatePlayerIDCache();
		base.SinglePlayerStarted();
	}

	private void AddVoice()
	{
		this.SetupVoice();
	}

	private void SetupVoice()
	{
		this.punVoice = PhotonVoiceNetwork.Instance;
		this.VoiceNetworkObject = this.punVoice.gameObject;
		this.VoiceNetworkObject.name = "VoiceNetworkObject";
		this.VoiceNetworkObject.transform.parent = base.transform;
		this.VoiceNetworkObject.transform.localPosition = Vector3.zero;
		this.punVoice.LogLevel = this.VoiceSettings.LogLevel;
		this.punVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
		this.punVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
		this.punVoice.AutoConnectAndJoin = this.VoiceSettings.AutoConnectAndJoin;
		this.punVoice.AutoLeaveAndDisconnect = this.VoiceSettings.AutoLeaveAndDisconnect;
		this.punVoice.WorkInOfflineMode = this.VoiceSettings.WorkInOfflineMode;
		this.punVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
		AppSettings appSettings = new AppSettings();
		appSettings.AppIdRealtime = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
		appSettings.AppIdVoice = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice;
		this.punVoice.Settings = appSettings;
		this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
		{
			this.punVoice.RemoteVoiceAdded += callback;
		});
		this.localRecorder = this.VoiceNetworkObject.GetComponent<Recorder>();
		if (this.localRecorder == null)
		{
			this.localRecorder = this.VoiceNetworkObject.AddComponent<Recorder>();
		}
		this.localRecorder.LogLevel = this.VoiceSettings.LogLevel;
		this.localRecorder.RecordOnlyWhenEnabled = this.VoiceSettings.RecordOnlyWhenEnabled;
		this.localRecorder.RecordOnlyWhenJoined = this.VoiceSettings.RecordOnlyWhenJoined;
		this.localRecorder.StopRecordingWhenPaused = this.VoiceSettings.StopRecordingWhenPaused;
		this.localRecorder.TransmitEnabled = this.VoiceSettings.TransmitEnabled;
		this.localRecorder.AutoStart = this.VoiceSettings.AutoStart;
		this.localRecorder.Encrypt = this.VoiceSettings.Encrypt;
		this.localRecorder.FrameDuration = this.VoiceSettings.FrameDuration;
		this.localRecorder.SamplingRate = this.VoiceSettings.SamplingRate;
		this.localRecorder.InterestGroup = this.VoiceSettings.InterestGroup;
		this.localRecorder.SourceType = this.VoiceSettings.InputSourceType;
		this.localRecorder.MicrophoneType = this.VoiceSettings.MicrophoneType;
		this.localRecorder.UseMicrophoneTypeFallback = this.VoiceSettings.UseFallback;
		this.localRecorder.VoiceDetection = this.VoiceSettings.Detect;
		this.localRecorder.VoiceDetectionThreshold = this.VoiceSettings.Threshold;
		this.localRecorder.Bitrate = this.VoiceSettings.Bitrate;
		this.localRecorder.VoiceDetectionDelayMs = this.VoiceSettings.Delay;
		this.localRecorder.DebugEchoMode = this.VoiceSettings.DebugEcho;
		this.punVoice.PrimaryRecorder = this.localRecorder;
		this.VoiceNetworkObject.AddComponent<VoiceToLoudness>();
	}

	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return Object.Instantiate<GameObject>(prefab, position, rotation);
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, 0, null);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, 0, null);
	}

	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, rotation, false);
	}

	public override void NetDestroy(GameObject instance)
	{
		PhotonView photonView;
		if (instance.TryGetComponent<PhotonView>(out photonView) && photonView.AmOwner)
		{
			PhotonNetwork.Destroy(instance);
			return;
		}
		Object.Destroy(instance);
	}

	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
	}

	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { NetworkSystem.EmptyArgs });
	}

	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		(ref args).SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { args.Data });
	}

	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { message });
	}

	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { NetworkSystem.EmptyArgs });
	}

	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		(ref args).SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { args.Data });
	}

	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { message });
	}

	public override async Task AwaitSceneReady()
	{
		while (PhotonNetwork.LevelLoadingProgress < 1f)
		{
			await Task.Yield();
		}
	}

	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != PhotonNetwork.PlayerList.Length)
		{
			this.UpdatePlayerIDCache();
			this.UpdateNetPlayerList();
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.IsLocal)
			{
				return netPlayer;
			}
		}
		Debug.LogError("Somehow no local net players found. This shouldn't happen");
		return null;
	}

	public override NetPlayer GetPlayer(int PlayerID)
	{
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ID == PlayerID)
			{
				return netPlayer;
			}
		}
		this.UpdatePlayerIDCache();
		this.UpdateNetPlayerList();
		foreach (NetPlayer netPlayer2 in this.netPlayerCache)
		{
			if (netPlayer2.ID == PlayerID)
			{
				return netPlayer2;
			}
		}
		Debug.LogError("There is no NetPlayer with this ID currently in game. Passed ID: " + PlayerID.ToString());
		return null;
	}

	public override void SetMyNickName(string id)
	{
		PlayerPrefs.SetString("playerName", id);
		PhotonNetwork.LocalPlayer.NickName = id;
	}

	public override string GetMyNickName()
	{
		return PhotonNetwork.LocalPlayer.NickName;
	}

	public override string GetNickName(int playerID)
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (player.ActorNumber == playerID)
			{
				return player.NickName;
			}
		}
		Debug.LogWarning(string.Format("Couldn't find NickName for player #{0}", playerID));
		return null;
	}

	public override void SetMyTutorialComplete()
	{
		bool flag = PlayerPrefs.GetString("didTutorial", "nope") == "done";
		if (!flag)
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", flag);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}

	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		Player[] array = PhotonNetwork.PlayerList;
		int i = 0;
		while (i < array.Length)
		{
			Player player = array[i];
			if (player.ActorNumber == playerID)
			{
				object obj;
				player.CustomProperties.TryGetValue("didTutorial", out obj);
				if (obj == null)
				{
					Debug.LogError("Couldnt get tutorial status for user, no property found");
					return false;
				}
				if (!(bool)obj)
				{
					Debug.Log("Player hasnt completed tutorial");
					return false;
				}
				Debug.Log("Player has completed tutorial");
				return true;
			}
			else
			{
				i++;
			}
		}
		Debug.LogError("Player not found, cant provide tutoprial status");
		return false;
	}

	public override string GetMyUserID()
	{
		return PhotonNetwork.LocalPlayer.UserId;
	}

	public override string GetUserID(int playerID)
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (player.ActorNumber == playerID)
			{
				return player.UserId;
			}
		}
		return null;
	}

	public override int GlobalPlayerCount()
	{
		int num = 0;
		foreach (NetworkRegionInfo networkRegionInfo in this.regionData)
		{
			num += networkRegionInfo.playersInRegion;
		}
		return num;
	}

	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		PhotonView photonView;
		return !this.IsOnline || !obj.TryGetComponent<PhotonView>(out photonView) || photonView.IsMine;
	}

	protected override void UpdateNetPlayerList()
	{
		if (!this.IsOnline)
		{
			bool flag = false;
			if (this.netPlayerCache.Count > 0)
			{
				foreach (NetPlayer netPlayer in this.netPlayerCache.ToArray())
				{
					if (((PunNetPlayer)netPlayer).playerRef == PhotonNetwork.LocalPlayer)
					{
						flag = true;
					}
					else
					{
						this.netPlayerCache.Remove(netPlayer);
					}
				}
			}
			if (!flag)
			{
				this.netPlayerCache.Add(new PunNetPlayer(PhotonNetwork.LocalPlayer));
			}
			return;
		}
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			bool flag2 = false;
			for (int j = 0; j < this.netPlayerCache.Count; j++)
			{
				if (player == ((PunNetPlayer)this.netPlayerCache[j]).playerRef)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				this.netPlayerCache.Add(new PunNetPlayer(player));
			}
		}
		foreach (NetPlayer netPlayer2 in this.netPlayerCache.ToArray())
		{
			bool flag3 = false;
			Player[] array2 = PhotonNetwork.PlayerList;
			for (int k = 0; k < array2.Length; k++)
			{
				if (array2[k] == ((PunNetPlayer)netPlayer2).playerRef)
				{
					flag3 = true;
				}
			}
			if (!flag3)
			{
				this.netPlayerCache.Remove(netPlayer2);
			}
		}
	}

	protected override void UpdatePlayerIDCache()
	{
		if (!this.IsOnline)
		{
			this.playerIDCache = new int[1];
			return;
		}
		Player[] array = PhotonNetwork.PlayerList;
		this.playerIDCache = new int[array.Count<Player>()];
		for (int i = 0; i < array.Count<Player>(); i++)
		{
			this.playerIDCache[i] = array[i].ActorNumber;
		}
	}

	public override bool IsObjectRoomObject(GameObject obj)
	{
		PhotonView component = obj.GetComponent<PhotonView>();
		if (component == null)
		{
			Debug.LogError("No photonview found on this Object, this shouldn't happen");
			return false;
		}
		return component.IsRoomView;
	}

	public override bool ShouldUpdateObject(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	public override bool ShouldWriteObjectData(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	public override int GetOwningPlayerID(GameObject obj)
	{
		PhotonView photonView;
		if (obj.TryGetComponent<PhotonView>(out photonView) && photonView.Owner != null)
		{
			return photonView.Owner.ActorNumber;
		}
		return -1;
	}

	public override bool ShouldSpawnLocally(int playerID)
	{
		return this.LocalPlayerID == playerID || (playerID == -1 && PhotonNetwork.MasterClient.IsLocal);
	}

	public override bool IsTotalAuthority()
	{
		return false;
	}

	public void OnConnectedtoMaster()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.ConnectingToMaster)
		{
			this.internalState = NetworkSystemPUN.InternalState.ConnectedToMaster;
		}
	}

	public void OnJoinedRoom()
	{
		Debug.Log("<color=blue>On Joined Room</color>");
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joined;
			return;
		}
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Created;
			return;
		}
		Debug.Log("Confusion");
	}

	public void OnJoinRoomFailed(short returnCode, string message)
	{
		if (this.internalState != NetworkSystemPUN.InternalState.Searching_Joining)
		{
			Debug.Log("Confusion");
			return;
		}
		if (returnCode == 32765)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_Full;
			return;
		}
		this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed;
	}

	public void OnCreateRoomFailed(short returnCode, string message)
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_CreateFailed;
		}
	}

	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.UpdatePlayerIDCache();
		this.UpdateNetPlayerList();
		base.PlayerJoined(newPlayer.ActorNumber);
	}

	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.PlayerLeft(otherPlayer.ActorNumber);
		this.UpdatePlayerIDCache();
		this.UpdateNetPlayerList();
	}

	public async void OnDisconnected(DisconnectCause cause)
	{
		await base.RefreshOculusNonce();
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Disconnecting)
		{
			Debug.Log("Disconnect - Searching_Disconnected");
			this.internalState = NetworkSystemPUN.InternalState.Searching_Disconnected;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.PingGathering)
		{
			Debug.Log("Disconnect - PingGather_Disconnected");
			this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnected;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.Internal_Disconnecting)
		{
			Debug.Log("Disconnect - Internal_Disconnected");
			this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnected;
		}
		else
		{
			Debug.Log("Networksystem - Unexpected disconnected.  PUN Cause: " + cause.ToString());
			this.UpdatePlayerIDCache();
			this.UpdateNetPlayerList();
			base.SinglePlayerStarted();
		}
	}

	private ValueTuple<CancellationTokenSource, CancellationToken> GetCancellationToken()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationToken token = cancellationTokenSource.Token;
		this._taskCancelTokens.Add(cancellationTokenSource);
		return new ValueTuple<CancellationTokenSource, CancellationToken>(cancellationTokenSource, token);
	}

	public void ResetSystem()
	{
		if (this.VoiceNetworkObject)
		{
			Object.Destroy(this.VoiceNetworkObject);
		}
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		PhotonNetwork.Disconnect();
		this._taskCancelTokens.ForEach(delegate(CancellationTokenSource token)
		{
			token.Cancel();
		});
		this._taskCancelTokens.Clear();
		this.internalState = NetworkSystemPUN.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	private NetworkRegionInfo[] regionData;

	private int currentRegionIndex;

	private Task<NetJoinResult> roomTask;

	private Player[] playerList;

	private List<CancellationTokenSource> _taskCancelTokens = new List<CancellationTokenSource>();

	private PhotonVoiceNetwork punVoice;

	private GameObject VoiceNetworkObject;

	private NetworkSystemPUN.InternalState currentState;

	private enum InternalState
	{
		AwaitingAuth,
		Authenticated,
		PingGathering,
		StateCheckFailed,
		ConnectingToMaster,
		ConnectedToMaster,
		Idle,
		Internal_Disconnecting,
		Internal_Disconnected,
		Searching_Connecting,
		Searching_Connected,
		Searching_Joining,
		Searching_Joined,
		Searching_JoinFailed,
		Searching_JoinFailed_Full,
		Searching_Creating,
		Searching_Created,
		Searching_CreateFailed,
		Searching_Disconnecting,
		Searching_Disconnected
	}
}
