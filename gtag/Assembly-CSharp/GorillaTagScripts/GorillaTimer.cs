using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	public class GorillaTimer : MonoBehaviourPun
	{
		private void Awake()
		{
			this.ResetTimer();
		}

		public void StartTimer()
		{
			this.startTimer = true;
			UnityAction<GorillaTimer> unityAction = this.onTimerStarted;
			if (unityAction == null)
			{
				return;
			}
			unityAction(this);
		}

		public IEnumerator DelayedReStartTimer(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			this.RestartTimer();
			yield break;
		}

		private void StopTimer()
		{
			this.startTimer = false;
			UnityAction<GorillaTimer> unityAction = this.onTimerStopped;
			if (unityAction == null)
			{
				return;
			}
			unityAction(this);
		}

		private void ResetTimer()
		{
			this.passedTime = 0f;
		}

		public void RestartTimer()
		{
			if (this.useRandomDuration)
			{
				this.SetTimerDuration(Random.Range(this.randTimeMin, this.randTimeMax));
			}
			this.ResetTimer();
			this.StartTimer();
		}

		private void SetTimerDuration(float timer)
		{
			this.timerDuration = timer;
		}

		public void InvokeUpdate()
		{
			if (this.startTimer)
			{
				this.passedTime += Time.deltaTime;
			}
			if (this.passedTime >= this.timerDuration)
			{
				this.StopTimer();
				this.ResetTimer();
			}
		}

		public float GetPassedTime()
		{
			return this.passedTime;
		}

		public void SetPassedTime(float time)
		{
			this.passedTime = time;
		}

		public float GetRemainingTime()
		{
			return this.timerDuration - this.passedTime;
		}

		public void OnEnable()
		{
			GorillaTimerManager.RegisterGorillaTimer(this);
		}

		public void OnDisable()
		{
			GorillaTimerManager.UnregisterGorillaTimer(this);
		}

		public GorillaTimer()
		{
		}

		[SerializeField]
		private float timerDuration;

		[SerializeField]
		private bool useRandomDuration;

		[SerializeField]
		private float randTimeMin;

		[SerializeField]
		private float randTimeMax;

		private float passedTime;

		private bool startTimer;

		private bool resetTimer;

		public UnityAction<GorillaTimer> onTimerStarted;

		public UnityAction<GorillaTimer> onTimerStopped;

		[CompilerGenerated]
		private sealed class <DelayedReStartTimer>d__11 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <DelayedReStartTimer>d__11(int <>1__state)
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
				GorillaTimer gorillaTimer = this;
				if (num == 0)
				{
					this.<>1__state = -1;
					this.<>2__current = new WaitForSeconds(delayTime);
					this.<>1__state = 1;
					return true;
				}
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				gorillaTimer.RestartTimer();
				return false;
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

			public float delayTime;

			public GorillaTimer <>4__this;
		}
	}
}
