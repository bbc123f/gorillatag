using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Token: 0x020001F8 RID: 504
internal class PlayerCosmeticsSystem : MonoBehaviour, ITickSystemPre
{
	// Token: 0x06000CDE RID: 3294 RVA: 0x0004C76A File Offset: 0x0004A96A
	private void Awake()
	{
		if (PlayerCosmeticsSystem.instance == null)
		{
			PlayerCosmeticsSystem.instance = this;
			Object.DontDestroyOnLoad(this);
			PhotonNetwork.NetworkingClient.EventReceived += this.OnEvent;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x0004C7A4 File Offset: 0x0004A9A4
	private void Start()
	{
		this.playerLookUpCooldown = Mathf.Max(this.playerLookUpCooldown, 3f);
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x0004C7BC File Offset: 0x0004A9BC
	private void OnDestroy()
	{
		if (PlayerCosmeticsSystem.instance == this)
		{
			PlayerCosmeticsSystem.instance = null;
		}
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x0004C7D1 File Offset: 0x0004A9D1
	private void LookUpPlayerCosmetics(bool wait = false)
	{
		if (!this.isLookingUp)
		{
			TickSystem.AddPreTickCallback(this);
			if (wait)
			{
				this.startSearchingTime = Time.time;
				return;
			}
			this.startSearchingTime = float.MinValue;
		}
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x0004C7FC File Offset: 0x0004A9FC
	public void PreTick()
	{
		if (PlayerCosmeticsSystem.playersToLookUp.Count < 1)
		{
			TickSystem.RemovePreTickCallback(this);
			this.startSearchingTime = float.MinValue;
			this.isLookingUp = false;
			return;
		}
		this.isLookingUp = true;
		if (this.startSearchingTime + this.playerLookUpCooldown > Time.time)
		{
			return;
		}
		Debug.Log("attempting to get player cosmetics");
		PlayerCosmeticsSystem.playerIDsList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			Photon.Realtime.Player player = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string item = player.ActorNumber.ToString();
			if (player.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(item))
			{
				PlayerCosmeticsSystem.playerIDsList.Add(item);
				PlayerCosmeticsSystem.playersWaiting.Add(player.ActorNumber);
			}
		}
		if (PlayerCosmeticsSystem.playerIDsList.Count > 0)
		{
			Debug.Log("group id to look up: " + PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper());
			GetSharedGroupDataRequest getSharedGroupDataRequest = new GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = PlayerCosmeticsSystem.playerIDsList;
			getSharedGroupDataRequest.SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper();
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, new Action<GetSharedGroupDataResult>(this.OnGetsharedGroupData), delegate(PlayFabError error)
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
		this.isLookingUp = false;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0004C974 File Offset: 0x0004AB74
	private void OnGetsharedGroupData(GetSharedGroupDataResult result)
	{
		if (!PhotonNetwork.InRoom)
		{
			PlayerCosmeticsSystem.playersWaiting.Clear();
			return;
		}
		bool flag = false;
		foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
		{
			this.playerTemp = null;
			int num;
			if (int.TryParse(keyValuePair.Key, out num))
			{
				if (!Utils.PlayerInRoom(num))
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num);
				}
				else
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num);
					this.playerTemp = PhotonNetwork.LocalPlayer.Get(num);
					this.tempCosmetics = keyValuePair.Value.Value;
					IUserCosmeticsCallback userCosmeticsCallback;
					if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(num, out userCosmeticsCallback))
					{
						PlayerCosmeticsSystem.userCosmeticsWaiting[num] = this.tempCosmetics;
					}
					else
					{
						userCosmeticsCallback.PendingUpdate = false;
						if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
						{
							Debug.Log("retrying cosmetics for " + this.playerTemp.ToStringFull());
							PlayerCosmeticsSystem.playersToLookUp.Enqueue(this.playerTemp);
							flag = true;
							userCosmeticsCallback.PendingUpdate = true;
						}
					}
				}
			}
		}
		if (PlayerCosmeticsSystem.playersWaiting.Count > 0)
		{
			Debug.Log("didn't recieve player cosmetics");
			foreach (int num2 in PlayerCosmeticsSystem.playersWaiting)
			{
				if (!Utils.PlayerInRoom(num2))
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num2);
				}
				else
				{
					Debug.Log(num2);
					PlayerCosmeticsSystem.playersToLookUp.Enqueue(PhotonNetwork.LocalPlayer.Get(num2));
					flag = true;
				}
			}
		}
		if (flag)
		{
			this.LookUpPlayerCosmetics(true);
		}
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x0004CB44 File Offset: 0x0004AD44
	private void OnEvent(EventData evData)
	{
		if (evData.Code != 199 || evData.Sender < 1)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(new PhotonMessageInfo(PhotonNetwork.LocalPlayer.Get(evData.Sender), PhotonNetwork.ServerTimestamp, null), "UpdatePlayerCosmetics");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(PhotonNetwork.LocalPlayer.Get(evData.Sender));
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x0004CBA2 File Offset: 0x0004ADA2
	private static bool nullInstance
	{
		get
		{
			return PlayerCosmeticsSystem.instance == null || !PlayerCosmeticsSystem.instance;
		}
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x0004CBBC File Offset: 0x0004ADBC
	public static void RegisterCosmeticCallback(int playerID, IUserCosmeticsCallback callback)
	{
		PlayerCosmeticsSystem.userCosmeticCallback[playerID] = callback;
		string cosmetics;
		if (PlayerCosmeticsSystem.userCosmeticsWaiting.TryGetValue(playerID, out cosmetics))
		{
			callback.PendingUpdate = false;
			callback.OnGetUserCosmetics(cosmetics);
			PlayerCosmeticsSystem.userCosmeticsWaiting.Remove(playerID);
		}
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0004CBFF File Offset: 0x0004ADFF
	public static void RemoveCosmeticCallback(int playerID)
	{
		if (PlayerCosmeticsSystem.userCosmeticCallback.ContainsKey(playerID))
		{
			PlayerCosmeticsSystem.userCosmeticCallback.Remove(playerID);
		}
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0004CC1C File Offset: 0x0004AE1C
	public static void UpdatePlayerCosmetics(Photon.Realtime.Player player)
	{
		if (player == null || player.IsLocal)
		{
			return;
		}
		PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
		IUserCosmeticsCallback userCosmeticsCallback;
		if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(player.ActorNumber, out userCosmeticsCallback))
		{
			userCosmeticsCallback.PendingUpdate = true;
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(true);
		}
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0004CC70 File Offset: 0x0004AE70
	public static void UpdatePlayerCosmetics(List<Photon.Realtime.Player> players)
	{
		foreach (Photon.Realtime.Player player in players)
		{
			if (player != null && !player.IsLocal)
			{
				PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
				IUserCosmeticsCallback userCosmeticsCallback;
				if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(player.ActorNumber, out userCosmeticsCallback))
				{
					userCosmeticsCallback.PendingUpdate = true;
				}
			}
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(false);
		}
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x0004CCFC File Offset: 0x0004AEFC
	public static void StaticReset()
	{
		PlayerCosmeticsSystem.playersToLookUp.Clear();
		PlayerCosmeticsSystem.userCosmeticCallback.Clear();
		PlayerCosmeticsSystem.userCosmeticsWaiting.Clear();
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playersWaiting.Clear();
	}

	// Token: 0x04001023 RID: 4131
	public float playerLookUpCooldown = 3f;

	// Token: 0x04001024 RID: 4132
	private float startSearchingTime = float.MinValue;

	// Token: 0x04001025 RID: 4133
	private bool isLookingUp;

	// Token: 0x04001026 RID: 4134
	private string tempCosmetics;

	// Token: 0x04001027 RID: 4135
	private Photon.Realtime.Player playerTemp;

	// Token: 0x04001028 RID: 4136
	private RigContainer tempRC;

	// Token: 0x04001029 RID: 4137
	private static PlayerCosmeticsSystem instance;

	// Token: 0x0400102A RID: 4138
	private static Queue<Photon.Realtime.Player> playersToLookUp = new Queue<Photon.Realtime.Player>(10);

	// Token: 0x0400102B RID: 4139
	private static Dictionary<int, IUserCosmeticsCallback> userCosmeticCallback = new Dictionary<int, IUserCosmeticsCallback>(10);

	// Token: 0x0400102C RID: 4140
	private static Dictionary<int, string> userCosmeticsWaiting = new Dictionary<int, string>(5);

	// Token: 0x0400102D RID: 4141
	private static List<string> playerIDsList = new List<string>(10);

	// Token: 0x0400102E RID: 4142
	private static HashSet<int> playersWaiting = new HashSet<int>();
}
