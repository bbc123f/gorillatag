using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TeleportAimHandler : TeleportSupport
{
	protected override void OnEnable()
	{
		base.OnEnable();
		base.LocomotionTeleport.AimHandler = this;
	}

	protected override void OnDisable()
	{
		if (base.LocomotionTeleport.AimHandler == this)
		{
			base.LocomotionTeleport.AimHandler = null;
		}
		base.OnDisable();
	}

	public abstract void GetPoints(List<Vector3> points);

	protected TeleportAimHandler()
	{
	}
}
