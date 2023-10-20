using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class TeleportAimHandlerLaser : TeleportAimHandler
{
	// Token: 0x060002E1 RID: 737 RVA: 0x00012274 File Offset: 0x00010474
	public override void GetPoints(List<Vector3> points)
	{
		Ray ray;
		base.LocomotionTeleport.InputHandler.GetAimData(out ray);
		points.Add(ray.origin);
		points.Add(ray.origin + ray.direction * this.Range);
	}

	// Token: 0x040003A2 RID: 930
	[Tooltip("Maximum range for aiming.")]
	public float Range = 100f;
}
