using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaNetworking
{
	public class CosmeticsController : MonoBehaviour
	{
		public void AddWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Add(instance);
			this.UpdateWardrobeModelsAndButtons();
		}

		public void RemoveWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Remove(instance);
		}

		public void Awake()
		{
			if (CosmeticsController.instance == null)
			{
				CosmeticsController.instance = this;
			}
			else if (CosmeticsController.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			if (base.gameObject.activeSelf)
			{
				this.catalog = "DLC";
				this.currencyName = "SR";
				this.nullItem = this.allCosmetics[0];
				this.nullItem.isNullItem = true;
				this.allCosmeticsDict[this.nullItem.itemName] = this.nullItem;
				this.allCosmeticsItemIDsfromDisplayNamesDict[this.nullItem.displayName] = this.nullItem.itemName;
				for (int i = 0; i < 11; i++)
				{
					this.tryOnSet.items[i] = this.nullItem;
				}
				this.cosmeticsPages[0] = 0;
				this.cosmeticsPages[1] = 0;
				this.cosmeticsPages[2] = 0;
				this.cosmeticsPages[3] = 0;
				this.itemLists[0] = this.unlockedHats;
				this.itemLists[1] = this.unlockedFaces;
				this.itemLists[2] = this.unlockedBadges;
				this.itemLists[3] = this.unlockedHoldable;
				this.SwitchToStage(CosmeticsController.ATMStages.Unavailable);
				base.StartCoroutine(this.CheckCanGetDaily());
			}
		}

		public void Start()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("BundleData", delegate(string data)
			{
				this.bundleList.FromJson(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting bundle data: {0}", e));
			});
		}

		public void Update()
		{
		}

		private CosmeticsController.CosmeticSlots CategoryToNonTransferrableSlot(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face;
			case CosmeticsController.CosmeticCategory.Skin:
				return CosmeticsController.CosmeticSlots.Skin;
			}
			return CosmeticsController.CosmeticSlots.Count;
		}

		private CosmeticsController.CosmeticSlots DropPositionToCosmeticSlot(BodyDockPositions.DropPositions pos)
		{
			switch (pos)
			{
			case BodyDockPositions.DropPositions.LeftArm:
				return CosmeticsController.CosmeticSlots.ArmLeft;
			case BodyDockPositions.DropPositions.RightArm:
				return CosmeticsController.CosmeticSlots.ArmRight;
			case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
				break;
			case BodyDockPositions.DropPositions.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			default:
				if (pos == BodyDockPositions.DropPositions.LeftBack)
				{
					return CosmeticsController.CosmeticSlots.BackLeft;
				}
				if (pos == BodyDockPositions.DropPositions.RightBack)
				{
					return CosmeticsController.CosmeticSlots.BackRight;
				}
				break;
			}
			return CosmeticsController.CosmeticSlots.Count;
		}

		private static BodyDockPositions.DropPositions CosmeticSlotToDropPosition(CosmeticsController.CosmeticSlots slot)
		{
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.ArmLeft:
				return BodyDockPositions.DropPositions.LeftArm;
			case CosmeticsController.CosmeticSlots.ArmRight:
				return BodyDockPositions.DropPositions.RightArm;
			case CosmeticsController.CosmeticSlots.BackLeft:
				return BodyDockPositions.DropPositions.LeftBack;
			case CosmeticsController.CosmeticSlots.BackRight:
				return BodyDockPositions.DropPositions.RightBack;
			case CosmeticsController.CosmeticSlots.Chest:
				return BodyDockPositions.DropPositions.Chest;
			}
			return BodyDockPositions.DropPositions.None;
		}

		private void SaveItemPreference(CosmeticsController.CosmeticSlots slot, int slotIdx, CosmeticsController.CosmeticItem newItem)
		{
			PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
			PlayerPrefs.Save();
		}

		public void SaveCurrentItemPreferences()
		{
			for (int i = 0; i < 11; i++)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
				this.SaveItemPreference(cosmeticSlots, i, this.currentWornSet.items[i]);
			}
		}

		private void ApplyCosmeticToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, int slotIdx, CosmeticsController.CosmeticSlots slot, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			CosmeticsController.CosmeticItem cosmeticItem = ((set.items[slotIdx].itemName == newItem.itemName) ? this.nullItem : newItem);
			set.items[slotIdx] = cosmeticItem;
			if (applyToPlayerPrefs)
			{
				this.SaveItemPreference(slot, slotIdx, cosmeticItem);
			}
			appliedSlots.Add(slot);
		}

		private void PrivApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			if (newItem.isNullItem)
			{
				return;
			}
			if (CosmeticsController.CosmeticSet.IsHoldable(newItem))
			{
				BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
				foreach (BodyDockPositions.DropPositions dropPositions in dockingResult.positionsDisabled)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = this.DropPositionToCosmeticSlot(dropPositions);
					if (cosmeticSlots != CosmeticsController.CosmeticSlots.Count)
					{
						int num = (int)cosmeticSlots;
						set.items[num] = this.nullItem;
						if (applyToPlayerPrefs)
						{
							this.SaveItemPreference(cosmeticSlots, num, this.nullItem);
						}
					}
				}
				using (List<BodyDockPositions.DropPositions>.Enumerator enumerator = dockingResult.dockedPosition.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BodyDockPositions.DropPositions dropPositions2 = enumerator.Current;
						if (dropPositions2 != BodyDockPositions.DropPositions.None)
						{
							CosmeticsController.CosmeticSlots cosmeticSlots2 = this.DropPositionToCosmeticSlot(dropPositions2);
							int num2 = (int)cosmeticSlots2;
							set.items[num2] = newItem;
							if (applyToPlayerPrefs)
							{
								this.SaveItemPreference(cosmeticSlots2, num2, newItem);
							}
							appliedSlots.Add(cosmeticSlots2);
						}
					}
					return;
				}
			}
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Gloves)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots3 = (isLeftHand ? CosmeticsController.CosmeticSlots.HandLeft : CosmeticsController.CosmeticSlots.HandRight);
				int num3 = (int)cosmeticSlots3;
				this.ApplyCosmeticToSet(set, newItem, num3, cosmeticSlots3, applyToPlayerPrefs, appliedSlots);
				CosmeticsController.CosmeticSlots cosmeticSlots4 = CosmeticsController.CosmeticSet.OppositeSlot(cosmeticSlots3);
				int num4 = (int)cosmeticSlots4;
				if (newItem.bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
				if (set.items[num4].itemName == newItem.itemName)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
				}
				if (set.items[num4].bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
			}
			else
			{
				CosmeticsController.CosmeticSlots cosmeticSlots5 = this.CategoryToNonTransferrableSlot(newItem.itemCategory);
				int num5 = (int)cosmeticSlots5;
				this.ApplyCosmeticToSet(set, newItem, num5, cosmeticSlots5, applyToPlayerPrefs, appliedSlots);
			}
		}

		public List<CosmeticsController.CosmeticSlots> ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs)
		{
			List<CosmeticsController.CosmeticSlots> list = new List<CosmeticsController.CosmeticSlots>(2);
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				foreach (string text in newItem.bundledItems)
				{
					CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(text);
					this.PrivApplyCosmeticItemToSet(set, itemFromDict, isLeftHand, applyToPlayerPrefs, list);
				}
			}
			else
			{
				this.PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, list);
			}
			return list;
		}

		public void RemoveCosmeticItemFromSet(CosmeticsController.CosmeticSet set, string itemName, bool applyToPlayerPrefs)
		{
			this.cachedSet.CopyItems(set);
			for (int i = 0; i < 11; i++)
			{
				if (set.items[i].displayName == itemName)
				{
					set.items[i] = this.nullItem;
					if (applyToPlayerPrefs)
					{
						this.SaveItemPreference((CosmeticsController.CosmeticSlots)i, i, this.nullItem);
					}
				}
			}
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			BodyDockPositions component = offlineVRRig.GetComponent<BodyDockPositions>();
			set.ActivateCosmetics(this.cachedSet, offlineVRRig, component, CosmeticsController.instance.nullItem, offlineVRRig.cosmeticsObjectRegistry);
		}

		public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
		{
			this.ApplyCosmeticItemToSet(this.tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, false);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
		}

		public void PressCosmeticStandButton(CosmeticStand pressedStand)
		{
			this.searchIndex = this.currentCart.IndexOf(pressedStand.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				this.currentCart.RemoveAt(this.searchIndex);
				pressedStand.isOn = false;
				for (int i = 0; i < 11; i++)
				{
					if (pressedStand.thisCosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
			}
			else
			{
				this.currentCart.Insert(0, pressedStand.thisCosmeticItem);
				pressedStand.isOn = true;
				if (this.currentCart.Count > this.fittingRoomButtons.Length)
				{
					foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
					{
						if (!(cosmeticStand == null) && cosmeticStand.thisCosmeticItem.itemName == this.currentCart[this.fittingRoomButtons.Length].itemName)
						{
							cosmeticStand.isOn = false;
							cosmeticStand.UpdateColor();
							break;
						}
					}
					this.currentCart.RemoveAt(this.fittingRoomButtons.Length);
				}
			}
			pressedStand.UpdateColor();
			this.UpdateShoppingCart();
		}

		public void PressWardrobeItemButton(CosmeticsController.CosmeticItem cosmeticItem, bool isLeftHand)
		{
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(cosmeticItem.itemName);
			foreach (CosmeticsController.CosmeticSlots cosmeticSlots in this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true))
			{
				this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
			}
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
		}

		public void PressWardrobeFunctionButton(string function)
		{
			if (!(function == "left"))
			{
				if (!(function == "right"))
				{
					if (!(function == "hat"))
					{
						if (!(function == "face"))
						{
							if (!(function == "badge"))
							{
								if (function == "hand")
								{
									if (this.wardrobeType == 3)
									{
										return;
									}
									this.wardrobeType = 3;
								}
							}
							else
							{
								if (this.wardrobeType == 2)
								{
									return;
								}
								this.wardrobeType = 2;
							}
						}
						else
						{
							if (this.wardrobeType == 1)
							{
								return;
							}
							this.wardrobeType = 1;
						}
					}
					else
					{
						if (this.wardrobeType == 0)
						{
							return;
						}
						this.wardrobeType = 0;
					}
				}
				else
				{
					this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] + 1;
					if (this.cosmeticsPages[this.wardrobeType] > (this.itemLists[this.wardrobeType].Count - 1) / 3)
					{
						this.cosmeticsPages[this.wardrobeType] = 0;
					}
				}
			}
			else
			{
				this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] - 1;
				if (this.cosmeticsPages[this.wardrobeType] < 0)
				{
					this.cosmeticsPages[this.wardrobeType] = (this.itemLists[this.wardrobeType].Count - 1) / 3;
				}
			}
			this.UpdateWardrobeModelsAndButtons();
		}

		public void ClearCheckout()
		{
			this.itemToBuy = this.allCosmetics[0];
			this.checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState(null, false);
		}

		public bool RemoveItemFromCart(CosmeticsController.CosmeticItem cosmeticItem)
		{
			this.searchIndex = this.currentCart.IndexOf(cosmeticItem);
			if (this.searchIndex != -1)
			{
				this.currentCart.RemoveAt(this.searchIndex);
				for (int i = 0; i < 11; i++)
				{
					if (cosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
				return true;
			}
			return false;
		}

		public void PressCheckoutCartButton(CheckoutCartButton pressedCheckoutCartButton, bool isLeftHand)
		{
			if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.tryOnSet.ClearSet(this.nullItem);
				if (this.itemToBuy.displayName == pressedCheckoutCartButton.currentCosmeticItem.displayName)
				{
					this.itemToBuy = this.allCosmetics[0];
					this.checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
				}
				else
				{
					this.itemToBuy = pressedCheckoutCartButton.currentCosmeticItem;
					this.checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
					if (this.itemToBuy.bundledItems != null && this.itemToBuy.bundledItems.Length != 0)
					{
						List<string> list = new List<string>();
						foreach (string text in this.itemToBuy.bundledItems)
						{
							this.tempItem = this.GetItemFromDict(text);
							list.Add(this.tempItem.displayName);
						}
						this.checkoutHeadModel.SetCosmeticActiveArray(list.ToArray(), new bool[list.Count]);
					}
					this.ApplyCosmeticItemToSet(this.tryOnSet, pressedCheckoutCartButton.currentCosmeticItem, isLeftHand, false);
					this.UpdateWornCosmetics(true);
				}
				this.ProcessPurchaseItemState(null, isLeftHand);
				this.UpdateShoppingCart();
			}
		}

		public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
		}

		public void PressEarlyAccessButton()
		{
			this.SwitchToStage(CosmeticsController.ATMStages.Begin);
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState("left", false);
			this.itemToPurchase = this.BundlePlayfabItemName;
			this.shinyRocksCost = (float)this.BundleShinyRocks;
			this.SteamPurchase();
			this.SwitchToStage(CosmeticsController.ATMStages.Purchasing);
		}

		public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
		{
			switch (this.currentPurchaseItemStage)
			{
			case CosmeticsController.PurchaseItemStages.Start:
				this.itemToBuy = this.nullItem;
				this.FormattedPurchaseText("SELECT AN ITEM FROM YOUR CART TO PURCHASE!");
				this.UpdateShoppingCart();
				return;
			case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
				this.searchIndex = this.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.itemToBuy.itemName == x.itemName);
				if (this.searchIndex > -1)
				{
					this.FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!");
					this.leftPurchaseButton.myText.text = "-";
					this.rightPurchaseButton.myText.text = "-";
					this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
					this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
					return;
				}
				if (this.itemToBuy.cost <= this.currencyBalance)
				{
					this.FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?");
					this.leftPurchaseButton.myText.text = "NO!";
					this.rightPurchaseButton.myText.text = "YES!";
					this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
					this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
					return;
				}
				this.FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!");
				this.leftPurchaseButton.myText.text = "-";
				this.rightPurchaseButton.myText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			case CosmeticsController.PurchaseItemStages.ItemSelected:
				if (buttonSide == "right")
				{
					this.FormattedPurchaseText("ARE YOU REALLY SURE?");
					this.leftPurchaseButton.myText.text = "YES! I NEED IT!";
					this.rightPurchaseButton.myText.text = "LET ME THINK ABOUT IT";
					this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
					this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement;
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.ItemOwned:
			case CosmeticsController.PurchaseItemStages.Buying:
				break;
			case CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement:
				if (buttonSide == "left")
				{
					this.FormattedPurchaseText("PURCHASING ITEM...");
					this.leftPurchaseButton.myText.text = "-";
					this.rightPurchaseButton.myText.text = "-";
					this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
					this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
					this.isLastHandTouchedLeft = isLeftHand;
					this.PurchaseItem();
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.Success:
			{
				this.FormattedPurchaseText("SUCCESS! ENJOY YOUR NEW ITEM!");
				VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
				offlineVRRig.concatStringOfCosmeticsAllowed += this.itemToBuy.itemName;
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
				if (itemFromDict.bundledItems != null)
				{
					foreach (string text in itemFromDict.bundledItems)
					{
						VRRig offlineVRRig2 = GorillaTagger.Instance.offlineVRRig;
						offlineVRRig2.concatStringOfCosmeticsAllowed += text;
					}
				}
				this.tryOnSet.ClearSet(this.nullItem);
				this.UpdateShoppingCart();
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics(false);
				this.leftPurchaseButton.myText.text = "-";
				this.rightPurchaseButton.myText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				break;
			}
			case CosmeticsController.PurchaseItemStages.Failure:
				this.FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.");
				this.leftPurchaseButton.myText.text = "-";
				this.rightPurchaseButton.myText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				return;
			default:
				return;
			}
		}

		public void FormattedPurchaseText(string finalLineVar)
		{
			this.finalLine = finalLineVar;
			this.purchaseText.text = string.Concat(new string[]
			{
				"SELECTION: ",
				this.GetItemDisplayName(this.itemToBuy),
				"\nITEM COST: ",
				this.itemToBuy.cost.ToString(),
				"\nYOU HAVE: ",
				this.currencyBalance.ToString(),
				"\n\n",
				this.finalLine
			});
		}

		public void PurchaseItem()
		{
			PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
			{
				ItemId = this.itemToBuy.itemName,
				Price = this.itemToBuy.cost,
				VirtualCurrency = this.currencyName,
				CatalogVersion = this.catalog
			}, delegate(PurchaseItemResult result)
			{
				if (result.Items.Count > 0)
				{
					foreach (ItemInstance itemInstance in result.Items)
					{
						CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
						if (itemFromDict.itemCategory == CosmeticsController.CosmeticCategory.Set)
						{
							this.UnlockItem(itemInstance.ItemId);
							foreach (string text in itemFromDict.bundledItems)
							{
								this.UnlockItem(text);
							}
						}
						else
						{
							this.UnlockItem(itemInstance.ItemId);
						}
					}
					if (PhotonNetwork.InRoom)
					{
						RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
						WebFlags webFlags = new WebFlags(1);
						raiseEventOptions.Flags = webFlags;
						object[] array = new object[0];
						PhotonNetwork.RaiseEvent(9, array, raiseEventOptions, SendOptions.SendReliable);
						base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.itemToBuy.itemName));
					}
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
					this.currencyBalance -= this.itemToBuy.cost;
					this.UpdateShoppingCart();
					this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, delegate(PlayFabError error)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, null, null);
		}

		private void UnlockItem(string itemIdToUnlock)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => itemIdToUnlock == x.itemName);
			if (num > -1)
			{
				if (!this.unlockedCosmetics.Contains(this.allCosmetics[num]))
				{
					this.unlockedCosmetics.Add(this.allCosmetics[num]);
				}
				this.concatStringCosmeticsAllowed += this.allCosmetics[num].itemName;
				switch (this.allCosmetics[num].itemCategory)
				{
				case CosmeticsController.CosmeticCategory.Hat:
					if (!this.unlockedHats.Contains(this.allCosmetics[num]))
					{
						this.unlockedHats.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Badge:
				case CosmeticsController.CosmeticCategory.Skin:
					if (!this.unlockedBadges.Contains(this.allCosmetics[num]))
					{
						this.unlockedBadges.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Face:
					if (!this.unlockedFaces.Contains(this.allCosmetics[num]))
					{
						this.unlockedFaces.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Holdable:
				case CosmeticsController.CosmeticCategory.Gloves:
				case CosmeticsController.CosmeticCategory.Slingshot:
					if (!this.unlockedHoldable.Contains(this.allCosmetics[num]))
					{
						this.unlockedHoldable.Add(this.allCosmetics[num]);
					}
					break;
				case CosmeticsController.CosmeticCategory.Count:
				case CosmeticsController.CosmeticCategory.Set:
					break;
				default:
					return;
				}
			}
		}

		private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
		{
			yield return new WaitForSeconds(1f);
			this.foundCosmetic = false;
			this.attempts = 0;
			while (!this.foundCosmetic && this.attempts < 10 && PhotonNetwork.InRoom)
			{
				this.playerIDList.Clear();
				this.playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
				PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
				{
					Keys = this.playerIDList,
					SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper()
				}, delegate(GetSharedGroupDataResult result)
				{
					this.attempts++;
					foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
					{
						if (keyValuePair.Value.Value.Contains(itemToBuyID))
						{
							PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
							{
								Receivers = ReceiverGroup.Others
							}, SendOptions.SendReliable);
							this.foundCosmetic = true;
						}
					}
					if (this.foundCosmetic)
					{
						this.UpdateWornCosmetics(true);
					}
				}, delegate(PlayFabError error)
				{
					this.attempts++;
					if (error.Error == PlayFabErrorCode.NotAuthenticated)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
						return;
					}
					if (error.Error == PlayFabErrorCode.AccountBanned)
					{
						Application.Quit();
						PhotonNetwork.Disconnect();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
						GameObject[] array = Object.FindObjectsOfType<GameObject>();
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
					}
				}, null, null);
				yield return new WaitForSeconds(1f);
			}
			yield break;
		}

		public void UpdateWardrobeModelsAndButtons()
		{
			foreach (WardrobeInstance wardrobeInstance in this.wardrobes)
			{
				wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 1 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 1] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 2 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 2] : this.nullItem);
				this.iterator = 0;
				while (this.iterator < wardrobeInstance.wardrobeItemButtons.Length)
				{
					CosmeticsController.CosmeticItem currentCosmeticItem = wardrobeInstance.wardrobeItemButtons[this.iterator].currentCosmeticItem;
					wardrobeInstance.wardrobeItemButtons[this.iterator].isOn = !currentCosmeticItem.isNullItem && this.AnyMatch(this.currentWornSet, currentCosmeticItem);
					wardrobeInstance.wardrobeItemButtons[this.iterator].UpdateColor();
					this.iterator++;
				}
				wardrobeInstance.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem.displayName, false);
				wardrobeInstance.selfDoll.SetCosmeticActiveArray(this.currentWornSet.ToDisplayNameArray(), this.currentWornSet.ToOnRightSideArray());
			}
		}

		public void UpdateShoppingCart()
		{
			this.iterator = 0;
			while (this.iterator < this.fittingRoomButtons.Length)
			{
				if (this.iterator < this.currentCart.Count)
				{
					this.fittingRoomButtons[this.iterator].currentCosmeticItem = this.currentCart[this.iterator];
					this.checkoutCartButtons[this.iterator].currentCosmeticItem = this.currentCart[this.iterator];
					this.checkoutCartButtons[this.iterator].isOn = this.checkoutCartButtons[this.iterator].currentCosmeticItem.itemName == this.itemToBuy.itemName;
					this.fittingRoomButtons[this.iterator].isOn = this.AnyMatch(this.tryOnSet, this.fittingRoomButtons[this.iterator].currentCosmeticItem);
				}
				else
				{
					this.checkoutCartButtons[this.iterator].currentCosmeticItem = this.nullItem;
					this.fittingRoomButtons[this.iterator].currentCosmeticItem = this.nullItem;
					this.checkoutCartButtons[this.iterator].isOn = false;
					this.fittingRoomButtons[this.iterator].isOn = false;
				}
				this.checkoutCartButtons[this.iterator].currentImage.sprite = this.checkoutCartButtons[this.iterator].currentCosmeticItem.itemPicture;
				this.fittingRoomButtons[this.iterator].currentImage.sprite = this.fittingRoomButtons[this.iterator].currentCosmeticItem.itemPicture;
				this.checkoutCartButtons[this.iterator].UpdateColor();
				this.fittingRoomButtons[this.iterator].UpdateColor();
				this.iterator++;
			}
			this.UpdateWardrobeModelsAndButtons();
		}

		public void UpdateWornCosmetics(bool sync = false)
		{
			GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(this.currentWornSet, this.tryOnSet);
			if (sync && GorillaTagger.Instance.myVRRig != null)
			{
				string[] array = this.currentWornSet.ToDisplayNameArray();
				string[] array2 = this.tryOnSet.ToDisplayNameArray();
				GorillaTagger.Instance.myVRRig.RPC("UpdateCosmeticsWithTryon", RpcTarget.All, new object[] { array, array2 });
			}
		}

		public CosmeticsController.CosmeticItem GetItemFromDict(string itemID)
		{
			if (!this.allCosmeticsDict.TryGetValue(itemID, out this.cosmeticItemVar))
			{
				return this.nullItem;
			}
			return this.cosmeticItemVar;
		}

		public string GetItemNameFromDisplayName(string displayName)
		{
			if (!this.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, out this.returnString))
			{
				return "null";
			}
			return this.returnString;
		}

		public bool AnyMatch(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem item)
		{
			if (item.itemCategory != CosmeticsController.CosmeticCategory.Set)
			{
				return set.IsActive(item.displayName);
			}
			if (item.bundledItems.Length == 1)
			{
				return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0]));
			}
			if (item.bundledItems.Length == 2)
			{
				return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1]));
			}
			return item.bundledItems.Length >= 3 && (this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[2])));
		}

		public void Initialize()
		{
			if (base.gameObject.activeSelf)
			{
				this.GetUserCosmeticsAllowed();
			}
		}

		public void GetLastDailyLogin()
		{
			PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest(), delegate(GetUserDataResult result)
			{
				if (result.Data.TryGetValue("DailyLogin", out this.userDataRecord))
				{
					this.lastDailyLogin = this.userDataRecord.Value;
					return;
				}
				this.lastDailyLogin = "NONE";
				base.StartCoroutine(this.GetMyDaily());
			}, delegate(PlayFabError error)
			{
				this.lastDailyLogin = "FAILED";
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					PhotonNetwork.Disconnect();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		private IEnumerator CheckCanGetDaily()
		{
			for (;;)
			{
				if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
				{
					this.currentTime = new DateTime((GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) * 10000L);
					this.secondsUntilTomorrow = (int)(this.currentTime.AddDays(1.0).Date - this.currentTime).TotalSeconds;
					if (this.lastDailyLogin == null || this.lastDailyLogin == "")
					{
						this.GetLastDailyLogin();
					}
					else if (this.currentTime.ToString("o").Substring(0, 10) == this.lastDailyLogin)
					{
						this.checkedDaily = true;
						this.gotMyDaily = true;
					}
					else if (this.currentTime.ToString("o").Substring(0, 10) != this.lastDailyLogin)
					{
						this.checkedDaily = true;
						this.gotMyDaily = false;
						base.StartCoroutine(this.GetMyDaily());
					}
					else if (this.lastDailyLogin == "FAILED")
					{
						this.GetLastDailyLogin();
					}
					this.secondsToWaitToCheckDaily = (this.checkedDaily ? 60f : 10f);
					this.UpdateCurrencyBoard();
					yield return new WaitForSeconds(this.secondsToWaitToCheckDaily);
				}
				else
				{
					yield return new WaitForSeconds(1f);
				}
			}
			yield break;
		}

		private IEnumerator GetMyDaily()
		{
			yield return new WaitForSeconds(10f);
			ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
			executeCloudScriptRequest.FunctionName = "TryDistributeCurrency";
			executeCloudScriptRequest.FunctionParameter = new { };
			PlayFabClientAPI.ExecuteCloudScript(executeCloudScriptRequest, delegate(ExecuteCloudScriptResult result)
			{
				this.GetCurrencyBalance();
				this.GetLastDailyLogin();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					PhotonNetwork.Disconnect();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
			yield break;
		}

		public void GetUserCosmeticsAllowed()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
				{
					CatalogVersion = this.catalog
				}, delegate(GetCatalogItemsResult result2)
				{
					this.unlockedCosmetics.Clear();
					this.unlockedHats.Clear();
					this.unlockedBadges.Clear();
					this.unlockedFaces.Clear();
					this.unlockedHoldable.Clear();
					this.catalogItems = result2.Catalog;
					using (List<CatalogItem>.Enumerator enumerator = this.catalogItems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CatalogItem catalogItem = enumerator.Current;
							this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => catalogItem.DisplayName == x.displayName);
							if (this.searchIndex > -1)
							{
								this.tempStringArray = null;
								this.hasPrice = false;
								if (catalogItem.Bundle != null)
								{
									this.tempStringArray = catalogItem.Bundle.BundledItems.ToArray();
								}
								uint num;
								if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out num))
								{
									this.hasPrice = true;
								}
								this.allCosmetics[this.searchIndex] = new CosmeticsController.CosmeticItem
								{
									itemName = catalogItem.ItemId,
									displayName = catalogItem.DisplayName,
									cost = (int)num,
									itemPicture = this.allCosmetics[this.searchIndex].itemPicture,
									itemPictureResourceString = this.allCosmetics[this.searchIndex].itemPictureResourceString,
									itemCategory = this.allCosmetics[this.searchIndex].itemCategory,
									bundledItems = this.tempStringArray,
									canTryOn = this.hasPrice,
									bothHandsHoldable = this.allCosmetics[this.searchIndex].bothHandsHoldable,
									overrideDisplayName = this.allCosmetics[this.searchIndex].overrideDisplayName,
									bLoadsFromResources = this.allCosmetics[this.searchIndex].bLoadsFromResources,
									bUsesMeshAtlas = this.allCosmetics[this.searchIndex].bUsesMeshAtlas,
									rotationOffset = this.allCosmetics[this.searchIndex].rotationOffset,
									positionOffset = this.allCosmetics[this.searchIndex].positionOffset,
									meshAtlasResourceString = this.allCosmetics[this.searchIndex].meshAtlasResourceString,
									meshResourceString = this.allCosmetics[this.searchIndex].meshResourceString,
									materialResourceString = this.allCosmetics[this.searchIndex].materialResourceString
								};
								this.allCosmeticsDict[this.allCosmetics[this.searchIndex].itemName] = this.allCosmetics[this.searchIndex];
								this.allCosmeticsItemIDsfromDisplayNamesDict[this.allCosmetics[this.searchIndex].displayName] = this.allCosmetics[this.searchIndex].itemName;
							}
						}
					}
					for (int i = this.allCosmetics.Count - 1; i > -1; i--)
					{
						this.tempItem = this.allCosmetics[i];
						if (this.tempItem.itemCategory == CosmeticsController.CosmeticCategory.Set && this.tempItem.canTryOn)
						{
							string[] bundledItems = this.tempItem.bundledItems;
							for (int j = 0; j < bundledItems.Length; j++)
							{
								string setItemName = bundledItems[j];
								this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = new CosmeticsController.CosmeticItem
									{
										itemName = this.allCosmetics[this.searchIndex].itemName,
										displayName = this.allCosmetics[this.searchIndex].displayName,
										cost = this.allCosmetics[this.searchIndex].cost,
										itemPicture = this.allCosmetics[this.searchIndex].itemPicture,
										itemCategory = this.allCosmetics[this.searchIndex].itemCategory,
										overrideDisplayName = this.allCosmetics[this.searchIndex].overrideDisplayName,
										bothHandsHoldable = this.allCosmetics[this.searchIndex].bothHandsHoldable,
										canTryOn = true
									};
									this.allCosmetics[this.searchIndex] = this.tempItem;
									this.allCosmeticsDict[this.allCosmetics[this.searchIndex].itemName] = this.allCosmetics[this.searchIndex];
									this.allCosmeticsItemIDsfromDisplayNamesDict[this.allCosmetics[this.searchIndex].displayName] = this.allCosmetics[this.searchIndex].itemName;
								}
							}
						}
					}
					using (List<ItemInstance>.Enumerator enumerator2 = result.Inventory.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ItemInstance item = enumerator2.Current;
							if (item.ItemId == "Early Access Supporter Pack")
							{
								this.unlockedCosmetics.Add(this.allCosmetics[1]);
								this.unlockedCosmetics.Add(this.allCosmetics[10]);
								this.unlockedCosmetics.Add(this.allCosmetics[11]);
								this.unlockedCosmetics.Add(this.allCosmetics[12]);
								this.unlockedCosmetics.Add(this.allCosmetics[13]);
								this.unlockedCosmetics.Add(this.allCosmetics[14]);
								this.unlockedCosmetics.Add(this.allCosmetics[15]);
								this.unlockedCosmetics.Add(this.allCosmetics[31]);
								this.unlockedCosmetics.Add(this.allCosmetics[32]);
								this.unlockedCosmetics.Add(this.allCosmetics[38]);
								this.unlockedCosmetics.Add(this.allCosmetics[67]);
								this.unlockedCosmetics.Add(this.allCosmetics[68]);
							}
							else
							{
								if (item.ItemId == this.BundlePlayfabItemName)
								{
									foreach (EarlyAccessButton earlyAccessButton in this.earlyAccessButtons)
									{
										this.AlreadyOwnAllBundleButtons();
									}
								}
								this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.ItemId == x.itemName);
								if (this.searchIndex > -1)
								{
									this.unlockedCosmetics.Add(this.allCosmetics[this.searchIndex]);
								}
							}
						}
					}
					this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => "Slingshot" == x.itemName);
					this.allCosmeticsDict["Slingshot"] = this.allCosmetics[this.searchIndex];
					this.allCosmeticsItemIDsfromDisplayNamesDict[this.allCosmetics[this.searchIndex].displayName] = this.allCosmetics[this.searchIndex].itemName;
					foreach (CosmeticsController.CosmeticItem cosmeticItem in this.unlockedCosmetics)
					{
						if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Hat && !this.unlockedHats.Contains(cosmeticItem))
						{
							this.unlockedHats.Add(cosmeticItem);
						}
						else if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Face && !this.unlockedFaces.Contains(cosmeticItem))
						{
							this.unlockedFaces.Add(cosmeticItem);
						}
						else if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Badge && !this.unlockedBadges.Contains(cosmeticItem))
						{
							this.unlockedBadges.Add(cosmeticItem);
						}
						else if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Skin && !this.unlockedBadges.Contains(cosmeticItem))
						{
							this.unlockedBadges.Add(cosmeticItem);
						}
						else if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Holdable && !this.unlockedHoldable.Contains(cosmeticItem))
						{
							this.unlockedHoldable.Add(cosmeticItem);
						}
						else if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Gloves && !this.unlockedHoldable.Contains(cosmeticItem))
						{
							this.unlockedHoldable.Add(cosmeticItem);
						}
						else if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Slingshot && !this.unlockedHoldable.Contains(cosmeticItem))
						{
							this.unlockedHoldable.Add(cosmeticItem);
						}
						this.concatStringCosmeticsAllowed += cosmeticItem.itemName;
					}
					foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
					{
						if (cosmeticStand != null)
						{
							cosmeticStand.InitializeCosmetic();
						}
					}
					this.currencyBalance = result.VirtualCurrency[this.currencyName];
					int num2;
					this.playedInBeta = result.VirtualCurrency.TryGetValue("TC", out num2) && num2 > 0;
					this.currentWornSet.LoadFromPlayerPreferences(this);
					this.SwitchToStage(CosmeticsController.ATMStages.Begin);
					this.ProcessPurchaseItemState(null, false);
					this.UpdateShoppingCart();
					this.UpdateWornCosmetics(false);
					this.UpdateCurrencyBoard();
				}, delegate(PlayFabError error)
				{
					if (error.Error == PlayFabErrorCode.NotAuthenticated)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
					else if (error.Error == PlayFabErrorCode.AccountBanned)
					{
						Application.Quit();
						PhotonNetwork.Disconnect();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
						GameObject[] array3 = Object.FindObjectsOfType<GameObject>();
						for (int k = 0; k < array3.Length; k++)
						{
							Object.Destroy(array3[k]);
						}
					}
					if (!this.tryTwice)
					{
						this.tryTwice = true;
						this.GetUserCosmeticsAllowed();
					}
				}, null, null);
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}
				else if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					PhotonNetwork.Disconnect();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array4 = Object.FindObjectsOfType<GameObject>();
					for (int l = 0; l < array4.Length; l++)
					{
						Object.Destroy(array4[l]);
					}
				}
				if (!this.tryTwice)
				{
					this.tryTwice = true;
					this.GetUserCosmeticsAllowed();
				}
			}, null, null);
		}

		private void SteamPurchase()
		{
			Debug.Log("attempting to purchase item through steam");
			StartPurchaseRequest startPurchaseRequest = new StartPurchaseRequest();
			startPurchaseRequest.CatalogVersion = this.catalog;
			startPurchaseRequest.Items = new List<ItemPurchaseRequest>
			{
				new ItemPurchaseRequest
				{
					ItemId = this.itemToPurchase,
					Quantity = 1U,
					Annotation = "Purchased via in-game store"
				}
			};
			PlayFabClientAPI.StartPurchase(startPurchaseRequest, delegate(StartPurchaseResult result)
			{
				Debug.Log("successfully started purchase. attempted to pay for purchase through steam");
				this.currentPurchaseID = result.OrderId;
				PlayFabClientAPI.PayForPurchase(new PayForPurchaseRequest
				{
					OrderId = this.currentPurchaseID,
					ProviderName = "Steam",
					Currency = "RM"
				}, delegate(PayForPurchaseResult result2)
				{
					Debug.Log("succeeded on sending request for paying with steam! waiting for response");
					this.buyingBundle = true;
					this.m_MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.OnMicroTxnAuthorizationResponse));
				}, delegate(PlayFabError error)
				{
					if (error.Error == PlayFabErrorCode.NotAuthenticated)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
					else if (error.Error == PlayFabErrorCode.AccountBanned)
					{
						Application.Quit();
						PhotonNetwork.Disconnect();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
						GameObject[] array = Object.FindObjectsOfType<GameObject>();
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
					}
					Debug.Log("failed to send request to purchase with steam!");
					Debug.Log(error.ToString());
					this.SwitchToStage(CosmeticsController.ATMStages.Failure);
				}, null, null);
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}
				else if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					PhotonNetwork.Disconnect();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array2 = Object.FindObjectsOfType<GameObject>();
					for (int j = 0; j < array2.Length; j++)
					{
						Object.Destroy(array2[j]);
					}
				}
				Debug.Log("error in starting purchase!");
			}, null, null);
		}

		public void ProcessATMState(string currencyButton)
		{
			switch (this.currentATMStage)
			{
			case CosmeticsController.ATMStages.Unavailable:
			case CosmeticsController.ATMStages.Purchasing:
				break;
			case CosmeticsController.ATMStages.Begin:
				this.SwitchToStage(CosmeticsController.ATMStages.Menu);
				return;
			case CosmeticsController.ATMStages.Menu:
				if (currencyButton == "one")
				{
					this.SwitchToStage(CosmeticsController.ATMStages.Balance);
					return;
				}
				if (currencyButton == "two")
				{
					this.SwitchToStage(CosmeticsController.ATMStages.Choose);
					return;
				}
				if (!(currencyButton == "four"))
				{
					return;
				}
				this.SwitchToStage(CosmeticsController.ATMStages.Begin);
				return;
			case CosmeticsController.ATMStages.Balance:
				if (currencyButton == "four")
				{
					this.SwitchToStage(CosmeticsController.ATMStages.Menu);
					return;
				}
				break;
			case CosmeticsController.ATMStages.Choose:
				if (currencyButton == "one")
				{
					this.numShinyRocksToBuy = 1000;
					this.shinyRocksCost = 4.99f;
					this.itemToPurchase = "1000SHINYROCKS";
					this.SwitchToStage(CosmeticsController.ATMStages.Confirm);
					return;
				}
				if (currencyButton == "two")
				{
					this.numShinyRocksToBuy = 2200;
					this.shinyRocksCost = 9.99f;
					this.itemToPurchase = "2200SHINYROCKS";
					this.SwitchToStage(CosmeticsController.ATMStages.Confirm);
					return;
				}
				if (currencyButton == "three")
				{
					this.numShinyRocksToBuy = 5000;
					this.shinyRocksCost = 19.99f;
					this.itemToPurchase = "5000SHINYROCKS";
					this.SwitchToStage(CosmeticsController.ATMStages.Confirm);
					return;
				}
				if (!(currencyButton == "four"))
				{
					return;
				}
				this.SwitchToStage(CosmeticsController.ATMStages.Menu);
				return;
			case CosmeticsController.ATMStages.Confirm:
				if (currencyButton == "one")
				{
					this.SteamPurchase();
					this.SwitchToStage(CosmeticsController.ATMStages.Purchasing);
					return;
				}
				if (!(currencyButton == "four"))
				{
					return;
				}
				this.SwitchToStage(CosmeticsController.ATMStages.Choose);
				return;
			default:
				this.SwitchToStage(CosmeticsController.ATMStages.Menu);
				break;
			}
		}

		public void SwitchToStage(CosmeticsController.ATMStages newStage)
		{
			this.currentATMStage = newStage;
			switch (newStage)
			{
			case CosmeticsController.ATMStages.Unavailable:
				this.atmText.text = "ATM NOT AVAILABLE! PLEASE TRY AGAIN LATER!";
				this.atmButtonsText.text = "";
				return;
			case CosmeticsController.ATMStages.Begin:
				this.atmText.text = "WELCOME! PRESS ANY BUTTON TO BEGIN.";
				this.atmButtonsText.text = "\n\n\n\n\n\n\n\n\nBEGIN   -->";
				return;
			case CosmeticsController.ATMStages.Menu:
				this.atmText.text = "CHECK YOUR BALANCE OR PURCHASE MORE SHINY ROCKS.";
				this.atmButtonsText.text = "BALANCE-- >\n\n\nPURCHASE-->\n\n\n\n\n\nBACK    -->";
				return;
			case CosmeticsController.ATMStages.Balance:
				this.atmText.text = "CURRENT BALANCE:\n\n" + this.currencyBalance.ToString();
				this.atmButtonsText.text = "\n\n\n\n\n\n\n\n\nBACK    -->";
				return;
			case CosmeticsController.ATMStages.Choose:
				this.atmText.text = "CHOOSE AN AMOUNT OF SHINY ROCKS TO PURCHASE.";
				this.atmButtonsText.text = "$4.99 FOR -->\n1000\n\n$9.99 FOR -->\n2200\n\n$19.99 FOR-->\n5000\n\nBACK -->";
				return;
			case CosmeticsController.ATMStages.Confirm:
				this.atmText.text = string.Concat(new string[]
				{
					"YOU HAVE CHOSEN TO PURCHASE ",
					this.numShinyRocksToBuy.ToString(),
					" SHINY ROCKS FOR $",
					this.shinyRocksCost.ToString(),
					". CONFIRM TO LAUNCH A STEAM WINDOW TO COMPLETE YOUR PURCHASE."
				});
				this.atmButtonsText.text = "CONFIRM -->\n\n\n\n\n\n\n\n\nBACK    -->";
				return;
			case CosmeticsController.ATMStages.Purchasing:
				this.atmText.text = "PURCHASING IN STEAM...";
				this.atmButtonsText.text = "";
				return;
			case CosmeticsController.ATMStages.Success:
				this.atmText.text = "SUCCESS! NEW SHINY ROCKS BALANCE: " + (this.currencyBalance + this.numShinyRocksToBuy).ToString();
				this.atmButtonsText.text = "\n\n\n\n\n\n\n\n\nRETURN  -->";
				return;
			case CosmeticsController.ATMStages.Failure:
				this.atmText.text = "PURCHASE CANCELED. NO FUNDS WERE SPENT.";
				this.atmButtonsText.text = "\n\n\n\n\n\n\n\n\nRETURN  -->";
				return;
			case CosmeticsController.ATMStages.Locked:
				this.atmText.text = "UNABLE TO PURCHASE AT THIS TIME. PLEASE RESTART THE GAME OR TRY AGAIN LATER.";
				this.atmButtonsText.text = "\n\n\n\n\n\n\n\n\nRETURN  -->";
				return;
			default:
				return;
			}
		}

		public void PressCurrencyPurchaseButton(string currencyPurchaseSize)
		{
			this.ProcessATMState(currencyPurchaseSize);
		}

		private void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t pCallback)
		{
			PlayFabClientAPI.ConfirmPurchase(new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID
			}, delegate(ConfirmPurchaseResult result)
			{
				if (this.buyingBundle)
				{
					this.buyingBundle = false;
					if (PhotonNetwork.InRoom)
					{
						RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
						WebFlags webFlags = new WebFlags(1);
						raiseEventOptions.Flags = webFlags;
						object[] array = new object[0];
						PhotonNetwork.RaiseEvent(9, array, raiseEventOptions, SendOptions.SendReliable);
					}
					base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.BundlePlayfabItemName));
				}
				this.SwitchToStage(CosmeticsController.ATMStages.Success);
				this.GetCurrencyBalance();
				this.UpdateCurrencyBoard();
				this.GetUserCosmeticsAllowed();
				GorillaTagger.Instance.offlineVRRig.GetUserCosmeticsAllowed();
			}, delegate(PlayFabError error)
			{
				this.atmText.text = "PURCHASE CANCELLED!\n\nCURRENT BALANCE IS: ";
				this.UpdateCurrencyBoard();
				this.SwitchToStage(CosmeticsController.ATMStages.Failure);
			}, null, null);
		}

		public void UpdateCurrencyBoard()
		{
			this.FormattedPurchaseText(this.finalLine);
			this.dailyText.text = (this.checkedDaily ? (this.gotMyDaily ? "SUCCESSFULLY GOT DAILY ROCKS!" : "WAITING TO GET DAILY ROCKS...") : "CHECKING DAILY ROCKS...");
			this.currencyBoardText.text = string.Concat(new string[]
			{
				this.currencyBalance.ToString(),
				"\n\n",
				(this.secondsUntilTomorrow / 3600).ToString(),
				" HR, ",
				(this.secondsUntilTomorrow % 3600 / 60).ToString(),
				"MIN"
			});
		}

		public void GetCurrencyBalance()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				this.currencyBalance = result.VirtualCurrency[this.currencyName];
				this.UpdateCurrencyBoard();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					PhotonNetwork.Disconnect();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		public string GetItemDisplayName(CosmeticsController.CosmeticItem item)
		{
			if (item.overrideDisplayName != null && item.overrideDisplayName != "")
			{
				return item.overrideDisplayName;
			}
			return item.displayName;
		}

		public void LeaveSystemMenu()
		{
		}

		private void AlreadyOwnAllBundleButtons()
		{
			EarlyAccessButton[] array = this.earlyAccessButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AlreadyOwn();
			}
		}

		public static int maximumTransferrableItems = 5;

		[OnEnterPlay_SetNull]
		public static volatile CosmeticsController instance;

		public List<CosmeticsController.CosmeticItem> allCosmetics;

		public Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict = new Dictionary<string, CosmeticsController.CosmeticItem>();

		public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>();

		public CosmeticsController.CosmeticItem nullItem;

		public string catalog;

		private string[] tempStringArray;

		private CosmeticsController.CosmeticItem tempItem;

		public List<CatalogItem> catalogItems;

		public bool tryTwice;

		[NonSerialized]
		public CosmeticsController.CosmeticSet tryOnSet = new CosmeticsController.CosmeticSet();

		public FittingRoomButton[] fittingRoomButtons;

		public CosmeticStand[] cosmeticStands;

		public List<CosmeticsController.CosmeticItem> currentCart = new List<CosmeticsController.CosmeticItem>();

		public CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

		public CheckoutCartButton[] checkoutCartButtons;

		public PurchaseItemButton leftPurchaseButton;

		public PurchaseItemButton rightPurchaseButton;

		public Text purchaseText;

		public CosmeticsController.CosmeticItem itemToBuy;

		public HeadModel checkoutHeadModel;

		private List<string> playerIDList = new List<string>();

		private bool foundCosmetic;

		private int attempts;

		private string finalLine;

		private bool purchaseLocked;

		private bool isLastHandTouchedLeft;

		private CosmeticsController.CosmeticSet cachedSet = new CosmeticsController.CosmeticSet();

		private List<WardrobeInstance> wardrobes = new List<WardrobeInstance>();

		public List<CosmeticsController.CosmeticItem> unlockedCosmetics = new List<CosmeticsController.CosmeticItem>();

		public List<CosmeticsController.CosmeticItem> unlockedHats = new List<CosmeticsController.CosmeticItem>();

		public List<CosmeticsController.CosmeticItem> unlockedFaces = new List<CosmeticsController.CosmeticItem>();

		public List<CosmeticsController.CosmeticItem> unlockedBadges = new List<CosmeticsController.CosmeticItem>();

		public List<CosmeticsController.CosmeticItem> unlockedHoldable = new List<CosmeticsController.CosmeticItem>();

		public int[] cosmeticsPages = new int[4];

		private List<CosmeticsController.CosmeticItem>[] itemLists = new List<CosmeticsController.CosmeticItem>[4];

		private int wardrobeType;

		[NonSerialized]
		public CosmeticsController.CosmeticSet currentWornSet = new CosmeticsController.CosmeticSet();

		public string concatStringCosmeticsAllowed = "";

		public Text atmText;

		public string currentAtmString;

		public Text infoText;

		public Text earlyAccessText;

		public Text[] purchaseButtonText;

		public Text dailyText;

		public CosmeticsController.ATMStages currentATMStage;

		public Text atmButtonsText;

		public int currencyBalance;

		public string currencyName;

		public PurchaseCurrencyButton[] purchaseCurrencyButtons;

		public Text currencyBoardText;

		public Text currencyBoxText;

		public string startingCurrencyBoxTextString;

		public string successfulCurrencyPurchaseTextString;

		public int numShinyRocksToBuy;

		public float shinyRocksCost;

		public string itemToPurchase;

		public bool confirmedDidntPlayInBeta;

		public bool playedInBeta;

		public bool gotMyDaily;

		public bool checkedDaily;

		public string currentPurchaseID;

		public bool hasPrice;

		private int searchIndex;

		private int iterator;

		private CosmeticsController.CosmeticItem cosmeticItemVar;

		public EarlyAccessButton[] earlyAccessButtons;

		private BundleList bundleList = new BundleList();

		public string BundleSkuName = "2024_i_lava_you_pack";

		public string BundlePlayfabItemName = "LSABG.";

		public int BundleShinyRocks = 10000;

		public bool buyingBundle;

		public DateTime currentTime;

		public string lastDailyLogin;

		public UserDataRecord userDataRecord;

		public int secondsUntilTomorrow;

		public float secondsToWaitToCheckDaily = 10f;

		private string returnString;

		protected Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse;

		public enum PurchaseItemStages
		{
			Start,
			CheckoutButtonPressed,
			ItemSelected,
			ItemOwned,
			FinalPurchaseAcknowledgement,
			Buying,
			Success,
			Failure
		}

		public enum ATMStages
		{
			Unavailable,
			Begin,
			Menu,
			Balance,
			Choose,
			Confirm,
			Purchasing,
			Success,
			Failure,
			Locked
		}

		public enum CosmeticCategory
		{
			None,
			Hat,
			Badge,
			Face,
			Holdable,
			Gloves,
			Slingshot,
			Skin,
			Count,
			Set
		}

		public enum CosmeticSlots
		{
			Hat,
			Badge,
			Face,
			ArmLeft,
			ArmRight,
			BackLeft,
			BackRight,
			HandLeft,
			HandRight,
			Chest,
			Skin,
			Count
		}

		[Serializable]
		public class CosmeticSet
		{
			public event CosmeticsController.CosmeticSet.OnSetActivatedHandler onSetActivatedEvent;

			protected void OnSetActivated(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer)
			{
				if (this.onSetActivatedEvent != null)
				{
					this.onSetActivatedEvent(prevSet, currentSet, netPlayer);
				}
			}

			public CosmeticSet()
			{
				this.items = new CosmeticsController.CosmeticItem[11];
			}

			public CosmeticSet(string[] itemNames, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[11];
				for (int i = 0; i < itemNames.Length; i++)
				{
					string text = itemNames[i];
					string itemNameFromDisplayName = controller.GetItemNameFromDisplayName(text);
					this.items[i] = controller.GetItemFromDict(itemNameFromDisplayName);
				}
			}

			public void CopyItems(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					this.items[i] = other.items[i];
				}
			}

			public void MergeSets(CosmeticsController.CosmeticSet tryOn, CosmeticsController.CosmeticSet current)
			{
				for (int i = 0; i < 11; i++)
				{
					if (tryOn == null)
					{
						this.items[i] = current.items[i];
					}
					else
					{
						this.items[i] = (tryOn.items[i].isNullItem ? current.items[i] : tryOn.items[i]);
					}
				}
			}

			public void ClearSet(CosmeticsController.CosmeticItem nullItem)
			{
				for (int i = 0; i < 11; i++)
				{
					this.items[i] = nullItem;
				}
			}

			public bool IsActive(string name)
			{
				int num = 11;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			public bool HasItemOfCategory(CosmeticsController.CosmeticCategory category)
			{
				int num = 11;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemCategory == category)
					{
						return true;
					}
				}
				return false;
			}

			public bool HasItem(string name)
			{
				int num = 11;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			public static bool IsSlotLeftHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.HandLeft;
			}

			public static bool IsSlotRightHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.HandRight;
			}

			public static bool IsHoldable(CosmeticsController.CosmeticItem item)
			{
				return item.itemCategory == CosmeticsController.CosmeticCategory.Holdable || item.itemCategory == CosmeticsController.CosmeticCategory.Slingshot;
			}

			public static bool IsSlotHoldable(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.Chest;
			}

			public static CosmeticsController.CosmeticSlots OppositeSlot(CosmeticsController.CosmeticSlots slot)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Hat:
					return CosmeticsController.CosmeticSlots.Hat;
				case CosmeticsController.CosmeticSlots.Badge:
					return CosmeticsController.CosmeticSlots.Badge;
				case CosmeticsController.CosmeticSlots.Face:
					return CosmeticsController.CosmeticSlots.Face;
				case CosmeticsController.CosmeticSlots.ArmLeft:
					return CosmeticsController.CosmeticSlots.ArmRight;
				case CosmeticsController.CosmeticSlots.ArmRight:
					return CosmeticsController.CosmeticSlots.ArmLeft;
				case CosmeticsController.CosmeticSlots.BackLeft:
					return CosmeticsController.CosmeticSlots.BackRight;
				case CosmeticsController.CosmeticSlots.BackRight:
					return CosmeticsController.CosmeticSlots.BackLeft;
				case CosmeticsController.CosmeticSlots.HandLeft:
					return CosmeticsController.CosmeticSlots.HandRight;
				case CosmeticsController.CosmeticSlots.HandRight:
					return CosmeticsController.CosmeticSlots.HandLeft;
				case CosmeticsController.CosmeticSlots.Chest:
					return CosmeticsController.CosmeticSlots.Chest;
				case CosmeticsController.CosmeticSlots.Skin:
					return CosmeticsController.CosmeticSlots.Skin;
				default:
					return CosmeticsController.CosmeticSlots.Count;
				}
			}

			public static string SlotPlayerPreferenceName(CosmeticsController.CosmeticSlots slot)
			{
				return "slot_" + slot.ToString();
			}

			private void ActivateHoldable(int cosmeticIdx, BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem)
			{
				BodyDockPositions.DropPositions dropPositions = CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)cosmeticIdx);
				CosmeticsController.CosmeticItem cosmeticItem = this.items[cosmeticIdx];
				if (cosmeticItem.isNullItem)
				{
					bDock.TransferrableItemDisableAtPosition(dropPositions);
					return;
				}
				if (bDock.ItemPositionInUse(dropPositions) == null)
				{
					bDock.TransferrableItemEnableAtPosition(cosmeticItem.displayName, dropPositions);
					return;
				}
				if (!bDock.TransferrableItemActiveAtPos(cosmeticItem.displayName, dropPositions))
				{
					bDock.TransferrableItemDisableAtPosition(dropPositions);
					bDock.TransferrableItemEnableAtPosition(cosmeticItem.displayName, dropPositions);
				}
			}

			private void ActivateCosmeticItem(CosmeticsController.CosmeticSet prevSet, VRRig rig, int cosmeticIdx, CosmeticItemRegistry cosmeticsObjectRegistry, CosmeticsController.CosmeticItem nullItem)
			{
				CosmeticsController.CosmeticItem cosmeticItem = prevSet.items[cosmeticIdx];
				CosmeticsController.CosmeticItem cosmeticItem2 = this.items[cosmeticIdx];
				CosmeticItemInstance cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem.displayName);
				CosmeticItemInstance cosmeticItemInstance2 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
				string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem2.displayName);
				string itemNameFromDisplayName2 = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem.displayName);
				if (itemNameFromDisplayName == itemNameFromDisplayName2)
				{
					if (cosmeticItem2.isNullItem)
					{
						return;
					}
					if (cosmeticItemInstance2 != null)
					{
						if (!rig.IsItemAllowed(itemNameFromDisplayName))
						{
							cosmeticItemInstance2.DisableItem((CosmeticsController.CosmeticSlots)cosmeticIdx);
							return;
						}
						cosmeticItemInstance2.EnableItem((CosmeticsController.CosmeticSlots)cosmeticIdx);
					}
					return;
				}
				else
				{
					if (cosmeticItem2.isNullItem)
					{
						if (!cosmeticItem.isNullItem && cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem((CosmeticsController.CosmeticSlots)cosmeticIdx);
						}
						return;
					}
					if (!cosmeticItem.isNullItem && cosmeticItemInstance != null)
					{
						cosmeticItemInstance.DisableItem((CosmeticsController.CosmeticSlots)cosmeticIdx);
					}
					if (rig.IsItemAllowed(itemNameFromDisplayName) && cosmeticItemInstance2 != null)
					{
						cosmeticItemInstance2.EnableItem((CosmeticsController.CosmeticSlots)cosmeticIdx);
					}
					return;
				}
			}

			public void ActivateCosmetics(CosmeticsController.CosmeticSet prevSet, VRRig rig, BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticsObjectRegistry)
			{
				int num = 11;
				for (int i = 0; i < num; i++)
				{
					if (CosmeticsController.CosmeticSet.IsSlotHoldable((CosmeticsController.CosmeticSlots)i))
					{
						this.ActivateHoldable(i, bDock, nullItem);
					}
					else
					{
						this.ActivateCosmeticItem(prevSet, rig, i, cosmeticsObjectRegistry, nullItem);
					}
				}
				this.OnSetActivated(prevSet, this, rig.creatorWrapped);
			}

			public void DeactivateAllCosmetcs(BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticObjectRegistry)
			{
				bDock.DisableAllTransferableItems();
				int num = 11;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = this.items[i];
					if (!cosmeticItem.isNullItem)
					{
						CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
						if (!CosmeticsController.CosmeticSet.IsSlotHoldable(cosmeticSlots))
						{
							CosmeticItemInstance cosmeticItemInstance = cosmeticObjectRegistry.Cosmetic(cosmeticItem.displayName);
							if (cosmeticItemInstance != null)
							{
								cosmeticItemInstance.DisableItem(cosmeticSlots);
							}
						}
						this.items[i] = nullItem;
					}
				}
			}

			public void LoadFromPlayerPreferences(CosmeticsController controller)
			{
				int num = 11;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
					CosmeticsController.CosmeticItem item = controller.GetItemFromDict(PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(cosmeticSlots), "NOTHING"));
					if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
					{
						this.items[i] = item;
					}
					else
					{
						this.items[i] = controller.nullItem;
					}
				}
			}

			public string[] ToDisplayNameArray()
			{
				int num = 11;
				for (int i = 0; i < num; i++)
				{
					this.returnArray[i] = this.items[i].displayName;
				}
				return this.returnArray;
			}

			public string[] HoldableDisplayNames(bool leftHoldables)
			{
				int num = 11;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].itemCategory == CosmeticsController.CosmeticCategory.Holdable && this.items[i].itemCategory == CosmeticsController.CosmeticCategory.Holdable)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
					}
				}
				if (num2 == 0)
				{
					return null;
				}
				int num3 = 0;
				string[] array = new string[num2];
				for (int j = 0; j < num; j++)
				{
					if (this.items[j].itemCategory == CosmeticsController.CosmeticCategory.Holdable)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
					}
				}
				return array;
			}

			public bool[] ToOnRightSideArray()
			{
				int num = 11;
				bool[] array = new bool[num];
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].itemCategory == CosmeticsController.CosmeticCategory.Holdable)
					{
						array[i] = !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i));
					}
					else
					{
						array[i] = false;
					}
				}
				return array;
			}

			public CosmeticsController.CosmeticItem[] items;

			public string[] returnArray = new string[11];

			public delegate void OnSetActivatedHandler(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer);
		}

		[Serializable]
		public struct CosmeticItem
		{
			[Tooltip("Should match the spreadsheet item name.")]
			public string itemName;

			[Tooltip("Determines what wardrobe section the item will show up in.")]
			public CosmeticsController.CosmeticCategory itemCategory;

			[Tooltip("Icon shown in the store menus & hunt watch.")]
			public Sprite itemPicture;

			public string displayName;

			public string itemPictureResourceString;

			[Tooltip("The name shown on the store checkout screen.")]
			public string overrideDisplayName;

			[DebugReadout]
			[NonSerialized]
			public int cost;

			[DebugReadout]
			[NonSerialized]
			public string[] bundledItems;

			[DebugReadout]
			[NonSerialized]
			public bool canTryOn;

			[Tooltip("Set to true if the item takes up both left and right wearable hand slots at the same time. Used for things like mittens/gloves.")]
			public bool bothHandsHoldable;

			public bool bLoadsFromResources;

			public bool bUsesMeshAtlas;

			public Vector3 rotationOffset;

			public Vector3 positionOffset;

			public string meshAtlasResourceString;

			public string meshResourceString;

			public string materialResourceString;

			[HideInInspector]
			public bool isNullItem;
		}

		[Serializable]
		public class IAPRequestBody
		{
			public string accessToken;

			public string userID;

			public string nonce;

			public string platform;

			public string sku;

			public string playFabId;

			public bool[] debugParameters;
		}
	}
}
