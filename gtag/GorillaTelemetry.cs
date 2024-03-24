using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;

public static class GorillaTelemetry
{
	public static void PostZoneEvent(GTZone zone, GTSubZone subZone, GTZoneEventType eventType)
	{
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			return;
		}
		string playFabPlayerIdCache = GorillaTelemetry.gPlayFabAuth._playFabPlayerIdCache;
		string name = EnumHelper<GTZone>.GetName(zone);
		string name2 = EnumHelper<GTSubZone>.GetName(subZone);
		string name3 = EnumHelper<GTZoneEventType>.GetName(eventType);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["User"] = playFabPlayerIdCache;
		dictionary["ZoneId"] = name;
		dictionary["SubZoneId"] = name2;
		dictionary["EventType"] = name3;
		Dictionary<string, object> dictionary2 = dictionary;
		PlayFabClientAPI.WriteTitleEvent(new WriteTitleEventRequest
		{
			Body = dictionary2,
			EventName = "telemetry_zone_event"
		}, new Action<WriteEventResponse>(GorillaTelemetry.PostZoneEvent_OnResult), new Action<PlayFabError>(GorillaTelemetry.PostZoneEvent_OnError), null, null);
	}

	private static void PostZoneEvent_OnError(PlayFabError error)
	{
	}

	private static void PostZoneEvent_OnResult(WriteEventResponse result)
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static GorillaTelemetry()
	{
	}

	private static PlayFabAuthenticator gPlayFabAuth;

	private const float EVENT_COOLDOWN = 0.5f;

	private static TimeSince gSinceLastZoneEvent = 0;

	public static class K
	{
		public const string User = "User";

		public const string ZoneId = "ZoneId";

		public const string SubZoneId = "SubZoneId";

		public const string EventType = "EventType";

		public const string telemetry_zone_event = "telemetry_zone_event";

		public const string telemetry_shop_event = "telemetry_shop_event";
	}
}
