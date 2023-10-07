using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000084 RID: 132
public abstract class TeleportAimHandler : TeleportSupport
{
	// Token: 0x060002DD RID: 733 RVA: 0x00012454 File Offset: 0x00010654
	protected override void OnEnable()
	{
		base.OnEnable();
		base.LocomotionTeleport.AimHandler = this;
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00012468 File Offset: 0x00010668
	protected override void OnDisable()
	{
		if (base.LocomotionTeleport.AimHandler == this)
		{
			base.LocomotionTeleport.AimHandler = null;
		}
		base.OnDisable();
	}

	// Token: 0x060002DF RID: 735
	public abstract void GetPoints(List<Vector3> points);
}
