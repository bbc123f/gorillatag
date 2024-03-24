using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SodaBubble : MonoBehaviour
{
	public void Pop()
	{
		base.StartCoroutine(this.PopCoroutine());
	}

	private IEnumerator PopCoroutine()
	{
		this.audioSource.Play();
		this.bubbleMesh.gameObject.SetActive(false);
		this.bubbleCollider.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		this.bubbleMesh.gameObject.SetActive(true);
		this.bubbleCollider.gameObject.SetActive(true);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	public SodaBubble()
	{
	}

	public MeshRenderer bubbleMesh;

	public Rigidbody body;

	public MeshCollider bubbleCollider;

	public AudioSource audioSource;

	[CompilerGenerated]
	private sealed class <PopCoroutine>d__5 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <PopCoroutine>d__5(int <>1__state)
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
			SodaBubble sodaBubble = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				sodaBubble.audioSource.Play();
				sodaBubble.bubbleMesh.gameObject.SetActive(false);
				sodaBubble.bubbleCollider.gameObject.SetActive(false);
				this.<>2__current = new WaitForSeconds(1f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			sodaBubble.bubbleMesh.gameObject.SetActive(true);
			sodaBubble.bubbleCollider.gameObject.SetActive(true);
			ObjectPools.instance.Destroy(sodaBubble.gameObject);
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

		public SodaBubble <>4__this;
	}
}
