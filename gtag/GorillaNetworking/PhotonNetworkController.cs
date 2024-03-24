using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using Photon.Pun;
using PlayFab;
using UnityEngine;
using WebSocketSharp;

namespace GorillaNetworking
{
	public class PhotonNetworkController : MonoBehaviour
	{
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

		public GorillaGeoHideShowTrigger StartGeoTrigger
		{
			get
			{
				return this.startGeoTrigger;
			}
			set
			{
				this.startGeoTrigger = value;
			}
		}

		public void Awake()
		{
			if (PhotonNetworkController.Instance == null)
			{
				PhotonNetworkController.Instance = this;
			}
			else if (PhotonNetworkController.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.updatedName = false;
			this.playersInRegion = new int[this.serverRegions.Length];
			this.pingInRegion = new int[this.serverRegions.Length];
		}

		public void Start()
		{
			base.StartCoroutine(this.DisableOnStart());
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
		}

		private IEnumerator DisableOnStart()
		{
			ZoneManagement.SetActiveZone(this.StartZone);
			yield break;
		}

		public void FixedUpdate()
		{
			this.headRightHandDistance = (Player.Instance.headCollider.transform.position - Player.Instance.rightControllerTransform.position).magnitude;
			this.headLeftHandDistance = (Player.Instance.headCollider.transform.position - Player.Instance.leftControllerTransform.position).magnitude;
			this.headQuat = Player.Instance.headCollider.transform.rotation;
			if (!this.disableAFKKick && Quaternion.Angle(this.headQuat, this.lastHeadQuat) <= 0.01f && Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) < 0.001f && Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) < 0.001f && this.pauseTime + this.disconnectTime < Time.realtimeSinceStartup)
			{
				this.pauseTime = Time.realtimeSinceStartup;
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			else if (Quaternion.Angle(this.headQuat, this.lastHeadQuat) > 0.01f || Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) >= 0.001f || Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) >= 0.001f)
			{
				this.pauseTime = Time.realtimeSinceStartup;
			}
			this.lastHeadRightHandDistance = this.headRightHandDistance;
			this.lastHeadLeftHandDistance = this.headLeftHandDistance;
			this.lastHeadQuat = this.headQuat;
			if (this.deferredJoin)
			{
				if (NetworkSystem.Instance.netState == NetSystemState.Idle && this.currentJoinTrigger != null)
				{
					this.deferredJoin = false;
					if (this.currentJoinTrigger == this.privateTrigger)
					{
						this.AttemptToJoinSpecificRoom(this.customRoomID);
						return;
					}
					this.AttemptToJoinPublicRoom(this.currentJoinTrigger, this.joiningWithFriend);
					return;
				}
				else if (NetworkSystem.Instance.netState != NetSystemState.PingRecon && NetworkSystem.Instance.netState != NetSystemState.Initialization)
				{
					this.deferredJoin = false;
				}
			}
		}

		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, bool joinWithFriends = false)
		{
			if (NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting)
			{
				Debug.Log("Cant join Public room, Still connecting or disconnecting");
				return;
			}
			if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
			{
				Debug.Log("Can't join public room, still connecting, but will try later");
				this.currentJoinTrigger = triggeredTrigger;
				this.joiningWithFriend = joinWithFriends;
				this.deferredJoin = true;
				return;
			}
			this.deferredJoin = false;
			Debug.Log("Attempting To Join public room.");
			Debug.Log("Joining with friends: " + joinWithFriends.ToString());
			if (this.joiningWithFriend)
			{
				string text = "Joining with: ";
				foreach (string text2 in this.friendIDList)
				{
					text = text + text2 + ", ";
				}
				Debug.Log(text);
			}
			if (NetworkSystem.Instance.InRoom && !joinWithFriends)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					Debug.Log("In a private room, dont want to join a pub.");
					return;
				}
				Debug.Log("Current gamemode string is: " + NetworkSystem.Instance.GameModeString);
				Debug.Log("Triggered gamemode is " + triggeredTrigger.gameModeName);
				if (NetworkSystem.Instance.GameModeString.Contains(triggeredTrigger.gameModeName))
				{
					Debug.Log("Already in triggered gamemode, staying in current room");
					return;
				}
				Debug.Log("Joining a new public room as selected gamemode is different to current rooms mode or joining with friends!");
			}
			Debug.Log("Joining a public room!");
			this.currentJoinTrigger = triggeredTrigger;
			this.joiningWithFriend = joinWithFriends;
			if (PlayFabClientAPI.IsClientLoggedIn())
			{
				this.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
			}
			RoomConfig roomConfig = RoomConfig.AnyPublicConfig();
			if (this.joiningWithFriend)
			{
				roomConfig.SetFriendIDs(this.friendIDList);
			}
			string gameModeName = this.currentJoinTrigger.gameModeName;
			string currentQueue = GorillaComputer.instance.currentQueue;
			string text3 = ((this.currentJoinTrigger.gameModeName == "city" || this.currentJoinTrigger.gameModeName == "basement") ? "CASUAL" : GorillaComputer.instance.currentGameMode);
			string text4 = gameModeName + currentQueue + ((text3 != null) ? text3.ToString() : null);
			Hashtable hashtable = new Hashtable { { "gameMode", text4 } };
			roomConfig.customProps = hashtable;
			roomConfig.MaxPlayers = this.GetRoomSize(this.currentJoinTrigger.gameModeName);
			Debug.Log("DEBUG GAMEMODE: " + text4);
			NetworkSystem.Instance.ConnectToRoom(null, roomConfig);
		}

		public void AttemptToJoinSpecificRoom(string roomID)
		{
			if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
			{
				Debug.Log("Can't join private room, still connecting, but will try later");
				this.deferredJoin = true;
				this.customRoomID = roomID;
				this.joiningWithFriend = false;
				this.currentJoinTrigger = this.privateTrigger;
				return;
			}
			if (NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame)
			{
				return;
			}
			this.customRoomID = roomID;
			this.joiningWithFriend = false;
			this.currentJoinTrigger = this.privateTrigger;
			this.deferredJoin = false;
			string gameModeName = this.currentJoinTrigger.gameModeName;
			string currentQueue = GorillaComputer.instance.currentQueue;
			string text = ((this.currentJoinTrigger.gameModeName == "city" || this.currentJoinTrigger.gameModeName == "basement") ? "CASUAL" : GorillaComputer.instance.currentGameMode);
			string text2 = gameModeName + currentQueue + ((text != null) ? text.ToString() : null);
			Hashtable hashtable = new Hashtable { { "gameMode", text2 } };
			RoomConfig roomConfig = new RoomConfig();
			roomConfig.createIfMissing = true;
			roomConfig.isJoinable = true;
			roomConfig.isPublic = false;
			roomConfig.MaxPlayers = this.GetRoomSize(this.currentJoinTrigger.gameModeName);
			roomConfig.customProps = hashtable;
			if (PlayFabClientAPI.IsClientLoggedIn())
			{
				this.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
			}
			NetworkSystem.Instance.ConnectToRoom(roomID, roomConfig);
		}

		private void DisconnectCleanup()
		{
			Debug.Log("Disconnect cleanup running!");
			if (GorillaParent.instance != null)
			{
				GorillaScoreboardSpawner[] componentsInChildren = GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnLeftRoom();
				}
			}
			this.attemptingToConnect = true;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.offlineVRRig)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			if (GorillaComputer.instance != null && !ApplicationQuittingState.IsQuitting)
			{
				foreach (GorillaLevelScreen gorillaLevelScreen in GorillaComputer.instance.levelScreens)
				{
					gorillaLevelScreen.UpdateText(gorillaLevelScreen.startingText, true);
				}
			}
			Player.Instance.maxJumpSpeed = 6.5f;
			Player.Instance.jumpMultiplier = 1.1f;
			GorillaNot.instance.currentMasterClient = null;
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
			this.initialGameMode = "";
		}

		public void OnJoinedRoom()
		{
			Debug.Log("PhotonNetworkController: On Joined Room!");
			if (NetworkSystem.Instance.GameModeString.IsNullOrEmpty())
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			this.initialGameMode = NetworkSystem.Instance.GameModeString;
			if (NetworkSystem.Instance.SessionIsPrivate)
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
					Debug.Log(GorillaComputer.instance.allowedMapsToJoin[j]);
					Debug.Log("Checking allowed in room check, Gamemode: " + NetworkSystem.Instance.GameModeString);
					if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.allowedMapsToJoin[j]))
					{
						this.allowedInPubRoom = true;
						break;
					}
				}
				if (!this.allowedInPubRoom)
				{
					Debug.Log("NOT ALLOWED IN ROOM");
					GorillaComputer.instance.roomNotAllowed = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
					return;
				}
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GorillaGameModes.GameMode.LoadGameModeFromProperty(this.initialGameMode);
			}
			NetworkSystem.Instance.SetMyTutorialComplete();
			Debug.Log("Spawning player");
			GameObject gameObject;
			VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", out gameObject);
			if (gameObject == null)
			{
				Debug.LogError("Unable to find player prefab to spawn");
				return;
			}
			NetworkSystem.Instance.NetInstantiate(gameObject, this.playerOffset.transform.position, this.playerOffset.transform.rotation, false);
			GorillaComputer.instance.roomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			if (this.joiningWithFriend)
			{
				NetworkSystem.Instance.BroadcastMyRoom(true, this.keyToFollow, this.shuffler);
			}
			GorillaNot.instance.currentMasterClient = null;
			this.UpdateTriggerScreens();
			GorillaScoreboardTotalUpdater.instance.JoinedRoom();
		}

		private void UpdateTriggerScreens()
		{
			Debug.Log(NetworkSystem.Instance.GameModeString);
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.forestMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.forestMapTrigger;
			}
			else if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.caveMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.caveMapTrigger;
			}
			else if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.canyonMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.canyonMapTrigger;
			}
			else if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.cityMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.cityMapTrigger;
			}
			else if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.mountainMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.mountainMapTrigger;
			}
			else if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.skyjungleMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.skyjungleMapTrigger;
			}
			else if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.basementMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.basementMapTrigger;
			}
			else if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.beachMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.beachMapTrigger;
			}
			else if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.rotatingMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.rotatingMapTrigger;
			}
			else if (NetworkSystem.Instance.SessionIsPrivate)
			{
				if (this.currentJoinTrigger != this.privateTrigger)
				{
					Debug.LogError("IN a private game but private trigger isnt current");
				}
			}
			else
			{
				Debug.LogError("Not in private room and unabel tp update jointrigger.");
			}
			this.currentJoinTrigger.UpdateScreens();
		}

		public void AttemptJoinPublicWithFriends(GorillaNetworkJoinTrigger triggeredTrigger)
		{
			this.currentJoinTrigger = triggeredTrigger;
			this.joiningWithFriend = true;
			this.keyToFollow = NetworkSystem.Instance.LocalPlayerID.ToString() + this.keyStr;
			NetworkSystem.Instance.BroadcastMyRoom(false, this.keyToFollow, this.shuffler);
			this.AttemptToJoinPublicRoom(this.currentJoinTrigger, true);
		}

		public void AttemptToFollowFriendIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr)
		{
			Debug.Log("Following friend into pub");
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = actorNumberToFollow.ToString() + newKeyStr;
			this.shuffler = shufflerStr;
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
			{
				NetworkSystem.Instance.JoinFriendsRoom(this.friendToFollow, actorNumberToFollow, this.keyToFollow, this.shuffler);
			}
		}

		public void OnDisconnected()
		{
			this.DisconnectCleanup();
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
				text += "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNOPQRSTUVWXYX123456789".Length), 1);
			}
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				return text;
			}
			return this.RandomRoomName();
		}

		public byte GetRoomSize(string gameModeName)
		{
			if (gameModeName.Contains("ball"))
			{
				return 5;
			}
			return 10;
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
			if (NetworkSystem.Instance == null)
			{
				Debug.Log("Null netsys!!!");
			}
			return NetworkSystem.Instance.netState.ToString();
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				this.timeWhenApplicationPaused = new DateTime?(DateTime.Now);
				return;
			}
			if ((DateTime.Now - (this.timeWhenApplicationPaused ?? DateTime.Now)).TotalSeconds > (double)this.disconnectTime)
			{
				this.timeWhenApplicationPaused = null;
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance != null)
				{
					instance.ReturnToSinglePlayer();
				}
			}
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance2 = NetworkSystem.Instance;
				if (instance2 == null)
				{
					return;
				}
				instance2.ReturnToSinglePlayer();
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			if (!focus && NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance == null)
				{
					return;
				}
				instance.ReturnToSinglePlayer();
			}
		}

		public PhotonNetworkController()
		{
		}

		public static volatile PhotonNetworkController Instance;

		public int incrementCounter;

		public PlayFabAuthenticator playFabAuthenticator;

		public string[] serverRegions;

		public bool isPrivate;

		public string customRoomID;

		public GameObject playerOffset;

		public SkinnedMeshRenderer[] offlineVRRig;

		public bool attemptingToConnect;

		private int currentRegionIndex;

		public string currentGameType;

		public bool roomCosmeticsInitialized;

		public GameObject photonVoiceObjectPrefab;

		public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

		private bool pastFirstConnection;

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

		public bool updatedName;

		private int[] playersInRegion;

		private int[] pingInRegion;

		public List<string> friendIDList = new List<string>();

		private bool joiningWithFriend;

		private string friendToFollow;

		private string keyToFollow;

		public string shuffler;

		public string keyStr;

		private string startLevel;

		private GTZone startZone;

		private GorillaGeoHideShowTrigger startGeoTrigger;

		public GorillaNetworkJoinTrigger privateTrigger;

		internal string initialGameMode = "";

		public GorillaNetworkJoinTrigger currentJoinTrigger;

		public bool allowedInPubRoom;

		public string autoJoinRoom;

		private bool deferredJoin;

		private DateTime? timeWhenApplicationPaused;

		[NetworkPrefab]
		[SerializeField]
		private NetworkObject testPlayerPrefab;

		[CompilerGenerated]
		private sealed class <DisableOnStart>d__57 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <DisableOnStart>d__57(int <>1__state)
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
				PhotonNetworkController photonNetworkController = this;
				if (num != 0)
				{
					return false;
				}
				this.<>1__state = -1;
				ZoneManagement.SetActiveZone(photonNetworkController.StartZone);
				return false;
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

			public PhotonNetworkController <>4__this;
		}
	}
}
