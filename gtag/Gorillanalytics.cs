using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

public class Gorillanalytics : MonoBehaviour
{
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

	public float interval = 60f;

	public double oneOverChance = 4320.0;

	public PhotonNetworkController photonNetworkController;

	public List<string> maps;

	public List<string> modes;

	public List<string> queues;

	private readonly UploadData uploadData = new UploadData();

	private IEnumerator Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("GorillanalyticsChance", delegate(string s)
		{
			if (double.TryParse(s, out var result))
			{
				oneOverChance = result;
			}
		}, delegate
		{
		});
		while (true)
		{
			yield return new WaitForSeconds(interval);
			if ((double)UnityEngine.Random.Range(0f, 1f) < 1.0 / oneOverChance && PlayFabClientAPI.IsClientLoggedIn())
			{
				UploadGorillanalytics();
			}
		}
	}

	private void UploadGorillanalytics()
	{
		try
		{
			GetMapModeQueue(out var map, out var mode, out var queue);
			Vector3 position = Player.Instance.headCollider.transform.position;
			Vector3 currentVelocity = Player.Instance.currentVelocity;
			uploadData.version = PhotonNetworkController.Instance.GameVersionString;
			uploadData.upload_chance = oneOverChance;
			uploadData.map = map;
			uploadData.mode = mode;
			uploadData.queue = queue;
			uploadData.player_count = (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.PlayerCount : 0);
			uploadData.pos_x = position.x;
			uploadData.pos_y = position.y;
			uploadData.pos_z = position.z;
			uploadData.vel_x = currentVelocity.x;
			uploadData.vel_y = currentVelocity.y;
			uploadData.vel_z = currentVelocity.z;
			uploadData.cosmetics_owned = string.Join(";", CosmeticsController.instance.unlockedCosmetics.Select((CosmeticsController.CosmeticItem c) => c.itemName));
			uploadData.cosmetics_worn = string.Join(";", CosmeticsController.instance.currentWornSet.items.Select((CosmeticsController.CosmeticItem c) => c.itemName));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = new EntityKey
				{
					Id = PlayFabSettings.staticPlayer.EntityId,
					Type = PlayFabSettings.staticPlayer.EntityType
				},
				FunctionName = "Gorillanalytics",
				FunctionParameter = uploadData,
				GeneratePlayStreamEvent = false
			}, delegate(ExecuteFunctionResult result)
			{
				Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error uploading Gorillanalytics: " + error.GenerateErrorReport());
			});
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
		object value = null;
		PhotonNetwork.CurrentRoom?.CustomProperties.TryGetValue("gameMode", out value);
		string gameMode = value?.ToString() ?? "";
		map = maps.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
		mode = modes.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
		queue = queues.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
	}
}
