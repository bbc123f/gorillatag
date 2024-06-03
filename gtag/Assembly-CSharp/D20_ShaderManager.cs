using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class D20_ShaderManager : MonoBehaviour
{
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastPosition = base.transform.position;
		Renderer component = base.GetComponent<Renderer>();
		this.material = component.material;
		this.material.SetVector("_Velocity", this.velocity);
		base.StartCoroutine(this.UpdateVelocityCoroutine());
	}

	private IEnumerator UpdateVelocityCoroutine()
	{
		for (;;)
		{
			Vector3 position = base.transform.position;
			this.velocity = (position - this.lastPosition) / this.updateInterval;
			this.lastPosition = position;
			this.material.SetVector("_Velocity", this.velocity);
			yield return new WaitForSeconds(this.updateInterval);
		}
		yield break;
	}

	public D20_ShaderManager()
	{
	}

	private Rigidbody rb;

	private Vector3 lastPosition;

	public float updateInterval = 0.1f;

	public Vector3 velocity;

	private Material material;

	[CompilerGenerated]
	private sealed class <UpdateVelocityCoroutine>d__6 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateVelocityCoroutine>d__6(int <>1__state)
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
			D20_ShaderManager d20_ShaderManager = this;
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
			Vector3 position = d20_ShaderManager.transform.position;
			d20_ShaderManager.velocity = (position - d20_ShaderManager.lastPosition) / d20_ShaderManager.updateInterval;
			d20_ShaderManager.lastPosition = position;
			d20_ShaderManager.material.SetVector("_Velocity", d20_ShaderManager.velocity);
			this.<>2__current = new WaitForSeconds(d20_ShaderManager.updateInterval);
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

		public D20_ShaderManager <>4__this;
	}
}
