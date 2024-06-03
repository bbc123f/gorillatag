using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTagScripts;
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

		public GTZone CurrentRoomZone
		{
			get
			{
				if (!(this.currentJoinTrigger != null))
				{
					return GTZone.none;
				}
				return this.currentJoinTrigger.zone;
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
			if (this.deferredJoin && Time.time >= this.partyJoinDeferredUntilTimestamp)
			{
				if ((this.partyJoinDeferredUntilTimestamp != 0f || NetworkSystem.Instance.netState == NetSystemState.Idle) && this.currentJoinTrigger != null)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
					if (this.currentJoinTrigger == this.privateTrigger)
					{
						this.AttemptToJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
						return;
					}
					this.AttemptToJoinPublicRoom(this.currentJoinTrigger, this.currentJoinType);
					return;
				}
				else if (NetworkSystem.Instance.netState != NetSystemState.PingRecon && NetworkSystem.Instance.netState != NetSystemState.Initialization)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
				}
			}
		}

		public void DeferJoining(float duration)
		{
			this.partyJoinDeferredUntilTimestamp = Mathf.Max(this.partyJoinDeferredUntilTimestamp, Time.time + duration);
		}

		public void ClearDeferredJoin()
		{
			this.partyJoinDeferredUntilTimestamp = 0f;
			this.deferredJoin = false;
		}

		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo)
		{
			this.AttemptToJoinPublicRoomAsync(triggeredTrigger, roomJoinType);
		}

		private void AttemptToJoinPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType)
		{
			PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__64 <AttemptToJoinPublicRoomAsync>d__;
			<AttemptToJoinPublicRoomAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AttemptToJoinPublicRoomAsync>d__.<>4__this = this;
			<AttemptToJoinPublicRoomAsync>d__.triggeredTrigger = triggeredTrigger;
			<AttemptToJoinPublicRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinPublicRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinPublicRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__64>(ref <AttemptToJoinPublicRoomAsync>d__);
		}

		private Task SendPartyFollowCommands()
		{
			PhotonNetworkController.<SendPartyFollowCommands>d__65 <SendPartyFollowCommands>d__;
			<SendPartyFollowCommands>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendPartyFollowCommands>d__.<>1__state = -1;
			<SendPartyFollowCommands>d__.<>t__builder.Start<PhotonNetworkController.<SendPartyFollowCommands>d__65>(ref <SendPartyFollowCommands>d__);
			return <SendPartyFollowCommands>d__.<>t__builder.Task;
		}

		public void AttemptToJoinSpecificRoom(string roomID, JoinType roomJoinType)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType);
		}

		public Task AttemptToJoinSpecificRoomAsync(string roomID, JoinType roomJoinType)
		{
			PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__67 <AttemptToJoinSpecificRoomAsync>d__;
			<AttemptToJoinSpecificRoomAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AttemptToJoinSpecificRoomAsync>d__.<>4__this = this;
			<AttemptToJoinSpecificRoomAsync>d__.roomID = roomID;
			<AttemptToJoinSpecificRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinSpecificRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinSpecificRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__67>(ref <AttemptToJoinSpecificRoomAsync>d__);
			return <AttemptToJoinSpecificRoomAsync>d__.<>t__builder.Task;
		}

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
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.offlineVRRig)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			if (GorillaComputer.instance != null && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateTriggerScreens();
			}
			Player.Instance.maxJumpSpeed = 6.5f;
			Player.Instance.jumpMultiplier = 1.1f;
			GorillaNot.instance.currentMasterClient = null;
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
			this.initialGameMode = "";
		}

		public void OnJoinedRoom()
		{
			if (NetworkSystem.Instance.GameModeString.IsNullOrEmpty())
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			this.initialGameMode = NetworkSystem.Instance.GameModeString;
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.currentJoinTrigger = this.privateTrigger;
				PhotonNetworkController.Instance.UpdateTriggerScreens();
			}
			else if (this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.FollowingNearby)
			{
				bool flag = false;
				for (int i = 0; i < GorillaComputer.instance.allowedMapsToJoin.Length; i++)
				{
					if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.allowedMapsToJoin[i]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
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
			GameObject gameObject;
			VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", out gameObject);
			if (gameObject == null)
			{
				Debug.LogError("OnJoinedRoom: Unable to find player prefab to spawn");
				return;
			}
			NetworkSystem.Instance.NetInstantiate(gameObject, this.playerOffset.transform.position, this.playerOffset.transform.rotation, false);
			GorillaComputer.instance.roomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.ForceJoinWithParty)
			{
				this.keyToFollow = NetworkSystem.Instance.LocalPlayer.UserId + this.keyStr;
				NetworkSystem.Instance.BroadcastMyRoom(true, this.keyToFollow, this.shuffler);
			}
			GorillaNot.instance.currentMasterClient = null;
			this.UpdateCurrentJoinTrigger();
			this.UpdateTriggerScreens();
			GorillaScoreboardTotalUpdater.instance.JoinedRoom();
		}

		public void RegisterJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.allJoinTriggers.Add(trigger);
		}

		private void UpdateCurrentJoinTrigger()
		{
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.forestMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.forestMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.caveMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.caveMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.canyonMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.canyonMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.cityMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.cityMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.mountainMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.mountainMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.skyjungleMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.skyjungleMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.basementMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.basementMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.beachMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.beachMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.GameModeString.Contains(GorillaComputer.instance.rotatingMapTrigger.gameModeName))
			{
				this.currentJoinTrigger = GorillaComputer.instance.rotatingMapTrigger;
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				if (this.currentJoinTrigger != this.privateTrigger)
				{
					Debug.LogError("IN a private game but private trigger isnt current");
					return;
				}
			}
			else
			{
				Debug.LogError("Not in private room and unabel tp update jointrigger.");
			}
		}

		public void UpdateTriggerScreens()
		{
			foreach (GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger in this.allJoinTriggers)
			{
				gorillaNetworkJoinTrigger.UpdateUI();
			}
		}

		public void AttemptToFollowIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr, JoinType joinType)
		{
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = userIDToFollow + newKeyStr;
			this.shuffler = shufflerStr;
			this.currentJoinType = joinType;
			this.ClearDeferredJoin();
			if (NetworkSystem.Instance.InRoom)
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
				text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
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

		private JoinType currentJoinType;

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

		public string autoJoinRoom;

		private bool deferredJoin;

		private float partyJoinDeferredUntilTimestamp;

		private DateTime? timeWhenApplicationPaused;

		[NetworkPrefab]
		[SerializeField]
		private NetworkObject testPlayerPrefab;

		private List<GorillaNetworkJoinTrigger> allJoinTriggers = new List<GorillaNetworkJoinTrigger>();

		[CompilerGenerated]
		[StructLayout(LayoutKind.Auto)]
		private struct <AttemptToJoinPublicRoomAsync>d__64 : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num = this.<>1__state;
				PhotonNetworkController photonNetworkController = this.<>4__this;
				try
				{
					TaskAwaiter<NetJoinResult> awaiter;
					TaskAwaiter awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = this.<>u__2;
							this.<>u__2 = default(TaskAwaiter<NetJoinResult>);
							this.<>1__state = -1;
							goto IL_2BA;
						}
						if (NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting)
						{
							goto IL_2DD;
						}
						if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon || Time.time < photonNetworkController.partyJoinDeferredUntilTimestamp)
						{
							photonNetworkController.currentJoinTrigger = this.triggeredTrigger;
							photonNetworkController.currentJoinType = this.roomJoinType;
							photonNetworkController.deferredJoin = true;
							goto IL_2DD;
						}
						photonNetworkController.deferredJoin = false;
						if (NetworkSystem.Instance.InRoom)
						{
							if (NetworkSystem.Instance.SessionIsPrivate)
							{
								if (this.roomJoinType != JoinType.JoinWithNearby && this.roomJoinType != JoinType.ForceJoinWithParty)
								{
									goto IL_2DD;
								}
							}
							else if (NetworkSystem.Instance.GameModeString.Contains(this.triggeredTrigger.gameModeName))
							{
								goto IL_2DD;
							}
						}
						if (this.roomJoinType != JoinType.JoinWithParty && this.roomJoinType != JoinType.ForceJoinWithParty)
						{
							goto IL_14B;
						}
						awaiter2 = photonNetworkController.SendPartyFollowCommands().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							this.<>1__state = 0;
							this.<>u__1 = awaiter2;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__64>(ref awaiter2, ref this);
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
					IL_14B:
					photonNetworkController.currentJoinTrigger = this.triggeredTrigger;
					photonNetworkController.currentJoinType = this.roomJoinType;
					if (PlayFabClientAPI.IsClientLoggedIn())
					{
						photonNetworkController.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
					}
					RoomConfig roomConfig = RoomConfig.AnyPublicConfig();
					if (photonNetworkController.currentJoinType == JoinType.JoinWithNearby)
					{
						roomConfig.SetFriendIDs(photonNetworkController.friendIDList);
					}
					else if (photonNetworkController.currentJoinType == JoinType.JoinWithParty || photonNetworkController.currentJoinType == JoinType.ForceJoinWithParty)
					{
						roomConfig.SetFriendIDs(FriendshipGroupDetection.Instance.PartyMemberIDs.ToList<string>());
					}
					string gameModeName = photonNetworkController.currentJoinTrigger.gameModeName;
					string currentQueue = GorillaComputer.instance.currentQueue;
					string text = (photonNetworkController.currentJoinTrigger.gameModeName == "city" || photonNetworkController.currentJoinTrigger.gameModeName == "basement") ? "CASUAL" : GorillaComputer.instance.currentGameMode;
					string value = gameModeName + currentQueue + ((text != null) ? text.ToString() : null);
					Hashtable customProps = new Hashtable
					{
						{
							"gameMode",
							value
						}
					};
					roomConfig.customProps = customProps;
					roomConfig.MaxPlayers = photonNetworkController.GetRoomSize(photonNetworkController.currentJoinTrigger.gameModeName);
					awaiter = NetworkSystem.Instance.ConnectToRoom(null, roomConfig, -1).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 1;
						this.<>u__2 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NetJoinResult>, PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__64>(ref awaiter, ref this);
						return;
					}
					IL_2BA:
					awaiter.GetResult();
				}
				catch (Exception exception)
				{
					this.<>1__state = -2;
					this.<>t__builder.SetException(exception);
					return;
				}
				IL_2DD:
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

			public PhotonNetworkController <>4__this;

			public GorillaNetworkJoinTrigger triggeredTrigger;

			public JoinType roomJoinType;

			private TaskAwaiter <>u__1;

			private TaskAwaiter<NetJoinResult> <>u__2;
		}

		[CompilerGenerated]
		[StructLayout(LayoutKind.Auto)]
		private struct <AttemptToJoinSpecificRoomAsync>d__67 : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num = this.<>1__state;
				PhotonNetworkController photonNetworkController = this.<>4__this;
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
						{
							photonNetworkController.deferredJoin = true;
							photonNetworkController.customRoomID = this.roomID;
							photonNetworkController.currentJoinType = this.roomJoinType;
							photonNetworkController.currentJoinTrigger = photonNetworkController.privateTrigger;
							goto IL_21D;
						}
						if (NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame)
						{
							goto IL_21D;
						}
						photonNetworkController.customRoomID = this.roomID;
						photonNetworkController.currentJoinType = this.roomJoinType;
						photonNetworkController.currentJoinTrigger = photonNetworkController.privateTrigger;
						photonNetworkController.deferredJoin = false;
						if (photonNetworkController.currentJoinType != JoinType.JoinWithParty && photonNetworkController.currentJoinType != JoinType.ForceJoinWithParty)
						{
							goto IL_117;
						}
						awaiter = photonNetworkController.SendPartyFollowCommands().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							this.<>1__state = 0;
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__67>(ref awaiter, ref this);
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
					IL_117:
					string gameModeName = photonNetworkController.currentJoinTrigger.gameModeName;
					string currentQueue = GorillaComputer.instance.currentQueue;
					string text = (photonNetworkController.currentJoinTrigger.gameModeName == "city" || photonNetworkController.currentJoinTrigger.gameModeName == "basement") ? "CASUAL" : GorillaComputer.instance.currentGameMode;
					string value = gameModeName + currentQueue + ((text != null) ? text.ToString() : null);
					Hashtable customProps = new Hashtable
					{
						{
							"gameMode",
							value
						}
					};
					RoomConfig roomConfig = new RoomConfig();
					roomConfig.createIfMissing = true;
					roomConfig.isJoinable = true;
					roomConfig.isPublic = false;
					roomConfig.MaxPlayers = photonNetworkController.GetRoomSize(photonNetworkController.currentJoinTrigger.gameModeName);
					roomConfig.customProps = customProps;
					if (PlayFabClientAPI.IsClientLoggedIn())
					{
						photonNetworkController.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
					}
					NetworkSystem.Instance.ConnectToRoom(this.roomID, roomConfig, -1);
				}
				catch (Exception exception)
				{
					this.<>1__state = -2;
					this.<>t__builder.SetException(exception);
					return;
				}
				IL_21D:
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

			public PhotonNetworkController <>4__this;

			public string roomID;

			public JoinType roomJoinType;

			private TaskAwaiter <>u__1;
		}

		[CompilerGenerated]
		private sealed class <DisableOnStart>d__59 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <DisableOnStart>d__59(int <>1__state)
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

		[CompilerGenerated]
		[StructLayout(LayoutKind.Auto)]
		private struct <SendPartyFollowCommands>d__65 : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num = this.<>1__state;
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
						PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
						RoomSystem.SendPartyFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
						PhotonNetwork.SendAllOutgoingCommands();
						awaiter = Task.Delay(1).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							this.<>1__state = 0;
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PhotonNetworkController.<SendPartyFollowCommands>d__65>(ref awaiter, ref this);
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

			public AsyncTaskMethodBuilder <>t__builder;

			private TaskAwaiter <>u__1;
		}
	}
}
