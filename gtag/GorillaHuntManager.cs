using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaHuntManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks, IPunObservable
{
	public float tagCoolDown = 5f;

	public int[] currentHuntedArray = new int[10];

	public List<Player> currentHunted = new List<Player>();

	public int[] currentTargetArray = new int[10];

	public List<Player> currentTarget = new List<Player>();

	public bool huntStarted;

	public bool waitingToStartNextHuntGame;

	public bool inStartCountdown;

	public int countDownTime;

	public double timeHuntGameEnded;

	public float timeLastSlowTagged;

	public object objRef;

	private int iterator1;

	private Player tempRandPlayer;

	private int tempRandIndex;

	private int notHuntedCount;

	private int tempTargetIndex;

	private Player tempPlayer;

	private int copyListToArrayIndex;

	private int copyArrayToListIndex;

	public override string GameMode()
	{
		return "HUNT";
	}

	public override void Awake()
	{
		base.Awake();
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(value: true);
	}

	public void UpdateState()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (PhotonNetwork.CurrentRoom.PlayerCount <= 3)
		{
			CleanUpHunt();
			huntStarted = false;
			waitingToStartNextHuntGame = false;
			for (iterator1 = 0; iterator1 < PhotonNetwork.PlayerList.Length; iterator1++)
			{
				FindVRRigForPlayer(PhotonNetwork.PlayerList[iterator1]).RPC("PlayTagSound", PhotonNetwork.PlayerList[iterator1], 0, 0.25f);
			}
		}
		else if (PhotonNetwork.CurrentRoom.PlayerCount > 3 && !huntStarted && !waitingToStartNextHuntGame && !inStartCountdown)
		{
			StartCoroutine(StartHuntCountdown());
		}
		else
		{
			UpdateHuntState();
		}
	}

	public void CleanUpHunt()
	{
		if (base.photonView.IsMine)
		{
			currentHunted.Clear();
			currentTarget.Clear();
			CopyHuntDataListToArray();
		}
	}

	public IEnumerator StartHuntCountdown()
	{
		if (base.photonView.IsMine && !inStartCountdown)
		{
			inStartCountdown = true;
			countDownTime = 5;
			CleanUpHunt();
			while (countDownTime > 0)
			{
				yield return new WaitForSeconds(1f);
				countDownTime--;
			}
			StartHunt();
		}
		yield return null;
	}

	public void StartHunt()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		huntStarted = true;
		waitingToStartNextHuntGame = false;
		countDownTime = 0;
		inStartCountdown = false;
		CleanUpHunt();
		for (iterator1 = 0; iterator1 < PhotonNetwork.PlayerList.Length; iterator1++)
		{
			if (currentTarget.Count < 10)
			{
				currentTarget.Add(PhotonNetwork.PlayerList[iterator1]);
				FindVRRigForPlayer(PhotonNetwork.PlayerList[iterator1]).RPC("PlayTagSound", PhotonNetwork.PlayerList[iterator1], 0, 0.25f);
			}
		}
		RandomizePlayerList(ref currentTarget);
		CopyHuntDataListToArray();
	}

	public void RandomizePlayerList(ref List<Player> listToRandomize)
	{
		for (int i = 0; i < listToRandomize.Count - 1; i++)
		{
			tempRandIndex = Random.Range(i, listToRandomize.Count);
			tempRandPlayer = listToRandomize[i];
			listToRandomize[i] = listToRandomize[tempRandIndex];
			listToRandomize[tempRandIndex] = tempRandPlayer;
		}
	}

	public IEnumerator HuntEnd()
	{
		if (base.photonView.IsMine)
		{
			while ((double)Time.time < timeHuntGameEnded + (double)tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (waitingToStartNextHuntGame)
			{
				StartCoroutine(StartHuntCountdown());
			}
			yield return null;
		}
		yield return null;
	}

	public void UpdateHuntState()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		CopyHuntDataListToArray();
		notHuntedCount = 0;
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player item in playerList)
		{
			if (currentTarget.Contains(item) && !currentHunted.Contains(item))
			{
				notHuntedCount++;
			}
		}
		if (notHuntedCount <= 2 && huntStarted)
		{
			EndHuntGame();
		}
	}

	private void EndHuntGame()
	{
		if (base.photonView.IsMine)
		{
			Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Player player in playerList)
			{
				PhotonView obj = FindVRRigForPlayer(player);
				obj.RPC("SetTaggedTime", player, null);
				obj.RPC("PlayTagSound", player, 2, 0.25f);
			}
			huntStarted = false;
			timeHuntGameEnded = Time.time;
			waitingToStartNextHuntGame = true;
			StartCoroutine(HuntEnd());
		}
	}

	public override bool LocalCanTag(Player myPlayer, Player otherPlayer)
	{
		if (waitingToStartNextHuntGame || countDownTime > 0 || GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Frozen)
		{
			return false;
		}
		if (currentHunted.Contains(myPlayer) && !currentHunted.Contains(otherPlayer) && Time.time > timeLastSlowTagged + 1f)
		{
			timeLastSlowTagged = Time.time;
			return true;
		}
		if (IsTargetOf(myPlayer, otherPlayer))
		{
			return true;
		}
		return false;
	}

	public override void ReportTag(Player taggedPlayer, Player taggingPlayer)
	{
		if (base.photonView.IsMine && !waitingToStartNextHuntGame)
		{
			PhotonView photonView = FindVRRigForPlayer(taggedPlayer);
			if ((currentHunted.Contains(taggingPlayer) || !currentTarget.Contains(taggingPlayer)) && !currentHunted.Contains(taggedPlayer) && currentTarget.Contains(taggedPlayer))
			{
				photonView.RPC("SetSlowedTime", taggedPlayer, null);
				photonView.RPC("PlayTagSound", RpcTarget.All, 5, 0.125f);
			}
			else if (IsTargetOf(taggingPlayer, taggedPlayer))
			{
				photonView.RPC("SetTaggedTime", taggedPlayer, null);
				photonView.RPC("PlayTagSound", RpcTarget.All, 0, 0.25f);
				currentHunted.Add(taggedPlayer);
				CopyHuntDataListToArray();
				UpdateHuntState();
			}
		}
	}

	public bool IsTargetOf(Player huntingPlayer, Player huntedPlayer)
	{
		if (!currentHunted.Contains(huntingPlayer) && !currentHunted.Contains(huntedPlayer) && currentTarget.Contains(huntingPlayer) && currentTarget.Contains(huntedPlayer))
		{
			return huntedPlayer == GetTargetOf(huntingPlayer);
		}
		return false;
	}

	public Player GetTargetOf(Player player)
	{
		if (currentHunted.Contains(player) || !currentTarget.Contains(player))
		{
			return null;
		}
		tempTargetIndex = currentTarget.IndexOf(player);
		for (int num = (tempTargetIndex + 1) % currentTarget.Count; num != tempTargetIndex; num = (num + 1) % currentTarget.Count)
		{
			if (currentTarget[num] == player)
			{
				return null;
			}
			if (!currentHunted.Contains(currentTarget[num]) && currentTarget[num] != null)
			{
				return currentTarget[num];
			}
		}
		return null;
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		_ = base.photonView.IsMine;
	}

	public override void NewVRRig(Player player, int vrrigPhotonViewID, bool didntDoTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didntDoTutorial);
		if (base.photonView.IsMine)
		{
			if (!waitingToStartNextHuntGame && huntStarted)
			{
				currentHunted.Add(player);
				CopyHuntDataListToArray();
			}
			UpdateState();
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (base.photonView.IsMine)
		{
			if (currentTarget.Contains(otherPlayer))
			{
				currentTarget.Remove(otherPlayer);
				CopyHuntDataListToArray();
			}
			if (currentHunted.Contains(otherPlayer))
			{
				currentHunted.Remove(otherPlayer);
				CopyHuntDataListToArray();
			}
			UpdateState();
		}
		playerVRRigDict.Remove(otherPlayer.ActorNumber);
		playerCosmeticsLookup.Remove(otherPlayer.ActorNumber);
	}

	private void CopyHuntDataListToArray()
	{
		for (copyListToArrayIndex = 0; copyListToArrayIndex < 10; copyListToArrayIndex++)
		{
			currentHuntedArray[copyListToArrayIndex] = 0;
			currentTargetArray[copyListToArrayIndex] = 0;
		}
		for (copyListToArrayIndex = currentHunted.Count - 1; copyListToArrayIndex >= 0; copyListToArrayIndex--)
		{
			if (currentHunted[copyListToArrayIndex] == null)
			{
				currentHunted.RemoveAt(copyListToArrayIndex);
			}
		}
		for (copyListToArrayIndex = currentTarget.Count - 1; copyListToArrayIndex >= 0; copyListToArrayIndex--)
		{
			if (currentTarget[copyListToArrayIndex] == null)
			{
				currentTarget.RemoveAt(copyListToArrayIndex);
			}
		}
		for (copyListToArrayIndex = 0; copyListToArrayIndex < currentHunted.Count; copyListToArrayIndex++)
		{
			currentHuntedArray[copyListToArrayIndex] = currentHunted[copyListToArrayIndex].ActorNumber;
		}
		for (copyListToArrayIndex = 0; copyListToArrayIndex < currentTarget.Count; copyListToArrayIndex++)
		{
			currentTargetArray[copyListToArrayIndex] = currentTarget[copyListToArrayIndex].ActorNumber;
		}
	}

	private void CopyHuntDataArrayToList()
	{
		currentTarget.Clear();
		for (copyArrayToListIndex = 0; copyArrayToListIndex < currentTargetArray.Length; copyArrayToListIndex++)
		{
			if (currentTargetArray[copyArrayToListIndex] != 0)
			{
				tempPlayer = PhotonNetwork.LocalPlayer.Get(currentTargetArray[copyArrayToListIndex]);
				if (tempPlayer != null)
				{
					currentTarget.Add(tempPlayer);
				}
			}
		}
		currentHunted.Clear();
		for (copyArrayToListIndex = 0; copyArrayToListIndex < currentHuntedArray.Length; copyArrayToListIndex++)
		{
			if (currentHuntedArray[copyArrayToListIndex] != 0)
			{
				tempPlayer = PhotonNetwork.LocalPlayer.Get(currentHuntedArray[copyArrayToListIndex]);
				if (tempPlayer != null)
				{
					currentHunted.Add(tempPlayer);
				}
			}
		}
	}

	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player player in playerList)
		{
			if (playerCosmeticsLookup.TryGetValue(player.ActorNumber, out tempString) && tempString == "BANNED")
			{
				PhotonNetwork.CloseConnection(player);
			}
		}
		CopyRoomDataToLocalData();
		UpdateState();
	}

	public void CopyRoomDataToLocalData()
	{
		waitingToStartNextHuntGame = false;
		UpdateHuntState();
	}

	[PunRPC]
	public override void ReportTagRPC(Player taggedPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTagRPC");
		ReportTag(taggedPlayer, info.Sender);
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(currentHuntedArray[0]);
				stream.SendNext(currentHuntedArray[1]);
				stream.SendNext(currentHuntedArray[2]);
				stream.SendNext(currentHuntedArray[3]);
				stream.SendNext(currentHuntedArray[4]);
				stream.SendNext(currentHuntedArray[5]);
				stream.SendNext(currentHuntedArray[6]);
				stream.SendNext(currentHuntedArray[7]);
				stream.SendNext(currentHuntedArray[8]);
				stream.SendNext(currentHuntedArray[9]);
				stream.SendNext(currentTargetArray[0]);
				stream.SendNext(currentTargetArray[1]);
				stream.SendNext(currentTargetArray[2]);
				stream.SendNext(currentTargetArray[3]);
				stream.SendNext(currentTargetArray[4]);
				stream.SendNext(currentTargetArray[5]);
				stream.SendNext(currentTargetArray[6]);
				stream.SendNext(currentTargetArray[7]);
				stream.SendNext(currentTargetArray[8]);
				stream.SendNext(currentTargetArray[9]);
				stream.SendNext(huntStarted);
				stream.SendNext(waitingToStartNextHuntGame);
				stream.SendNext(countDownTime);
			}
			else
			{
				currentHuntedArray[0] = (int)stream.ReceiveNext();
				currentHuntedArray[1] = (int)stream.ReceiveNext();
				currentHuntedArray[2] = (int)stream.ReceiveNext();
				currentHuntedArray[3] = (int)stream.ReceiveNext();
				currentHuntedArray[4] = (int)stream.ReceiveNext();
				currentHuntedArray[5] = (int)stream.ReceiveNext();
				currentHuntedArray[6] = (int)stream.ReceiveNext();
				currentHuntedArray[7] = (int)stream.ReceiveNext();
				currentHuntedArray[8] = (int)stream.ReceiveNext();
				currentHuntedArray[9] = (int)stream.ReceiveNext();
				currentTargetArray[0] = (int)stream.ReceiveNext();
				currentTargetArray[1] = (int)stream.ReceiveNext();
				currentTargetArray[2] = (int)stream.ReceiveNext();
				currentTargetArray[3] = (int)stream.ReceiveNext();
				currentTargetArray[4] = (int)stream.ReceiveNext();
				currentTargetArray[5] = (int)stream.ReceiveNext();
				currentTargetArray[6] = (int)stream.ReceiveNext();
				currentTargetArray[7] = (int)stream.ReceiveNext();
				currentTargetArray[8] = (int)stream.ReceiveNext();
				currentTargetArray[9] = (int)stream.ReceiveNext();
				huntStarted = (bool)stream.ReceiveNext();
				waitingToStartNextHuntGame = (bool)stream.ReceiveNext();
				countDownTime = (int)stream.ReceiveNext();
				CopyHuntDataArrayToList();
			}
		}
	}

	public override int MyMatIndex(Player forPlayer)
	{
		if (currentHunted.Contains(forPlayer) || (huntStarted && GetTargetOf(forPlayer) == null))
		{
			return 3;
		}
		return 0;
	}

	public override float[] LocalPlayerSpeed()
	{
		if (!currentHunted.Contains(PhotonNetwork.LocalPlayer) && (!huntStarted || GetTargetOf(PhotonNetwork.LocalPlayer) != null))
		{
			if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Slowed)
			{
				return new float[2] { 6.5f, 1.1f };
			}
			return new float[2] { 5.5f, 0.9f };
		}
		return new float[2] { 8.5f, 1.3f };
	}

	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		if (base.photonView.IsMine)
		{
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			Debug.Log(obj.ToString());
			if (obj.ToString().Contains("CASUAL") || obj.ToString().Contains("INFECTION"))
			{
				Object.Destroy(this);
			}
		}
	}
}
