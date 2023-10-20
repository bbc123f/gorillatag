using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viveport
{
	// Token: 0x02000243 RID: 579
	public class MainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x06000E2D RID: 3629 RVA: 0x00052335 File Offset: 0x00050535
		private void Awake()
		{
			if (MainThreadDispatcher.instance == null)
			{
				MainThreadDispatcher.instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x00052358 File Offset: 0x00050558
		public void Update()
		{
			Queue<Action> obj = MainThreadDispatcher.actions;
			lock (obj)
			{
				while (MainThreadDispatcher.actions.Count > 0)
				{
					MainThreadDispatcher.actions.Dequeue()();
				}
			}
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x000523B0 File Offset: 0x000505B0
		public static MainThreadDispatcher Instance()
		{
			if (MainThreadDispatcher.instance == null)
			{
				throw new Exception("Could not find the MainThreadDispatcher GameObject. Please ensure you have added this script to an empty GameObject in your scene.");
			}
			return MainThreadDispatcher.instance;
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x000523CF File Offset: 0x000505CF
		private void OnDestroy()
		{
			MainThreadDispatcher.instance = null;
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x000523D8 File Offset: 0x000505D8
		public void Enqueue(IEnumerator action)
		{
			Queue<Action> obj = MainThreadDispatcher.actions;
			lock (obj)
			{
				MainThreadDispatcher.actions.Enqueue(delegate
				{
					this.StartCoroutine(action);
				});
			}
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x0005243C File Offset: 0x0005063C
		public void Enqueue(Action action)
		{
			this.Enqueue(this.ActionWrapper(action));
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x0005244B File Offset: 0x0005064B
		public void Enqueue<T1>(Action<T1> action, T1 param1)
		{
			this.Enqueue(this.ActionWrapper<T1>(action, param1));
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x0005245B File Offset: 0x0005065B
		public void Enqueue<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			this.Enqueue(this.ActionWrapper<T1, T2>(action, param1, param2));
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x0005246C File Offset: 0x0005066C
		public void Enqueue<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3>(action, param1, param2, param3));
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x0005247F File Offset: 0x0005067F
		public void Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3, T4>(action, param1, param2, param3, param4));
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x00052494 File Offset: 0x00050694
		private IEnumerator ActionWrapper(Action action)
		{
			action();
			yield return null;
			yield break;
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x000524A3 File Offset: 0x000506A3
		private IEnumerator ActionWrapper<T1>(Action<T1> action, T1 param1)
		{
			action(param1);
			yield return null;
			yield break;
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x000524B9 File Offset: 0x000506B9
		private IEnumerator ActionWrapper<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			action(param1, param2);
			yield return null;
			yield break;
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x000524D6 File Offset: 0x000506D6
		private IEnumerator ActionWrapper<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			action(param1, param2, param3);
			yield return null;
			yield break;
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x000524FB File Offset: 0x000506FB
		private IEnumerator ActionWrapper<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			action(param1, param2, param3, param4);
			yield return null;
			yield break;
		}

		// Token: 0x0400115C RID: 4444
		private static readonly Queue<Action> actions = new Queue<Action>();

		// Token: 0x0400115D RID: 4445
		private static MainThreadDispatcher instance = null;
	}
}
