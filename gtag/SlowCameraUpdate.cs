using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SlowCameraUpdate : MonoBehaviour
{
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	public IEnumerator UpdateMirror()
	{
		for (;;)
		{
			if (base.gameObject.activeSelf)
			{
				Debug.Log("rendering camera!");
				this.myCamera.Render();
			}
			yield return new WaitForSeconds(this.timeToNextFrame);
		}
		yield break;
	}

	public SlowCameraUpdate()
	{
	}

	private Camera myCamera;

	private float frameRate;

	private float timeToNextFrame;

	[CompilerGenerated]
	private sealed class <UpdateMirror>d__6 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateMirror>d__6(int <>1__state)
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
			SlowCameraUpdate slowCameraUpdate = this;
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
			if (slowCameraUpdate.gameObject.activeSelf)
			{
				Debug.Log("rendering camera!");
				slowCameraUpdate.myCamera.Render();
			}
			this.<>2__current = new WaitForSeconds(slowCameraUpdate.timeToNextFrame);
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

		public SlowCameraUpdate <>4__this;
	}
}
