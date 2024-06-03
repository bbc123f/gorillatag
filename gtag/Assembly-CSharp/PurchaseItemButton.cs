using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

public class PurchaseItemButton : GorillaPressableButton
{
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressPurchaseItemButton(this, isLeftHand);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	public PurchaseItemButton()
	{
	}

	public string buttonSide;

	[CompilerGenerated]
	private sealed class <ButtonColorUpdate>d__2 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <ButtonColorUpdate>d__2(int <>1__state)
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
			PurchaseItemButton purchaseItemButton = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				Debug.Log("did this happen?");
				purchaseItemButton.buttonRenderer.material = purchaseItemButton.pressedMaterial;
				this.<>2__current = new WaitForSeconds(purchaseItemButton.debounceTime);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			purchaseItemButton.buttonRenderer.material = (purchaseItemButton.isOn ? purchaseItemButton.pressedMaterial : purchaseItemButton.unpressedMaterial);
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

		public PurchaseItemButton <>4__this;
	}
}
