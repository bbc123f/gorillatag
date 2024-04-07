using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ZoneDef : MonoBehaviour
{
	public ZoneDef()
	{
	}

	public GTZone zoneId;

	[FormerlySerializedAs("subZoneType")]
	[FormerlySerializedAs("subZone")]
	public GTSubZone subZoneId;

	[Space]
	public bool trackEnter = true;

	public bool trackExit;

	public bool trackStay = true;

	[Space]
	public BoxCollider[] colliders = new BoxCollider[0];

	[Space]
	public ZoneNode[] nodes = new ZoneNode[0];

	[Space]
	public Bounds bounds;

	[Space]
	public ZoneDef[] zoneOverlaps = new ZoneDef[0];
}
