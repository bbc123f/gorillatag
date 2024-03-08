using System;
using System.Collections;
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
	}
}
