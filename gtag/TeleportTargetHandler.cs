using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TeleportTargetHandler : TeleportSupport
{
	protected TeleportTargetHandler()
	{
		this._startAimAction = delegate
		{
			base.StartCoroutine(this.TargetAimCoroutine());
		};
	}

	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._startAimAction;
	}

	private IEnumerator TargetAimCoroutine()
	{
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim)
		{
			this.ResetAimData();
			Vector3 vector = base.LocomotionTeleport.transform.position;
			this._aimPoints.Clear();
			base.LocomotionTeleport.AimHandler.GetPoints(this._aimPoints);
			for (int i = 0; i < this._aimPoints.Count; i++)
			{
				Vector3 vector2 = this._aimPoints[i];
				this.AimData.TargetValid = this.ConsiderTeleport(vector, ref vector2);
				this.AimData.Points.Add(vector2);
				if (this.AimData.TargetValid)
				{
					this.AimData.Destination = this.ConsiderDestination(vector2);
					this.AimData.TargetValid = this.AimData.Destination != null;
					break;
				}
				vector = this._aimPoints[i];
			}
			base.LocomotionTeleport.OnUpdateAimData(this.AimData);
			yield return null;
		}
		yield break;
	}

	protected virtual void ResetAimData()
	{
		this.AimData.Reset();
	}

	protected abstract bool ConsiderTeleport(Vector3 start, ref Vector3 end);

	public virtual Vector3? ConsiderDestination(Vector3 location)
	{
		CapsuleCollider characterController = base.LocomotionTeleport.LocomotionController.CharacterController;
		float num = characterController.radius - 0.1f;
		Vector3 vector = location;
		vector.y += num + 0.1f;
		Vector3 vector2 = vector;
		vector2.y += characterController.height - 0.1f;
		if (Physics.CheckCapsule(vector, vector2, num, this.AimCollisionLayerMask, QueryTriggerInteraction.Ignore))
		{
			return null;
		}
		return new Vector3?(location);
	}

	[Tooltip("This bitmask controls which game object layers will be included in the targeting collision tests.")]
	public LayerMask AimCollisionLayerMask;

	protected readonly LocomotionTeleport.AimData AimData = new LocomotionTeleport.AimData();

	private readonly Action _startAimAction;

	private readonly List<Vector3> _aimPoints = new List<Vector3>();

	private const float ERROR_MARGIN = 0.1f;
}
