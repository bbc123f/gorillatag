using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

namespace GameObjectScheduling
{
	public class GameObjectScheduler : MonoBehaviour
	{
		private void Start()
		{
			this.schedule.Validate();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				list.Add(base.transform.GetChild(i).gameObject);
			}
			this.scheduledGameObject = list.ToArray();
			for (int j = 0; j < this.scheduledGameObject.Length; j++)
			{
				this.scheduledGameObject[j].SetActive(false);
			}
			this.dispatcher = base.GetComponent<GameObjectSchedulerEventDispatcher>();
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}

		private void OnEnable()
		{
			if (this.monitor == null && this.scheduledGameObject != null)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		private void OnDisable()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			bool previousState = this.getActiveState();
			for (int i = 0; i < this.scheduledGameObject.Length; i++)
			{
				this.scheduledGameObject[i].SetActive(previousState);
			}
			for (;;)
			{
				yield return new WaitForSeconds(60f);
				bool activeState = this.getActiveState();
				if (previousState != activeState)
				{
					this.changeActiveState(activeState);
					previousState = activeState;
				}
			}
			yield break;
		}

		private bool getActiveState()
		{
			this.currentNodeIndex = this.schedule.GetCurrentNodeIndex(this.getServerTime(), 0);
			bool flag;
			if (this.currentNodeIndex == -1)
			{
				flag = this.schedule.InitialState;
			}
			else if (this.currentNodeIndex < this.schedule.Nodes.Length)
			{
				flag = this.schedule.Nodes[this.currentNodeIndex].ActiveState;
			}
			else
			{
				flag = this.schedule.Nodes[this.schedule.Nodes.Length - 1].ActiveState;
			}
			return flag;
		}

		private DateTime getServerTime()
		{
			return GorillaComputer.instance.GetServerTime();
		}

		private void changeActiveState(bool state)
		{
			if (state)
			{
				for (int i = 0; i < this.scheduledGameObject.Length; i++)
				{
					this.scheduledGameObject[i].SetActive(true);
				}
				if (this.dispatcher != null && this.dispatcher.OnScheduledActivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
			}
			else
			{
				if (this.dispatcher != null && this.dispatcher.OnScheduledDeactivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
				for (int j = 0; j < this.scheduledGameObject.Length; j++)
				{
					this.scheduledGameObject[j].SetActive(false);
				}
			}
		}

		public GameObjectScheduler()
		{
		}

		[SerializeField]
		private GameObjectSchedule schedule;

		private GameObject[] scheduledGameObject;

		private GameObjectSchedulerEventDispatcher dispatcher;

		private int currentNodeIndex = -1;

		private Coroutine monitor;

		[CompilerGenerated]
		private sealed class <MonitorTime>d__8 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <MonitorTime>d__8(int <>1__state)
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
				GameObjectScheduler gameObjectScheduler = this;
				switch (num)
				{
				case 0:
					this.<>1__state = -1;
					break;
				case 1:
					this.<>1__state = -1;
					break;
				case 2:
				{
					this.<>1__state = -1;
					bool activeState = gameObjectScheduler.getActiveState();
					if (previousState != activeState)
					{
						gameObjectScheduler.changeActiveState(activeState);
						previousState = activeState;
						goto IL_91;
					}
					goto IL_91;
				}
				default:
					return false;
				}
				if (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
				{
					this.<>2__current = null;
					this.<>1__state = 1;
					return true;
				}
				previousState = gameObjectScheduler.getActiveState();
				for (int i = 0; i < gameObjectScheduler.scheduledGameObject.Length; i++)
				{
					gameObjectScheduler.scheduledGameObject[i].SetActive(previousState);
				}
				IL_91:
				this.<>2__current = new WaitForSeconds(60f);
				this.<>1__state = 2;
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

			public GameObjectScheduler <>4__this;

			private bool <previousState>5__2;
		}
	}
}
