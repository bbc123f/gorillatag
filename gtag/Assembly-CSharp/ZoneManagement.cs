using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class ZoneManagement : MonoBehaviour
{
	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000267 RID: 615 RVA: 0x0000FF94 File Offset: 0x0000E194
	// (set) Token: 0x06000268 RID: 616 RVA: 0x0000FF9C File Offset: 0x0000E19C
	public bool hasInstance { get; private set; }

	// Token: 0x06000269 RID: 617 RVA: 0x0000FFA5 File Offset: 0x0000E1A5
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

	// Token: 0x0600026A RID: 618 RVA: 0x0000FFD3 File Offset: 0x0000E1D3
	public static void SetActiveZone(GTZone zone)
	{
		ZoneManagement.SetActiveZones(new GTZone[]
		{
			zone
		});
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0000FFE4 File Offset: 0x0000E1E4
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

	// Token: 0x0600026C RID: 620 RVA: 0x0001000C File Offset: 0x0000E20C
	public static bool IsInZone(GTZone zone)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.rootGameObjects.Length != 0 && zoneData.rootGameObjects[0].activeSelf;
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00010052 File Offset: 0x0000E252
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

	// Token: 0x0600026E RID: 622 RVA: 0x0001007C File Offset: 0x0000E27C
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

	// Token: 0x0600026F RID: 623 RVA: 0x00010134 File Offset: 0x0000E334
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

	// Token: 0x06000270 RID: 624 RVA: 0x00010220 File Offset: 0x0000E420
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

	// Token: 0x04000337 RID: 823
	public static ZoneManagement instance;

	// Token: 0x04000339 RID: 825
	[SerializeField]
	private ZoneData[] zones;

	// Token: 0x0400033A RID: 826
	private GameObject[] allObjects;

	// Token: 0x0400033B RID: 827
	private bool[] objectActivationState;

	// Token: 0x0400033C RID: 828
	private GTZone activeZone;
}
