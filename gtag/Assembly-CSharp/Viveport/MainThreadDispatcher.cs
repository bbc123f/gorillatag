using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viveport
{
	// Token: 0x02000242 RID: 578
	public class MainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x06000E26 RID: 3622 RVA: 0x00051F59 File Offset: 0x00050159
		private void Awake()
		{
			if (MainThreadDispatcher.instance == null)
			{
				MainThreadDispatcher.instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x00051F7C File Offset: 0x0005017C
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

		// Token: 0x06000E28 RID: 3624 RVA: 0x00051FD4 File Offset: 0x000501D4
		public static MainThreadDispatcher Instance()
		{
			if (MainThreadDispatcher.instance == null)
			{
				throw new Exception("Could not find the MainThreadDispatcher GameObject. Please ensure you have added this script to an empty GameObject in your scene.");
			}
			return MainThreadDispatcher.instance;
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x00051FF3 File Offset: 0x000501F3
		private void OnDestroy()
		{
			MainThreadDispatcher.instance = null;
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x00051FFC File Offset: 0x000501FC
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

		// Token: 0x06000E2B RID: 3627 RVA: 0x00052060 File Offset: 0x00050260
		public void Enqueue(Action action)
		{
			this.Enqueue(this.ActionWrapper(action));
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x0005206F File Offset: 0x0005026F
		public void Enqueue<T1>(Action<T1> action, T1 param1)
		{
			this.Enqueue(this.ActionWrapper<T1>(action, param1));
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x0005207F File Offset: 0x0005027F
		public void Enqueue<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			this.Enqueue(this.ActionWrapper<T1, T2>(action, param1, param2));
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x00052090 File Offset: 0x00050290
		public void Enqueue<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3>(action, param1, param2, param3));
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x000520A3 File Offset: 0x000502A3
		public void Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3, T4>(action, param1, param2, param3, param4));
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x000520B8 File Offset: 0x000502B8
		private IEnumerator ActionWrapper(Action action)
		{
			action();
			yield return null;
			yield break;
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x000520C7 File Offset: 0x000502C7
		private IEnumerator ActionWrapper<T1>(Action<T1> action, T1 param1)
		{
			action(param1);
			yield return null;
			yield break;
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x000520DD File Offset: 0x000502DD
		private IEnumerator ActionWrapper<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			action(param1, param2);
			yield return null;
			yield break;
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x000520FA File Offset: 0x000502FA
		private IEnumerator ActionWrapper<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			action(param1, param2, param3);
			yield return null;
			yield break;
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x0005211F File Offset: 0x0005031F
		private IEnumerator ActionWrapper<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			action(param1, param2, param3, param4);
			yield return null;
			yield break;
		}

		// Token: 0x04001156 RID: 4438
		private static readonly Queue<Action> actions = new Queue<Action>();

		// Token: 0x04001157 RID: 4439
		private static MainThreadDispatcher instance = null;
	}
}
