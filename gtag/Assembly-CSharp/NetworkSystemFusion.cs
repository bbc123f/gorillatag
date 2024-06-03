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

	public override void Initialise()
	{
		NetworkSystemFusion.<Initialise>d__47 <Initialise>d__;
		<Initialise>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialise>d__.<>4__this = this;
		<Initialise>d__.<>1__state = -1;
		<Initialise>d__.<>t__builder.Start<NetworkSystemFusion.<Initialise>d__47>(ref <Initialise>d__);
	}

	private void CreateRegionCrawler()
	{
		GameObject gameObject = new GameObject("[Network Crawler]");
		gameObject.transform.SetParent(base.transform);
		this.regionCrawler = gameObject.AddComponent<FusionRegionCrawler>();
	}

	private Task AwaitAuth()
	{
		NetworkSystemFusion.<AwaitAuth>d__49 <AwaitAuth>d__;
		<AwaitAuth>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitAuth>d__.<>4__this = this;
		<AwaitAuth>d__.<>1__state = -1;
		<AwaitAuth>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitAuth>d__49>(ref <AwaitAuth>d__);
		return <AwaitAuth>d__.<>t__builder.Task;
	}

	public override void SetAuthenticationValues(Dictionary<string, string> authValues)
	{
		this.cachedPlayfabAuth = authValues.ToAuthValues_F();
	}

	public override Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemFusion.<ConnectToRoom>d__51 <ConnectToRoom>d__;
		<ConnectToRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<ConnectToRoom>d__.<>4__this = this;
		<ConnectToRoom>d__.roomName = roomName;
		<ConnectToRoom>d__.opts = opts;
		<ConnectToRoom>d__.<>1__state = -1;
		<ConnectToRoom>d__.<>t__builder.Start<NetworkSystemFusion.<ConnectToRoom>d__51>(ref <ConnectToRoom>d__);
		return <ConnectToRoom>d__.<>t__builder.Task;
	}

	private Task<bool> Connect(GameMode mode, string targetSessionName, RoomConfig opts)
	{
		NetworkSystemFusion.<Connect>d__52 <Connect>d__;
		<Connect>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Connect>d__.<>4__this = this;
		<Connect>d__.mode = mode;
		<Connect>d__.targetSessionName = targetSessionName;
		<Connect>d__.opts = opts;
		<Connect>d__.<>1__state = -1;
		<Connect>d__.<>t__builder.Start<NetworkSystemFusion.<Connect>d__52>(ref <Connect>d__);
		return <Connect>d__.<>t__builder.Task;
	}

	private Task<NetJoinResult> MakeOrJoinRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemFusion.<MakeOrJoinRoom>d__53 <MakeOrJoinRoom>d__;
		<MakeOrJoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<MakeOrJoinRoom>d__.<>4__this = this;
		<MakeOrJoinRoom>d__.roomName = roomName;
		<MakeOrJoinRoom>d__.opts = opts;
		<MakeOrJoinRoom>d__.<>1__state = -1;
		<MakeOrJoinRoom>d__.<>t__builder.Start<NetworkSystemFusion.<MakeOrJoinRoom>d__53>(ref <MakeOrJoinRoom>d__);
		return <MakeOrJoinRoom>d__.<>t__builder.Task;
	}

	private Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		NetworkSystemFusion.<JoinRandomPublicRoom>d__54 <JoinRandomPublicRoom>d__;
		<JoinRandomPublicRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<JoinRandomPublicRoom>d__.<>4__this = this;
		<JoinRandomPublicRoom>d__.opts = opts;
		<JoinRandomPublicRoom>d__.<>1__state = -1;
		<JoinRandomPublicRoom>d__.<>t__builder.Start<NetworkSystemFusion.<JoinRandomPublicRoom>d__54>(ref <JoinRandomPublicRoom>d__);
		return <JoinRandomPublicRoom>d__.<>t__builder.Task;
	}

	public override Task JoinFriendsRoom(string userID, int actorID, string keyToFollow, string shufflerToFollow)
	{
		throw new NotImplementedException();
	}

	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	public override Task ReturnToSinglePlayer()
	{
		NetworkSystemFusion.<ReturnToSinglePlayer>d__57 <ReturnToSinglePlayer>d__;
		<ReturnToSinglePlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ReturnToSinglePlayer>d__.<>4__this = this;
		<ReturnToSinglePlayer>d__.<>1__state = -1;
		<ReturnToSinglePlayer>d__.<>t__builder.Start<NetworkSystemFusion.<ReturnToSinglePlayer>d__57>(ref <ReturnToSinglePlayer>d__);
		return <ReturnToSinglePlayer>d__.<>t__builder.Task;
	}

	private Task CloseRunner()
	{
		NetworkSystemFusion.<CloseRunner>d__58 <CloseRunner>d__;
		<CloseRunner>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CloseRunner>d__.<>4__this = this;
		<CloseRunner>d__.<>1__state = -1;
		<CloseRunner>d__.<>t__builder.Start<NetworkSystemFusion.<CloseRunner>d__58>(ref <CloseRunner>d__);
		return <CloseRunner>d__.<>t__builder.Task;
	}

	public void MigrateHost(HostMigrationToken hostMigrationToken)
	{
		Debug.Log("HOSTTEST : MigrateHostTriggered, returning to single player!");
		this.ReturnToSinglePlayer();
	}

	public void ResetSystem()
	{
		NetworkSystemFusion.<ResetSystem>d__60 <ResetSystem>d__;
		<ResetSystem>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ResetSystem>d__.<>4__this = this;
		<ResetSystem>d__.<>1__state = -1;
		<ResetSystem>d__.<>t__builder.Start<NetworkSystemFusion.<ResetSystem>d__60>(ref <ResetSystem>d__);
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
		foreach (PlayerRef value in this.runner.ActivePlayers)
		{
			if (value.PlayerId == playerAuthID)
			{
				return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(value), null, null, true).gameObject;
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
		foreach (PlayerRef a in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				a != this.runner.LocalPlayer;
			}
		}
	}

	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		Debug.Log(rpcMethod.GetDelegateName() + "RPC called!");
		ref args.SerializeToRPCData<T>();
		foreach (PlayerRef a in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				a != this.runner.LocalPlayer;
			}
		}
	}

	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		foreach (PlayerRef a in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				a != this.runner.LocalPlayer;
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

	public override Task AwaitSceneReady()
	{
		NetworkSystemFusion.<AwaitSceneReady>d__75 <AwaitSceneReady>d__;
		<AwaitSceneReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitSceneReady>d__.<>4__this = this;
		<AwaitSceneReady>d__.<>1__state = -1;
		<AwaitSceneReady>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitSceneReady>d__75>(ref <AwaitSceneReady>d__);
		return <AwaitSceneReady>d__.<>t__builder.Task;
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

	private Task AwaitJoiningPlayerClientReady(PlayerRef player)
	{
		NetworkSystemFusion.<AwaitJoiningPlayerClientReady>d__81 <AwaitJoiningPlayerClientReady>d__;
		<AwaitJoiningPlayerClientReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitJoiningPlayerClientReady>d__.<>4__this = this;
		<AwaitJoiningPlayerClientReady>d__.player = player;
		<AwaitJoiningPlayerClientReady>d__.<>1__state = -1;
		<AwaitJoiningPlayerClientReady>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitJoiningPlayerClientReady>d__81>(ref <AwaitJoiningPlayerClientReady>d__);
		return <AwaitJoiningPlayerClientReady>d__.<>t__builder.Task;
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
		PlayerRef player = this.runner.LocalPlayer;
		if (owningPlayerID != null)
		{
			player = this.GetPlayerRef(owningPlayerID.Value);
		}
		this.runner.SetPlayerObject(player, playerInstance.GetComponent<NetworkObject>());
	}

	private PlayerRef GetPlayerRef(int playerID)
	{
		foreach (PlayerRef result in this.runner.ActivePlayers)
		{
			if (result.PlayerId == playerID)
			{
				return result;
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
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			try
			{
				if (num != 0)
				{
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.AwaitingAuth;
					goto IL_74;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_6D:
				awaiter.GetResult();
				IL_74:
				if (networkSystemFusion.cachedPlayfabAuth != null)
				{
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Idle;
					networkSystemFusion.netState = NetSystemState.Idle;
				}
				else
				{
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<AwaitAuth>d__49>(ref awaiter, ref this);
						return;
					}
					goto IL_6D;
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

		public NetworkSystemFusion <>4__this;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <AwaitJoiningPlayerClientReady>d__81 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			try
			{
				Debug.Log("Player: " + this.player.PlayerId.ToString() + "is joining!");
				networkSystemFusion.UpdatePlayerIDCache();
				networkSystemFusion.UpdateNetPlayerList();
				if (networkSystemFusion.runner != null && this.player == networkSystemFusion.runner.LocalPlayer && !networkSystemFusion.runner.IsSinglePlayer)
				{
					Debug.Log("Multiplayer started");
					networkSystemFusion.MultiplayerStarted();
				}
				if (networkSystemFusion.runner != null && this.player == networkSystemFusion.runner.LocalPlayer && networkSystemFusion.runner.IsSinglePlayer)
				{
					Debug.Log("Singleplayer started");
					networkSystemFusion.SinglePlayerStarted();
				}
				Debug.Log("Player " + this.player.PlayerId.ToString() + " properties sorted!");
				networkSystemFusion.PlayerJoined(this.player.PlayerId);
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

		public PlayerRef player;

		public NetworkSystemFusion <>4__this;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <AwaitSceneReady>d__75 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			try
			{
				YieldAwaitable.YieldAwaiter awaiter;
				if (num != 0)
				{
					if (num != 1)
					{
						goto IL_77;
					}
					awaiter = this.<>u__1;
					this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
					this.<>1__state = -1;
					goto IL_EF;
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
					this.<>1__state = -1;
				}
				IL_70:
				awaiter.GetResult();
				IL_77:
				if (networkSystemFusion.runner.SceneManager.IsReady(networkSystemFusion.runner))
				{
					this.<counter>5__2 = 0f;
					goto IL_108;
				}
				awaiter = Task.Yield().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 0;
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<AwaitSceneReady>d__75>(ref awaiter, ref this);
					return;
				}
				goto IL_70;
				IL_EF:
				awaiter.GetResult();
				this.<counter>5__2 += Time.deltaTime;
				IL_108:
				if (this.<counter>5__2 < 0.5f)
				{
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 1;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<AwaitSceneReady>d__75>(ref awaiter, ref this);
						return;
					}
					goto IL_EF;
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
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			try
			{
				if (num != 0)
				{
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Disconnecting;
				}
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						awaiter = networkSystemFusion.runner.Shutdown(true, ShutdownReason.Ok, false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							this.<>1__state = 0;
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemFusion.<CloseRunner>d__58>(ref awaiter, ref this);
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

		public NetworkSystemFusion <>4__this;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <Connect>d__52 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			bool ok;
			try
			{
				TaskAwaiter awaiter;
				YieldAwaitable.YieldAwaiter awaiter2;
				TaskAwaiter<StartGameResult> awaiter3;
				switch (num)
				{
				case 0:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
					break;
				case 1:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(YieldAwaitable.YieldAwaiter);
					this.<>1__state = -1;
					goto IL_10E;
				case 2:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(YieldAwaitable.YieldAwaiter);
					this.<>1__state = -1;
					goto IL_17D;
				case 3:
					awaiter3 = this.<>u__3;
					this.<>u__3 = default(TaskAwaiter<StartGameResult>);
					this.<>1__state = -1;
					goto IL_347;
				case 4:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(YieldAwaitable.YieldAwaiter);
					this.<>1__state = -1;
					goto IL_44E;
				default:
					if (!(networkSystemFusion.runner != null))
					{
						goto IL_184;
					}
					this.<goingBetweenRooms>5__3 = (networkSystemFusion.InRoom && this.mode != GameMode.Single);
					awaiter = networkSystemFusion.CloseRunner().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemFusion.<Connect>d__52>(ref awaiter, ref this);
						return;
					}
					break;
				}
				awaiter.GetResult();
				awaiter2 = Task.Yield().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__2 = awaiter2;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<Connect>d__52>(ref awaiter2, ref this);
					return;
				}
				IL_10E:
				awaiter2.GetResult();
				if (!this.<goingBetweenRooms>5__3)
				{
					goto IL_184;
				}
				networkSystemFusion.SinglePlayerStarted();
				awaiter2 = Task.Yield().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					this.<>1__state = 2;
					this.<>u__2 = awaiter2;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<Connect>d__52>(ref awaiter2, ref this);
					return;
				}
				IL_17D:
				awaiter2.GetResult();
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
				if (this.mode != GameMode.Single)
				{
					SceneManager.GetActiveScene();
				}
				networkSystemFusion.lastConnectAttempt_WasFull = false;
				networkSystemFusion.lastConnectAttempt_WasMissing = false;
				networkSystemFusion.internalState = NetworkSystemFusion.InternalState.ConnectingToRoom;
				Hashtable customProps = this.opts.customProps;
				Dictionary<string, SessionProperty> sessionProperties = (customProps != null) ? customProps.ToPropDict() : null;
				this.<startupTask>5__2 = networkSystemFusion.runner.StartGame(new StartGameArgs
				{
					IsVisible = new bool?(this.opts.isPublic),
					IsOpen = new bool?(this.opts.isJoinable),
					GameMode = this.mode,
					SessionName = this.targetSessionName,
					PlayerCount = new int?((int)this.opts.MaxPlayers),
					SceneManager = networkSystemFusion.volatileNetObj.AddComponent<NetworkSceneManagerDefault>(),
					SessionProperties = sessionProperties,
					DisableClientSessionCreation = !this.opts.createIfMissing
				});
				awaiter3 = this.<startupTask>5__2.GetAwaiter();
				if (!awaiter3.IsCompleted)
				{
					this.<>1__state = 3;
					this.<>u__3 = awaiter3;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<StartGameResult>, NetworkSystemFusion.<Connect>d__52>(ref awaiter3, ref this);
					return;
				}
				IL_347:
				awaiter3.GetResult();
				Debug.Log("Startuptask finished : " + this.<startupTask>5__2.Result.ToString());
				if (!this.<startupTask>5__2.Result.Ok)
				{
					networkSystemFusion.CurrentRoom = null;
					ok = this.<startupTask>5__2.Result.Ok;
					goto IL_4C4;
				}
				networkSystemFusion.AddVoice();
				networkSystemFusion.CurrentRoom = this.opts;
				if (networkSystemFusion.IsTotalAuthority() || networkSystemFusion.runner.IsSharedModeMasterClient)
				{
					this.opts.SetFusionOpts(networkSystemFusion.runner);
					networkSystemFusion.runner.SetActiveScene(SceneManager.GetActiveScene().name);
					goto IL_455;
				}
				goto IL_46D;
				IL_44E:
				awaiter2.GetResult();
				IL_455:
				if (!networkSystemFusion.runner.SceneManager.IsReady(networkSystemFusion.runner))
				{
					awaiter2 = Task.Yield().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						this.<>1__state = 4;
						this.<>u__2 = awaiter2;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<Connect>d__52>(ref awaiter2, ref this);
						return;
					}
					goto IL_44E;
				}
				IL_46D:
				networkSystemFusion.SetMyNickName(GorillaComputer.instance.savedName);
				networkSystemFusion.runner.AddSimulationBehaviour(networkSystemFusion.internalRPCProvider, null);
				ok = this.<startupTask>5__2.Result.Ok;
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<startupTask>5__2 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_4C4:
			this.<>1__state = -2;
			this.<startupTask>5__2 = null;
			this.<>t__builder.SetResult(ok);
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
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			NetJoinResult result;
			try
			{
				TaskAwaiter<NetJoinResult> awaiter;
				NetJoinResult result2;
				if (num != 0)
				{
					if (num != 1)
					{
						if (networkSystemFusion.isWrongVersion)
						{
							result = NetJoinResult.Failed_Other;
							goto IL_227;
						}
						if (networkSystemFusion.netState != NetSystemState.Idle && networkSystemFusion.netState != NetSystemState.InGame)
						{
							result = NetJoinResult.Failed_Other;
							goto IL_227;
						}
						if (networkSystemFusion.InRoom && this.roomName == networkSystemFusion.RoomName)
						{
							result = NetJoinResult.AlreadyInRoom;
							goto IL_227;
						}
						networkSystemFusion.netState = NetSystemState.Connecting;
						Debug.Log("Connecting to:" + (string.IsNullOrEmpty(this.roomName) ? "random room" : this.roomName));
						if (!string.IsNullOrEmpty(this.roomName))
						{
							this.<makeOrJoinTask>5__2 = networkSystemFusion.MakeOrJoinRoom(this.roomName, this.opts);
							awaiter = this.<makeOrJoinTask>5__2.GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								this.<>1__state = 0;
								this.<>u__1 = awaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, NetworkSystemFusion.<ConnectToRoom>d__51>(ref awaiter, ref this);
								return;
							}
							goto IL_116;
						}
						else
						{
							this.<makeOrJoinTask>5__2 = networkSystemFusion.JoinRandomPublicRoom(this.opts);
							awaiter = this.<makeOrJoinTask>5__2.GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								this.<>1__state = 1;
								this.<>u__1 = awaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, NetworkSystemFusion.<ConnectToRoom>d__51>(ref awaiter, ref this);
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
					awaiter.GetResult();
					result2 = this.<makeOrJoinTask>5__2.Result;
					this.<makeOrJoinTask>5__2 = null;
					goto IL_1BA;
				}
				awaiter = this.<>u__1;
				this.<>u__1 = default(TaskAwaiter<NetJoinResult>);
				this.<>1__state = -1;
				IL_116:
				awaiter.GetResult();
				result2 = this.<makeOrJoinTask>5__2.Result;
				this.<makeOrJoinTask>5__2 = null;
				IL_1BA:
				if (result2 == NetJoinResult.Failed_Full || result2 == NetJoinResult.Failed_Other)
				{
					networkSystemFusion.ResetSystem();
					result = result2;
				}
				else if (result2 == NetJoinResult.AlreadyInRoom)
				{
					networkSystemFusion.netState = NetSystemState.InGame;
					result = result2;
				}
				else
				{
					networkSystemFusion.UpdatePlayerIDCache();
					networkSystemFusion.UpdateNetPlayerList();
					networkSystemFusion.netState = NetSystemState.InGame;
					Debug.Log("Connect to room result: " + result2.ToString());
					result = result2;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_227:
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
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					networkSystemFusion.<>n__0();
					networkSystemFusion.netState = NetSystemState.Initialization;
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Idle;
					awaiter = networkSystemFusion.ReturnToSinglePlayer().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemFusion.<Initialise>d__47>(ref awaiter, ref this);
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
				Debug.Log("After return to single player task.");
				networkSystemFusion.AwaitAuth();
				Debug.Log("Creating region crawler");
				networkSystemFusion.CreateRegionCrawler();
				networkSystemFusion.netState = NetSystemState.Idle;
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

		public NetworkSystemFusion <>4__this;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <JoinRandomPublicRoom>d__54 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
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
						goto IL_15E;
					}
					this.<shouldCreateIfNone>5__2 = this.opts.createIfMissing;
					PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
					networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
					this.opts.createIfMissing = false;
					this.<connectTask>5__3 = networkSystemFusion.Connect(GameMode.Shared, null, this.opts);
					awaiter = this.<connectTask>5__3.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<JoinRandomPublicRoom>d__54>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
				}
				awaiter.GetResult();
				if (!(!this.<connectTask>5__3.Result & this.<shouldCreateIfNone>5__2))
				{
					result = NetJoinResult.Success;
					goto IL_1BA;
				}
				this.opts.createIfMissing = this.<shouldCreateIfNone>5__2;
				string randomRoomName = NetworkSystem.GetRandomRoomName();
				this.<createTask>5__4 = networkSystemFusion.Connect(GameMode.Shared, randomRoomName, this.opts);
				awaiter = this.<createTask>5__4.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<JoinRandomPublicRoom>d__54>(ref awaiter, ref this);
					return;
				}
				IL_15E:
				awaiter.GetResult();
				if (!this.<createTask>5__4.Result)
				{
					Debug.LogError("NS-FUS] Failed to create public room");
					result = NetJoinResult.Failed_Other;
				}
				else
				{
					this.opts.SetFusionOpts(networkSystemFusion.runner);
					result = NetJoinResult.FallbackCreated;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<connectTask>5__3 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_1BA:
			this.<>1__state = -2;
			this.<connectTask>5__3 = null;
			this.<>t__builder.SetResult(result);
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
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			NetJoinResult result;
			try
			{
				TaskAwaiter<bool> awaiter;
				bool flag;
				YieldAwaitable.YieldAwaiter awaiter2;
				switch (num)
				{
				case 0:
					IL_3A:
					try
					{
						if (num != 0)
						{
							PhotonAppSettings.Instance.AppSettings.FixedRegion = networkSystemFusion.regionNames[this.<currentRegionIndex>5__2];
							networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
							this.<connectTask>5__3 = networkSystemFusion.Connect(GameMode.Shared, this.roomName, this.opts);
							awaiter = this.<connectTask>5__3.GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (this.<>1__state = 0);
								this.<>u__1 = awaiter;
								this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<MakeOrJoinRoom>d__53>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = this.<>u__1;
							this.<>u__1 = default(TaskAwaiter<bool>);
							num = (this.<>1__state = -1);
						}
						awaiter.GetResult();
						flag = this.<connectTask>5__3.Result;
						if (!flag)
						{
							if (networkSystemFusion.lastConnectAttempt_WasFull)
							{
								Debug.Log("Found room but it was full");
								goto IL_162;
							}
							Debug.Log("Region incrimenting");
							int num2 = this.<currentRegionIndex>5__2 + 1;
							this.<currentRegionIndex>5__2 = num2;
						}
						this.<connectTask>5__3 = null;
					}
					catch (Exception ex)
					{
						Debug.LogError("MakeOrJoinRoom - message: " + ex.Message + "\nStacktrace : " + ex.StackTrace);
						result = NetJoinResult.Failed_Other;
						goto IL_2DC;
					}
					break;
				case 1:
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					num = (this.<>1__state = -1);
					goto IL_21B;
				case 2:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(YieldAwaitable.YieldAwaiter);
					num = (this.<>1__state = -1);
					goto IL_2A2;
				default:
					this.<currentRegionIndex>5__2 = 0;
					flag = false;
					this.opts.createIfMissing = false;
					break;
				}
				if (this.<currentRegionIndex>5__2 < networkSystemFusion.regionNames.Length && !flag)
				{
					goto IL_3A;
				}
				IL_162:
				if (networkSystemFusion.lastConnectAttempt_WasFull)
				{
					PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
					result = NetJoinResult.Failed_Full;
					goto IL_2DC;
				}
				if (flag)
				{
					result = NetJoinResult.Success;
					goto IL_2DC;
				}
				PhotonAppSettings.Instance.AppSettings.FixedRegion = "";
				this.opts.createIfMissing = true;
				this.<connectTask>5__3 = networkSystemFusion.Connect(GameMode.Shared, this.roomName, this.opts);
				awaiter = this.<connectTask>5__3.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (this.<>1__state = 1);
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<MakeOrJoinRoom>d__53>(ref awaiter, ref this);
					return;
				}
				IL_21B:
				awaiter.GetResult();
				Debug.Log("made room?");
				if (!this.<connectTask>5__3.Result)
				{
					Debug.LogError("NS-FUS] Failed to create private room");
					result = NetJoinResult.Failed_Other;
					goto IL_2DC;
				}
				goto IL_2A9;
				IL_2A2:
				awaiter2.GetResult();
				IL_2A9:
				if (networkSystemFusion.runner.SessionInfo.IsValid)
				{
					result = NetJoinResult.FallbackCreated;
				}
				else
				{
					awaiter2 = Task.Yield().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (this.<>1__state = 2);
						this.<>u__2 = awaiter2;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<MakeOrJoinRoom>d__53>(ref awaiter2, ref this);
						return;
					}
					goto IL_2A2;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_2DC:
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
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			try
			{
				TaskAwaiter<bool> awaiter;
				if (num != 0)
				{
					awaiter = networkSystemFusion.Connect(GameMode.Single, "--", RoomConfig.SPConfig()).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, NetworkSystemFusion.<ResetSystem>d__60>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
				}
				awaiter.GetResult();
				Debug.Log("Connect in return to single player");
				networkSystemFusion.netState = NetSystemState.Idle;
				networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Idle;
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

		public NetworkSystemFusion <>4__this;

		private TaskAwaiter<bool> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <ReturnToSinglePlayer>d__57 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystemFusion networkSystemFusion = this.<>4__this;
			try
			{
				YieldAwaitable.YieldAwaiter awaiter;
				TaskAwaiter awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = this.<>u__2;
						this.<>u__2 = default(YieldAwaitable.YieldAwaiter);
						this.<>1__state = -1;
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
					awaiter2 = networkSystemFusion.CloseRunner().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter2;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, NetworkSystemFusion.<ReturnToSinglePlayer>d__57>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
				}
				awaiter2.GetResult();
				awaiter = Task.Yield().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__2 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, NetworkSystemFusion.<ReturnToSinglePlayer>d__57>(ref awaiter, ref this);
					return;
				}
				IL_FE:
				awaiter.GetResult();
				Debug.Log("Connect in return to single player");
				IL_10F:
				networkSystemFusion.netState = NetSystemState.Idle;
				networkSystemFusion.internalState = NetworkSystemFusion.InternalState.Idle;
				networkSystemFusion.SinglePlayerStarted();
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_13E:
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

		public NetworkSystemFusion <>4__this;

		private TaskAwaiter <>u__1;

		private YieldAwaitable.YieldAwaiter <>u__2;
	}
}
