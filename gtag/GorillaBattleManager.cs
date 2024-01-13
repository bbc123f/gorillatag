using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaBattleManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks, IPunObservable
{
	public enum BattleStatus
	{
		RedTeam = 1,
		BlueTeam = 2,
		Normal = 4,
		Hit = 8,
		Stunned = 16,
		Grace = 32,
		Eliminated = 64,
		None = 0
	}

	private enum BattleState
	{
		NotEnoughPlayers,
		GameEnd,
		GameEndWaiting,
		StartCountdown,
		CountingDownToStart,
		GameStart,
		GameRunning
	}

	private float playerMin = 2f;

	public float tagCoolDown = 5f;

	public Dictionary<int, int> playerLives = new Dictionary<int, int>();

	public Dictionary<int, BattleStatus> playerStatusDict = new Dictionary<int, BattleStatus>();

	public Dictionary<int, float> playerHitTimes = new Dictionary<int, float>();

	public Dictionary<int, float> playerStunTimes = new Dictionary<int, float>();

	public int[] playerActorNumberArray = new int[10];

	public int[] playerLivesArray = new int[10];

	public BattleStatus[] playerStatusArray = new BattleStatus[10];

	public bool teamBattle = true;

	public int countDownTime;

	private float timeBattleEnded;

	public float hitCooldown = 3f;

	public float stunGracePeriod = 2f;

	public object objRef;

	private bool playerInList;

	private bool coroutineRunning;

	private int lives;

	private int outLives;

	private int bcount;

	private int rcount;

	private int randInt;

	private float outHitTime;

	private PhotonView tempView;

	private KeyValuePair<int, int>[] keyValuePairs;

	private KeyValuePair<int, BattleStatus>[] keyValuePairsStatus;

	private BattleStatus tempStatus;

	private BattleState currentState;

	private void ActivateBattleBalloons(bool enable)
	{
		if (GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.battleBalloons.gameObject.SetActive(enable);
		}
	}

	private bool HasFlag(BattleStatus state, BattleStatus statusFlag)
	{
		return (state & statusFlag) != 0;
	}

	public override string GameMode()
	{
		return "BATTLE";
	}

	private void ActivateDefaultSlingShot()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig != null && !Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController cosmeticsController = CosmeticsController.instance;
			cosmeticsController.ApplyCosmeticItemToSet(newItem: cosmeticsController.GetItemFromDict("Slingshot"), set: offlineVRRig.cosmeticSet, isLeftHand: true, applyToPlayerPrefs: false);
		}
	}

	public override void Awake()
	{
		base.Awake();
		coroutineRunning = false;
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
		if (base.photonView.IsMine)
		{
			currentState = BattleState.NotEnoughPlayers;
		}
		ActivateDefaultSlingShot();
		ActivateBattleBalloons(enable: true);
	}

	private void Transition(BattleState newState)
	{
		currentState = newState;
		Debug.Log("current state is: " + currentState);
	}

	public void UpdateBattleState()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		switch (currentState)
		{
		case BattleState.NotEnoughPlayers:
			if ((float)currentPlayerArray.Length >= playerMin)
			{
				Transition(BattleState.StartCountdown);
			}
			break;
		case BattleState.GameRunning:
			if (CheckForGameEnd())
			{
				Transition(BattleState.GameEnd);
			}
			if ((float)currentPlayerArray.Length < playerMin)
			{
				InitializePlayerStatus();
				ActivateBattleBalloons(enable: false);
				Transition(BattleState.NotEnoughPlayers);
			}
			break;
		case BattleState.GameEnd:
			if (EndBattleGame())
			{
				Transition(BattleState.GameEndWaiting);
			}
			break;
		case BattleState.GameEndWaiting:
			if (BattleEnd())
			{
				Transition(BattleState.StartCountdown);
			}
			break;
		case BattleState.StartCountdown:
			RandomizeTeams();
			ActivateBattleBalloons(enable: true);
			StartCoroutine(StartBattleCountdown());
			Transition(BattleState.CountingDownToStart);
			break;
		case BattleState.CountingDownToStart:
			if (!coroutineRunning)
			{
				Transition(BattleState.StartCountdown);
			}
			break;
		case BattleState.GameStart:
			StartBattle();
			Transition(BattleState.GameRunning);
			break;
		}
		UpdatePlayerStatus();
	}

	private bool CheckForGameEnd()
	{
		bcount = 0;
		rcount = 0;
		Player[] array = currentPlayerArray;
		foreach (Player player in array)
		{
			if (playerLives.TryGetValue(player.ActorNumber, out lives))
			{
				if (lives > 0 && playerStatusDict.TryGetValue(player.ActorNumber, out tempStatus))
				{
					if (HasFlag(tempStatus, BattleStatus.RedTeam))
					{
						rcount++;
					}
					else if (HasFlag(tempStatus, BattleStatus.BlueTeam))
					{
						bcount++;
					}
				}
			}
			else
			{
				playerLives.Add(player.ActorNumber, 0);
			}
		}
		if (bcount == 0 || rcount == 0)
		{
			return true;
		}
		return false;
	}

	public IEnumerator StartBattleCountdown()
	{
		coroutineRunning = true;
		for (countDownTime = 5; countDownTime > 0; countDownTime--)
		{
			try
			{
				Player[] playerList = PhotonNetwork.PlayerList;
				foreach (Player player in playerList)
				{
					playerLives[player.ActorNumber] = 3;
					PhotonView photonView = FindVRRigForPlayer(player);
					if (photonView != null)
					{
						photonView.RPC("PlayTagSound", player, 6, 0.25f);
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		coroutineRunning = false;
		currentState = BattleState.GameStart;
		yield return null;
	}

	public void StartBattle()
	{
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player player in playerList)
		{
			playerLives[player.ActorNumber] = 3;
			PhotonView photonView = FindVRRigForPlayer(player);
			if (photonView != null)
			{
				photonView.RPC("PlayTagSound", player, 7, 0.5f);
			}
		}
	}

	private bool EndBattleGame()
	{
		if ((float)PhotonNetwork.PlayerList.Length >= playerMin)
		{
			Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Player player in playerList)
			{
				PhotonView photonView = FindVRRigForPlayer(player);
				if (photonView != null)
				{
					photonView.RPC("SetTaggedTime", player, null);
					photonView.RPC("PlayTagSound", player, 2, 0.25f);
				}
			}
			timeBattleEnded = Time.time;
			return true;
		}
		return false;
	}

	public bool BattleEnd()
	{
		return Time.time > timeBattleEnded + tagCoolDown;
	}

	public bool SlingshotHit(Player myPlayer, Player otherPlayer)
	{
		if (playerLives.TryGetValue(otherPlayer.ActorNumber, out lives))
		{
			return lives > 0;
		}
		return false;
	}

	[PunRPC]
	public void ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportSlingshotHit");
		if (!base.photonView.IsMine || currentState != BattleState.GameRunning || OnSameTeam(taggedPlayer, info.Sender))
		{
			return;
		}
		if (GetPlayerLives(taggedPlayer) > 0 && GetPlayerLives(info.Sender) > 0 && !PlayerInHitCooldown(taggedPlayer))
		{
			if (!playerHitTimes.TryGetValue(taggedPlayer.ActorNumber, out outHitTime))
			{
				playerHitTimes.Add(taggedPlayer.ActorNumber, Time.time);
			}
			else
			{
				playerHitTimes[taggedPlayer.ActorNumber] = Time.time;
			}
			playerLives[taggedPlayer.ActorNumber]--;
			tempView = FindVRRigForPlayer(taggedPlayer);
			if (tempView != null)
			{
				tempView.RPC("PlayTagSound", RpcTarget.All, 0, 0.25f);
			}
		}
		else
		{
			if (GetPlayerLives(info.Sender) != 0 || GetPlayerLives(taggedPlayer) <= 0)
			{
				return;
			}
			tempStatus = GetPlayerStatus(taggedPlayer);
			if (HasFlag(tempStatus, BattleStatus.Normal) && !PlayerInHitCooldown(taggedPlayer) && !PlayerInStunCooldown(taggedPlayer))
			{
				if (!playerStunTimes.TryGetValue(taggedPlayer.ActorNumber, out outHitTime))
				{
					playerStunTimes.Add(taggedPlayer.ActorNumber, Time.time);
				}
				else
				{
					playerStunTimes[taggedPlayer.ActorNumber] = Time.time;
				}
				tempView = FindVRRigForPlayer(taggedPlayer);
				if (tempView != null)
				{
					tempView.RPC("SetSlowedTime", taggedPlayer, null);
					tempView.RPC("PlayTagSound", RpcTarget.All, 5, 0.125f);
				}
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (base.photonView.IsMine)
		{
			if (currentState == BattleState.GameRunning)
			{
				playerLives.Add(newPlayer.ActorNumber, 0);
			}
			else
			{
				playerLives.Add(newPlayer.ActorNumber, 3);
			}
			playerStatusDict.Add(newPlayer.ActorNumber, BattleStatus.None);
			CopyBattleDictToArray();
			AddPlayerToCorrectTeam(newPlayer);
		}
		playerProjectiles.Add(newPlayer, new List<ProjectileInfo>());
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (playerLives.ContainsKey(otherPlayer.ActorNumber))
		{
			playerLives.Remove(otherPlayer.ActorNumber);
		}
		if (playerStatusDict.ContainsKey(otherPlayer.ActorNumber))
		{
			playerStatusDict.Remove(otherPlayer.ActorNumber);
		}
		if (playerProjectiles.ContainsKey(otherPlayer))
		{
			playerProjectiles.Remove(otherPlayer);
		}
		playerVRRigDict.Remove(otherPlayer.ActorNumber);
	}

	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			CopyBattleDictToArray();
			for (int i = 0; i < playerLivesArray.Length; i++)
			{
				stream.SendNext(playerActorNumberArray[i]);
				stream.SendNext(playerLivesArray[i]);
				stream.SendNext(playerStatusArray[i]);
			}
			stream.SendNext((int)currentState);
		}
		else
		{
			for (int j = 0; j < playerLivesArray.Length; j++)
			{
				playerActorNumberArray[j] = (int)stream.ReceiveNext();
				playerLivesArray[j] = (int)stream.ReceiveNext();
				playerStatusArray[j] = (BattleStatus)stream.ReceiveNext();
			}
			currentState = (BattleState)stream.ReceiveNext();
			CopyArrayToBattleDict();
		}
	}

	public override int MyMatIndex(Player forPlayer)
	{
		tempStatus = GetPlayerStatus(forPlayer);
		if (tempStatus != 0)
		{
			if (HasFlag(tempStatus, BattleStatus.RedTeam))
			{
				if (HasFlag(tempStatus, BattleStatus.Normal))
				{
					return 8;
				}
				if (HasFlag(tempStatus, BattleStatus.Hit))
				{
					return 9;
				}
				if (HasFlag(tempStatus, BattleStatus.Stunned))
				{
					return 10;
				}
				if (HasFlag(tempStatus, BattleStatus.Grace))
				{
					return 10;
				}
				if (HasFlag(tempStatus, BattleStatus.Eliminated))
				{
					return 11;
				}
			}
			else
			{
				if (HasFlag(tempStatus, BattleStatus.Normal))
				{
					return 4;
				}
				if (HasFlag(tempStatus, BattleStatus.Hit))
				{
					return 5;
				}
				if (HasFlag(tempStatus, BattleStatus.Stunned))
				{
					return 6;
				}
				if (HasFlag(tempStatus, BattleStatus.Grace))
				{
					return 6;
				}
				if (HasFlag(tempStatus, BattleStatus.Eliminated))
				{
					return 7;
				}
			}
		}
		return 0;
	}

	public override float[] LocalPlayerSpeed()
	{
		if (playerStatusDict.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber, out tempStatus))
		{
			if (HasFlag(tempStatus, BattleStatus.Normal))
			{
				playerSpeed[0] = 6.5f;
				playerSpeed[1] = 1.1f;
				return playerSpeed;
			}
			if (HasFlag(tempStatus, BattleStatus.Stunned))
			{
				playerSpeed[0] = 2f;
				playerSpeed[1] = 0.5f;
				return playerSpeed;
			}
			if (HasFlag(tempStatus, BattleStatus.Eliminated))
			{
				playerSpeed[0] = fastJumpLimit;
				playerSpeed[1] = fastJumpMultiplier;
				return playerSpeed;
			}
		}
		playerSpeed[0] = 6.5f;
		playerSpeed[1] = 1.1f;
		return playerSpeed;
	}

	public override void Update()
	{
		base.Update();
		if (base.photonView.IsMine)
		{
			UpdateBattleState();
		}
		ActivateDefaultSlingShot();
	}

	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		foreach (int key in playerLives.Keys)
		{
			playerInList = false;
			Player[] array = currentPlayerArray;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ActorNumber == key)
				{
					playerInList = true;
				}
			}
			if (!playerInList)
			{
				playerLives.Remove(key);
			}
		}
	}

	public int GetPlayerLives(Player player)
	{
		if (player == null)
		{
			return 0;
		}
		if (playerLives.TryGetValue(player.ActorNumber, out outLives))
		{
			return outLives;
		}
		return 0;
	}

	public bool PlayerInHitCooldown(Player player)
	{
		if (playerHitTimes.TryGetValue(player.ActorNumber, out var value))
		{
			return value + hitCooldown > Time.time;
		}
		return false;
	}

	public bool PlayerInStunCooldown(Player player)
	{
		if (playerStunTimes.TryGetValue(player.ActorNumber, out var value))
		{
			return value + hitCooldown + stunGracePeriod > Time.time;
		}
		return false;
	}

	public BattleStatus GetPlayerStatus(Player player)
	{
		if (playerStatusDict.TryGetValue(player.ActorNumber, out tempStatus))
		{
			return tempStatus;
		}
		return BattleStatus.None;
	}

	public bool OnRedTeam(BattleStatus status)
	{
		return HasFlag(status, BattleStatus.RedTeam);
	}

	public bool OnRedTeam(Player player)
	{
		BattleStatus playerStatus = GetPlayerStatus(player);
		return OnRedTeam(playerStatus);
	}

	public bool OnBlueTeam(BattleStatus status)
	{
		return HasFlag(status, BattleStatus.BlueTeam);
	}

	public bool OnBlueTeam(Player player)
	{
		BattleStatus playerStatus = GetPlayerStatus(player);
		return OnBlueTeam(playerStatus);
	}

	public bool OnNoTeam(BattleStatus status)
	{
		if (!OnRedTeam(status))
		{
			return !OnBlueTeam(status);
		}
		return false;
	}

	public bool OnNoTeam(Player player)
	{
		BattleStatus playerStatus = GetPlayerStatus(player);
		return OnNoTeam(playerStatus);
	}

	public override bool LocalCanTag(Player myPlayer, Player otherPlayer)
	{
		return false;
	}

	public bool OnSameTeam(BattleStatus playerA, BattleStatus playerB)
	{
		bool num = OnRedTeam(playerA) && OnRedTeam(playerB);
		bool flag = OnBlueTeam(playerA) && OnBlueTeam(playerB);
		return num || flag;
	}

	public bool OnSameTeam(Player myPlayer, Player otherPlayer)
	{
		BattleStatus playerStatus = GetPlayerStatus(myPlayer);
		BattleStatus playerStatus2 = GetPlayerStatus(otherPlayer);
		return OnSameTeam(playerStatus, playerStatus2);
	}

	public bool LocalCanHit(Player myPlayer, Player otherPlayer)
	{
		bool num = !OnSameTeam(myPlayer, otherPlayer);
		bool flag = GetPlayerLives(otherPlayer) != 0;
		return num && flag;
	}

	private void CopyBattleDictToArray()
	{
		for (int i = 0; i < playerLivesArray.Length; i++)
		{
			playerLivesArray[i] = 0;
			playerActorNumberArray[i] = 0;
		}
		keyValuePairs = playerLives.ToArray();
		for (int j = 0; j < playerLivesArray.Length && j < keyValuePairs.Length; j++)
		{
			playerActorNumberArray[j] = keyValuePairs[j].Key;
			playerLivesArray[j] = keyValuePairs[j].Value;
			playerStatusArray[j] = GetPlayerStatus(PhotonNetwork.LocalPlayer.Get(keyValuePairs[j].Key));
		}
	}

	private void CopyArrayToBattleDict()
	{
		for (int i = 0; i < playerLivesArray.Length; i++)
		{
			if (playerActorNumberArray[i] != 0)
			{
				if (playerLives.TryGetValue(playerActorNumberArray[i], out outLives))
				{
					playerLives[playerActorNumberArray[i]] = playerLivesArray[i];
				}
				else
				{
					playerLives.Add(playerActorNumberArray[i], playerLivesArray[i]);
				}
				if (playerStatusDict.ContainsKey(playerActorNumberArray[i]))
				{
					playerStatusDict[playerActorNumberArray[i]] = playerStatusArray[i];
				}
				else
				{
					playerStatusDict.Add(playerActorNumberArray[i], playerStatusArray[i]);
				}
			}
		}
	}

	private BattleStatus SetFlag(BattleStatus currState, BattleStatus flag)
	{
		return currState | flag;
	}

	private BattleStatus SetFlagExclusive(BattleStatus currState, BattleStatus flag)
	{
		return flag;
	}

	private BattleStatus ClearFlag(BattleStatus currState, BattleStatus flag)
	{
		return currState & ~flag;
	}

	private bool FlagIsSet(BattleStatus currState, BattleStatus flag)
	{
		return (currState & flag) != 0;
	}

	public void RandomizeTeams()
	{
		int[] array = new int[currentPlayerArray.Length];
		for (int i = 0; i < currentPlayerArray.Length; i++)
		{
			array[i] = i;
		}
		System.Random rand = new System.Random();
		int[] array2 = array.OrderBy((int x) => rand.Next()).ToArray();
		BattleStatus battleStatus = ((rand.Next(0, 2) == 0) ? BattleStatus.RedTeam : BattleStatus.BlueTeam);
		BattleStatus battleStatus2 = ((battleStatus != BattleStatus.RedTeam) ? BattleStatus.RedTeam : BattleStatus.BlueTeam);
		for (int j = 0; j < currentPlayerArray.Length; j++)
		{
			BattleStatus value = ((array2[j] % 2 == 0) ? battleStatus2 : battleStatus);
			playerStatusDict[currentPlayerArray[j].ActorNumber] = value;
		}
	}

	public void AddPlayerToCorrectTeam(Player newPlayer)
	{
		rcount = 0;
		for (int i = 0; i < currentPlayerArray.Length; i++)
		{
			if (playerStatusDict.ContainsKey(currentPlayerArray[i].ActorNumber))
			{
				BattleStatus state = playerStatusDict[currentPlayerArray[i].ActorNumber];
				rcount = (HasFlag(state, BattleStatus.RedTeam) ? (rcount + 1) : rcount);
			}
		}
		if ((currentPlayerArray.Length - 1) / 2 == rcount)
		{
			playerStatusDict[newPlayer.ActorNumber] = ((UnityEngine.Random.Range(0, 2) == 0) ? SetFlag(playerStatusDict[newPlayer.ActorNumber], BattleStatus.RedTeam) : SetFlag(playerStatusDict[newPlayer.ActorNumber], BattleStatus.BlueTeam));
		}
		else if (rcount <= (currentPlayerArray.Length - 1) / 2)
		{
			playerStatusDict[newPlayer.ActorNumber] = SetFlag(playerStatusDict[newPlayer.ActorNumber], BattleStatus.RedTeam);
		}
	}

	private void InitializePlayerStatus()
	{
		keyValuePairsStatus = playerStatusDict.ToArray();
		KeyValuePair<int, BattleStatus>[] array = keyValuePairsStatus;
		foreach (KeyValuePair<int, BattleStatus> keyValuePair in array)
		{
			playerStatusDict[keyValuePair.Key] = BattleStatus.Normal;
		}
	}

	private void UpdatePlayerStatus()
	{
		keyValuePairsStatus = playerStatusDict.ToArray();
		KeyValuePair<int, BattleStatus>[] array = keyValuePairsStatus;
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<int, BattleStatus> keyValuePair = array[i];
			BattleStatus battleStatus = (HasFlag(playerStatusDict[keyValuePair.Key], BattleStatus.RedTeam) ? BattleStatus.RedTeam : BattleStatus.BlueTeam);
			if (playerLives.TryGetValue(keyValuePair.Key, out outLives) && outLives == 0)
			{
				playerStatusDict[keyValuePair.Key] = battleStatus | BattleStatus.Eliminated;
			}
			else if (playerHitTimes.TryGetValue(keyValuePair.Key, out outHitTime) && outHitTime + hitCooldown > Time.time)
			{
				playerStatusDict[keyValuePair.Key] = battleStatus | BattleStatus.Hit;
			}
			else if (playerStunTimes.TryGetValue(keyValuePair.Key, out outHitTime))
			{
				if (outHitTime + hitCooldown > Time.time)
				{
					playerStatusDict[keyValuePair.Key] = battleStatus | BattleStatus.Stunned;
				}
				else if (outHitTime + hitCooldown + stunGracePeriod > Time.time)
				{
					playerStatusDict[keyValuePair.Key] = battleStatus | BattleStatus.Grace;
				}
				else
				{
					playerStatusDict[keyValuePair.Key] = battleStatus | BattleStatus.Normal;
				}
			}
			else
			{
				playerStatusDict[keyValuePair.Key] = battleStatus | BattleStatus.Normal;
			}
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController cosmeticsController = CosmeticsController.instance;
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if (offlineVRRig.cosmeticSet.HasItem("Slingshot"))
			{
				cosmeticsController.RemoveCosmeticItemFromSet(offlineVRRig.cosmeticSet, "Slingshot", applyToPlayerPrefs: true);
			}
		}
		ActivateBattleBalloons(enable: false);
	}
}
