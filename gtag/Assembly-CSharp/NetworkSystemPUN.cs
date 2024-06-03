using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
			int result = -1;
			for (int i = 0; i < this.regionData.Length; i++)
			{
				if (this.regionData[i].pingToRegion < num)
				{
					num = this.regionData[i].pingToRegion;
					result = i;
				}
			}
			return result;
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

	public override void Initialise()
	{
		NetworkSystemPUN.<Initialise>d__44 <Initialise>d__;
		<Initialise>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialise>d__.<>4__this = this;
		<Initialise>d__.<>1__state = -1;
		<Initialise>d__.<>t__builder.Start<NetworkSystemPUN.<Initialise>d__44>(ref <Initialise>d__);
	}

	private Task CacheRegionInfo()
	{
		NetworkSystemPUN.<CacheRegionInfo>d__45 <CacheRegionInfo>d__;
		<CacheRegionInfo>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CacheRegionInfo>d__.<>4__this = this;
		<CacheRegionInfo>d__.<>1__state = -1;
		<CacheRegionInfo>d__.<>t__builder.Start<NetworkSystemPUN.<CacheRegionInfo>d__45>(ref <CacheRegionInfo>d__);
		return <CacheRegionInfo>d__.<>t__builder.Task;
	}

	public override void SetAuthenticationValues(Dictionary<string, string> authValues)
	{
		this.internalState = NetworkSystemPUN.InternalState.Authenticated;
	}

	private Task WaitForState(CancellationToken ct, params NetworkSystemPUN.InternalState[] desiredStates)
	{
		NetworkSystemPUN.<WaitForState>d__47 <WaitForState>d__;
		<WaitForState>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForState>d__.<>4__this = this;
		<WaitForState>d__.ct = ct;
		<WaitForState>d__.desiredStates = desiredStates;
		<WaitForState>d__.<>1__state = -1;
		<WaitForState>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForState>d__47>(ref <WaitForState>d__);
		return <WaitForState>d__.<>t__builder.Task;
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

	private Task<NetJoinResult> MakeOrFindRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemPUN.<MakeOrFindRoom>d__49 <MakeOrFindRoom>d__;
		<MakeOrFindRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<MakeOrFindRoom>d__.<>4__this = this;
		<MakeOrFindRoom>d__.roomName = roomName;
		<MakeOrFindRoom>d__.opts = opts;
		<MakeOrFindRoom>d__.regionIndex = regionIndex;
		<MakeOrFindRoom>d__.<>1__state = -1;
		<MakeOrFindRoom>d__.<>t__builder.Start<NetworkSystemPUN.<MakeOrFindRoom>d__49>(ref <MakeOrFindRoom>d__);
		return <MakeOrFindRoom>d__.<>t__builder.Task;
	}

	private Task<bool> TryJoinRoom(string roomName)
	{
		NetworkSystemPUN.<TryJoinRoom>d__50 <TryJoinRoom>d__;
		<TryJoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryJoinRoom>d__.<>4__this = this;
		<TryJoinRoom>d__.roomName = roomName;
		<TryJoinRoom>d__.<>1__state = -1;
		<TryJoinRoom>d__.<>t__builder.Start<NetworkSystemPUN.<TryJoinRoom>d__50>(ref <TryJoinRoom>d__);
		return <TryJoinRoom>d__.<>t__builder.Task;
	}

	private Task<bool> TryJoinRoomInRegion(string roomName, int regionIndex)
	{
		NetworkSystemPUN.<TryJoinRoomInRegion>d__51 <TryJoinRoomInRegion>d__;
		<TryJoinRoomInRegion>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryJoinRoomInRegion>d__.<>4__this = this;
		<TryJoinRoomInRegion>d__.roomName = roomName;
		<TryJoinRoomInRegion>d__.regionIndex = regionIndex;
		<TryJoinRoomInRegion>d__.<>1__state = -1;
		<TryJoinRoomInRegion>d__.<>t__builder.Start<NetworkSystemPUN.<TryJoinRoomInRegion>d__51>(ref <TryJoinRoomInRegion>d__);
		return <TryJoinRoomInRegion>d__.<>t__builder.Task;
	}

	private Task<NetJoinResult> TryCreateRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemPUN.<TryCreateRoom>d__52 <TryCreateRoom>d__;
		<TryCreateRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<TryCreateRoom>d__.<>4__this = this;
		<TryCreateRoom>d__.roomName = roomName;
		<TryCreateRoom>d__.opts = opts;
		<TryCreateRoom>d__.<>1__state = -1;
		<TryCreateRoom>d__.<>t__builder.Start<NetworkSystemPUN.<TryCreateRoom>d__52>(ref <TryCreateRoom>d__);
		return <TryCreateRoom>d__.<>t__builder.Task;
	}

	private Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		NetworkSystemPUN.<JoinRandomPublicRoom>d__53 <JoinRandomPublicRoom>d__;
		<JoinRandomPublicRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<JoinRandomPublicRoom>d__.<>4__this = this;
		<JoinRandomPublicRoom>d__.opts = opts;
		<JoinRandomPublicRoom>d__.<>1__state = -1;
		<JoinRandomPublicRoom>d__.<>t__builder.Start<NetworkSystemPUN.<JoinRandomPublicRoom>d__53>(ref <JoinRandomPublicRoom>d__);
		return <JoinRandomPublicRoom>d__.<>t__builder.Task;
	}

	public override Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemPUN.<ConnectToRoom>d__54 <ConnectToRoom>d__;
		<ConnectToRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<ConnectToRoom>d__.<>4__this = this;
		<ConnectToRoom>d__.roomName = roomName;
		<ConnectToRoom>d__.opts = opts;
		<ConnectToRoom>d__.regionIndex = regionIndex;
		<ConnectToRoom>d__.<>1__state = -1;
		<ConnectToRoom>d__.<>t__builder.Start<NetworkSystemPUN.<ConnectToRoom>d__54>(ref <ConnectToRoom>d__);
		return <ConnectToRoom>d__.<>t__builder.Task;
	}

	public override Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		NetworkSystemPUN.<JoinFriendsRoom>d__55 <JoinFriendsRoom>d__;
		<JoinFriendsRoom>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<JoinFriendsRoom>d__.<>4__this = this;
		<JoinFriendsRoom>d__.userID = userID;
		<JoinFriendsRoom>d__.actorIDToFollow = actorIDToFollow;
		<JoinFriendsRoom>d__.keyToFollow = keyToFollow;
		<JoinFriendsRoom>d__.shufflerToFollow = shufflerToFollow;
		<JoinFriendsRoom>d__.<>1__state = -1;
		<JoinFriendsRoom>d__.<>t__builder.Start<NetworkSystemPUN.<JoinFriendsRoom>d__55>(ref <JoinFriendsRoom>d__);
		return <JoinFriendsRoom>d__.<>t__builder.Task;
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

	public override Task ReturnToSinglePlayer()
	{
		NetworkSystemPUN.<ReturnToSinglePlayer>d__58 <ReturnToSinglePlayer>d__;
		<ReturnToSinglePlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ReturnToSinglePlayer>d__.<>4__this = this;
		<ReturnToSinglePlayer>d__.<>1__state = -1;
		<ReturnToSinglePlayer>d__.<>t__builder.Start<NetworkSystemPUN.<ReturnToSinglePlayer>d__58>(ref <ReturnToSinglePlayer>d__);
		return <ReturnToSinglePlayer>d__.<>t__builder.Task;
	}

	private Task InternalDisconnect()
	{
		NetworkSystemPUN.<InternalDisconnect>d__59 <InternalDisconnect>d__;
		<InternalDisconnect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InternalDisconnect>d__.<>4__this = this;
		<InternalDisconnect>d__.<>1__state = -1;
		<InternalDisconnect>d__.<>t__builder.Start<NetworkSystemPUN.<InternalDisconnect>d__59>(ref <InternalDisconnect>d__);
		return <InternalDisconnect>d__.<>t__builder.Task;
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
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			NetworkSystem.EmptyArgs
		});
	}

	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		ref args.SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			args.Data
		});
	}

	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			message
		});
	}

	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			NetworkSystem.EmptyArgs
		});
	}

	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		ref args.SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			args.Data
		});
	}

	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			message
		});
	}

	public override Task AwaitSceneReady()
	{
		NetworkSystemPUN.<AwaitSceneReady>d__73 <AwaitSceneReady>d__;
		<AwaitSceneReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitSceneReady>d__.<>1__state = -1;
		<AwaitSceneReady>d__.<>t__builder.Start<NetworkSystemPUN.<AwaitSceneReady>d__73>(ref <AwaitSceneReady>d__);
		return <AwaitSceneReady>d__.<>t__builder.Task;
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

	public override string GetMyDefaultName()
	{
		return PhotonNetwork.LocalPlayer.DefaultName;
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
				return (bool)obj;
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
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joined;
			return;
		}
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Created;
		}
	}

	public void OnJoinRoomFailed(short returnCode, string message)
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			if (returnCode == 32765)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_Full;
				return;
			}
			this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed;
		}
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

	public void OnDisconnected(DisconnectCause cause)
	{
		NetworkSystemPUN.<OnDisconnected>d__101 <OnDisconnected>d__;
		<OnDisconnected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnDisconnected>d__.<>4__this = this;
		<OnDisconnected>d__.<>1__state = -1;
		<OnDisconnected>d__.<>t__builder.Start<NetworkSystemPUN.<OnDisconnected>d__101>(ref <OnDisconnected>d__);
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
		this.currentRegionIndex = this.lowestPingRegionIndex;
		PhotonNetwork.Disconnect();
		this._taskCancelTokens.ForEach(delegate(CancellationTokenSource token)
		{
			token.Cancel();
		});
		this._taskCancelTokens.Clear();
		this.internalState = NetworkSystemPUN.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	public NetworkSystemPUN()
	{
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void <>n__0()
	{
		base.Initialise();
	}

	[CompilerGenerated]
	private void <SetupVoice>b__61_0(Action<RemoteVoiceLink> callback)
	{
		this.punVoice.RemoteVoiceAdded += callback;
	}

	private NetworkRegionInfo[] regionData;

	private Task<NetJoinResult> roomTask;

	private Player[] playerList;

	private List<CancellationTokenSource> _taskCancelTokens = new List<CancellationTokenSource>();

	private PhotonVoiceNetwork punVoice;

	private GameObject VoiceNetworkObject;

	private NetworkSystemPUN.InternalState currentState;

	private bool firstRoomJoin;

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

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal void <ReturnToSinglePlayer>b__58_0(CancellationTokenSource cts)
		{
			cts.Cancel();
		}

		internal void <ResetSystem>b__103_0(CancellationTokenSource token)
		{
			token.Cancel();
		}

		public static readonly NetworkSystemPUN.<>c <>9 = new NetworkSystemPUN.<>c();

		public static Action<CancellationTokenSource> <>9__58_0;

		public static Action<CancellationTokenSource> <>9__103_0;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass55_0
	{
		public <>c__DisplayClass55_0()
		{
		}

		internal void <JoinFriendsRoom>b__0(GetSharedGroupDataResult result)
		{
			this.data = result.Data;
			Debug.Log(string.Format("Got friend follow data, {0} entries", this.data.Count));
			this.callbackFinished = true;
		}

		internal void <JoinFriendsRoom>b__1(PlayFabError error)
		{
			Debug.Log(string.Format("GetSharedGroupData returns error: {0}", error));
			this.callbackFinished = true;
		}

		public Dictionary<string, SharedGroupDataRecord> data;

		public bool callbackFinished;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <AwaitSceneReady>d__73 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			try
			{
				if (num != 0)
				{
					goto IL_66;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_5F:
				awaiter.GetResult();
				IL_66:
				if (PhotonNetwork.LevelLoadingProgress < 1f)
				{
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemPUN.<AwaitSceneReady>d__73>(ref awaiter, ref this);
						return;
					}
					goto IL_5F;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <CacheRegionInfo>d__45 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			try
			{
				TaskAwaiter<bool> awaiter;
				switch (num)
				{
				case 0:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					break;
				case 1:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_172;
				case 2:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_24E;
				default:
					if (networkSystemPUN.isWrongVersion)
					{
						goto IL_2A7;
					}
					networkSystemPUN.regionData = new NetworkRegionInfo[networkSystemPUN.regionNames.Length];
					for (int i = 0; i < networkSystemPUN.regionData.Length; i++)
					{
						networkSystemPUN.regionData[i] = new NetworkRegionInfo();
					}
					this.<tryingRegionIndex>5__2 = 0;
					awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
					{
						NetworkSystemPUN.InternalState.Authenticated
					}).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<CacheRegionInfo>d__45>(ref awaiter, ref this);
						return;
					}
					break;
				}
				if (!awaiter.GetResult())
				{
					goto IL_2A7;
				}
				networkSystemPUN.netState = NetSystemState.PingRecon;
				goto IL_26B;
				IL_172:
				if (!awaiter.GetResult())
				{
					goto IL_2A7;
				}
				networkSystemPUN.regionData[networkSystemPUN.currentRegionIndex].playersInRegion = PhotonNetwork.CountOfPlayers;
				networkSystemPUN.regionData[networkSystemPUN.currentRegionIndex].pingToRegion = PhotonNetwork.GetPing();
				Debug.Log("Ping for " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion.ToString() + " is " + PhotonNetwork.GetPing().ToString());
				networkSystemPUN.internalState = NetworkSystemPUN.InternalState.PingGathering;
				PhotonNetwork.Disconnect();
				awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Internal_Disconnected
				}).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 2;
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<CacheRegionInfo>d__45>(ref awaiter, ref this);
					return;
				}
				IL_24E:
				if (!awaiter.GetResult())
				{
					goto IL_2A7;
				}
				int num2 = this.<tryingRegionIndex>5__2 + 1;
				this.<tryingRegionIndex>5__2 = num2;
				IL_26B:
				if (this.<tryingRegionIndex>5__2 >= networkSystemPUN.regionNames.Length)
				{
					networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Idle;
					networkSystemPUN.netState = NetSystemState.Idle;
				}
				else
				{
					networkSystemPUN.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = networkSystemPUN.regionNames[this.<tryingRegionIndex>5__2];
					networkSystemPUN.currentRegionIndex = this.<tryingRegionIndex>5__2;
					PhotonNetwork.ConnectUsingSettings();
					awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
					{
						NetworkSystemPUN.InternalState.ConnectedToMaster
					}).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 1;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<CacheRegionInfo>d__45>(ref awaiter, ref this);
						return;
					}
					goto IL_172;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_2A7:
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public NetworkSystemPUN <>4__this;

		private int <tryingRegionIndex>5__2;

		private TaskAwaiter<bool> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <ConnectToRoom>d__54 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			NetJoinResult result;
			try
			{
				TaskAwaiter<NetJoinResult> awaiter;
				NetJoinResult result2;
				if (num != 0)
				{
					if (num != 1)
					{
						if (networkSystemPUN.isWrongVersion)
						{
							result = NetJoinResult.Failed_Other;
							goto IL_1EB;
						}
						if (networkSystemPUN.netState != NetSystemState.Idle && networkSystemPUN.netState != NetSystemState.InGame)
						{
							result = NetJoinResult.Failed_Other;
							goto IL_1EB;
						}
						if (networkSystemPUN.InRoom && this.roomName == networkSystemPUN.RoomName)
						{
							result = NetJoinResult.AlreadyInRoom;
							goto IL_1EB;
						}
						networkSystemPUN.netState = NetSystemState.Connecting;
						if (this.roomName != null)
						{
							networkSystemPUN.roomTask = networkSystemPUN.MakeOrFindRoom(this.roomName, this.opts, this.regionIndex);
							awaiter = networkSystemPUN.roomTask.GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								this.<>1__state = 0;
								this.<>u__1 = awaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, NetworkSystemPUN.<ConnectToRoom>d__54>(ref awaiter, ref this);
								return;
							}
							goto IL_EE;
						}
						else
						{
							networkSystemPUN.roomTask = networkSystemPUN.JoinRandomPublicRoom(this.opts);
							awaiter = networkSystemPUN.roomTask.GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								this.<>1__state = 1;
								this.<>u__1 = awaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, NetworkSystemPUN.<ConnectToRoom>d__54>(ref awaiter, ref this);
								return;
							}
						}
					}
					else
					{
						awaiter = this.<>u__1;
						this.<>u__1 = default(TaskAwaiter<NetJoinResult>);
						this.<>1__state = -1;
					}
					result2 = awaiter.GetResult();
					networkSystemPUN.roomTask = null;
					goto IL_177;
				}
				awaiter = this.<>u__1;
				this.<>u__1 = default(TaskAwaiter<NetJoinResult>);
				this.<>1__state = -1;
				IL_EE:
				result2 = awaiter.GetResult();
				networkSystemPUN.roomTask = null;
				IL_177:
				if (result2 == NetJoinResult.Failed_Full || result2 == NetJoinResult.Failed_Other)
				{
					networkSystemPUN.ResetSystem();
					result = result2;
				}
				else if (result2 == NetJoinResult.AlreadyInRoom)
				{
					networkSystemPUN.netState = NetSystemState.InGame;
					result = result2;
				}
				else
				{
					networkSystemPUN.AddVoice();
					networkSystemPUN.UpdatePlayerIDCache();
					networkSystemPUN.UpdateNetPlayerList();
					networkSystemPUN.netState = NetSystemState.InGame;
					networkSystemPUN.MultiplayerStarted();
					networkSystemPUN.PlayerJoined(networkSystemPUN.LocalPlayerID);
					networkSystemPUN.localRecorder.StartRecording();
					result = result2;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_1EB:
			this.<>1__state = -2;
			this.<>t__builder.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<NetJoinResult> <>t__builder;

		public NetworkSystemPUN <>4__this;

		public string roomName;

		public RoomConfig opts;

		public int regionIndex;

		private TaskAwaiter<NetJoinResult> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <Initialise>d__44 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					networkSystemPUN.<>n__0();
					networkSystemPUN.netState = NetSystemState.Initialization;
					PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = NetworkSystemConfig.AppVersion;
					PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
					PhotonNetwork.EnableCloseConnection = false;
					PhotonNetwork.AutomaticallySyncScene = false;
					if (PlayerPrefs.GetString("playerName") != "")
					{
						networkSystemPUN.SetMyNickName(PlayerPrefs.GetString("playerName"));
					}
					else
					{
						networkSystemPUN.SetMyNickName("gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0'));
					}
					awaiter = networkSystemPUN.CacheRegionInfo().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemPUN.<Initialise>d__44>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
				}
				awaiter.GetResult();
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public NetworkSystemPUN <>4__this;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <InternalDisconnect>d__59 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			try
			{
				TaskAwaiter<bool> awaiter;
				if (num != 0)
				{
					networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Internal_Disconnecting;
					PhotonNetwork.Disconnect();
					awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
					{
						NetworkSystemPUN.InternalState.Internal_Disconnected
					}).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<InternalDisconnect>d__59>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
				}
				if (!awaiter.GetResult())
				{
					Debug.LogError("Failed to achieve internal disconnected state");
				}
				Object.Destroy(networkSystemPUN.VoiceNetworkObject);
				networkSystemPUN.UpdateNetPlayerList();
				networkSystemPUN.UpdatePlayerIDCache();
				networkSystemPUN.SinglePlayerStarted();
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public NetworkSystemPUN <>4__this;

		private TaskAwaiter<bool> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <JoinFriendsRoom>d__55 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			try
			{
				if (num > 3)
				{
					this.<foundFriend>5__2 = false;
					this.<searchStartTime>5__3 = Time.time;
					this.<timeToSpendSearching>5__4 = 15f;
					this.<dummyData>5__5 = new Dictionary<string, SharedGroupDataRecord>();
				}
				try
				{
					YieldAwaitable.YieldAwaiter awaiter;
					TaskAwaiter awaiter2;
					switch (num)
					{
					case 0:
						awaiter = this.<>u__1;
						this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
						num = (this.<>1__state = -1);
						break;
					case 1:
					case 2:
						IL_159:
						try
						{
							if (num == 1)
							{
								awaiter2 = this.<>u__2;
								this.<>u__2 = default(TaskAwaiter);
								num = (this.<>1__state = -1);
								goto IL_2D9;
							}
							TaskAwaiter<NetJoinResult> awaiter3;
							if (num == 2)
							{
								awaiter3 = this.<>u__3;
								this.<>u__3 = default(TaskAwaiter<NetJoinResult>);
								num = (this.<>1__state = -1);
								goto IL_370;
							}
							IL_392:
							while (this.<>7__wrap5.MoveNext())
							{
								KeyValuePair<string, SharedGroupDataRecord> keyValuePair = this.<>7__wrap5.Current;
								if (keyValuePair.Key == this.keyToFollow)
								{
									string[] array = keyValuePair.Value.Value.Split("|", StringSplitOptions.None);
									if (array.Length == 2)
									{
										this.<roomID>5__7 = NetworkSystem.ShuffleRoomName(array[0], this.shufflerToFollow.Substring(2, 8), false);
										string value = NetworkSystem.ShuffleRoomName(array[1], this.shufflerToFollow.Substring(0, 2), false);
										this.<regionIndex>5__8 = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".IndexOf(value);
										if (this.<regionIndex>5__8 >= 0 && this.<regionIndex>5__8 < NetworkSystem.Instance.regionNames.Length)
										{
											this.<foundFriend>5__2 = true;
											Player player;
											if (networkSystemPUN.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(this.actorIDToFollow, out player) && player != null)
											{
												GorillaNot.instance.SendReport("possible kick attempt", player.UserId, player.NickName);
												goto IL_38B;
											}
											if (!(networkSystemPUN.RoomName != this.<roomID>5__7))
											{
												goto IL_38B;
											}
											awaiter2 = networkSystemPUN.ReturnToSinglePlayer().GetAwaiter();
											if (!awaiter2.IsCompleted)
											{
												num = (this.<>1__state = 1);
												this.<>u__2 = awaiter2;
												this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemPUN.<JoinFriendsRoom>d__55>(ref awaiter2, ref this);
												return;
											}
											goto IL_2D9;
										}
									}
								}
							}
							goto IL_3BA;
							IL_2D9:
							awaiter2.GetResult();
							RoomConfig roomConfig = new RoomConfig();
							roomConfig.createIfMissing = false;
							roomConfig.isPublic = true;
							roomConfig.isJoinable = true;
							this.<ConnectToRoomTask>5__9 = networkSystemPUN.ConnectToRoom(this.<roomID>5__7, roomConfig, this.<regionIndex>5__8);
							awaiter3 = this.<ConnectToRoomTask>5__9.GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (this.<>1__state = 2);
								this.<>u__3 = awaiter3;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, NetworkSystemPUN.<JoinFriendsRoom>d__55>(ref awaiter3, ref this);
								return;
							}
							IL_370:
							awaiter3.GetResult();
							NetJoinResult result = this.<ConnectToRoomTask>5__9.Result;
							this.<ConnectToRoomTask>5__9 = null;
							IL_38B:
							this.<roomID>5__7 = null;
							goto IL_392;
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)this.<>7__wrap5).Dispose();
							}
						}
						IL_3BA:
						this.<>7__wrap5 = default(Dictionary<string, SharedGroupDataRecord>.Enumerator);
						awaiter2 = Task.Delay(500).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (this.<>1__state = 3);
							this.<>u__2 = awaiter2;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemPUN.<JoinFriendsRoom>d__55>(ref awaiter2, ref this);
							return;
						}
						goto IL_421;
					case 3:
						awaiter2 = this.<>u__2;
						this.<>u__2 = default(TaskAwaiter);
						num = (this.<>1__state = -1);
						goto IL_421;
					default:
						networkSystemPUN.groupJoinInProgress = true;
						goto IL_42F;
					}
					IL_12F:
					awaiter.GetResult();
					IL_136:
					if (this.<>8__1.callbackFinished)
					{
						this.<>7__wrap5 = this.<>8__1.data.GetEnumerator();
						goto IL_159;
					}
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (this.<>1__state = 0);
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemPUN.<JoinFriendsRoom>d__55>(ref awaiter, ref this);
						return;
					}
					goto IL_12F;
					IL_421:
					awaiter2.GetResult();
					this.<>8__1 = null;
					IL_42F:
					if (!this.<foundFriend>5__2 && this.<searchStartTime>5__3 + this.<timeToSpendSearching>5__4 > Time.time)
					{
						this.<>8__1 = new NetworkSystemPUN.<>c__DisplayClass55_0();
						this.<>8__1.data = this.<dummyData>5__5;
						this.<>8__1.callbackFinished = false;
						PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
						{
							Keys = new List<string>
							{
								this.keyToFollow
							},
							SharedGroupId = this.userID
						}, new Action<GetSharedGroupDataResult>(this.<>8__1.<JoinFriendsRoom>b__0), new Action<PlayFabError>(this.<>8__1.<JoinFriendsRoom>b__1), null, null);
						goto IL_136;
					}
				}
				finally
				{
					if (num < 0)
					{
						networkSystemPUN.groupJoinInProgress = false;
					}
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<dummyData>5__5 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<dummyData>5__5 = null;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public NetworkSystemPUN <>4__this;

		public string keyToFollow;

		public string userID;

		private NetworkSystemPUN.<>c__DisplayClass55_0 <>8__1;

		public string shufflerToFollow;

		public int actorIDToFollow;

		private bool <foundFriend>5__2;

		private float <searchStartTime>5__3;

		private float <timeToSpendSearching>5__4;

		private Dictionary<string, SharedGroupDataRecord> <dummyData>5__5;

		private YieldAwaitable.YieldAwaiter <>u__1;

		private Dictionary<string, SharedGroupDataRecord>.Enumerator <>7__wrap5;

		private string <roomID>5__7;

		private int <regionIndex>5__8;

		private Task<NetJoinResult> <ConnectToRoomTask>5__9;

		private TaskAwaiter <>u__2;

		private TaskAwaiter<NetJoinResult> <>u__3;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <JoinRandomPublicRoom>d__53 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			NetJoinResult result;
			try
			{
				TaskAwaiter awaiter;
				TaskAwaiter<bool> awaiter2;
				switch (num)
				{
				case 0:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
					break;
				case 1:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_172;
				case 2:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_24F;
				case 3:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_321;
				default:
					if (!networkSystemPUN.InRoom)
					{
						goto IL_8A;
					}
					awaiter = networkSystemPUN.InternalDisconnect().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemPUN.<JoinRandomPublicRoom>d__53>(ref awaiter, ref this);
						return;
					}
					break;
				}
				awaiter.GetResult();
				IL_8A:
				networkSystemPUN.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
				object obj;
				if (!networkSystemPUN.firstRoomJoin && this.opts.customProps.TryGetValue("gameMode", out obj) && !obj.ToString().Contains("city"))
				{
					networkSystemPUN.firstRoomJoin = true;
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = networkSystemPUN.regionNames[networkSystemPUN.lowestPingRegionIndex];
				}
				else if (!this.opts.IsJoiningWithFriends)
				{
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = networkSystemPUN.GetRandomWeightedRegion();
				}
				PhotonNetwork.ConnectUsingSettings();
				awaiter2 = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.ConnectedToMaster
				}).GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__2 = awaiter2;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<JoinRandomPublicRoom>d__53>(ref awaiter2, ref this);
					return;
				}
				IL_172:
				if (!awaiter2.GetResult())
				{
					result = NetJoinResult.Failed_Other;
					goto IL_35D;
				}
				networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Searching_Joining;
				if (this.opts.IsJoiningWithFriends)
				{
					PhotonNetwork.JoinRandomRoom(this.opts.customProps, this.opts.MaxPlayers, MatchmakingMode.RandomMatching, null, null, this.opts.joinFriendIDs.ToArray<string>());
				}
				else
				{
					PhotonNetwork.JoinRandomRoom(this.opts.customProps, this.opts.MaxPlayers, MatchmakingMode.FillRoom, null, null, null);
				}
				awaiter2 = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Joined,
					NetworkSystemPUN.InternalState.Searching_JoinFailed
				}).GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					this.<>1__state = 2;
					this.<>u__2 = awaiter2;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<JoinRandomPublicRoom>d__53>(ref awaiter2, ref this);
					return;
				}
				IL_24F:
				if (!awaiter2.GetResult())
				{
					result = NetJoinResult.Failed_Other;
					goto IL_35D;
				}
				if (networkSystemPUN.internalState != NetworkSystemPUN.InternalState.Searching_JoinFailed)
				{
					result = NetJoinResult.Success;
					goto IL_35D;
				}
				networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
				if (this.opts.IsJoiningWithFriends)
				{
					PhotonNetwork.CreateRoom(NetworkSystem.GetRandomRoomName(), this.opts.ToPUNOpts(), null, this.opts.joinFriendIDs);
				}
				else
				{
					PhotonNetwork.CreateRoom(NetworkSystem.GetRandomRoomName(), this.opts.ToPUNOpts(), null, null);
				}
				awaiter2 = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Created,
					NetworkSystemPUN.InternalState.Searching_CreateFailed
				}).GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					this.<>1__state = 3;
					this.<>u__2 = awaiter2;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<JoinRandomPublicRoom>d__53>(ref awaiter2, ref this);
					return;
				}
				IL_321:
				if (!awaiter2.GetResult())
				{
					result = NetJoinResult.Failed_Other;
				}
				else if (networkSystemPUN.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
				{
					result = NetJoinResult.Failed_Other;
				}
				else
				{
					result = NetJoinResult.FallbackCreated;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_35D:
			this.<>1__state = -2;
			this.<>t__builder.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<NetJoinResult> <>t__builder;

		public NetworkSystemPUN <>4__this;

		public RoomConfig opts;

		private TaskAwaiter <>u__1;

		private TaskAwaiter<bool> <>u__2;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <MakeOrFindRoom>d__49 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			NetJoinResult result2;
			try
			{
				TaskAwaiter awaiter;
				TaskAwaiter<bool> awaiter2;
				TaskAwaiter<NetJoinResult> awaiter3;
				switch (num)
				{
				case 0:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
					break;
				case 1:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_F7;
				case 2:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_165;
				case 3:
					awaiter3 = this.<>u__3;
					this.<>u__3 = default(TaskAwaiter<NetJoinResult>);
					this.<>1__state = -1;
					goto IL_1E5;
				default:
					if (!networkSystemPUN.InRoom)
					{
						goto IL_8A;
					}
					awaiter = networkSystemPUN.InternalDisconnect().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemPUN.<MakeOrFindRoom>d__49>(ref awaiter, ref this);
						return;
					}
					break;
				}
				awaiter.GetResult();
				IL_8A:
				networkSystemPUN.currentRegionIndex = 0;
				if (this.regionIndex < 0)
				{
					awaiter2 = networkSystemPUN.TryJoinRoom(this.roomName).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						this.<>1__state = 1;
						this.<>u__2 = awaiter2;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<MakeOrFindRoom>d__49>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = networkSystemPUN.TryJoinRoomInRegion(this.roomName, this.regionIndex).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						this.<>1__state = 2;
						this.<>u__2 = awaiter2;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<MakeOrFindRoom>d__49>(ref awaiter2, ref this);
						return;
					}
					goto IL_165;
				}
				IL_F7:
				bool result = awaiter2.GetResult();
				goto IL_16E;
				IL_165:
				result = awaiter2.GetResult();
				IL_16E:
				bool flag = result;
				if (networkSystemPUN.internalState == NetworkSystemPUN.InternalState.Searching_JoinFailed_Full)
				{
					result2 = NetJoinResult.Failed_Full;
					goto IL_20C;
				}
				if (flag)
				{
					result2 = NetJoinResult.Success;
					goto IL_20C;
				}
				awaiter3 = networkSystemPUN.TryCreateRoom(this.roomName, this.opts).GetAwaiter();
				if (!awaiter3.IsCompleted)
				{
					this.<>1__state = 3;
					this.<>u__3 = awaiter3;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, NetworkSystemPUN.<MakeOrFindRoom>d__49>(ref awaiter3, ref this);
					return;
				}
				IL_1E5:
				result2 = awaiter3.GetResult();
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_20C:
			this.<>1__state = -2;
			this.<>t__builder.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<NetJoinResult> <>t__builder;

		public NetworkSystemPUN <>4__this;

		public int regionIndex;

		public string roomName;

		public RoomConfig opts;

		private TaskAwaiter <>u__1;

		private TaskAwaiter<bool> <>u__2;

		private TaskAwaiter<NetJoinResult> <>u__3;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <OnDisconnected>d__101 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					awaiter = networkSystemPUN.RefreshOculusNonce().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemPUN.<OnDisconnected>d__101>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
				}
				awaiter.GetResult();
				if (networkSystemPUN.internalState == NetworkSystemPUN.InternalState.Searching_Disconnecting)
				{
					networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Searching_Disconnected;
				}
				else if (networkSystemPUN.internalState == NetworkSystemPUN.InternalState.PingGathering)
				{
					networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Internal_Disconnected;
				}
				else if (networkSystemPUN.internalState == NetworkSystemPUN.InternalState.Internal_Disconnecting)
				{
					networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Internal_Disconnected;
				}
				else
				{
					networkSystemPUN.UpdatePlayerIDCache();
					networkSystemPUN.UpdateNetPlayerList();
					networkSystemPUN.SinglePlayerStarted();
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public NetworkSystemPUN <>4__this;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <ReturnToSinglePlayer>d__58 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (networkSystemPUN.netState != NetSystemState.InGame)
					{
						goto IL_D3;
					}
					networkSystemPUN.netState = NetSystemState.Disconnecting;
					networkSystemPUN._taskCancelTokens.ForEach(new Action<CancellationTokenSource>(NetworkSystemPUN.<>c.<>9.<ReturnToSinglePlayer>b__58_0));
					networkSystemPUN._taskCancelTokens.Clear();
					awaiter = networkSystemPUN.InternalDisconnect().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemPUN.<ReturnToSinglePlayer>d__58>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
				}
				awaiter.GetResult();
				networkSystemPUN.netState = NetSystemState.Idle;
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_D3:
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public NetworkSystemPUN <>4__this;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <TryCreateRoom>d__52 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			NetJoinResult result;
			try
			{
				TaskAwaiter<bool> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = this.<>u__1;
						this.<>u__1 = default(TaskAwaiter<bool>);
						this.<>1__state = -1;
						goto IL_13D;
					}
					networkSystemPUN.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = networkSystemPUN.regionNames[networkSystemPUN.lowestPingRegionIndex];
					networkSystemPUN.currentRegionIndex = networkSystemPUN.lowestPingRegionIndex;
					PhotonNetwork.ConnectUsingSettings();
					awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
					{
						NetworkSystemPUN.InternalState.ConnectedToMaster
					}).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<TryCreateRoom>d__52>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
				}
				if (!awaiter.GetResult())
				{
					result = NetJoinResult.Failed_Other;
					goto IL_175;
				}
				networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
				PhotonNetwork.CreateRoom(this.roomName, this.opts.ToPUNOpts(), null, null);
				awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Created,
					NetworkSystemPUN.InternalState.Searching_CreateFailed
				}).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<TryCreateRoom>d__52>(ref awaiter, ref this);
					return;
				}
				IL_13D:
				if (!awaiter.GetResult())
				{
					result = NetJoinResult.Failed_Other;
				}
				else if (networkSystemPUN.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
				{
					result = NetJoinResult.Failed_Other;
				}
				else
				{
					result = NetJoinResult.FallbackCreated;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_175:
			this.<>1__state = -2;
			this.<>t__builder.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<NetJoinResult> <>t__builder;

		public NetworkSystemPUN <>4__this;

		public string roomName;

		public RoomConfig opts;

		private TaskAwaiter<bool> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <TryJoinRoom>d__50 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			bool result;
			try
			{
				if (num != 0)
				{
					goto IL_8B;
				}
				TaskAwaiter<bool> awaiter = this.<>u__1;
				this.<>u__1 = default(TaskAwaiter<bool>);
				this.<>1__state = -1;
				IL_70:
				if (awaiter.GetResult())
				{
					result = true;
					goto IL_BB;
				}
				networkSystemPUN.currentRegionIndex++;
				IL_8B:
				if (networkSystemPUN.currentRegionIndex >= networkSystemPUN.regionNames.Length)
				{
					result = false;
				}
				else
				{
					awaiter = networkSystemPUN.TryJoinRoomInRegion(this.roomName, networkSystemPUN.currentRegionIndex).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<TryJoinRoom>d__50>(ref awaiter, ref this);
						return;
					}
					goto IL_70;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_BB:
			this.<>1__state = -2;
			this.<>t__builder.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<bool> <>t__builder;

		public NetworkSystemPUN <>4__this;

		public string roomName;

		private TaskAwaiter<bool> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <TryJoinRoomInRegion>d__51 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			bool result;
			try
			{
				TaskAwaiter<bool> awaiter;
				switch (num)
				{
				case 0:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					break;
				case 1:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_145;
				case 2:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_1D9;
				default:
				{
					networkSystemPUN.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
					string fixedRegion = networkSystemPUN.regionNames[this.regionIndex];
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = fixedRegion;
					networkSystemPUN.currentRegionIndex = this.regionIndex;
					PhotonNetwork.ConnectUsingSettings();
					awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
					{
						NetworkSystemPUN.InternalState.ConnectedToMaster
					}).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<TryJoinRoomInRegion>d__51>(ref awaiter, ref this);
						return;
					}
					break;
				}
				}
				if (!awaiter.GetResult())
				{
					result = false;
					goto IL_208;
				}
				networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Searching_Joining;
				PhotonNetwork.JoinRoom(this.roomName, null);
				awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Joined,
					NetworkSystemPUN.InternalState.Searching_JoinFailed,
					NetworkSystemPUN.InternalState.Searching_JoinFailed_Full
				}).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<TryJoinRoomInRegion>d__51>(ref awaiter, ref this);
					return;
				}
				IL_145:
				if (!awaiter.GetResult())
				{
					result = false;
					goto IL_208;
				}
				this.<foundRoom>5__2 = (networkSystemPUN.internalState == NetworkSystemPUN.InternalState.Searching_Joined);
				if (this.<foundRoom>5__2)
				{
					goto IL_1E6;
				}
				PhotonNetwork.Disconnect();
				networkSystemPUN.internalState = NetworkSystemPUN.InternalState.Searching_Disconnecting;
				awaiter = networkSystemPUN.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Disconnected
				}).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 2;
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemPUN.<TryJoinRoomInRegion>d__51>(ref awaiter, ref this);
					return;
				}
				IL_1D9:
				if (!awaiter.GetResult())
				{
					result = false;
					goto IL_208;
				}
				IL_1E6:
				result = this.<foundRoom>5__2;
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_208:
			this.<>1__state = -2;
			this.<>t__builder.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<bool> <>t__builder;

		public NetworkSystemPUN <>4__this;

		public int regionIndex;

		public string roomName;

		private bool <foundRoom>5__2;

		private TaskAwaiter<bool> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <WaitForState>d__47 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			try
			{
				if (num != 0)
				{
					this.<timeoutTime>5__2 = Time.time + 10f;
					goto IL_FB;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_F4:
				awaiter.GetResult();
				IL_FB:
				if (!this.desiredStates.Contains(networkSystemPUN.internalState) && !this.ct.IsCancellationRequested)
				{
					if (this.<timeoutTime>5__2 < Time.time)
					{
						string text = "";
						foreach (NetworkSystemPUN.InternalState internalState in this.desiredStates)
						{
							text += string.Format("- {0}", internalState);
						}
						Debug.LogError("Got stuck waiting for states " + text);
						networkSystemPUN.internalState = NetworkSystemPUN.InternalState.StateCheckFailed;
					}
					else
					{
						awaiter = Task.Yield().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							this.<>1__state = 0;
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemPUN.<WaitForState>d__47>(ref awaiter, ref this);
							return;
						}
						goto IL_F4;
					}
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public CancellationToken ct;

		public NetworkSystemPUN.InternalState[] desiredStates;

		public NetworkSystemPUN <>4__this;

		private float <timeoutTime>5__2;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <WaitForStateCheck>d__48 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemPUN networkSystemPUN = this.<>4__this;
			bool result;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					this.<token>5__2 = networkSystemPUN.GetCancellationToken();
					awaiter = networkSystemPUN.WaitForState(this.<token>5__2.Item2, this.desiredStates).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemPUN.<WaitForStateCheck>d__48>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
				}
				awaiter.GetResult();
				networkSystemPUN._taskCancelTokens.Remove(this.<token>5__2.Item1);
				if (networkSystemPUN.internalState != NetworkSystemPUN.InternalState.StateCheckFailed)
				{
					result = true;
				}
				else
				{
					networkSystemPUN.ResetSystem();
					result = false;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<token>5__2 = default(ValueTuple<CancellationTokenSource, CancellationToken>);
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<token>5__2 = default(ValueTuple<CancellationTokenSource, CancellationToken>);
			this.<>t__builder.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<bool> <>t__builder;

		public NetworkSystemPUN <>4__this;

		public NetworkSystemPUN.InternalState[] desiredStates;

		private ValueTuple<CancellationTokenSource, CancellationToken> <token>5__2;

		private TaskAwaiter <>u__1;
	}
}
