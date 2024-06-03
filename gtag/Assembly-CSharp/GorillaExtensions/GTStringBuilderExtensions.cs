using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace GorillaExtensions
{
	public static class GTStringBuilderExtensions
	{
		public static IEnumerable<string> GetChunks(this StringBuilder sb, int max = 16300)
		{
			int end;
			for (int i = 0; i < sb.Length; i = end + 1)
			{
				end = Math.Min(i + max, sb.Length);
				if (end < sb.Length)
				{
					int num = -1;
					for (int j = end - 1; j >= i; j--)
					{
						if (sb[j] == '\n')
						{
							num = j;
							break;
						}
					}
					if (num != -1)
					{
						end = num;
					}
				}
				yield return sb.ToString(i, end - i);
			}
			yield break;
		}

		public static StringBuilder GTAddPath(this StringBuilder stringBuilderToAddTo, GameObject gameObject)
		{
			gameObject.transform.GetPath(ref stringBuilderToAddTo);
			return stringBuilderToAddTo;
		}

		public static StringBuilder GTAddPath(this StringBuilder stringBuilderToAddTo, Transform transform)
		{
			transform.GetPath(ref stringBuilderToAddTo);
			return stringBuilderToAddTo;
		}

		public static StringBuilder GTQuote(this StringBuilder sb, string value)
		{
			return sb.Append('"').Append(value).Append('"');
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b)
		{
			return sb.Append(a).Append(b);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c)
		{
			return sb.Append(a).Append(b).Append(c);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d)
		{
			return sb.Append(a).Append(b).Append(c).Append(d);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e)
		{
			return sb.Append(a).Append(b).Append(c).Append(d).Append(e);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f)
		{
			return sb.Append(a).Append(b).Append(c).Append(d).Append(e).Append(f);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f, string g)
		{
			return sb.Append(a).Append(b).Append(c).Append(d).Append(e).Append(f).Append(g);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h)
		{
			return sb.Append(a).Append(b).Append(c).Append(d).Append(e).Append(f).Append(g).Append(h);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i)
		{
			return sb.Append(a).Append(b).Append(c).Append(d).Append(e).Append(f).Append(g).Append(h).Append(i);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j)
		{
			return sb.Append(a).Append(b).Append(c).Append(d).Append(e).Append(f).Append(g).Append(h).Append(i).Append(j);
		}

		[CompilerGenerated]
		private sealed class <GetChunks>d__0 : IEnumerable<string>, IEnumerable, IEnumerator<string>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <GetChunks>d__0(int <>1__state)
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
				int num2;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
					num2 = end + 1;
				}
				else
				{
					this.<>1__state = -1;
					num2 = 0;
				}
				if (num2 >= sb.Length)
				{
					return false;
				}
				end = Math.Min(num2 + max, sb.Length);
				if (end < sb.Length)
				{
					int num3 = -1;
					for (int i = end - 1; i >= num2; i--)
					{
						if (sb[i] == '\n')
						{
							num3 = i;
							break;
						}
					}
					if (num3 != -1)
					{
						end = num3;
					}
				}
				this.<>2__current = sb.ToString(num2, end - num2);
				this.<>1__state = 1;
				return true;
			}

			string IEnumerator<string>.Current
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
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				GTStringBuilderExtensions.<GetChunks>d__0 <GetChunks>d__;
				if (this.<>1__state == -2 && this.<>l__initialThreadId == Environment.CurrentManagedThreadId)
				{
					this.<>1__state = 0;
					<GetChunks>d__ = this;
				}
				else
				{
					<GetChunks>d__ = new GTStringBuilderExtensions.<GetChunks>d__0(0);
				}
				<GetChunks>d__.sb = sb;
				<GetChunks>d__.max = max;
				return <GetChunks>d__;
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<System.String>.GetEnumerator();
			}

			private int <>1__state;

			private string <>2__current;

			private int <>l__initialThreadId;

			private int max;

			public int <>3__max;

			private StringBuilder sb;

			public StringBuilder <>3__sb;

			private int <end>5__2;
		}
	}
}
