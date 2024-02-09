using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameMode : MonoBehaviour
{
	private void Awake()
	{
		if (GameMode.instance.IsNull())
		{
			GameMode.instance = this;
			foreach (GorillaGameManager gorillaGameManager in base.gameObject.GetComponentsInChildren<GorillaGameManager>(true))
			{
				int num = (int)gorillaGameManager.GameType();
				string text = gorillaGameManager.GameModeName();
				if (GameMode.gameModeTable.ContainsKey(num))
				{
					Debug.LogWarning("Duplicate gamemode type, skipping this instance", gorillaGameManager);
				}
				else
				{
					GameMode.gameModeTable.Add((int)gorillaGameManager.GameType(), gorillaGameManager);
					GameMode.gameModeKeyByName.Add(text, num);
					GameMode.gameModes.Add(gorillaGameManager);
					GameMode.gameModeNames.Add(text);
				}
			}
			return;
		}
		Object.Destroy(this);
	}

	private void OnDestroy()
	{
		if (GameMode.instance == this)
		{
			GameMode.instance = null;
		}
	}

	public static GorillaGameManager ActiveGameMode
	{
		get
		{
			return GameMode.activeGameMode;
		}
	}

	internal static GameModeSerializer ActiveNetworkHandler
	{
		get
		{
			return GameMode.activeNetworkHandler;
		}
	}

	static GameMode()
	{
		GameMode.StaticLoad();
	}

	[OnEnterPlay_Run]
	private static void StaticLoad()
	{
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(GameMode.ResetGameModes));
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(GameMode.RefreshPlayers));
		RoomSystem.PlayersChangedEvent = (Action)Delegate.Combine(RoomSystem.PlayersChangedEvent, new Action(GameMode.RefreshPlayers));
	}

	internal static bool LoadGameModeFromProperty()
	{
		return GameMode.LoadGameMode(GameMode.FindGameModeFromRoomProperty());
	}

	internal static bool ChangeGameFromProperty()
	{
		return GameMode.ChangeGameMode(GameMode.FindGameModeFromRoomProperty());
	}

	internal static bool LoadGameModeFromProperty(string prop)
	{
		return GameMode.LoadGameMode(GameMode.FindGameModeInString(prop));
	}

	internal static bool ChangeGameFromProperty(string prop)
	{
		return GameMode.ChangeGameMode(GameMode.FindGameModeInString(prop));
	}

	private static string FindGameModeFromRoomProperty()
	{
		object obj;
		if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj))
		{
			string text = obj as string;
			if (text != null)
			{
				return GameMode.FindGameModeInString(text);
			}
		}
		return null;
	}

	private static string FindGameModeInString(string gmString)
	{
		string text = null;
		for (int i = 0; i < GameMode.gameModeNames.Count; i++)
		{
			text = GameMode.gameModeNames[i];
			if (gmString.Contains(text))
			{
				break;
			}
		}
		return text;
	}

	public static bool LoadGameMode(string gameMode)
	{
		if (gameMode == null)
		{
			return false;
		}
		int num;
		if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out num))
		{
			Debug.LogWarning("Unable to find game mode key for " + gameMode);
			return false;
		}
		return GameMode.LoadGameMode(num);
	}

	public static bool LoadGameMode(int key)
	{
		if (!GameMode.gameModeTable.ContainsKey(key))
		{
			Debug.LogWarning("Missing game mode for key " + key.ToString());
			return false;
		}
		if (PhotonNetwork.InstantiateRoomObject("GameMode", Vector3.zero, Quaternion.identity, 0, new object[] { key }).IsNull())
		{
			Debug.LogWarning("Unable to create GameManager with key " + key.ToString());
			return false;
		}
		return true;
	}

	internal static bool ChangeGameMode(string gameMode)
	{
		if (gameMode == null)
		{
			return false;
		}
		int num;
		if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out num))
		{
			Debug.LogWarning("Unable to find game mode key for " + gameMode);
			return false;
		}
		return GameMode.ChangeGameMode(num);
	}

	internal static bool ChangeGameMode(int key)
	{
		GorillaGameManager gorillaGameManager;
		if (!PhotonNetwork.IsMasterClient || !GameMode.gameModeTable.TryGetValue(key, out gorillaGameManager) || gorillaGameManager == GameMode.activeGameMode)
		{
			return false;
		}
		if (GameMode.activeNetworkHandler.IsNotNull())
		{
			PhotonNetwork.Destroy(GameMode.activeNetworkHandler.gameObject);
		}
		GameMode.activeGameMode.StopPlaying();
		GameMode.activeGameMode = null;
		GameMode.activeNetworkHandler = null;
		return GameMode.LoadGameMode(key);
	}

	internal static void SetupGameModeRemote(GameModeSerializer networkSerializer)
	{
		GorillaGameManager gameModeInstance = networkSerializer.GameModeInstance;
		if (GameMode.activeGameMode.IsNotNull() && gameModeInstance.IsNotNull() && gameModeInstance != GameMode.activeGameMode)
		{
			GameMode.activeGameMode.StopPlaying();
		}
		GameMode.activeNetworkHandler = networkSerializer;
		GameMode.activeGameMode = gameModeInstance;
		GameMode.activeGameMode.NetworkLinkSetup(networkSerializer);
		GameMode.activeGameMode.StartPlaying();
	}

	internal static void RemoveNetworkLink(GameModeSerializer networkSerializer)
	{
		if (networkSerializer == GameMode.activeNetworkHandler)
		{
			GameMode.activeGameMode.NetworkLinkDestroyed(networkSerializer);
			GameMode.activeNetworkHandler = null;
			return;
		}
	}

	public static GorillaGameManager GetGameModeInstance(GameModeType type)
	{
		return GameMode.GetGameModeInstance((int)type);
	}

	public static GorillaGameManager GetGameModeInstance(int type)
	{
		GorillaGameManager gorillaGameManager;
		if (GameMode.gameModeTable.TryGetValue(type, out gorillaGameManager))
		{
			return gorillaGameManager;
		}
		return null;
	}

	public static T GetGameModeInstance<T>(GameModeType type) where T : GorillaGameManager
	{
		return GameMode.GetGameModeInstance<T>((int)type);
	}

	public static T GetGameModeInstance<T>(int type) where T : GorillaGameManager
	{
		T t = GameMode.GetGameModeInstance(type) as T;
		if (t != null)
		{
			return t;
		}
		return default(T);
	}

	public static void ResetGameModes()
	{
		GameMode.activeGameMode = null;
		GameMode.activeNetworkHandler = null;
		GameMode.optOutPlayers.Clear();
		GameMode.ParticipatingPlayers.Clear();
		for (int i = 0; i < GameMode.gameModes.Count; i++)
		{
			GorillaGameManager gorillaGameManager = GameMode.gameModes[i];
			gorillaGameManager.StopPlaying();
			gorillaGameManager.Reset();
		}
	}

	public static void ReportTag(Player player)
	{
		if (PhotonNetwork.InRoom && GameMode.activeNetworkHandler.IsNotNull())
		{
			GameMode.activeNetworkHandler.SendRPC("ReportTagRPC", false, new object[] { player });
		}
	}

	public static void ReportHit()
	{
		if (PhotonNetwork.InRoom && GameMode.activeNetworkHandler.IsNotNull())
		{
			GameMode.activeNetworkHandler.SendRPC("ReportHitRPC", false, Array.Empty<object>());
		}
	}

	public static void RefreshPlayers()
	{
		List<Player> playersInRoom = RoomSystem.PlayersInRoom;
		int num = Mathf.Min(playersInRoom.Count, 10);
		GameMode.ParticipatingPlayers.Clear();
		for (int i = 0; i < num; i++)
		{
			if (GameMode.CanParticipate(playersInRoom[i]))
			{
				GameMode.ParticipatingPlayers.Add(playersInRoom[i]);
			}
		}
	}

	public static void OptOut(VRRig rig)
	{
		GameMode.OptOut(rig.creator.ActorNumber);
	}

	public static void OptOut(Player player)
	{
		GameMode.OptOut(player.ActorNumber);
	}

	public static void OptOut(int playerActorNumber)
	{
		if (GameMode.optOutPlayers.Add(playerActorNumber))
		{
			GameMode.RefreshPlayers();
		}
	}

	public static void OptIn(VRRig rig)
	{
		GameMode.OptIn(rig.creator.ActorNumber);
	}

	public static void OptIn(Player player)
	{
		GameMode.OptIn(player.ActorNumber);
	}

	public static void OptIn(int playerActorNumber)
	{
		if (GameMode.optOutPlayers.Remove(playerActorNumber))
		{
			GameMode.RefreshPlayers();
		}
	}

	private static bool CanParticipate(Player player)
	{
		object obj;
		return player.InRoom() && !GameMode.optOutPlayers.Contains(player.ActorNumber) && (!player.CustomProperties.TryGetValue("didTutorial", out obj) || (bool)obj);
	}

	[OnEnterPlay_SetNull]
	private static GameMode instance;

	[OnEnterPlay_Clear]
	private static Dictionary<int, GorillaGameManager> gameModeTable = new Dictionary<int, GorillaGameManager>();

	[OnEnterPlay_Clear]
	private static Dictionary<string, int> gameModeKeyByName = new Dictionary<string, int>();

	[OnEnterPlay_Clear]
	private static List<GorillaGameManager> gameModes = new List<GorillaGameManager>(10);

	[OnEnterPlay_Clear]
	public static readonly List<string> gameModeNames = new List<string>(10);

	[OnEnterPlay_SetNull]
	private static GorillaGameManager activeGameMode = null;

	[OnEnterPlay_SetNull]
	private static GameModeSerializer activeNetworkHandler = null;

	private static List<Player> participatingPlayers = new List<Player>(10);

	[OnEnterPlay_Clear]
	private static readonly HashSet<int> optOutPlayers = new HashSet<int>(10);

	[OnEnterPlay_Clear]
	public static readonly List<Player> ParticipatingPlayers = new List<Player>(10);
}
