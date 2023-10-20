using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class GorillaParent : MonoBehaviour
{
	// Token: 0x060009CB RID: 2507 RVA: 0x0003BC18 File Offset: 0x00039E18
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

	// Token: 0x060009CC RID: 2508 RVA: 0x0003BC53 File Offset: 0x00039E53
	protected void OnDestroy()
	{
		if (GorillaParent.instance == this)
		{
			GorillaParent.hasInstance = false;
			GorillaParent.instance = null;
		}
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0003BC74 File Offset: 0x00039E74
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

	// Token: 0x060009CE RID: 2510 RVA: 0x0003BD0C File Offset: 0x00039F0C
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

	// Token: 0x060009CF RID: 2511 RVA: 0x0003BD23 File Offset: 0x00039F23
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaParent.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaParent.onReplicatedClientReady = (Action)Delegate.Combine(GorillaParent.onReplicatedClientReady, action);
	}

	// Token: 0x04000C03 RID: 3075
	public GameObject tagUI;

	// Token: 0x04000C04 RID: 3076
	public GameObject playerParent;

	// Token: 0x04000C05 RID: 3077
	public GameObject vrrigParent;

	// Token: 0x04000C06 RID: 3078
	public static volatile GorillaParent instance;

	// Token: 0x04000C07 RID: 3079
	public static bool hasInstance;

	// Token: 0x04000C08 RID: 3080
	public List<VRRig> vrrigs;

	// Token: 0x04000C09 RID: 3081
	public Dictionary<Player, VRRig> vrrigDict = new Dictionary<Player, VRRig>();

	// Token: 0x04000C0A RID: 3082
	private int i;

	// Token: 0x04000C0B RID: 3083
	private PhotonView[] childPhotonViews;

	// Token: 0x04000C0C RID: 3084
	private bool joinedRoom;

	// Token: 0x04000C0D RID: 3085
	private static bool replicatedClientReady;

	// Token: 0x04000C0E RID: 3086
	private static Action onReplicatedClientReady;
}
