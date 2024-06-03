using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class TeleportInputHandler : TeleportSupport
{
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

	public abstract LocomotionTeleport.TeleportIntentions GetIntention();

	public abstract void GetAimData(out Ray aimRay);

	[CompilerGenerated]
	private void <.ctor>b__2_0()
	{
		base.StartCoroutine(this.TeleportReadyCoroutine());
	}

	[CompilerGenerated]
	private void <.ctor>b__2_1()
	{
		base.StartCoroutine(this.TeleportAimCoroutine());
	}

	private readonly Action _startReadyAction;

	private readonly Action _startAimAction;

	[CompilerGenerated]
	private sealed class <TeleportAimCoroutine>d__6 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <TeleportAimCoroutine>d__6(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			TeleportInputHandler teleportInputHandler = this;
			LocomotionTeleport.TeleportIntentions intention;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				intention = teleportInputHandler.GetIntention();
			}
			else
			{
				this.<>1__state = -1;
				intention = teleportInputHandler.GetIntention();
			}
			if (intention != LocomotionTeleport.TeleportIntentions.Aim && intention != LocomotionTeleport.TeleportIntentions.PreTeleport)
			{
				teleportInputHandler.LocomotionTeleport.CurrentIntention = intention;
				return false;
			}
			teleportInputHandler.LocomotionTeleport.CurrentIntention = intention;
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public TeleportInputHandler <>4__this;
	}

	[CompilerGenerated]
	private sealed class <TeleportReadyCoroutine>d__5 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <TeleportReadyCoroutine>d__5(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			TeleportInputHandler teleportInputHandler = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
			}
			else
			{
				this.<>1__state = -1;
			}
			if (teleportInputHandler.GetIntention() == LocomotionTeleport.TeleportIntentions.Aim)
			{
				teleportInputHandler.LocomotionTeleport.CurrentIntention = LocomotionTeleport.TeleportIntentions.Aim;
				return false;
			}
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public TeleportInputHandler <>4__this;
	}
}
