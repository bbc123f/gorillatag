using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class Gorillanalytics : MonoBehaviour
{
	// Token: 0x060009A9 RID: 2473 RVA: 0x0003B1CF File Offset: 0x000393CF
	private IEnumerator Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("GorillanalyticsChance", delegate(string s)
		{
			double num;
			if (double.TryParse(s, out num))
			{
				this.oneOverChance = num;
			}
		}, delegate(PlayFabError e)
		{
		});
		for (;;)
		{
			yield return new WaitForSeconds(this.interval);
			if ((double)Random.Range(0f, 1f) < 1.0 / this.oneOverChance && PlayFabClientAPI.IsClientLoggedIn())
			{
				this.UploadGorillanalytics();
			}
		}
		yield break;
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0003B1E0 File Offset: 0x000393E0
	private void UploadGorillanalytics()
	{
		try
		{
			string map;
			string mode;
			string queue;
			this.GetMapModeQueue(out map, out mode, out queue);
			Vector3 position = GorillaLocomotion.Player.Instance.headCollider.transform.position;
			Vector3 currentVelocity = GorillaLocomotion.Player.Instance.currentVelocity;
			this.uploadData.version = PhotonNetworkController.Instance.GameVersionString;
			this.uploadData.upload_chance = this.oneOverChance;
			this.uploadData.map = map;
			this.uploadData.mode = mode;
			this.uploadData.queue = queue;
			this.uploadData.player_count = (int)(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.PlayerCount : 0);
			this.uploadData.pos_x = position.x;
			this.uploadData.pos_y = position.y;
			this.uploadData.pos_z = position.z;
			this.uploadData.vel_x = currentVelocity.x;
			this.uploadData.vel_y = currentVelocity.y;
			this.uploadData.vel_z = currentVelocity.z;
			this.uploadData.cosmetics_owned = string.Join(";", from c in CosmeticsController.instance.unlockedCosmetics
			select c.itemName);
			this.uploadData.cosmetics_worn = string.Join(";", from c in CosmeticsController.instance.currentWornSet.items
			select c.itemName);
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = new EntityKey
			{
				Id = PlayFabSettings.staticPlayer.EntityId,
				Type = PlayFabSettings.staticPlayer.EntityType
			};
			executeFunctionRequest.FunctionName = "Gorillanalytics";
			executeFunctionRequest.FunctionParameter = this.uploadData;
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				Debug.Log(string.Format("The {0} function took {1} to complete", result.FunctionName, result.ExecutionTimeMilliseconds));
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error uploading Gorillanalytics: " + error.GenerateErrorReport());
			}, null, null);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0003B43C File Offset: 0x0003963C
	private void GetMapModeQueue(out string map, out string mode, out string queue)
	{
		if (!PhotonNetwork.InRoom)
		{
			map = "none";
			mode = "none";
			queue = "none";
			return;
		}
		object obj = null;
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null)
		{
			currentRoom.CustomProperties.TryGetValue("gameMode", out obj);
		}
		string gameMode = ((obj != null) ? obj.ToString() : null) ?? "";
		map = (this.maps.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown");
		mode = (this.modes.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown");
		queue = (this.queues.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown");
	}

	// Token: 0x04000BDB RID: 3035
	public float interval = 60f;

	// Token: 0x04000BDC RID: 3036
	public double oneOverChance = 4320.0;

	// Token: 0x04000BDD RID: 3037
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04000BDE RID: 3038
	public List<string> maps;

	// Token: 0x04000BDF RID: 3039
	public List<string> modes;

	// Token: 0x04000BE0 RID: 3040
	public List<string> queues;

	// Token: 0x04000BE1 RID: 3041
	private readonly Gorillanalytics.UploadData uploadData = new Gorillanalytics.UploadData();

	// Token: 0x0200042A RID: 1066
	private class UploadData
	{
		// Token: 0x04001D3F RID: 7487
		public string version;

		// Token: 0x04001D40 RID: 7488
		public double upload_chance;

		// Token: 0x04001D41 RID: 7489
		public string map;

		// Token: 0x04001D42 RID: 7490
		public string mode;

		// Token: 0x04001D43 RID: 7491
		public string queue;

		// Token: 0x04001D44 RID: 7492
		public int player_count;

		// Token: 0x04001D45 RID: 7493
		public float pos_x;

		// Token: 0x04001D46 RID: 7494
		public float pos_y;

		// Token: 0x04001D47 RID: 7495
		public float pos_z;

		// Token: 0x04001D48 RID: 7496
		public float vel_x;

		// Token: 0x04001D49 RID: 7497
		public float vel_y;

		// Token: 0x04001D4A RID: 7498
		public float vel_z;

		// Token: 0x04001D4B RID: 7499
		public string cosmetics_owned;

		// Token: 0x04001D4C RID: 7500
		public string cosmetics_worn;
	}
}
