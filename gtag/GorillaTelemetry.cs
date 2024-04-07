using System;
using System.Collections.Generic;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public static class GorillaTelemetry
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void InitLocalRig()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		if (GorillaTelemetry.gLocalRig == null)
		{
			GorillaTelemetry.gLocalRig = GameObject.Find("Local Gorilla Player").GetComponentInChildren<VRRig>();
		}
	}

	private static bool IsConnected()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return false;
		}
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	private static string PlayFabUserId()
	{
		return GorillaTelemetry.gPlayFabAuth._playFabPlayerIdCache;
	}

	public static void PostZoneEvent(GTZone zone, GTSubZone subZone, GTZoneEventType zoneEvent)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = zoneEvent.GetName<GTZoneEventType>();
		string name2 = zone.GetName<GTZone>();
		string name3 = subZone.GetName<GTSubZone>();
		Dictionary<string, object> dictionary = GorillaTelemetry.gZoneEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["ZoneId"] = name2;
		dictionary["SubZoneId"] = name3;
		PlayFabClientAPI.WriteTitleEvent(new WriteTitleEventRequest
		{
			EventName = "telemetry_zone_event",
			Body = dictionary
		}, new Action<WriteEventResponse>(GorillaTelemetry.PostZoneEvent_OnResult), new Action<PlayFabError>(GorillaTelemetry.PostZoneEvent_OnError), null, null);
	}

	private static void PostZoneEvent_OnError(PlayFabError error)
	{
	}

	private static void PostZoneEvent_OnResult(WriteEventResponse result)
	{
	}

	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, CosmeticsController.CosmeticItem item)
	{
		GorillaTelemetry.gSingleItemParam[0] = item;
		GorillaTelemetry.PostShopEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemParam);
		GorillaTelemetry.gSingleItemParam[0] = default(CosmeticsController.CosmeticItem);
	}

	private static string[] FetchItemArgs(ICollection<CosmeticsController.CosmeticItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		foreach (CosmeticsController.CosmeticItem cosmeticItem in items)
		{
			if (!cosmeticItem.isNullItem)
			{
				string itemName = cosmeticItem.itemName;
				if (!string.IsNullOrWhiteSpace(itemName) && !itemName.Contains("NOTHING") && hashSet.Add(itemName))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, ICollection<CosmeticsController.CosmeticItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (playerRig != GorillaTelemetry.gLocalRig)
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] array = GorillaTelemetry.FetchItemArgs(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["Items"] = array;
		PlayFabClientAPI.WriteTitleEvent(new WriteTitleEventRequest
		{
			EventName = "telemetry_shop_event",
			Body = dictionary
		}, new Action<WriteEventResponse>(GorillaTelemetry.PostShopEvent_OnResult), new Action<PlayFabError>(GorillaTelemetry.PostShopEvent_OnError), null, null);
	}

	private static void PostShopEvent_OnResult(WriteEventResponse result)
	{
	}

	private static void PostShopEvent_OnError(PlayFabError error)
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static GorillaTelemetry()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["User"] = null;
		dictionary["EventType"] = null;
		dictionary["ZoneId"] = null;
		dictionary["SubZoneId"] = null;
		GorillaTelemetry.gZoneEventArgs = dictionary;
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2["User"] = null;
		dictionary2["EventType"] = null;
		dictionary2["Items"] = null;
		GorillaTelemetry.gShopEventArgs = dictionary2;
		GorillaTelemetry.gSingleItemParam = new CosmeticsController.CosmeticItem[1];
	}

	private static PlayFabAuthenticator gPlayFabAuth;

	private static readonly Dictionary<string, object> gZoneEventArgs;

	private static VRRig gLocalRig;

	private static readonly Dictionary<string, object> gShopEventArgs;

	private static CosmeticsController.CosmeticItem[] gSingleItemParam;

	public static class k
	{
		public const string User = "User";

		public const string ZoneId = "ZoneId";

		public const string SubZoneId = "SubZoneId";

		public const string EventType = "EventType";

		public const string Items = "Items";

		public const string telemetry_zone_event = "telemetry_zone_event";

		public const string telemetry_shop_event = "telemetry_shop_event";
	}
}
