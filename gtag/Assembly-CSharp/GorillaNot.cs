using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class GorillaNot : MonoBehaviourPunCallbacks
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060009B2 RID: 2482 RVA: 0x0003B512 File Offset: 0x00039712
	// (set) Token: 0x060009B3 RID: 2483 RVA: 0x0003B51A File Offset: 0x0003971A
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

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060009B4 RID: 2484 RVA: 0x0003B52B File Offset: 0x0003972B
	// (set) Token: 0x060009B5 RID: 2485 RVA: 0x0003B533 File Offset: 0x00039733
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

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060009B6 RID: 2486 RVA: 0x0003B54E File Offset: 0x0003974E
	// (set) Token: 0x060009B7 RID: 2487 RVA: 0x0003B556 File Offset: 0x00039756
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

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060009B8 RID: 2488 RVA: 0x0003B571 File Offset: 0x00039771
	// (set) Token: 0x060009B9 RID: 2489 RVA: 0x0003B579 File Offset: 0x00039779
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

	// Token: 0x060009BA RID: 2490 RVA: 0x0003B594 File Offset: 0x00039794
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

	// Token: 0x060009BB RID: 2491 RVA: 0x0003B5F4 File Offset: 0x000397F4
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

	// Token: 0x060009BC RID: 2492 RVA: 0x0003B6F8 File Offset: 0x000398F8
	public void SendReport(string susReason, string susId, string susNick)
	{
		this.suspiciousReason = susReason;
		this.suspiciousPlayerId = susId;
		this.suspiciousPlayerName = susNick;
		this.sendReport = true;
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x0003B718 File Offset: 0x00039918
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DispatchReport()
	{
		if (this.sendReport || this.testAssault)
		{
			if (this.suspiciousPlayerId != "" && this.reportedPlayers.IndexOf(this.suspiciousPlayerId) == -1)
			{
				if (this._suspiciousPlayerName.Length > 12)
				{
					this._suspiciousPlayerName = this._suspiciousPlayerName.Remove(12);
				}
				this.reportedPlayers.Add(this.suspiciousPlayerId);
				this.testAssault = false;
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
				WebFlags flags = new WebFlags(1);
				raiseEventOptions.Flags = flags;
				string[] array = new string[this.cachedPlayerList.Length];
				int num = 0;
				foreach (Player player in this.cachedPlayerList)
				{
					array[num] = player.UserId;
					num++;
				}
				object[] eventContent = new object[]
				{
					PhotonNetwork.CurrentRoom.ToStringStripped(),
					array,
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
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x0003B88E File Offset: 0x00039A8E
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
					this.DispatchReport();
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

	// Token: 0x060009BF RID: 2495 RVA: 0x0003B8A0 File Offset: 0x00039AA0
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

	// Token: 0x060009C0 RID: 2496 RVA: 0x0003B8F6 File Offset: 0x00039AF6
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		this.cachedPlayerList = PhotonNetwork.PlayerList;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0003B90C File Offset: 0x00039B0C
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

	// Token: 0x060009C2 RID: 2498 RVA: 0x0003B952 File Offset: 0x00039B52
	public static void IncrementRPCCall(PhotonMessageInfo info, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.instance.IncrementRPCCallLocal(info, callingMethod);
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0003B964 File Offset: 0x00039B64
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

	// Token: 0x060009C4 RID: 2500 RVA: 0x0003B9C0 File Offset: 0x00039BC0
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

	// Token: 0x060009C5 RID: 2501 RVA: 0x0003BA10 File Offset: 0x00039C10
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

	// Token: 0x060009C6 RID: 2502 RVA: 0x0003BAA6 File Offset: 0x00039CA6
	private IEnumerator QuitDelay()
	{
		yield return new WaitForSeconds(1f);
		PhotonNetworkController.Instance.AttemptDisconnect();
		yield break;
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0003BAB0 File Offset: 0x00039CB0
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

	// Token: 0x060009C8 RID: 2504 RVA: 0x0003BB10 File Offset: 0x00039D10
	private bool ShouldDisconnectFromRoom()
	{
		return this._suspiciousReason.Contains("too many players") || this._suspiciousReason.Contains("invalid room name") || this._suspiciousReason.Contains("invalid game mode") || this._suspiciousReason.Contains("missing player ids");
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0003BB65 File Offset: 0x00039D65
	private void CloseInvalidRoom()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentGameType);
	}

	// Token: 0x04000BE6 RID: 3046
	public static volatile GorillaNot instance;

	// Token: 0x04000BE7 RID: 3047
	private bool _sendReport;

	// Token: 0x04000BE8 RID: 3048
	private string _suspiciousPlayerId = "";

	// Token: 0x04000BE9 RID: 3049
	private string _suspiciousPlayerName = "";

	// Token: 0x04000BEA RID: 3050
	private string _suspiciousReason = "";

	// Token: 0x04000BEB RID: 3051
	internal List<string> reportedPlayers = new List<string>();

	// Token: 0x04000BEC RID: 3052
	public byte roomSize;

	// Token: 0x04000BED RID: 3053
	public float lastCheck;

	// Token: 0x04000BEE RID: 3054
	public float checkCooldown = 3f;

	// Token: 0x04000BEF RID: 3055
	public float userDecayTime = 15f;

	// Token: 0x04000BF0 RID: 3056
	public Player currentMasterClient;

	// Token: 0x04000BF1 RID: 3057
	public bool testAssault;

	// Token: 0x04000BF2 RID: 3058
	private const byte ReportAssault = 8;

	// Token: 0x04000BF3 RID: 3059
	private int lowestActorNumber;

	// Token: 0x04000BF4 RID: 3060
	private int calls;

	// Token: 0x04000BF5 RID: 3061
	public int rpcCallLimit = 50;

	// Token: 0x04000BF6 RID: 3062
	public int logErrorMax = 50;

	// Token: 0x04000BF7 RID: 3063
	public int rpcErrorMax = 10;

	// Token: 0x04000BF8 RID: 3064
	private object outObj;

	// Token: 0x04000BF9 RID: 3065
	private Player tempPlayer;

	// Token: 0x04000BFA RID: 3066
	private int logErrorCount;

	// Token: 0x04000BFB RID: 3067
	private int stringIndex;

	// Token: 0x04000BFC RID: 3068
	private string playerID;

	// Token: 0x04000BFD RID: 3069
	private string playerNick;

	// Token: 0x04000BFE RID: 3070
	private int lastServerTimestamp;

	// Token: 0x04000BFF RID: 3071
	private const string InvalidRPC = "invalid RPC stuff";

	// Token: 0x04000C00 RID: 3072
	public Player[] cachedPlayerList;

	// Token: 0x04000C01 RID: 3073
	private Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>> userRPCCalls = new Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>>();

	// Token: 0x04000C02 RID: 3074
	private Hashtable hashTable;

	// Token: 0x02000430 RID: 1072
	private class RPCCallTracker
	{
		// Token: 0x04001D64 RID: 7524
		public int RPCCalls;

		// Token: 0x04001D65 RID: 7525
		public int RPCCallsMax;
	}
}
