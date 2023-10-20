using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class DebugSpawnPointChanger : MonoBehaviour
{
	// Token: 0x06000808 RID: 2056 RVA: 0x00032920 File Offset: 0x00030B20
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

	// Token: 0x06000809 RID: 2057 RVA: 0x00032A14 File Offset: 0x00030C14
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

	// Token: 0x0600080A RID: 2058 RVA: 0x00032A69 File Offset: 0x00030C69
	public List<string> GetPlausibleJumpLocation()
	{
		return (from index in this.levelTriggers[this.lastLocationIndex].canJumpToIndex
		select this.levelTriggers[index].levelName).ToList<string>();
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00032A98 File Offset: 0x00030C98
	public void JumpTo(int canJumpIndex)
	{
		DebugSpawnPointChanger.GeoTriggersGroup geoTriggersGroup = this.levelTriggers[this.lastLocationIndex];
		this.ChangePoint(geoTriggersGroup.canJumpToIndex[canJumpIndex]);
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x00032AC8 File Offset: 0x00030CC8
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

	// Token: 0x02000414 RID: 1044
	[Serializable]
	private struct GeoTriggersGroup
	{
		// Token: 0x04001CF4 RID: 7412
		public string levelName;

		// Token: 0x04001CF5 RID: 7413
		public GorillaGeoHideShowTrigger enterTrigger;

		// Token: 0x04001CF6 RID: 7414
		public GorillaGeoHideShowTrigger[] leaveTrigger;

		// Token: 0x04001CF7 RID: 7415
		public int[] canJumpToIndex;
	}
}
