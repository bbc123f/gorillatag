using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class VRRigCache : MonoBehaviourPunCallbacks
{
	public RigContainer localRig;

	[SerializeField]
	private Transform rigParent;

	[SerializeField]
	private Transform networkParent;

	[SerializeField]
	private GameObject rigTemplate;

	[SerializeField]
	private float rigAmount = 10f;

	private static Queue<RigContainer> freeRigs = new Queue<RigContainer>(10);

	private static Dictionary<Player, RigContainer> rigsInUse = new Dictionary<Player, RigContainer>(10);

	public static VRRigCache Instance { get; private set; }

	public Transform NetworkParent => networkParent;

	private void Start()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		if (rigParent == null)
		{
			rigParent = base.transform;
		}
		if (networkParent == null)
		{
			networkParent = base.transform;
		}
		for (int i = 0; (float)i < rigAmount; i++)
		{
			RigContainer item = SpawnRig();
			freeRigs.Enqueue(item);
		}
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private RigContainer SpawnRig()
	{
		if (rigTemplate.activeSelf)
		{
			rigTemplate.SetActive(value: false);
		}
		return Object.Instantiate(rigTemplate, rigParent, worldPositionStays: false)?.GetComponent<RigContainer>();
	}

	internal bool TryGetVrrig(Player targetPlayer, out RigContainer playerRig)
	{
		playerRig = null;
		if (targetPlayer == null)
		{
			Debug.LogWarning("Player for rig is null");
			return false;
		}
		if (targetPlayer.IsLocal)
		{
			playerRig = localRig;
			return true;
		}
		if (!targetPlayer.InRoom())
		{
			LogWarning("player is not in room?? " + targetPlayer.ToStringFull());
			return false;
		}
		if (rigsInUse.ContainsKey(targetPlayer))
		{
			playerRig = rigsInUse[targetPlayer];
		}
		else
		{
			if (freeRigs.Count <= 0)
			{
				LogWarning("all rigs are in use");
				return false;
			}
			playerRig = freeRigs.Dequeue();
			playerRig.Creator = targetPlayer;
			rigsInUse.Add(targetPlayer, playerRig);
			playerRig.gameObject.SetActive(value: true);
		}
		return true;
	}

	private void AddRigToGorillaParent(Player player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (!(instance == null))
		{
			if (!instance.vrrigs.Contains(vrrig))
			{
				instance.vrrigs.Add(vrrig);
			}
			if (!instance.vrrigDict.ContainsKey(player))
			{
				instance.vrrigDict.Add(player, vrrig);
			}
			else
			{
				instance.vrrigDict[player] = vrrig;
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (TryGetVrrig(newPlayer, out var playerRig))
		{
			AddRigToGorillaParent(newPlayer, playerRig.Rig);
		}
	}

	public override void OnJoinedRoom()
	{
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player player in playerList)
		{
			if (TryGetVrrig(player, out var playerRig))
			{
				AddRigToGorillaParent(player, playerRig.Rig);
			}
		}
	}

	private void RemoveRigFromGorillaParent(Player player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (!(instance == null))
		{
			if (instance.vrrigs.Contains(vrrig))
			{
				instance.vrrigs.Remove(vrrig);
			}
			if (instance.vrrigDict.ContainsKey(player))
			{
				instance.vrrigDict.Remove(player);
			}
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (rigsInUse.TryGetValue(otherPlayer, out var value))
		{
			value.gameObject.Disable();
			freeRigs.Enqueue(value);
			rigsInUse.Remove(otherPlayer);
			RemoveRigFromGorillaParent(otherPlayer, value.Rig);
		}
		else
		{
			LogError("failed to find player's vrrig who left " + otherPlayer.ToStringFull());
		}
	}

	public override void OnLeftRoom()
	{
		Player[] array = rigsInUse.Keys.ToArray();
		foreach (Player player in array)
		{
			RigContainer rigContainer = rigsInUse[player];
			if (!(rigContainer == null))
			{
				VRRig rig = rigsInUse[player].Rig;
				rigContainer.gameObject.Disable();
				rigsInUse.Remove(player);
				RemoveRigFromGorillaParent(player, rig);
				freeRigs.Enqueue(rigContainer);
			}
		}
	}

	private void LogInfo(string log)
	{
	}

	private void LogWarning(string log)
	{
	}

	private void LogError(string log)
	{
	}
}
