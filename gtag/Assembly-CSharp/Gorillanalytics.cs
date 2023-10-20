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

// Token: 0x02000181 RID: 385
public class Gorillanalytics : MonoBehaviour
{
	// Token: 0x060009AD RID: 2477 RVA: 0x0003B187 File Offset: 0x00039387
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

	// Token: 0x060009AE RID: 2478 RVA: 0x0003B198 File Offset: 0x00039398
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

	// Token: 0x060009AF RID: 2479 RVA: 0x0003B3F4 File Offset: 0x000395F4
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

	// Token: 0x04000BDF RID: 3039
	public float interval = 60f;

	// Token: 0x04000BE0 RID: 3040
	public double oneOverChance = 4320.0;

	// Token: 0x04000BE1 RID: 3041
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04000BE2 RID: 3042
	public List<string> maps;

	// Token: 0x04000BE3 RID: 3043
	public List<string> modes;

	// Token: 0x04000BE4 RID: 3044
	public List<string> queues;

	// Token: 0x04000BE5 RID: 3045
	private readonly Gorillanalytics.UploadData uploadData = new Gorillanalytics.UploadData();

	// Token: 0x0200042C RID: 1068
	private class UploadData
	{
		// Token: 0x04001D4C RID: 7500
		public string version;

		// Token: 0x04001D4D RID: 7501
		public double upload_chance;

		// Token: 0x04001D4E RID: 7502
		public string map;

		// Token: 0x04001D4F RID: 7503
		public string mode;

		// Token: 0x04001D50 RID: 7504
		public string queue;

		// Token: 0x04001D51 RID: 7505
		public int player_count;

		// Token: 0x04001D52 RID: 7506
		public float pos_x;

		// Token: 0x04001D53 RID: 7507
		public float pos_y;

		// Token: 0x04001D54 RID: 7508
		public float pos_z;

		// Token: 0x04001D55 RID: 7509
		public float vel_x;

		// Token: 0x04001D56 RID: 7510
		public float vel_y;

		// Token: 0x04001D57 RID: 7511
		public float vel_z;

		// Token: 0x04001D58 RID: 7512
		public string cosmetics_owned;

		// Token: 0x04001D59 RID: 7513
		public string cosmetics_worn;
	}
}
