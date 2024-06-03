using System;
using System.Linq;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class TryOnBundlesStand : MonoBehaviour
{
	public static string CleanUpTitleDataValues(string titleDataResult)
	{
		string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
		if (text[0] == '"' && text[text.Length - 1] == '"')
		{
			text = text.Substring(1, text.Length - 2);
		}
		return text;
	}

	private void Awake()
	{
		this.SetupButtons();
	}

	private void SetupButtons()
	{
		this.TryOnBundleButtons = new TryOnBundleButton[5];
		this.TryOnBundleButtons[0] = this.TryOnButton1;
		this.TryOnBundleButtons[1] = this.TryOnButton2;
		this.TryOnBundleButtons[2] = this.TryOnButton3;
		this.TryOnBundleButtons[3] = this.TryOnButton4;
		this.TryOnBundleButtons[4] = this.TryOnButton5;
		this.BundleIcons = new Image[5];
		this.BundleIcons[0] = this.BundleIcon1;
		this.BundleIcons[1] = this.BundleIcon2;
		this.BundleIcons[2] = this.BundleIcon3;
		this.BundleIcons[3] = this.BundleIcon4;
		this.BundleIcons[4] = this.BundleIcon5;
	}

	private void InitalizeButtons()
	{
		for (int i = 0; i < this.TryOnBundleButtons.Length; i++)
		{
			if (this.TryOnBundleButtons[i].bundleIndex > -1 && this.TryOnBundleButtons[i].bundleIndex < CosmeticsController.instance.CosmeticBundles.Length && !CosmeticsController.instance.GetItemFromDict(CosmeticsController.instance.CosmeticBundles[this.TryOnBundleButtons[i].bundleIndex].bundlePlayfabItemName).isNullItem)
			{
				this.BundleIcons[i].sprite = CosmeticsController.instance.CosmeticBundles[this.TryOnBundleButtons[i].bundleIndex].bundleIcon;
				this.TryOnBundleButtons[i].UpdateColor();
			}
		}
	}

	private void Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData(this.ComputerDefaultTextTitleDataKey, new Action<string>(this.OnComputerDefaultTextTitleDataSuccess), new Action<PlayFabError>(this.OnComputerDefaultTextTitleDataFailure));
		PlayFabTitleDataCache.Instance.GetTitleData(this.ComputerAlreadyOwnTextTitleDataKey, new Action<string>(this.OnComputerAlreadyOwnTextTitleDataSuccess), new Action<PlayFabError>(this.OnComputerAlreadyOwnTextTitleDataFailure));
		PlayFabTitleDataCache.Instance.GetTitleData(this.PurchaseButtonDefaultTextTitleDataKey, new Action<string>(this.OnPurchaseButtonDefaultTextTitleDataSuccess), new Action<PlayFabError>(this.OnPurchaseButtonDefaultTextTitleDataFailure));
		PlayFabTitleDataCache.Instance.GetTitleData(this.PurchaseButtonAlreadyOwnTextTitleDataKey, new Action<string>(this.OnPurchaseButtonAlreadyOwnTextTitleDataSuccess), new Action<PlayFabError>(this.OnPurchaseButtonAlreadyOwnTextTitleDataFailure));
		this.InitalizeButtons();
	}

	private void OnComputerDefaultTextTitleDataSuccess(string data)
	{
		this.ComputerDefaultTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
	}

	private void OnComputerDefaultTextTitleDataFailure(PlayFabError error)
	{
		this.ComputerDefaultTextTitleDataValue = "Failed to get TD Key : " + this.ComputerDefaultTextTitleDataKey;
		this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
		Debug.LogError(string.Format("Error getting Computer Screen Title Data: {0}", error));
	}

	private void OnComputerAlreadyOwnTextTitleDataSuccess(string data)
	{
		this.ComputerAlreadyOwnTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
	}

	private void OnComputerAlreadyOwnTextTitleDataFailure(PlayFabError error)
	{
		this.ComputerAlreadyOwnTextTitleDataValue = "Failed to get TD Key : " + this.ComputerAlreadyOwnTextTitleDataKey;
		Debug.LogError(string.Format("Error getting Computer Already Screen Title Data: {0}", error));
	}

	private void OnPurchaseButtonDefaultTextTitleDataSuccess(string data)
	{
		this.PurchaseButtonDefaultTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
		this.purchaseButton.UpdateColor();
	}

	private void OnPurchaseButtonDefaultTextTitleDataFailure(PlayFabError error)
	{
		this.PurchaseButtonDefaultTextTitleDataValue = "Failed to get TD Key : " + this.PurchaseButtonDefaultTextTitleDataKey;
		this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
		this.purchaseButton.UpdateColor();
		Debug.LogError(string.Format("Error getting Tryon Purchase Button Default Text Title Data: {0}", error));
	}

	private void OnPurchaseButtonAlreadyOwnTextTitleDataSuccess(string data)
	{
		this.PurchaseButtonAlreadyOwnTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.purchaseButton.AlreadyOwnText = this.PurchaseButtonAlreadyOwnTextTitleDataValue;
	}

	private void OnPurchaseButtonAlreadyOwnTextTitleDataFailure(PlayFabError error)
	{
		this.PurchaseButtonAlreadyOwnTextTitleDataValue = "Failed to get TD Key : " + this.PurchaseButtonAlreadyOwnTextTitleDataKey;
		this.purchaseButton.AlreadyOwnText = this.PurchaseButtonAlreadyOwnTextTitleDataValue;
		Debug.LogError(string.Format("Error getting Tryon Purchase Button Already Own Text Title Data: {0}", error));
	}

	public void ClearSelectedBundle()
	{
		foreach (TryOnBundleButton tryOnBundleButton in this.TryOnBundleButtons)
		{
			tryOnBundleButton.isOn = false;
			if (this.SelectedBundleIndex == tryOnBundleButton.bundleIndex && tryOnBundleButton.bundleIndex != -1)
			{
				string bundlePlayfabItemName = CosmeticsController.instance.CosmeticBundles[tryOnBundleButton.bundleIndex].bundlePlayfabItemName;
				this.RemoveBundle(bundlePlayfabItemName);
				this.SelectedBundleIndex = -1;
				this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
				this.purchaseButton.ResetButton();
				this.selectedBundleImage.sprite = null;
				tryOnBundleButton.UpdateColor();
				break;
			}
		}
		this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
	}

	private void RemoveBundle(string BundleID)
	{
		CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(BundleID);
		if (itemFromDict.isNullItem)
		{
			return;
		}
		foreach (string itemName in itemFromDict.bundledItems)
		{
			CosmeticsController.instance.RemoveCosmeticItemFromSet(CosmeticsController.instance.tryOnSet, itemName, false);
		}
	}

	private void TryOnBundle(string BundleID)
	{
		CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(BundleID);
		if (itemFromDict.isNullItem)
		{
			return;
		}
		foreach (CosmeticsController.CosmeticItem cosmeticItem in CosmeticsController.instance.tryOnSet.items)
		{
			if (!itemFromDict.bundledItems.Contains(cosmeticItem.itemName))
			{
				CosmeticsController.instance.RemoveCosmeticItemFromSet(CosmeticsController.instance.tryOnSet, cosmeticItem.itemName, false);
			}
		}
		foreach (string text in itemFromDict.bundledItems)
		{
			if (!CosmeticsController.instance.tryOnSet.HasItem(text))
			{
				CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, CosmeticsController.instance.GetItemFromDict(text), false, false);
			}
		}
	}

	public void PressTryOnBundleButton(TryOnBundleButton pressedTryOnBundleButton, bool isLeftHand)
	{
		if (pressedTryOnBundleButton.bundleIndex < 0 || pressedTryOnBundleButton.bundleIndex >= CosmeticsController.instance.CosmeticBundles.Length)
		{
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Invalid bundle index");
			return;
		}
		string bundlePlayfabItemName = CosmeticsController.instance.CosmeticBundles[pressedTryOnBundleButton.bundleIndex].bundlePlayfabItemName;
		if (CosmeticsController.instance.GetItemFromDict(bundlePlayfabItemName).isNullItem)
		{
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Bundle is Null + " + bundlePlayfabItemName);
			return;
		}
		if (this.SelectedBundleIndex != pressedTryOnBundleButton.bundleIndex)
		{
			this.ClearSelectedBundle();
		}
		switch (CosmeticsController.instance.CheckIfCosmeticSetMatchesItemSet(CosmeticsController.instance.tryOnSet, bundlePlayfabItemName))
		{
		case CosmeticsController.EWearingCosmeticSet.NotASet:
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Item is Not A Set");
			break;
		case CosmeticsController.EWearingCosmeticSet.NotWearing:
			this.TryOnBundle(bundlePlayfabItemName);
			this.SelectedBundleIndex = pressedTryOnBundleButton.bundleIndex;
			break;
		case CosmeticsController.EWearingCosmeticSet.Partial:
			this.TryOnBundle(bundlePlayfabItemName);
			this.SelectedBundleIndex = pressedTryOnBundleButton.bundleIndex;
			break;
		case CosmeticsController.EWearingCosmeticSet.Complete:
			this.RemoveBundle(bundlePlayfabItemName);
			this.SelectedBundleIndex = -1;
			break;
		}
		if (this.SelectedBundleIndex != -1)
		{
			if (!this.bError)
			{
				this.selectedBundleImage.sprite = CosmeticsController.instance.CosmeticBundles[this.SelectedBundleIndex].bundleIcon;
				pressedTryOnBundleButton.isOn = true;
				this.purchaseButton.offText = CosmeticsController.instance.CosmeticBundles[this.SelectedBundleIndex].purchaseButtonTitleDataValue;
				this.computerScreenText.text = CosmeticsController.instance.CosmeticBundles[this.SelectedBundleIndex].tryOnDescriptionTitleDataValue;
				this.AlreadyOwnCheck();
			}
			pressedTryOnBundleButton.UpdateColor();
		}
		else
		{
			if (!this.bError)
			{
				this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
				this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
			}
			pressedTryOnBundleButton.isOn = false;
			this.selectedBundleImage.sprite = null;
			this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
			this.purchaseButton.ResetButton();
			this.purchaseButton.UpdateColor();
		}
		CosmeticsController.instance.UpdateShoppingCart();
		CosmeticsController.instance.UpdateWornCosmetics(true);
		pressedTryOnBundleButton.UpdateColor();
	}

	public void PurchaseButtonPressed()
	{
		if (this.SelectedBundleIndex == -1)
		{
			return;
		}
		CosmeticsController.instance.PurchaseFromBundleIndex(this.SelectedBundleIndex);
	}

	public void AlreadyOwnCheck()
	{
		if (this.SelectedBundleIndex == -1)
		{
			return;
		}
		if (CosmeticsController.instance.CosmeticBundles[this.SelectedBundleIndex].bAlreadyOwn)
		{
			this.purchaseButton.AlreadyOwn();
			if (!this.bError)
			{
				this.computerScreenText.text = this.ComputerAlreadyOwnTextTitleDataValue;
				return;
			}
		}
		else
		{
			if (!this.bError)
			{
				this.computerScreenText.text = CosmeticsController.instance.CosmeticBundles[this.SelectedBundleIndex].tryOnDescriptionTitleDataValue;
			}
			this.purchaseButton.UpdateColor();
		}
	}

	public void ErrorCompleting()
	{
		this.bError = true;
		this.purchaseButton.ErrorHappened();
		this.computerScreenText.text = this.computerScreeErrorText;
	}

	public TryOnBundlesStand()
	{
	}

	[SerializeField]
	private TryOnBundleButton TryOnButton1;

	[SerializeField]
	private Image BundleIcon1;

	[SerializeField]
	private TryOnBundleButton TryOnButton2;

	[SerializeField]
	private Image BundleIcon2;

	[SerializeField]
	private TryOnBundleButton TryOnButton3;

	[SerializeField]
	private Image BundleIcon3;

	[SerializeField]
	private TryOnBundleButton TryOnButton4;

	[SerializeField]
	private Image BundleIcon4;

	[SerializeField]
	private TryOnBundleButton TryOnButton5;

	[SerializeField]
	private Image BundleIcon5;

	private TryOnBundleButton[] TryOnBundleButtons;

	private Image[] BundleIcons;

	[Header("The Index of the Selected Bundle from CosmeticsBundle Array in CosmeticsController")]
	public int SelectedBundleIndex = -1;

	public TryOnPurchaseButton purchaseButton;

	public Image selectedBundleImage;

	public Text computerScreenText;

	public string ComputerDefaultTextTitleDataKey;

	[SerializeField]
	private string ComputerDefaultTextTitleDataValue;

	public string ComputerAlreadyOwnTextTitleDataKey;

	[SerializeField]
	private string ComputerAlreadyOwnTextTitleDataValue;

	public string PurchaseButtonDefaultTextTitleDataKey;

	[SerializeField]
	private string PurchaseButtonDefaultTextTitleDataValue;

	public string PurchaseButtonAlreadyOwnTextTitleDataKey;

	[SerializeField]
	private string PurchaseButtonAlreadyOwnTextTitleDataValue;

	private bool bError;

	[Header("Error Text for Computer Screen")]
	public string computerScreeErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME, AND MAKE SURE YOU HAVE A STABLE INTERNET CONNECTION. ";
}
