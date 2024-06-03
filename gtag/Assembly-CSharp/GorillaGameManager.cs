using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class GorillaGameManager : MonoBehaviour, IInRoomCallbacks, IGorillaSerializeable, ITickSystemTick
{
	public static event GorillaGameManager.OnTouchDelegate OnTouch
	{
		[CompilerGenerated]
		add
		{
			GorillaGameManager.OnTouchDelegate onTouchDelegate = GorillaGameManager.OnTouch;
			GorillaGameManager.OnTouchDelegate onTouchDelegate2;
			do
			{
				onTouchDelegate2 = onTouchDelegate;
				GorillaGameManager.OnTouchDelegate value2 = (GorillaGameManager.OnTouchDelegate)Delegate.Combine(onTouchDelegate2, value);
				onTouchDelegate = Interlocked.CompareExchange<GorillaGameManager.OnTouchDelegate>(ref GorillaGameManager.OnTouch, value2, onTouchDelegate2);
			}
			while (onTouchDelegate != onTouchDelegate2);
		}
		[CompilerGenerated]
		remove
		{
			GorillaGameManager.OnTouchDelegate onTouchDelegate = GorillaGameManager.OnTouch;
			GorillaGameManager.OnTouchDelegate onTouchDelegate2;
			do
			{
				onTouchDelegate2 = onTouchDelegate;
				GorillaGameManager.OnTouchDelegate value2 = (GorillaGameManager.OnTouchDelegate)Delegate.Remove(onTouchDelegate2, value);
				onTouchDelegate = Interlocked.CompareExchange<GorillaGameManager.OnTouchDelegate>(ref GorillaGameManager.OnTouch, value2, onTouchDelegate2);
			}
			while (onTouchDelegate != onTouchDelegate2);
		}
	}

	public static GorillaGameManager instance
	{
		get
		{
			return GameMode.ActiveGameMode;
		}
	}

	bool ITickSystemTick.TickRunning
	{
		[CompilerGenerated]
		get
		{
			return this.<ITickSystemTick.TickRunning>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<ITickSystemTick.TickRunning>k__BackingField = value;
		}
	}

	public virtual void Awake()
	{
	}

	public virtual void Tick()
	{
		if (this.lastCheck + this.checkCooldown < Time.time)
		{
			this.lastCheck = Time.time;
			if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				int num = 0;
				if (PhotonNetwork.CurrentRoom.ExpectedUsers != null && PhotonNetwork.CurrentRoom.ExpectedUsers.Length != 0)
				{
					foreach (string key in PhotonNetwork.CurrentRoom.ExpectedUsers)
					{
						float num2;
						if (this.expectedUsersDecay.TryGetValue(key, out num2))
						{
							if (num2 + this.userDecayTime < Time.time)
							{
								num++;
							}
						}
						else
						{
							this.expectedUsersDecay.Add(key, Time.time);
						}
					}
					if (num >= PhotonNetwork.CurrentRoom.ExpectedUsers.Length && num != 0)
					{
						PhotonNetwork.CurrentRoom.ClearExpectedUsers();
					}
				}
			}
			if (PhotonNetwork.IsMasterClient && !this.ValidGameMode())
			{
				GameMode.ChangeGameFromProperty();
				return;
			}
			this.InfrequentUpdate();
		}
	}

	public virtual void InfrequentUpdate()
	{
		GameMode.RefreshPlayers();
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual string GameModeName()
	{
		return "NONE";
	}

	public virtual void ReportTag(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer)
	{
	}

	public virtual void HitPlayer(Photon.Realtime.Player player)
	{
	}

	public virtual bool CanAffectPlayer(Photon.Realtime.Player player, bool thisFrame)
	{
		return false;
	}

	public virtual void NewVRRig(Photon.Realtime.Player player, int vrrigPhotonViewID, bool didTutorial)
	{
	}

	public virtual bool LocalCanTag(Photon.Realtime.Player myPlayer, Photon.Realtime.Player otherPlayer)
	{
		return false;
	}

	public virtual PhotonView FindVRRigForPlayer(Photon.Realtime.Player player)
	{
		VRRig vrrig = this.FindPlayerVRRig(player);
		if (vrrig == null)
		{
			return null;
		}
		return vrrig.photonView;
	}

	public virtual VRRig FindPlayerVRRig(Photon.Realtime.Player player)
	{
		this.returnRig = null;
		this.outContainer = null;
		if (player == null)
		{
			return null;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.outContainer))
		{
			this.returnRig = this.outContainer.Rig;
		}
		return this.returnRig;
	}

	public static VRRig StaticFindRigForPlayer(Photon.Realtime.Player player)
	{
		VRRig result = null;
		RigContainer rigContainer;
		if (GorillaGameManager.instance != null)
		{
			result = GorillaGameManager.instance.FindPlayerVRRig(player);
		}
		else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			result = rigContainer.Rig;
		}
		return result;
	}

	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	public virtual int MyMatIndex(Photon.Realtime.Player forPlayer)
	{
		return 0;
	}

	public virtual bool ValidGameMode()
	{
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out this.obj);
			return this.obj.ToString().Contains(this.GameModeName());
		}
		return false;
	}

	public static void OnInstanceReady(Action action)
	{
		GorillaParent.OnReplicatedClientReady(delegate
		{
			if (GorillaGameManager.instance)
			{
				action();
				return;
			}
			GorillaGameManager.onInstanceReady = (Action)Delegate.Combine(GorillaGameManager.onInstanceReady, action);
		});
	}

	public static void ReplicatedClientReady()
	{
		GorillaGameManager.replicatedClientReady = true;
	}

	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaGameManager.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaGameManager.onReplicatedClientReady = (Action)Delegate.Combine(GorillaGameManager.onReplicatedClientReady, action);
	}

	internal GameModeSerializer Serializer
	{
		get
		{
			return this.serializer;
		}
	}

	internal virtual void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		this.serializer = netSerializer;
	}

	internal virtual void NetworkLinkDestroyed(GameModeSerializer netSerializer)
	{
		if (this.serializer == netSerializer)
		{
			this.serializer = null;
		}
	}

	public abstract GameModeType GameType();

	public abstract void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	public abstract void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);

	public virtual void Reset()
	{
	}

	public virtual void StartPlaying()
	{
		TickSystem<object>.AddTickCallback(this);
		PhotonNetwork.AddCallbackTarget(this);
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual void StopPlaying()
	{
		TickSystem<object>.RemoveTickCallback(this);
		PhotonNetwork.RemoveCallbackTarget(this);
		this.lastCheck = 0f;
	}

	public virtual void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual void OnMasterClientSwitched(Photon.Realtime.Player newMaster)
	{
	}

	public virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	public virtual void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
	{
		if (changedProps.ContainsKey(255))
		{
			GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
		}
	}

	internal static void ForceStopGame_DisconnectAndDestroy()
	{
		Application.Quit();
		NetworkSystem instance = NetworkSystem.Instance;
		if (instance != null)
		{
			instance.ReturnToSinglePlayer();
		}
		Object.DestroyImmediate(PhotonNetworkController.Instance);
		Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	protected GorillaGameManager()
	{
	}

	[CompilerGenerated]
	private static GorillaGameManager.OnTouchDelegate OnTouch;

	public Room currentRoom;

	public object obj;

	public float fastJumpLimit;

	public float fastJumpMultiplier;

	public float slowJumpLimit;

	public float slowJumpMultiplier;

	public float lastCheck;

	public float checkCooldown = 3f;

	public float userDecayTime = 15f;

	public Dictionary<string, float> expectedUsersDecay = new Dictionary<string, float>();

	public float startingToLookForFriend;

	public float timeToSpendLookingForFriend = 10f;

	public bool successfullyFoundFriend;

	public float tagDistanceThreshold = 4f;

	public bool testAssault;

	public bool endGameManually;

	public Photon.Realtime.Player currentMasterClient;

	public PhotonView returnPhotonView;

	public VRRig returnRig;

	private Photon.Realtime.Player outPlayer;

	private int outInt;

	private VRRig tempRig;

	public Photon.Realtime.Player[] currentPlayerArray;

	public float[] playerSpeed = new float[2];

	private RigContainer outContainer;

	[CompilerGenerated]
	private bool <ITickSystemTick.TickRunning>k__BackingField;

	private static Action onInstanceReady;

	private static bool replicatedClientReady;

	private static Action onReplicatedClientReady;

	private GameModeSerializer serializer;

	public delegate void OnTouchDelegate(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer);

	[CompilerGenerated]
	private sealed class <>c__DisplayClass51_0
	{
		public <>c__DisplayClass51_0()
		{
		}

		internal void <OnInstanceReady>b__0()
		{
			if (GorillaGameManager.instance)
			{
				this.action();
				return;
			}
			GorillaGameManager.onInstanceReady = (Action)Delegate.Combine(GorillaGameManager.onInstanceReady, this.action);
		}

		public Action action;
	}
}
