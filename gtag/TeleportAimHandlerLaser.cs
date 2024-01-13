using System.Collections.Generic;
using UnityEngine;

public class TeleportAimHandlerLaser : TeleportAimHandler
{
	[Tooltip("Maximum range for aiming.")]
	public float Range = 100f;

	public override void GetPoints(List<Vector3> points)
	{
		base.LocomotionTeleport.InputHandler.GetAimData(out var aimRay);
		points.Add(aimRay.origin);
		points.Add(aimRay.origin + aimRay.direction * Range);
	}
}
