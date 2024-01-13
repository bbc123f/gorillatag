using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaParent : MonoBehaviourPunCallbacks
{
	public GameObject tagUI;

	public GameObject playerParent;

	public GameObject vrrigParent;

	public static volatile GorillaParent instance;

	public static bool hasInstance;

	public List<VRRig> vrrigs;

	public Dictionary<Player, VRRig> vrrigDict = new Dictionary<Player, VRRig>();

	private int i;

	private PhotonView[] childPhotonViews;

	private bool joinedRoom;

	private static bool replicatedClientReady;

	private static Action onReplicatedClientReady;

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
			hasInstance = true;
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected void OnDestroy()
	{
		if (instance == this)
		{
			hasInstance = false;
			instance = null;
		}
	}

	public void LateUpdate()
	{
		for (i = vrrigs.Count - 1; i > -1; i--)
		{
			if (vrrigs[i] == null)
			{
				vrrigs.RemoveAt(i);
			}
		}
		if (PhotonNetwork.InRoom && joinedRoom)
		{
			if (GorillaTagger.Instance.offlineVRRig.photonView == null)
			{
				Debug.Log("online rig missing, re-instantiating it", base.gameObject);
				PhotonNetwork.Instantiate("GorillaPrefabs/Gorilla Player Networked", Vector3.zero, Quaternion.identity, 0);
			}
		}
		else if (!PhotonNetwork.InRoom && joinedRoom)
		{
			joinedRoom = false;
		}
	}

	public override void OnJoinedRoom()
	{
		joinedRoom = true;
	}

	public override void OnLeftRoom()
	{
		joinedRoom = false;
	}

	public static void ReplicatedClientReady()
	{
		replicatedClientReady = true;
		onReplicatedClientReady?.Invoke();
	}

	public static void OnReplicatedClientReady(Action action)
	{
		if (replicatedClientReady)
		{
			action();
		}
		else
		{
			onReplicatedClientReady = (Action)Delegate.Combine(onReplicatedClientReady, action);
		}
	}
}
