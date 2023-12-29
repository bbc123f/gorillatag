using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public abstract class GorillaGameManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, IPunInstantiateMagicCallback
{
	public static event GorillaGameManager.OnTouchDelegate OnTouch;

	public virtual void Awake()
	{
		this.currentRoom = PhotonNetwork.CurrentRoom;
		this.currentPlayerArray = PhotonNetwork.PlayerList;
		this.DestroyInvalidManager();
		this.localPlayerProjectileCounter = 0;
		this.playerProjectiles.Add(PhotonNetwork.LocalPlayer, new List<GorillaGameManager.ProjectileInfo>());
	}

	public virtual void Update()
	{
		if (GorillaGameManager.instance == null)
		{
			GorillaGameManager.instance = this;
			Action action = GorillaGameManager.onInstanceReady;
			if (action != null)
			{
				action();
			}
			GorillaGameManager.onInstanceReady = delegate()
			{
			};
		}
		else if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && GorillaGameManager.instance != this && PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
		if (this.lastCheck + this.checkCooldown < Time.time)
		{
			this.lastCheck = Time.time;
			if (base.photonView.IsMine && PhotonNetwork.InRoom)
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
			this.InfrequentUpdate();
		}
	}

	public virtual void InfrequentUpdate()
	{
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual string GameMode()
	{
		return "NONE";
	}

	public virtual void ReportTag(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer)
	{
	}

	public virtual void ReportTouch(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer)
	{
		Debug.Log("Dispatching Touch");
		if (GorillaGameManager.OnTouch != null)
		{
			GorillaGameManager.OnTouch(taggedPlayer, taggingPlayer);
		}
	}

	public virtual void ReportContactWithLava(Photon.Realtime.Player player)
	{
	}

	public virtual bool LavaWouldAffectPlayer(Photon.Realtime.Player player, bool enteredLavaThisFrame)
	{
		return false;
	}

	public void ReportStep(VRRig steppingRig, bool isLeftHand, float velocityRatio)
	{
		float num = 0f;
		if (isLeftHand)
		{
			num = 1f;
		}
		PhotonView photonView = steppingRig.photonView;
		if (photonView != null)
		{
			photonView.RPC("PlayHandTap", RpcTarget.All, new object[]
			{
				num + Mathf.Max(velocityRatio * this.stepVolumeMax, this.stepVolumeMin)
			});
		}
		Debug.Log("bbbb:sending tap to " + isLeftHand.ToString() + " at volume " + Mathf.Max(velocityRatio * this.stepVolumeMax, this.stepVolumeMin).ToString());
	}

	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "OnPhotonInstantiate");
		if (info.Sender != null && !PhotonNetwork.CurrentRoom.Players.TryGetValue(info.Sender.ActorNumber, out this.outPlayer) && info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("trying to inappropriately create game managers", info.Sender.UserId, info.Sender.NickName);
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
			Object.Destroy(base.gameObject);
			return;
		}
		else
		{
			if (info.Sender == null || !(GorillaGameManager.instance != null) || !(GorillaGameManager.instance != this))
			{
				if ((GorillaGameManager.instance == null && info.Sender != null && info.Sender.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId) || (base.photonView.Owner != null && base.photonView.Owner.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId))
				{
					GorillaGameManager.instance = this;
					Action action = GorillaGameManager.onInstanceReady;
					if (action != null)
					{
						action();
					}
					GorillaGameManager.onInstanceReady = delegate()
					{
					};
				}
				else if (GorillaGameManager.instance != null && GorillaGameManager.instance != this)
				{
					if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
					{
						Debug.Log("existing game manager! i'm host. destroying newly created manager");
						PhotonNetwork.Destroy(base.photonView);
						return;
					}
					Debug.Log("existing game manager! i'm not host. destroying newly created manager");
					Object.Destroy(this);
					return;
				}
				base.transform.parent = GorillaParent.instance.transform;
				for (int i = 0; i < this.prefabsToInstantiateByPath.Count; i++)
				{
					this.prefabsInstantiated.Add(PhotonNetwork.InstantiateRoomObject(this.prefabsToInstantiateByPath[i], Vector3.zero, Quaternion.identity, 0, null));
				}
				return;
			}
			GorillaNot.instance.SendReport("trying to create multiple game managers", info.Sender.UserId, info.Sender.NickName);
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
			Object.Destroy(base.gameObject);
			return;
		}
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

	[PunRPC]
	public virtual void ReportTagRPC(Photon.Realtime.Player taggedPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTagRPC");
		this.ReportTag(taggedPlayer, info.Sender);
	}

	[PunRPC]
	public virtual void ReportContactWithLavaRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportContactWithLavaRPC");
		InfectionLavaController infectionLavaController = InfectionLavaController.Instance;
		VRRig vrrig = this.FindPlayerVRRig(info.Sender);
		if (vrrig != null && infectionLavaController != null && infectionLavaController.LavaCurrentlyActivated && (infectionLavaController.SurfaceCenter - vrrig.syncPos).sqrMagnitude < 2500f && infectionLavaController.LavaPlane.GetDistanceToPoint(vrrig.syncPos) < 5f)
		{
			this.ReportContactWithLava(info.Sender);
		}
	}

	public void AttemptGetNewPlayerCosmetic(Photon.Realtime.Player playerToUpdate, int attempts)
	{
		List<string> list = new List<string>();
		list.Add(playerToUpdate.ActorNumber.ToString());
		GetSharedGroupDataRequest getSharedGroupDataRequest = new GetSharedGroupDataRequest();
		getSharedGroupDataRequest.Keys = list;
		getSharedGroupDataRequest.SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper();
		PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
		{
			foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
			{
				int key;
				if (int.TryParse(keyValuePair.Key, out key))
				{
					Debug.Log("for player " + playerToUpdate.UserId);
					Debug.Log("current allowed: " + this.playerCosmeticsLookup[key]);
					Debug.Log("new allowed: " + keyValuePair.Value.Value);
					if (this.playerCosmeticsLookup[key] != keyValuePair.Value.Value)
					{
						this.playerCosmeticsLookup[key] = keyValuePair.Value.Value;
						this.FindPlayerVRRig(playerToUpdate).UpdateAllowedCosmetics();
						this.FindPlayerVRRig(playerToUpdate).SetCosmeticsActive();
						Debug.Log("success on attempt " + attempts.ToString());
					}
					else if (attempts - 1 >= 0)
					{
						Debug.Log("failure on attempt " + attempts.ToString() + ". trying again");
						this.AttemptGetNewPlayerCosmetic(playerToUpdate, attempts - 1);
					}
				}
			}
		}, delegate(PlayFabError error)
		{
			Debug.Log("Got error retrieving user data:");
			Debug.Log(error.GenerateErrorReport());
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				Object.DestroyImmediate(PhotonNetworkController.Instance);
				Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
			}
		}, null, null);
	}

	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		this.playerVRRigDict.Remove(otherPlayer.ActorNumber);
		this.playerCosmeticsLookup.Remove(otherPlayer.ActorNumber);
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	[PunRPC]
	public void UpdatePlayerCosmetic(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UpdatePlayerCosmetic");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(info.Sender);
	}

	[PunRPC]
	public void JoinPubWithFriends(string shufflerStr, string keyStr, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "JoinPubWithFriends");
		if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
		{
			this.startingToLookForFriend = Time.time;
			PhotonNetworkController.Instance.AttemptToFollowFriendIntoPub(info.Sender.UserId, info.Sender.ActorNumber, keyStr, shufflerStr);
			return;
		}
		GorillaNot.instance.SendReport("possible kick attempt", info.Sender.UserId, info.Sender.NickName);
	}

	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	public bool FindUserIDInRoom(string userID)
	{
		Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerList;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].UserId == userID)
			{
				return false;
			}
		}
		return true;
	}

	public virtual int MyMatIndex(Photon.Realtime.Player forPlayer)
	{
		return 0;
	}

	public virtual void DestroyInvalidManager()
	{
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out this.obj);
			if (!this.obj.ToString().Contains(this.GameMode()))
			{
				if (base.photonView.IsMine)
				{
					PhotonNetwork.Destroy(base.photonView);
					return;
				}
				Object.Destroy(base.gameObject);
				return;
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	[PunRPC]
	public void LaunchSlingshotProjectile(Vector3 slingshotLaunchLocation, Vector3 slingshotLaunchVelocity, int projHash, int trailHash, bool forLeftHand, int projectileCount, bool shouldOverrideColor, float colorR, float colorG, float colorB, float colorA, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "LaunchSlingshotProjectile");
		if (Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime)) > 1f)
		{
			return;
		}
		if (!slingshotLaunchLocation.IsValid() || !slingshotLaunchVelocity.IsValid() || !float.IsFinite(colorR) || !float.IsFinite(colorG) || !float.IsFinite(colorB) || !float.IsFinite(colorA))
		{
			GorillaNot.instance.SendReport("invalid projectile state", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		this.tempRig = this.FindPlayerVRRig(info.Sender);
		if (this.tempRig != null && (this.tempRig.transform.position - slingshotLaunchLocation).magnitude < this.tagDistanceThreshold)
		{
			this.tempRig.slingshot.LaunchNetworkedProjectile(slingshotLaunchLocation, slingshotLaunchVelocity, projHash, trailHash, projectileCount, this.tempRig.scaleFactor, shouldOverrideColor, new Color(colorR, colorG, colorB, colorA), info);
		}
	}

	[PunRPC]
	public void SpawnSlingshotPlayerImpactEffect(Vector3 hitLocation, float teamColorR, float teamColorG, float teamColorB, float teamColorA, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SpawnSlingshotPlayerImpactEffect");
		if (Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime)) > 1f)
		{
			return;
		}
		if (!hitLocation.IsValid() || !float.IsFinite(teamColorR) || !float.IsFinite(teamColorG) || !float.IsFinite(teamColorB) || !float.IsFinite(teamColorA))
		{
			GorillaNot.instance.SendReport("invalid impact state", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		this.tempRig = this.FindPlayerVRRig(info.Sender);
		if (this.tempRig == null)
		{
			return;
		}
		this.tempRig.slingshot.DestroyProjectile(projectileCount, hitLocation);
		Color color = new Color(teamColorR, teamColorG, teamColorB, teamColorA);
		GameObject gameObject = ObjectPools.instance.Instantiate(this.playerImpactEffectPrefab, hitLocation);
		gameObject.transform.localScale = Vector3.one * this.tempRig.scaleFactor;
		gameObject.GetComponent<GorillaColorizableBase>().SetColor(color);
	}

	public int IncrementLocalPlayerProjectileCount()
	{
		this.localPlayerProjectileCounter++;
		return this.localPlayerProjectileCounter;
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

	[OnEnterPlay_SetNull]
	public static volatile GorillaGameManager instance;

	public Room currentRoom;

	public object obj;

	public float stepVolumeMax = 0.2f;

	public float stepVolumeMin = 0.05f;

	public float fastJumpLimit;

	public float fastJumpMultiplier;

	public float slowJumpLimit;

	public float slowJumpMultiplier;

	public byte roomSize;

	public float lastCheck;

	public float checkCooldown = 3f;

	public float userDecayTime = 15f;

	public Dictionary<int, VRRig> playerVRRigDict = new Dictionary<int, VRRig>();

	public Dictionary<string, float> expectedUsersDecay = new Dictionary<string, float>();

	public Dictionary<int, string> playerCosmeticsLookup = new Dictionary<int, string>();

	public string tempString;

	public float startingToLookForFriend;

	public float timeToSpendLookingForFriend = 10f;

	public bool successfullyFoundFriend;

	public int maxProjectilesToKeepTrackOfPerPlayer = 50;

	public GameObject playerImpactEffectPrefab;

	private int localPlayerProjectileCounter;

	public Dictionary<Photon.Realtime.Player, List<GorillaGameManager.ProjectileInfo>> playerProjectiles = new Dictionary<Photon.Realtime.Player, List<GorillaGameManager.ProjectileInfo>>();

	public float tagDistanceThreshold = 8f;

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

	public List<string> prefabsToInstantiateByPath = new List<string>();

	private List<GameObject> prefabsInstantiated = new List<GameObject>();

	private RigContainer outContainer;

	private static Action onInstanceReady;

	private static bool replicatedClientReady;

	private static Action onReplicatedClientReady;

	public delegate void OnTouchDelegate(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer);

	public struct ProjectileInfo
	{
		public ProjectileInfo(double newTime, Vector3 newVel, Vector3 origin, int newCount, float newScale)
		{
			this.timeLaunched = newTime;
			this.shotVelocity = newVel;
			this.launchOrigin = origin;
			this.projectileCount = newCount;
			this.scale = newScale;
		}

		public double timeLaunched;

		public Vector3 shotVelocity;

		public Vector3 launchOrigin;

		public int projectileCount;

		public float scale;
	}
}
