using System;
using System.Collections;
using System.Collections.Generic;
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

	public virtual Speaker LocalSpeaker { get; set; }

	public event Action OnMultiplayerStarted;

	protected void MultiplayerStarted()
	{
		Action onMultiplayerStarted = this.OnMultiplayerStarted;
		if (onMultiplayerStarted == null)
		{
			return;
		}
		onMultiplayerStarted();
	}

	public event Action OnReturnedToSinglePlayer;

	protected void SinglePlayerStarted()
	{
		Action onReturnedToSinglePlayer = this.OnReturnedToSinglePlayer;
		if (onReturnedToSinglePlayer == null)
		{
			return;
		}
		onReturnedToSinglePlayer();
	}

	public event Action<int> OnPlayerJoined;

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

	public event Action<int> OnPlayerLeft;

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

	public abstract Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts);

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
			text += "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Length), 1);
		}
		if (GorillaComputer.instance.CheckAutoBanListForName(text))
		{
			return text;
		}
		return NetworkSystem.GetRandomRoomName();
	}

	public abstract string GetRandomWeightedRegion();

	protected async Task RefreshOculusNonce()
	{
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
		Debug.Log("Broadcasting room. Create: " + create.ToString());
		ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
		executeCloudScriptRequest.FunctionName = "BroadcastMyRoom";
		executeCloudScriptRequest.FunctionParameter = new
		{
			KeyToFollow = key,
			RoomToJoin = NetworkSystem.ShuffleRoomName(NetworkSystem.Instance.RoomName, shuffler, true),
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
		getSharedGroupDataRequest.Keys = new List<string> { keyToFollow };
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
		string text = "";
		int num;
		if (!int.TryParse(shuffle, out num) || shuffle.Length != room.Length * 2)
		{
			Debug.Log("Shuffle room failed");
			return "";
		}
		for (int i = 0; i < room.Length; i++)
		{
			string text2 = room.Substring(i, 1);
			int num2 = "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".IndexOf(text2);
			if (encode)
			{
				text += "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Substring(NetworkSystem.mod(num2 + int.Parse(shuffle.Substring(i * 2, 2)), "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Length), 1);
			}
			else
			{
				text += "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Substring(NetworkSystem.mod(num2 - int.Parse(shuffle.Substring(i * 2, 2)), "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Length), 1);
			}
		}
		return text;
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

	public RoomConfig CurrentRoom { get; protected set; }

	public abstract bool IsObjectLocallyOwned(GameObject obj);

	public abstract bool IsObjectRoomObject(GameObject obj);

	public abstract bool ShouldUpdateObject(GameObject obj);

	public abstract bool ShouldWriteObjectData(GameObject obj);

	public abstract int GetOwningPlayerID(GameObject obj);

	public abstract bool ShouldSpawnLocally(int playerID);

	public abstract bool IsTotalAuthority();

	public static NetworkSystem Instance;

	public NetworkSystemConfig config;

	public bool changingSceneManually;

	public string[] regionNames;

	private bool nonceRefreshed;

	protected bool isWrongVersion;

	private NetSystemState testState;

	protected int[] playerIDCache;

	protected List<NetPlayer> netPlayerCache = new List<NetPlayer>();

	protected Recorder localRecorder;

	protected Speaker localSpeaker;

	protected SO_NetworkVoiceSettings VoiceSettings;

	protected List<Action<RemoteVoiceLink>> remoteVoiceAddedCallbacks = new List<Action<RemoteVoiceLink>>();

	protected static readonly byte[] EmptyArgs = new byte[0];

	public const string roomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789";

	public delegate void RPC(byte[] data);

	public delegate void StringRPC(string message);

	public delegate void StaticRPC(byte[] data);

	public delegate void StaticRPCPlaceholder(byte[] args);
}
