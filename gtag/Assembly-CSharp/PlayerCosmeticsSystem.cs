using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

internal class PlayerCosmeticsSystem : MonoBehaviour, ITickSystemPre
{
	bool ITickSystemPre.PreTickRunning
	{
		[CompilerGenerated]
		get
		{
			return this.<ITickSystemPre.PreTickRunning>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<ITickSystemPre.PreTickRunning>k__BackingField = value;
		}
	}

	private void Awake()
	{
		if (PlayerCosmeticsSystem.instance == null)
		{
			PlayerCosmeticsSystem.instance = this;
			base.transform.SetParent(null, true);
			Object.DontDestroyOnLoad(this);
			PhotonNetwork.NetworkingClient.EventReceived += this.OnEvent;
			return;
		}
		Object.Destroy(this);
	}

	private void Start()
	{
		this.playerLookUpCooldown = Mathf.Max(this.playerLookUpCooldown, 3f);
	}

	private void OnDestroy()
	{
		if (PlayerCosmeticsSystem.instance == this)
		{
			PlayerCosmeticsSystem.instance = null;
		}
	}

	private void LookUpPlayerCosmetics(bool wait = false)
	{
		if (!this.isLookingUp)
		{
			TickSystem<object>.AddPreTickCallback(this);
			if (wait)
			{
				this.startSearchingTime = Time.time;
				return;
			}
			this.startSearchingTime = float.MinValue;
		}
	}

	public void PreTick()
	{
		if (PlayerCosmeticsSystem.playersToLookUp.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
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
			Player player = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
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
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
		}
		this.isLookingUp = false;
	}

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

	private void OnEvent(EventData evData)
	{
		if (evData.Code != 199 || evData.Sender < 1)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(new PhotonMessageInfo(PhotonNetwork.LocalPlayer.Get(evData.Sender), PhotonNetwork.ServerTimestamp, null), "UpdatePlayerCosmetics");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(PhotonNetwork.LocalPlayer.Get(evData.Sender));
	}

	private static bool nullInstance
	{
		get
		{
			return PlayerCosmeticsSystem.instance == null || !PlayerCosmeticsSystem.instance;
		}
	}

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

	public static void RemoveCosmeticCallback(int playerID)
	{
		if (PlayerCosmeticsSystem.userCosmeticCallback.ContainsKey(playerID))
		{
			PlayerCosmeticsSystem.userCosmeticCallback.Remove(playerID);
		}
	}

	public static void UpdatePlayerCosmetics(Player player)
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

	public static void UpdatePlayerCosmetics(List<Player> players)
	{
		foreach (Player player in players)
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

	public static void StaticReset()
	{
		PlayerCosmeticsSystem.playersToLookUp.Clear();
		PlayerCosmeticsSystem.userCosmeticCallback.Clear();
		PlayerCosmeticsSystem.userCosmeticsWaiting.Clear();
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playersWaiting.Clear();
	}

	public PlayerCosmeticsSystem()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static PlayerCosmeticsSystem()
	{
	}

	[CompilerGenerated]
	private bool <ITickSystemPre.PreTickRunning>k__BackingField;

	public float playerLookUpCooldown = 3f;

	private float startSearchingTime = float.MinValue;

	private bool isLookingUp;

	private string tempCosmetics;

	private Player playerTemp;

	private RigContainer tempRC;

	private static PlayerCosmeticsSystem instance;

	private static Queue<Player> playersToLookUp = new Queue<Player>(10);

	private static Dictionary<int, IUserCosmeticsCallback> userCosmeticCallback = new Dictionary<int, IUserCosmeticsCallback>(10);

	private static Dictionary<int, string> userCosmeticsWaiting = new Dictionary<int, string>(5);

	private static List<string> playerIDsList = new List<string>(10);

	private static HashSet<int> playersWaiting = new HashSet<int>();

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal void <PreTick>b__14_0(PlayFabError error)
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
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
		}

		public static readonly PlayerCosmeticsSystem.<>c <>9 = new PlayerCosmeticsSystem.<>c();

		public static Action<PlayFabError> <>9__14_0;
	}
}
