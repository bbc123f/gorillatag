using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class TeleportAimVisualLaser : TeleportSupport
{
	// Token: 0x060002E5 RID: 741 RVA: 0x000125EF File Offset: 0x000107EF
	public TeleportAimVisualLaser()
	{
		this._enterAimStateAction = new Action(this.EnterAimState);
		this._exitAimStateAction = new Action(this.ExitAimState);
		this._updateAimDataAction = new Action<LocomotionTeleport.AimData>(this.UpdateAimData);
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0001262D File Offset: 0x0001082D
	private void EnterAimState()
	{
		this._lineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00012640 File Offset: 0x00010840
	private void ExitAimState()
	{
		this._lineRenderer.gameObject.SetActive(false);
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00012653 File Offset: 0x00010853
	private void Awake()
	{
		this.LaserPrefab.gameObject.SetActive(false);
		this._lineRenderer = Object.Instantiate<LineRenderer>(this.LaserPrefab);
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00012677 File Offset: 0x00010877
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._enterAimStateAction;
		base.LocomotionTeleport.ExitStateAim += this._exitAimStateAction;
		base.LocomotionTeleport.UpdateAimData += this._updateAimDataAction;
	}

	// Token: 0x060002EA RID: 746 RVA: 0x000126B2 File Offset: 0x000108B2
	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateAim -= this._enterAimStateAction;
		base.LocomotionTeleport.ExitStateAim -= this._exitAimStateAction;
		base.LocomotionTeleport.UpdateAimData -= this._updateAimDataAction;
		base.RemoveEventHandlers();
	}

	// Token: 0x060002EB RID: 747 RVA: 0x000126F0 File Offset: 0x000108F0
	private void UpdateAimData(LocomotionTeleport.AimData obj)
	{
		this._lineRenderer.sharedMaterial.color = (obj.TargetValid ? Color.green : Color.red);
		List<Vector3> points = obj.Points;
		this._lineRenderer.positionCount = points.Count;
		for (int i = 0; i < points.Count; i++)
		{
			this._lineRenderer.SetPosition(i, points[i]);
		}
	}

	// Token: 0x040003A8 RID: 936
	[Tooltip("This prefab will be instantiated when the aim visual is awakened, and will be set active when the user is aiming, and deactivated when they are done aiming.")]
	public LineRenderer LaserPrefab;

	// Token: 0x040003A9 RID: 937
	private readonly Action _enterAimStateAction;

	// Token: 0x040003AA RID: 938
	private readonly Action _exitAimStateAction;

	// Token: 0x040003AB RID: 939
	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	// Token: 0x040003AC RID: 940
	private LineRenderer _lineRenderer;

	// Token: 0x040003AD RID: 941
	private Vector3[] _linePoints;
}
