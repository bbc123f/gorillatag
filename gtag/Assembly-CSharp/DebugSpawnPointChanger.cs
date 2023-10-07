using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class DebugSpawnPointChanger : MonoBehaviour
{
	// Token: 0x06000807 RID: 2055 RVA: 0x00032AE0 File Offset: 0x00030CE0
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

	// Token: 0x06000808 RID: 2056 RVA: 0x00032BD4 File Offset: 0x00030DD4
	private void ChangePoint(int index)
	{
		SpawnManager spawnManager = Object.FindObjectOfType<SpawnManager>();
		if (spawnManager != null)
		{
			Transform[] spawnPts = spawnManager.ChildrenXfs();
			foreach (VRRig rig in (VRRig[])Object.FindObjectsOfType(typeof(VRRig)))
			{
				this.AttachSpawnPoint(rig, spawnPts, index);
			}
		}
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x00032C29 File Offset: 0x00030E29
	public List<string> GetPlausibleJumpLocation()
	{
		return (from index in this.levelTriggers[this.lastLocationIndex].canJumpToIndex
		select this.levelTriggers[index].levelName).ToList<string>();
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x00032C58 File Offset: 0x00030E58
	public void JumpTo(int canJumpIndex)
	{
		DebugSpawnPointChanger.GeoTriggersGroup geoTriggersGroup = this.levelTriggers[this.lastLocationIndex];
		this.ChangePoint(geoTriggersGroup.canJumpToIndex[canJumpIndex]);
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00032C88 File Offset: 0x00030E88
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

	// Token: 0x040009B0 RID: 2480
	[SerializeField]
	private DebugSpawnPointChanger.GeoTriggersGroup[] levelTriggers;

	// Token: 0x040009B1 RID: 2481
	private int lastLocationIndex;

	// Token: 0x02000412 RID: 1042
	[Serializable]
	private struct GeoTriggersGroup
	{
		// Token: 0x04001CE7 RID: 7399
		public string levelName;

		// Token: 0x04001CE8 RID: 7400
		public GorillaGeoHideShowTrigger enterTrigger;

		// Token: 0x04001CE9 RID: 7401
		public GorillaGeoHideShowTrigger[] leaveTrigger;

		// Token: 0x04001CEA RID: 7402
		public int[] canJumpToIndex;
	}
}
