using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000091 RID: 145
public abstract class TeleportSupport : MonoBehaviour
{
	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000320 RID: 800 RVA: 0x00013382 File Offset: 0x00011582
	// (set) Token: 0x06000321 RID: 801 RVA: 0x0001338A File Offset: 0x0001158A
	private protected LocomotionTeleport LocomotionTeleport { protected get; private set; }

	// Token: 0x06000322 RID: 802 RVA: 0x00013393 File Offset: 0x00011593
	protected virtual void OnEnable()
	{
		this.LocomotionTeleport = base.GetComponent<LocomotionTeleport>();
		this.AddEventHandlers();
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000133A7 File Offset: 0x000115A7
	protected virtual void OnDisable()
	{
		this.RemoveEventHandlers();
		this.LocomotionTeleport = null;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x000133B6 File Offset: 0x000115B6
	[Conditional("DEBUG_TELEPORT_EVENT_HANDLERS")]
	private void LogEventHandler(string msg)
	{
		Debug.Log("EventHandler: " + base.GetType().Name + ": " + msg);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x000133D8 File Offset: 0x000115D8
	protected virtual void AddEventHandlers()
	{
		this._eventsActive = true;
	}

	// Token: 0x06000326 RID: 806 RVA: 0x000133E1 File Offset: 0x000115E1
	protected virtual void RemoveEventHandlers()
	{
		this._eventsActive = false;
	}

	// Token: 0x040003DA RID: 986
	private bool _eventsActive;
}
