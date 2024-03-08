using System;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;

public static class GorillaTelemetry
{
	public static void PostZoneEvent(GTZone zone, GTSubZone subZone, GTZoneEventType eventType, bool debug)
	{
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		string text = zone.ToString();
		if (subZone != GTSubZone.none)
		{
			text += string.Format(".{0}", subZone);
		}
		if (debug)
		{
			return;
		}
		WriteTitleEventRequest writeTitleEventRequest = new WriteTitleEventRequest();
		writeTitleEventRequest.Body["User"] = PlayFabAuthenticator.instance._playFabPlayerIdCache;
		writeTitleEventRequest.Body["ZoneId"] = text;
		writeTitleEventRequest.Body["EventType"] = eventType.ToString();
		writeTitleEventRequest.EventName = "telemetry_zone_event";
		PlayFabClientAPI.WriteTitleEvent(writeTitleEventRequest, new Action<WriteEventResponse>(GorillaTelemetry.PostZoneEvent_OnResult), new Action<PlayFabError>(GorillaTelemetry.PostZoneEvent_OnError), null, null);
	}

	private static void PostZoneEvent_OnError(PlayFabError error)
	{
	}

	private static void PostZoneEvent_OnResult(WriteEventResponse result)
	{
	}
}
