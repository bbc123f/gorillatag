using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x020002AE RID: 686
	public class CosmeticsController : MonoBehaviour
	{
		// Token: 0x060011E7 RID: 4583 RVA: 0x00065FD4 File Offset: 0x000641D4
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
				for (int i = 0; i < 10; i++)
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

		// Token: 0x060011E8 RID: 4584 RVA: 0x00066129 File Offset: 0x00064329
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

		// Token: 0x060011E9 RID: 4585 RVA: 0x00066165 File Offset: 0x00064365
		public void Update()
		{
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x00066167 File Offset: 0x00064367
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
			default:
				return CosmeticsController.CosmeticSlots.Count;
			}
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x00066187 File Offset: 0x00064387
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

		// Token: 0x060011EC RID: 4588 RVA: 0x000661B9 File Offset: 0x000643B9
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

		// Token: 0x060011ED RID: 4589 RVA: 0x000661ED File Offset: 0x000643ED
		private void SaveItemPreference(CosmeticsController.CosmeticSlots slot, int slotIdx, CosmeticsController.CosmeticItem newItem)
		{
			PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
			PlayerPrefs.Save();
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x00066208 File Offset: 0x00064408
		public void SaveCurrentItemPreferences()
		{
			for (int i = 0; i < 10; i++)
			{
				CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
				this.SaveItemPreference(slot, i, this.currentWornSet.items[i]);
			}
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x00066240 File Offset: 0x00064440
		private void ApplyCosmeticToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, int slotIdx, CosmeticsController.CosmeticSlots slot, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			CosmeticsController.CosmeticItem cosmeticItem = (set.items[slotIdx].itemName == newItem.itemName) ? this.nullItem : newItem;
			set.items[slotIdx] = cosmeticItem;
			if (applyToPlayerPrefs)
			{
				this.SaveItemPreference(slot, slotIdx, cosmeticItem);
			}
			appliedSlots.Add(slot);
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x0006629C File Offset: 0x0006449C
		private void PrivApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			if (newItem.isNullItem)
			{
				return;
			}
			if (CosmeticsController.CosmeticSet.IsHoldable(newItem))
			{
				BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
				foreach (BodyDockPositions.DropPositions pos in dockingResult.positionsDisabled)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = this.DropPositionToCosmeticSlot(pos);
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
						BodyDockPositions.DropPositions dropPositions = enumerator.Current;
						if (dropPositions != BodyDockPositions.DropPositions.None)
						{
							CosmeticsController.CosmeticSlots cosmeticSlots2 = this.DropPositionToCosmeticSlot(dropPositions);
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
				CosmeticsController.CosmeticSlots cosmeticSlots3 = isLeftHand ? CosmeticsController.CosmeticSlots.HandLeft : CosmeticsController.CosmeticSlots.HandRight;
				int slotIdx = (int)cosmeticSlots3;
				this.ApplyCosmeticToSet(set, newItem, slotIdx, cosmeticSlots3, applyToPlayerPrefs, appliedSlots);
				CosmeticsController.CosmeticSlots cosmeticSlots4 = CosmeticsController.CosmeticSet.OppositeSlot(cosmeticSlots3);
				int num3 = (int)cosmeticSlots4;
				if (newItem.bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
				if (set.items[num3].itemName == newItem.itemName)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
				}
				if (set.items[num3].bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
			}
			else
			{
				CosmeticsController.CosmeticSlots cosmeticSlots5 = this.CategoryToNonTransferrableSlot(newItem.itemCategory);
				int slotIdx2 = (int)cosmeticSlots5;
				this.ApplyCosmeticToSet(set, newItem, slotIdx2, cosmeticSlots5, applyToPlayerPrefs, appliedSlots);
			}
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x000664A0 File Offset: 0x000646A0
		public List<CosmeticsController.CosmeticSlots> ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs)
		{
			List<CosmeticsController.CosmeticSlots> list = new List<CosmeticsController.CosmeticSlots>(2);
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				foreach (string itemID in newItem.bundledItems)
				{
					CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
					this.PrivApplyCosmeticItemToSet(set, itemFromDict, isLeftHand, applyToPlayerPrefs, list);
				}
			}
			else
			{
				this.PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, list);
			}
			return list;
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x000664FC File Offset: 0x000646FC
		public void RemoveCosmeticItemFromSet(CosmeticsController.CosmeticSet set, string itemName, bool applyToPlayerPrefs)
		{
			this.cachedSet.CopyItems(set);
			for (int i = 0; i < 10; i++)
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

		// Token: 0x060011F3 RID: 4595 RVA: 0x00066590 File Offset: 0x00064790
		public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
		{
			this.ApplyCosmeticItemToSet(this.tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, false);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x000665B4 File Offset: 0x000647B4
		public void PressCosmeticStandButton(CosmeticStand pressedStand)
		{
			this.searchIndex = this.currentCart.IndexOf(pressedStand.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				this.currentCart.RemoveAt(this.searchIndex);
				pressedStand.isOn = false;
				for (int i = 0; i < 10; i++)
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

		// Token: 0x060011F5 RID: 4597 RVA: 0x000666F4 File Offset: 0x000648F4
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

		// Token: 0x060011F6 RID: 4598 RVA: 0x00066784 File Offset: 0x00064984
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

		// Token: 0x060011F7 RID: 4599 RVA: 0x000668EA File Offset: 0x00064AEA
		public void ClearCheckout()
		{
			this.itemToBuy = this.allCosmetics[0];
			this.checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState(null, false);
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x00066924 File Offset: 0x00064B24
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
						foreach (string itemID in this.itemToBuy.bundledItems)
						{
							this.tempItem = this.GetItemFromDict(itemID);
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

		// Token: 0x060011F9 RID: 4601 RVA: 0x00066A66 File Offset: 0x00064C66
		public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x00066A78 File Offset: 0x00064C78
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

		// Token: 0x060011FB RID: 4603 RVA: 0x00066AC8 File Offset: 0x00064CC8
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
					foreach (string str in itemFromDict.bundledItems)
					{
						VRRig offlineVRRig2 = GorillaTagger.Instance.offlineVRRig;
						offlineVRRig2.concatStringOfCosmeticsAllowed += str;
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

		// Token: 0x060011FC RID: 4604 RVA: 0x00066F64 File Offset: 0x00065164
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

		// Token: 0x060011FD RID: 4605 RVA: 0x00066FE8 File Offset: 0x000651E8
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
							foreach (string itemIdToUnlock in itemFromDict.bundledItems)
							{
								this.UnlockItem(itemIdToUnlock);
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
						WebFlags flags = new WebFlags(1);
						raiseEventOptions.Flags = flags;
						object[] eventContent = new object[0];
						PhotonNetwork.RaiseEvent(9, eventContent, raiseEventOptions, SendOptions.SendReliable);
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

		// Token: 0x060011FE RID: 4606 RVA: 0x00067054 File Offset: 0x00065254
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

		// Token: 0x060011FF RID: 4607 RVA: 0x000671E0 File Offset: 0x000653E0
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

		// Token: 0x06001200 RID: 4608 RVA: 0x000671F8 File Offset: 0x000653F8
		public void UpdateWardrobeModelsAndButtons()
		{
			foreach (CosmeticsController.Wardrobe wardrobe in this.wardrobes)
			{
				wardrobe.wardrobeItemButtons[0].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3] : this.nullItem);
				wardrobe.wardrobeItemButtons[1].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 1 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 1] : this.nullItem);
				wardrobe.wardrobeItemButtons[2].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 2 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 2] : this.nullItem);
				this.iterator = 0;
				while (this.iterator < wardrobe.wardrobeItemButtons.Length)
				{
					CosmeticsController.CosmeticItem currentCosmeticItem = wardrobe.wardrobeItemButtons[this.iterator].currentCosmeticItem;
					wardrobe.wardrobeItemButtons[this.iterator].isOn = (!currentCosmeticItem.isNullItem && this.AnyMatch(this.currentWornSet, currentCosmeticItem));
					wardrobe.wardrobeItemButtons[this.iterator].UpdateColor();
					this.iterator++;
				}
				wardrobe.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[0].currentCosmeticItem.displayName, false);
				wardrobe.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[1].currentCosmeticItem.displayName, false);
				wardrobe.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[2].currentCosmeticItem.displayName, false);
				wardrobe.selfDoll.SetCosmeticActiveArray(this.currentWornSet.ToDisplayNameArray(), this.currentWornSet.ToOnRightSideArray());
			}
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x00067444 File Offset: 0x00065644
		public void UpdateShoppingCart()
		{
			this.iterator = 0;
			while (this.iterator < this.fittingRoomButtons.Length)
			{
				if (this.iterator < this.currentCart.Count)
				{
					this.fittingRoomButtons[this.iterator].currentCosmeticItem = this.currentCart[this.iterator];
					this.checkoutCartButtons[this.iterator].currentCosmeticItem = this.currentCart[this.iterator];
					this.checkoutCartButtons[this.iterator].isOn = (this.checkoutCartButtons[this.iterator].currentCosmeticItem.itemName == this.itemToBuy.itemName);
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

		// Token: 0x06001202 RID: 4610 RVA: 0x00067624 File Offset: 0x00065824
		public void UpdateWornCosmetics(bool sync = false)
		{
			GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(this.currentWornSet, this.tryOnSet);
			if (sync && GorillaTagger.Instance.myVRRig != null)
			{
				string[] array = this.currentWornSet.ToDisplayNameArray();
				string[] array2 = this.tryOnSet.ToDisplayNameArray();
				GorillaTagger.Instance.myVRRig.RPC("UpdateCosmeticsWithTryon", RpcTarget.All, new object[]
				{
					array,
					array2
				});
			}
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0006769C File Offset: 0x0006589C
		public CosmeticsController.CosmeticItem GetItemFromDict(string itemID)
		{
			if (!this.allCosmeticsDict.TryGetValue(itemID, out this.cosmeticItemVar))
			{
				return this.nullItem;
			}
			return this.cosmeticItemVar;
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x000676BF File Offset: 0x000658BF
		public string GetItemNameFromDisplayName(string displayName)
		{
			if (!this.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, out this.returnString))
			{
				return "null";
			}
			return this.returnString;
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x000676E4 File Offset: 0x000658E4
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

		// Token: 0x06001206 RID: 4614 RVA: 0x000677B4 File Offset: 0x000659B4
		public void Initialize()
		{
			if (base.gameObject.activeSelf)
			{
				this.GetUserCosmeticsAllowed();
			}
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x000677C9 File Offset: 0x000659C9
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

		// Token: 0x06001208 RID: 4616 RVA: 0x000677EF File Offset: 0x000659EF
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

		// Token: 0x06001209 RID: 4617 RVA: 0x000677FE File Offset: 0x000659FE
		private IEnumerator GetMyDaily()
		{
			yield return new WaitForSeconds(10f);
			ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
			executeCloudScriptRequest.FunctionName = "TryDistributeCurrency";
			executeCloudScriptRequest.FunctionParameter = new
			{

			};
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

		// Token: 0x0600120A RID: 4618 RVA: 0x0006780D File Offset: 0x00065A0D
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
								uint cost;
								if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out cost))
								{
									this.hasPrice = true;
								}
								this.allCosmetics[this.searchIndex] = new CosmeticsController.CosmeticItem
								{
									itemName = catalogItem.ItemId,
									displayName = catalogItem.DisplayName,
									cost = (int)cost,
									itemPicture = this.allCosmetics[this.searchIndex].itemPicture,
									itemCategory = this.allCosmetics[this.searchIndex].itemCategory,
									bundledItems = this.tempStringArray,
									canTryOn = this.hasPrice,
									bothHandsHoldable = this.allCosmetics[this.searchIndex].bothHandsHoldable,
									overrideDisplayName = this.allCosmetics[this.searchIndex].overrideDisplayName
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
					int num;
					this.playedInBeta = (result.VirtualCurrency.TryGetValue("TC", out num) && num > 0);
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
						GameObject[] array = Object.FindObjectsOfType<GameObject>();
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
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
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
				if (!this.tryTwice)
				{
					this.tryTwice = true;
					this.GetUserCosmeticsAllowed();
				}
			}, null, null);
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x00067834 File Offset: 0x00065A34
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
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
				Debug.Log("error in starting purchase!");
			}, null, null);
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x000678C4 File Offset: 0x00065AC4
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

		// Token: 0x0600120D RID: 4621 RVA: 0x00067A50 File Offset: 0x00065C50
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

		// Token: 0x0600120E RID: 4622 RVA: 0x00067C40 File Offset: 0x00065E40
		public void PressCurrencyPurchaseButton(string currencyPurchaseSize)
		{
			this.ProcessATMState(currencyPurchaseSize);
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x00067C49 File Offset: 0x00065E49
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
						WebFlags flags = new WebFlags(1);
						raiseEventOptions.Flags = flags;
						object[] eventContent = new object[0];
						PhotonNetwork.RaiseEvent(9, eventContent, raiseEventOptions, SendOptions.SendReliable);
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

		// Token: 0x06001210 RID: 4624 RVA: 0x00067C7C File Offset: 0x00065E7C
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

		// Token: 0x06001211 RID: 4625 RVA: 0x00067D30 File Offset: 0x00065F30
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

		// Token: 0x06001212 RID: 4626 RVA: 0x00067D69 File Offset: 0x00065F69
		public string GetItemDisplayName(CosmeticsController.CosmeticItem item)
		{
			if (item.overrideDisplayName != null && item.overrideDisplayName != "")
			{
				return item.overrideDisplayName;
			}
			return item.displayName;
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x00067D92 File Offset: 0x00065F92
		public void LeaveSystemMenu()
		{
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x00067D94 File Offset: 0x00065F94
		private void AlreadyOwnAllBundleButtons()
		{
			EarlyAccessButton[] array = this.earlyAccessButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AlreadyOwn();
			}
		}

		// Token: 0x040014C5 RID: 5317
		public static int maximumTransferrableItems = 5;

		// Token: 0x040014C6 RID: 5318
		public static volatile CosmeticsController instance;

		// Token: 0x040014C7 RID: 5319
		public List<CosmeticsController.CosmeticItem> allCosmetics;

		// Token: 0x040014C8 RID: 5320
		public Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict = new Dictionary<string, CosmeticsController.CosmeticItem>();

		// Token: 0x040014C9 RID: 5321
		public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>();

		// Token: 0x040014CA RID: 5322
		public CosmeticsController.CosmeticItem nullItem;

		// Token: 0x040014CB RID: 5323
		public string catalog;

		// Token: 0x040014CC RID: 5324
		private string[] tempStringArray;

		// Token: 0x040014CD RID: 5325
		private CosmeticsController.CosmeticItem tempItem;

		// Token: 0x040014CE RID: 5326
		public List<CatalogItem> catalogItems;

		// Token: 0x040014CF RID: 5327
		public bool tryTwice;

		// Token: 0x040014D0 RID: 5328
		[NonSerialized]
		public CosmeticsController.CosmeticSet tryOnSet = new CosmeticsController.CosmeticSet();

		// Token: 0x040014D1 RID: 5329
		public FittingRoomButton[] fittingRoomButtons;

		// Token: 0x040014D2 RID: 5330
		public CosmeticStand[] cosmeticStands;

		// Token: 0x040014D3 RID: 5331
		public List<CosmeticsController.CosmeticItem> currentCart = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x040014D4 RID: 5332
		public CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

		// Token: 0x040014D5 RID: 5333
		public CheckoutCartButton[] checkoutCartButtons;

		// Token: 0x040014D6 RID: 5334
		public PurchaseItemButton leftPurchaseButton;

		// Token: 0x040014D7 RID: 5335
		public PurchaseItemButton rightPurchaseButton;

		// Token: 0x040014D8 RID: 5336
		public Text purchaseText;

		// Token: 0x040014D9 RID: 5337
		public CosmeticsController.CosmeticItem itemToBuy;

		// Token: 0x040014DA RID: 5338
		public HeadModel checkoutHeadModel;

		// Token: 0x040014DB RID: 5339
		private List<string> playerIDList = new List<string>();

		// Token: 0x040014DC RID: 5340
		private bool foundCosmetic;

		// Token: 0x040014DD RID: 5341
		private int attempts;

		// Token: 0x040014DE RID: 5342
		private string finalLine;

		// Token: 0x040014DF RID: 5343
		private bool purchaseLocked;

		// Token: 0x040014E0 RID: 5344
		private bool isLastHandTouchedLeft;

		// Token: 0x040014E1 RID: 5345
		private CosmeticsController.CosmeticSet cachedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x040014E2 RID: 5346
		public CosmeticsController.Wardrobe[] wardrobes;

		// Token: 0x040014E3 RID: 5347
		public List<CosmeticsController.CosmeticItem> unlockedCosmetics = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x040014E4 RID: 5348
		public List<CosmeticsController.CosmeticItem> unlockedHats = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x040014E5 RID: 5349
		public List<CosmeticsController.CosmeticItem> unlockedFaces = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x040014E6 RID: 5350
		public List<CosmeticsController.CosmeticItem> unlockedBadges = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x040014E7 RID: 5351
		public List<CosmeticsController.CosmeticItem> unlockedHoldable = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x040014E8 RID: 5352
		public int[] cosmeticsPages = new int[4];

		// Token: 0x040014E9 RID: 5353
		private List<CosmeticsController.CosmeticItem>[] itemLists = new List<CosmeticsController.CosmeticItem>[4];

		// Token: 0x040014EA RID: 5354
		private int wardrobeType;

		// Token: 0x040014EB RID: 5355
		[NonSerialized]
		public CosmeticsController.CosmeticSet currentWornSet = new CosmeticsController.CosmeticSet();

		// Token: 0x040014EC RID: 5356
		public string concatStringCosmeticsAllowed = "";

		// Token: 0x040014ED RID: 5357
		public Text atmText;

		// Token: 0x040014EE RID: 5358
		public string currentAtmString;

		// Token: 0x040014EF RID: 5359
		public Text infoText;

		// Token: 0x040014F0 RID: 5360
		public Text earlyAccessText;

		// Token: 0x040014F1 RID: 5361
		public Text[] purchaseButtonText;

		// Token: 0x040014F2 RID: 5362
		public Text dailyText;

		// Token: 0x040014F3 RID: 5363
		public CosmeticsController.ATMStages currentATMStage;

		// Token: 0x040014F4 RID: 5364
		public Text atmButtonsText;

		// Token: 0x040014F5 RID: 5365
		public int currencyBalance;

		// Token: 0x040014F6 RID: 5366
		public string currencyName;

		// Token: 0x040014F7 RID: 5367
		public PurchaseCurrencyButton[] purchaseCurrencyButtons;

		// Token: 0x040014F8 RID: 5368
		public Text currencyBoardText;

		// Token: 0x040014F9 RID: 5369
		public Text currencyBoxText;

		// Token: 0x040014FA RID: 5370
		public string startingCurrencyBoxTextString;

		// Token: 0x040014FB RID: 5371
		public string successfulCurrencyPurchaseTextString;

		// Token: 0x040014FC RID: 5372
		public int numShinyRocksToBuy;

		// Token: 0x040014FD RID: 5373
		public float shinyRocksCost;

		// Token: 0x040014FE RID: 5374
		public string itemToPurchase;

		// Token: 0x040014FF RID: 5375
		public bool confirmedDidntPlayInBeta;

		// Token: 0x04001500 RID: 5376
		public bool playedInBeta;

		// Token: 0x04001501 RID: 5377
		public bool gotMyDaily;

		// Token: 0x04001502 RID: 5378
		public bool checkedDaily;

		// Token: 0x04001503 RID: 5379
		public string currentPurchaseID;

		// Token: 0x04001504 RID: 5380
		public bool hasPrice;

		// Token: 0x04001505 RID: 5381
		private int searchIndex;

		// Token: 0x04001506 RID: 5382
		private int iterator;

		// Token: 0x04001507 RID: 5383
		private CosmeticsController.CosmeticItem cosmeticItemVar;

		// Token: 0x04001508 RID: 5384
		public EarlyAccessButton[] earlyAccessButtons;

		// Token: 0x04001509 RID: 5385
		private BundleList bundleList = new BundleList();

		// Token: 0x0400150A RID: 5386
		public string BundleSkuName = "2023_spider_monke_bundle";

		// Token: 0x0400150B RID: 5387
		public string BundlePlayfabItemName = "LSABD.";

		// Token: 0x0400150C RID: 5388
		public int BundleShinyRocks = 10000;

		// Token: 0x0400150D RID: 5389
		public bool buyingBundle;

		// Token: 0x0400150E RID: 5390
		public DateTime currentTime;

		// Token: 0x0400150F RID: 5391
		public string lastDailyLogin;

		// Token: 0x04001510 RID: 5392
		public UserDataRecord userDataRecord;

		// Token: 0x04001511 RID: 5393
		public int secondsUntilTomorrow;

		// Token: 0x04001512 RID: 5394
		public float secondsToWaitToCheckDaily = 10f;

		// Token: 0x04001513 RID: 5395
		private string returnString;

		// Token: 0x04001514 RID: 5396
		protected Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse;

		// Token: 0x020004B5 RID: 1205
		public enum PurchaseItemStages
		{
			// Token: 0x04001F7F RID: 8063
			Start,
			// Token: 0x04001F80 RID: 8064
			CheckoutButtonPressed,
			// Token: 0x04001F81 RID: 8065
			ItemSelected,
			// Token: 0x04001F82 RID: 8066
			ItemOwned,
			// Token: 0x04001F83 RID: 8067
			FinalPurchaseAcknowledgement,
			// Token: 0x04001F84 RID: 8068
			Buying,
			// Token: 0x04001F85 RID: 8069
			Success,
			// Token: 0x04001F86 RID: 8070
			Failure
		}

		// Token: 0x020004B6 RID: 1206
		[Serializable]
		public struct Wardrobe
		{
			// Token: 0x04001F87 RID: 8071
			public WardrobeItemButton[] wardrobeItemButtons;

			// Token: 0x04001F88 RID: 8072
			public HeadModel selfDoll;
		}

		// Token: 0x020004B7 RID: 1207
		public enum ATMStages
		{
			// Token: 0x04001F8A RID: 8074
			Unavailable,
			// Token: 0x04001F8B RID: 8075
			Begin,
			// Token: 0x04001F8C RID: 8076
			Menu,
			// Token: 0x04001F8D RID: 8077
			Balance,
			// Token: 0x04001F8E RID: 8078
			Choose,
			// Token: 0x04001F8F RID: 8079
			Confirm,
			// Token: 0x04001F90 RID: 8080
			Purchasing,
			// Token: 0x04001F91 RID: 8081
			Success,
			// Token: 0x04001F92 RID: 8082
			Failure,
			// Token: 0x04001F93 RID: 8083
			Locked
		}

		// Token: 0x020004B8 RID: 1208
		public enum CosmeticCategory
		{
			// Token: 0x04001F95 RID: 8085
			None,
			// Token: 0x04001F96 RID: 8086
			Hat,
			// Token: 0x04001F97 RID: 8087
			Badge,
			// Token: 0x04001F98 RID: 8088
			Face,
			// Token: 0x04001F99 RID: 8089
			Holdable,
			// Token: 0x04001F9A RID: 8090
			Gloves,
			// Token: 0x04001F9B RID: 8091
			Slingshot,
			// Token: 0x04001F9C RID: 8092
			Count,
			// Token: 0x04001F9D RID: 8093
			Set
		}

		// Token: 0x020004B9 RID: 1209
		public enum CosmeticSlots
		{
			// Token: 0x04001F9F RID: 8095
			Hat,
			// Token: 0x04001FA0 RID: 8096
			Badge,
			// Token: 0x04001FA1 RID: 8097
			Face,
			// Token: 0x04001FA2 RID: 8098
			ArmLeft,
			// Token: 0x04001FA3 RID: 8099
			ArmRight,
			// Token: 0x04001FA4 RID: 8100
			BackLeft,
			// Token: 0x04001FA5 RID: 8101
			BackRight,
			// Token: 0x04001FA6 RID: 8102
			HandLeft,
			// Token: 0x04001FA7 RID: 8103
			HandRight,
			// Token: 0x04001FA8 RID: 8104
			Chest,
			// Token: 0x04001FA9 RID: 8105
			Count
		}

		// Token: 0x020004BA RID: 1210
		[Serializable]
		public class CosmeticSet
		{
			// Token: 0x14000026 RID: 38
			// (add) Token: 0x06001E55 RID: 7765 RVA: 0x0009E244 File Offset: 0x0009C444
			// (remove) Token: 0x06001E56 RID: 7766 RVA: 0x0009E27C File Offset: 0x0009C47C
			public event CosmeticsController.CosmeticSet.OnSetActivatedHandler onSetActivatedEvent;

			// Token: 0x06001E57 RID: 7767 RVA: 0x0009E2B1 File Offset: 0x0009C4B1
			protected void OnSetActivated(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, Photon.Realtime.Player player)
			{
				if (this.onSetActivatedEvent != null)
				{
					this.onSetActivatedEvent(prevSet, currentSet, player);
				}
			}

			// Token: 0x06001E58 RID: 7768 RVA: 0x0009E2C9 File Offset: 0x0009C4C9
			public CosmeticSet()
			{
				this.items = new CosmeticsController.CosmeticItem[10];
			}

			// Token: 0x06001E59 RID: 7769 RVA: 0x0009E2EC File Offset: 0x0009C4EC
			public CosmeticSet(string[] itemNames, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[10];
				for (int i = 0; i < itemNames.Length; i++)
				{
					string displayName = itemNames[i];
					string itemNameFromDisplayName = controller.GetItemNameFromDisplayName(displayName);
					this.items[i] = controller.GetItemFromDict(itemNameFromDisplayName);
				}
			}

			// Token: 0x06001E5A RID: 7770 RVA: 0x0009E348 File Offset: 0x0009C548
			public void CopyItems(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					this.items[i] = other.items[i];
				}
			}

			// Token: 0x06001E5B RID: 7771 RVA: 0x0009E380 File Offset: 0x0009C580
			public void MergeSets(CosmeticsController.CosmeticSet tryOn, CosmeticsController.CosmeticSet current)
			{
				for (int i = 0; i < 10; i++)
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

			// Token: 0x06001E5C RID: 7772 RVA: 0x0009E3F0 File Offset: 0x0009C5F0
			public void ClearSet(CosmeticsController.CosmeticItem nullItem)
			{
				for (int i = 0; i < 10; i++)
				{
					this.items[i] = nullItem;
				}
			}

			// Token: 0x06001E5D RID: 7773 RVA: 0x0009E418 File Offset: 0x0009C618
			public bool IsActive(string name)
			{
				int num = 10;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06001E5E RID: 7774 RVA: 0x0009E450 File Offset: 0x0009C650
			public bool HasItemOfCategory(CosmeticsController.CosmeticCategory category)
			{
				int num = 10;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemCategory == category)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06001E5F RID: 7775 RVA: 0x0009E498 File Offset: 0x0009C698
			public bool HasItem(string name)
			{
				int num = 10;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06001E60 RID: 7776 RVA: 0x0009E4E3 File Offset: 0x0009C6E3
			public static bool IsSlotLeftHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.HandLeft;
			}

			// Token: 0x06001E61 RID: 7777 RVA: 0x0009E4F3 File Offset: 0x0009C6F3
			public static bool IsSlotRightHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.HandRight;
			}

			// Token: 0x06001E62 RID: 7778 RVA: 0x0009E503 File Offset: 0x0009C703
			public static bool IsHoldable(CosmeticsController.CosmeticItem item)
			{
				return item.itemCategory == CosmeticsController.CosmeticCategory.Holdable || item.itemCategory == CosmeticsController.CosmeticCategory.Slingshot;
			}

			// Token: 0x06001E63 RID: 7779 RVA: 0x0009E519 File Offset: 0x0009C719
			public static bool IsSlotHoldable(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.Chest;
			}

			// Token: 0x06001E64 RID: 7780 RVA: 0x0009E534 File Offset: 0x0009C734
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
				default:
					return CosmeticsController.CosmeticSlots.Count;
				}
			}

			// Token: 0x06001E65 RID: 7781 RVA: 0x0009E588 File Offset: 0x0009C788
			public static string SlotPlayerPreferenceName(CosmeticsController.CosmeticSlots slot)
			{
				return "slot_" + slot.ToString();
			}

			// Token: 0x06001E66 RID: 7782 RVA: 0x0009E5A4 File Offset: 0x0009C7A4
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

			// Token: 0x06001E67 RID: 7783 RVA: 0x0009E618 File Offset: 0x0009C818
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

			// Token: 0x06001E68 RID: 7784 RVA: 0x0009E700 File Offset: 0x0009C900
			public void ActivateCosmetics(CosmeticsController.CosmeticSet prevSet, VRRig rig, BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticsObjectRegistry)
			{
				int num = 10;
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
				this.OnSetActivated(prevSet, this, rig.myPlayer);
			}

			// Token: 0x06001E69 RID: 7785 RVA: 0x0009E74C File Offset: 0x0009C94C
			public void DeactivateAllCosmetcs(BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticObjectRegistry)
			{
				bDock.DisableAllTransferableItems();
				int num = 10;
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

			// Token: 0x06001E6A RID: 7786 RVA: 0x0009E7B4 File Offset: 0x0009C9B4
			public void LoadFromPlayerPreferences(CosmeticsController controller)
			{
				int num = 10;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
					CosmeticsController.CosmeticItem item = controller.GetItemFromDict(PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), "NOTHING"));
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

			// Token: 0x06001E6B RID: 7787 RVA: 0x0009E834 File Offset: 0x0009CA34
			public string[] ToDisplayNameArray()
			{
				int num = 10;
				for (int i = 0; i < num; i++)
				{
					this.returnArray[i] = this.items[i].displayName;
				}
				return this.returnArray;
			}

			// Token: 0x06001E6C RID: 7788 RVA: 0x0009E870 File Offset: 0x0009CA70
			public string[] HoldableDisplayNames(bool leftHoldables)
			{
				int num = 10;
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

			// Token: 0x06001E6D RID: 7789 RVA: 0x0009E974 File Offset: 0x0009CB74
			public bool[] ToOnRightSideArray()
			{
				int num = 10;
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

			// Token: 0x04001FAA RID: 8106
			public CosmeticsController.CosmeticItem[] items;

			// Token: 0x04001FAC RID: 8108
			public string[] returnArray = new string[10];

			// Token: 0x0200054F RID: 1359
			// (Invoke) Token: 0x06001FC7 RID: 8135
			public delegate void OnSetActivatedHandler(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, Photon.Realtime.Player player);
		}

		// Token: 0x020004BB RID: 1211
		[Serializable]
		public struct CosmeticItem
		{
			// Token: 0x04001FAD RID: 8109
			public string itemName;

			// Token: 0x04001FAE RID: 8110
			public CosmeticsController.CosmeticCategory itemCategory;

			// Token: 0x04001FAF RID: 8111
			public Sprite itemPicture;

			// Token: 0x04001FB0 RID: 8112
			public string displayName;

			// Token: 0x04001FB1 RID: 8113
			public string overrideDisplayName;

			// Token: 0x04001FB2 RID: 8114
			public int cost;

			// Token: 0x04001FB3 RID: 8115
			public string[] bundledItems;

			// Token: 0x04001FB4 RID: 8116
			public bool canTryOn;

			// Token: 0x04001FB5 RID: 8117
			public bool bothHandsHoldable;

			// Token: 0x04001FB6 RID: 8118
			[HideInInspector]
			public bool isNullItem;
		}
	}
}
