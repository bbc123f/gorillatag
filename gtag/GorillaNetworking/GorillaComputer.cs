using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Backtrace.Unity;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaNetworking;

public class GorillaComputer : MonoBehaviourPunCallbacks
{
	public enum ComputerState
	{
		Startup,
		Color,
		Name,
		Turn,
		Mic,
		Room,
		Queue,
		Group,
		Voice,
		Credits,
		Visuals,
		Time,
		NameWarning,
		Loading,
		Support
	}

	private enum NameCheckResult
	{
		Success,
		Warning,
		Ban
	}

	public static volatile GorillaComputer instance;

	public static bool hasInstance;

	public bool tryGetTimeAgain;

	public Material unpressedMaterial;

	public Material pressedMaterial;

	public string currentTextField;

	public float buttonFadeTime;

	public GorillaText offlineScoreboard;

	public GorillaText screenText;

	public GorillaText functionSelectText;

	public GorillaText wallScreenText;

	public GorillaText tutorialWallScreenText;

	public Text offlineVRRigNametagText;

	public string versionText = "";

	public string versionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

	public string unableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

	public Material wrongVersionMaterial;

	public MeshRenderer wallScreenRenderer;

	public MeshRenderer tutorialWallScreenRenderer;

	public MeshRenderer computerScreenRenderer;

	public MeshRenderer scoreboardRenderer;

	public GorillaLevelScreen[] levelScreens;

	public long startupMillis;

	public Text currentGameModeText;

	public int includeUpdatedServerSynchTest;

	public float updateCooldown = 1f;

	public float lastUpdateTime;

	public bool isConnectedToMaster;

	public bool internetFailure;

	public string[] allowedMapsToJoin;

	private Stack<ComputerState> stateStack = new Stack<ComputerState>();

	public bool stateUpdated;

	public bool screenChanged;

	private int usersBanned;

	private float redValue;

	private string redText;

	private float blueValue;

	private string blueText;

	private float greenValue;

	private string greenText;

	private int colorCursorLine;

	public string savedName;

	public string currentName;

	private string[] exactOneWeek;

	private string[] anywhereOneWeek;

	private string[] anywhereTwoWeek;

	[SerializeField]
	public TextAsset exactOneWeekFile;

	public TextAsset anywhereOneWeekFile;

	public TextAsset anywhereTwoWeekFile;

	private string warningConfirmationInputString = string.Empty;

	public string roomToJoin;

	public bool roomFull;

	public bool roomNotAllowed;

	private int turnValue;

	private string turnType;

	private GorillaSnapTurn gorillaTurn;

	public string pttType;

	public string currentQueue;

	public bool allowedInCompetitive;

	public string groupMapJoin;

	public int groupMapJoinIndex;

	public GorillaFriendCollider friendJoinCollider;

	public GorillaNetworkJoinTrigger caveMapTrigger;

	public GorillaNetworkJoinTrigger forestMapTrigger;

	public GorillaNetworkJoinTrigger canyonMapTrigger;

	public GorillaNetworkJoinTrigger cityMapTrigger;

	public GorillaNetworkJoinTrigger mountainMapTrigger;

	public GorillaNetworkJoinTrigger skyjungleMapTrigger;

	public GorillaNetworkJoinTrigger basementMapTrigger;

	public GorillaNetworkJoinTrigger beachMapTrigger;

	public string voiceChatOn;

	public ModeSelectButton[] modeSelectButtons;

	public string currentGameMode;

	public string version;

	public string buildDate;

	public string buildCode;

	public bool disableParticles;

	public float instrumentVolume;

	public CreditsView creditsView;

	private bool displaySupport;

	public bool leftHanded;

	public ComputerState currentState => stateStack.Peek();

	private bool CheckInternetConnection()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	private void Awake()
	{
		offlineScoreboard.Initialize(scoreboardRenderer, wrongVersionMaterial);
		screenText.Initialize(computerScreenRenderer, wrongVersionMaterial);
		functionSelectText.Initialize(computerScreenRenderer, wrongVersionMaterial);
		wallScreenText.Initialize(wallScreenRenderer, wrongVersionMaterial);
		tutorialWallScreenText.Initialize(tutorialWallScreenRenderer, wrongVersionMaterial);
		if (instance == null)
		{
			instance = this;
			hasInstance = true;
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		currentTextField = "";
		roomToJoin = "";
		redText = "";
		blueText = "";
		greenText = "";
		currentName = "";
		savedName = "";
		SwitchState(ComputerState.Startup);
		InitializeColorState();
		InitializeNameState();
		InitializeRoomState();
		InitializeTurnState();
		InitializeStartupState();
		InitializeQueueState();
		InitializeMicState();
		InitializeGroupState();
		InitializeVoiceState();
		InitializeGameMode();
		InitializeVisualsState();
		InitializeCreditsState();
		InitializeTimeState();
		InitializeSupportState();
	}

	private IEnumerator Start()
	{
		yield return null;
		if ((bool)BacktraceClient.Instance && includeUpdatedServerSynchTest == 1)
		{
			UnityEngine.Object.Destroy(BacktraceClient.Instance.gameObject);
		}
	}

	protected void OnDestroy()
	{
		if (instance == this)
		{
			hasInstance = false;
			instance = null;
		}
	}

	private void Update()
	{
		stateUpdated = false;
		if (!CheckInternetConnection())
		{
			UpdateFailureText("NO WIFI OR LAN CONNECTION DETECTED.");
			internetFailure = true;
		}
		else if (internetFailure)
		{
			RestoreFromFailureState();
			UpdateScreen();
		}
		else if (isConnectedToMaster && Time.time > lastUpdateTime + updateCooldown)
		{
			lastUpdateTime = Time.time;
			UpdateScreen();
		}
	}

	public void OnConnectedToMasterStuff()
	{
		if (!isConnectedToMaster)
		{
			isConnectedToMaster = true;
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "ReturnCurrentVersionNew",
				FunctionParameter = new
				{
					CurrentVersion = PhotonNetworkController.Instance.GameVersionString,
					UpdatedSynchTest = includeUpdatedServerSynchTest
				}
			}, OnReturnCurrentVersion, OnErrorShared);
			if (startupMillis == 0L && !tryGetTimeAgain)
			{
				GetCurrentTime();
			}
			_ = Application.platform;
			_ = 11;
			SaveModAccountData();
		}
	}

	public void PressButton(GorillaKeyboardButton buttonPressed)
	{
		switch (currentState)
		{
		case ComputerState.Startup:
			ProcessStartupState(buttonPressed);
			break;
		case ComputerState.Color:
			ProcessColorState(buttonPressed);
			break;
		case ComputerState.Room:
			ProcessRoomState(buttonPressed);
			break;
		case ComputerState.Name:
			ProcessNameState(buttonPressed);
			break;
		case ComputerState.Turn:
			ProcessTurnState(buttonPressed);
			break;
		case ComputerState.Mic:
			ProcessMicState(buttonPressed);
			break;
		case ComputerState.Queue:
			ProcessQueueState(buttonPressed);
			break;
		case ComputerState.Group:
			ProcessGroupState(buttonPressed);
			break;
		case ComputerState.Voice:
			ProcessVoiceState(buttonPressed);
			break;
		case ComputerState.Credits:
			ProcessCreditsState(buttonPressed);
			break;
		case ComputerState.Support:
			ProcessSupportState(buttonPressed);
			break;
		case ComputerState.Visuals:
			ProcessVisualsState(buttonPressed);
			break;
		case ComputerState.NameWarning:
			ProcessNameWarningState(buttonPressed);
			break;
		}
		buttonPressed.GetComponent<MeshRenderer>().material = pressedMaterial;
		buttonPressed.pressTime = Time.time;
		StartCoroutine(ButtonColorUpdate(buttonPressed));
	}

	private IEnumerator ButtonColorUpdate(GorillaKeyboardButton button)
	{
		yield return new WaitForSeconds(buttonFadeTime);
		if (button.pressTime != 0f && Time.time > buttonFadeTime + button.pressTime)
		{
			button.GetComponent<MeshRenderer>().material = unpressedMaterial;
			button.pressTime = 0f;
		}
	}

	private void InitializeStartupState()
	{
	}

	private void InitializeRoomState()
	{
	}

	private void InitializeColorState()
	{
		redValue = PlayerPrefs.GetFloat("redValue", 0f);
		greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
		blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
		colorCursorLine = 0;
		GorillaTagger.Instance.UpdateColor(redValue, greenValue, blueValue);
	}

	private void InitializeNameState()
	{
		savedName = PlayerPrefs.GetString("playerName", "gorilla");
		PhotonNetwork.LocalPlayer.NickName = savedName;
		currentName = savedName;
		exactOneWeek = exactOneWeekFile.text.Split('\n');
		anywhereOneWeek = anywhereOneWeekFile.text.Split('\n');
		anywhereTwoWeek = anywhereTwoWeekFile.text.Split('\n');
		for (int i = 0; i < exactOneWeek.Length; i++)
		{
			exactOneWeek[i] = exactOneWeek[i].ToLower().TrimEnd('\r', '\n');
		}
		for (int j = 0; j < anywhereOneWeek.Length; j++)
		{
			anywhereOneWeek[j] = anywhereOneWeek[j].ToLower().TrimEnd('\r', '\n');
		}
		for (int k = 0; k < anywhereTwoWeek.Length; k++)
		{
			anywhereTwoWeek[k] = anywhereTwoWeek[k].ToLower().TrimEnd('\r', '\n');
		}
	}

	private void InitializeTurnState()
	{
		gorillaTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
		string defaultValue = ((Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP");
		turnType = PlayerPrefs.GetString("stickTurning", defaultValue);
		turnValue = PlayerPrefs.GetInt("turnFactor", 4);
		gorillaTurn.ChangeTurnMode(turnType, turnValue);
	}

	private void InitializeMicState()
	{
		pttType = PlayerPrefs.GetString("pttType", "ALL CHAT");
	}

	private void InitializeQueueState()
	{
		currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
		allowedInCompetitive = PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1;
		if (!allowedInCompetitive && currentQueue == "COMPETITIVE")
		{
			PlayerPrefs.SetString("currentQueue", "DEFAULT");
			PlayerPrefs.Save();
			currentQueue = "DEFAULT";
		}
	}

	private void InitializeGroupState()
	{
		groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
		groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
		allowedMapsToJoin = friendJoinCollider.myAllowedMapsToJoin;
	}

	private void InitializeVoiceState()
	{
		voiceChatOn = PlayerPrefs.GetString("voiceChatOn", "TRUE");
	}

	private void InitializeGameMode()
	{
		currentGameMode = PlayerPrefs.GetString("currentGameMode", "INFECTION");
		if (currentGameMode != "CASUAL" && currentGameMode != "INFECTION" && currentGameMode != "HUNT" && currentGameMode != "BATTLE")
		{
			PlayerPrefs.SetString("currentGameMode", "INFECTION");
			PlayerPrefs.Save();
			currentGameMode = "INFECTION";
		}
		leftHanded = PlayerPrefs.GetInt("leftHanded", 0) == 1;
		OnModeSelectButtonPress(currentGameMode, leftHanded);
	}

	private void InitializeCreditsState()
	{
	}

	private void InitializeTimeState()
	{
		BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
	}

	private void InitializeSupportState()
	{
		displaySupport = false;
	}

	private void InitializeVisualsState()
	{
		disableParticles = PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE";
		GorillaTagger.Instance.ShowCosmeticParticles(!disableParticles);
		instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
	}

	private void SwitchState(ComputerState newState, bool clearStack = true)
	{
		if (clearStack)
		{
			stateStack.Clear();
		}
		stateStack.Push(newState);
		UpdateScreen();
	}

	private void PopState()
	{
		if (stateStack.Count <= 1)
		{
			Debug.LogError("Can't pop into an empty stack");
			return;
		}
		stateStack.Pop();
		UpdateScreen();
	}

	private void SwitchToColorState()
	{
		blueText = Mathf.Floor(blueValue * 9f).ToString();
		redText = Mathf.Floor(redValue * 9f).ToString();
		greenText = Mathf.Floor(greenValue * 9f).ToString();
		SwitchState(ComputerState.Color);
	}

	private void SwitchToRoomState()
	{
		SwitchState(ComputerState.Room);
	}

	private void SwitchToNameState()
	{
		SwitchState(ComputerState.Name);
	}

	private void SwitchToTurnState()
	{
		SwitchState(ComputerState.Turn);
	}

	private void SwitchToMicState()
	{
		SwitchState(ComputerState.Mic);
	}

	private void SwitchToQueueState()
	{
		SwitchState(ComputerState.Queue);
	}

	private void SwitchToGroupState()
	{
		SwitchState(ComputerState.Group);
	}

	private void SwitchToVoiceState()
	{
		SwitchState(ComputerState.Voice);
	}

	private void SwitchToCreditsState()
	{
		SwitchState(ComputerState.Credits);
	}

	private void SwitchToSupportState()
	{
		SwitchState(ComputerState.Support);
	}

	private void SwitchToVisualsState()
	{
		SwitchState(ComputerState.Visuals);
	}

	private void SwitchToWarningState()
	{
		warningConfirmationInputString = string.Empty;
		SwitchState(ComputerState.NameWarning, clearStack: false);
	}

	private void SwitchToLoadingState()
	{
		SwitchState(ComputerState.Loading, clearStack: false);
	}

	private void ProcessStartupState(GorillaKeyboardButton buttonPressed)
	{
		_ = buttonPressed.characterString;
		SwitchToRoomState();
		UpdateScreen();
	}

	private void ProcessColorState(GorillaKeyboardButton buttonPressed)
	{
		if (int.TryParse(buttonPressed.characterString, out var result))
		{
			switch (colorCursorLine)
			{
			case 0:
				redText = result.ToString();
				break;
			case 1:
				greenText = result.ToString();
				break;
			case 2:
				blueText = result.ToString();
				break;
			}
			if (int.TryParse(redText, out var result2))
			{
				redValue = (float)result2 / 9f;
			}
			if (int.TryParse(greenText, out result2))
			{
				greenValue = (float)result2 / 9f;
			}
			if (int.TryParse(blueText, out result2))
			{
				blueValue = (float)result2 / 9f;
			}
			PlayerPrefs.SetFloat("redValue", redValue);
			PlayerPrefs.SetFloat("greenValue", greenValue);
			PlayerPrefs.SetFloat("blueValue", blueValue);
			GorillaTagger.Instance.UpdateColor(redValue, greenValue, blueValue);
			PlayerPrefs.Save();
			if (PhotonNetwork.InRoom)
			{
				GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, redValue, greenValue, blueValue, leftHanded);
			}
		}
		else
		{
			switch (buttonPressed.characterString)
			{
			case "up":
				SwitchToNameState();
				break;
			case "down":
				SwitchToTurnState();
				break;
			case "option1":
				colorCursorLine = 0;
				break;
			case "option2":
				colorCursorLine = 1;
				break;
			case "option3":
				colorCursorLine = 2;
				break;
			}
		}
		UpdateScreen();
	}

	public void ProcessNameState(GorillaKeyboardButton buttonPressed)
	{
		switch (buttonPressed.characterString)
		{
		case "up":
			SwitchToRoomState();
			break;
		case "down":
			SwitchToColorState();
			break;
		case "enter":
			if (currentName != savedName && currentName != "")
			{
				CheckAutoBanList(currentName, forRoom: false);
			}
			break;
		case "delete":
			if (currentName.Length > 0)
			{
				currentName = currentName.Substring(0, currentName.Length - 1);
			}
			break;
		default:
			if (currentName.Length < 12 && buttonPressed.characterString.Length == 1)
			{
				currentName += buttonPressed.characterString;
			}
			break;
		}
		UpdateScreen();
	}

	private void ProcessRoomState(GorillaKeyboardButton buttonPressed)
	{
		switch (buttonPressed.characterString)
		{
		case "up":
			SwitchToSupportState();
			break;
		case "down":
			SwitchToNameState();
			break;
		case "option1":
			PhotonNetworkController.Instance.AttemptDisconnect();
			break;
		case "enter":
			if (roomToJoin != "")
			{
				CheckAutoBanList(roomToJoin, forRoom: true);
			}
			break;
		case "delete":
			if (roomToJoin.Length > 0)
			{
				roomToJoin = roomToJoin.Substring(0, roomToJoin.Length - 1);
			}
			break;
		default:
			if (roomToJoin.Length < 10)
			{
				roomToJoin += buttonPressed.characterString;
			}
			break;
		case "option2":
		case "option3":
			break;
		}
		UpdateScreen();
	}

	private void ProcessTurnState(GorillaKeyboardButton buttonPressed)
	{
		if (int.TryParse(buttonPressed.characterString, out var result))
		{
			turnValue = result;
			PlayerPrefs.SetInt("turnFactor", turnValue);
			PlayerPrefs.Save();
			gorillaTurn.ChangeTurnMode(turnType, turnValue);
		}
		else
		{
			switch (buttonPressed.characterString)
			{
			case "up":
				SwitchToColorState();
				break;
			case "down":
				SwitchToMicState();
				break;
			case "option1":
				turnType = "SNAP";
				PlayerPrefs.SetString("stickTurning", turnType);
				PlayerPrefs.Save();
				gorillaTurn.ChangeTurnMode(turnType, turnValue);
				break;
			case "option2":
				turnType = "SMOOTH";
				PlayerPrefs.SetString("stickTurning", turnType);
				PlayerPrefs.Save();
				gorillaTurn.ChangeTurnMode(turnType, turnValue);
				break;
			case "option3":
				turnType = "NONE";
				PlayerPrefs.SetString("stickTurning", turnType);
				PlayerPrefs.Save();
				gorillaTurn.ChangeTurnMode(turnType, turnValue);
				break;
			}
		}
		UpdateScreen();
	}

	private void ProcessMicState(GorillaKeyboardButton buttonPressed)
	{
		switch (buttonPressed.characterString)
		{
		case "up":
			SwitchToTurnState();
			break;
		case "down":
			SwitchToQueueState();
			break;
		case "option1":
			pttType = "ALL CHAT";
			PlayerPrefs.SetString("pttType", pttType);
			PlayerPrefs.Save();
			break;
		case "option2":
			pttType = "PUSH TO TALK";
			PlayerPrefs.SetString("pttType", pttType);
			PlayerPrefs.Save();
			break;
		case "option3":
			pttType = "PUSH TO MUTE";
			PlayerPrefs.SetString("pttType", pttType);
			PlayerPrefs.Save();
			break;
		}
		UpdateScreen();
	}

	private void ProcessQueueState(GorillaKeyboardButton buttonPressed)
	{
		switch (buttonPressed.characterString)
		{
		case "up":
			SwitchToMicState();
			break;
		case "down":
			SwitchToGroupState();
			break;
		case "option1":
			currentQueue = "DEFAULT";
			PlayerPrefs.SetString("currentQueue", currentQueue);
			PlayerPrefs.Save();
			break;
		case "option2":
			currentQueue = "MINIGAMES";
			PlayerPrefs.SetString("currentQueue", currentQueue);
			PlayerPrefs.Save();
			break;
		case "option3":
			if (allowedInCompetitive)
			{
				currentQueue = "COMPETITIVE";
				PlayerPrefs.SetString("currentQueue", currentQueue);
				PlayerPrefs.Save();
			}
			break;
		}
		UpdateScreen();
	}

	private void ProcessGroupState(GorillaKeyboardButton buttonPressed)
	{
		switch (buttonPressed.characterString)
		{
		case "up":
			SwitchToQueueState();
			break;
		case "down":
			SwitchToVoiceState();
			break;
		case "1":
			groupMapJoin = "FOREST";
			groupMapJoinIndex = 0;
			PlayerPrefs.SetString("groupMapJoin", groupMapJoin);
			PlayerPrefs.SetInt("groupMapJoinIndex", groupMapJoinIndex);
			PlayerPrefs.Save();
			break;
		case "2":
			groupMapJoin = "CAVE";
			groupMapJoinIndex = 1;
			PlayerPrefs.SetString("groupMapJoin", groupMapJoin);
			PlayerPrefs.SetInt("groupMapJoinIndex", groupMapJoinIndex);
			PlayerPrefs.Save();
			break;
		case "3":
			groupMapJoin = "CANYON";
			groupMapJoinIndex = 2;
			PlayerPrefs.SetString("groupMapJoin", groupMapJoin);
			PlayerPrefs.SetInt("groupMapJoinIndex", groupMapJoinIndex);
			PlayerPrefs.Save();
			break;
		case "4":
			groupMapJoin = "CITY";
			groupMapJoinIndex = 3;
			PlayerPrefs.SetString("groupMapJoin", groupMapJoin);
			PlayerPrefs.SetInt("groupMapJoinIndex", groupMapJoinIndex);
			PlayerPrefs.Save();
			break;
		case "enter":
			OnGroupJoinButtonPress(Mathf.Min(allowedMapsToJoin.Length - 1, groupMapJoinIndex), friendJoinCollider);
			break;
		}
		roomFull = false;
		UpdateScreen();
	}

	private void ProcessVoiceState(GorillaKeyboardButton buttonPressed)
	{
		switch (buttonPressed.characterString)
		{
		case "up":
			SwitchToGroupState();
			break;
		case "down":
			SwitchToVisualsState();
			break;
		case "option1":
			voiceChatOn = "TRUE";
			PlayerPrefs.SetString("voiceChatOn", voiceChatOn);
			PlayerPrefs.Save();
			RigContainer.RefreshAllRigVoices();
			break;
		case "option2":
			voiceChatOn = "FALSE";
			PlayerPrefs.SetString("voiceChatOn", voiceChatOn);
			PlayerPrefs.Save();
			RigContainer.RefreshAllRigVoices();
			break;
		}
		UpdateScreen();
	}

	private void ProcessVisualsState(GorillaKeyboardButton buttonPressed)
	{
		if (int.TryParse(buttonPressed.characterString, out var result))
		{
			instrumentVolume = (float)result / 50f;
			PlayerPrefs.SetFloat("instrumentVolume", instrumentVolume);
			PlayerPrefs.Save();
		}
		else
		{
			switch (buttonPressed.characterString)
			{
			case "up":
				SwitchToVoiceState();
				break;
			case "down":
				SwitchToCreditsState();
				break;
			case "option1":
				disableParticles = false;
				PlayerPrefs.SetString("disableParticles", "FALSE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!disableParticles);
				break;
			case "option2":
				disableParticles = true;
				PlayerPrefs.SetString("disableParticles", "TRUE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!disableParticles);
				break;
			}
		}
		UpdateScreen();
	}

	private void ProcessCreditsState(GorillaKeyboardButton buttonPressed)
	{
		switch (buttonPressed.characterString)
		{
		case "up":
			SwitchToVisualsState();
			break;
		case "down":
			SwitchToSupportState();
			break;
		case "enter":
			creditsView.ProcessButtonPress(buttonPressed);
			break;
		}
		UpdateScreen();
	}

	private void ProcessSupportState(GorillaKeyboardButton buttonPressed)
	{
		switch (buttonPressed.characterString)
		{
		case "up":
			displaySupport = false;
			SwitchToCreditsState();
			break;
		case "down":
			displaySupport = false;
			SwitchToRoomState();
			break;
		case "enter":
			displaySupport = true;
			break;
		}
		UpdateScreen();
	}

	private void ProcessNameWarningState(GorillaKeyboardButton buttonPressed)
	{
		if (warningConfirmationInputString.ToLower() == "yes")
		{
			PopState();
		}
		else
		{
			switch (buttonPressed.characterString)
			{
			case "delete":
				if (warningConfirmationInputString.Length > 0)
				{
					warningConfirmationInputString = warningConfirmationInputString.Substring(0, warningConfirmationInputString.Length - 1);
				}
				break;
			default:
				if (warningConfirmationInputString.Length < 3)
				{
					warningConfirmationInputString += buttonPressed.characterString;
				}
				break;
			case "up":
			case "down":
			case "option1":
			case "option2":
			case "option3":
			case "enter":
				break;
			}
		}
		UpdateScreen();
	}

	public void UpdateScreen()
	{
		if (PhotonNetworkController.Instance != null && !PhotonNetworkController.Instance.wrongVersion)
		{
			UpdateFunctionScreen();
			switch (currentState)
			{
			case ComputerState.Startup:
				screenText.Text = "GORILLA OS\n\n" + PhotonNetworkController.Instance.TotalUsers() + " PLAYERS ONLINE\n\n" + usersBanned + " USERS BANNED YESTERDAY\n\nPRESS ANY KEY TO BEGIN";
				break;
			case ComputerState.Color:
			{
				screenText.Text = "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.";
				GorillaText gorillaText6 = screenText;
				gorillaText6.Text = gorillaText6.Text + "\n\n  RED: " + Mathf.FloorToInt(redValue * 9f) + ((colorCursorLine == 0) ? "<--" : "");
				GorillaText gorillaText7 = screenText;
				gorillaText7.Text = gorillaText7.Text + "\n\nGREEN: " + Mathf.FloorToInt(greenValue * 9f) + ((colorCursorLine == 1) ? "<--" : "");
				GorillaText gorillaText8 = screenText;
				gorillaText8.Text = gorillaText8.Text + "\n\n BLUE: " + Mathf.FloorToInt(blueValue * 9f) + ((colorCursorLine == 2) ? "<--" : "");
				break;
			}
			case ComputerState.Room:
			{
				screenText.Text = "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE. PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM.\n\nCURRENT ROOM: ";
				if (PhotonNetwork.InRoom)
				{
					screenText.Text += PhotonNetwork.CurrentRoom.Name;
					GorillaText gorillaText3 = screenText;
					gorillaText3.Text = gorillaText3.Text + "\n\nPLAYERS IN ROOM: " + PhotonNetwork.CurrentRoom.PlayerCount;
				}
				else
				{
					screenText.Text += "-NOT IN ROOM-";
					GorillaText gorillaText4 = screenText;
					gorillaText4.Text = gorillaText4.Text + "\n\nPLAYERS ONLINE: " + PhotonNetworkController.Instance.TotalUsers();
				}
				GorillaText gorillaText5 = screenText;
				gorillaText5.Text = gorillaText5.Text + "\n\nROOM TO JOIN: " + roomToJoin;
				if (roomFull)
				{
					screenText.Text += "\n\nROOM FULL. JOIN ROOM FAILED.";
				}
				else if (roomNotAllowed)
				{
					screenText.Text += "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
				}
				break;
			}
			case ComputerState.Name:
			{
				screenText.Text = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\nCURRENT NAME: " + savedName;
				GorillaText gorillaText2 = screenText;
				gorillaText2.Text = gorillaText2.Text + "\n\n    NEW NAME: " + currentName;
				break;
			}
			case ComputerState.Turn:
				screenText.Text = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING. PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.\n CURRENT TURN TYPE: " + turnType + "\nCURRENT TURN SPEED: " + turnValue;
				break;
			case ComputerState.Queue:
				if (allowedInCompetitive)
				{
					screenText.Text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN. PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.\n\nCURRENT QUEUE: " + currentQueue;
				}
				else
				{
					screenText.Text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY. PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES\n\nCURRENT QUEUE: " + currentQueue;
				}
				break;
			case ComputerState.Mic:
				screenText.Text = "CHOOSE ALL CHAT, PUSH TO TALK, OR PUSH TO MUTE. THE BUTTONS FOR PUSH TO TALK AND PUSH TO MUTE ARE ANY OF THE FACE BUTTONS.\nPRESS OPTION 1 TO CHOOSE ALL CHAT.\nPRESS OPTION 2 TO CHOOSE PUSH TO TALK.\nPRESS OPTION 3 TO CHOOSE PUSH TO MUTE.\n\nCURRENT MIC SETTING: " + pttType;
				break;
			case ComputerState.Group:
				if (allowedMapsToJoin.Length == 1)
				{
					screenText.Text = "USE THIS TO JOIN A PUBLIC ROOM WITH A GROUP OF FRIENDS. GET EVERYONE IN A PRIVATE ROOM. PRESS THE NUMBER KEYS TO SELECT THE MAP. 1 FOR " + allowedMapsToJoin[Mathf.Min(allowedMapsToJoin.Length - 1, groupMapJoinIndex)].ToUpper() + ". WHILE EVERYONE IS SITTING NEXT TO THE COMPUTER, PRESS ENTER. YOU WILL ALL JOIN A PUBLIC ROOM TOGETHER AS LONG AS EVERYONE IS NEXT TO THE COMPUTER.\nCURRENT MAP SELECTION : " + allowedMapsToJoin[Mathf.Min(allowedMapsToJoin.Length - 1, groupMapJoinIndex)].ToUpper();
				}
				else
				{
					screenText.Text = "USE THIS TO JOIN A PUBLIC ROOM WITH A GROUP OF FRIENDS. GET EVERYONE IN A PRIVATE ROOM. PRESS THE NUMBER KEYS TO SELECT THE MAP. 1 FOR FOREST, 2 FOR CAVE, AND 3 FOR CANYON, AND 4 FOR CITY. WHILE EVERYONE IS SITTING NEXT TO THE COMPUTER, PRESS ENTER. YOU WILL ALL JOIN A PUBLIC ROOM TOGETHER AS LONG AS EVERYONE IS NEXT TO THE COMPUTER.\nCURRENT MAP SELECTION : " + allowedMapsToJoin[Mathf.Min(allowedMapsToJoin.Length - 1, groupMapJoinIndex)].ToUpper();
				}
				break;
			case ComputerState.Voice:
				screenText.Text = "USE THIS TO ENABLE OR DISABLE VOICE CHAT.\nPRESS OPTION 1 TO ENABLE VOICE CHAT.\nPRESS OPTION 2 TO DISABLE VOICE CHAT.\n\nVOICE CHAT ON: " + voiceChatOn;
				break;
			case ComputerState.Visuals:
				screenText.Text = "UPDATE ITEMS SETTINGS. PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.\n\nITEM PARTICLES ON: " + (disableParticles ? "FALSE" : "TRUE") + "\nINSTRUMENT VOLUME: " + Mathf.CeilToInt(instrumentVolume * 50f);
				break;
			case ComputerState.Credits:
				screenText.Text = creditsView.GetScreenText();
				break;
			case ComputerState.Time:
				screenText.Text = "UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: " + BetterDayNightManager.instance.currentSetting.ToString().ToUpper() + ". \nTIME OF DAY: " + BetterDayNightManager.instance.currentTimeOfDay.ToUpper() + ". \n";
				break;
			case ComputerState.Support:
				if (displaySupport)
				{
					string text = "OCULUS PC";
					text = "STEAM";
					screenText.Text = "SUPPORT\n\nPLAYERID   " + PlayFabAuthenticator.instance._playFabPlayerIdCache + "\nVERSION    " + version.ToUpper() + "\nPLATFORM   " + text + "\nBUILD DATE " + buildDate + "\n";
				}
				else
				{
					screenText.Text = "SUPPORT\n\n";
					screenText.Text += "PRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION\n\n\n\n";
					screenText.Text += "<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER ";
					screenText.Text += "THAN ANOTHER AXIOM SUPPORT</color>";
				}
				break;
			case ComputerState.NameWarning:
			{
				screenText.Text = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
				if (warningConfirmationInputString.ToLower() == "yes")
				{
					screenText.Text += "\n\nPRESS ANY KEY TO CONTINUE";
					break;
				}
				GorillaText gorillaText = screenText;
				gorillaText.Text = gorillaText.Text + "\n\nTYPE 'YES' TO CONFIRM: " + warningConfirmationInputString;
				break;
			}
			case ComputerState.Loading:
				screenText.Text = "LOADING...";
				break;
			}
		}
		if (PhotonNetwork.InRoom)
		{
			if (GorillaGameManager.instance != null && GorillaGameManager.instance.GetComponent<GorillaTagManager>() != null)
			{
				if (!GorillaGameManager.instance.GetComponent<GorillaTagManager>().IsGameModeTag())
				{
					currentGameModeText.text = "CURRENT MODE\nCASUAL";
				}
				else
				{
					currentGameModeText.text = "CURRENT MODE\nINFECTION";
				}
			}
			else if (GorillaGameManager.instance != null && GorillaGameManager.instance.GetComponent<GorillaHuntManager>() != null)
			{
				currentGameModeText.text = "CURRENT MODE\nHUNT";
			}
			else if (GorillaGameManager.instance != null && GorillaGameManager.instance.GetComponent<GorillaBattleManager>() != null)
			{
				currentGameModeText.text = "CURRENT MODE\nPAINTBRAWL";
			}
			else
			{
				currentGameModeText.text = "CURRENT MODE\nERROR";
			}
		}
		else
		{
			currentGameModeText.text = "CURRENT MODE\n-NOT IN ROOM-";
		}
	}

	private void UpdateFunctionScreen()
	{
		functionSelectText.Text = "ROOM   " + ((currentState == ComputerState.Room) ? "<-" : "") + "\nNAME   " + ((currentState == ComputerState.Name) ? "<-" : "") + "\nCOLOR  " + ((currentState == ComputerState.Color) ? "<-" : "") + "\nTURN   " + ((currentState == ComputerState.Turn) ? "<-" : "") + "\nMIC    " + ((currentState == ComputerState.Mic) ? "<-" : "") + "\nQUEUE  " + ((currentState == ComputerState.Queue) ? "<-" : "") + "\nGROUP  " + ((currentState == ComputerState.Group) ? "<-" : "") + "\nVOICE  " + ((currentState == ComputerState.Voice) ? "<-" : "") + "\nITEMS  " + ((currentState == ComputerState.Visuals) ? "<-" : "") + "\nCREDITS" + ((currentState == ComputerState.Credits) ? "<-" : "") + "\nSUPPORT" + ((currentState == ComputerState.Support) ? "<-" : "");
	}

	private void OnReturnCurrentVersion(PlayFab.ClientModels.ExecuteCloudScriptResult result)
	{
		JsonObject jsonObject = (JsonObject)result.FunctionResult;
		if (jsonObject != null)
		{
			if (jsonObject.TryGetValue("SynchTime", out var value))
			{
				Debug.Log("message value is: " + (string)value);
			}
			if (jsonObject.TryGetValue("Fail", out value) && (bool)value)
			{
				GeneralFailureMessage(versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("ResultCode", out value) && (ulong)value != 0L)
			{
				GeneralFailureMessage(versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("BannedUsers", out value))
			{
				usersBanned = int.Parse((string)value);
			}
			versionText = "WELCOME TO GORILLA TAG! HEAD OUTSIDE TO AUTOMATICALLY JOIN A PUBLIC GAME, OR USE THE TERMINAL TO JOIN A SPECIFIC ROOM OR ADJUST YOUR SETTINGS.";
			UpdateScreen();
		}
		else
		{
			GeneralFailureMessage(versionMismatch);
		}
	}

	private void OnRoomNameChecked(ExecuteFunctionResult result)
	{
		if (currentState == ComputerState.Loading)
		{
			PopState();
		}
		if (!((JsonObject)result.FunctionResult).TryGetValue("result", out var value))
		{
			return;
		}
		switch ((NameCheckResult)int.Parse(value.ToString()))
		{
		case NameCheckResult.Success:
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomToJoin);
			break;
		case NameCheckResult.Warning:
			roomToJoin = "";
			SwitchToWarningState();
			break;
		case NameCheckResult.Ban:
		{
			roomToJoin = "";
			Application.Quit();
			PhotonNetwork.Disconnect();
			UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
			UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
			GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
			break;
		}
		}
	}

	private void OnPlayerNameChecked(ExecuteFunctionResult result)
	{
		if (currentState == ComputerState.Loading)
		{
			PopState();
		}
		if (((JsonObject)result.FunctionResult).TryGetValue("result", out var value))
		{
			switch ((NameCheckResult)int.Parse(value.ToString()))
			{
			case NameCheckResult.Success:
				PhotonNetwork.LocalPlayer.NickName = currentName;
				break;
			case NameCheckResult.Warning:
				PhotonNetwork.LocalPlayer.NickName = "gorilla";
				currentName = "gorilla";
				SwitchToWarningState();
				break;
			case NameCheckResult.Ban:
			{
				PhotonNetwork.LocalPlayer.NickName = "gorilla";
				currentName = "gorilla";
				Application.Quit();
				PhotonNetwork.Disconnect();
				UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
				UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
				break;
			}
			}
		}
		offlineVRRigNametagText.text = currentName;
		savedName = currentName;
		PlayerPrefs.SetString("playerName", currentName);
		PlayerPrefs.Save();
		if (PhotonNetwork.InRoom)
		{
			GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, redValue, greenValue, blueValue, leftHanded);
		}
	}

	private void OnErrorNameCheck(PlayFabError error)
	{
		if (currentState == ComputerState.Loading)
		{
			PopState();
		}
		OnErrorShared(error);
	}

	private void CheckAutoBanList(string nameToCheck, bool forRoom)
	{
		if (forRoom)
		{
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = new PlayFab.CloudScriptModels.EntityKey
				{
					Id = PlayFabSettings.staticPlayer.EntityId,
					Type = PlayFabSettings.staticPlayer.EntityType
				},
				FunctionName = "CheckForBadName",
				FunctionParameter = new Dictionary<string, string>
				{
					{ "name", nameToCheck },
					{
						"forRoom",
						forRoom.ToString()
					}
				},
				GeneratePlayStreamEvent = false
			}, OnRoomNameChecked, OnErrorNameCheck);
		}
		else
		{
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = new PlayFab.CloudScriptModels.EntityKey
				{
					Id = PlayFabSettings.staticPlayer.EntityId,
					Type = PlayFabSettings.staticPlayer.EntityType
				},
				FunctionName = "CheckForBadName",
				FunctionParameter = new Dictionary<string, string>
				{
					{ "name", nameToCheck },
					{
						"forRoom",
						forRoom.ToString()
					}
				},
				GeneratePlayStreamEvent = false
			}, OnPlayerNameChecked, OnErrorNameCheck);
		}
		SwitchToLoadingState();
	}

	public bool CheckAutoBanListForName(string nameToCheck)
	{
		nameToCheck = nameToCheck.ToLower();
		nameToCheck = new string(Array.FindAll(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
		string[] array = anywhereTwoWeek;
		foreach (string value in array)
		{
			if (nameToCheck.IndexOf(value) >= 0)
			{
				return false;
			}
		}
		array = anywhereOneWeek;
		foreach (string value2 in array)
		{
			if (nameToCheck.IndexOf(value2) >= 0 && !nameToCheck.Contains("fagol"))
			{
				return false;
			}
		}
		array = exactOneWeek;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == nameToCheck)
			{
				return false;
			}
		}
		return true;
	}

	public void UpdateFailureText(string failMessage)
	{
		GorillaLevelScreen[] array = levelScreens;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateText(failMessage, setToGoodMaterial: false);
		}
		offlineScoreboard.EnableFailedState(failMessage);
		screenText.EnableFailedState(failMessage);
		functionSelectText.EnableFailedState(failMessage);
		wallScreenText.EnableFailedState(failMessage);
		tutorialWallScreenText.EnableFailedState(failMessage);
	}

	private void RestoreFromFailureState()
	{
		GorillaLevelScreen[] array = levelScreens;
		foreach (GorillaLevelScreen obj in array)
		{
			obj.UpdateText(obj.startingText, setToGoodMaterial: true);
		}
		offlineScoreboard.DisableFailedState();
		screenText.DisableFailedState();
		functionSelectText.DisableFailedState();
		wallScreenText.DisableFailedState();
		tutorialWallScreenText.DisableFailedState();
	}

	public void GeneralFailureMessage(string failMessage)
	{
		isConnectedToMaster = false;
		PhotonNetworkController.Instance.WrongVersion();
		UpdateFailureText(failMessage);
		UpdateScreen();
	}

	private static void OnErrorShared(PlayFabError error)
	{
		if (error.Error == PlayFabErrorCode.NotAuthenticated)
		{
			PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
		}
		else if (error.Error == PlayFabErrorCode.AccountBanned)
		{
			Application.Quit();
			PhotonNetwork.Disconnect();
			UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
			UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
			GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
		}
		if (error.ErrorMessage == "The account making this request is currently banned")
		{
			using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<string, List<string>> current = enumerator.Current;
					if (current.Value[0] != "Indefinite")
					{
						instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance._playFabPlayerIdCache + " HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + current.Key + "\nHOURS LEFT: " + (int)((DateTime.Parse(current.Value[0]) - DateTime.UtcNow).TotalHours + 1.0));
					}
					else
					{
						instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance._playFabPlayerIdCache + " HAS BEEN BANNED INDEFINITELY.\nREASON: " + current.Key);
					}
				}
				return;
			}
		}
		if (!(error.ErrorMessage == "The IP making this request is currently banned"))
		{
			return;
		}
		using Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator();
		if (enumerator.MoveNext())
		{
			KeyValuePair<string, List<string>> current2 = enumerator.Current;
			if (current2.Value[0] != "Indefinite")
			{
				instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + current2.Key + "\nHOURS LEFT: " + (int)((DateTime.Parse(current2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0));
			}
			else
			{
				instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + current2.Key);
			}
		}
	}

	private void GetCurrentTime()
	{
		tryGetTimeAgain = true;
		PlayFabClientAPI.GetTime(new GetTimeRequest(), OnGetTimeSuccess, OnGetTimeFailure);
	}

	private void OnGetTimeSuccess(GetTimeResult result)
	{
		startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
	}

	private void OnGetTimeFailure(PlayFabError error)
	{
		startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
		if (error.Error == PlayFabErrorCode.NotAuthenticated)
		{
			PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
		}
		else if (error.Error == PlayFabErrorCode.AccountBanned)
		{
			Application.Quit();
			PhotonNetwork.Disconnect();
			UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
			UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
			GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
		}
	}

	public void OnModeSelectButtonPress(string gameMode, bool leftHand)
	{
		currentGameMode = gameMode;
		PlayerPrefs.SetString("currentGameMode", gameMode);
		if (leftHand != leftHanded)
		{
			PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
			leftHanded = leftHand;
		}
		PlayerPrefs.Save();
		ModeSelectButton[] array = modeSelectButtons;
		foreach (ModeSelectButton modeSelectButton in array)
		{
			modeSelectButton.buttonRenderer.material = ((currentGameMode == modeSelectButton.gameMode) ? modeSelectButton.pressedMaterial : modeSelectButton.unpressedMaterial);
		}
	}

	public void OnGroupJoinButtonPress(int mapJoinIndex, GorillaFriendCollider chosenFriendJoinCollider)
	{
		if (mapJoinIndex >= allowedMapsToJoin.Length)
		{
			roomNotAllowed = true;
			SwitchToRoomState();
		}
		else
		{
			if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.IsVisible)
			{
				return;
			}
			PhotonNetworkController.Instance.friendIDList = new List<string>(chosenFriendJoinCollider.playerIDsCurrentlyTouching);
			foreach (string friendID in PhotonNetworkController.Instance.friendIDList)
			{
				_ = friendID;
			}
			PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Photon.Realtime.Player player in playerList)
			{
				if (chosenFriendJoinCollider.playerIDsCurrentlyTouching.Contains(player.UserId) && player != PhotonNetwork.LocalPlayer)
				{
					GorillaGameManager.instance.photonView.RPC("JoinPubWithFriends", player, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
				}
			}
			PhotonNetwork.SendAllOutgoingCommands();
			GorillaNetworkJoinTrigger triggeredTrigger = null;
			if (allowedMapsToJoin[mapJoinIndex] == "forest")
			{
				triggeredTrigger = forestMapTrigger;
			}
			else if (allowedMapsToJoin[mapJoinIndex] == "cave")
			{
				triggeredTrigger = caveMapTrigger;
			}
			else if (allowedMapsToJoin[mapJoinIndex] == "canyon")
			{
				triggeredTrigger = canyonMapTrigger;
			}
			else if (allowedMapsToJoin[mapJoinIndex] == "city")
			{
				triggeredTrigger = cityMapTrigger;
			}
			else if (allowedMapsToJoin[mapJoinIndex] == "mountain")
			{
				triggeredTrigger = mountainMapTrigger;
			}
			else if (allowedMapsToJoin[mapJoinIndex] == "clouds")
			{
				triggeredTrigger = skyjungleMapTrigger;
			}
			else if (allowedMapsToJoin[mapJoinIndex] == "basement")
			{
				triggeredTrigger = basementMapTrigger;
			}
			else if (allowedMapsToJoin[mapJoinIndex] == "beach")
			{
				triggeredTrigger = beachMapTrigger;
			}
			PhotonNetworkController.Instance.AttemptJoinPublicWithFriends(triggeredTrigger);
			SwitchToRoomState();
		}
	}

	public void SaveModAccountData()
	{
		string path = Application.persistentDataPath + "/DoNotShareWithAnyoneEVERNoMatterWhatTheySay.txt";
		if (File.Exists(path))
		{
			return;
		}
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
		{
			FunctionName = "ReturnMyOculusHash"
		}, delegate(PlayFab.ClientModels.ExecuteCloudScriptResult result)
		{
			if (((JsonObject)result.FunctionResult).TryGetValue("oculusHash", out var value))
			{
				StreamWriter streamWriter = new StreamWriter(path);
				streamWriter.Write(PlayFabAuthenticator.instance._playFabPlayerIdCache + "." + (string)value);
				streamWriter.Close();
			}
		}, delegate(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
				UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
		});
	}

	public void CompQueueUnlockButtonPress()
	{
		allowedInCompetitive = true;
		PlayerPrefs.SetInt("allowedInCompetitive", 1);
		PlayerPrefs.Save();
	}
}
