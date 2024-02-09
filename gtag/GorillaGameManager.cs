using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class GorillaGameManager : MonoBehaviour, IInRoomCallbacks, IGorillaSerializeable, ITickSystemTick
{
	public static event GorillaGameManager.OnTouchDelegate OnTouch;

	public static GorillaGameManager instance
	{
		get
		{
			return GameMode.ActiveGameMode;
		}
	}

	bool ITickSystemTick.TickRunning { get; set; }

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
					foreach (string text in PhotonNetwork.CurrentRoom.ExpectedUsers)
					{
						float num2;
						if (this.expectedUsersDecay.TryGetValue(text, out num2))
						{
							if (num2 + this.userDecayTime < Time.time)
							{
								num++;
							}
						}
						else
						{
							this.expectedUsersDecay.Add(text, Time.time);
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
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual string GameModeName()
	{
		return "NONE";
	}

	public virtual void ReportTag(Player taggedPlayer, Player taggingPlayer)
	{
	}

	public virtual void HitPlayer(Player player)
	{
	}

	public virtual bool CanAffectPlayer(Player player, bool thisFrame)
	{
		return false;
	}

	public virtual void NewVRRig(Player player, int vrrigPhotonViewID, bool didTutorial)
	{
	}

	public virtual bool LocalCanTag(Player myPlayer, Player otherPlayer)
	{
		return false;
	}

	public virtual PhotonView FindVRRigForPlayer(Player player)
	{
		VRRig vrrig = this.FindPlayerVRRig(player);
		if (vrrig == null)
		{
			return null;
		}
		return vrrig.photonView;
	}

	public virtual VRRig FindPlayerVRRig(Player player)
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

	public static VRRig StaticFindRigForPlayer(Player player)
	{
		VRRig vrrig = null;
		RigContainer rigContainer;
		if (GorillaGameManager.instance != null)
		{
			vrrig = GorillaGameManager.instance.FindPlayerVRRig(player);
		}
		else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			vrrig = rigContainer.Rig;
		}
		return vrrig;
	}

	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	public virtual int MyMatIndex(Player forPlayer)
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

	public virtual void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual void OnMasterClientSwitched(Player newMaster)
	{
	}

	public virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	public virtual void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

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

	public Player currentMasterClient;

	public PhotonView returnPhotonView;

	public VRRig returnRig;

	private Player outPlayer;

	private int outInt;

	private VRRig tempRig;

	public Player[] currentPlayerArray;

	public float[] playerSpeed = new float[2];

	private RigContainer outContainer;

	private static Action onInstanceReady;

	private static bool replicatedClientReady;

	private static Action onReplicatedClientReady;

	private GameModeSerializer serializer;

	public delegate void OnTouchDelegate(Player taggedPlayer, Player taggingPlayer);
}
