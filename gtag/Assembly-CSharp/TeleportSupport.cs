using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000091 RID: 145
public abstract class TeleportSupport : MonoBehaviour
{
	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000320 RID: 800 RVA: 0x0001315E File Offset: 0x0001135E
	// (set) Token: 0x06000321 RID: 801 RVA: 0x00013166 File Offset: 0x00011366
	private protected LocomotionTeleport LocomotionTeleport { protected get; private set; }

	// Token: 0x06000322 RID: 802 RVA: 0x0001316F File Offset: 0x0001136F
	protected virtual void OnEnable()
	{
		this.LocomotionTeleport = base.GetComponent<LocomotionTeleport>();
		this.AddEventHandlers();
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00013183 File Offset: 0x00011383
	protected virtual void OnDisable()
	{
		this.RemoveEventHandlers();
		this.LocomotionTeleport = null;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00013192 File Offset: 0x00011392
	[Conditional("DEBUG_TELEPORT_EVENT_HANDLERS")]
	private void LogEventHandler(string msg)
	{
		Debug.Log("EventHandler: " + base.GetType().Name + ": " + msg);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x000131B4 File Offset: 0x000113B4
	protected virtual void AddEventHandlers()
	{
		this._eventsActive = true;
	}

	// Token: 0x06000326 RID: 806 RVA: 0x000131BD File Offset: 0x000113BD
	protected virtual void RemoveEventHandlers()
	{
		this._eventsActive = false;
	}

	// Token: 0x040003DA RID: 986
	private bool _eventsActive;
}
