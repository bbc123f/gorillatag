using System;
using System.Collections.Generic;
using GorillaExtensions;
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
		if (RoomSystem.JoinedRoom && GorillaTagger.Instance.myVRRig.IsNull())
		{
			GameObject gameObject;
			VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", out gameObject);
			if (gameObject == null)
			{
				return;
			}
			NetworkSystem.Instance.NetInstantiate(gameObject, GorillaTagger.Instance.offlineVRRig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.rotation, false);
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

	public GorillaParent()
	{
	}

	public GameObject tagUI;

	public GameObject playerParent;

	public GameObject vrrigParent;

	[OnEnterPlay_SetNull]
	public static volatile GorillaParent instance;

	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	public List<VRRig> vrrigs;

	public Dictionary<NetPlayer, VRRig> vrrigDict = new Dictionary<NetPlayer, VRRig>();

	private int i;

	private bool joinedRoom;

	private static bool replicatedClientReady;

	private static Action onReplicatedClientReady;
}
