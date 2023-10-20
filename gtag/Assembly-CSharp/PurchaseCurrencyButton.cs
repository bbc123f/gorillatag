using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class PurchaseCurrencyButton : GorillaPressableButton
{
	// Token: 0x060007AD RID: 1965 RVA: 0x00030EC0 File Offset: 0x0002F0C0
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCurrencyPurchaseButton(this.purchaseCurrencySize);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00030EE7 File Offset: 0x0002F0E7
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x0400094E RID: 2382
	public string purchaseCurrencySize;

	// Token: 0x0400094F RID: 2383
	public float buttonFadeTime = 0.25f;
}
