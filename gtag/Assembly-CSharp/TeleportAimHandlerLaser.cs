using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAimHandlerLaser : TeleportAimHandler
{
	public override void GetPoints(List<Vector3> points)
	{
		Ray ray;
		base.LocomotionTeleport.InputHandler.GetAimData(out ray);
		points.Add(ray.origin);
		points.Add(ray.origin + ray.direction * this.Range);
	}

	[Tooltip("Maximum range for aiming.")]
	public float Range = 100f;
}
