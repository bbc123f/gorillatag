using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

public class EarlyAccessButton : GorillaPressableButton
{
	private void Awake()
	{
	}

	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressEarlyAccessButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	public void AlreadyOwn()
	{
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";
	}

	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	public EarlyAccessButton()
	{
	}

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
			EarlyAccessButton earlyAccessButton = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				Debug.Log("did this happen?");
				earlyAccessButton.buttonRenderer.material = earlyAccessButton.pressedMaterial;
				this.<>2__current = new WaitForSeconds(earlyAccessButton.debounceTime);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			earlyAccessButton.buttonRenderer.material = (earlyAccessButton.isOn ? earlyAccessButton.pressedMaterial : earlyAccessButton.unpressedMaterial);
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

		public EarlyAccessButton <>4__this;
	}
}
