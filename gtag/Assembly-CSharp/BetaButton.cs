using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BetaButton : GorillaPressableButton
{
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.count++;
		base.StartCoroutine(this.ButtonColorUpdate());
		if (this.count >= 10)
		{
			this.betaParent.SetActive(false);
			PlayerPrefs.SetString("CheckedBox2", "true");
			PlayerPrefs.Save();
		}
	}

	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	public BetaButton()
	{
	}

	public GameObject betaParent;

	public int count;

	public float buttonFadeTime = 0.25f;

	public Text messageText;

	[CompilerGenerated]
	private sealed class <ButtonColorUpdate>d__5 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <ButtonColorUpdate>d__5(int <>1__state)
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
			BetaButton betaButton = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				betaButton.buttonRenderer.material = betaButton.pressedMaterial;
				this.<>2__current = new WaitForSeconds(betaButton.buttonFadeTime);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			betaButton.buttonRenderer.material = betaButton.unpressedMaterial;
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

		public BetaButton <>4__this;
	}
}
