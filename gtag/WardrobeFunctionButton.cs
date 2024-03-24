using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

public class WardrobeFunctionButton : GorillaPressableButton
{
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeFunctionButton(this.function);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	public override void UpdateColor()
	{
	}

	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	public WardrobeFunctionButton()
	{
	}

	public string function;

	public float buttonFadeTime = 0.25f;

	[CompilerGenerated]
	private sealed class <ButtonColorUpdate>d__4 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <ButtonColorUpdate>d__4(int <>1__state)
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
			WardrobeFunctionButton wardrobeFunctionButton = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				wardrobeFunctionButton.buttonRenderer.material = wardrobeFunctionButton.pressedMaterial;
				this.<>2__current = new WaitForSeconds(wardrobeFunctionButton.buttonFadeTime);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			wardrobeFunctionButton.buttonRenderer.material = wardrobeFunctionButton.unpressedMaterial;
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

		public WardrobeFunctionButton <>4__this;
	}
}
