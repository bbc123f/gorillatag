using System;
using UnityEngine;

// Token: 0x02000088 RID: 136
public class TeleportDestination : MonoBehaviour
{
	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060002EC RID: 748 RVA: 0x0001275D File Offset: 0x0001095D
	// (set) Token: 0x060002ED RID: 749 RVA: 0x00012765 File Offset: 0x00010965
	public bool IsValidDestination { get; private set; }

	// Token: 0x060002EE RID: 750 RVA: 0x0001276E File Offset: 0x0001096E
	private TeleportDestination()
	{
		this._updateTeleportDestinationAction = new Action<bool, Vector3?, Quaternion?, Quaternion?>(this.UpdateTeleportDestination);
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0001278C File Offset: 0x0001098C
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

	// Token: 0x060002F0 RID: 752 RVA: 0x000127E1 File Offset: 0x000109E1
	private void TryDisableEventHandlers()
	{
		if (!this._eventsActive)
		{
			return;
		}
		this.LocomotionTeleport.UpdateTeleportDestination -= this._updateTeleportDestinationAction;
		this._eventsActive = false;
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00012804 File Offset: 0x00010A04
	public void OnDisable()
	{
		this.TryDisableEventHandlers();
	}

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x060002F2 RID: 754 RVA: 0x0001280C File Offset: 0x00010A0C
	// (remove) Token: 0x060002F3 RID: 755 RVA: 0x00012844 File Offset: 0x00010A44
	public event Action<TeleportDestination> Deactivated;

	// Token: 0x060002F4 RID: 756 RVA: 0x00012879 File Offset: 0x00010A79
	public void OnDeactivated()
	{
		if (this.Deactivated != null)
		{
			this.Deactivated(this);
			return;
		}
		this.Recycle();
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00012896 File Offset: 0x00010A96
	public void Recycle()
	{
		this.LocomotionTeleport.RecycleTeleportDestination(this);
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x000128A4 File Offset: 0x00010AA4
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

	// Token: 0x040003AF RID: 943
	[Tooltip("If the target handler provides a target position, this transform will be moved to that position and it's game object enabled. A target position being provided does not mean the position is valid, only that the aim handler found something to test as a destination.")]
	public Transform PositionIndicator;

	// Token: 0x040003B0 RID: 944
	[Tooltip("This transform will be rotated to match the rotation of the aiming target. Simple teleport destinations should assign this to the object containing this component. More complex teleport destinations might assign this to a sub-object that is used to indicate the landing orientation independently from the rest of the destination indicator, such as when world space effects are required. This will typically be a child of the PositionIndicator.")]
	public Transform OrientationIndicator;

	// Token: 0x040003B1 RID: 945
	[Tooltip("After the player teleports, the character controller will have it's rotation set to this value. It is different from the OrientationIndicator transform.rotation in order to support both head-relative and forward-facing teleport modes (See TeleportOrientationHandlerThumbstick.cs).")]
	public Quaternion LandingRotation;

	// Token: 0x040003B2 RID: 946
	[NonSerialized]
	public LocomotionTeleport LocomotionTeleport;

	// Token: 0x040003B3 RID: 947
	[NonSerialized]
	public LocomotionTeleport.States TeleportState;

	// Token: 0x040003B4 RID: 948
	private readonly Action<bool, Vector3?, Quaternion?, Quaternion?> _updateTeleportDestinationAction;

	// Token: 0x040003B5 RID: 949
	private bool _eventsActive;
}
