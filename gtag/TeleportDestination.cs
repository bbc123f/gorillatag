using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class TeleportDestination : MonoBehaviour
{
	public bool IsValidDestination
	{
		[CompilerGenerated]
		get
		{
			return this.<IsValidDestination>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<IsValidDestination>k__BackingField = value;
		}
	}

	private TeleportDestination()
	{
		this._updateTeleportDestinationAction = new Action<bool, Vector3?, Quaternion?, Quaternion?>(this.UpdateTeleportDestination);
	}

	public void OnEnable()
	{
		this.PositionIndicator.gameObject.SetActive(false);
		if (this.OrientationIndicator != null)
		{
			this.OrientationIndicator.gameObject.SetActive(false);
		}
		this.LocomotionTeleport.UpdateTeleportDestination += this._updateTeleportDestinationAction;
		this._eventsActive = true;
	}

	private void TryDisableEventHandlers()
	{
		if (!this._eventsActive)
		{
			return;
		}
		this.LocomotionTeleport.UpdateTeleportDestination -= this._updateTeleportDestinationAction;
		this._eventsActive = false;
	}

	public void OnDisable()
	{
		this.TryDisableEventHandlers();
	}

	public event Action<TeleportDestination> Deactivated
	{
		[CompilerGenerated]
		add
		{
			Action<TeleportDestination> action = this.Deactivated;
			Action<TeleportDestination> action2;
			do
			{
				action2 = action;
				Action<TeleportDestination> action3 = (Action<TeleportDestination>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<TeleportDestination>>(ref this.Deactivated, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<TeleportDestination> action = this.Deactivated;
			Action<TeleportDestination> action2;
			do
			{
				action2 = action;
				Action<TeleportDestination> action3 = (Action<TeleportDestination>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<TeleportDestination>>(ref this.Deactivated, action3, action2);
			}
			while (action != action2);
		}
	}

	public void OnDeactivated()
	{
		if (this.Deactivated != null)
		{
			this.Deactivated(this);
			return;
		}
		this.Recycle();
	}

	public void Recycle()
	{
		this.LocomotionTeleport.RecycleTeleportDestination(this);
	}

	public virtual void UpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		this.IsValidDestination = isValidDestination;
		this.LandingRotation = landingRotation.GetValueOrDefault();
		GameObject gameObject = this.PositionIndicator.gameObject;
		bool activeInHierarchy = gameObject.activeInHierarchy;
		if (position == null)
		{
			if (activeInHierarchy)
			{
				gameObject.SetActive(false);
			}
			return;
		}
		if (!activeInHierarchy)
		{
			gameObject.SetActive(true);
		}
		base.transform.position = position.GetValueOrDefault();
		if (this.OrientationIndicator == null)
		{
			if (rotation != null)
			{
				base.transform.rotation = rotation.GetValueOrDefault();
			}
			return;
		}
		GameObject gameObject2 = this.OrientationIndicator.gameObject;
		bool activeInHierarchy2 = gameObject2.activeInHierarchy;
		if (rotation == null)
		{
			if (activeInHierarchy2)
			{
				gameObject2.SetActive(false);
			}
			return;
		}
		this.OrientationIndicator.rotation = rotation.GetValueOrDefault();
		if (!activeInHierarchy2)
		{
			gameObject2.SetActive(true);
		}
	}

	[CompilerGenerated]
	private bool <IsValidDestination>k__BackingField;

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

	[CompilerGenerated]
	private Action<TeleportDestination> Deactivated;
}
