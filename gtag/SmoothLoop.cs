using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SmoothLoop : MonoBehaviour
{
	private void Start()
	{
		if (this.delay != 0f && !this.randomStart)
		{
			this.source.Stop();
			base.StartCoroutine(this.DelayedStart());
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.Play();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	private void Update()
	{
		if (this.source.time > this.source.clip.length * 0.95f)
		{
			this.source.time = 0.1f;
		}
	}

	private void OnEnable()
	{
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.Play();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	public IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(this.delay);
		this.source.Play();
		yield break;
	}

	public SmoothLoop()
	{
	}

	public AudioSource source;

	public float delay;

	public bool randomStart;

	[CompilerGenerated]
	private sealed class <DelayedStart>d__6 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <DelayedStart>d__6(int <>1__state)
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
			SmoothLoop smoothLoop = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(smoothLoop.delay);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			smoothLoop.source.Play();
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

		public SmoothLoop <>4__this;
	}
}
