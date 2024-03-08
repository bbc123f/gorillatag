using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;

public class PhotonPrefabPool : MonoBehaviour, IPunPrefabPool, ITickSystemPre
{
	bool ITickSystemPre.PreTickRunning { get; set; }

	private void Start()
	{
		PhotonNetwork.PrefabPool = this;
		NetworkSystem.Instance.AddRemoteVoiceAddedCallback(new Action<RemoteVoiceLink>(this.CheckVOIPSettings));
		for (int i = 0; i < this.networkPrefabsData.Length; i++)
		{
			PrefabType prefabType = this.networkPrefabsData[i];
			if (prefabType.prefab)
			{
				if (string.IsNullOrEmpty(prefabType.prefabName))
				{
					prefabType.prefabName = prefabType.prefab.name;
				}
				this.networkPrefabs.Add(prefabType.prefabName, prefabType.prefab);
			}
		}
	}

	GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject;
		if (!this.networkPrefabs.TryGetValue(prefabId, out gameObject))
		{
			return null;
		}
		bool activeSelf = gameObject.activeSelf;
		if (activeSelf)
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, position, rotation);
		this.netInstantiedObjects.Add(gameObject2);
		if (activeSelf)
		{
			gameObject.SetActive(true);
		}
		return gameObject2;
	}

	void IPunPrefabPool.Destroy(GameObject gameObject)
	{
		if (gameObject.IsNull())
		{
			return;
		}
		if (this.netInstantiedObjects.Contains(gameObject))
		{
			this.netInstantiedObjects.Remove(gameObject);
			Object.Destroy(gameObject);
			return;
		}
		if (!this.objectsQueued.Contains(gameObject))
		{
			this.objectsWaiting.Enqueue(gameObject);
			this.objectsQueued.Add(gameObject);
		}
		if (!this.waiting)
		{
			this.waiting = true;
			TickSystem<object>.AddPreTickCallback(this);
		}
	}

	void ITickSystemPre.PreTick()
	{
		if (this.waiting)
		{
			this.waiting = false;
			return;
		}
		while (this.objectsWaiting.Count > 0)
		{
			GameObject gameObject = this.objectsWaiting.Dequeue();
			this.objectsQueued.Remove(gameObject);
			if (!gameObject.IsNull())
			{
				gameObject.SetActive(true);
				gameObject.GetComponents<PhotonView>(this.tempViews);
				for (int i = 0; i < this.tempViews.Count; i++)
				{
					PhotonNetwork.RegisterPhotonView(this.tempViews[i]);
				}
			}
		}
		TickSystem<object>.RemovePreTickCallback(this);
	}

	private void CheckVOIPSettings(RemoteVoiceLink voiceLink)
	{
		try
		{
			Player player = PhotonNetwork.CurrentRoom.GetPlayer((int)voiceLink.Info.UserData / PhotonNetwork.MAX_VIEW_IDS, false);
			if (player != null)
			{
				RigContainer rigContainer;
				if ((voiceLink.Info.Bitrate > 20000 || voiceLink.Info.SamplingRate > 16000) && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
				{
					rigContainer.ForceMute = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	[SerializeField]
	private PrefabType[] networkPrefabsData;

	public Dictionary<string, GameObject> networkPrefabs = new Dictionary<string, GameObject>();

	private Queue<GameObject> objectsWaiting = new Queue<GameObject>(20);

	private HashSet<GameObject> objectsQueued = new HashSet<GameObject>();

	private HashSet<GameObject> netInstantiedObjects = new HashSet<GameObject>();

	private List<PhotonView> tempViews = new List<PhotonView>(5);

	private bool waiting;
}
