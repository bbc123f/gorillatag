using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class LinqUtils
{
	public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		HashSet<TResult> set = new HashSet<TResult>();
		foreach (TSource tsource in source)
		{
			TResult item = selector(tsource);
			if (set.Add(item))
			{
				yield return tsource;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T obj in source)
		{
			action(obj);
		}
		return source;
	}

	public static T[] AsArray<T>(this IEnumerable<T> source)
	{
		return (T[])source;
	}

	public static List<T> AsList<T>(this IEnumerable<T> source)
	{
		return (List<T>)source;
	}

	public static IList<T> Transform<T>(this IList<T> list, Func<T, T> action)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = action(list[i]);
		}
		return list;
	}

	public static IEnumerable<T> Self<T>(this T value)
	{
		yield return value;
		yield break;
	}

	[CompilerGenerated]
	private sealed class <DistinctBy>d__0<TSource, TResult> : IEnumerable<!0>, IEnumerable, IEnumerator<TSource>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <DistinctBy>d__0(int <>1__state)
		{
			this.<>1__state = <>1__state;
			this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = this.<>1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					this.<>m__Finally1();
				}
			}
		}

		bool IEnumerator.MoveNext()
		{
			bool result;
			try
			{
				int num = this.<>1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -3;
				}
				else
				{
					this.<>1__state = -1;
					set = new HashSet<TResult>();
					enumerator = source.GetEnumerator();
					this.<>1__state = -3;
				}
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					TResult item = selector(arg);
					if (set.Add(item))
					{
						this.<>2__current = arg;
						this.<>1__state = 1;
						return true;
					}
				}
				this.<>m__Finally1();
				enumerator = null;
				result = false;
			}
			catch
			{
				this.System.IDisposable.Dispose();
				throw;
			}
			return result;
		}

		private void <>m__Finally1()
		{
			this.<>1__state = -1;
			if (enumerator != null)
			{
				enumerator.Dispose();
			}
		}

		TSource IEnumerator<!0>.Current
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
		IEnumerator<TSource> IEnumerable<!0>.GetEnumerator()
		{
			LinqUtils.<DistinctBy>d__0<TSource, TResult> <DistinctBy>d__;
			if (this.<>1__state == -2 && this.<>l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				this.<>1__state = 0;
				<DistinctBy>d__ = this;
			}
			else
			{
				<DistinctBy>d__ = new LinqUtils.<DistinctBy>d__0<TSource, TResult>(0);
			}
			<DistinctBy>d__.source = source;
			<DistinctBy>d__.selector = selector;
			return <DistinctBy>d__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.System.Collections.Generic.IEnumerable<TSource>.GetEnumerator();
		}

		private int <>1__state;

		private TSource <>2__current;

		private int <>l__initialThreadId;

		private IEnumerable<TSource> source;

		public IEnumerable<TSource> <>3__source;

		private Func<TSource, TResult> selector;

		public Func<TSource, TResult> <>3__selector;

		private HashSet<TResult> <set>5__2;

		private IEnumerator<TSource> <>7__wrap2;
	}

	[CompilerGenerated]
	private sealed class <Self>d__5<T> : IEnumerable<!0>, IEnumerable, IEnumerator<!0>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <Self>d__5(int <>1__state)
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

		T IEnumerator<!0>.Current
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
		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			LinqUtils.<Self>d__5<T> <Self>d__;
			if (this.<>1__state == -2 && this.<>l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				this.<>1__state = 0;
				<Self>d__ = this;
			}
			else
			{
				<Self>d__ = new LinqUtils.<Self>d__5<T>(0);
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
