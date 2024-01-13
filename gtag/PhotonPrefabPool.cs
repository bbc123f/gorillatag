using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class PhotonPrefabPool : MonoBehaviour, IPunPrefabPool
{
	[SerializeField]
	private PrefabType[] networkPrefabsData;

	private Dictionary<string, GameObject> networkPrefabs = new Dictionary<string, GameObject>();

	private Queue<GameObject> objectsWaiting = new Queue<GameObject>(20);

	private HashSet<GameObject> netInstantiedObjects = new HashSet<GameObject>();

	private List<PhotonView> tempViews = new List<PhotonView>(5);

	private Coroutine frameDelay;

	private IEnumerator waitOneFrame;

	private bool waiting;

	private void Start()
	{
		PhotonNetwork.PrefabPool = this;
		waitOneFrame = WaitOneFrame();
		PhotonVoiceNetwork.Instance.RemoteVoiceAdded += CheckVOIPSettings;
		for (int i = 0; i < networkPrefabsData.Length; i++)
		{
			PrefabType prefabType = networkPrefabsData[i];
			if ((bool)prefabType.prefab)
			{
				if (string.IsNullOrEmpty(prefabType.prefabName))
				{
					prefabType.prefabName = prefabType.prefab.name;
				}
				networkPrefabs.Add(prefabType.prefabName, prefabType.prefab);
			}
		}
	}

	GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		if (!networkPrefabs.TryGetValue(prefabId, out var value))
		{
			return null;
		}
		bool activeSelf = value.activeSelf;
		if (activeSelf)
		{
			value.SetActive(value: false);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(value, position, rotation);
		netInstantiedObjects.Add(gameObject);
		if (activeSelf)
		{
			value.SetActive(value: true);
		}
		return gameObject;
	}

	void IPunPrefabPool.Destroy(GameObject gameObject)
	{
		if (netInstantiedObjects.Contains(gameObject))
		{
			netInstantiedObjects.Remove(gameObject);
			UnityEngine.Object.Destroy(gameObject);
			return;
		}
		objectsWaiting.Enqueue(gameObject);
		if (!waiting)
		{
			waiting = true;
			frameDelay = StartCoroutine(WaitOneFrame());
		}
	}

	private IEnumerator WaitOneFrame()
	{
		yield return null;
		while (objectsWaiting.Count > 0)
		{
			GameObject obj = objectsWaiting.Dequeue();
			obj.SetActive(value: true);
			obj.GetComponents(tempViews);
			for (int i = 0; i < tempViews.Count; i++)
			{
				PhotonNetwork.RegisterPhotonView(tempViews[i]);
			}
		}
		waiting = false;
		StopCoroutine(frameDelay);
	}

	private void CheckVOIPSettings(RemoteVoiceLink voiceLink)
	{
		try
		{
			Player player = PhotonNetwork.CurrentRoom.GetPlayer((int)voiceLink.Info.UserData / PhotonNetwork.MAX_VIEW_IDS);
			if (player != null && (voiceLink.Info.Bitrate > 20000 || voiceLink.Info.SamplingRate > 16000) && VRRigCache.Instance.TryGetVrrig(player, out var playerRig))
			{
				playerRig.ForceMute = true;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}
}
