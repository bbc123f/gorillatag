using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
	public NetworkRunner runner
	{
		[CompilerGenerated]
		get
		{
			return this.<runner>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<runner>k__BackingField = value;
		}
	}

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

	public override string GetMyDefaultName()
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
		if (PlayFabAuthenticator.instance.GetSafety())
		{
			return rigContainer.Rig.rigSerializer.defaultName.Value ?? "";
		}
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

	public NetworkSystemFusion()
	{
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void <>n__0()
	{
		base.Initialise();
	}

	[CompilerGenerated]
	private void <SetupVoice>b__62_0(Action<RemoteVoiceLink> callback)
	{
		this.FusionVoice.RemoteVoiceAdded += callback;
	}

	[CompilerGenerated]
	private NetworkRunner <runner>k__BackingField;

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

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <AwaitAuth>d__49 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			try
			{
				if (num != 0)
				{
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.AwaitingAuth;
					goto IL_74;
				}
				YieldAwaitable.YieldAwaiter yieldAwaiter2;
				YieldAwaitable.YieldAwaiter yieldAwaiter = yieldAwaiter2;
				yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
				num2 = -1;
				IL_6D:
				yieldAwaiter.GetResult();
				IL_74:
				if (networkSystemFusion.cachedPlayfabAuth != null)
				{
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Idle;
					networkSystemFusion.netState = NetSystemState.Idle;
				}
				else
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<AwaitAuth>d__49>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_6D;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public NetworkSystemFusion <>4__this;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <AwaitJoiningPlayerClientReady>d__81 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			NetworkSystemFusion networkSystemFusion = this;
			try
			{
				Debug.Log("Player: " + player.PlayerId.ToString() + "is joining!");
				networkSystemFusion.UpdatePlayerIDCache();
				networkSystemFusion.UpdateNetPlayerList();
				if (networkSystemFusion.runner != null && player == networkSystemFusion.runner.LocalPlayer && !networkSystemFusion.runner.IsSinglePlayer)
				{
					Debug.Log("Multiplayer started");
					networkSystemFusion.MultiplayerStarted();
				}
				if (networkSystemFusion.runner != null && player == networkSystemFusion.runner.LocalPlayer && networkSystemFusion.runner.IsSinglePlayer)
				{
					Debug.Log("Singleplayer started");
					networkSystemFusion.SinglePlayerStarted();
				}
				Debug.Log("Player " + player.PlayerId.ToString() + " properties sorted!");
				networkSystemFusion.PlayerJoined(player.PlayerId);
			}
			catch (Exception ex)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(ex);
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

		public PlayerRef player;

		public NetworkSystemFusion <>4__this;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <AwaitSceneReady>d__75 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			try
			{
				YieldAwaitable.YieldAwaiter yieldAwaiter;
				if (num != 0)
				{
					if (num != 1)
					{
						goto IL_77;
					}
					YieldAwaitable.YieldAwaiter yieldAwaiter2;
					yieldAwaiter = yieldAwaiter2;
					yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
					num2 = -1;
					goto IL_EF;
				}
				else
				{
					YieldAwaitable.YieldAwaiter yieldAwaiter2;
					yieldAwaiter = yieldAwaiter2;
					yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
					num2 = -1;
				}
				IL_70:
				yieldAwaiter.GetResult();
				IL_77:
				if (networkSystemFusion.runner.SceneManager.IsReady(networkSystemFusion.runner))
				{
					counter = 0f;
					goto IL_108;
				}
				yieldAwaiter = Task.Yield().GetAwaiter();
				if (!yieldAwaiter.IsCompleted)
				{
					num2 = 0;
					YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<AwaitSceneReady>d__75>(ref yieldAwaiter, ref this);
					return;
				}
				goto IL_70;
				IL_EF:
				yieldAwaiter.GetResult();
				counter += Time.deltaTime;
				IL_108:
				if (counter < 0.5f)
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 1;
						YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<AwaitSceneReady>d__75>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_EF;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public NetworkSystemFusion <>4__this;

		private float <counter>5__2;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <CloseRunner>d__58 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			try
			{
				if (num != 0)
				{
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Disconnecting;
				}
				try
				{
					TaskAwaiter taskAwaiter;
					if (num != 0)
					{
						taskAwaiter = networkSystemFusion.runner.Shutdown(true, ShutdownReason.Ok, false).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							num2 = 0;
							TaskAwaiter taskAwaiter2 = taskAwaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemFusion.<CloseRunner>d__58>(ref taskAwaiter, ref this);
							return;
						}
					}
					else
					{
						TaskAwaiter taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter);
						num2 = -1;
					}
					taskAwaiter.GetResult();
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
				Object.Destroy(networkSystemFusion.volatileNetObj);
				networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Disconnected;
			}
			catch (Exception ex2)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex2);
				return;
			}
			num2 = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public NetworkSystemFusion <>4__this;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <Connect>d__52 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			bool flag;
			try
			{
				TaskAwaiter taskAwaiter;
				YieldAwaitable.YieldAwaiter yieldAwaiter;
				TaskAwaiter<StartGameResult> taskAwaiter3;
				switch (num)
				{
				case 0:
				{
					TaskAwaiter taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter);
					num2 = -1;
					break;
				}
				case 1:
				{
					YieldAwaitable.YieldAwaiter yieldAwaiter2;
					yieldAwaiter = yieldAwaiter2;
					yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
					num2 = -1;
					goto IL_10E;
				}
				case 2:
				{
					YieldAwaitable.YieldAwaiter yieldAwaiter2;
					yieldAwaiter = yieldAwaiter2;
					yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
					num2 = -1;
					goto IL_17D;
				}
				case 3:
				{
					TaskAwaiter<StartGameResult> taskAwaiter4;
					taskAwaiter3 = taskAwaiter4;
					taskAwaiter4 = default(TaskAwaiter<StartGameResult>);
					num2 = -1;
					goto IL_347;
				}
				case 4:
				{
					YieldAwaitable.YieldAwaiter yieldAwaiter2;
					yieldAwaiter = yieldAwaiter2;
					yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
					num2 = -1;
					goto IL_44E;
				}
				default:
					if (!(networkSystemFusion.runner != null))
					{
						goto IL_184;
					}
					goingBetweenRooms = networkSystemFusion.InRoom && mode != GameMode.Single;
					taskAwaiter = networkSystemFusion.CloseRunner().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemFusion.<Connect>d__52>(ref taskAwaiter, ref this);
						return;
					}
					break;
				}
				taskAwaiter.GetResult();
				yieldAwaiter = Task.Yield().GetAwaiter();
				if (!yieldAwaiter.IsCompleted)
				{
					num2 = 1;
					YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<Connect>d__52>(ref yieldAwaiter, ref this);
					return;
				}
				IL_10E:
				yieldAwaiter.GetResult();
				if (!goingBetweenRooms)
				{
					goto IL_184;
				}
				networkSystemFusion.SinglePlayerStarted();
				yieldAwaiter = Task.Yield().GetAwaiter();
				if (!yieldAwaiter.IsCompleted)
				{
					num2 = 2;
					YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<Connect>d__52>(ref yieldAwaiter, ref this);
					return;
				}
				IL_17D:
				yieldAwaiter.GetResult();
				IL_184:
				if (networkSystemFusion.volatileNetObj)
				{
					Debug.LogError("Volatile net obj should not exist - destroying and recreating");
					Object.Destroy(networkSystemFusion.volatileNetObj);
				}
				networkSystemFusion.volatileNetObj = new GameObject("VolatileFusionObj");
				networkSystemFusion.volatileNetObj.transform.parent = networkSystemFusion.transform;
				networkSystemFusion.runner = networkSystemFusion.volatileNetObj.AddComponent<NetworkRunner>();
				networkSystemFusion.internalRPCProvider = networkSystemFusion.volatileNetObj.AddComponent<FusionInternalRPCs>();
				networkSystemFusion.callbackHandler = networkSystemFusion.volatileNetObj.AddComponent<FusionCallbackHandler>();
				networkSystemFusion.callbackHandler.Setup(networkSystemFusion);
				if (mode != GameMode.Single)
				{
					SceneManager.GetActiveScene();
				}
				networkSystemFusion.lastConnectAttempt_WasFull = false;
				networkSystemFusion.lastConnectAttempt_WasMissing = false;
				networkSystemFusion.internalState = NetworkSystemFusion.InternalState.ConnectingToRoom;
				Hashtable customProps = opts.customProps;
				Dictionary<string, SessionProperty> dictionary = ((customProps != null) ? customProps.ToPropDict() : null);
				startupTask = networkSystemFusion.runner.StartGame(new StartGameArgs
				{
					IsVisible = new bool?(opts.isPublic),
					IsOpen = new bool?(opts.isJoinable),
					GameMode = mode,
					SessionName = targetSessionName,
					PlayerCount = new int?((int)opts.MaxPlayers),
					SceneManager = networkSystemFusion.volatileNetObj.AddComponent<NetworkSceneManagerDefault>(),
					SessionProperties = dictionary,
					DisableClientSessionCreation = !opts.createIfMissing
				});
				taskAwaiter3 = startupTask.GetAwaiter();
				if (!taskAwaiter3.IsCompleted)
				{
					num2 = 3;
					TaskAwaiter<StartGameResult> taskAwaiter4 = taskAwaiter3;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<StartGameResult>, NetworkSystemFusion.<Connect>d__52>(ref taskAwaiter3, ref this);
					return;
				}
				IL_347:
				taskAwaiter3.GetResult();
				Debug.Log("Startuptask finished : " + startupTask.Result.ToString());
				if (!startupTask.Result.Ok)
				{
					networkSystemFusion.CurrentRoom = null;
					flag = startupTask.Result.Ok;
					goto IL_4C4;
				}
				networkSystemFusion.AddVoice();
				networkSystemFusion.CurrentRoom = opts;
				if (networkSystemFusion.IsTotalAuthority() || networkSystemFusion.runner.IsSharedModeMasterClient)
				{
					opts.SetFusionOpts(networkSystemFusion.runner);
					networkSystemFusion.runner.SetActiveScene(SceneManager.GetActiveScene().name);
					goto IL_455;
				}
				goto IL_46D;
				IL_44E:
				yieldAwaiter.GetResult();
				IL_455:
				if (!networkSystemFusion.runner.SceneManager.IsReady(networkSystemFusion.runner))
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 4;
						YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<Connect>d__52>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_44E;
				}
				IL_46D:
				networkSystemFusion.SetMyNickName(GorillaComputer.instance.savedName);
				networkSystemFusion.runner.AddSimulationBehaviour(networkSystemFusion.internalRPCProvider, null);
				flag = startupTask.Result.Ok;
			}
			catch (Exception ex)
			{
				num2 = -2;
				startupTask = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			IL_4C4:
			num2 = -2;
			startupTask = null;
			this.<>t__builder.SetResult(flag);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<bool> <>t__builder;

		public NetworkSystemFusion <>4__this;

		public GameMode mode;

		public RoomConfig opts;

		public string targetSessionName;

		private Task<StartGameResult> <startupTask>5__2;

		private bool <goingBetweenRooms>5__3;

		private TaskAwaiter <>u__1;

		private YieldAwaitable.YieldAwaiter <>u__2;

		private TaskAwaiter<StartGameResult> <>u__3;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <ConnectToRoom>d__51 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			NetJoinResult netJoinResult;
			try
			{
				TaskAwaiter<NetJoinResult> taskAwaiter;
				TaskAwaiter<NetJoinResult> taskAwaiter2;
				NetJoinResult netJoinResult2;
				if (num != 0)
				{
					if (num != 1)
					{
						if (networkSystemFusion.isWrongVersion)
						{
							netJoinResult = NetJoinResult.Failed_Other;
							goto IL_227;
						}
						if (networkSystemFusion.netState != NetSystemState.Idle && networkSystemFusion.netState != NetSystemState.InGame)
						{
							netJoinResult = NetJoinResult.Failed_Other;
							goto IL_227;
						}
						if (networkSystemFusion.InRoom && roomName == networkSystemFusion.RoomName)
						{
							netJoinResult = NetJoinResult.AlreadyInRoom;
							goto IL_227;
						}
						networkSystemFusion.netState = NetSystemState.Connecting;
						Debug.Log("Connecting to:" + (string.IsNullOrEmpty(roomName) ? "random room" : roomName));
						if (!string.IsNullOrEmpty(roomName))
						{
							makeOrJoinTask = networkSystemFusion.MakeOrJoinRoom(roomName, opts);
							taskAwaiter = makeOrJoinTask.GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								num2 = 0;
								taskAwaiter2 = taskAwaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, NetworkSystemFusion.<ConnectToRoom>d__51>(ref taskAwaiter, ref this);
								return;
							}
							goto IL_116;
						}
						else
						{
							makeOrJoinTask = networkSystemFusion.JoinRandomPublicRoom(opts);
							taskAwaiter = makeOrJoinTask.GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								num2 = 1;
								taskAwaiter2 = taskAwaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, NetworkSystemFusion.<ConnectToRoom>d__51>(ref taskAwaiter, ref this);
								return;
							}
						}
					}
					else
					{
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<NetJoinResult>);
						num2 = -1;
					}
					taskAwaiter.GetResult();
					netJoinResult2 = makeOrJoinTask.Result;
					makeOrJoinTask = null;
					goto IL_1BA;
				}
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<NetJoinResult>);
				num2 = -1;
				IL_116:
				taskAwaiter.GetResult();
				netJoinResult2 = makeOrJoinTask.Result;
				makeOrJoinTask = null;
				IL_1BA:
				if (netJoinResult2 == NetJoinResult.Failed_Full || netJoinResult2 == NetJoinResult.Failed_Other)
				{
					networkSystemFusion.ResetSystem();
					netJoinResult = netJoinResult2;
				}
				else if (netJoinResult2 == NetJoinResult.AlreadyInRoom)
				{
					networkSystemFusion.netState = NetSystemState.InGame;
					netJoinResult = netJoinResult2;
				}
				else
				{
					networkSystemFusion.UpdatePlayerIDCache();
					networkSystemFusion.UpdateNetPlayerList();
					networkSystemFusion.netState = NetSystemState.InGame;
					Debug.Log("Connect to room result: " + netJoinResult2.ToString());
					netJoinResult = netJoinResult2;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			IL_227:
			num2 = -2;
			this.<>t__builder.SetResult(netJoinResult);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<NetJoinResult> <>t__builder;

		public NetworkSystemFusion <>4__this;

		public string roomName;

		public RoomConfig opts;

		private Task<NetJoinResult> <makeOrJoinTask>5__2;

		private TaskAwaiter<NetJoinResult> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <Initialise>d__47 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			try
			{
				TaskAwaiter taskAwaiter;
				if (num != 0)
				{
					networkSystemFusion.<>n__0();
					networkSystemFusion.netState = NetSystemState.Initialization;
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Idle;
					taskAwaiter = networkSystemFusion.ReturnToSinglePlayer().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemFusion.<Initialise>d__47>(ref taskAwaiter, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter);
					num2 = -1;
				}
				taskAwaiter.GetResult();
				Debug.Log("After return to single player task.");
				networkSystemFusion.AwaitAuth();
				Debug.Log("Creating region crawler");
				networkSystemFusion.CreateRegionCrawler();
				networkSystemFusion.netState = NetSystemState.Idle;
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public NetworkSystemFusion <>4__this;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <JoinRandomPublicRoom>d__54 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			NetJoinResult netJoinResult;
			try
			{
				TaskAwaiter<bool> taskAwaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
						num2 = -1;
						goto IL_15E;
					}
					shouldCreateIfNone = opts.createIfMissing;
					PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
					opts.createIfMissing = false;
					connectTask = networkSystemFusion.Connect(GameMode.Shared, null, opts);
					taskAwaiter = connectTask.GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter<bool> taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<JoinRandomPublicRoom>d__54>(ref taskAwaiter, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
					num2 = -1;
				}
				taskAwaiter.GetResult();
				if (connectTask.Result || !shouldCreateIfNone)
				{
					netJoinResult = NetJoinResult.Success;
					goto IL_1BA;
				}
				opts.createIfMissing = shouldCreateIfNone;
				string randomRoomName = NetworkSystem.GetRandomRoomName();
				createTask = networkSystemFusion.Connect(GameMode.Shared, randomRoomName, opts);
				taskAwaiter = createTask.GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					num2 = 1;
					TaskAwaiter<bool> taskAwaiter2 = taskAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<JoinRandomPublicRoom>d__54>(ref taskAwaiter, ref this);
					return;
				}
				IL_15E:
				taskAwaiter.GetResult();
				if (!createTask.Result)
				{
					Debug.LogError("NS-FUS] Failed to create public room");
					netJoinResult = NetJoinResult.Failed_Other;
				}
				else
				{
					opts.SetFusionOpts(networkSystemFusion.runner);
					netJoinResult = NetJoinResult.FallbackCreated;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				connectTask = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			IL_1BA:
			num2 = -2;
			connectTask = null;
			this.<>t__builder.SetResult(netJoinResult);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<NetJoinResult> <>t__builder;

		public RoomConfig opts;

		public NetworkSystemFusion <>4__this;

		private bool <shouldCreateIfNone>5__2;

		private Task<bool> <connectTask>5__3;

		private TaskAwaiter<bool> <>u__1;

		private Task<bool> <createTask>5__4;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <MakeOrJoinRoom>d__53 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			NetJoinResult netJoinResult;
			try
			{
				TaskAwaiter<bool> taskAwaiter;
				bool flag;
				YieldAwaitable.YieldAwaiter yieldAwaiter;
				switch (num)
				{
				case 0:
					IL_3A:
					try
					{
						if (num != 0)
						{
							PhotonAppSettings.Instance.AppSettings.FixedRegion = networkSystemFusion.regionNames[currentRegionIndex];
							networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
							connectTask = networkSystemFusion.Connect(GameMode.Shared, roomName, opts);
							taskAwaiter = connectTask.GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								num = (num2 = 0);
								TaskAwaiter<bool> taskAwaiter2 = taskAwaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<MakeOrJoinRoom>d__53>(ref taskAwaiter, ref this);
								return;
							}
						}
						else
						{
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
							num = (num2 = -1);
						}
						taskAwaiter.GetResult();
						flag = connectTask.Result;
						if (!flag)
						{
							if (networkSystemFusion.lastConnectAttempt_WasFull)
							{
								Debug.Log("Found room but it was full");
								goto IL_162;
							}
							Debug.Log("Region incrimenting");
							int num3 = currentRegionIndex + 1;
							currentRegionIndex = num3;
						}
						connectTask = null;
					}
					catch (Exception ex)
					{
						Debug.LogError("MakeOrJoinRoom - message: " + ex.Message + "\nStacktrace : " + ex.StackTrace);
						netJoinResult = NetJoinResult.Failed_Other;
						goto IL_2DC;
					}
					break;
				case 1:
				{
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
					num = (num2 = -1);
					goto IL_21B;
				}
				case 2:
				{
					YieldAwaitable.YieldAwaiter yieldAwaiter2;
					yieldAwaiter = yieldAwaiter2;
					yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
					num = (num2 = -1);
					goto IL_2A2;
				}
				default:
					currentRegionIndex = 0;
					flag = false;
					opts.createIfMissing = false;
					break;
				}
				if (currentRegionIndex < networkSystemFusion.regionNames.Length && !flag)
				{
					goto IL_3A;
				}
				IL_162:
				if (networkSystemFusion.lastConnectAttempt_WasFull)
				{
					PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
					netJoinResult = NetJoinResult.Failed_Full;
					goto IL_2DC;
				}
				if (flag)
				{
					netJoinResult = NetJoinResult.Success;
					goto IL_2DC;
				}
				PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
				opts.createIfMissing = true;
				connectTask = networkSystemFusion.Connect(GameMode.Shared, roomName, opts);
				taskAwaiter = connectTask.GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					num = (num2 = 1);
					TaskAwaiter<bool> taskAwaiter2 = taskAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<MakeOrJoinRoom>d__53>(ref taskAwaiter, ref this);
					return;
				}
				IL_21B:
				taskAwaiter.GetResult();
				Debug.Log("made room?");
				if (!connectTask.Result)
				{
					Debug.LogError("NS-FUS] Failed to create private room");
					netJoinResult = NetJoinResult.Failed_Other;
					goto IL_2DC;
				}
				goto IL_2A9;
				IL_2A2:
				yieldAwaiter.GetResult();
				IL_2A9:
				if (networkSystemFusion.runner.SessionInfo.IsValid)
				{
					netJoinResult = NetJoinResult.FallbackCreated;
				}
				else
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num = (num2 = 2);
						YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<MakeOrJoinRoom>d__53>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_2A2;
				}
			}
			catch (Exception ex2)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex2);
				return;
			}
			IL_2DC:
			num2 = -2;
			this.<>t__builder.SetResult(netJoinResult);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<NetJoinResult> <>t__builder;

		public RoomConfig opts;

		public NetworkSystemFusion <>4__this;

		public string roomName;

		private int <currentRegionIndex>5__2;

		private Task<bool> <connectTask>5__3;

		private TaskAwaiter<bool> <>u__1;

		private YieldAwaitable.YieldAwaiter <>u__2;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <ResetSystem>d__60 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			try
			{
				TaskAwaiter<bool> taskAwaiter;
				if (num != 0)
				{
					taskAwaiter = networkSystemFusion.Connect(GameMode.Single, "--", RoomConfig.SPConfig()).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter<bool> taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<ResetSystem>d__60>(ref taskAwaiter, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
					num2 = -1;
				}
				taskAwaiter.GetResult();
				Debug.Log("Connect in return to single player");
				networkSystemFusion.netState = NetSystemState.Idle;
				networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Idle;
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public NetworkSystemFusion <>4__this;

		private TaskAwaiter<bool> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <ReturnToSinglePlayer>d__57 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			NetworkSystemFusion networkSystemFusion = this;
			try
			{
				YieldAwaitable.YieldAwaiter yieldAwaiter;
				TaskAwaiter taskAwaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						YieldAwaitable.YieldAwaiter yieldAwaiter2;
						yieldAwaiter = yieldAwaiter2;
						yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
						num2 = -1;
						goto IL_FE;
					}
					if (networkSystemFusion.netState != NetSystemState.InGame && networkSystemFusion.netState != NetSystemState.Initialization)
					{
						goto IL_13E;
					}
					networkSystemFusion.netState = NetSystemState.Disconnecting;
					Debug.Log("Returning to single player");
					if (!networkSystemFusion.runner)
					{
						goto IL_10F;
					}
					taskAwaiter = networkSystemFusion.CloseRunner().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemFusion.<ReturnToSinglePlayer>d__57>(ref taskAwaiter, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter);
					num2 = -1;
				}
				taskAwaiter.GetResult();
				yieldAwaiter = Task.Yield().GetAwaiter();
				if (!yieldAwaiter.IsCompleted)
				{
					num2 = 1;
					YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<ReturnToSinglePlayer>d__57>(ref yieldAwaiter, ref this);
					return;
				}
				IL_FE:
				yieldAwaiter.GetResult();
				Debug.Log("Connect in return to single player");
				IL_10F:
				networkSystemFusion.netState = NetSystemState.Idle;
				networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Idle;
				networkSystemFusion.SinglePlayerStarted();
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			IL_13E:
			num2 = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public NetworkSystemFusion <>4__this;

		private TaskAwaiter <>u__1;

		private YieldAwaitable.YieldAwaiter <>u__2;
	}
}
