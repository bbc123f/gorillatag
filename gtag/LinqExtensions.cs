using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class LinqExtensions
{
	public static IEnumerable<T> Self<T>(this T value)
	{
		yield return value;
		yield break;
	}

	[CompilerGenerated]
	private sealed class <Self>d__0<T> : IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <Self>d__0(int <>1__state)
		{
			this.<>1__state = <>1__state;
			this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = value;
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			return false;
		}

		T IEnumerator<T>.Current
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

		[DebuggerHidden]
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			LinqExtensions.<Self>d__0<T> <Self>d__;
			if (this.<>1__state == -2 && this.<>l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				this.<>1__state = 0;
				<Self>d__ = this;
			}
			else
			{
				<Self>d__ = new LinqExtensions.<Self>d__0<T>(0);
			}
			<Self>d__.value = value;
			return <Self>d__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();
		}

		private int <>1__state;

		private T <>2__current;

		private int <>l__initialThreadId;

		private T value;

		public T <>3__value;
	}
}
