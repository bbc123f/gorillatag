using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAimVisualLaser : TeleportSupport
{
	[Tooltip("This prefab will be instantiated when the aim visual is awakened, and will be set active when the user is aiming, and deactivated when they are done aiming.")]
	public LineRenderer LaserPrefab;

	private readonly Action _enterAimStateAction;

	private readonly Action _exitAimStateAction;

	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	private LineRenderer _lineRenderer;

	private Vector3[] _linePoints;

	public TeleportAimVisualLaser()
	{
		_enterAimStateAction = EnterAimState;
		_exitAimStateAction = ExitAimState;
		_updateAimDataAction = UpdateAimData;
	}

	private void EnterAimState()
	{
		_lineRenderer.gameObject.SetActive(value: true);
	}

	private void ExitAimState()
	{
		_lineRenderer.gameObject.SetActive(value: false);
	}

	private void Awake()
	{
		LaserPrefab.gameObject.SetActive(value: false);
		_lineRenderer = UnityEngine.Object.Instantiate(LaserPrefab);
	}

	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += _enterAimStateAction;
		base.LocomotionTeleport.ExitStateAim += _exitAimStateAction;
		base.LocomotionTeleport.UpdateAimData += _updateAimDataAction;
	}

	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateAim -= _enterAimStateAction;
		base.LocomotionTeleport.ExitStateAim -= _exitAimStateAction;
		base.LocomotionTeleport.UpdateAimData -= _updateAimDataAction;
		base.RemoveEventHandlers();
	}

	private void UpdateAimData(LocomotionTeleport.AimData obj)
	{
		_lineRenderer.sharedMaterial.color = (obj.TargetValid ? Color.green : Color.red);
		List<Vector3> points = obj.Points;
		_lineRenderer.positionCount = points.Count;
		for (int i = 0; i < points.Count; i++)
		{
			_lineRenderer.SetPosition(i, points[i]);
		}
	}
}
