using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TeleportTransitionWarp : TeleportTransition
{
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.StartCoroutine(this.DoWarp());
	}

	private IEnumerator DoWarp()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		Vector3 startPosition = base.LocomotionTeleport.GetCharacterPosition();
		float elapsedTime = 0f;
		while (elapsedTime < this.TransitionDuration)
		{
			elapsedTime += Time.deltaTime;
			float num = elapsedTime / this.TransitionDuration;
			float num2 = this.PositionLerp.Evaluate(num);
			base.LocomotionTeleport.DoWarp(startPosition, num2);
			yield return null;
		}
		base.LocomotionTeleport.DoWarp(startPosition, 1f);
		base.LocomotionTeleport.IsTransitioning = false;
		yield break;
	}

	public TeleportTransitionWarp()
	{
	}

	[Tooltip("How much time the warp transition takes to complete.")]
	[Range(0.01f, 1f)]
	public float TransitionDuration = 0.5f;

	[HideInInspector]
	public AnimationCurve PositionLerp = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[CompilerGenerated]
	private sealed class <DoWarp>d__3 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <DoWarp>d__3(int <>1__state)
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
			TeleportTransitionWarp teleportTransitionWarp = this;
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
				teleportTransitionWarp.LocomotionTeleport.IsTransitioning = true;
				startPosition = teleportTransitionWarp.LocomotionTeleport.GetCharacterPosition();
				elapsedTime = 0f;
			}
			if (elapsedTime >= teleportTransitionWarp.TransitionDuration)
			{
				teleportTransitionWarp.LocomotionTeleport.DoWarp(startPosition, 1f);
				teleportTransitionWarp.LocomotionTeleport.IsTransitioning = false;
				return false;
			}
			elapsedTime += Time.deltaTime;
			float num2 = elapsedTime / teleportTransitionWarp.TransitionDuration;
			float num3 = teleportTransitionWarp.PositionLerp.Evaluate(num2);
			teleportTransitionWarp.LocomotionTeleport.DoWarp(startPosition, num3);
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

		public TeleportTransitionWarp <>4__this;

		private Vector3 <startPosition>5__2;

		private float <elapsedTime>5__3;
	}
}
