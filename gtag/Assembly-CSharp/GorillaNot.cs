using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class GorillaNot : MonoBehaviourPunCallbacks
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060009AE RID: 2478 RVA: 0x0003B55A File Offset: 0x0003975A
	// (set) Token: 0x060009AF RID: 2479 RVA: 0x0003B562 File Offset: 0x00039762
	private bool sendReport
	{
		get
		{
			return this._sendReport;
		}
		set
		{
			if (!this._sendReport)
			{
				this._sendReport = true;
			}
		}
	}

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060009B0 RID: 2480 RVA: 0x0003B573 File Offset: 0x00039773
	// (set) Token: 0x060009B1 RID: 2481 RVA: 0x0003B57B File Offset: 0x0003977B
	private string suspiciousPlayerId
	{
		get
		{
			return this._suspiciousPlayerId;
		}
		set
		{
			if (this._suspiciousPlayerId == "")
			{
				this._suspiciousPlayerId = value;
			}
		}
	}

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060009B2 RID: 2482 RVA: 0x0003B596 File Offset: 0x00039796
	// (set) Token: 0x060009B3 RID: 2483 RVA: 0x0003B59E File Offset: 0x0003979E
	private string suspiciousPlayerName
	{
		get
		{
			return this._suspiciousPlayerName;
		}
		set
		{
			if (this._suspiciousPlayerName == "")
			{
				this._suspiciousPlayerName = value;
			}
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060009B4 RID: 2484 RVA: 0x0003B5B9 File Offset: 0x000397B9
	// (set) Token: 0x060009B5 RID: 2485 RVA: 0x0003B5C1 File Offset: 0x000397C1
	private string suspiciousReason
	{
		get
		{
			return this._suspiciousReason;
		}
		set
		{
			if (this._suspiciousReason == "")
			{
				this._suspiciousReason = value;
			}
		}
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x0003B5DC File Offset: 0x000397DC
	private void Start()
	{
		if (GorillaNot.instance == null)
		{
			GorillaNot.instance = this;
		}
		else if (GorillaNot.instance != this)
		{
			Object.Destroy(this);
		}
		base.StartCoroutine(this.CheckReports());
		this.logErrorCount = 0;
		Application.logMessageReceived += this.LogErrorCount;
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x0003B63C File Offset: 0x0003983C
	public void LogErrorCount(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error)
		{
			this.logErrorCount++;
			this.stringIndex = logString.LastIndexOf("Sender is ");
			if (logString.Contains("RPC") && this.stringIndex >= 0)
			{
				this.playerID = logString.Substring(this.stringIndex + 10);
				this.tempPlayer = null;
				for (int i = 0; i < this.cachedPlayerList.Length; i++)
				{
					if (this.cachedPlayerList[i].UserId == this.playerID)
					{
						this.tempPlayer = this.cachedPlayerList[i];
						break;
					}
				}
				string text = "invalid RPC stuff";
				if (!this.IncrementRPCTracker(this.tempPlayer, text, this.rpcErrorMax))
				{
					this.SendReport("invalid RPC stuff", this.tempPlayer.UserId, this.tempPlayer.NickName);
				}
				this.tempPlayer = null;
			}
			if (this.logErrorCount > this.logErrorMax)
			{
				Debug.unityLogger.logEnabled = false;
			}
		}
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x0003B740 File Offset: 0x00039940
	public void SendReport(string susReason, string susId, string susNick)
	{
		this.suspiciousReason = susReason;
		this.suspiciousPlayerId = susId;
		this.suspiciousPlayerName = susNick;
		this.sendReport = true;
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x0003B75E File Offset: 0x0003995E
	private IEnumerator CheckReports()
	{
		for (;;)
		{
			try
			{
				this.logErrorCount = 0;
				if (PhotonNetwork.InRoom)
				{
					this.lastCheck = Time.time;
					this.lastServerTimestamp = PhotonNetwork.ServerTimestamp;
					if (!PhotonNetwork.CurrentRoom.PublishUserId)
					{
						this.sendReport = true;
						this.suspiciousReason = "missing player ids";
						this.SetToRoomCreatorIfHere();
						this.CloseInvalidRoom();
						Debug.Log("publish user id's is off");
					}
					else if (this.cachedPlayerList.Length > (int)PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentGameType))
					{
						this.sendReport = true;
						this.suspiciousReason = "too many players";
						this.SetToRoomCreatorIfHere();
						this.CloseInvalidRoom();
					}
					if (this.currentMasterClient != PhotonNetwork.MasterClient || this.LowestActorNumber() != PhotonNetwork.MasterClient.ActorNumber)
					{
						foreach (Player player in this.cachedPlayerList)
						{
							if (this.currentMasterClient == player)
							{
								this.sendReport = true;
								this.suspiciousReason = "room host force changed";
								this.suspiciousPlayerId = PhotonNetwork.MasterClient.UserId;
								this.suspiciousPlayerName = PhotonNetwork.MasterClient.NickName;
							}
						}
						this.currentMasterClient = PhotonNetwork.MasterClient;
					}
					if (this.sendReport || this.testAssault)
					{
						if (this.suspiciousPlayerId != "" && this.reportedPlayers.IndexOf(this.suspiciousPlayerId) == -1)
						{
							this.reportedPlayers.Add(this.suspiciousPlayerId);
							this.testAssault = false;
							RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
							WebFlags flags = new WebFlags(1);
							raiseEventOptions.Flags = flags;
							string[] array2 = new string[this.cachedPlayerList.Length];
							int num = 0;
							foreach (Player player2 in this.cachedPlayerList)
							{
								array2[num] = player2.UserId;
								num++;
							}
							object[] eventContent = new object[]
							{
								PhotonNetwork.CurrentRoom.ToStringFull(),
								array2,
								PhotonNetwork.MasterClient.UserId,
								this.suspiciousPlayerId,
								this.suspiciousPlayerName,
								this.suspiciousReason,
								PhotonNetworkController.Instance.GameVersionString
							};
							PhotonNetwork.RaiseEvent(8, eventContent, raiseEventOptions, SendOptions.SendReliable);
							if (this.ShouldDisconnectFromRoom())
							{
								base.StartCoroutine(this.QuitDelay());
							}
						}
						this._sendReport = false;
						this._suspiciousPlayerId = "";
						this._suspiciousPlayerName = "";
						this._suspiciousReason = "";
					}
					foreach (Dictionary<string, GorillaNot.RPCCallTracker> dictionary in this.userRPCCalls.Values)
					{
						foreach (GorillaNot.RPCCallTracker rpccallTracker in dictionary.Values)
						{
							rpccallTracker.RPCCalls = 0;
						}
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1.01f);
		}
		yield break;
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x0003B770 File Offset: 0x00039970
	private int LowestActorNumber()
	{
		this.lowestActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		foreach (Player player in this.cachedPlayerList)
		{
			if (player.ActorNumber < this.lowestActorNumber)
			{
				this.lowestActorNumber = player.ActorNumber;
			}
		}
		return this.lowestActorNumber;
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x0003B7C6 File Offset: 0x000399C6
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		this.cachedPlayerList = PhotonNetwork.PlayerList;
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x0003B7DC File Offset: 0x000399DC
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		this.cachedPlayerList = PhotonNetwork.PlayerList;
		Dictionary<string, GorillaNot.RPCCallTracker> dictionary;
		if (this.userRPCCalls.TryGetValue(otherPlayer.UserId, out dictionary))
		{
			this.userRPCCalls.Remove(otherPlayer.UserId);
		}
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x0003B822 File Offset: 0x00039A22
	public static void IncrementRPCCall(PhotonMessageInfo info, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.instance.IncrementRPCCallLocal(info, callingMethod);
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x0003B834 File Offset: 0x00039A34
	private void IncrementRPCCallLocal(PhotonMessageInfo info, string rpcFunction)
	{
		if (info.SentServerTimestamp < this.lastServerTimestamp)
		{
			return;
		}
		if (!this.IncrementRPCTracker(info.Sender, rpcFunction, this.rpcCallLimit))
		{
			this.SendReport("too many rpc calls! " + rpcFunction, info.Sender.UserId, info.Sender.NickName);
			return;
		}
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x0003B890 File Offset: 0x00039A90
	private bool IncrementRPCTracker(in Player sender, in string rpcFunction, in int callLimit)
	{
		GorillaNot.RPCCallTracker rpccallTracker = this.GetRPCCallTracker(sender, rpcFunction);
		if (rpccallTracker == null)
		{
			return true;
		}
		rpccallTracker.RPCCalls++;
		if (rpccallTracker.RPCCalls > rpccallTracker.RPCCallsMax)
		{
			rpccallTracker.RPCCallsMax = rpccallTracker.RPCCalls;
		}
		return rpccallTracker.RPCCalls <= callLimit;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0003B8E0 File Offset: 0x00039AE0
	private GorillaNot.RPCCallTracker GetRPCCallTracker(in Player sender, in string rpcFunction)
	{
		if (sender == null || sender.UserId == null)
		{
			return null;
		}
		GorillaNot.RPCCallTracker rpccallTracker = null;
		Dictionary<string, GorillaNot.RPCCallTracker> dictionary;
		if (!this.userRPCCalls.TryGetValue(sender.UserId, out dictionary))
		{
			rpccallTracker = new GorillaNot.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			Dictionary<string, GorillaNot.RPCCallTracker> dictionary2 = new Dictionary<string, GorillaNot.RPCCallTracker>();
			dictionary2.Add(rpcFunction, rpccallTracker);
			this.userRPCCalls.Add(sender.UserId, dictionary2);
		}
		else if (!dictionary.TryGetValue(rpcFunction, out rpccallTracker))
		{
			rpccallTracker = new GorillaNot.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			dictionary.Add(rpcFunction, rpccallTracker);
		}
		return rpccallTracker;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0003B976 File Offset: 0x00039B76
	private IEnumerator QuitDelay()
	{
		yield return new WaitForSeconds(1f);
		PhotonNetworkController.Instance.AttemptDisconnect();
		yield break;
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0003B980 File Offset: 0x00039B80
	private void SetToRoomCreatorIfHere()
	{
		this.tempPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1, false);
		if (this.tempPlayer != null)
		{
			this.suspiciousPlayerId = this.tempPlayer.UserId;
			this.suspiciousPlayerName = this.tempPlayer.NickName;
			return;
		}
		this.suspiciousPlayerId = "n/a";
		this.suspiciousPlayerName = "n/a";
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0003B9E0 File Offset: 0x00039BE0
	private bool ShouldDisconnectFromRoom()
	{
		return this._suspiciousReason.Contains("too many players") || this._suspiciousReason.Contains("invalid room name") || this._suspiciousReason.Contains("invalid game mode") || this._suspiciousReason.Contains("missing player ids");
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0003BA35 File Offset: 0x00039C35
	private void CloseInvalidRoom()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentGameType);
	}

	// Token: 0x04000BE2 RID: 3042
	public static volatile GorillaNot instance;

	// Token: 0x04000BE3 RID: 3043
	private bool _sendReport;

	// Token: 0x04000BE4 RID: 3044
	private string _suspiciousPlayerId = "";

	// Token: 0x04000BE5 RID: 3045
	private string _suspiciousPlayerName = "";

	// Token: 0x04000BE6 RID: 3046
	private string _suspiciousReason = "";

	// Token: 0x04000BE7 RID: 3047
	internal List<string> reportedPlayers = new List<string>();

	// Token: 0x04000BE8 RID: 3048
	public byte roomSize;

	// Token: 0x04000BE9 RID: 3049
	public float lastCheck;

	// Token: 0x04000BEA RID: 3050
	public float checkCooldown = 3f;

	// Token: 0x04000BEB RID: 3051
	public float userDecayTime = 15f;

	// Token: 0x04000BEC RID: 3052
	public Player currentMasterClient;

	// Token: 0x04000BED RID: 3053
	public bool testAssault;

	// Token: 0x04000BEE RID: 3054
	private const byte ReportAssault = 8;

	// Token: 0x04000BEF RID: 3055
	private int lowestActorNumber;

	// Token: 0x04000BF0 RID: 3056
	private int calls;

	// Token: 0x04000BF1 RID: 3057
	public int rpcCallLimit = 50;

	// Token: 0x04000BF2 RID: 3058
	public int logErrorMax = 50;

	// Token: 0x04000BF3 RID: 3059
	public int rpcErrorMax = 10;

	// Token: 0x04000BF4 RID: 3060
	private object outObj;

	// Token: 0x04000BF5 RID: 3061
	private Player tempPlayer;

	// Token: 0x04000BF6 RID: 3062
	private int logErrorCount;

	// Token: 0x04000BF7 RID: 3063
	private int stringIndex;

	// Token: 0x04000BF8 RID: 3064
	private string playerID;

	// Token: 0x04000BF9 RID: 3065
	private string playerNick;

	// Token: 0x04000BFA RID: 3066
	private int lastServerTimestamp;

	// Token: 0x04000BFB RID: 3067
	private const string InvalidRPC = "invalid RPC stuff";

	// Token: 0x04000BFC RID: 3068
	public Player[] cachedPlayerList;

	// Token: 0x04000BFD RID: 3069
	private Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>> userRPCCalls = new Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>>();

	// Token: 0x04000BFE RID: 3070
	private Hashtable hashTable;

	// Token: 0x0200042E RID: 1070
	private class RPCCallTracker
	{
		// Token: 0x04001D57 RID: 7511
		public int RPCCalls;

		// Token: 0x04001D58 RID: 7512
		public int RPCCallsMax;
	}
}
