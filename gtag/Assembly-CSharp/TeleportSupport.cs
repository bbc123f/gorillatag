using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class TeleportSupport : MonoBehaviour
{
	private protected LocomotionTeleport LocomotionTeleport
	{
		[CompilerGenerated]
		protected get
		{
			return this.<LocomotionTeleport>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<LocomotionTeleport>k__BackingField = value;
		}
	}

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

	protected TeleportSupport()
	{
	}

	[CompilerGenerated]
	private LocomotionTeleport <LocomotionTeleport>k__BackingField;

	private bool _eventsActive;
}
