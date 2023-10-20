using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class PurchaseItemButton : GorillaPressableButton
{
	// Token: 0x060007B0 RID: 1968 RVA: 0x00030F09 File Offset: 0x0002F109
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressPurchaseItemButton(this, isLeftHand);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x00030F2C File Offset: 0x0002F12C
	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x04000950 RID: 2384
	public string buttonSide;
}
