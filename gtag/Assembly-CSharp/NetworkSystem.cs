using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public abstract class NetworkSystem : MonoBehaviour
{
	public bool groupJoinInProgress
	{
		[CompilerGenerated]
		get
		{
			return this.<groupJoinInProgress>k__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			this.<groupJoinInProgress>k__BackingField = value;
		}
	}

	public NetSystemState netState
	{
		get
		{
			return this.testState;
		}
		protected set
		{
			Debug.Log("netstate set to:" + value.ToString());
			this.testState = value;
		}
	}

	public NetPlayer LocalPlayer
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsLocal);
		}
	}

	public bool IsMasterClient
	{
		get
		{
			return this.LocalPlayer.IsMaster;
		}
	}

	public Recorder LocalRecorder
	{
		get
		{
			return this.localRecorder;
		}
	}

	public virtual Speaker LocalSpeaker
	{
		[CompilerGenerated]
		get
		{
			return this.<LocalSpeaker>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<LocalSpeaker>k__BackingField = value;
		}
	}

	public event Action OnMultiplayerStarted
	{
		[CompilerGenerated]
		add
		{
			Action action = this.OnMultiplayerStarted;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.OnMultiplayerStarted, value2, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action action = this.OnMultiplayerStarted;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.OnMultiplayerStarted, value2, action2);
			}
			while (action != action2);
		}
	}

	protected void MultiplayerStarted()
	{
		Action onMultiplayerStarted = this.OnMultiplayerStarted;
		if (onMultiplayerStarted == null)
		{
			return;
		}
		onMultiplayerStarted();
	}

	public event Action OnReturnedToSinglePlayer
	{
		[CompilerGenerated]
		add
		{
			Action action = this.OnReturnedToSinglePlayer;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.OnReturnedToSinglePlayer, value2, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action action = this.OnReturnedToSinglePlayer;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.OnReturnedToSinglePlayer, value2, action2);
			}
			while (action != action2);
		}
	}

	protected void SinglePlayerStarted()
	{
		Action onReturnedToSinglePlayer = this.OnReturnedToSinglePlayer;
		if (onReturnedToSinglePlayer == null)
		{
			return;
		}
		onReturnedToSinglePlayer();
	}

	public event Action<int> OnPlayerJoined
	{
		[CompilerGenerated]
		add
		{
			Action<int> action = this.OnPlayerJoined;
			Action<int> action2;
			do
			{
				action2 = action;
				Action<int> value2 = (Action<int>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<int>>(ref this.OnPlayerJoined, value2, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<int> action = this.OnPlayerJoined;
			Action<int> action2;
			do
			{
				action2 = action;
				Action<int> value2 = (Action<int>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<int>>(ref this.OnPlayerJoined, value2, action2);
			}
			while (action != action2);
		}
	}

	protected void PlayerJoined(int playerID)
	{
		if (this.IsOnline)
		{
			Action<int> onPlayerJoined = this.OnPlayerJoined;
			if (onPlayerJoined == null)
			{
				return;
			}
			onPlayerJoined(playerID);
		}
	}

	public event Action<int> OnPlayerLeft
	{
		[CompilerGenerated]
		add
		{
			Action<int> action = this.OnPlayerLeft;
			Action<int> action2;
			do
			{
				action2 = action;
				Action<int> value2 = (Action<int>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<int>>(ref this.OnPlayerLeft, value2, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<int> action = this.OnPlayerLeft;
			Action<int> action2;
			do
			{
				action2 = action;
				Action<int> value2 = (Action<int>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<int>>(ref this.OnPlayerLeft, value2, action2);
			}
			while (action != action2);
		}
	}

	protected void PlayerLeft(int playerID)
	{
		Action<int> onPlayerLeft = this.OnPlayerLeft;
		if (onPlayerLeft == null)
		{
			return;
		}
		onPlayerLeft(playerID);
	}

	public virtual void Initialise()
	{
		Debug.Log("INITIALISING NETWORKSYSTEMS");
		if (NetworkSystem.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		NetworkSystem.Instance = this;
		NetCrossoverUtils.Prewarm();
	}

	protected virtual void Update()
	{
	}

	public abstract void SetAuthenticationValues(Dictionary<string, string> authValues);

	public abstract Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1);

	public abstract Task JoinFriendsRoom(string userID, int actorID, string keyToFollow, string shufflerToFollow);

	public abstract Task ReturnToSinglePlayer();

	public abstract void JoinPubWithFriends();

	public bool WrongVersion
	{
		get
		{
			return this.isWrongVersion;
		}
	}

	public void SetWrongVersion()
	{
		this.isWrongVersion = true;
	}

	public GameObject NetInstantiate(GameObject prefab, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, false);
	}

	public GameObject NetInstantiate(GameObject prefab, Vector3 position, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, Quaternion.identity, false);
	}

	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false);

	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false);

	public abstract void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null);

	public abstract void NetDestroy(GameObject instance);

	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true);

	public abstract void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true) where T : struct;

	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true);

	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod);

	public abstract void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args) where T : struct;

	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message);

	public static string GetRandomRoomName()
	{
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
		}
		if (GorillaComputer.instance.CheckAutoBanListForName(text))
		{
			return text;
		}
		return NetworkSystem.GetRandomRoomName();
	}

	public abstract string GetRandomWeightedRegion();

	protected Task RefreshOculusNonce()
	{
		NetworkSystem.<RefreshOculusNonce>d__78 <RefreshOculusNonce>d__;
		<RefreshOculusNonce>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RefreshOculusNonce>d__.<>1__state = -1;
		<RefreshOculusNonce>d__.<>t__builder.Start<NetworkSystem.<RefreshOculusNonce>d__78>(ref <RefreshOculusNonce>d__);
		return <RefreshOculusNonce>d__.<>t__builder.Task;
	}

	protected virtual void GetOculusNonceCallback(Message<UserProof> message)
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
				this.nonceRefreshed = true;
			}
		}
	}

	private IEnumerator ReGetNonce()
	{
		yield return new WaitForSeconds(3f);
		Users.GetUserProof().OnComplete(new Message<UserProof>.Callback(this.GetOculusNonceCallback));
		yield return null;
		yield break;
	}

	public void BroadcastMyRoom(bool create, string key, string shuffler)
	{
		string text = NetworkSystem.ShuffleRoomName(NetworkSystem.Instance.RoomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler.Substring(0, 2), true);
		Debug.Log(string.Format("Broadcasting room {0} region {1}({2}). Create: {3} key: {4} (shuffler {5}) shuffled: {6}", new object[]
		{
			NetworkSystem.Instance.RoomName,
			NetworkSystem.Instance.currentRegionIndex,
			NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex],
			create,
			key,
			shuffler,
			text
		}));
		ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
		executeCloudScriptRequest.FunctionName = "BroadcastMyRoom";
		executeCloudScriptRequest.FunctionParameter = new
		{
			KeyToFollow = key,
			RoomToJoin = text,
			Set = create
		};
		PlayFabClientAPI.ExecuteCloudScript(executeCloudScriptRequest, delegate(ExecuteCloudScriptResult result)
		{
		}, delegate(PlayFabError error)
		{
		}, null, null);
	}

	public bool InstantCheckGroupData(string userID, string keyToFollow)
	{
		bool success = false;
		GetSharedGroupDataRequest getSharedGroupDataRequest = new GetSharedGroupDataRequest();
		getSharedGroupDataRequest.Keys = new List<string>
		{
			keyToFollow
		};
		getSharedGroupDataRequest.SharedGroupId = userID;
		PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
		{
			Debug.Log("Get Shared Group Data returned a success");
			Debug.Log(result.Data.ToStringFull());
			if (result.Data.Count > 0)
			{
				success = true;
				return;
			}
			Debug.Log("RESULT returned but no DATA");
		}, delegate(PlayFabError error)
		{
			Debug.Log("ERROR - no group data found");
		}, null, null);
		return success;
	}

	public NetPlayer GetNetPlayerByID(int playerActorNumber)
	{
		return this.netPlayerCache.Find((NetPlayer a) => a.ID == playerActorNumber);
	}

	public static string ShuffleRoomName(string room, string shuffle, bool encode)
	{
		NetworkSystem.shuffleStringBuilder.Clear();
		int num;
		if (!int.TryParse(shuffle, out num))
		{
			Debug.Log("Shuffle room failed");
			return "";
		}
		for (int i = 0; i < room.Length; i++)
		{
			int num2 = int.Parse(shuffle.Substring(i * 2 % (shuffle.Length - 1), 2));
			int index = NetworkSystem.mod("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".IndexOf(room[i]) + (encode ? num2 : (-num2)), "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length);
			NetworkSystem.shuffleStringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[index]);
		}
		return NetworkSystem.shuffleStringBuilder.ToString();
	}

	public static int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	public abstract Task AwaitSceneReady();

	public abstract string CurrentPhotonBackend { get; }

	public abstract NetPlayer GetLocalPlayer();

	public abstract NetPlayer GetPlayer(int PlayerID);

	public abstract void SetMyNickName(string name);

	public abstract string GetMyNickName();

	public abstract string GetMyDefaultName();

	public abstract string GetNickName(int playerID);

	public abstract string GetMyUserID();

	public abstract string GetUserID(int playerID);

	public abstract void SetMyTutorialComplete();

	public abstract bool GetMyTutorialCompletion();

	public abstract bool GetPlayerTutorialCompletion(int playerID);

	public void AddVoiceSettings(SO_NetworkVoiceSettings settings)
	{
		this.VoiceSettings = settings;
	}

	public abstract void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback);

	public abstract VoiceConnection VoiceConnection { get; }

	public abstract bool IsOnline { get; }

	public abstract bool InRoom { get; }

	public abstract string RoomName { get; }

	public abstract string GameModeString { get; }

	public abstract string CurrentRegion { get; }

	public abstract bool SessionIsPrivate { get; }

	public abstract int LocalPlayerID { get; }

	public abstract int MasterAuthID { get; }

	public abstract int[] AllPlayerIDs { get; }

	public NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.netPlayerCache.ToArray();
		}
	}

	protected abstract void UpdatePlayerIDCache();

	protected abstract void UpdateNetPlayerList();

	public abstract float SimTime { get; }

	public abstract float SimDeltaTime { get; }

	public abstract int SimTick { get; }

	public abstract int RoomPlayerCount { get; }

	public abstract int GlobalPlayerCount();

	public RoomConfig CurrentRoom
	{
		[CompilerGenerated]
		get
		{
			return this.<CurrentRoom>k__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			this.<CurrentRoom>k__BackingField = value;
		}
	}

	public abstract bool IsObjectLocallyOwned(GameObject obj);

	public abstract bool IsObjectRoomObject(GameObject obj);

	public abstract bool ShouldUpdateObject(GameObject obj);

	public abstract bool ShouldWriteObjectData(GameObject obj);

	public abstract int GetOwningPlayerID(GameObject obj);

	public abstract bool ShouldSpawnLocally(int playerID);

	public abstract bool IsTotalAuthority();

	protected NetworkSystem()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static NetworkSystem()
	{
	}

	public static NetworkSystem Instance;

	public NetworkSystemConfig config;

	public bool changingSceneManually;

	public string[] regionNames;

	public int currentRegionIndex;

	[CompilerGenerated]
	private bool <groupJoinInProgress>k__BackingField;

	private bool nonceRefreshed;

	protected bool isWrongVersion;

	private NetSystemState testState;

	protected int[] playerIDCache;

	protected List<NetPlayer> netPlayerCache = new List<NetPlayer>();

	protected Recorder localRecorder;

	protected Speaker localSpeaker;

	[CompilerGenerated]
	private Speaker <LocalSpeaker>k__BackingField;

	protected SO_NetworkVoiceSettings VoiceSettings;

	protected List<Action<RemoteVoiceLink>> remoteVoiceAddedCallbacks = new List<Action<RemoteVoiceLink>>();

	[CompilerGenerated]
	private Action OnMultiplayerStarted;

	[CompilerGenerated]
	private Action OnReturnedToSinglePlayer;

	[CompilerGenerated]
	private Action<int> OnPlayerJoined;

	[CompilerGenerated]
	private Action<int> OnPlayerLeft;

	protected static readonly byte[] EmptyArgs = new byte[0];

	public const string roomCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

	public const string shuffleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

	private static StringBuilder shuffleStringBuilder = new StringBuilder(4);

	[CompilerGenerated]
	private RoomConfig <CurrentRoom>k__BackingField;

	public delegate void RPC(byte[] data);

	public delegate void StringRPC(string message);

	public delegate void StaticRPC(byte[] data);

	public delegate void StaticRPCPlaceholder(byte[] args);

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

		internal bool <get_LocalPlayer>b__24_0(NetPlayer p)
		{
			return p.IsLocal;
		}

		internal void <BroadcastMyRoom>b__81_0(ExecuteCloudScriptResult result)
		{
		}

		internal void <BroadcastMyRoom>b__81_1(PlayFabError error)
		{
		}

		internal void <InstantCheckGroupData>b__82_1(PlayFabError error)
		{
			Debug.Log("ERROR - no group data found");
		}

		public static readonly NetworkSystem.<>c <>9 = new NetworkSystem.<>c();

		public static Predicate<NetPlayer> <>9__24_0;

		public static Action<ExecuteCloudScriptResult> <>9__81_0;

		public static Action<PlayFabError> <>9__81_1;

		public static Action<PlayFabError> <>9__82_1;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass82_0
	{
		public <>c__DisplayClass82_0()
		{
		}

		internal void <InstantCheckGroupData>b__0(GetSharedGroupDataResult result)
		{
			Debug.Log("Get Shared Group Data returned a success");
			Debug.Log(result.Data.ToStringFull());
			if (result.Data.Count > 0)
			{
				this.success = true;
				return;
			}
			Debug.Log("RESULT returned but no DATA");
		}

		public bool success;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass83_0
	{
		public <>c__DisplayClass83_0()
		{
		}

		internal bool <GetNetPlayerByID>b__0(NetPlayer a)
		{
			return a.ID == this.playerActorNumber;
		}

		public int playerActorNumber;
	}

	[CompilerGenerated]
	private sealed class <ReGetNonce>d__80 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <ReGetNonce>d__80(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			NetworkSystem @object = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(3f);
				this.<>1__state = 1;
				return true;
			case 1:
				this.<>1__state = -1;
				Users.GetUserProof().OnComplete(new Message<UserProof>.Callback(@object.GetOculusNonceCallback));
				this.<>2__current = null;
				this.<>1__state = 2;
				return true;
			case 2:
				this.<>1__state = -1;
				return false;
			default:
				return false;
			}
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public NetworkSystem <>4__this;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <RefreshOculusNonce>d__78 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			try
			{
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
	}
}
