using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class PhotonPrefabPool : MonoBehaviour, IPunPrefabPool
{
	// Token: 0x06000899 RID: 2201 RVA: 0x000350C0 File Offset: 0x000332C0
	private void Start()
	{
		PhotonNetwork.PrefabPool = this;
		this.waitOneFrame = this.WaitOneFrame();
		PhotonVoiceNetwork.Instance.RemoteVoiceAdded += this.CheckVOIPSettings;
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

	// Token: 0x0600089A RID: 2202 RVA: 0x00035158 File Offset: 0x00033358
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

	// Token: 0x0600089B RID: 2203 RVA: 0x000351A8 File Offset: 0x000333A8
	void IPunPrefabPool.Destroy(GameObject gameObject)
	{
		if (this.netInstantiedObjects.Contains(gameObject))
		{
			this.netInstantiedObjects.Remove(gameObject);
			Object.Destroy(gameObject);
			return;
		}
		this.objectsWaiting.Enqueue(gameObject);
		if (!this.waiting)
		{
			this.waiting = true;
			this.frameDelay = base.StartCoroutine(this.WaitOneFrame());
		}
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x00035204 File Offset: 0x00033404
	private IEnumerator WaitOneFrame()
	{
		yield return null;
		while (this.objectsWaiting.Count > 0)
		{
			GameObject gameObject = this.objectsWaiting.Dequeue();
			gameObject.SetActive(true);
			gameObject.GetComponents<PhotonView>(this.tempViews);
			for (int i = 0; i < this.tempViews.Count; i++)
			{
				PhotonNetwork.RegisterPhotonView(this.tempViews[i]);
			}
		}
		this.waiting = false;
		base.StopCoroutine(this.frameDelay);
		yield break;
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x00035214 File Offset: 0x00033414
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

	// Token: 0x04000ACF RID: 2767
	[SerializeField]
	private PrefabType[] networkPrefabsData;

	// Token: 0x04000AD0 RID: 2768
	private Dictionary<string, GameObject> networkPrefabs = new Dictionary<string, GameObject>();

	// Token: 0x04000AD1 RID: 2769
	private Queue<GameObject> objectsWaiting = new Queue<GameObject>(20);

	// Token: 0x04000AD2 RID: 2770
	private HashSet<GameObject> netInstantiedObjects = new HashSet<GameObject>();

	// Token: 0x04000AD3 RID: 2771
	private List<PhotonView> tempViews = new List<PhotonView>(5);

	// Token: 0x04000AD4 RID: 2772
	private Coroutine frameDelay;

	// Token: 0x04000AD5 RID: 2773
	private IEnumerator waitOneFrame;

	// Token: 0x04000AD6 RID: 2774
	private bool waiting;
}
