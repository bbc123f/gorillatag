using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Token: 0x02000172 RID: 370
public abstract class GorillaGameManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, IPunInstantiateMagicCallback
{
	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000938 RID: 2360 RVA: 0x00037EA0 File Offset: 0x000360A0
	// (remove) Token: 0x06000939 RID: 2361 RVA: 0x00037ED4 File Offset: 0x000360D4
	public static event GorillaGameManager.OnTouchDelegate OnTouch;

	// Token: 0x0600093A RID: 2362 RVA: 0x00037F07 File Offset: 0x00036107
	public virtual void Awake()
	{
		this.currentRoom = PhotonNetwork.CurrentRoom;
		this.currentPlayerArray = PhotonNetwork.PlayerList;
		this.DestroyInvalidManager();
		this.localPlayerProjectileCounter = 0;
		this.playerProjectiles.Add(PhotonNetwork.LocalPlayer, new List<GorillaGameManager.ProjectileInfo>());
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00037F44 File Offset: 0x00036144
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

	// Token: 0x0600093C RID: 2364 RVA: 0x000380A0 File Offset: 0x000362A0
	public virtual void InfrequentUpdate()
	{
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x000380AD File Offset: 0x000362AD
	public virtual string GameMode()
	{
		return "NONE";
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x000380B4 File Offset: 0x000362B4
	public virtual void ReportTag(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer)
	{
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x000380B6 File Offset: 0x000362B6
	public virtual void ReportTouch(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer)
	{
		Debug.Log("Dispatching Touch");
		if (GorillaGameManager.OnTouch != null)
		{
			GorillaGameManager.OnTouch(taggedPlayer, taggingPlayer);
		}
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x000380D5 File Offset: 0x000362D5
	public virtual void ReportContactWithLava(Photon.Realtime.Player player)
	{
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x000380D7 File Offset: 0x000362D7
	public virtual bool LavaWouldAffectPlayer(Photon.Realtime.Player player, bool enteredLavaThisFrame)
	{
		return false;
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x000380DC File Offset: 0x000362DC
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

	// Token: 0x06000943 RID: 2371 RVA: 0x00038168 File Offset: 0x00036368
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

	// Token: 0x06000944 RID: 2372 RVA: 0x000383A6 File Offset: 0x000365A6
	public virtual void NewVRRig(Photon.Realtime.Player player, int vrrigPhotonViewID, bool didTutorial)
	{
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x000383A8 File Offset: 0x000365A8
	public virtual bool LocalCanTag(Photon.Realtime.Player myPlayer, Photon.Realtime.Player otherPlayer)
	{
		return false;
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x000383AB File Offset: 0x000365AB
	public virtual PhotonView FindVRRigForPlayer(Photon.Realtime.Player player)
	{
		VRRig vrrig = this.FindPlayerVRRig(player);
		if (vrrig == null)
		{
			return null;
		}
		return vrrig.photonView;
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x000383BF File Offset: 0x000365BF
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

	// Token: 0x06000948 RID: 2376 RVA: 0x00038400 File Offset: 0x00036600
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

	// Token: 0x06000949 RID: 2377 RVA: 0x00038445 File Offset: 0x00036645
	[PunRPC]
	public virtual void ReportTagRPC(Photon.Realtime.Player taggedPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTagRPC");
		this.ReportTag(taggedPlayer, info.Sender);
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00038460 File Offset: 0x00036660
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

	// Token: 0x0600094B RID: 2379 RVA: 0x000384EC File Offset: 0x000366EC
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

	// Token: 0x0600094C RID: 2380 RVA: 0x0003859F File Offset: 0x0003679F
	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		this.playerVRRigDict.Remove(otherPlayer.ActorNumber);
		this.playerCosmeticsLookup.Remove(otherPlayer.ActorNumber);
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x000385D7 File Offset: 0x000367D7
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x000385EB File Offset: 0x000367EB
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.currentPlayerArray = PhotonNetwork.PlayerList;
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x000385FE File Offset: 0x000367FE
	[PunRPC]
	public void UpdatePlayerCosmetic(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UpdatePlayerCosmetic");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(info.Sender);
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00038618 File Offset: 0x00036818
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

	// Token: 0x06000951 RID: 2385 RVA: 0x000386A9 File Offset: 0x000368A9
	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x000386D0 File Offset: 0x000368D0
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

	// Token: 0x06000953 RID: 2387 RVA: 0x00038704 File Offset: 0x00036904
	public virtual int MyMatIndex(Photon.Realtime.Player forPlayer)
	{
		return 0;
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00038708 File Offset: 0x00036908
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

	// Token: 0x06000955 RID: 2389 RVA: 0x00038780 File Offset: 0x00036980
	[PunRPC]
	public void LaunchSlingshotProjectile(Vector3 slingshotLaunchLocation, Vector3 slingshotLaunchVelocity, int projHash, int trailHash, bool forLeftHand, int projectileCount, bool shouldOverrideColor, float colorR, float colorG, float colorB, float colorA, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "LaunchSlingshotProjectile");
		this.tempRig = this.FindPlayerVRRig(info.Sender);
		if (this.tempRig != null && (this.tempRig.transform.position - slingshotLaunchLocation).magnitude < this.tagDistanceThreshold)
		{
			this.tempRig.slingshot.LaunchNetworkedProjectile(slingshotLaunchLocation, slingshotLaunchVelocity, projHash, trailHash, projectileCount, this.tempRig.scaleFactor, shouldOverrideColor, new Color(colorR, colorG, colorB, colorA), info);
		}
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00038814 File Offset: 0x00036A14
	[PunRPC]
	public void SpawnSlingshotPlayerImpactEffect(Vector3 hitLocation, float teamColorR, float teamColorG, float teamColorB, float teamColorA, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SpawnSlingshotPlayerImpactEffect");
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

	// Token: 0x06000957 RID: 2391 RVA: 0x000388AB File Offset: 0x00036AAB
	public int IncrementLocalPlayerProjectileCount()
	{
		this.localPlayerProjectileCounter++;
		return this.localPlayerProjectileCounter;
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x000388C1 File Offset: 0x00036AC1
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

	// Token: 0x06000959 RID: 2393 RVA: 0x000388DF File Offset: 0x00036ADF
	public static void ReplicatedClientReady()
	{
		GorillaGameManager.replicatedClientReady = true;
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x000388E7 File Offset: 0x00036AE7
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaGameManager.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaGameManager.onReplicatedClientReady = (Action)Delegate.Combine(GorillaGameManager.onReplicatedClientReady, action);
	}

	// Token: 0x04000B44 RID: 2884
	public static volatile GorillaGameManager instance;

	// Token: 0x04000B45 RID: 2885
	public Room currentRoom;

	// Token: 0x04000B46 RID: 2886
	public object obj;

	// Token: 0x04000B47 RID: 2887
	public float stepVolumeMax = 0.2f;

	// Token: 0x04000B48 RID: 2888
	public float stepVolumeMin = 0.05f;

	// Token: 0x04000B49 RID: 2889
	public float fastJumpLimit;

	// Token: 0x04000B4A RID: 2890
	public float fastJumpMultiplier;

	// Token: 0x04000B4B RID: 2891
	public float slowJumpLimit;

	// Token: 0x04000B4C RID: 2892
	public float slowJumpMultiplier;

	// Token: 0x04000B4D RID: 2893
	public byte roomSize;

	// Token: 0x04000B4E RID: 2894
	public float lastCheck;

	// Token: 0x04000B4F RID: 2895
	public float checkCooldown = 3f;

	// Token: 0x04000B50 RID: 2896
	public float userDecayTime = 15f;

	// Token: 0x04000B51 RID: 2897
	public Dictionary<int, VRRig> playerVRRigDict = new Dictionary<int, VRRig>();

	// Token: 0x04000B52 RID: 2898
	public Dictionary<string, float> expectedUsersDecay = new Dictionary<string, float>();

	// Token: 0x04000B53 RID: 2899
	public Dictionary<int, string> playerCosmeticsLookup = new Dictionary<int, string>();

	// Token: 0x04000B54 RID: 2900
	public string tempString;

	// Token: 0x04000B55 RID: 2901
	public float startingToLookForFriend;

	// Token: 0x04000B56 RID: 2902
	public float timeToSpendLookingForFriend = 10f;

	// Token: 0x04000B57 RID: 2903
	public bool successfullyFoundFriend;

	// Token: 0x04000B58 RID: 2904
	public int maxProjectilesToKeepTrackOfPerPlayer = 50;

	// Token: 0x04000B59 RID: 2905
	public GameObject playerImpactEffectPrefab;

	// Token: 0x04000B5A RID: 2906
	private int localPlayerProjectileCounter;

	// Token: 0x04000B5B RID: 2907
	public Dictionary<Photon.Realtime.Player, List<GorillaGameManager.ProjectileInfo>> playerProjectiles = new Dictionary<Photon.Realtime.Player, List<GorillaGameManager.ProjectileInfo>>();

	// Token: 0x04000B5C RID: 2908
	public float tagDistanceThreshold = 8f;

	// Token: 0x04000B5D RID: 2909
	public bool testAssault;

	// Token: 0x04000B5E RID: 2910
	public bool endGameManually;

	// Token: 0x04000B5F RID: 2911
	public Photon.Realtime.Player currentMasterClient;

	// Token: 0x04000B60 RID: 2912
	public PhotonView returnPhotonView;

	// Token: 0x04000B61 RID: 2913
	public VRRig returnRig;

	// Token: 0x04000B62 RID: 2914
	private Photon.Realtime.Player outPlayer;

	// Token: 0x04000B63 RID: 2915
	private int outInt;

	// Token: 0x04000B64 RID: 2916
	private VRRig tempRig;

	// Token: 0x04000B65 RID: 2917
	public Photon.Realtime.Player[] currentPlayerArray;

	// Token: 0x04000B66 RID: 2918
	public float[] playerSpeed = new float[2];

	// Token: 0x04000B67 RID: 2919
	public List<string> prefabsToInstantiateByPath = new List<string>();

	// Token: 0x04000B68 RID: 2920
	private List<GameObject> prefabsInstantiated = new List<GameObject>();

	// Token: 0x04000B69 RID: 2921
	private RigContainer outContainer;

	// Token: 0x04000B6A RID: 2922
	private static Action onInstanceReady;

	// Token: 0x04000B6B RID: 2923
	private static bool replicatedClientReady;

	// Token: 0x04000B6C RID: 2924
	private static Action onReplicatedClientReady;

	// Token: 0x0200041E RID: 1054
	// (Invoke) Token: 0x06001C53 RID: 7251
	public delegate void OnTouchDelegate(Photon.Realtime.Player taggedPlayer, Photon.Realtime.Player taggingPlayer);

	// Token: 0x0200041F RID: 1055
	public struct ProjectileInfo
	{
		// Token: 0x06001C56 RID: 7254 RVA: 0x00097684 File Offset: 0x00095884
		public ProjectileInfo(double newTime, Vector3 newVel, Vector3 origin, int newCount, float newScale)
		{
			this.timeLaunched = newTime;
			this.shotVelocity = newVel;
			this.launchOrigin = origin;
			this.projectileCount = newCount;
			this.scale = newScale;
		}

		// Token: 0x04001D17 RID: 7447
		public double timeLaunched;

		// Token: 0x04001D18 RID: 7448
		public Vector3 shotVelocity;

		// Token: 0x04001D19 RID: 7449
		public Vector3 launchOrigin;

		// Token: 0x04001D1A RID: 7450
		public int projectileCount;

		// Token: 0x04001D1B RID: 7451
		public float scale;
	}
}
