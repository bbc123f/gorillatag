using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000036 RID: 54
public class ScheduledTimelinePlayer : MonoBehaviour
{
	// Token: 0x06000136 RID: 310 RVA: 0x0000AB3F File Offset: 0x00008D3F
	protected void OnEnable()
	{
		this.scheduledEventID = BetterDayNightManager.RegisterScheduledEvent(this.eventHour, new Action(this.HandleScheduledEvent));
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000AB5E File Offset: 0x00008D5E
	protected void OnDisable()
	{
		BetterDayNightManager.UnregisterScheduledEvent(this.scheduledEventID);
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000AB6B File Offset: 0x00008D6B
	private void HandleScheduledEvent()
	{
		this.timeline.Play();
	}

	// Token: 0x040001AA RID: 426
	public PlayableDirector timeline;

	// Token: 0x040001AB RID: 427
	public int eventHour = 7;

	// Token: 0x040001AC RID: 428
	private int scheduledEventID;
}
