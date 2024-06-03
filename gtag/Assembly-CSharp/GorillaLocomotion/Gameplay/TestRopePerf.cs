using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	public class TestRopePerf : MonoBehaviour
	{
		private IEnumerator Start()
		{
			yield break;
		}

		public TestRopePerf()
		{
		}

		[SerializeField]
		private GameObject ropesOld;

		[SerializeField]
		private GameObject ropesCustom;

		[SerializeField]
		private GameObject ropesCustomVectorized;

		[CompilerGenerated]
		private sealed class <Start>d__3 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <Start>d__3(int <>1__state)
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
				if (num != 0)
				{
					return false;
				}
				this.<>1__state = -1;
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
		}
	}
}
