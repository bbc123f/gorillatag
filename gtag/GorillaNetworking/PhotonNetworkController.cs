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

namespace GorillaNetworking;

public class PhotonNetworkController : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
	public enum ConnectionState
	{
		Initialization,
		WrongVersion,
		DeterminingPingsAndPlayerCount,
		ConnectedAndWaiting,
		DisconnectingFromRoom,
		JoiningPublicRoom,
		JoiningSpecificRoom,
		JoiningFriend,
		InPrivateRoom,
		InPublicRoom
	}

	public enum ConnectionEvent
	{
		InitialConnection,
		OnConnectedToMaster,
		AttemptJoinPublicRoom,
		AttemptJoinSpecificRoom,
		AttemptToCreateRoom,
		Disconnect,
		OnJoinedRoom,
		OnJoinRoomFailed,
		OnJoinRandomFailed,
		OnCreateRoomFailed,
		OnDisconnected,
		FoundFriendToJoin,
		FollowFriendToPub
	}

	public static volatile PhotonNetworkController Instance;

	public int incrementCounter;

	private ConnectionState currentState;

	public PlayFabAuthenticator playFabAuthenticator;

	public string[] serverRegions;

	private string gameVersionType = "live1";

	private int majorVersion = 1;

	private int minorVersion = 1;

	private int minorVersion2 = 54;

	private string _gameVersionString = "";

	public bool isPrivate;

	public string customRoomID;

	public GameObject playerOffset;

	public SkinnedMeshRenderer[] offlineVRRig;

	public bool attemptingToConnect;

	private int currentRegionIndex;

	public string currentGameType;

	public bool wrongVersion;

	public bool roomCosmeticsInitialized;

	public GameObject photonVoiceObjectPrefab;

	public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

	private bool pastFirstConnection;

	private float timeToRetryWithBackoff = 0.1f;

	private bool retry;

	private float lastTimeConnected;

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

	public string roomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";

	public bool updatedName;

	private int[] playersInRegion;

	private int[] pingInRegion;

	public List<string> friendIDList = new List<string>();

	private bool successfullyFoundFriend;

	private float startingToLookForFriend;

	private float timeToSpendLookingForFriend = 15f;

	private bool joiningWithFriend;

	private string friendToFollow;

	private string keyToFollow;

	private int actorIdToFollow;

	public string shuffler;

	public string keyStr;

	private Photon.Realtime.Player tempPlayer;

	private bool isRoomFull;

	private bool doesRoomExist;

	private bool createRoom;

	private string startLevel;

	private GTZone startZone;

	public GorillaNetworkJoinTrigger privateTrigger;

	internal string initialGameMode = "";

	public GorillaNetworkJoinTrigger currentJoinTrigger;

	public bool allowedInPubRoom;

	public string GameVersionType => gameVersionType;

	public int GameMajorVersion => majorVersion;

	public int GameMinorVersion => minorVersion;

	public int GameMinorVersion2 => minorVersion2;

	public string StartLevel
	{
		get
		{
			return startLevel;
		}
		set
		{
			startLevel = value;
		}
	}

	public GTZone StartZone
	{
		get
		{
			return startZone;
		}
		set
		{
			startZone = value;
		}
	}

	public string GameVersionString
	{
		get
		{
			return _gameVersionString;
		}
		set
		{
			_gameVersionString = "MODDED";
		}
	}

	public void FullDisconnect()
	{
		currentState = ConnectionState.Initialization;
		PhotonNetwork.Disconnect();
	}

	public void InitiateConnection()
	{
		ProcessState(ConnectionEvent.InitialConnection);
	}

	public void Awake()
	{
		_gameVersionString = gameVersionType + "." + majorVersion + "." + minorVersion + "." + minorVersion2;
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		updatedName = false;
		roomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
		playersInRegion = new int[serverRegions.Length];
		pingInRegion = new int[serverRegions.Length];
	}

	public override void OnCustomAuthenticationFailed(string debugMessage)
	{
		retry = true;
		if (timeToRetryWithBackoff < 1f)
		{
			timeToRetryWithBackoff = 1f;
		}
		Debug.Log("auth failed, backing off connecting, with message: " + debugMessage);
	}

	public void Start()
	{
		StartCoroutine(DisableOnStart());
		currentState = ConnectionState.Initialization;
		PhotonNetwork.EnableCloseConnection = false;
		PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
	}

	private IEnumerator DisableOnStart()
	{
		ZoneManagement.SetActiveZone(StartZone);
		yield break;
	}

	public void FixedUpdate()
	{
		headRightHandDistance = (GorillaLocomotion.Player.Instance.headCollider.transform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.position).magnitude;
		headLeftHandDistance = (GorillaLocomotion.Player.Instance.headCollider.transform.position - GorillaLocomotion.Player.Instance.leftControllerTransform.position).magnitude;
		headQuat = GorillaLocomotion.Player.Instance.headCollider.transform.rotation;
		if (!disableAFKKick && Quaternion.Angle(headQuat, lastHeadQuat) <= 0.01f && Mathf.Abs(headRightHandDistance - lastHeadRightHandDistance) < 0.001f && Mathf.Abs(headLeftHandDistance - lastHeadLeftHandDistance) < 0.001f && pauseTime + disconnectTime < Time.realtimeSinceStartup)
		{
			pauseTime = Time.realtimeSinceStartup;
			ProcessState(ConnectionEvent.Disconnect);
		}
		else if (Quaternion.Angle(headQuat, lastHeadQuat) > 0.01f || Mathf.Abs(headRightHandDistance - lastHeadRightHandDistance) >= 0.001f || Mathf.Abs(headLeftHandDistance - lastHeadLeftHandDistance) >= 0.001f)
		{
			pauseTime = Time.realtimeSinceStartup;
		}
		lastHeadRightHandDistance = headRightHandDistance;
		lastHeadLeftHandDistance = headLeftHandDistance;
		lastHeadQuat = headQuat;
	}

	private void ProcessInitializationState(ConnectionEvent connectionEvent)
	{
		if (connectionEvent == ConnectionEvent.InitialConnection)
		{
			PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = GameVersionString;
			currentRegionIndex = 0;
			if (PlayerPrefs.GetString("playerName") != "")
			{
				PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("playerName");
			}
			else
			{
				PhotonNetwork.LocalPlayer.NickName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			}
			PhotonNetwork.AutomaticallySyncScene = false;
			currentState = ConnectionState.DeterminingPingsAndPlayerCount;
			ConnectToRegion(serverRegions[currentRegionIndex]);
		}
		else
		{
			InvalidState(connectionEvent);
		}
	}

	private void ProcessDeterminingPingsAndPlayerCountState(ConnectionEvent connectionEvent)
	{
		switch (connectionEvent)
		{
		case ConnectionEvent.OnConnectedToMaster:
		{
			int ping = PhotonNetwork.GetPing();
			Debug.Log("current ping is " + ping + " on region " + serverRegions[currentRegionIndex] + ". player count is " + PhotonNetwork.CountOfPlayers);
			GorillaComputer.instance.screenChanged = true;
			playersInRegion[currentRegionIndex] = PhotonNetwork.CountOfPlayers;
			pingInRegion[currentRegionIndex] = ping;
			PhotonNetwork.Disconnect();
			break;
		}
		case ConnectionEvent.OnDisconnected:
			currentRegionIndex++;
			if (currentRegionIndex >= serverRegions.Length)
			{
				Debug.Log("checked all servers. connecting to server with best ping: " + GetRegionWithLowestPing());
				currentState = ConnectionState.ConnectedAndWaiting;
				GorillaComputer.instance.screenChanged = true;
				if (currentJoinTrigger != null)
				{
					AttemptToJoinPublicRoom(currentJoinTrigger);
				}
				ConnectToRegion(GetRegionWithLowestPing());
			}
			else
			{
				Debug.Log("checking next region");
				ConnectToRegion(serverRegions[currentRegionIndex]);
			}
			break;
		default:
			InvalidState(connectionEvent);
			break;
		}
	}

	private void ProcessConnectedAndWaitingState(ConnectionEvent connectionEvent)
	{
		switch (connectionEvent)
		{
		case ConnectionEvent.Disconnect:
			PhotonNetwork.Disconnect();
			break;
		case ConnectionEvent.OnDisconnected:
			Debug.Log("not sure what happened, reconnecting to region with best ping");
			retry = true;
			ConnectToRegion(GetRegionWithLowestPing());
			break;
		case ConnectionEvent.AttemptJoinPublicRoom:
			currentState = ConnectionState.JoiningPublicRoom;
			ProcessState(ConnectionEvent.AttemptJoinPublicRoom);
			break;
		case ConnectionEvent.AttemptJoinSpecificRoom:
			currentState = ConnectionState.JoiningSpecificRoom;
			ProcessState(ConnectionEvent.AttemptJoinSpecificRoom);
			break;
		default:
			InvalidState(connectionEvent);
			break;
		}
	}

	private void ProcessDisconnectingFromRoomState(ConnectionEvent connectionEvent)
	{
		switch (connectionEvent)
		{
		case ConnectionEvent.Disconnect:
			PhotonNetwork.Disconnect();
			break;
		case ConnectionEvent.OnConnectedToMaster:
			Debug.Log("successfully reconnected to master. waiting on what to do next.");
			currentState = ConnectionState.ConnectedAndWaiting;
			break;
		case ConnectionEvent.OnDisconnected:
			Debug.Log("just disconnected while trying to disconnect. attempting to reconnect to best region.");
			currentState = ConnectionState.ConnectedAndWaiting;
			ConnectToRegion(GetRegionWithLowestPing());
			break;
		default:
			InvalidState(connectionEvent);
			break;
		}
	}

	private void ProcessJoiningPublicRoomState(ConnectionEvent connectionEvent)
	{
		switch (connectionEvent)
		{
		case ConnectionEvent.Disconnect:
			DisconnectFromRoom(ConnectionState.DisconnectingFromRoom);
			break;
		case ConnectionEvent.OnDisconnected:
			if (!joiningWithFriend)
			{
				if (!pastFirstConnection)
				{
					pastFirstConnection = true;
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = GetRegionWithLowestPing();
				}
				else
				{
					float value = Random.value;
					int num = 0;
					for (int i = 0; i < playersInRegion.Length; i++)
					{
						num += playersInRegion[i];
					}
					float num2 = 0f;
					int num3;
					for (num3 = -1; num2 < value; num2 += (float)playersInRegion[num3] / (float)num)
					{
						if (num3 >= playersInRegion.Length - 1)
						{
							break;
						}
						num3++;
					}
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = serverRegions[num3];
				}
			}
			StartCoroutine(ConnectUsingSettingsWithBackoff());
			break;
		case ConnectionEvent.OnConnectedToMaster:
			JoinPublicRoom(joiningWithFriend);
			break;
		case ConnectionEvent.AttemptJoinPublicRoom:
			if (!pastFirstConnection)
			{
				JoinPublicRoom(joiningWithFriend);
			}
			else
			{
				PhotonNetwork.Disconnect();
			}
			break;
		case ConnectionEvent.AttemptJoinSpecificRoom:
			JoinSpecificRoom();
			break;
		case ConnectionEvent.OnJoinedRoom:
			pastFirstConnection = true;
			currentJoinTrigger.UpdateScreens();
			currentState = ConnectionState.InPublicRoom;
			break;
		case ConnectionEvent.OnJoinRandomFailed:
			CreatePublicRoom(joiningWithFriend);
			break;
		case ConnectionEvent.OnCreateRoomFailed:
			CreatePublicRoom(joiningWithFriend);
			break;
		default:
			InvalidState(connectionEvent);
			break;
		}
	}

	private void ProcessJoiningSpecificRoomState(ConnectionEvent connectionEvent)
	{
		switch (connectionEvent)
		{
		case ConnectionEvent.Disconnect:
			DisconnectFromRoom(ConnectionState.DisconnectingFromRoom);
			break;
		case ConnectionEvent.OnDisconnected:
			if (!createRoom)
			{
				ConnectToRegion(serverRegions[currentRegionIndex]);
				break;
			}
			Debug.Log("checked all the rooms, it doesn't exist. lets go back to our fav region and create the room");
			ConnectToRegion(GetRegionWithLowestPing());
			break;
		case ConnectionEvent.OnConnectedToMaster:
			if (!createRoom)
			{
				Debug.Log("connected to master in the determined region. joining specific room");
				JoinSpecificRoom();
			}
			else
			{
				createRoom = false;
				CreatePrivateRoom();
			}
			break;
		case ConnectionEvent.AttemptJoinPublicRoom:
			currentState = ConnectionState.JoiningPublicRoom;
			JoinPublicRoom(joinWithFriends: false);
			break;
		case ConnectionEvent.AttemptJoinSpecificRoom:
			isRoomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			doesRoomExist = false;
			createRoom = false;
			currentRegionIndex = 0;
			PhotonNetwork.Disconnect();
			break;
		case ConnectionEvent.OnJoinedRoom:
			Debug.Log("successfully joined room!");
			currentState = (PhotonNetwork.CurrentRoom.IsVisible ? ConnectionState.InPublicRoom : ConnectionState.InPrivateRoom);
			if (currentState == ConnectionState.InPublicRoom)
			{
				Debug.Log("game mode of room joined is: " + (string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]);
				if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.forestMapTrigger.gameModeName))
				{
					currentJoinTrigger = GorillaComputer.instance.forestMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.caveMapTrigger.gameModeName))
				{
					currentJoinTrigger = GorillaComputer.instance.caveMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.canyonMapTrigger.gameModeName))
				{
					currentJoinTrigger = GorillaComputer.instance.canyonMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.cityMapTrigger.gameModeName))
				{
					currentJoinTrigger = GorillaComputer.instance.cityMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.mountainMapTrigger.gameModeName))
				{
					currentJoinTrigger = GorillaComputer.instance.mountainMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.skyjungleMapTrigger.gameModeName))
				{
					currentJoinTrigger = GorillaComputer.instance.skyjungleMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.basementMapTrigger.gameModeName))
				{
					currentJoinTrigger = GorillaComputer.instance.basementMapTrigger;
				}
				else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.beachMapTrigger.gameModeName))
				{
					currentJoinTrigger = GorillaComputer.instance.beachMapTrigger;
				}
				currentJoinTrigger.UpdateScreens();
			}
			break;
		case ConnectionEvent.OnJoinRoomFailed:
			if (doesRoomExist && isRoomFull)
			{
				Debug.Log("cant join, the room is full! going back to best region.");
				GorillaComputer.instance.roomFull = true;
				DisconnectFromRoom(ConnectionState.DisconnectingFromRoom);
			}
			else if (currentRegionIndex == serverRegions.Length - 1)
			{
				createRoom = true;
				PhotonNetwork.Disconnect();
			}
			else
			{
				Debug.Log("room was missing. check the next region");
				currentRegionIndex++;
				PhotonNetwork.Disconnect();
			}
			break;
		case ConnectionEvent.OnCreateRoomFailed:
			Debug.Log("the room probably actually already exists, so maybe it was created just now? either way, give up.");
			DisconnectFromRoom(ConnectionState.ConnectedAndWaiting);
			break;
		default:
			InvalidState(connectionEvent);
			break;
		}
	}

	private void ProcessJoiningFriendState(ConnectionEvent connectionEvent)
	{
		switch (connectionEvent)
		{
		case ConnectionEvent.Disconnect:
			PhotonNetwork.Disconnect();
			break;
		case ConnectionEvent.OnDisconnected:
			StartCoroutine(ConnectUsingSettingsWithBackoff());
			break;
		case ConnectionEvent.OnConnectedToMaster:
			StartSearchingForFriend();
			break;
		case ConnectionEvent.FoundFriendToJoin:
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorIdToFollow, out tempPlayer) && tempPlayer != null)
			{
				currentState = ConnectionState.InPrivateRoom;
				GorillaNot.instance.SendReport("possible kick attempt", tempPlayer.UserId, tempPlayer.NickName);
			}
			else if (PhotonNetwork.CurrentRoom.Name != customRoomID)
			{
				DisconnectCleanup();
				currentState = ConnectionState.JoiningSpecificRoom;
				currentRegionIndex = 0;
				PhotonNetwork.Disconnect();
			}
			break;
		case ConnectionEvent.OnJoinRoomFailed:
			currentState = ConnectionState.ConnectedAndWaiting;
			break;
		case ConnectionEvent.AttemptJoinPublicRoom:
			currentState = ConnectionState.JoiningPublicRoom;
			ProcessState(ConnectionEvent.AttemptJoinPublicRoom);
			break;
		case ConnectionEvent.AttemptJoinSpecificRoom:
			JoinSpecificRoom();
			break;
		case ConnectionEvent.OnJoinedRoom:
			currentState = ConnectionState.InPublicRoom;
			if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.forestMapTrigger.gameModeName))
			{
				currentJoinTrigger = GorillaComputer.instance.forestMapTrigger;
			}
			else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.caveMapTrigger.gameModeName))
			{
				currentJoinTrigger = GorillaComputer.instance.caveMapTrigger;
			}
			else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.canyonMapTrigger.gameModeName))
			{
				currentJoinTrigger = GorillaComputer.instance.canyonMapTrigger;
			}
			else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.cityMapTrigger.gameModeName))
			{
				currentJoinTrigger = GorillaComputer.instance.cityMapTrigger;
			}
			else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.basementMapTrigger.gameModeName))
			{
				currentJoinTrigger = GorillaComputer.instance.basementMapTrigger;
			}
			else if (((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(GorillaComputer.instance.beachMapTrigger.gameModeName))
			{
				currentJoinTrigger = GorillaComputer.instance.beachMapTrigger;
			}
			currentJoinTrigger.UpdateScreens();
			break;
		default:
			InvalidState(connectionEvent);
			break;
		}
	}

	private void ProcessInPrivateRoomState(ConnectionEvent connectionEvent)
	{
		switch (connectionEvent)
		{
		case ConnectionEvent.OnDisconnected:
			DisconnectCleanup();
			currentState = ConnectionState.DisconnectingFromRoom;
			StartCoroutine(ConnectUsingSettingsWithBackoff());
			break;
		case ConnectionEvent.Disconnect:
			DisconnectFromRoom(ConnectionState.DisconnectingFromRoom);
			break;
		case ConnectionEvent.AttemptJoinPublicRoom:
			if (joiningWithFriend)
			{
				DisconnectFromRoom(ConnectionState.JoiningPublicRoom);
			}
			break;
		case ConnectionEvent.FollowFriendToPub:
			currentState = ConnectionState.JoiningFriend;
			StartSearchingForFriend();
			break;
		case ConnectionEvent.AttemptJoinSpecificRoom:
			if (PhotonNetwork.CurrentRoom.Name != customRoomID)
			{
				DisconnectCleanup();
				currentState = ConnectionState.JoiningSpecificRoom;
				currentRegionIndex = 0;
				PhotonNetwork.Disconnect();
			}
			break;
		default:
			InvalidState(connectionEvent);
			break;
		}
	}

	private void ProcessInPublicRoomState(ConnectionEvent connectionEvent)
	{
		switch (connectionEvent)
		{
		case ConnectionEvent.OnDisconnected:
			DisconnectCleanup();
			currentState = ConnectionState.DisconnectingFromRoom;
			StartCoroutine(ConnectUsingSettingsWithBackoff());
			break;
		case ConnectionEvent.Disconnect:
			DisconnectFromRoom(ConnectionState.DisconnectingFromRoom);
			break;
		case ConnectionEvent.AttemptJoinSpecificRoom:
			if (PhotonNetwork.CurrentRoom.Name != customRoomID)
			{
				DisconnectCleanup();
				currentState = ConnectionState.JoiningSpecificRoom;
				currentRegionIndex = 0;
				PhotonNetwork.Disconnect();
			}
			break;
		case ConnectionEvent.AttemptJoinPublicRoom:
			if (!((string)PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]).Contains(currentJoinTrigger.gameModeName))
			{
				DisconnectFromRoom(ConnectionState.JoiningPublicRoom);
			}
			break;
		default:
			InvalidState(connectionEvent);
			break;
		}
	}

	private void ProcessWrongVersionState(ConnectionEvent connectionEvent)
	{
		InvalidState(connectionEvent);
	}

	private void ProcessState(ConnectionEvent connectionEvent)
	{
		if (currentState == ConnectionState.Initialization)
		{
			ProcessInitializationState(connectionEvent);
		}
		else if (currentState == ConnectionState.DeterminingPingsAndPlayerCount)
		{
			ProcessDeterminingPingsAndPlayerCountState(connectionEvent);
		}
		else if (currentState == ConnectionState.ConnectedAndWaiting)
		{
			ProcessConnectedAndWaitingState(connectionEvent);
		}
		else if (currentState == ConnectionState.DisconnectingFromRoom)
		{
			ProcessDisconnectingFromRoomState(connectionEvent);
		}
		else if (currentState == ConnectionState.JoiningPublicRoom)
		{
			ProcessJoiningPublicRoomState(connectionEvent);
		}
		else if (currentState == ConnectionState.JoiningSpecificRoom)
		{
			ProcessJoiningSpecificRoomState(connectionEvent);
		}
		else if (currentState == ConnectionState.JoiningFriend)
		{
			ProcessJoiningFriendState(connectionEvent);
		}
		else if (currentState == ConnectionState.InPrivateRoom)
		{
			ProcessInPrivateRoomState(connectionEvent);
		}
		else if (currentState == ConnectionState.InPublicRoom)
		{
			ProcessInPublicRoomState(connectionEvent);
		}
		else if (currentState == ConnectionState.WrongVersion)
		{
			ProcessWrongVersionState(connectionEvent);
		}
	}

	private void InvalidState(ConnectionEvent connectionEvent)
	{
	}

	public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger)
	{
		currentJoinTrigger = triggeredTrigger;
		joiningWithFriend = false;
		ProcessState(ConnectionEvent.AttemptJoinPublicRoom);
	}

	public void AttemptToJoinSpecificRoom(string roomID)
	{
		customRoomID = roomID;
		joiningWithFriend = false;
		ProcessState(ConnectionEvent.AttemptJoinSpecificRoom);
	}

	private void JoinPublicRoom(bool joinWithFriends)
	{
		PhotonNetwork.LocalPlayer.NickName = GorillaComputer.instance.savedName;
		if (PlayFabClientAPI.IsClientLoggedIn())
		{
			playFabAuthenticator.SetDisplayName(PhotonNetwork.LocalPlayer.NickName);
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if (currentJoinTrigger.gameModeName != "city" && currentJoinTrigger.gameModeName != "basement")
		{
			hashtable.Add("gameMode", currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + GorillaComputer.instance.currentGameMode);
		}
		else
		{
			hashtable.Add("gameMode", currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + "CASUAL");
		}
		PhotonNetwork.AutomaticallySyncScene = false;
		if (joinWithFriends)
		{
			friendIDList.Remove(PhotonNetwork.LocalPlayer.UserId);
			PhotonNetwork.JoinRandomRoom(hashtable, GetRoomSize(currentJoinTrigger.gameModeName), MatchmakingMode.RandomMatching, null, null, friendIDList.ToArray());
		}
		else
		{
			PhotonNetwork.JoinRandomRoom(hashtable, GetRoomSize(currentJoinTrigger.gameModeName), MatchmakingMode.FillRoom, null, null);
		}
	}

	private void JoinSpecificRoom()
	{
		PhotonNetwork.LocalPlayer.NickName = GorillaComputer.instance.savedName;
		if (PlayFabClientAPI.IsClientLoggedIn())
		{
			playFabAuthenticator.SetDisplayName(PhotonNetwork.LocalPlayer.NickName);
		}
		PhotonNetwork.JoinRoom(customRoomID);
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
		attemptingToConnect = true;
		if (GorillaComputer.instance != null)
		{
			GorillaLevelScreen[] levelScreens = GorillaComputer.instance.levelScreens;
			foreach (GorillaLevelScreen obj in levelScreens)
			{
				obj.UpdateText(obj.startingText, setToGoodMaterial: true);
			}
		}
		GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
		GorillaLocomotion.Player.Instance.jumpMultiplier = 1.1f;
		GorillaNot.instance.currentMasterClient = null;
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(value: false);
		initialGameMode = "";
	}

	public override void OnConnectedToMaster()
	{
		ProcessState(ConnectionEvent.OnConnectedToMaster);
	}

	public override void OnJoinedRoom()
	{
		timeToRetryWithBackoff = 0.1f;
		if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out var value))
		{
			AttemptDisconnect();
			return;
		}
		initialGameMode = value.ToString();
		if (!PhotonNetwork.CurrentRoom.IsVisible)
		{
			currentJoinTrigger = privateTrigger;
			GorillaLevelScreen[] levelScreens = GorillaComputer.instance.levelScreens;
			for (int i = 0; i < levelScreens.Length; i++)
			{
				levelScreens[i].UpdateText("YOU'RE IN A PRIVATE ROOM, SO GO WHEREVER YOU WANT. MAKE SURE YOU PLAY WITHIN THE BOUNDARIES SET BY THE PLAYERS IN THIS ROOM!", setToGoodMaterial: true);
			}
		}
		else
		{
			allowedInPubRoom = false;
			for (int j = 0; j < GorillaComputer.instance.allowedMapsToJoin.Length; j++)
			{
				if (value.ToString().Contains(GorillaComputer.instance.allowedMapsToJoin[j]))
				{
					allowedInPubRoom = true;
					break;
				}
			}
			if (!allowedInPubRoom)
			{
				GorillaComputer.instance.roomNotAllowed = true;
				PhotonNetwork.Disconnect();
				return;
			}
		}
		PhotonVoiceNetwork.Instance.PrimaryRecorder.StartRecording();
		if (PhotonNetwork.IsMasterClient)
		{
			if (initialGameMode.Contains("CASUAL") || initialGameMode.Contains("INFECTION"))
			{
				PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Tag Manager", playerOffset.transform.position, playerOffset.transform.rotation, 0);
			}
			else if (initialGameMode.Contains("HUNT"))
			{
				PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Hunt Manager", playerOffset.transform.position, playerOffset.transform.rotation, 0);
			}
			else if (initialGameMode.Contains("BATTLE"))
			{
				PhotonNetwork.InstantiateRoomObject("GorillaPrefabs/Gorilla Battle Manager", playerOffset.transform.position, playerOffset.transform.rotation, 0);
			}
		}
		bool flag = PlayerPrefs.GetString("tutorial", "nope") == "done";
		if (!flag)
		{
			PlayerPrefs.SetString("tutorial", "done");
			PlayerPrefs.Save();
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("didTutorial", flag);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
		PhotonNetwork.Instantiate("GorillaPrefabs/Gorilla Player Networked", playerOffset.transform.position, playerOffset.transform.rotation, 0);
		GorillaComputer.instance.roomFull = false;
		GorillaComputer.instance.roomNotAllowed = false;
		if (joiningWithFriend)
		{
			BroadcastMyRoom(create: true);
		}
		PhotonNetwork.NickName = PlayerPrefs.GetString("playerName", "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0'));
		GorillaNot.instance.currentMasterClient = null;
		ProcessState(ConnectionEvent.OnJoinedRoom);
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log("join room failed:" + returnCode + " " + message);
		if (returnCode == 32758)
		{
			Debug.Log("room didn't exist!");
			doesRoomExist = false;
		}
		else
		{
			doesRoomExist = true;
		}
		if (returnCode == 32765)
		{
			Debug.Log("room was full!");
			isRoomFull = true;
		}
		else
		{
			isRoomFull = false;
		}
		ProcessState(ConnectionEvent.OnJoinRoomFailed);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		ProcessState(ConnectionEvent.OnJoinRandomFailed);
	}

	private void CreatePublicRoom(bool joinWithFriends)
	{
		ExitGames.Client.Photon.Hashtable customRoomProperties = ((!(currentJoinTrigger.gameModeName != "city") || !(currentJoinTrigger.gameModeName != "basement")) ? new ExitGames.Client.Photon.Hashtable { 
		{
			"gameMode",
			currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + "CASUAL"
		} } : new ExitGames.Client.Photon.Hashtable { 
		{
			"gameMode",
			currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + GorillaComputer.instance.currentGameMode
		} });
		Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
		roomOptions.IsVisible = true;
		roomOptions.IsOpen = true;
		roomOptions.MaxPlayers = GetRoomSize(currentJoinTrigger.gameModeName);
		roomOptions.CustomRoomProperties = customRoomProperties;
		roomOptions.PublishUserId = true;
		roomOptions.CustomRoomPropertiesForLobby = new string[1] { "gameMode" };
		if (joinWithFriends)
		{
			string[] array = friendIDList.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				_ = array[i];
			}
			friendIDList.Remove(PhotonNetwork.LocalPlayer.UserId);
			PhotonNetwork.CreateRoom(ReturnRoomName(), roomOptions, null, friendIDList.ToArray());
		}
		else
		{
			PhotonNetwork.CreateRoom(ReturnRoomName(), roomOptions);
		}
	}

	private void CreatePrivateRoom()
	{
		currentJoinTrigger = privateTrigger;
		ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable { 
		{
			"gameMode",
			currentJoinTrigger.gameModeName + GorillaComputer.instance.currentQueue + GorillaComputer.instance.currentGameMode
		} };
		Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
		roomOptions.IsVisible = false;
		roomOptions.IsOpen = true;
		roomOptions.MaxPlayers = GetRoomSize(currentJoinTrigger.gameModeName);
		roomOptions.CustomRoomProperties = customRoomProperties;
		roomOptions.PublishUserId = true;
		roomOptions.CustomRoomPropertiesForLobby = new string[1] { "gameMode" };
		PhotonNetwork.CreateRoom(customRoomID, roomOptions);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		ProcessState(ConnectionEvent.OnCreateRoomFailed);
	}

	public void ConnectToRegion(string region)
	{
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
		StartCoroutine(ConnectUsingSettingsWithBackoff());
	}

	private IEnumerator ConnectUsingSettingsWithBackoff()
	{
		if (retry)
		{
			retry = false;
			timeToRetryWithBackoff *= 2f;
			Debug.Log("backing off retry, current time to wait is " + timeToRetryWithBackoff);
		}
		yield return new WaitForSeconds(timeToRetryWithBackoff);
		PhotonNetwork.ConnectUsingSettings();
	}

	public void AttemptJoinPublicWithFriends(GorillaNetworkJoinTrigger triggeredTrigger)
	{
		currentJoinTrigger = triggeredTrigger;
		joiningWithFriend = true;
		keyToFollow = PhotonNetwork.LocalPlayer.ActorNumber + keyStr;
		BroadcastMyRoom(create: false);
		ProcessState(ConnectionEvent.AttemptJoinPublicRoom);
	}

	public void AttemptToFollowFriendIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr)
	{
		friendToFollow = userIDToFollow;
		keyToFollow = actorNumberToFollow + newKeyStr;
		actorIdToFollow = actorNumberToFollow;
		shuffler = shufflerStr;
		ProcessState(ConnectionEvent.FollowFriendToPub);
	}

	public void AttemptDisconnect()
	{
		ProcessState(ConnectionEvent.Disconnect);
	}

	private void DisconnectFromRoom(ConnectionState newState)
	{
		currentState = newState;
		PhotonNetwork.Disconnect();
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		DisconnectCleanup();
		ProcessState(ConnectionEvent.OnDisconnected);
	}

	private void GetOculusNonceCallback(Message<UserProof> message)
	{
		AuthenticationValues authValues = PhotonNetwork.AuthValues;
		if (authValues != null && PhotonNetwork.AuthValues.AuthPostData is Dictionary<string, object> dictionary)
		{
			if (message.IsError)
			{
				StartCoroutine(ReGetNonce());
				return;
			}
			dictionary["Nonce"] = message.Data.Value;
			authValues.SetAuthPostData(dictionary);
			PhotonNetwork.AuthValues = authValues;
		}
		ProcessState(ConnectionEvent.OnDisconnected);
	}

	private IEnumerator ReGetNonce()
	{
		yield return new WaitForSeconds(3f);
		Users.GetUserProof().OnComplete(GetOculusNonceCallback);
		yield return null;
	}

	public void WrongVersion()
	{
		wrongVersion = true;
		currentState = ConnectionState.WrongVersion;
	}

	public void OnApplicationQuit()
	{
		if (PhotonNetwork.IsConnected)
		{
			_ = PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev";
		}
	}

	private string ReturnRoomName()
	{
		if (isPrivate)
		{
			return customRoomID;
		}
		return RandomRoomName();
	}

	private string RandomRoomName()
	{
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			text += roomCharacters.Substring(Random.Range(0, roomCharacters.Length), 1);
		}
		if (GorillaComputer.instance.CheckAutoBanListForName(text))
		{
			return text;
		}
		return RandomRoomName();
	}

	public string ShuffleRoomName(string room, string shuffle, bool encode)
	{
		string text = "";
		if (!int.TryParse(shuffle, out var _) || shuffle.Length != room.Length * 2)
		{
			return "";
		}
		for (int i = 0; i < room.Length; i++)
		{
			string value = room.Substring(i, 1);
			int num = roomCharacters.IndexOf(value);
			text = ((!encode) ? (text + roomCharacters.Substring(mod(num - int.Parse(shuffle.Substring(i * 2, 2)), roomCharacters.Length), 1)) : (text + roomCharacters.Substring(mod(num + int.Parse(shuffle.Substring(i * 2, 2)), roomCharacters.Length), 1)));
		}
		return text;
	}

	public byte GetRoomSize(string gameModeName)
	{
		if (gameModeName.Contains("ball"))
		{
			return 5;
		}
		return 10;
	}

	public void StartSearchingForFriend()
	{
		startingToLookForFriend = Time.time;
		successfullyFoundFriend = false;
		StartCoroutine(SearchForFriendToJoin(friendToFollow, keyToFollow, shuffler));
	}

	private IEnumerator SearchForFriendToJoin(string userID, string keyToFollow, string shufflerToFollow)
	{
		while (!successfullyFoundFriend && startingToLookForFriend + timeToSpendLookingForFriend > Time.time)
		{
			try
			{
				PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
				{
					Keys = new List<string> { keyToFollow },
					SharedGroupId = userID
				}, delegate(GetSharedGroupDataResult result)
				{
					foreach (KeyValuePair<string, SharedGroupDataRecord> datum in result.Data)
					{
						if (datum.Key == keyToFollow)
						{
							customRoomID = ShuffleRoomName(datum.Value.Value, shufflerToFollow, encode: false);
							successfullyFoundFriend = true;
							ProcessState(ConnectionEvent.FoundFriendToJoin);
						}
					}
				}, delegate
				{
				});
			}
			catch
			{
			}
			yield return new WaitForSeconds(3f);
		}
		yield return null;
	}

	private string GetRegionWithLowestPing()
	{
		int num = 10000;
		int num2 = 0;
		for (int i = 0; i < serverRegions.Length; i++)
		{
			Debug.Log("ping in region " + serverRegions[i] + " is " + pingInRegion[i]);
			if (pingInRegion[i] < num && pingInRegion[i] > 0)
			{
				num = pingInRegion[i];
				num2 = i;
			}
		}
		return serverRegions[num2];
	}

	public int TotalUsers()
	{
		int num = 0;
		int[] array = playersInRegion;
		foreach (int num2 in array)
		{
			num += num2;
		}
		return num;
	}

	public string CurrentState()
	{
		return currentState.ToString();
	}

	private void BroadcastMyRoom(bool create)
	{
		Debug.Log("broadcasting room");
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
		{
			FunctionName = "BroadcastMyRoom",
			FunctionParameter = new
			{
				KeyToFollow = keyToFollow,
				RoomToJoin = ShuffleRoomName(PhotonNetwork.CurrentRoom.Name, shuffler, encode: true),
				Set = create
			}
		}, delegate
		{
		}, delegate
		{
		});
	}

	public static int mod(int x, int m)
	{
		return (x % m + m) % m;
	}
}
