﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace NetSynchrony
{
	[CreateAssetMenu(fileName = "RandomDispatcher", menuName = "NetSynchrony/RandomDispatcher", order = 0)]
	public class RandomDispatcher : ScriptableObject
	{
		public event RandomDispatcher.RandomDispatcherEvent Dispatch
		{
			[CompilerGenerated]
			add
			{
				RandomDispatcher.RandomDispatcherEvent randomDispatcherEvent = this.Dispatch;
				RandomDispatcher.RandomDispatcherEvent randomDispatcherEvent2;
				do
				{
					randomDispatcherEvent2 = randomDispatcherEvent;
					RandomDispatcher.RandomDispatcherEvent value2 = (RandomDispatcher.RandomDispatcherEvent)Delegate.Combine(randomDispatcherEvent2, value);
					randomDispatcherEvent = Interlocked.CompareExchange<RandomDispatcher.RandomDispatcherEvent>(ref this.Dispatch, value2, randomDispatcherEvent2);
				}
				while (randomDispatcherEvent != randomDispatcherEvent2);
			}
			[CompilerGenerated]
			remove
			{
				RandomDispatcher.RandomDispatcherEvent randomDispatcherEvent = this.Dispatch;
				RandomDispatcher.RandomDispatcherEvent randomDispatcherEvent2;
				do
				{
					randomDispatcherEvent2 = randomDispatcherEvent;
					RandomDispatcher.RandomDispatcherEvent value2 = (RandomDispatcher.RandomDispatcherEvent)Delegate.Remove(randomDispatcherEvent2, value);
					randomDispatcherEvent = Interlocked.CompareExchange<RandomDispatcher.RandomDispatcherEvent>(ref this.Dispatch, value2, randomDispatcherEvent2);
				}
				while (randomDispatcherEvent != randomDispatcherEvent2);
			}
		}

		public void Init(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			this.dispatchTimes = new List<float>();
			float num = 0f;
			float num2 = this.totalMinutes * 60f;
			Random.InitState(StaticHash.Calculate(Application.buildGUID));
			while (num < num2)
			{
				float num3 = Random.Range(this.minWaitTime, this.maxWaitTime);
				num += num3;
				if ((double)num < seconds)
				{
					this.index = this.dispatchTimes.Count;
				}
				this.dispatchTimes.Add(num);
			}
			Random.InitState((int)DateTime.Now.Ticks);
		}

		public void Sync(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			for (int i = 0; i < this.dispatchTimes.Count; i++)
			{
				if ((double)this.dispatchTimes[i] < seconds)
				{
					this.index = i;
				}
			}
		}

		public void Tick(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			if ((double)this.dispatchTimes[this.index] < seconds)
			{
				this.index = (this.index + 1) % this.dispatchTimes.Count;
				if (this.Dispatch != null)
				{
					this.Dispatch(this);
				}
			}
		}

		public RandomDispatcher()
		{
		}

		[CompilerGenerated]
		private RandomDispatcher.RandomDispatcherEvent Dispatch;

		[SerializeField]
		private float minWaitTime = 1f;

		[SerializeField]
		private float maxWaitTime = 10f;

		[SerializeField]
		private float totalMinutes = 60f;

		private List<float> dispatchTimes;

		private int index = -1;

		public delegate void RandomDispatcherEvent(RandomDispatcher randomDispatcher);
	}
}
