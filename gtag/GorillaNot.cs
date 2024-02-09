using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaNot : MonoBehaviourPunCallbacks
{
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

	public void SendReport(string susReason, string susId, string susNick)
	{
		this.suspiciousReason = susReason;
		this.suspiciousPlayerId = susId;
		this.suspiciousPlayerName = susNick;
		this.sendReport = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DispatchReport()
	{
		if ((this.sendReport || this.testAssault) && this.suspiciousPlayerId != "" && this.reportedPlayers.IndexOf(this.suspiciousPlayerId) == -1)
		{
			if (this._suspiciousPlayerName.Length > 12)
			{
				this._suspiciousPlayerName = this._suspiciousPlayerName.Remove(12);
			}
			this.reportedPlayers.Add(this.suspiciousPlayerId);
			this.testAssault = false;
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			WebFlags webFlags = new WebFlags(1);
			raiseEventOptions.Flags = webFlags;
			raiseEventOptions.TargetActors = GorillaNot.targetActors;
			raiseEventOptions.Receivers = ReceiverGroup.MasterClient;
			string[] array = new string[this.cachedPlayerList.Length];
			int num = 0;
			foreach (Player player in this.cachedPlayerList)
			{
				array[num] = player.UserId;
				num++;
			}
			object[] array3 = new object[]
			{
				PhotonNetwork.CurrentRoom.ToStringStripped(),
				array,
				PhotonNetwork.MasterClient.UserId,
				this.suspiciousPlayerId,
				this.suspiciousPlayerName,
				this.suspiciousReason,
				PhotonNetworkController.Instance.GameVersionString
			};
			PhotonNetwork.RaiseEvent(8, array3, raiseEventOptions, SendOptions.SendReliable);
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

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		this.cachedPlayerList = PhotonNetwork.PlayerList;
	}

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

	public static void IncrementRPCCall(PhotonMessageInfo info, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.instance.IncrementRPCCallLocal(info, callingMethod);
	}

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

	private IEnumerator QuitDelay()
	{
		yield return new WaitForSeconds(1f);
		PhotonNetworkController.Instance.AttemptDisconnect();
		yield break;
	}

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

	private bool ShouldDisconnectFromRoom()
	{
		return this._suspiciousReason.Contains("too many players") || this._suspiciousReason.Contains("invalid room name") || this._suspiciousReason.Contains("invalid game mode") || this._suspiciousReason.Contains("missing player ids");
	}

	private void CloseInvalidRoom()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentGameType);
	}

	[OnEnterPlay_SetNull]
	public static volatile GorillaNot instance;

	private bool _sendReport;

	private string _suspiciousPlayerId = "";

	private string _suspiciousPlayerName = "";

	private string _suspiciousReason = "";

	internal List<string> reportedPlayers = new List<string>();

	public byte roomSize;

	public float lastCheck;

	public float checkCooldown = 3f;

	public float userDecayTime = 15f;

	public Player currentMasterClient;

	public bool testAssault;

	private const byte ReportAssault = 8;

	private int lowestActorNumber;

	private int calls;

	public int rpcCallLimit = 50;

	public int logErrorMax = 50;

	public int rpcErrorMax = 10;

	private object outObj;

	private Player tempPlayer;

	private int logErrorCount;

	private int stringIndex;

	private string playerID;

	private string playerNick;

	private int lastServerTimestamp;

	private const string InvalidRPC = "invalid RPC stuff";

	public Player[] cachedPlayerList;

	private static int[] targetActors = new int[] { -1 };

	private Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>> userRPCCalls = new Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>>();

	private Hashtable hashTable;

	private class RPCCallTracker
	{
		public int RPCCalls;

		public int RPCCallsMax;
	}
}
