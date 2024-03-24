using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaBattleManager : GorillaGameManager
{
	private void ActivateBattleBalloons(bool enable)
	{
		if (GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.battleBalloons.gameObject.SetActive(enable);
		}
	}

	private bool HasFlag(GorillaBattleManager.BattleStatus state, GorillaBattleManager.BattleStatus statusFlag)
	{
		return (state & statusFlag) > GorillaBattleManager.BattleStatus.None;
	}

	public override GameModeType GameType()
	{
		return GameModeType.Battle;
	}

	public override string GameModeName()
	{
		return "BATTLE";
	}

	private void ActivateDefaultSlingShot()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig != null && !Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			instance.ApplyCosmeticItemToSet(offlineVRRig.cosmeticSet, itemFromDict, true, false);
		}
	}

	public override void Awake()
	{
		base.Awake();
		this.coroutineRunning = false;
		this.currentState = GorillaBattleManager.BattleState.NotEnoughPlayers;
	}

	public override void StartPlaying()
	{
		base.StartPlaying();
		this.ActivateBattleBalloons(true);
		this.VerifyPlayersInDict<int>(this.playerLives);
		this.VerifyPlayersInDict<GorillaBattleManager.BattleStatus>(this.playerStatusDict);
		this.VerifyPlayersInDict<float>(this.playerHitTimes);
		this.VerifyPlayersInDict<float>(this.playerStunTimes);
		this.CopyBattleDictToArray();
		this.UpdateBattleState();
	}

	public override void StopPlaying()
	{
		base.StopPlaying();
		if (Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if (offlineVRRig.cosmeticSet.HasItem("Slingshot"))
			{
				instance.RemoveCosmeticItemFromSet(offlineVRRig.cosmeticSet, "Slingshot", true);
			}
		}
		this.ActivateBattleBalloons(false);
		base.StopAllCoroutines();
		this.coroutineRunning = false;
	}

	public override void Reset()
	{
		base.Reset();
		this.playerLives.Clear();
		this.playerStatusDict.Clear();
		this.playerHitTimes.Clear();
		this.playerStunTimes.Clear();
		for (int i = 0; i < this.playerActorNumberArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = 0;
			this.playerStatusArray[i] = GorillaBattleManager.BattleStatus.None;
		}
		this.currentState = GorillaBattleManager.BattleState.NotEnoughPlayers;
	}

	private void VerifyPlayersInDict<T>(Dictionary<int, T> dict)
	{
		if (dict.Count < 1)
		{
			return;
		}
		int[] array = dict.Keys.ToArray<int>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!Utils.PlayerInRoom(array[i]))
			{
				dict.Remove(array[i]);
			}
		}
	}

	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<BattleRPCs>();
	}

	private void Transition(GorillaBattleManager.BattleState newState)
	{
		this.currentState = newState;
		Debug.Log("current state is: " + this.currentState.ToString());
	}

	public void UpdateBattleState()
	{
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			switch (this.currentState)
			{
			case GorillaBattleManager.BattleState.NotEnoughPlayers:
				if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
				{
					this.Transition(GorillaBattleManager.BattleState.StartCountdown);
				}
				break;
			case GorillaBattleManager.BattleState.GameEnd:
				if (this.EndBattleGame())
				{
					this.Transition(GorillaBattleManager.BattleState.GameEndWaiting);
				}
				break;
			case GorillaBattleManager.BattleState.GameEndWaiting:
				if (this.BattleEnd())
				{
					this.Transition(GorillaBattleManager.BattleState.StartCountdown);
				}
				break;
			case GorillaBattleManager.BattleState.StartCountdown:
				this.RandomizeTeams();
				this.ActivateBattleBalloons(true);
				base.StartCoroutine(this.StartBattleCountdown());
				this.Transition(GorillaBattleManager.BattleState.CountingDownToStart);
				break;
			case GorillaBattleManager.BattleState.CountingDownToStart:
				if (!this.coroutineRunning)
				{
					this.Transition(GorillaBattleManager.BattleState.StartCountdown);
				}
				break;
			case GorillaBattleManager.BattleState.GameStart:
				this.StartBattle();
				this.Transition(GorillaBattleManager.BattleState.GameRunning);
				break;
			case GorillaBattleManager.BattleState.GameRunning:
				if (this.CheckForGameEnd())
				{
					this.Transition(GorillaBattleManager.BattleState.GameEnd);
				}
				if ((float)RoomSystem.PlayersInRoom.Count < this.playerMin)
				{
					this.InitializePlayerStatus();
					this.ActivateBattleBalloons(false);
					this.Transition(GorillaBattleManager.BattleState.NotEnoughPlayers);
				}
				break;
			}
			this.UpdatePlayerStatus();
		}
	}

	private bool CheckForGameEnd()
	{
		this.bcount = 0;
		this.rcount = 0;
		foreach (Player player in RoomSystem.PlayersInRoom)
		{
			if (this.playerLives.TryGetValue(player.ActorNumber, out this.lives))
			{
				if (this.lives > 0 && this.playerStatusDict.TryGetValue(player.ActorNumber, out this.tempStatus))
				{
					if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.RedTeam))
					{
						this.rcount++;
					}
					else if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.BlueTeam))
					{
						this.bcount++;
					}
				}
			}
			else
			{
				this.playerLives.Add(player.ActorNumber, 0);
			}
		}
		return this.bcount == 0 || this.rcount == 0;
	}

	public IEnumerator StartBattleCountdown()
	{
		this.coroutineRunning = true;
		this.countDownTime = 5;
		while (this.countDownTime > 0)
		{
			try
			{
				RoomSystem.SendSoundEffectAll(6, 0.25f);
				foreach (Player player in RoomSystem.PlayersInRoom)
				{
					this.playerLives[player.ActorNumber] = 3;
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
			this.countDownTime--;
		}
		this.coroutineRunning = false;
		this.currentState = GorillaBattleManager.BattleState.GameStart;
		yield return null;
		yield break;
	}

	public void StartBattle()
	{
		RoomSystem.SendSoundEffectAll(7, 0.5f);
		foreach (Player player in RoomSystem.PlayersInRoom)
		{
			this.playerLives[player.ActorNumber] = 3;
		}
	}

	private bool EndBattleGame()
	{
		if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
		{
			RoomSystem.SendStatusEffectAll(RoomSystem.StatusEffects.TaggedTime);
			RoomSystem.SendSoundEffectAll(2, 0.25f);
			this.timeBattleEnded = Time.time;
			return true;
		}
		return false;
	}

	public bool BattleEnd()
	{
		return Time.time > this.timeBattleEnded + this.tagCoolDown;
	}

	public bool SlingshotHit(Player myPlayer, Player otherPlayer)
	{
		return this.playerLives.TryGetValue(otherPlayer.ActorNumber, out this.lives) && this.lives > 0;
	}

	public void ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (this.currentState != GorillaBattleManager.BattleState.GameRunning)
		{
			return;
		}
		if (this.OnSameTeam(taggedPlayer, info.Sender))
		{
			return;
		}
		if (this.GetPlayerLives(taggedPlayer) > 0 && this.GetPlayerLives(info.Sender) > 0 && !this.PlayerInHitCooldown(taggedPlayer))
		{
			if (!this.playerHitTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
			{
				this.playerHitTimes.Add(taggedPlayer.ActorNumber, Time.time);
			}
			else
			{
				this.playerHitTimes[taggedPlayer.ActorNumber] = Time.time;
			}
			Dictionary<int, int> dictionary = this.playerLives;
			int actorNumber = taggedPlayer.ActorNumber;
			int num = dictionary[actorNumber];
			dictionary[actorNumber] = num - 1;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer);
			return;
		}
		if (this.GetPlayerLives(info.Sender) == 0 && this.GetPlayerLives(taggedPlayer) > 0)
		{
			this.tempStatus = this.GetPlayerStatus(taggedPlayer);
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Normal) && !this.PlayerInHitCooldown(taggedPlayer) && !this.PlayerInStunCooldown(taggedPlayer))
			{
				if (!this.playerStunTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
				{
					this.playerStunTimes.Add(taggedPlayer.ActorNumber, Time.time);
				}
				else
				{
					this.playerStunTimes[taggedPlayer.ActorNumber] = Time.time;
				}
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(5, 0.125f, taggedPlayer);
				this.tempView = this.FindVRRigForPlayer(taggedPlayer);
			}
		}
	}

	public override void HitPlayer(Player player)
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient || this.currentState != GorillaBattleManager.BattleState.GameRunning)
		{
			return;
		}
		if (this.GetPlayerLives(player) > 0)
		{
			this.playerLives[player.ActorNumber] = 0;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, player);
		}
	}

	public override bool CanAffectPlayer(Player player, bool thisFrame)
	{
		return this.playerLives.TryGetValue(player.ActorNumber, out this.lives) && this.lives > 0;
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			if (this.currentState == GorillaBattleManager.BattleState.GameRunning)
			{
				this.playerLives.Add(newPlayer.ActorNumber, 0);
			}
			else
			{
				this.playerLives.Add(newPlayer.ActorNumber, 3);
			}
			this.playerStatusDict.Add(newPlayer.ActorNumber, GorillaBattleManager.BattleStatus.None);
			this.CopyBattleDictToArray();
			this.AddPlayerToCorrectTeam(newPlayer);
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (this.playerLives.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerLives.Remove(otherPlayer.ActorNumber);
		}
		if (this.playerStatusDict.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerStatusDict.Remove(otherPlayer.ActorNumber);
		}
	}

	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyBattleDictToArray();
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			stream.SendNext(this.playerActorNumberArray[i]);
			stream.SendNext(this.playerLivesArray[i]);
			stream.SendNext(this.playerStatusArray[i]);
		}
		stream.SendNext((int)this.currentState);
	}

	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerActorNumberArray[i] = (int)stream.ReceiveNext();
			this.playerLivesArray[i] = (int)stream.ReceiveNext();
			this.playerStatusArray[i] = (GorillaBattleManager.BattleStatus)stream.ReceiveNext();
		}
		this.currentState = (GorillaBattleManager.BattleState)stream.ReceiveNext();
		this.CopyArrayToBattleDict();
	}

	public override int MyMatIndex(Player forPlayer)
	{
		this.tempStatus = this.GetPlayerStatus(forPlayer);
		if (this.tempStatus != GorillaBattleManager.BattleStatus.None)
		{
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.RedTeam))
			{
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Normal))
				{
					return 8;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Hit))
				{
					return 9;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Stunned))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Grace))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Eliminated))
				{
					return 11;
				}
			}
			else
			{
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Normal))
				{
					return 4;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Hit))
				{
					return 5;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Stunned))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Grace))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Eliminated))
				{
					return 7;
				}
			}
		}
		return 0;
	}

	public override float[] LocalPlayerSpeed()
	{
		if (this.playerStatusDict.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber, out this.tempStatus))
		{
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Normal))
			{
				this.playerSpeed[0] = 6.5f;
				this.playerSpeed[1] = 1.1f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Stunned))
			{
				this.playerSpeed[0] = 2f;
				this.playerSpeed[1] = 0.5f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Eliminated))
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
		}
		this.playerSpeed[0] = 6.5f;
		this.playerSpeed[1] = 1.1f;
		return this.playerSpeed;
	}

	public override void Tick()
	{
		base.Tick();
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			this.UpdateBattleState();
		}
		this.ActivateDefaultSlingShot();
	}

	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		foreach (int num in this.playerLives.Keys)
		{
			this.playerInList = false;
			using (List<Player>.Enumerator enumerator2 = RoomSystem.PlayersInRoom.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.ActorNumber == num)
					{
						this.playerInList = true;
					}
				}
			}
			if (!this.playerInList)
			{
				this.playerLives.Remove(num);
			}
		}
	}

	public int GetPlayerLives(Player player)
	{
		if (player == null)
		{
			return 0;
		}
		if (this.playerLives.TryGetValue(player.ActorNumber, out this.outLives))
		{
			return this.outLives;
		}
		return 0;
	}

	public bool PlayerInHitCooldown(Player player)
	{
		float num;
		return this.playerHitTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown > Time.time;
	}

	public bool PlayerInStunCooldown(Player player)
	{
		float num;
		return this.playerStunTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown + this.stunGracePeriod > Time.time;
	}

	public GorillaBattleManager.BattleStatus GetPlayerStatus(Player player)
	{
		if (this.playerStatusDict.TryGetValue(player.ActorNumber, out this.tempStatus))
		{
			return this.tempStatus;
		}
		return GorillaBattleManager.BattleStatus.None;
	}

	public bool OnRedTeam(GorillaBattleManager.BattleStatus status)
	{
		return this.HasFlag(status, GorillaBattleManager.BattleStatus.RedTeam);
	}

	public bool OnRedTeam(Player player)
	{
		GorillaBattleManager.BattleStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnRedTeam(playerStatus);
	}

	public bool OnBlueTeam(GorillaBattleManager.BattleStatus status)
	{
		return this.HasFlag(status, GorillaBattleManager.BattleStatus.BlueTeam);
	}

	public bool OnBlueTeam(Player player)
	{
		GorillaBattleManager.BattleStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnBlueTeam(playerStatus);
	}

	public bool OnNoTeam(GorillaBattleManager.BattleStatus status)
	{
		return !this.OnRedTeam(status) && !this.OnBlueTeam(status);
	}

	public bool OnNoTeam(Player player)
	{
		GorillaBattleManager.BattleStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnNoTeam(playerStatus);
	}

	public override bool LocalCanTag(Player myPlayer, Player otherPlayer)
	{
		return false;
	}

	public bool OnSameTeam(GorillaBattleManager.BattleStatus playerA, GorillaBattleManager.BattleStatus playerB)
	{
		bool flag = this.OnRedTeam(playerA) && this.OnRedTeam(playerB);
		bool flag2 = this.OnBlueTeam(playerA) && this.OnBlueTeam(playerB);
		return flag || flag2;
	}

	public bool OnSameTeam(Player myPlayer, Player otherPlayer)
	{
		GorillaBattleManager.BattleStatus playerStatus = this.GetPlayerStatus(myPlayer);
		GorillaBattleManager.BattleStatus playerStatus2 = this.GetPlayerStatus(otherPlayer);
		return this.OnSameTeam(playerStatus, playerStatus2);
	}

	public bool LocalCanHit(Player myPlayer, Player otherPlayer)
	{
		bool flag = !this.OnSameTeam(myPlayer, otherPlayer);
		bool flag2 = this.GetPlayerLives(otherPlayer) != 0;
		return flag && flag2;
	}

	private void CopyBattleDictToArray()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = 0;
		}
		this.keyValuePairs = this.playerLives.ToArray<KeyValuePair<int, int>>();
		int num = 0;
		while (num < this.playerLivesArray.Length && num < this.keyValuePairs.Length)
		{
			this.playerActorNumberArray[num] = this.keyValuePairs[num].Key;
			this.playerLivesArray[num] = this.keyValuePairs[num].Value;
			this.playerStatusArray[num] = this.GetPlayerStatus(PhotonNetwork.LocalPlayer.Get(this.keyValuePairs[num].Key));
			num++;
		}
	}

	private void CopyArrayToBattleDict()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			if (this.playerActorNumberArray[i] != 0 && Utils.PlayerInRoom(this.playerActorNumberArray[i]))
			{
				if (this.playerLives.TryGetValue(this.playerActorNumberArray[i], out this.outLives))
				{
					this.playerLives[this.playerActorNumberArray[i]] = this.playerLivesArray[i];
				}
				else
				{
					this.playerLives.Add(this.playerActorNumberArray[i], this.playerLivesArray[i]);
				}
				if (this.playerStatusDict.ContainsKey(this.playerActorNumberArray[i]))
				{
					this.playerStatusDict[this.playerActorNumberArray[i]] = this.playerStatusArray[i];
				}
				else
				{
					this.playerStatusDict.Add(this.playerActorNumberArray[i], this.playerStatusArray[i]);
				}
			}
		}
	}

	private GorillaBattleManager.BattleStatus SetFlag(GorillaBattleManager.BattleStatus currState, GorillaBattleManager.BattleStatus flag)
	{
		return currState | flag;
	}

	private GorillaBattleManager.BattleStatus SetFlagExclusive(GorillaBattleManager.BattleStatus currState, GorillaBattleManager.BattleStatus flag)
	{
		return flag;
	}

	private GorillaBattleManager.BattleStatus ClearFlag(GorillaBattleManager.BattleStatus currState, GorillaBattleManager.BattleStatus flag)
	{
		return currState & ~flag;
	}

	private bool FlagIsSet(GorillaBattleManager.BattleStatus currState, GorillaBattleManager.BattleStatus flag)
	{
		return (currState & flag) > GorillaBattleManager.BattleStatus.None;
	}

	public void RandomizeTeams()
	{
		int[] array = new int[RoomSystem.PlayersInRoom.Count];
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			array[i] = i;
		}
		Random rand = new Random();
		int[] array2 = array.OrderBy((int x) => rand.Next()).ToArray<int>();
		GorillaBattleManager.BattleStatus battleStatus = ((rand.Next(0, 2) == 0) ? GorillaBattleManager.BattleStatus.RedTeam : GorillaBattleManager.BattleStatus.BlueTeam);
		GorillaBattleManager.BattleStatus battleStatus2 = ((battleStatus == GorillaBattleManager.BattleStatus.RedTeam) ? GorillaBattleManager.BattleStatus.BlueTeam : GorillaBattleManager.BattleStatus.RedTeam);
		for (int j = 0; j < RoomSystem.PlayersInRoom.Count; j++)
		{
			GorillaBattleManager.BattleStatus battleStatus3 = ((array2[j] % 2 == 0) ? battleStatus2 : battleStatus);
			this.playerStatusDict[RoomSystem.PlayersInRoom[j].ActorNumber] = battleStatus3;
		}
	}

	public void AddPlayerToCorrectTeam(Player newPlayer)
	{
		this.rcount = 0;
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			if (this.playerStatusDict.ContainsKey(RoomSystem.PlayersInRoom[i].ActorNumber))
			{
				GorillaBattleManager.BattleStatus battleStatus = this.playerStatusDict[RoomSystem.PlayersInRoom[i].ActorNumber];
				this.rcount = (this.HasFlag(battleStatus, GorillaBattleManager.BattleStatus.RedTeam) ? (this.rcount + 1) : this.rcount);
			}
		}
		if ((RoomSystem.PlayersInRoom.Count - 1) / 2 == this.rcount)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = ((Random.Range(0, 2) == 0) ? this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaBattleManager.BattleStatus.RedTeam) : this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaBattleManager.BattleStatus.BlueTeam));
			return;
		}
		if (this.rcount <= (RoomSystem.PlayersInRoom.Count - 1) / 2)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaBattleManager.BattleStatus.RedTeam);
		}
	}

	private void InitializePlayerStatus()
	{
		this.keyValuePairsStatus = this.playerStatusDict.ToArray<KeyValuePair<int, GorillaBattleManager.BattleStatus>>();
		foreach (KeyValuePair<int, GorillaBattleManager.BattleStatus> keyValuePair in this.keyValuePairsStatus)
		{
			this.playerStatusDict[keyValuePair.Key] = GorillaBattleManager.BattleStatus.Normal;
		}
	}

	private void UpdatePlayerStatus()
	{
		this.keyValuePairsStatus = this.playerStatusDict.ToArray<KeyValuePair<int, GorillaBattleManager.BattleStatus>>();
		foreach (KeyValuePair<int, GorillaBattleManager.BattleStatus> keyValuePair in this.keyValuePairsStatus)
		{
			GorillaBattleManager.BattleStatus battleStatus = (this.HasFlag(this.playerStatusDict[keyValuePair.Key], GorillaBattleManager.BattleStatus.RedTeam) ? GorillaBattleManager.BattleStatus.RedTeam : GorillaBattleManager.BattleStatus.BlueTeam);
			if (this.playerLives.TryGetValue(keyValuePair.Key, out this.outLives) && this.outLives == 0)
			{
				this.playerStatusDict[keyValuePair.Key] = battleStatus | GorillaBattleManager.BattleStatus.Eliminated;
			}
			else if (this.playerHitTimes.TryGetValue(keyValuePair.Key, out this.outHitTime) && this.outHitTime + this.hitCooldown > Time.time)
			{
				this.playerStatusDict[keyValuePair.Key] = battleStatus | GorillaBattleManager.BattleStatus.Hit;
			}
			else if (this.playerStunTimes.TryGetValue(keyValuePair.Key, out this.outHitTime))
			{
				if (this.outHitTime + this.hitCooldown > Time.time)
				{
					this.playerStatusDict[keyValuePair.Key] = battleStatus | GorillaBattleManager.BattleStatus.Stunned;
				}
				else if (this.outHitTime + this.hitCooldown + this.stunGracePeriod > Time.time)
				{
					this.playerStatusDict[keyValuePair.Key] = battleStatus | GorillaBattleManager.BattleStatus.Grace;
				}
				else
				{
					this.playerStatusDict[keyValuePair.Key] = battleStatus | GorillaBattleManager.BattleStatus.Normal;
				}
			}
			else
			{
				this.playerStatusDict[keyValuePair.Key] = battleStatus | GorillaBattleManager.BattleStatus.Normal;
			}
		}
	}

	public GorillaBattleManager()
	{
	}

	private float playerMin = 2f;

	public float tagCoolDown = 5f;

	public Dictionary<int, int> playerLives = new Dictionary<int, int>();

	public Dictionary<int, GorillaBattleManager.BattleStatus> playerStatusDict = new Dictionary<int, GorillaBattleManager.BattleStatus>();

	public Dictionary<int, float> playerHitTimes = new Dictionary<int, float>();

	public Dictionary<int, float> playerStunTimes = new Dictionary<int, float>();

	public int[] playerActorNumberArray = new int[10];

	public int[] playerLivesArray = new int[10];

	public GorillaBattleManager.BattleStatus[] playerStatusArray = new GorillaBattleManager.BattleStatus[10];

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

	private KeyValuePair<int, GorillaBattleManager.BattleStatus>[] keyValuePairsStatus;

	private GorillaBattleManager.BattleStatus tempStatus;

	private GorillaBattleManager.BattleState currentState;

	public enum BattleStatus
	{
		RedTeam = 1,
		BlueTeam,
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

	[CompilerGenerated]
	private sealed class <>c__DisplayClass80_0
	{
		public <>c__DisplayClass80_0()
		{
		}

		internal int <RandomizeTeams>b__0(int x)
		{
			return this.rand.Next();
		}

		public Random rand;
	}

	[CompilerGenerated]
	private sealed class <StartBattleCountdown>d__44 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <StartBattleCountdown>d__44(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			GorillaBattleManager gorillaBattleManager = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				gorillaBattleManager.coroutineRunning = true;
				gorillaBattleManager.countDownTime = 5;
				break;
			case 1:
				this.<>1__state = -1;
				gorillaBattleManager.countDownTime--;
				break;
			case 2:
				this.<>1__state = -1;
				return false;
			default:
				return false;
			}
			if (gorillaBattleManager.countDownTime <= 0)
			{
				gorillaBattleManager.coroutineRunning = false;
				gorillaBattleManager.currentState = GorillaBattleManager.BattleState.GameStart;
				this.<>2__current = null;
				this.<>1__state = 2;
				return true;
			}
			try
			{
				RoomSystem.SendSoundEffectAll(6, 0.25f);
				foreach (Player player in RoomSystem.PlayersInRoom)
				{
					gorillaBattleManager.playerLives[player.ActorNumber] = 3;
				}
			}
			catch
			{
			}
			this.<>2__current = new WaitForSeconds(1f);
			this.<>1__state = 1;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public GorillaBattleManager <>4__this;
	}
}
