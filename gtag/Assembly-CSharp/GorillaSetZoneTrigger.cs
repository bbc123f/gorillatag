using System;
using UnityEngine;

// Token: 0x02000079 RID: 121
public class GorillaSetZoneTrigger : GorillaTriggerBox
{
	// Token: 0x06000265 RID: 613 RVA: 0x0000FF7F File Offset: 0x0000E17F
	public override void OnBoxTriggered()
	{
		ZoneManagement.SetActiveZones(this.zones);
	}

	// Token: 0x0400032B RID: 811
	[SerializeField]
	private GTZone[] zones;
}
