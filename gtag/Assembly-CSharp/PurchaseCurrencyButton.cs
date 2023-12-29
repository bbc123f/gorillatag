using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

public class PurchaseCurrencyButton : GorillaPressableButton
{
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCurrencyPurchaseButton(this.purchaseCurrencySize);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	public string purchaseCurrencySize;

	public float buttonFadeTime = 0.25f;
}
