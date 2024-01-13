using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class CheckoutCartButton : GorillaPressableButton
{
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	public Image currentImage;

	public MeshRenderer button;

	public Material blank;

	public string noCosmeticText;

	public Text buttonText;

	public override void Start()
	{
		currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	public override void UpdateColor()
	{
		if (currentCosmeticItem.itemName == "null")
		{
			button.material = unpressedMaterial;
			buttonText.text = noCosmeticText;
		}
		else if (isOn)
		{
			button.material = pressedMaterial;
			buttonText.text = onText;
		}
		else
		{
			button.material = unpressedMaterial;
			buttonText.text = offText;
		}
	}

	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCheckoutCartButton(this, isLeftHand);
	}
}
