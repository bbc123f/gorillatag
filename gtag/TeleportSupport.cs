using System;
using System.Diagnostics;
using UnityEngine;

public abstract class TeleportSupport : MonoBehaviour
{
	private protected LocomotionTeleport LocomotionTeleport { protected get; private set; }

	protected virtual void OnEnable()
	{
		this.LocomotionTeleport = base.GetComponent<LocomotionTeleport>();
		this.AddEventHandlers();
	}

	protected virtual void OnDisable()
	{
		this.RemoveEventHandlers();
		this.LocomotionTeleport = null;
	}

	[Conditional("DEBUG_TELEPORT_EVENT_HANDLERS")]
	private void LogEventHandler(string msg)
	{
		Debug.Log("EventHandler: " + base.GetType().Name + ": " + msg);
	}

	protected virtual void AddEventHandlers()
	{
		this._eventsActive = true;
	}

	protected virtual void RemoveEventHandlers()
	{
		this._eventsActive = false;
	}

	private bool _eventsActive;
}
