using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000161 RID: 353
internal class VRRigCache : MonoBehaviourPunCallbacks
{
	// Token: 0x060008B3 RID: 2227 RVA: 0x000356A0 File Offset: 0x000338A0
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

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060008B4 RID: 2228 RVA: 0x00035734 File Offset: 0x00033934
	// (set) Token: 0x060008B5 RID: 2229 RVA: 0x0003573B File Offset: 0x0003393B
	public static VRRigCache Instance { get; private set; }

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060008B6 RID: 2230 RVA: 0x00035743 File Offset: 0x00033943
	public Transform NetworkParent
	{
		get
		{
			return this.networkParent;
		}
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0003574C File Offset: 0x0003394C
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

	// Token: 0x060008B8 RID: 2232 RVA: 0x000357D8 File Offset: 0x000339D8
	private void OnDestroy()
	{
		if (VRRigCache.Instance == this)
		{
			VRRigCache.Instance = null;
		}
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x000357ED File Offset: 0x000339ED
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

	// Token: 0x060008BA RID: 2234 RVA: 0x00035828 File Offset: 0x00033A28
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

	// Token: 0x060008BB RID: 2235 RVA: 0x000358E0 File Offset: 0x00033AE0
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

	// Token: 0x060008BC RID: 2236 RVA: 0x00035944 File Offset: 0x00033B44
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		RigContainer rigContainer;
		if (this.TryGetVrrig(newPlayer, out rigContainer))
		{
			this.AddRigToGorillaParent(newPlayer, rigContainer.Rig);
		}
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x0003596C File Offset: 0x00033B6C
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

	// Token: 0x060008BE RID: 2238 RVA: 0x000359AC File Offset: 0x00033BAC
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

	// Token: 0x060008BF RID: 2239 RVA: 0x00035A04 File Offset: 0x00033C04
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

	// Token: 0x060008C0 RID: 2240 RVA: 0x00035A68 File Offset: 0x00033C68
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

	// Token: 0x060008C1 RID: 2241 RVA: 0x00035AE9 File Offset: 0x00033CE9
	private void LogInfo(string log)
	{
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x00035AEB File Offset: 0x00033CEB
	private void LogWarning(string log)
	{
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x00035AED File Offset: 0x00033CED
	private void LogError(string log)
	{
	}

	// Token: 0x04000AE0 RID: 2784
	public RigContainer localRig;

	// Token: 0x04000AE1 RID: 2785
	[SerializeField]
	private Transform rigParent;

	// Token: 0x04000AE2 RID: 2786
	[SerializeField]
	private Transform networkParent;

	// Token: 0x04000AE3 RID: 2787
	[SerializeField]
	private GameObject rigTemplate;

	// Token: 0x04000AE4 RID: 2788
	[SerializeField]
	private float rigAmount = 10f;

	// Token: 0x04000AE5 RID: 2789
	private static Queue<RigContainer> freeRigs = new Queue<RigContainer>(10);

	// Token: 0x04000AE6 RID: 2790
	private static Dictionary<Player, RigContainer> rigsInUse = new Dictionary<Player, RigContainer>(10);
}
