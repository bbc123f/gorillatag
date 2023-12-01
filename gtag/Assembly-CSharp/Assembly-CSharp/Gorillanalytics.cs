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

public class Gorillanalytics : MonoBehaviour
{
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

	public float interval = 60f;

	public double oneOverChance = 4320.0;

	public PhotonNetworkController photonNetworkController;

	public List<string> maps;

	public List<string> modes;

	public List<string> queues;

	private readonly Gorillanalytics.UploadData uploadData = new Gorillanalytics.UploadData();

	private class UploadData
	{
		public string version;

		public double upload_chance;

		public string map;

		public string mode;

		public string queue;

		public int player_count;

		public float pos_x;

		public float pos_y;

		public float pos_z;

		public float vel_x;

		public float vel_y;

		public float vel_z;

		public string cosmetics_owned;

		public string cosmetics_worn;
	}
}
