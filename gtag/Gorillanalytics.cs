using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
			string text;
			string text2;
			string text3;
			this.GetMapModeQueue(out text, out text2, out text3);
			Vector3 position = GorillaLocomotion.Player.Instance.headCollider.transform.position;
			Vector3 currentVelocity = GorillaLocomotion.Player.Instance.currentVelocity;
			this.uploadData.version = NetworkSystemConfig.AppVersion;
			this.uploadData.upload_chance = this.oneOverChance;
			this.uploadData.map = text;
			this.uploadData.mode = text2;
			this.uploadData.queue = text3;
			this.uploadData.player_count = (int)(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.PlayerCount : 0);
			this.uploadData.pos_x = position.x;
			this.uploadData.pos_y = position.y;
			this.uploadData.pos_z = position.z;
			this.uploadData.vel_x = currentVelocity.x;
			this.uploadData.vel_y = currentVelocity.y;
			this.uploadData.vel_z = currentVelocity.z;
			this.uploadData.cosmetics_owned = string.Join(";", CosmeticsController.instance.unlockedCosmetics.Select((CosmeticsController.CosmeticItem c) => c.itemName));
			this.uploadData.cosmetics_worn = string.Join(";", CosmeticsController.instance.currentWornSet.items.Select((CosmeticsController.CosmeticItem c) => c.itemName));
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
		catch (Exception ex)
		{
			Debug.LogError(ex);
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
		map = this.maps.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
		mode = this.modes.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
		queue = this.queues.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
	}

	public Gorillanalytics()
	{
	}

	[CompilerGenerated]
	private void <Start>b__8_0(string s)
	{
		double num;
		if (double.TryParse(s, out num))
		{
			this.oneOverChance = num;
		}
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
		public UploadData()
		{
		}

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

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal void <Start>b__8_1(PlayFabError e)
		{
		}

		internal string <UploadGorillanalytics>b__9_0(CosmeticsController.CosmeticItem c)
		{
			return c.itemName;
		}

		internal string <UploadGorillanalytics>b__9_1(CosmeticsController.CosmeticItem c)
		{
			return c.itemName;
		}

		internal void <UploadGorillanalytics>b__9_2(ExecuteFunctionResult result)
		{
			Debug.Log(string.Format("The {0} function took {1} to complete", result.FunctionName, result.ExecutionTimeMilliseconds));
		}

		internal void <UploadGorillanalytics>b__9_3(PlayFabError error)
		{
			Debug.Log("Error uploading Gorillanalytics: " + error.GenerateErrorReport());
		}

		public static readonly Gorillanalytics.<>c <>9 = new Gorillanalytics.<>c();

		public static Action<PlayFabError> <>9__8_1;

		public static Func<CosmeticsController.CosmeticItem, string> <>9__9_0;

		public static Func<CosmeticsController.CosmeticItem, string> <>9__9_1;

		public static Action<ExecuteFunctionResult> <>9__9_2;

		public static Action<PlayFabError> <>9__9_3;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass10_0
	{
		public <>c__DisplayClass10_0()
		{
		}

		internal bool <GetMapModeQueue>b__0(string s)
		{
			return this.gameMode.Contains(s);
		}

		internal bool <GetMapModeQueue>b__1(string s)
		{
			return this.gameMode.Contains(s);
		}

		internal bool <GetMapModeQueue>b__2(string s)
		{
			return this.gameMode.Contains(s);
		}

		public string gameMode;
	}

	[CompilerGenerated]
	private sealed class <Start>d__8 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <Start>d__8(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			Gorillanalytics gorillanalytics = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				if ((double)Random.Range(0f, 1f) < 1.0 / gorillanalytics.oneOverChance && PlayFabClientAPI.IsClientLoggedIn())
				{
					gorillanalytics.UploadGorillanalytics();
				}
			}
			else
			{
				this.<>1__state = -1;
				PlayFabTitleDataCache.Instance.GetTitleData("GorillanalyticsChance", delegate(string s)
				{
					double num2;
					if (double.TryParse(s, out num2))
					{
						gorillanalytics.oneOverChance = num2;
					}
				}, delegate(PlayFabError e)
				{
				});
			}
			this.<>2__current = new WaitForSeconds(gorillanalytics.interval);
			this.<>1__state = 1;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public Gorillanalytics <>4__this;
	}
}
