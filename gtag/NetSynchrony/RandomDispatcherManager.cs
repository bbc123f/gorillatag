﻿using System;
using GorillaNetworking;
using UnityEngine;

namespace NetSynchrony
{
	public class RandomDispatcherManager : MonoBehaviour
	{
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
		}

		private void OnTimeChanged()
		{
			this.AdjustedServerTime();
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Sync((float)this.serverTime);
			}
		}

		private void AdjustedServerTime()
		{
			DateTime dateTime = new DateTime(2020, 1, 1);
			long num = GorillaComputer.instance.GetServerTime().Ticks - dateTime.Ticks;
			this.serverTime = (double)((float)num / 10000000f);
		}

		private void Start()
		{
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Init((float)this.serverTime);
			}
		}

		private void Update()
		{
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Tick((float)this.serverTime);
			}
			this.serverTime += (double)Time.deltaTime;
		}

		[SerializeField]
		private RandomDispatcher[] randomDispatchers;

		private static RandomDispatcherManager __instance;

		private double serverTime;
	}
}
