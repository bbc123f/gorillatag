using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TeleportTargetHandler : TeleportSupport
{
	[Tooltip("This bitmask controls which game object layers will be included in the targeting collision tests.")]
	public LayerMask AimCollisionLayerMask;

	protected readonly LocomotionTeleport.AimData AimData = new LocomotionTeleport.AimData();

	private readonly Action _startAimAction;

	private readonly List<Vector3> _aimPoints = new List<Vector3>();

	private const float ERROR_MARGIN = 0.1f;

	protected TeleportTargetHandler()
	{
		_startAimAction = delegate
		{
			StartCoroutine(TargetAimCoroutine());
		};
	}

	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += _startAimAction;
	}

	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= _startAimAction;
	}

	private IEnumerator TargetAimCoroutine()
	{
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim)
		{
			ResetAimData();
			Vector3 start = base.LocomotionTeleport.transform.position;
			_aimPoints.Clear();
			base.LocomotionTeleport.AimHandler.GetPoints(_aimPoints);
			for (int i = 0; i < _aimPoints.Count; i++)
			{
				Vector3 end = _aimPoints[i];
				AimData.TargetValid = ConsiderTeleport(start, ref end);
				AimData.Points.Add(end);
				if (AimData.TargetValid)
				{
					AimData.Destination = ConsiderDestination(end);
					AimData.TargetValid = AimData.Destination.HasValue;
					break;
				}
				start = _aimPoints[i];
			}
			base.LocomotionTeleport.OnUpdateAimData(AimData);
			yield return null;
		}
	}

	protected virtual void ResetAimData()
	{
		AimData.Reset();
	}

	protected abstract bool ConsiderTeleport(Vector3 start, ref Vector3 end);

	public virtual Vector3? ConsiderDestination(Vector3 location)
	{
		CapsuleCollider characterController = base.LocomotionTeleport.LocomotionController.CharacterController;
		float num = characterController.radius - 0.1f;
		Vector3 vector = location;
		vector.y += num + 0.1f;
		Vector3 end = vector;
		end.y += characterController.height - 0.1f;
		if (Physics.CheckCapsule(vector, end, num, AimCollisionLayerMask, QueryTriggerInteraction.Ignore))
		{
			return null;
		}
		return location;
	}
}
