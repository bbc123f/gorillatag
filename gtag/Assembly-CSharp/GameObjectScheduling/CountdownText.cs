using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GameObjectScheduling
{
	public class CountdownText : MonoBehaviour
	{
		private void Awake()
		{
			this.displayText = base.GetComponent<TMP_Text>();
			this.displayTextFormat = this.displayText.text;
			if (this.CountdownTo.FormatString.Length > 0)
			{
				this.displayTextFormat = this.CountdownTo.FormatString;
			}
			this.displayText.text = string.Empty;
		}

		private void OnEnable()
		{
			if (this.monitor == null)
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
			TimeSpan timeSpan = this.TryParseDateTime().Subtract(GorillaComputer.instance.GetServerTime());
			this.displayText.text = string.Empty;
			if (timeSpan.TotalDays < (double)this.CountdownTo.DaysThreshold)
			{
				if (timeSpan.Days > 0)
				{
					this.displayText.text = string.Format(this.displayTextFormat, timeSpan.Days, this.getTimeChunkString(CountdownText.TimeChunk.DAY, timeSpan.Days));
				}
				else if (timeSpan.Hours > 0)
				{
					this.displayText.text = string.Format(this.displayTextFormat, timeSpan.Hours, this.getTimeChunkString(CountdownText.TimeChunk.HOUR, timeSpan.Hours));
				}
				else if (timeSpan.Minutes > 0)
				{
					this.displayText.text = string.Format(this.displayTextFormat, timeSpan.Minutes, this.getTimeChunkString(CountdownText.TimeChunk.MINUTE, timeSpan.Minutes));
				}
			}
			this.monitor = null;
			yield break;
		}

		private string getTimeChunkString(CountdownText.TimeChunk chunk, int n)
		{
			switch (chunk)
			{
			case CountdownText.TimeChunk.DAY:
				if (n == 1)
				{
					return "day";
				}
				return "days";
			case CountdownText.TimeChunk.HOUR:
				if (n == 1)
				{
					return "hour";
				}
				return "hours";
			case CountdownText.TimeChunk.MINUTE:
				if (n == 1)
				{
					return "minute";
				}
				return "minutes";
			default:
				return string.Empty;
			}
		}

		private DateTime TryParseDateTime()
		{
			DateTime result;
			try
			{
				result = DateTime.Parse(this.CountdownTo.CountdownTo, CultureInfo.InvariantCulture);
			}
			catch
			{
				result = DateTime.MinValue;
			}
			return result;
		}

		public CountdownText()
		{
		}

		[SerializeField]
		private CountdownTextDate CountdownTo;

		private TMP_Text displayText;

		private string displayTextFormat;

		private Coroutine monitor;

		private enum TimeChunk
		{
			DAY,
			HOUR,
			MINUTE
		}

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
				CountdownText countdownText = this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
				}
				if (!(GorillaComputer.instance == null) && GorillaComputer.instance.startupMillis != 0L)
				{
					TimeSpan timeSpan = countdownText.TryParseDateTime().Subtract(GorillaComputer.instance.GetServerTime());
					countdownText.displayText.text = string.Empty;
					if (timeSpan.TotalDays < (double)countdownText.CountdownTo.DaysThreshold)
					{
						if (timeSpan.Days > 0)
						{
							countdownText.displayText.text = string.Format(countdownText.displayTextFormat, timeSpan.Days, countdownText.getTimeChunkString(CountdownText.TimeChunk.DAY, timeSpan.Days));
						}
						else if (timeSpan.Hours > 0)
						{
							countdownText.displayText.text = string.Format(countdownText.displayTextFormat, timeSpan.Hours, countdownText.getTimeChunkString(CountdownText.TimeChunk.HOUR, timeSpan.Hours));
						}
						else if (timeSpan.Minutes > 0)
						{
							countdownText.displayText.text = string.Format(countdownText.displayTextFormat, timeSpan.Minutes, countdownText.getTimeChunkString(CountdownText.TimeChunk.MINUTE, timeSpan.Minutes));
						}
					}
					countdownText.monitor = null;
					return false;
				}
				this.<>2__current = null;
				this.<>1__state = 1;
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

			public CountdownText <>4__this;
		}
	}
}
