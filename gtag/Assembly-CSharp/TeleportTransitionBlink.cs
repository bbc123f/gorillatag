using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TeleportTransitionBlink : TeleportTransition
{
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.StartCoroutine(this.BlinkCoroutine());
	}

	protected IEnumerator BlinkCoroutine()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		float elapsedTime = 0f;
		float teleportTime = this.TransitionDuration * this.TeleportDelay;
		bool teleported = false;
		while (elapsedTime < this.TransitionDuration)
		{
			yield return null;
			elapsedTime += Time.deltaTime;
			if (!teleported && elapsedTime >= teleportTime)
			{
				teleported = true;
				base.LocomotionTeleport.DoTeleport();
			}
		}
		base.LocomotionTeleport.IsTransitioning = false;
		yield break;
	}

	public TeleportTransitionBlink()
	{
	}

	[Tooltip("How long the transition takes. Usually this is greater than Teleport Delay.")]
	[Range(0.01f, 2f)]
	public float TransitionDuration = 0.5f;

	[Tooltip("At what percentage of the elapsed transition time does the teleport occur?")]
	[Range(0f, 1f)]
	public float TeleportDelay = 0.5f;

	[Tooltip("Fade to black over the duration of the transition")]
	public AnimationCurve FadeLevels = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	[CompilerGenerated]
	private sealed class <BlinkCoroutine>d__4 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <BlinkCoroutine>d__4(int <>1__state)
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
			TeleportTransitionBlink teleportTransitionBlink = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				elapsedTime += Time.deltaTime;
				if (!teleported && elapsedTime >= teleportTime)
				{
					teleported = true;
					teleportTransitionBlink.LocomotionTeleport.DoTeleport();
				}
			}
			else
			{
				this.<>1__state = -1;
				teleportTransitionBlink.LocomotionTeleport.IsTransitioning = true;
				elapsedTime = 0f;
				teleportTime = teleportTransitionBlink.TransitionDuration * teleportTransitionBlink.TeleportDelay;
				teleported = false;
			}
			if (elapsedTime >= teleportTransitionBlink.TransitionDuration)
			{
				teleportTransitionBlink.LocomotionTeleport.IsTransitioning = false;
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

		public TeleportTransitionBlink <>4__this;

		private float <elapsedTime>5__2;

		private float <teleportTime>5__3;

		private bool <teleported>5__4;
	}
}
