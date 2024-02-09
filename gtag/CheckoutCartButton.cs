using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class CheckoutCartButton : GorillaPressableButton
{
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	public override void UpdateColor()
	{
		if (this.currentCosmeticItem.itemName == "null")
		{
			this.button.material = this.unpressedMaterial;
			this.buttonText.text = this.noCosmeticText;
			return;
		}
		if (this.isOn)
		{
			this.button.material = this.pressedMaterial;
			this.buttonText.text = this.onText;
			return;
		}
		this.button.material = this.unpressedMaterial;
		this.buttonText.text = this.offText;
	}

	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCheckoutCartButton(this, isLeftHand);
	}

	public CosmeticsController.CosmeticItem currentCosmeticItem;

	public Image currentImage;

	public MeshRenderer button;

	public Material blank;

	public string noCosmeticText;

	public Text buttonText;
}
