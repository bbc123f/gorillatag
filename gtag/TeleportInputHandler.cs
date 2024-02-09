using System;
using System.Collections;
using UnityEngine;

public abstract class TeleportInputHandler : TeleportSupport
{
	protected TeleportInputHandler()
	{
		this._startReadyAction = delegate
		{
			base.StartCoroutine(this.TeleportReadyCoroutine());
		};
		this._startAimAction = delegate
		{
			base.StartCoroutine(this.TeleportAimCoroutine());
		};
	}

	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.InputHandler = this;
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateReady += this._startReadyAction;
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

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

	private IEnumerator TeleportReadyCoroutine()
	{
		while (this.GetIntention() != LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		base.LocomotionTeleport.CurrentIntention = LocomotionTeleport.TeleportIntentions.Aim;
		yield break;
	}

	private IEnumerator TeleportAimCoroutine()
	{
		LocomotionTeleport.TeleportIntentions teleportIntentions = this.GetIntention();
		while (teleportIntentions == LocomotionTeleport.TeleportIntentions.Aim || teleportIntentions == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			base.LocomotionTeleport.CurrentIntention = teleportIntentions;
			yield return null;
			teleportIntentions = this.GetIntention();
		}
		base.LocomotionTeleport.CurrentIntention = teleportIntentions;
		yield break;
	}

	public abstract LocomotionTeleport.TeleportIntentions GetIntention();

	public abstract void GetAimData(out Ray aimRay);

	private readonly Action _startReadyAction;

	private readonly Action _startAimAction;
}
