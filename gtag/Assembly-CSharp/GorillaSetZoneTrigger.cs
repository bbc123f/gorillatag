using System;
using UnityEngine;

public class GorillaSetZoneTrigger : GorillaTriggerBox
{
	public override void OnBoxTriggered()
	{
		ZoneManagement.SetActiveZones(this.zones);
	}

	public GorillaSetZoneTrigger()
	{
	}

	[SerializeField]
	private GTZone[] zones;
}
