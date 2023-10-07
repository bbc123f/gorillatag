using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class GorillaParent : MonoBehaviour
{
	// Token: 0x060009C6 RID: 2502 RVA: 0x0003BAE8 File Offset: 0x00039CE8
	public void Awake()
	{
		if (GorillaParent.instance == null)
		{
			GorillaParent.instance = this;
			GorillaParent.hasInstance = true;
			return;
		}
		if (GorillaParent.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0003BB23 File Offset: 0x00039D23
	protected void OnDestroy()
	{
		if (GorillaParent.instance == this)
		{
			GorillaParent.hasInstance = false;
			GorillaParent.instance = null;
		}
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x0003BB44 File Offset: 0x00039D44
	public void LateUpdate()
	{
		this.i = this.vrrigs.Count - 1;
		while (this.i > -1)
		{
			if (this.vrrigs[this.i] == null)
			{
				this.vrrigs.RemoveAt(this.i);
			}
			this.i--;
		}
		if (RoomSystem.JoinedRoom && GorillaTagger.Instance.offlineVRRig.photonView == null)
		{
			PhotonNetwork.Instantiate("GorillaPrefabs/Gorilla Player Networked", Vector3.zero, Quaternion.identity, 0, null);
		}
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0003BBDC File Offset: 0x00039DDC
	public static void ReplicatedClientReady()
	{
		GorillaParent.replicatedClientReady = true;
		Action action = GorillaParent.onReplicatedClientReady;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0003BBF3 File Offset: 0x00039DF3
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaParent.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaParent.onReplicatedClientReady = (Action)Delegate.Combine(GorillaParent.onReplicatedClientReady, action);
	}

	// Token: 0x04000BFF RID: 3071
	public GameObject tagUI;

	// Token: 0x04000C00 RID: 3072
	public GameObject playerParent;

	// Token: 0x04000C01 RID: 3073
	public GameObject vrrigParent;

	// Token: 0x04000C02 RID: 3074
	public static volatile GorillaParent instance;

	// Token: 0x04000C03 RID: 3075
	public static bool hasInstance;

	// Token: 0x04000C04 RID: 3076
	public List<VRRig> vrrigs;

	// Token: 0x04000C05 RID: 3077
	public Dictionary<Player, VRRig> vrrigDict = new Dictionary<Player, VRRig>();

	// Token: 0x04000C06 RID: 3078
	private int i;

	// Token: 0x04000C07 RID: 3079
	private PhotonView[] childPhotonViews;

	// Token: 0x04000C08 RID: 3080
	private bool joinedRoom;

	// Token: 0x04000C09 RID: 3081
	private static bool replicatedClientReady;

	// Token: 0x04000C0A RID: 3082
	private static Action onReplicatedClientReady;
}
