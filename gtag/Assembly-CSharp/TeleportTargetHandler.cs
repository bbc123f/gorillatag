using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000092 RID: 146
public abstract class TeleportTargetHandler : TeleportSupport
{
	// Token: 0x06000328 RID: 808 RVA: 0x000133F2 File Offset: 0x000115F2
	protected TeleportTargetHandler()
	{
		this._startAimAction = delegate()
		{
			base.StartCoroutine(this.TargetAimCoroutine());
		};
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00013422 File Offset: 0x00011622
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0001343B File Offset: 0x0001163B
	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._startAimAction;
	}

	// Token: 0x0600032B RID: 811 RVA: 0x00013454 File Offset: 0x00011654
	private IEnumerator TargetAimCoroutine()
	{
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim)
		{
			this.ResetAimData();
			Vector3 start = base.LocomotionTeleport.transform.position;
			this._aimPoints.Clear();
			base.LocomotionTeleport.AimHandler.GetPoints(this._aimPoints);
			for (int i = 0; i < this._aimPoints.Count; i++)
			{
				Vector3 vector = this._aimPoints[i];
				this.AimData.TargetValid = this.ConsiderTeleport(start, ref vector);
				this.AimData.Points.Add(vector);
				if (this.AimData.TargetValid)
				{
					this.AimData.Destination = this.ConsiderDestination(vector);
					this.AimData.TargetValid = (this.AimData.Destination != null);
					break;
				}
				start = this._aimPoints[i];
			}
			base.LocomotionTeleport.OnUpdateAimData(this.AimData);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00013463 File Offset: 0x00011663
	protected virtual void ResetAimData()
	{
		this.AimData.Reset();
	}

	// Token: 0x0600032D RID: 813
	protected abstract bool ConsiderTeleport(Vector3 start, ref Vector3 end);

	// Token: 0x0600032E RID: 814 RVA: 0x00013470 File Offset: 0x00011670
	public virtual Vector3? ConsiderDestination(Vector3 location)
	{
		CapsuleCollider characterController = base.LocomotionTeleport.LocomotionController.CharacterController;
		float num = characterController.radius - 0.1f;
		Vector3 vector = location;
		vector.y += num + 0.1f;
		Vector3 end = vector;
		end.y += characterController.height - 0.1f;
		if (Physics.CheckCapsule(vector, end, num, this.AimCollisionLayerMask, QueryTriggerInteraction.Ignore))
		{
			return null;
		}
		return new Vector3?(location);
	}

	// Token: 0x040003DB RID: 987
	[Tooltip("This bitmask controls which game object layers will be included in the targeting collision tests.")]
	public LayerMask AimCollisionLayerMask;

	// Token: 0x040003DC RID: 988
	protected readonly LocomotionTeleport.AimData AimData = new LocomotionTeleport.AimData();

	// Token: 0x040003DD RID: 989
	private readonly Action _startAimAction;

	// Token: 0x040003DE RID: 990
	private readonly List<Vector3> _aimPoints = new List<Vector3>();

	// Token: 0x040003DF RID: 991
	private const float ERROR_MARGIN = 0.1f;
}
