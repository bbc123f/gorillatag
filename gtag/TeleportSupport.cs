using System.Diagnostics;
using UnityEngine;

public abstract class TeleportSupport : MonoBehaviour
{
	private bool _eventsActive;

	protected LocomotionTeleport LocomotionTeleport { get; private set; }

	protected virtual void OnEnable()
	{
		LocomotionTeleport = GetComponent<LocomotionTeleport>();
		AddEventHandlers();
	}

	protected virtual void OnDisable()
	{
		RemoveEventHandlers();
		LocomotionTeleport = null;
	}

	[Conditional("DEBUG_TELEPORT_EVENT_HANDLERS")]
	private void LogEventHandler(string msg)
	{
		UnityEngine.Debug.Log("EventHandler: " + GetType().Name + ": " + msg);
	}

	protected virtual void AddEventHandlers()
	{
		_eventsActive = true;
	}

	protected virtual void RemoveEventHandlers()
	{
		_eventsActive = false;
	}
}
