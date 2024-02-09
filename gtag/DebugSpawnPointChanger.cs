using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using UnityEngine;

public class DebugSpawnPointChanger : MonoBehaviour
{
	private void AttachSpawnPoint(VRRig rig, Transform[] spawnPts, int locationIndex)
	{
		if (spawnPts == null)
		{
			return;
		}
		Player player = Object.FindObjectOfType<Player>();
		if (player == null)
		{
			return;
		}
		this.lastLocationIndex = locationIndex;
		int i = 0;
		while (i < spawnPts.Length)
		{
			Transform transform = spawnPts[i];
			if (transform.name == this.levelTriggers[locationIndex].levelName)
			{
				rig.transform.position = transform.position;
				rig.transform.rotation = transform.rotation;
				player.transform.position = transform.position;
				player.transform.rotation = transform.rotation;
				player.InitializeValues();
				SpawnPoint component = transform.GetComponent<SpawnPoint>();
				if (component != null)
				{
					player.scale = component.startSize;
					ZoneManagement.SetActiveZone(component.startZone);
					return;
				}
				Debug.LogWarning("Attempt to spawn at transform that does not have SpawnPoint component will be ignored: " + transform.name);
				return;
			}
			else
			{
				i++;
			}
		}
	}

	private void ChangePoint(int index)
	{
		SpawnManager spawnManager = Object.FindObjectOfType<SpawnManager>();
		if (spawnManager != null)
		{
			Transform[] array = spawnManager.ChildrenXfs();
			foreach (VRRig vrrig in (VRRig[])Object.FindObjectsOfType(typeof(VRRig)))
			{
				this.AttachSpawnPoint(vrrig, array, index);
			}
		}
	}

	public List<string> GetPlausibleJumpLocation()
	{
		return this.levelTriggers[this.lastLocationIndex].canJumpToIndex.Select((int index) => this.levelTriggers[index].levelName).ToList<string>();
	}

	public void JumpTo(int canJumpIndex)
	{
		DebugSpawnPointChanger.GeoTriggersGroup geoTriggersGroup = this.levelTriggers[this.lastLocationIndex];
		this.ChangePoint(geoTriggersGroup.canJumpToIndex[canJumpIndex]);
	}

	public void SetLastLocation(string levelName)
	{
		for (int i = 0; i < this.levelTriggers.Length; i++)
		{
			if (!(this.levelTriggers[i].levelName != levelName))
			{
				this.lastLocationIndex = i;
				return;
			}
		}
	}

	[SerializeField]
	private DebugSpawnPointChanger.GeoTriggersGroup[] levelTriggers;

	private int lastLocationIndex;

	[Serializable]
	private struct GeoTriggersGroup
	{
		public string levelName;

		public GorillaGeoHideShowTrigger enterTrigger;

		public GorillaGeoHideShowTrigger[] leaveTrigger;

		public int[] canJumpToIndex;
	}
}
