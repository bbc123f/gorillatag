using System;
using UnityEngine;

public class TeleportDestination : MonoBehaviour
{
	[Tooltip("If the target handler provides a target position, this transform will be moved to that position and it's game object enabled. A target position being provided does not mean the position is valid, only that the aim handler found something to test as a destination.")]
	public Transform PositionIndicator;

	[Tooltip("This transform will be rotated to match the rotation of the aiming target. Simple teleport destinations should assign this to the object containing this component. More complex teleport destinations might assign this to a sub-object that is used to indicate the landing orientation independently from the rest of the destination indicator, such as when world space effects are required. This will typically be a child of the PositionIndicator.")]
	public Transform OrientationIndicator;

	[Tooltip("After the player teleports, the character controller will have it's rotation set to this value. It is different from the OrientationIndicator transform.rotation in order to support both head-relative and forward-facing teleport modes (See TeleportOrientationHandlerThumbstick.cs).")]
	public Quaternion LandingRotation;

	[NonSerialized]
	public LocomotionTeleport LocomotionTeleport;

	[NonSerialized]
	public LocomotionTeleport.States TeleportState;

	private readonly Action<bool, Vector3?, Quaternion?, Quaternion?> _updateTeleportDestinationAction;

	private bool _eventsActive;

	public bool IsValidDestination { get; private set; }

	public event Action<TeleportDestination> Deactivated;

	private TeleportDestination()
	{
		_updateTeleportDestinationAction = UpdateTeleportDestination;
	}

	public void OnEnable()
	{
		PositionIndicator.gameObject.SetActive(value: false);
		if (OrientationIndicator != null)
		{
			OrientationIndicator.gameObject.SetActive(value: false);
		}
		LocomotionTeleport.UpdateTeleportDestination += _updateTeleportDestinationAction;
		_eventsActive = true;
	}

	private void TryDisableEventHandlers()
	{
		if (_eventsActive)
		{
			LocomotionTeleport.UpdateTeleportDestination -= _updateTeleportDestinationAction;
			_eventsActive = false;
		}
	}

	public void OnDisable()
	{
		TryDisableEventHandlers();
	}

	public void OnDeactivated()
	{
		if (this.Deactivated != null)
		{
			this.Deactivated(this);
		}
		else
		{
			Recycle();
		}
	}

	public void Recycle()
	{
		LocomotionTeleport.RecycleTeleportDestination(this);
	}

	public virtual void UpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		IsValidDestination = isValidDestination;
		LandingRotation = landingRotation.GetValueOrDefault();
		GameObject gameObject = PositionIndicator.gameObject;
		bool activeInHierarchy = gameObject.activeInHierarchy;
		if (!position.HasValue)
		{
			if (activeInHierarchy)
			{
				gameObject.SetActive(value: false);
			}
			return;
		}
		if (!activeInHierarchy)
		{
			gameObject.SetActive(value: true);
		}
		base.transform.position = position.GetValueOrDefault();
		if (OrientationIndicator == null)
		{
			if (rotation.HasValue)
			{
				base.transform.rotation = rotation.GetValueOrDefault();
			}
			return;
		}
		GameObject gameObject2 = OrientationIndicator.gameObject;
		bool activeInHierarchy2 = gameObject2.activeInHierarchy;
		if (!rotation.HasValue)
		{
			if (activeInHierarchy2)
			{
				gameObject2.SetActive(value: false);
			}
			return;
		}
		OrientationIndicator.rotation = rotation.GetValueOrDefault();
		if (!activeInHierarchy2)
		{
			gameObject2.SetActive(value: true);
		}
	}
}
