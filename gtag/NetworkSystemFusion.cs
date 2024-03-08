using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using GorillaNetworking;
using GorillaTag.Audio;
using Photon.Realtime;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSystemFusion : NetworkSystem
{
	public NetworkRunner runner { get; private set; }

	public override bool IsOnline
	{
		get
		{
			return this.runner != null && !this.runner.IsSinglePlayer;
		}
	}

	public override bool InRoom
	{
		get
		{
			return this.runner != null && this.runner.State != NetworkRunner.States.Shutdown && !this.runner.IsSinglePlayer && this.runner.IsConnectedToServer;
		}
	}

	public override string RoomName
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Name;
		}
	}

	public override string GameModeString
	{
		get
		{
			SessionProperty sessionProperty;
			this.runner.SessionInfo.Properties.TryGetValue("gameMode", out sessionProperty);
			if (sessionProperty != null)
			{
				return sessionProperty.ToString();
			}
			return null;
		}
	}

	public override string CurrentRegion
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Region;
		}
	}

	public override bool SessionIsPrivate
	{
		get
		{
			return !this.runner.SessionInfo.IsVisible;
		}
	}

	public override int LocalPlayerID
	{
		get
		{
			return this.runner.LocalPlayer.PlayerId;
		}
	}

	public override int MasterAuthID
	{
		get
		{
			NetworkRunner runner = this.runner;
			int? num;
			if (runner == null)
			{
				num = null;
			}
			else
			{
				NetworkProjectConfig config = runner.Config;
				int? num2;
				if (config == null)
				{
					num2 = null;
				}
				else
				{
					SimulationConfig simulation = config.Simulation;
					num2 = ((simulation != null) ? new int?(simulation.DefaultPlayers) : null);
				}
				num = num2 - 1;
			}
			int? num3 = num;
			if (num3 == null)
			{
				return -1;
			}
			return num3.GetValueOrDefault();
		}
	}

	public override string CurrentPhotonBackend
	{
		get
		{
			return "Fusion";
		}
	}

	public override int[] AllPlayerIDs
	{
		get
		{
			return this.cachedPlayerIDs;
		}
	}

	public override float SimTime
	{
		get
		{
			return this.runner.SimulationTime;
		}
	}

	public override float SimDeltaTime
	{
		get
		{
			return this.runner.DeltaTime;
		}
	}

	public override int SimTick
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	public override int RoomPlayerCount
	{
		get
		{
			return this.runner.SessionInfo.PlayerCount;
		}
	}

	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.FusionVoice;
		}
	}

	public override async void Initialise()
	{
		base.Initialise();
		base.netState = NetSystemState.Initialization;
		this.internalState = NetworkSystemFusion.InternalState.Idle;
		await this.ReturnToSinglePlayer();
		Debug.Log("After return to single player task.");
		this.AwaitAuth();
		Debug.Log("Creating region crawler");
		this.CreateRegionCrawler();
		base.netState = NetSystemState.Idle;
	}

	private void CreateRegionCrawler()
	{
		GameObject gameObject = new GameObject("[Network Crawler]");
		gameObject.transform.SetParent(base.transform);
		this.regionCrawler = gameObject.AddComponent<FusionRegionCrawler>();
	}

	private async Task AwaitAuth()
	{
		this.internalState = NetworkSystemFusion.InternalState.AwaitingAuth;
		while (this.cachedPlayfabAuth == null)
		{
			await Task.Yield();
		}
		this.internalState = NetworkSystemFusion.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	public override void SetAuthenticationValues(Dictionary<string, string> authValues)
	{
		this.cachedPlayfabAuth = authValues.ToAuthValues_F();
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
			Debug.Log("Connecting to:" + (string.IsNullOrEmpty(roomName) ? "random room" : roomName));
			NetJoinResult netJoinResult2;
			if (!string.IsNullOrEmpty(roomName))
			{
				Task<NetJoinResult> makeOrJoinTask = this.MakeOrJoinRoom(roomName, opts);
				await makeOrJoinTask;
				netJoinResult2 = makeOrJoinTask.Result;
				makeOrJoinTask = null;
			}
			else
			{
				Task<NetJoinResult> makeOrJoinTask = this.JoinRandomPublicRoom(opts);
				await makeOrJoinTask;
				netJoinResult2 = makeOrJoinTask.Result;
				makeOrJoinTask = null;
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
				this.UpdatePlayerIDCache();
				this.UpdateNetPlayerList();
				base.netState = NetSystemState.InGame;
				Debug.Log("Connect to room result: " + netJoinResult2.ToString());
				netJoinResult = netJoinResult2;
			}
		}
		return netJoinResult;
	}

	private async Task<bool> Connect(GameMode mode, string targetSessionName, RoomConfig opts)
	{
		if (this.runner != null)
		{
			bool goingBetweenRooms = this.InRoom && mode != GameMode.Single;
			await this.CloseRunner();
			await Task.Yield();
			if (goingBetweenRooms)
			{
				base.SinglePlayerStarted();
				await Task.Yield();
			}
		}
		if (this.volatileNetObj)
		{
			Debug.LogError("Volatile net obj should not exist - destroying and recreating");
			Object.Destroy(this.volatileNetObj);
		}
		this.volatileNetObj = new GameObject("VolatileFusionObj");
		this.volatileNetObj.transform.parent = base.transform;
		this.runner = this.volatileNetObj.AddComponent<NetworkRunner>();
		this.internalRPCProvider = this.volatileNetObj.AddComponent<FusionInternalRPCs>();
		this.callbackHandler = this.volatileNetObj.AddComponent<FusionCallbackHandler>();
		this.callbackHandler.Setup(this);
		if (mode != GameMode.Single)
		{
			SceneManager.GetActiveScene();
		}
		this.lastConnectAttempt_WasFull = false;
		this.lastConnectAttempt_WasMissing = false;
		this.internalState = NetworkSystemFusion.InternalState.ConnectingToRoom;
		Hashtable customProps = opts.customProps;
		Dictionary<string, SessionProperty> dictionary = ((customProps != null) ? customProps.ToPropDict() : null);
		Task<StartGameResult> startupTask = this.runner.StartGame(new StartGameArgs
		{
			IsVisible = new bool?(opts.isPublic),
			IsOpen = new bool?(opts.isJoinable),
			GameMode = mode,
			SessionName = targetSessionName,
			PlayerCount = new int?((int)opts.MaxPlayers),
			SceneManager = this.volatileNetObj.AddComponent<NetworkSceneManagerDefault>(),
			SessionProperties = dictionary,
			DisableClientSessionCreation = !opts.createIfMissing
		});
		await startupTask;
		Debug.Log("Startuptask finished : " + startupTask.Result.ToString());
		bool flag;
		if (!startupTask.Result.Ok)
		{
			base.CurrentRoom = null;
			flag = startupTask.Result.Ok;
		}
		else
		{
			this.AddVoice();
			base.CurrentRoom = opts;
			if (this.IsTotalAuthority() || this.runner.IsSharedModeMasterClient)
			{
				opts.SetFusionOpts(this.runner);
				this.runner.SetActiveScene(SceneManager.GetActiveScene().name);
				while (!this.runner.SceneManager.IsReady(this.runner))
				{
					await Task.Yield();
				}
			}
			this.SetMyNickName(GorillaComputer.instance.savedName);
			this.runner.AddSimulationBehaviour(this.internalRPCProvider, null);
			flag = startupTask.Result.Ok;
		}
		return flag;
	}

	private async Task<NetJoinResult> MakeOrJoinRoom(string roomName, RoomConfig opts)
	{
		int currentRegionIndex = 0;
		bool flag = false;
		opts.createIfMissing = false;
		Task<bool> connectTask;
		while (currentRegionIndex < this.regionNames.Length && !flag)
		{
			try
			{
				PhotonAppSettings.Instance.AppSettings.FixedRegion = this.regionNames[currentRegionIndex];
				this.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
				connectTask = this.Connect(GameMode.Shared, roomName, opts);
				await connectTask;
				flag = connectTask.Result;
				if (!flag)
				{
					if (this.lastConnectAttempt_WasFull)
					{
						Debug.Log("Found room but it was full");
						break;
					}
					Debug.Log("Region incrimenting");
					currentRegionIndex++;
				}
				connectTask = null;
			}
			catch (Exception ex)
			{
				Debug.LogError("MakeOrJoinRoom - message: " + ex.Message + "\nStacktrace : " + ex.StackTrace);
				return NetJoinResult.Failed_Other;
			}
		}
		if (this.lastConnectAttempt_WasFull)
		{
			PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
			return NetJoinResult.Failed_Full;
		}
		if (flag)
		{
			return NetJoinResult.Success;
		}
		PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
		opts.createIfMissing = true;
		connectTask = this.Connect(GameMode.Shared, roomName, opts);
		await connectTask;
		Debug.Log("made room?");
		if (!connectTask.Result)
		{
			Debug.LogError("NS-FUS] Failed to create private room");
			return NetJoinResult.Failed_Other;
		}
		while (!this.runner.SessionInfo.IsValid)
		{
			await Task.Yield();
		}
		return NetJoinResult.FallbackCreated;
	}

	private async Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		bool shouldCreateIfNone = opts.createIfMissing;
		PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
		this.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
		opts.createIfMissing = false;
		Task<bool> connectTask = this.Connect(GameMode.Shared, null, opts);
		await connectTask;
		NetJoinResult netJoinResult;
		if (!connectTask.Result && shouldCreateIfNone)
		{
			opts.createIfMissing = shouldCreateIfNone;
			Task<bool> createTask = this.Connect(GameMode.Shared, NetworkSystem.GetRandomRoomName(), opts);
			await createTask;
			if (!createTask.Result)
			{
				Debug.LogError("NS-FUS] Failed to create public room");
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else
			{
				opts.SetFusionOpts(this.runner);
				netJoinResult = NetJoinResult.FallbackCreated;
			}
		}
		else
		{
			netJoinResult = NetJoinResult.Success;
		}
		return netJoinResult;
	}

	public override Task JoinFriendsRoom(string userID, int actorID, string keyToFollow, string shufflerToFollow)
	{
		throw new NotImplementedException();
	}

	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	public override async Task ReturnToSinglePlayer()
	{
		if (base.netState == NetSystemState.InGame || base.netState == NetSystemState.Initialization)
		{
			base.netState = NetSystemState.Disconnecting;
			Debug.Log("Returning to single player");
			if (this.runner)
			{
				await this.CloseRunner();
				await Task.Yield();
				Debug.Log("Connect in return to single player");
			}
			base.netState = NetSystemState.Idle;
			this.internalState = NetworkSystemFusion.InternalState.Idle;
			base.SinglePlayerStarted();
		}
	}

	private async Task CloseRunner()
	{
		this.internalState = NetworkSystemFusion.InternalState.Disconnecting;
		try
		{
			await this.runner.Shutdown(true, ShutdownReason.Ok, false);
		}
		catch (Exception ex)
		{
			StackFrame frame = new StackTrace(ex, true).GetFrame(0);
			int fileLineNumber = frame.GetFileLineNumber();
			Debug.LogError(string.Concat(new string[]
			{
				ex.Message,
				" File:",
				frame.GetFileName(),
				" line: ",
				fileLineNumber.ToString()
			}));
		}
		Object.Destroy(this.volatileNetObj);
		this.internalState = NetworkSystemFusion.InternalState.Disconnected;
	}

	public void MigrateHost(HostMigrationToken hostMigrationToken)
	{
		Debug.Log("HOSTTEST : MigrateHostTriggered, returning to single player!");
		this.ReturnToSinglePlayer();
	}

	public async void ResetSystem()
	{
		await this.Connect(GameMode.Single, "--", RoomConfig.SPConfig());
		Debug.Log("Connect in return to single player");
		base.netState = NetSystemState.Idle;
		this.internalState = NetworkSystemFusion.InternalState.Idle;
	}

	private void AddVoice()
	{
		this.SetupVoice();
		this.FusionVoiceBridge = this.volatileNetObj.AddComponent<FusionVoiceBridge>();
	}

	private void SetupVoice()
	{
		this.FusionVoice = this.volatileNetObj.AddComponent<VoiceConnection>();
		this.FusionVoice.LogLevel = this.VoiceSettings.LogLevel;
		this.FusionVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
		this.FusionVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
		this.FusionVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
		Photon.Realtime.AppSettings appSettings = new Photon.Realtime.AppSettings();
		appSettings.AppIdFusion = PhotonAppSettings.Instance.AppSettings.AppIdFusion;
		appSettings.AppIdVoice = PhotonAppSettings.Instance.AppSettings.AppIdVoice;
		this.FusionVoice.Settings = appSettings;
		this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
		{
			this.FusionVoice.RemoteVoiceAdded += callback;
		});
		this.localRecorder = this.volatileNetObj.AddComponent<Recorder>();
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
		this.FusionVoice.PrimaryRecorder = this.localRecorder;
		this.volatileNetObj.AddComponent<VoiceToLoudness>();
	}

	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		Debug.Log("Net instantiate Fusion: " + prefab.name);
		return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), null, null, true).gameObject;
	}

	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (playerRef.PlayerId == playerAuthID)
			{
				return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(playerRef), null, null, true).gameObject;
			}
		}
		Debug.LogError(string.Format("Couldn't find player with ID: {0}, cancelling requested spawn...", playerAuthID));
		return null;
	}

	public override void NetDestroy(GameObject instance)
	{
		NetworkObject networkObject;
		if (instance.TryGetComponent<NetworkObject>(out networkObject))
		{
			this.runner.Despawn(networkObject, false);
			return;
		}
		Object.Destroy(instance);
	}

	public override bool ShouldSpawnLocally(int playerID)
	{
		if (this.runner.GameMode == GameMode.Shared)
		{
			return this.runner.LocalPlayer.PlayerId == playerID || (playerID == -1 && this.runner.IsSharedModeMasterClient);
		}
		return this.runner.GameMode != GameMode.Client;
	}

	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		(ref args).SerializeToRPCData<T>();
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		this.GetPlayerRef(targetPlayerID);
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
	}

	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		this.GetPlayerRef(targetPlayerID);
	}

	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		this.GetPlayerRef(targetPlayerID);
	}

	public override string GetRandomWeightedRegion()
	{
		throw new NotImplementedException();
	}

	public override async Task AwaitSceneReady()
	{
		while (!this.runner.SceneManager.IsReady(this.runner))
		{
			await Task.Yield();
		}
		for (float counter = 0f; counter < 0.5f; counter += Time.deltaTime)
		{
			await Task.Yield();
		}
	}

	public void OnJoinedSession()
	{
	}

	public void OnJoinFailed(NetConnectFailedReason reason)
	{
		switch (reason)
		{
		case NetConnectFailedReason.Timeout:
			this.lastConnectAttempt_WasMissing = true;
			return;
		case NetConnectFailedReason.ServerFull:
			this.lastConnectAttempt_WasFull = true;
			break;
		case NetConnectFailedReason.ServerRefused:
			break;
		default:
			return;
		}
	}

	public void OnDisconnectedFromSession()
	{
		Debug.Log("On Disconnected");
		this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		this.UpdatePlayerIDCache();
		this.UpdateNetPlayerList();
	}

	public void OnRunnerShutDown()
	{
		Debug.Log("Runner shutdown callback");
		if (this.internalState == NetworkSystemFusion.InternalState.Disconnecting)
		{
			this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		}
	}

	public void OnFusionPlayerJoined(PlayerRef player)
	{
		this.AwaitJoiningPlayerClientReady(player);
	}

	private async Task AwaitJoiningPlayerClientReady(PlayerRef player)
	{
		Debug.Log("Player: " + player.PlayerId.ToString() + "is joining!");
		this.UpdatePlayerIDCache();
		this.UpdateNetPlayerList();
		if (this.runner != null && player == this.runner.LocalPlayer && !this.runner.IsSinglePlayer)
		{
			Debug.Log("Multiplayer started");
			base.MultiplayerStarted();
		}
		if (this.runner != null && player == this.runner.LocalPlayer && this.runner.IsSinglePlayer)
		{
			Debug.Log("Singleplayer started");
			base.SinglePlayerStarted();
		}
		Debug.Log("Player " + player.PlayerId.ToString() + " properties sorted!");
		base.PlayerJoined(player.PlayerId);
	}

	public void OnFusionPlayerLeft(PlayerRef player)
	{
		if (this.IsTotalAuthority())
		{
			NetworkObject playerObject = this.runner.GetPlayerObject(player);
			if (playerObject != null)
			{
				Debug.Log("Destroying player object for leaving player!");
				this.NetDestroy(playerObject.gameObject);
			}
			else
			{
				Debug.Log("Player left without destroying an avatar for it somehow?");
			}
		}
		base.PlayerLeft(player.PlayerId);
		this.UpdatePlayerIDCache();
		this.UpdateNetPlayerList();
	}

	protected override void UpdateNetPlayerList()
	{
		if (this.runner == null)
		{
			Debug.LogError("Runner is null, trying to update list and cant");
		}
		NetPlayer[] array;
		if (this.runner == null || this.runner.IsSinglePlayer)
		{
			bool flag = false;
			array = this.netPlayerCache.ToArray();
			if (this.netPlayerCache.Count > 0)
			{
				foreach (NetPlayer netPlayer in array)
				{
					if (((FusionNetPlayer)netPlayer).playerRef == this.runner.LocalPlayer)
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
				this.netPlayerCache.Add(new FusionNetPlayer(this.runner.LocalPlayer));
			}
			return;
		}
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			bool flag2 = false;
			for (int j = 0; j < this.netPlayerCache.Count; j++)
			{
				if (playerRef == ((FusionNetPlayer)this.netPlayerCache[j]).playerRef)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				this.netPlayerCache.Add(new FusionNetPlayer(playerRef));
			}
		}
		array = this.netPlayerCache.ToArray();
		foreach (NetPlayer netPlayer2 in array)
		{
			bool flag3 = false;
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == ((FusionNetPlayer)netPlayer2).playerRef)
					{
						flag3 = true;
					}
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
		if (this.runner == null || this.runner.GameMode == GameMode.Single)
		{
			this.cachedPlayerIDs = new int[1];
			return;
		}
		this.cachedPlayerIDs = new int[this.runner.ActivePlayers.Count<PlayerRef>()];
		int num = 0;
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			this.cachedPlayerIDs[num] = playerRef.PlayerId;
			num++;
		}
	}

	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
		PlayerRef playerRef = this.runner.LocalPlayer;
		if (owningPlayerID != null)
		{
			playerRef = this.GetPlayerRef(owningPlayerID.Value);
		}
		this.runner.SetPlayerObject(playerRef, playerInstance.GetComponent<NetworkObject>());
	}

	private PlayerRef GetPlayerRef(int playerID)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (playerRef.PlayerId == playerID)
			{
				return playerRef;
			}
		}
		Debug.LogError(string.Format("Couldn't find player with ID #{0}", playerID));
		return default(PlayerRef);
	}

	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
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
		Debug.LogError("Somehow there is no local NetPlayer. This shoulnd't happen.");
		return null;
	}

	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (PlayerID == -1)
		{
			Debug.LogError("Attempting to get NetPlayer for local -1 ID.");
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ID == PlayerID)
			{
				return netPlayer;
			}
		}
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
		{
			this.UpdatePlayerIDCache();
			this.UpdateNetPlayerList();
			foreach (NetPlayer netPlayer2 in this.netPlayerCache)
			{
				if (netPlayer2.ID == PlayerID)
				{
					return netPlayer2;
				}
			}
		}
		Debug.LogError("Failed to find the player, before and after resyncing the player cache, this probably shoulnd't happen...");
		return null;
	}

	public override void SetMyNickName(string name)
	{
		PlayerPrefs.SetString("playerName", name);
		if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
		{
			GorillaTagger.Instance.rigSerializer.nickName = name;
		}
	}

	public override string GetMyNickName()
	{
		return PlayerPrefs.GetString("playerName");
	}

	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			Debug.LogError("Cant get nick name as playerID doesnt have a NetPlayer...");
			return null;
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		return rigContainer.Rig.rigSerializer.nickName.Value ?? "";
	}

	public override string GetMyUserID()
	{
		return this.runner.GetPlayerUserId(this.runner.LocalPlayer);
	}

	public override string GetUserID(int playerID)
	{
		return this.runner.GetPlayerUserId(this.GetPlayerRef(playerID));
	}

	public override void SetMyTutorialComplete()
	{
		if (!(PlayerPrefs.GetString("didTutorial", "nope") == "done"))
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
	}

	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			Debug.LogError("Player not found");
			return false;
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer == null)
		{
			Debug.LogError("VRRig not found for player");
			return false;
		}
		return rigContainer.Rig.rigSerializer.tutorialComplete;
	}

	public override int GlobalPlayerCount()
	{
		if (this.regionCrawler == null)
		{
			return 0;
		}
		return this.regionCrawler.PlayerCountGlobal;
	}

	public override int GetOwningPlayerID(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return -1;
		}
		if (this.runner.GameMode == GameMode.Shared)
		{
			return networkObject.StateAuthority.PlayerId;
		}
		return networkObject.InputAuthority.PlayerId;
	}

	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return false;
		}
		if (this.runner.GameMode == GameMode.Shared)
		{
			return networkObject.StateAuthority == this.runner.LocalPlayer;
		}
		return networkObject.InputAuthority == this.runner.LocalPlayer;
	}

	public override bool IsTotalAuthority()
	{
		return this.runner.Mode == SimulationModes.Server || this.runner.Mode == SimulationModes.Host || this.runner.GameMode == GameMode.Single || this.runner.IsSharedModeMasterClient;
	}

	public override bool ShouldWriteObjectData(GameObject obj)
	{
		NetworkObject networkObject;
		return obj.TryGetComponent<NetworkObject>(out networkObject) && networkObject.HasStateAuthority;
	}

	public override bool ShouldUpdateObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return true;
		}
		if (this.IsTotalAuthority())
		{
			return true;
		}
		if (networkObject.InputAuthority.IsValid && !networkObject.InputAuthority.IsNone)
		{
			return networkObject.InputAuthority == this.runner.LocalPlayer;
		}
		return this.runner.IsSharedModeMasterClient;
	}

	public override bool IsObjectRoomObject(GameObject obj)
	{
		NetworkObject networkObject;
		return obj.TryGetComponent<NetworkObject>(out networkObject) && networkObject.IsSceneObject;
	}

	private NetworkSystemFusion.InternalState internalState;

	private FusionInternalRPCs internalRPCProvider;

	private FusionCallbackHandler callbackHandler;

	private FusionRegionCrawler regionCrawler;

	private GameObject volatileNetObj;

	private Fusion.Photon.Realtime.AuthenticationValues cachedPlayfabAuth;

	private const string playerPropertiesPath = "P_FusionProperties";

	private bool lastConnectAttempt_WasMissing;

	private bool lastConnectAttempt_WasFull;

	private int[] cachedPlayerIDs;

	private FusionVoiceBridge FusionVoiceBridge;

	private VoiceConnection FusionVoice;

	private enum InternalState
	{
		AwaitingAuth,
		Idle,
		Searching_Joining,
		Searching_Joined,
		Searching_JoinFailed,
		Searching_Disconnecting,
		Searching_Disconnected,
		ConnectingToRoom,
		ConnectedToRoom,
		JoinRoomFailed,
		Disconnecting,
		Disconnected,
		StateCheckFailed
	}
}
