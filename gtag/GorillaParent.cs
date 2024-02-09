using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaParent : MonoBehaviour
{
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

	protected void OnDestroy()
	{
		if (GorillaParent.instance == this)
		{
			GorillaParent.hasInstance = false;
			GorillaParent.instance = null;
		}
	}

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

	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaParent.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaParent.onReplicatedClientReady = (Action)Delegate.Combine(GorillaParent.onReplicatedClientReady, action);
	}

	public GameObject tagUI;

	public GameObject playerParent;

	public GameObject vrrigParent;

	[OnEnterPlay_SetNull]
	public static volatile GorillaParent instance;

	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	public List<VRRig> vrrigs;

	public Dictionary<Player, VRRig> vrrigDict = new Dictionary<Player, VRRig>();

	private int i;

	private PhotonView[] childPhotonViews;

	private bool joinedRoom;

	private static bool replicatedClientReady;

	private static Action onReplicatedClientReady;
}
