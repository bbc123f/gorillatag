using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneManagement : MonoBehaviour
{
	public bool hasInstance
	{
		[CompilerGenerated]
		get
		{
			return this.<hasInstance>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<hasInstance>k__BackingField = value;
		}
	}

	private void Awake()
	{
		if (ZoneManagement.instance == null)
		{
			this.Initialize();
			return;
		}
		if (ZoneManagement.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public static void SetActiveZone(GTZone zone)
	{
		ZoneManagement.SetActiveZones(new GTZone[] { zone });
	}

	public static void SetActiveZones(GTZone[] zones)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		if (zones == null || zones.Length == 0)
		{
			return;
		}
		ZoneManagement.instance.SetZones(zones);
		Action action = ZoneManagement.instance.onZoneChanged;
		if (action == null)
		{
			return;
		}
		action();
	}

	public static bool IsInZone(GTZone zone)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	public GameObject GetPrimaryGameObject(GTZone zone)
	{
		return this.GetZoneData(zone).rootGameObjects[0];
	}

	public static void AddSceneToForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Add(sceneName);
	}

	public static void RemoveSceneFromForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Remove(sceneName);
	}

	public static void FindInstance()
	{
		ZoneManagement zoneManagement = Object.FindObjectOfType<ZoneManagement>();
		if (zoneManagement == null)
		{
			throw new NullReferenceException("Unable to find ZoneManagement object in scene.");
		}
		Debug.LogWarning("ZoneManagement accessed before MonoBehaviour awake function called; consider delaying zone management functions to avoid FindObject lookup.");
		zoneManagement.Initialize();
	}

	public bool IsSceneLoaded(GTZone gtZone)
	{
		foreach (ZoneData zoneData in this.zones)
		{
			if (zoneData.zone == gtZone && this.scenesLoaded.Contains(zoneData.sceneName))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsZoneActive(GTZone zone)
	{
		ZoneData zoneData = this.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	public HashSet<string> GetAllLoadedScenes()
	{
		return this.scenesLoaded;
	}

	public bool IsSceneLoaded(string sceneName)
	{
		return this.scenesLoaded.Contains(sceneName);
	}

	private void Initialize()
	{
		ZoneManagement.instance = this;
		this.hasInstance = true;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		List<GameObject> list = new List<GameObject>(8);
		for (int i = 0; i < this.zones.Length; i++)
		{
			list.Clear();
			ZoneData zoneData = this.zones[i];
			if (zoneData != null && zoneData.rootGameObjects != null)
			{
				hashSet.UnionWith(zoneData.rootGameObjects);
				for (int j = 0; j < zoneData.rootGameObjects.Length; j++)
				{
					GameObject gameObject = zoneData.rootGameObjects[j];
					if (!(gameObject == null))
					{
						list.Add(gameObject);
					}
				}
				hashSet.UnionWith(list);
			}
		}
		hashSet.Remove(null);
		this.allObjects = hashSet.ToArray<GameObject>();
		this.objectActivationState = new bool[this.allObjects.Length];
	}

	private void SetZones(GTZone[] newActiveZones)
	{
		for (int i = 0; i < this.objectActivationState.Length; i++)
		{
			this.objectActivationState[i] = false;
		}
		this.scenesRequested.Clear();
		this.scenesRequested.Add("GorillaTag");
		for (int j = 0; j < this.zones.Length; j++)
		{
			ZoneData zoneData = this.zones[j];
			if (zoneData == null || zoneData.rootGameObjects == null || !newActiveZones.Contains(zoneData.zone))
			{
				zoneData.active = false;
			}
			else
			{
				zoneData.active = true;
				if (!string.IsNullOrEmpty(zoneData.sceneName))
				{
					this.scenesRequested.Add(zoneData.sceneName);
				}
				foreach (GameObject gameObject in zoneData.rootGameObjects)
				{
					if (!(gameObject == null))
					{
						for (int l = 0; l < this.allObjects.Length; l++)
						{
							if (gameObject == this.allObjects[l])
							{
								this.objectActivationState[l] = true;
								break;
							}
						}
					}
				}
			}
		}
		int loadedSceneCount = SceneManager.loadedSceneCount;
		for (int m = 0; m < loadedSceneCount; m++)
		{
			this.scenesLoaded.Add(SceneManager.GetSceneAt(m).name);
		}
		foreach (string text in this.scenesRequested)
		{
			if (!this.scenesLoaded.Contains(text))
			{
				this.scenesLoaded.Add(text);
				SceneManager.LoadSceneAsync(text, LoadSceneMode.Additive);
			}
		}
		this.scenesToUnload.Clear();
		foreach (string text2 in this.scenesLoaded)
		{
			if (!this.scenesRequested.Contains(text2) && !this.sceneForceStayLoaded.Contains(text2))
			{
				this.scenesToUnload.Add(text2);
			}
		}
		foreach (string text3 in this.scenesToUnload)
		{
			this.scenesLoaded.Remove(text3);
			SceneManager.UnloadSceneAsync(text3);
		}
		for (int n = 0; n < this.objectActivationState.Length; n++)
		{
			if (!(this.allObjects[n] == null))
			{
				this.allObjects[n].SetActive(this.objectActivationState[n]);
			}
		}
	}

	private ZoneData GetZoneData(GTZone zone)
	{
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].zone == zone)
			{
				return this.zones[i];
			}
		}
		return null;
	}

	public ZoneManagement()
	{
	}

	public static ZoneManagement instance;

	[CompilerGenerated]
	private bool <hasInstance>k__BackingField;

	[SerializeField]
	private ZoneData[] zones;

	private GameObject[] allObjects;

	private bool[] objectActivationState;

	public Action onZoneChanged;

	private HashSet<string> scenesLoaded = new HashSet<string>();

	private HashSet<string> scenesRequested = new HashSet<string>();

	private HashSet<string> sceneForceStayLoaded = new HashSet<string>(8);

	private List<string> scenesToUnload = new List<string>();
}
