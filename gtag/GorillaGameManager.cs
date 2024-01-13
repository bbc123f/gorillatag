using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public abstract class GorillaGameManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, IPunInstantiateMagicCallback
{
	public struct ProjectileInfo
	{
		public double timeLaunched;

		public Vector3 shotVelocity;

		public Vector3 launchOrigin;

		public int projectileCount;

		public float scale;

		public ProjectileInfo(double newTime, Vector3 newVel, Vector3 origin, int newCount, float newScale)
		{
			timeLaunched = newTime;
			shotVelocity = newVel;
			launchOrigin = origin;
			projectileCount = newCount;
			scale = newScale;
		}
	}

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

	public Dictionary<Photon.Realtime.Player, List<ProjectileInfo>> playerProjectiles = new Dictionary<Photon.Realtime.Player, List<ProjectileInfo>>();

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

	private RigContainer outContainer;

	private static Action onInstanceReady;

	private static bool replicatedClientReady;

	private static Action onReplicatedClientReady;

	public virtual void Awake()
	{
		currentRoom = PhotonNetwork.CurrentRoom;
		currentPlayerArray = PhotonNetwork.PlayerList;
		DestroyInvalidManager();
		localPlayerProjectileCounter = 0;
		playerProjectiles.Add(PhotonNetwork.LocalPlayer, new List<ProjectileInfo>());
	}

	public virtual void Update()
	{
		if (instance == null)
		{
			instance = this;
			onInstanceReady?.Invoke();
			onInstanceReady = delegate
			{
			};
		}
		else if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && instance != this && PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
		if (!(lastCheck + checkCooldown < Time.time))
		{
			return;
		}
		List<string> list = new List<string>();
		Photon.Realtime.Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
		foreach (Photon.Realtime.Player player in playerListOthers)
		{
			if (!playerCosmeticsLookup.TryGetValue(player.ActorNumber, out var _))
			{
				list.Add(player.ActorNumber.ToString());
			}
		}
		if (list.Count > 0)
		{
			Debug.Log("group id to look up: " + PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper());
			PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
			{
				Keys = list,
				SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper()
			}, delegate(GetSharedGroupDataResult result)
			{
				foreach (KeyValuePair<string, SharedGroupDataRecord> datum in result.Data)
				{
					if (int.TryParse(datum.Key, out var result2))
					{
						playerCosmeticsLookup[result2] = datum.Value.Value;
					}
				}
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
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
					for (int j = 0; j < array.Length; j++)
					{
						UnityEngine.Object.Destroy(array[j]);
					}
				}
			});
		}
		lastCheck = Time.time;
		if (base.photonView.IsMine && PhotonNetwork.InRoom)
		{
			int num = 0;
			if (PhotonNetwork.CurrentRoom.ExpectedUsers != null && PhotonNetwork.CurrentRoom.ExpectedUsers.Length != 0)
			{
				string[] expectedUsers = PhotonNetwork.CurrentRoom.ExpectedUsers;
				foreach (string key in expectedUsers)
				{
					if (expectedUsersDecay.TryGetValue(key, out var value2))
					{
						if (value2 + userDecayTime < Time.time)
						{
							num++;
						}
					}
					else
					{
						expectedUsersDecay.Add(key, Time.time);
					}
				}
				if (num >= PhotonNetwork.CurrentRoom.ExpectedUsers.Length && num != 0)
				{
					PhotonNetwork.CurrentRoom.ClearExpectedUsers();
				}
			}
		}
		InfrequentUpdate();
	}

	public virtual void InfrequentUpdate()
	{
		currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public virtual string GameMode()
	{
		return "NONE";
	}

	public virtual void ReportTag(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer)
	{
	}

	public void ReportStep(VRRig steppingRig, bool isLeftHand, float velocityRatio)
	{
		float num = 0f;
		if (isLeftHand)
		{
			num = 1f;
		}
		steppingRig.photonView?.RPC("PlayHandTap", RpcTarget.All, num + Mathf.Max(velocityRatio * stepVolumeMax, stepVolumeMin));
		Debug.Log("bbbb:sending tap to " + isLeftHand.ToString() + " at volume " + Mathf.Max(velocityRatio * stepVolumeMax, stepVolumeMin));
	}

	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (info.Sender != null && !PhotonNetwork.CurrentRoom.Players.TryGetValue(info.Sender.ActorNumber, out outPlayer) && info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("trying to inappropriately create game managers", info.Sender.UserId, info.Sender.NickName);
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return;
		}
		if (info.Sender != null && instance != null && instance != this)
		{
			GorillaNot.instance.SendReport("trying to create multiple game managers", info.Sender.UserId, info.Sender.NickName);
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return;
		}
		if ((instance == null && info.Sender != null && info.Sender.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId) || (base.photonView.Owner != null && base.photonView.Owner.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId))
		{
			instance = this;
			onInstanceReady?.Invoke();
			onInstanceReady = delegate
			{
			};
		}
		else if (instance != null && instance != this)
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
			{
				Debug.Log("existing game manager! i'm host. destroying newly created manager");
				PhotonNetwork.Destroy(base.photonView);
			}
			else
			{
				Debug.Log("existing game manager! i'm not host. destroying newly created manager");
				UnityEngine.Object.Destroy(this);
			}
			return;
		}
		base.transform.parent = GorillaParent.instance.transform;
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
		return FindPlayerVRRig(player)?.photonView;
	}

	public virtual VRRig FindPlayerVRRig(Photon.Realtime.Player player)
	{
		returnRig = null;
		outContainer = null;
		if (player == null)
		{
			return null;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out outContainer))
		{
			returnRig = outContainer.Rig;
		}
		return returnRig;
	}

	public static VRRig StaticFindRigForPlayer(Photon.Realtime.Player player)
	{
		VRRig result = null;
		RigContainer playerRig;
		if (instance != null)
		{
			result = instance.FindPlayerVRRig(player);
		}
		else if (VRRigCache.Instance.TryGetVrrig(player, out playerRig))
		{
			result = playerRig.Rig;
		}
		return result;
	}

	[PunRPC]
	public virtual void ReportTagRPC(Photon.Realtime.Player taggedPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTagRPC");
		ReportTag(taggedPlayer, info.Sender);
	}

	public void AttemptGetNewPlayerCosmetic(Photon.Realtime.Player playerToUpdate, int attempts)
	{
		List<string> list = new List<string>();
		list.Add(playerToUpdate.ActorNumber.ToString());
		PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
		{
			Keys = list,
			SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper()
		}, delegate(GetSharedGroupDataResult result)
		{
			foreach (KeyValuePair<string, SharedGroupDataRecord> datum in result.Data)
			{
				if (int.TryParse(datum.Key, out var result2))
				{
					Debug.Log("for player " + playerToUpdate.UserId);
					Debug.Log("current allowed: " + playerCosmeticsLookup[result2]);
					Debug.Log("new allowed: " + datum.Value.Value);
					if (playerCosmeticsLookup[result2] != datum.Value.Value)
					{
						playerCosmeticsLookup[result2] = datum.Value.Value;
						FindPlayerVRRig(playerToUpdate).UpdateAllowedCosmetics();
						FindPlayerVRRig(playerToUpdate).SetCosmeticsActive();
						Debug.Log("success on attempt " + attempts);
					}
					else if (attempts - 1 >= 0)
					{
						Debug.Log("failure on attempt " + attempts + ". trying again");
						AttemptGetNewPlayerCosmetic(playerToUpdate, attempts - 1);
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

	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		playerVRRigDict.Remove(otherPlayer.ActorNumber);
		playerCosmeticsLookup.Remove(otherPlayer.ActorNumber);
		currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		currentPlayerArray = PhotonNetwork.PlayerList;
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		currentPlayerArray = PhotonNetwork.PlayerList;
	}

	[PunRPC]
	public void UpdatePlayerCosmetic(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UpdatePlayerCosmetic");
		AttemptGetNewPlayerCosmetic(info.Sender, 2);
	}

	[PunRPC]
	public void JoinPubWithFriends(string shufflerStr, string keyStr, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "JoinPubWithFriends");
		if (GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
		{
			startingToLookForFriend = Time.time;
			PhotonNetworkController.Instance.AttemptToFollowFriendIntoPub(info.Sender.UserId, info.Sender.ActorNumber, keyStr, shufflerStr);
		}
		else
		{
			GorillaNot.instance.SendReport("possible kick attempt", info.Sender.UserId, info.Sender.NickName);
		}
	}

	public virtual float[] LocalPlayerSpeed()
	{
		playerSpeed[0] = slowJumpLimit;
		playerSpeed[1] = slowJumpMultiplier;
		return playerSpeed;
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
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (!obj.ToString().Contains(GameMode()))
			{
				if (base.photonView.IsMine)
				{
					PhotonNetwork.Destroy(base.photonView);
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	[PunRPC]
	public void LaunchSlingshotProjectile(Vector3 slingshotLaunchLocation, Vector3 slingshotLaunchVelocity, int projHash, int trailHash, bool forLeftHand, int projectileCount, bool shouldOverrideColor, float colorR, float colorG, float colorB, float colorA, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "LaunchSlingshotProjectile");
		tempRig = FindPlayerVRRig(info.Sender);
		if (tempRig != null && (tempRig.transform.position - slingshotLaunchLocation).magnitude < tagDistanceThreshold)
		{
			tempRig.slingshot.LaunchNetworkedProjectile(slingshotLaunchLocation, slingshotLaunchVelocity, projHash, trailHash, projectileCount, tempRig.scaleFactor, shouldOverrideColor, new Color(colorR, colorG, colorB, colorA), info);
		}
	}

	[PunRPC]
	public void SpawnSlingshotPlayerImpactEffect(Vector3 hitLocation, float teamColorR, float teamColorG, float teamColorB, float teamColorA, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SpawnSlingshotPlayerImpactEffect");
		tempRig = FindPlayerVRRig(info.Sender);
		if (!(tempRig == null))
		{
			tempRig.slingshot.DestroyProjectile(projectileCount, hitLocation);
			Color color = new Color(teamColorR, teamColorG, teamColorB, teamColorA);
			GameObject obj = ObjectPools.instance.Instantiate(playerImpactEffectPrefab, hitLocation);
			obj.transform.localScale = Vector3.one * tempRig.scaleFactor;
			obj.GetComponent<GorillaColorizableBase>().SetColor(color);
		}
	}

	public int IncrementLocalPlayerProjectileCount()
	{
		localPlayerProjectileCounter++;
		return localPlayerProjectileCounter;
	}

	public static void OnInstanceReady(Action action)
	{
		GorillaParent.OnReplicatedClientReady(delegate
		{
			if ((bool)instance)
			{
				action();
			}
			else
			{
				onInstanceReady = (Action)Delegate.Combine(onInstanceReady, action);
			}
		});
	}

	public static void ReplicatedClientReady()
	{
		replicatedClientReady = true;
	}

	public static void OnReplicatedClientReady(Action action)
	{
		if (replicatedClientReady)
		{
			action();
		}
		else
		{
			onReplicatedClientReady = (Action)Delegate.Combine(onReplicatedClientReady, action);
		}
	}
}
