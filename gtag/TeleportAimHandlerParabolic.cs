using System.Collections.Generic;
using UnityEngine;

public class TeleportAimHandlerParabolic : TeleportAimHandler
{
	[Tooltip("Maximum range for aiming.")]
	public float Range;

	[Tooltip("The MinimumElevation is relative to the AimPosition.")]
	public float MinimumElevation = -100f;

	[Tooltip("The Gravity is used in conjunction with AimVelocity and the aim direction to simulate a projectile.")]
	public float Gravity = -9.8f;

	[Tooltip("The AimVelocity is the initial speed of the faked projectile.")]
	[Range(0.001f, 50f)]
	public float AimVelocity = 1f;

	[Tooltip("The AimStep is the how much to subdivide the iteration.")]
	[Range(0.001f, 1f)]
	public float AimStep = 1f;

	public override void GetPoints(List<Vector3> points)
	{
		base.LocomotionTeleport.InputHandler.GetAimData(out var aimRay);
		Vector3 origin = aimRay.origin;
		Vector3 vector = aimRay.direction * AimVelocity;
		float num = Range * Range;
		do
		{
			points.Add(origin);
			Vector3 vector2 = vector;
			vector2.y += Gravity * (1f / 90f) * AimStep;
			vector = vector2;
			origin += vector2 * AimStep;
		}
		while (origin.y - aimRay.origin.y > MinimumElevation && (aimRay.origin - origin).sqrMagnitude <= num);
	}
}
