using System;
using System.Collections;
using UnityEngine;

public abstract class TeleportInputHandler : TeleportSupport
{
	private readonly Action _startReadyAction;

	private readonly Action _startAimAction;

	protected TeleportInputHandler()
	{
		_startReadyAction = delegate
		{
			StartCoroutine(TeleportReadyCoroutine());
		};
		_startAimAction = delegate
		{
			StartCoroutine(TeleportAimCoroutine());
		};
	}

	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.InputHandler = this;
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateReady += _startReadyAction;
		base.LocomotionTeleport.EnterStateAim += _startAimAction;
	}

	protected override void RemoveEventHandlers()
	{
		if (base.LocomotionTeleport.InputHandler == this)
		{
			base.LocomotionTeleport.InputHandler = null;
		}
		base.LocomotionTeleport.EnterStateReady -= _startReadyAction;
		base.LocomotionTeleport.EnterStateAim -= _startAimAction;
		base.RemoveEventHandlers();
	}

	private IEnumerator TeleportReadyCoroutine()
	{
		while (GetIntention() != LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		base.LocomotionTeleport.CurrentIntention = LocomotionTeleport.TeleportIntentions.Aim;
	}

	private IEnumerator TeleportAimCoroutine()
	{
		LocomotionTeleport.TeleportIntentions intention = GetIntention();
		while (intention == LocomotionTeleport.TeleportIntentions.Aim || intention == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			base.LocomotionTeleport.CurrentIntention = intention;
			yield return null;
			intention = GetIntention();
		}
		base.LocomotionTeleport.CurrentIntention = intention;
	}

	public abstract LocomotionTeleport.TeleportIntentions GetIntention();

	public abstract void GetAimData(out Ray aimRay);
}
