using System.Collections;
using GorillaNetworking;
using UnityEngine;

public class PurchaseCurrencyButton : GorillaPressableButton
{
	public string purchaseCurrencySize;

	public float buttonFadeTime = 0.25f;

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCurrencyPurchaseButton(purchaseCurrencySize);
		StartCoroutine(ButtonColorUpdate());
	}

	private IEnumerator ButtonColorUpdate()
	{
		buttonRenderer.material = pressedMaterial;
		yield return new WaitForSeconds(buttonFadeTime);
		buttonRenderer.material = unpressedMaterial;
	}
}
