using System;
using UnityEngine;

[Serializable]
public class ZoneData
{
	public GTZone zone;

	public string sceneName;

	public GameObject[] rootGameObjects;

	[NonSerialized]
	public bool active;
}
