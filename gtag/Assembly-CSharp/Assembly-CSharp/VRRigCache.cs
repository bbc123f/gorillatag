using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class VRRigCache : MonoBehaviourPunCallbacks
{
	public static bool TryFindRigPlayer(VRRig rig, out Player player)
	{
		player = null;
		if (rig == null)
		{
			return false;
		}
		if (VRRigCache.rigsInUse == null)
		{
			return false;
		}
		foreach (KeyValuePair<Player, RigContainer> keyValuePair in VRRigCache.rigsInUse)
		{
			RigContainer value = keyValuePair.Value;
			if (!(value == null) && !(value.Rig != rig))
			{
				player = keyValuePair.Key;
				return true;
			}
		}
		return false;
	}

	public static VRRigCache Instance { get; private set; }

	public Transform NetworkParent
	{
		get
		{
			return this.networkParent;
		}
	}

	private void Start()
	{
		if (VRRigCache.Instance != null && VRRigCache.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		VRRigCache.Instance = this;
		if (this.rigParent == null)
		{
			this.rigParent = base.transform;
		}
		if (this.networkParent == null)
		{
			this.networkParent = base.transform;
		}
		int num = 0;
		while ((float)num < this.rigAmount)
		{
			RigContainer item = this.SpawnRig();
			VRRigCache.freeRigs.Enqueue(item);
			num++;
		}
	}

	private void OnDestroy()
	{
		if (VRRigCache.Instance == this)
		{
			VRRigCache.Instance = null;
		}
	}

	private RigContainer SpawnRig()
	{
		if (this.rigTemplate.activeSelf)
		{
			this.rigTemplate.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.rigTemplate, this.rigParent, false);
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<RigContainer>();
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
			playerRig = this.localRig;
			return true;
		}
		if (!targetPlayer.InRoom())
		{
			this.LogWarning("player is not in room?? " + targetPlayer.ToStringFull());
			return false;
		}
		if (VRRigCache.rigsInUse.ContainsKey(targetPlayer))
		{
			playerRig = VRRigCache.rigsInUse[targetPlayer];
		}
		else
		{
			if (VRRigCache.freeRigs.Count <= 0)
			{
				this.LogWarning("all rigs are in use");
				return false;
			}
			playerRig = VRRigCache.freeRigs.Dequeue();
			playerRig.Creator = targetPlayer;
			VRRigCache.rigsInUse.Add(targetPlayer, playerRig);
			playerRig.gameObject.SetActive(true);
		}
		return true;
	}

	private void AddRigToGorillaParent(Player player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (instance == null)
		{
			return;
		}
		if (!instance.vrrigs.Contains(vrrig))
		{
			instance.vrrigs.Add(vrrig);
		}
		if (!instance.vrrigDict.ContainsKey(player))
		{
			instance.vrrigDict.Add(player, vrrig);
			return;
		}
		instance.vrrigDict[player] = vrrig;
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		RigContainer rigContainer;
		if (this.TryGetVrrig(newPlayer, out rigContainer))
		{
			this.AddRigToGorillaParent(newPlayer, rigContainer.Rig);
		}
	}

	public override void OnJoinedRoom()
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			RigContainer rigContainer;
			if (this.TryGetVrrig(player, out rigContainer))
			{
				this.AddRigToGorillaParent(player, rigContainer.Rig);
			}
		}
	}

	private void RemoveRigFromGorillaParent(Player player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (instance == null)
		{
			return;
		}
		if (instance.vrrigs.Contains(vrrig))
		{
			instance.vrrigs.Remove(vrrig);
		}
		if (instance.vrrigDict.ContainsKey(player))
		{
			instance.vrrigDict.Remove(player);
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		RigContainer rigContainer;
		if (VRRigCache.rigsInUse.TryGetValue(otherPlayer, out rigContainer))
		{
			rigContainer.gameObject.Disable();
			VRRigCache.freeRigs.Enqueue(rigContainer);
			VRRigCache.rigsInUse.Remove(otherPlayer);
			this.RemoveRigFromGorillaParent(otherPlayer, rigContainer.Rig);
			return;
		}
		this.LogError("failed to find player's vrrig who left " + otherPlayer.ToStringFull());
	}

	public override void OnLeftRoom()
	{
		foreach (Player player in VRRigCache.rigsInUse.Keys.ToArray<Player>())
		{
			RigContainer rigContainer = VRRigCache.rigsInUse[player];
			if (!(rigContainer == null))
			{
				VRRig rig = VRRigCache.rigsInUse[player].Rig;
				rigContainer.gameObject.Disable();
				VRRigCache.rigsInUse.Remove(player);
				this.RemoveRigFromGorillaParent(player, rig);
				VRRigCache.freeRigs.Enqueue(rigContainer);
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
}
