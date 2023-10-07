using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000036 RID: 54
public class ScheduledTimelinePlayer : MonoBehaviour
{
	// Token: 0x06000136 RID: 310 RVA: 0x0000AAF7 File Offset: 0x00008CF7
	protected void OnEnable()
	{
		this.scheduledEventID = BetterDayNightManager.RegisterScheduledEvent(this.eventHour, new Action(this.HandleScheduledEvent));
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000AB16 File Offset: 0x00008D16
	protected void OnDisable()
	{
		BetterDayNightManager.UnregisterScheduledEvent(this.scheduledEventID);
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000AB23 File Offset: 0x00008D23
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
