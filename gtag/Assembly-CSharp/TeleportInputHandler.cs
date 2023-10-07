using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000089 RID: 137
public abstract class TeleportInputHandler : TeleportSupport
{
	// Token: 0x060002F7 RID: 759 RVA: 0x00012975 File Offset: 0x00010B75
	protected TeleportInputHandler()
	{
		this._startReadyAction = delegate()
		{
			base.StartCoroutine(this.TeleportReadyCoroutine());
		};
		this._startAimAction = delegate()
		{
			base.StartCoroutine(this.TeleportAimCoroutine());
		};
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x000129A1 File Offset: 0x00010BA1
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.InputHandler = this;
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateReady += this._startReadyAction;
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x000129D8 File Offset: 0x00010BD8
	protected override void RemoveEventHandlers()
	{
		if (base.LocomotionTeleport.InputHandler == this)
		{
			base.LocomotionTeleport.InputHandler = null;
		}
		base.LocomotionTeleport.EnterStateReady -= this._startReadyAction;
		base.LocomotionTeleport.EnterStateAim -= this._startAimAction;
		base.RemoveEventHandlers();
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00012A2C File Offset: 0x00010C2C
	private IEnumerator TeleportReadyCoroutine()
	{
		while (this.GetIntention() != LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		base.LocomotionTeleport.CurrentIntention = LocomotionTeleport.TeleportIntentions.Aim;
		yield break;
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00012A3B File Offset: 0x00010C3B
	private IEnumerator TeleportAimCoroutine()
	{
		LocomotionTeleport.TeleportIntentions intention = this.GetIntention();
		while (intention == LocomotionTeleport.TeleportIntentions.Aim || intention == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			base.LocomotionTeleport.CurrentIntention = intention;
			yield return null;
			intention = this.GetIntention();
		}
		base.LocomotionTeleport.CurrentIntention = intention;
		yield break;
	}

	// Token: 0x060002FC RID: 764
	public abstract LocomotionTeleport.TeleportIntentions GetIntention();

	// Token: 0x060002FD RID: 765
	public abstract void GetAimData(out Ray aimRay);

	// Token: 0x040003B7 RID: 951
	private readonly Action _startReadyAction;

	// Token: 0x040003B8 RID: 952
	private readonly Action _startAimAction;
}
