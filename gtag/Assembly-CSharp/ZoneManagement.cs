using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneManagement : MonoBehaviour
{
	public bool hasInstance { get; private set; }

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
		ZoneManagement.SetActiveZones(new GTZone[]
		{
			zone
		});
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
	}

	public static bool IsInZone(GTZone zone)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.rootGameObjects.Length != 0 && zoneData.rootGameObjects[0].activeSelf;
	}

	public GameObject GetPrimaryGameObject(GTZone zone)
	{
		return this.GetZoneData(zone).rootGameObjects[0];
	}

	private static void FindInstance()
	{
		ZoneManagement zoneManagement = Object.FindObjectOfType<ZoneManagement>();
		if (zoneManagement == null)
		{
			throw new NullReferenceException("Unable to find ZoneManagement object in scene.");
		}
		Debug.LogWarning("ZoneManagement accessed before MonoBehaviour awake function called; consider delaying zone management functions to avoid FindObject lookup.");
		zoneManagement.Initialize();
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

	private void SetZones(GTZone[] zones)
	{
		for (int i = 0; i < this.objectActivationState.Length; i++)
		{
			this.objectActivationState[i] = false;
		}
		for (int j = 0; j < zones.Length; j++)
		{
			ZoneData zoneData = this.GetZoneData(zones[j]);
			if (zoneData != null && zoneData.rootGameObjects != null)
			{
				foreach (GameObject x in zoneData.rootGameObjects)
				{
					if (!(x == null))
					{
						for (int l = 0; l < this.allObjects.Length; l++)
						{
							if (x == this.allObjects[l])
							{
								this.objectActivationState[l] = true;
								break;
							}
						}
					}
				}
			}
		}
		for (int m = 0; m < this.objectActivationState.Length; m++)
		{
			if (!(this.allObjects[m] == null))
			{
				this.allObjects[m].SetActive(this.objectActivationState[m]);
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

	public static ZoneManagement instance;

	[SerializeField]
	private ZoneData[] zones;

	private GameObject[] allObjects;

	private bool[] objectActivationState;

	private GTZone activeZone;
}
