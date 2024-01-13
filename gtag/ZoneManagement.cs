using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneManagement : MonoBehaviour
{
	private static ZoneManagement instance;

	[SerializeField]
	private ZoneData[] zones;

	private GameObject[] allObjects;

	private bool[] objectActivationState;

	private void Awake()
	{
		if (instance == null)
		{
			Initialize();
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public static void SetActiveZone(GTZone zone)
	{
		SetActiveZones(new GTZone[1] { zone });
	}

	public static void SetActiveZones(GTZone[] zones)
	{
		if (instance == null)
		{
			FindInstance();
		}
		if (zones != null && zones.Length != 0)
		{
			instance.SetZones(zones);
		}
	}

	private static void FindInstance()
	{
		ZoneManagement zoneManagement = UnityEngine.Object.FindObjectOfType<ZoneManagement>();
		if (zoneManagement == null)
		{
			throw new NullReferenceException("Unable to find ZoneManagement object in scene.");
		}
		Debug.LogWarning("ZoneManagement accessed before MonoBehaviour awake function called; consider delaying zone management functions to avoid FindObject lookup.");
		zoneManagement.Initialize();
	}

	private void Initialize()
	{
		instance = this;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		for (int i = 0; i < zones.Length; i++)
		{
			ZoneData zoneData = zones[i];
			if (zoneData != null && zoneData.rootGameObjects != null)
			{
				hashSet.UnionWith(zoneData.rootGameObjects);
			}
		}
		hashSet.Remove(null);
		allObjects = hashSet.ToArray();
		objectActivationState = new bool[allObjects.Length];
	}

	private void SetZones(GTZone[] zones)
	{
		for (int i = 0; i < objectActivationState.Length; i++)
		{
			objectActivationState[i] = false;
		}
		for (int j = 0; j < zones.Length; j++)
		{
			ZoneData zoneData = GetZoneData(zones[j]);
			if (zoneData == null || zoneData.rootGameObjects == null)
			{
				continue;
			}
			GameObject[] rootGameObjects = zoneData.rootGameObjects;
			foreach (GameObject gameObject in rootGameObjects)
			{
				for (int l = 0; l < allObjects.Length; l++)
				{
					if (gameObject == allObjects[l])
					{
						objectActivationState[l] = true;
						break;
					}
				}
			}
		}
		for (int m = 0; m < objectActivationState.Length; m++)
		{
			allObjects[m].SetActive(objectActivationState[m]);
		}
	}

	private ZoneData GetZoneData(GTZone zone)
	{
		for (int i = 0; i < zones.Length; i++)
		{
			if (zones[i].zone == zone)
			{
				return zones[i];
			}
		}
		return null;
	}
}
