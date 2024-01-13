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

namespace GorillaNetworking;

public class CosmeticsController : MonoBehaviour
{
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

	[Serializable]
	public struct Wardrobe
	{
		public WardrobeItemButton[] wardrobeItemButtons;

		public HeadModel selfDoll;
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
		Count
	}

	[Serializable]
	public class CosmeticSet
	{
		public delegate void OnSetActivatedHandler(CosmeticSet prevSet, CosmeticSet currentSet, Photon.Realtime.Player player);

		public CosmeticItem[] items;

		public string[] returnArray = new string[10];

		public event OnSetActivatedHandler onSetActivatedEvent;

		protected void OnSetActivated(CosmeticSet prevSet, CosmeticSet currentSet, Photon.Realtime.Player player)
		{
			if (this.onSetActivatedEvent != null)
			{
				this.onSetActivatedEvent(prevSet, currentSet, player);
			}
		}

		public CosmeticSet()
		{
			items = new CosmeticItem[10];
		}

		public CosmeticSet(string[] itemNames, CosmeticsController controller)
		{
			items = new CosmeticItem[10];
			for (int i = 0; i < itemNames.Length; i++)
			{
				string displayName = itemNames[i];
				string itemNameFromDisplayName = controller.GetItemNameFromDisplayName(displayName);
				items[i] = controller.GetItemFromDict(itemNameFromDisplayName);
			}
		}

		public void CopyItems(CosmeticSet other)
		{
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = other.items[i];
			}
		}

		public void MergeSets(CosmeticSet tryOn, CosmeticSet current)
		{
			for (int i = 0; i < 10; i++)
			{
				if (tryOn == null)
				{
					items[i] = current.items[i];
				}
				else
				{
					items[i] = (tryOn.items[i].isNullItem ? current.items[i] : tryOn.items[i]);
				}
			}
		}

		public void ClearSet(CosmeticItem nullItem)
		{
			for (int i = 0; i < 10; i++)
			{
				items[i] = nullItem;
			}
		}

		public bool IsActive(string name)
		{
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				if (items[i].displayName == name)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasItemOfCategory(CosmeticCategory category)
		{
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				if (!items[i].isNullItem && items[i].itemCategory == category)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasItem(string name)
		{
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				if (!items[i].isNullItem && items[i].displayName == name)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsSlotLeftHanded(CosmeticSlots slot)
		{
			if (slot != CosmeticSlots.ArmLeft && slot != CosmeticSlots.BackLeft)
			{
				return slot == CosmeticSlots.HandLeft;
			}
			return true;
		}

		public static bool IsSlotRightHanded(CosmeticSlots slot)
		{
			if (slot != CosmeticSlots.ArmRight && slot != CosmeticSlots.BackRight)
			{
				return slot == CosmeticSlots.HandRight;
			}
			return true;
		}

		public static bool IsHoldable(CosmeticItem item)
		{
			if (item.itemCategory != CosmeticCategory.Holdable)
			{
				return item.itemCategory == CosmeticCategory.Slingshot;
			}
			return true;
		}

		public static bool IsSlotHoldable(CosmeticSlots slot)
		{
			if (slot != CosmeticSlots.ArmLeft && slot != CosmeticSlots.ArmRight && slot != CosmeticSlots.BackLeft && slot != CosmeticSlots.BackRight)
			{
				return slot == CosmeticSlots.Chest;
			}
			return true;
		}

		public static CosmeticSlots OppositeSlot(CosmeticSlots slot)
		{
			return slot switch
			{
				CosmeticSlots.Hat => CosmeticSlots.Hat, 
				CosmeticSlots.Badge => CosmeticSlots.Badge, 
				CosmeticSlots.Face => CosmeticSlots.Face, 
				CosmeticSlots.ArmLeft => CosmeticSlots.ArmRight, 
				CosmeticSlots.ArmRight => CosmeticSlots.ArmLeft, 
				CosmeticSlots.BackLeft => CosmeticSlots.BackRight, 
				CosmeticSlots.BackRight => CosmeticSlots.BackLeft, 
				CosmeticSlots.HandLeft => CosmeticSlots.HandRight, 
				CosmeticSlots.HandRight => CosmeticSlots.HandLeft, 
				CosmeticSlots.Chest => CosmeticSlots.Chest, 
				_ => CosmeticSlots.Count, 
			};
		}

		public static string SlotPlayerPreferenceName(CosmeticSlots slot)
		{
			return "slot_" + slot;
		}

		private void ActivateHoldable(int cosmeticIdx, BodyDockPositions bDock, CosmeticItem nullItem)
		{
			BodyDockPositions.DropPositions dropPositions = CosmeticSlotToDropPosition((CosmeticSlots)cosmeticIdx);
			CosmeticItem cosmeticItem = items[cosmeticIdx];
			if (cosmeticItem.isNullItem)
			{
				bDock.TransferrableItemDisableAtPosition(dropPositions);
			}
			else if (bDock.ItemPositionInUse(dropPositions) == null)
			{
				bDock.TransferrableItemEnableAtPosition(cosmeticItem.displayName, dropPositions);
			}
			else if (!bDock.TransferrableItemActiveAtPos(cosmeticItem.displayName, dropPositions))
			{
				bDock.TransferrableItemDisableAtPosition(dropPositions);
				bDock.TransferrableItemEnableAtPosition(cosmeticItem.displayName, dropPositions);
			}
		}

		private void ActivateCosmeticItem(CosmeticSet prevSet, VRRig rig, int cosmeticIdx, CosmeticItemRegistry cosmeticsObjectRegistry, CosmeticItem nullItem)
		{
			CosmeticItem cosmeticItem = prevSet.items[cosmeticIdx];
			CosmeticItem cosmeticItem2 = items[cosmeticIdx];
			CosmeticItemInstance cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem.displayName);
			CosmeticItemInstance cosmeticItemInstance2 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
			string itemNameFromDisplayName = instance.GetItemNameFromDisplayName(cosmeticItem2.displayName);
			string itemNameFromDisplayName2 = instance.GetItemNameFromDisplayName(cosmeticItem.displayName);
			if (itemNameFromDisplayName == itemNameFromDisplayName2)
			{
				if (!cosmeticItem2.isNullItem && cosmeticItemInstance2 != null)
				{
					if (!rig.IsItemAllowed(itemNameFromDisplayName))
					{
						cosmeticItemInstance2.DisableItem((CosmeticSlots)cosmeticIdx);
					}
					else
					{
						cosmeticItemInstance2.EnableItem((CosmeticSlots)cosmeticIdx);
					}
				}
				return;
			}
			if (cosmeticItem2.isNullItem)
			{
				if (!cosmeticItem.isNullItem)
				{
					cosmeticItemInstance?.DisableItem((CosmeticSlots)cosmeticIdx);
				}
				return;
			}
			if (!cosmeticItem.isNullItem)
			{
				cosmeticItemInstance?.DisableItem((CosmeticSlots)cosmeticIdx);
			}
			if (rig.IsItemAllowed(itemNameFromDisplayName))
			{
				cosmeticItemInstance2?.EnableItem((CosmeticSlots)cosmeticIdx);
			}
		}

		public void ActivateCosmetics(CosmeticSet prevSet, VRRig rig, BodyDockPositions bDock, CosmeticItem nullItem, CosmeticItemRegistry cosmeticsObjectRegistry)
		{
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				if (IsSlotHoldable((CosmeticSlots)i))
				{
					ActivateHoldable(i, bDock, nullItem);
				}
				else
				{
					ActivateCosmeticItem(prevSet, rig, i, cosmeticsObjectRegistry, nullItem);
				}
			}
			OnSetActivated(prevSet, this, rig.myPlayer);
		}

		public void DeactivateAllCosmetcs(BodyDockPositions bDock, CosmeticItem nullItem, CosmeticItemRegistry cosmeticObjectRegistry)
		{
			bDock.DisableAllTransferableItems();
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				CosmeticItem cosmeticItem = items[i];
				if (!cosmeticItem.isNullItem)
				{
					CosmeticSlots cosmeticSlots = (CosmeticSlots)i;
					if (!IsSlotHoldable(cosmeticSlots))
					{
						cosmeticObjectRegistry.Cosmetic(cosmeticItem.displayName)?.DisableItem(cosmeticSlots);
					}
					items[i] = nullItem;
				}
			}
		}

		public void LoadFromPlayerPreferences(CosmeticsController controller)
		{
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				CosmeticSlots slot = (CosmeticSlots)i;
				CosmeticItem item = controller.GetItemFromDict(PlayerPrefs.GetString(SlotPlayerPreferenceName(slot), "NOTHING"));
				if (controller.unlockedCosmetics.FindIndex((CosmeticItem x) => item.itemName == x.itemName) >= 0)
				{
					items[i] = item;
				}
				else
				{
					items[i] = controller.nullItem;
				}
			}
		}

		public string[] ToDisplayNameArray()
		{
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				returnArray[i] = items[i].displayName;
			}
			return returnArray;
		}

		public string[] HoldableDisplayNames(bool leftHoldables)
		{
			int num = 10;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (items[i].itemCategory == CosmeticCategory.Holdable && items[i].itemCategory == CosmeticCategory.Holdable)
				{
					if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticSlotToDropPosition((CosmeticSlots)i)))
					{
						num2++;
					}
					else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticSlotToDropPosition((CosmeticSlots)i)))
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
				if (items[j].itemCategory == CosmeticCategory.Holdable)
				{
					if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticSlotToDropPosition((CosmeticSlots)j)))
					{
						array[num3] = items[j].displayName;
						num3++;
					}
					else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticSlotToDropPosition((CosmeticSlots)j)))
					{
						array[num3] = items[j].displayName;
						num3++;
					}
				}
			}
			return array;
		}

		public bool[] ToOnRightSideArray()
		{
			int num = 10;
			bool[] array = new bool[num];
			for (int i = 0; i < num; i++)
			{
				if (items[i].itemCategory == CosmeticCategory.Holdable)
				{
					array[i] = !BodyDockPositions.IsPositionLeft(CosmeticSlotToDropPosition((CosmeticSlots)i));
				}
				else
				{
					array[i] = false;
				}
			}
			return array;
		}
	}

	[Serializable]
	public struct CosmeticItem
	{
		public string itemName;

		public CosmeticCategory itemCategory;

		public Sprite itemPicture;

		public string displayName;

		public string overrideDisplayName;

		public int cost;

		public string[] bundledItems;

		public bool canTryOn;

		public bool bothHandsHoldable;

		[HideInInspector]
		public bool isNullItem;
	}

	public static int maximumTransferrableItems = 5;

	public static volatile CosmeticsController instance;

	public List<CosmeticItem> allCosmetics;

	public Dictionary<string, CosmeticItem> allCosmeticsDict = new Dictionary<string, CosmeticItem>();

	public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>();

	public CosmeticItem nullItem;

	public string catalog;

	private string[] tempStringArray;

	private CosmeticItem tempItem;

	public List<CatalogItem> catalogItems;

	public bool tryTwice;

	[NonSerialized]
	public CosmeticSet tryOnSet = new CosmeticSet();

	public FittingRoomButton[] fittingRoomButtons;

	public CosmeticStand[] cosmeticStands;

	public List<CosmeticItem> currentCart = new List<CosmeticItem>();

	public PurchaseItemStages currentPurchaseItemStage;

	public CheckoutCartButton[] checkoutCartButtons;

	public PurchaseItemButton leftPurchaseButton;

	public PurchaseItemButton rightPurchaseButton;

	public Text purchaseText;

	public CosmeticItem itemToBuy;

	public HeadModel checkoutHeadModel;

	private List<string> playerIDList = new List<string>();

	private bool foundCosmetic;

	private int attempts;

	private string finalLine;

	private bool purchaseLocked;

	private bool isLastHandTouchedLeft;

	private CosmeticSet cachedSet = new CosmeticSet();

	public Wardrobe[] wardrobes;

	public List<CosmeticItem> unlockedCosmetics = new List<CosmeticItem>();

	public List<CosmeticItem> unlockedHats = new List<CosmeticItem>();

	public List<CosmeticItem> unlockedFaces = new List<CosmeticItem>();

	public List<CosmeticItem> unlockedBadges = new List<CosmeticItem>();

	public List<CosmeticItem> unlockedHoldable = new List<CosmeticItem>();

	public int[] cosmeticsPages = new int[4];

	private List<CosmeticItem>[] itemLists = new List<CosmeticItem>[4];

	private int wardrobeType;

	[NonSerialized]
	public CosmeticSet currentWornSet = new CosmeticSet();

	public string concatStringCosmeticsAllowed = "";

	public Text atmText;

	public string currentAtmString;

	public Text infoText;

	public Text earlyAccessText;

	public Text[] purchaseButtonText;

	public Text dailyText;

	public ATMStages currentATMStage;

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

	private CosmeticItem cosmeticItemVar;

	public EarlyAccessButton[] earlyAccessButtons;

	private BundleList bundleList = new BundleList();

	public string BundleSkuName = "2023_caves_bundle";

	public string BundlePlayfabItemName = "LSABC.";

	public int BundleShinyRocks = 10000;

	public bool buyingBundle;

	public DateTime currentTime;

	public string lastDailyLogin;

	public UserDataRecord userDataRecord;

	public int secondsUntilTomorrow;

	public float secondsToWaitToCheckDaily = 10f;

	private string returnString;

	protected Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse;

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (base.gameObject.activeSelf)
		{
			catalog = "DLC";
			currencyName = "SR";
			nullItem = allCosmetics[0];
			nullItem.isNullItem = true;
			allCosmeticsDict[nullItem.itemName] = nullItem;
			allCosmeticsItemIDsfromDisplayNamesDict[nullItem.displayName] = nullItem.itemName;
			for (int i = 0; i < 10; i++)
			{
				tryOnSet.items[i] = nullItem;
			}
			cosmeticsPages[0] = 0;
			cosmeticsPages[1] = 0;
			cosmeticsPages[2] = 0;
			cosmeticsPages[3] = 0;
			itemLists[0] = unlockedHats;
			itemLists[1] = unlockedFaces;
			itemLists[2] = unlockedBadges;
			itemLists[3] = unlockedHoldable;
			SwitchToStage(ATMStages.Unavailable);
			StartCoroutine(CheckCanGetDaily());
		}
	}

	public void Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("BundleData", delegate(string data)
		{
			bundleList.FromJson(data);
		}, delegate(PlayFabError e)
		{
			Debug.LogError($"Error getting bundle data: {e}");
		});
	}

	public void Update()
	{
	}

	private CosmeticSlots CategoryToNonTransferrableSlot(CosmeticCategory category)
	{
		return category switch
		{
			CosmeticCategory.Hat => CosmeticSlots.Hat, 
			CosmeticCategory.Badge => CosmeticSlots.Badge, 
			CosmeticCategory.Face => CosmeticSlots.Face, 
			_ => CosmeticSlots.Count, 
		};
	}

	private CosmeticSlots DropPositionToCosmeticSlot(BodyDockPositions.DropPositions pos)
	{
		return pos switch
		{
			BodyDockPositions.DropPositions.LeftArm => CosmeticSlots.ArmLeft, 
			BodyDockPositions.DropPositions.RightArm => CosmeticSlots.ArmRight, 
			BodyDockPositions.DropPositions.LeftBack => CosmeticSlots.BackLeft, 
			BodyDockPositions.DropPositions.RightBack => CosmeticSlots.BackRight, 
			BodyDockPositions.DropPositions.Chest => CosmeticSlots.Chest, 
			_ => CosmeticSlots.Count, 
		};
	}

	private static BodyDockPositions.DropPositions CosmeticSlotToDropPosition(CosmeticSlots slot)
	{
		return slot switch
		{
			CosmeticSlots.ArmLeft => BodyDockPositions.DropPositions.LeftArm, 
			CosmeticSlots.ArmRight => BodyDockPositions.DropPositions.RightArm, 
			CosmeticSlots.BackLeft => BodyDockPositions.DropPositions.LeftBack, 
			CosmeticSlots.BackRight => BodyDockPositions.DropPositions.RightBack, 
			CosmeticSlots.Chest => BodyDockPositions.DropPositions.Chest, 
			_ => BodyDockPositions.DropPositions.None, 
		};
	}

	private void SaveItemPreference(CosmeticSlots slot, int slotIdx, CosmeticItem newItem)
	{
		PlayerPrefs.SetString(CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
		PlayerPrefs.Save();
	}

	public void SaveCurrentItemPreferences()
	{
		for (int i = 0; i < 10; i++)
		{
			CosmeticSlots slot = (CosmeticSlots)i;
			SaveItemPreference(slot, i, currentWornSet.items[i]);
		}
	}

	private void ApplyCosmeticToSet(CosmeticSet set, CosmeticItem newItem, int slotIdx, CosmeticSlots slot, bool applyToPlayerPrefs, List<CosmeticSlots> appliedSlots)
	{
		CosmeticItem cosmeticItem = ((set.items[slotIdx].itemName == newItem.itemName) ? nullItem : newItem);
		set.items[slotIdx] = cosmeticItem;
		if (applyToPlayerPrefs)
		{
			SaveItemPreference(slot, slotIdx, cosmeticItem);
		}
		appliedSlots.Add(slot);
	}

	private void PrivApplyCosmeticItemToSet(CosmeticSet set, CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticSlots> appliedSlots)
	{
		if (newItem.isNullItem)
		{
			return;
		}
		if (CosmeticSet.IsHoldable(newItem))
		{
			BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
			foreach (BodyDockPositions.DropPositions item in dockingResult.positionsDisabled)
			{
				CosmeticSlots cosmeticSlots = DropPositionToCosmeticSlot(item);
				if (cosmeticSlots != CosmeticSlots.Count)
				{
					int num = (int)cosmeticSlots;
					set.items[num] = nullItem;
					if (applyToPlayerPrefs)
					{
						SaveItemPreference(cosmeticSlots, num, nullItem);
					}
				}
			}
			{
				foreach (BodyDockPositions.DropPositions item2 in dockingResult.dockedPosition)
				{
					if (item2 != 0)
					{
						CosmeticSlots cosmeticSlots2 = DropPositionToCosmeticSlot(item2);
						int num2 = (int)cosmeticSlots2;
						set.items[num2] = newItem;
						if (applyToPlayerPrefs)
						{
							SaveItemPreference(cosmeticSlots2, num2, newItem);
						}
						appliedSlots.Add(cosmeticSlots2);
					}
				}
				return;
			}
		}
		if (newItem.itemCategory == CosmeticCategory.Gloves)
		{
			CosmeticSlots cosmeticSlots3 = (isLeftHand ? CosmeticSlots.HandLeft : CosmeticSlots.HandRight);
			int slotIdx = (int)cosmeticSlots3;
			ApplyCosmeticToSet(set, newItem, slotIdx, cosmeticSlots3, applyToPlayerPrefs, appliedSlots);
			CosmeticSlots cosmeticSlots4 = CosmeticSet.OppositeSlot(cosmeticSlots3);
			int num3 = (int)cosmeticSlots4;
			if (newItem.bothHandsHoldable)
			{
				ApplyCosmeticToSet(set, nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
				return;
			}
			if (set.items[num3].itemName == newItem.itemName)
			{
				ApplyCosmeticToSet(set, nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
			}
			if (set.items[num3].bothHandsHoldable)
			{
				ApplyCosmeticToSet(set, nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
			}
		}
		else
		{
			CosmeticSlots cosmeticSlots5 = CategoryToNonTransferrableSlot(newItem.itemCategory);
			int slotIdx2 = (int)cosmeticSlots5;
			ApplyCosmeticToSet(set, newItem, slotIdx2, cosmeticSlots5, applyToPlayerPrefs, appliedSlots);
		}
	}

	public List<CosmeticSlots> ApplyCosmeticItemToSet(CosmeticSet set, CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs)
	{
		List<CosmeticSlots> list = new List<CosmeticSlots>(2);
		if (newItem.itemCategory == CosmeticCategory.Set)
		{
			string[] bundledItems = newItem.bundledItems;
			foreach (string itemID in bundledItems)
			{
				CosmeticItem itemFromDict = GetItemFromDict(itemID);
				PrivApplyCosmeticItemToSet(set, itemFromDict, isLeftHand, applyToPlayerPrefs, list);
			}
		}
		else
		{
			PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, list);
		}
		return list;
	}

	public void RemoveCosmeticItemFromSet(CosmeticSet set, string itemName, bool applyToPlayerPrefs)
	{
		cachedSet.CopyItems(set);
		for (int i = 0; i < 10; i++)
		{
			if (set.items[i].displayName == itemName)
			{
				set.items[i] = nullItem;
				if (applyToPlayerPrefs)
				{
					SaveItemPreference((CosmeticSlots)i, i, nullItem);
				}
			}
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		BodyDockPositions component = offlineVRRig.GetComponent<BodyDockPositions>();
		set.ActivateCosmetics(cachedSet, offlineVRRig, component, instance.nullItem, offlineVRRig.cosmeticsObjectRegistry);
	}

	public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
	{
		ApplyCosmeticItemToSet(tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, applyToPlayerPrefs: false);
		UpdateShoppingCart();
	}

	public void PressCosmeticStandButton(CosmeticStand pressedStand)
	{
		searchIndex = currentCart.IndexOf(pressedStand.thisCosmeticItem);
		if (searchIndex != -1)
		{
			currentCart.RemoveAt(searchIndex);
			pressedStand.isOn = false;
			for (int i = 0; i < 10; i++)
			{
				if (pressedStand.thisCosmeticItem.itemName == tryOnSet.items[i].itemName)
				{
					tryOnSet.items[i] = nullItem;
				}
			}
		}
		else
		{
			currentCart.Insert(0, pressedStand.thisCosmeticItem);
			pressedStand.isOn = true;
			if (currentCart.Count > fittingRoomButtons.Length)
			{
				CosmeticStand[] array = cosmeticStands;
				foreach (CosmeticStand cosmeticStand in array)
				{
					if (!(cosmeticStand == null) && cosmeticStand.thisCosmeticItem.itemName == currentCart[fittingRoomButtons.Length].itemName)
					{
						cosmeticStand.isOn = false;
						cosmeticStand.UpdateColor();
						break;
					}
				}
				currentCart.RemoveAt(fittingRoomButtons.Length);
			}
		}
		pressedStand.UpdateColor();
		UpdateShoppingCart();
	}

	public void PressWardrobeItemButton(CosmeticItem cosmeticItem, bool isLeftHand)
	{
		if (cosmeticItem.isNullItem)
		{
			return;
		}
		CosmeticItem itemFromDict = GetItemFromDict(cosmeticItem.itemName);
		foreach (CosmeticSlots item in ApplyCosmeticItemToSet(currentWornSet, itemFromDict, isLeftHand, applyToPlayerPrefs: true))
		{
			tryOnSet.items[(int)item] = nullItem;
		}
		UpdateShoppingCart();
	}

	public void PressWardrobeFunctionButton(string function)
	{
		switch (function)
		{
		case "left":
			cosmeticsPages[wardrobeType]--;
			if (cosmeticsPages[wardrobeType] < 0)
			{
				cosmeticsPages[wardrobeType] = (itemLists[wardrobeType].Count - 1) / 3;
			}
			break;
		case "right":
			cosmeticsPages[wardrobeType]++;
			if (cosmeticsPages[wardrobeType] > (itemLists[wardrobeType].Count - 1) / 3)
			{
				cosmeticsPages[wardrobeType] = 0;
			}
			break;
		case "hat":
			if (wardrobeType == 0)
			{
				return;
			}
			wardrobeType = 0;
			break;
		case "face":
			if (wardrobeType == 1)
			{
				return;
			}
			wardrobeType = 1;
			break;
		case "badge":
			if (wardrobeType == 2)
			{
				return;
			}
			wardrobeType = 2;
			break;
		case "hand":
			if (wardrobeType == 3)
			{
				return;
			}
			wardrobeType = 3;
			break;
		}
		UpdateWardrobeModelsAndButtons();
	}

	public void ClearCheckout()
	{
		itemToBuy = allCosmetics[0];
		checkoutHeadModel.SetCosmeticActive(itemToBuy.displayName);
		currentPurchaseItemStage = PurchaseItemStages.Start;
		ProcessPurchaseItemState(null, isLeftHand: false);
	}

	public void PressCheckoutCartButton(CheckoutCartButton pressedCheckoutCartButton, bool isLeftHand)
	{
		if (currentPurchaseItemStage == PurchaseItemStages.Buying)
		{
			return;
		}
		currentPurchaseItemStage = PurchaseItemStages.CheckoutButtonPressed;
		tryOnSet.ClearSet(nullItem);
		if (itemToBuy.displayName == pressedCheckoutCartButton.currentCosmeticItem.displayName)
		{
			itemToBuy = allCosmetics[0];
			checkoutHeadModel.SetCosmeticActive(itemToBuy.displayName);
		}
		else
		{
			itemToBuy = pressedCheckoutCartButton.currentCosmeticItem;
			checkoutHeadModel.SetCosmeticActive(itemToBuy.displayName);
			if (itemToBuy.bundledItems != null && itemToBuy.bundledItems.Length != 0)
			{
				List<string> list = new List<string>();
				string[] bundledItems = itemToBuy.bundledItems;
				foreach (string itemID in bundledItems)
				{
					tempItem = GetItemFromDict(itemID);
					list.Add(tempItem.displayName);
				}
				checkoutHeadModel.SetCosmeticActiveArray(list.ToArray(), new bool[list.Count]);
			}
			ApplyCosmeticItemToSet(tryOnSet, pressedCheckoutCartButton.currentCosmeticItem, isLeftHand, applyToPlayerPrefs: false);
		}
		ProcessPurchaseItemState(null, isLeftHand);
		UpdateShoppingCart();
	}

	public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
	{
		ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
	}

	public void PressEarlyAccessButton()
	{
		SwitchToStage(ATMStages.Begin);
		currentPurchaseItemStage = PurchaseItemStages.Start;
		ProcessPurchaseItemState("left", isLeftHand: false);
		itemToPurchase = BundlePlayfabItemName;
		shinyRocksCost = BundleShinyRocks;
		SteamPurchase();
		SwitchToStage(ATMStages.Purchasing);
	}

	public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
	{
		switch (currentPurchaseItemStage)
		{
		case PurchaseItemStages.Start:
			itemToBuy = nullItem;
			FormattedPurchaseText("SELECT AN ITEM FROM YOUR CART TO PURCHASE!");
			UpdateShoppingCart();
			break;
		case PurchaseItemStages.CheckoutButtonPressed:
			searchIndex = unlockedCosmetics.FindIndex((CosmeticItem x) => itemToBuy.itemName == x.itemName);
			if (searchIndex > -1)
			{
				FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!");
				leftPurchaseButton.myText.text = "-";
				rightPurchaseButton.myText.text = "-";
				leftPurchaseButton.buttonRenderer.material = leftPurchaseButton.pressedMaterial;
				rightPurchaseButton.buttonRenderer.material = rightPurchaseButton.pressedMaterial;
				currentPurchaseItemStage = PurchaseItemStages.ItemOwned;
			}
			else if (itemToBuy.cost <= currencyBalance)
			{
				FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?");
				leftPurchaseButton.myText.text = "NO!";
				rightPurchaseButton.myText.text = "YES!";
				leftPurchaseButton.buttonRenderer.material = leftPurchaseButton.unpressedMaterial;
				rightPurchaseButton.buttonRenderer.material = rightPurchaseButton.unpressedMaterial;
				currentPurchaseItemStage = PurchaseItemStages.ItemSelected;
			}
			else
			{
				FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!");
				leftPurchaseButton.myText.text = "-";
				rightPurchaseButton.myText.text = "-";
				leftPurchaseButton.buttonRenderer.material = leftPurchaseButton.pressedMaterial;
				rightPurchaseButton.buttonRenderer.material = rightPurchaseButton.pressedMaterial;
				currentPurchaseItemStage = PurchaseItemStages.Start;
			}
			break;
		case PurchaseItemStages.ItemSelected:
			if (buttonSide == "right")
			{
				FormattedPurchaseText("ARE YOU REALLY SURE?");
				leftPurchaseButton.myText.text = "YES! I NEED IT!";
				rightPurchaseButton.myText.text = "LET ME THINK ABOUT IT";
				leftPurchaseButton.buttonRenderer.material = leftPurchaseButton.unpressedMaterial;
				rightPurchaseButton.buttonRenderer.material = rightPurchaseButton.unpressedMaterial;
				currentPurchaseItemStage = PurchaseItemStages.FinalPurchaseAcknowledgement;
			}
			else
			{
				currentPurchaseItemStage = PurchaseItemStages.CheckoutButtonPressed;
				ProcessPurchaseItemState(null, isLeftHand);
			}
			break;
		case PurchaseItemStages.FinalPurchaseAcknowledgement:
			if (buttonSide == "left")
			{
				FormattedPurchaseText("PURCHASING ITEM...");
				leftPurchaseButton.myText.text = "-";
				rightPurchaseButton.myText.text = "-";
				leftPurchaseButton.buttonRenderer.material = leftPurchaseButton.pressedMaterial;
				rightPurchaseButton.buttonRenderer.material = rightPurchaseButton.pressedMaterial;
				currentPurchaseItemStage = PurchaseItemStages.Buying;
				isLastHandTouchedLeft = isLeftHand;
				PurchaseItem();
			}
			else
			{
				currentPurchaseItemStage = PurchaseItemStages.CheckoutButtonPressed;
				ProcessPurchaseItemState(null, isLeftHand);
			}
			break;
		case PurchaseItemStages.Failure:
			FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.");
			leftPurchaseButton.myText.text = "-";
			rightPurchaseButton.myText.text = "-";
			leftPurchaseButton.buttonRenderer.material = leftPurchaseButton.pressedMaterial;
			rightPurchaseButton.buttonRenderer.material = rightPurchaseButton.pressedMaterial;
			break;
		case PurchaseItemStages.Success:
		{
			FormattedPurchaseText("SUCCESS! ENJOY YOUR NEW ITEM!");
			GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed += itemToBuy.itemName;
			CosmeticItem itemFromDict = GetItemFromDict(itemToBuy.itemName);
			if (itemFromDict.bundledItems != null)
			{
				string[] bundledItems = itemFromDict.bundledItems;
				foreach (string text in bundledItems)
				{
					GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed += text;
				}
			}
			tryOnSet.ClearSet(nullItem);
			UpdateShoppingCart();
			ApplyCosmeticItemToSet(currentWornSet, itemFromDict, isLeftHand, applyToPlayerPrefs: true);
			UpdateShoppingCart();
			leftPurchaseButton.myText.text = "-";
			rightPurchaseButton.myText.text = "-";
			leftPurchaseButton.buttonRenderer.material = leftPurchaseButton.pressedMaterial;
			rightPurchaseButton.buttonRenderer.material = rightPurchaseButton.pressedMaterial;
			break;
		}
		case PurchaseItemStages.ItemOwned:
		case PurchaseItemStages.Buying:
			break;
		}
	}

	public void FormattedPurchaseText(string finalLineVar)
	{
		finalLine = finalLineVar;
		purchaseText.text = "SELECTION: " + GetItemDisplayName(itemToBuy) + "\nITEM COST: " + itemToBuy.cost + "\nYOU HAVE: " + currencyBalance + "\n\n" + finalLine;
	}

	public void PurchaseItem()
	{
		PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
		{
			ItemId = itemToBuy.itemName,
			Price = itemToBuy.cost,
			VirtualCurrency = currencyName,
			CatalogVersion = catalog
		}, delegate(PurchaseItemResult result)
		{
			if (result.Items.Count > 0)
			{
				foreach (ItemInstance item in result.Items)
				{
					CosmeticItem itemFromDict = GetItemFromDict(itemToBuy.itemName);
					if (itemFromDict.itemCategory == CosmeticCategory.Set)
					{
						UnlockItem(item.ItemId);
						string[] bundledItems = itemFromDict.bundledItems;
						foreach (string itemIdToUnlock in bundledItems)
						{
							UnlockItem(itemIdToUnlock);
						}
					}
					else
					{
						UnlockItem(item.ItemId);
					}
				}
				if (PhotonNetwork.InRoom)
				{
					RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
					WebFlags flags = new WebFlags(1);
					raiseEventOptions.Flags = flags;
					object[] eventContent = new object[0];
					PhotonNetwork.RaiseEvent(9, eventContent, raiseEventOptions, SendOptions.SendReliable);
				}
				StartCoroutine(CheckIfMyCosmeticsUpdated(itemToBuy.itemName));
				currentPurchaseItemStage = PurchaseItemStages.Success;
				currencyBalance -= itemToBuy.cost;
				UpdateShoppingCart();
				ProcessPurchaseItemState(null, isLastHandTouchedLeft);
			}
			else
			{
				currentPurchaseItemStage = PurchaseItemStages.Failure;
				ProcessPurchaseItemState(null, isLeftHand: false);
			}
		}, delegate
		{
			currentPurchaseItemStage = PurchaseItemStages.Failure;
			ProcessPurchaseItemState(null, isLeftHand: false);
		});
	}

	private void UnlockItem(string itemIdToUnlock)
	{
		int num = allCosmetics.FindIndex((CosmeticItem x) => itemIdToUnlock == x.itemName);
		if (num <= -1)
		{
			return;
		}
		if (!unlockedCosmetics.Contains(allCosmetics[num]))
		{
			unlockedCosmetics.Add(allCosmetics[num]);
		}
		concatStringCosmeticsAllowed += allCosmetics[num].itemName;
		switch (allCosmetics[num].itemCategory)
		{
		case CosmeticCategory.Hat:
			if (!unlockedHats.Contains(allCosmetics[num]))
			{
				unlockedHats.Add(allCosmetics[num]);
			}
			break;
		case CosmeticCategory.Badge:
			if (!unlockedBadges.Contains(allCosmetics[num]))
			{
				unlockedBadges.Add(allCosmetics[num]);
			}
			break;
		case CosmeticCategory.Face:
			if (!unlockedFaces.Contains(allCosmetics[num]))
			{
				unlockedFaces.Add(allCosmetics[num]);
			}
			break;
		case CosmeticCategory.Holdable:
		case CosmeticCategory.Gloves:
		case CosmeticCategory.Slingshot:
			if (!unlockedHoldable.Contains(allCosmetics[num]))
			{
				unlockedHoldable.Add(allCosmetics[num]);
			}
			break;
		case CosmeticCategory.Count:
		case CosmeticCategory.Set:
			break;
		}
	}

	private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
	{
		yield return new WaitForSeconds(1f);
		foundCosmetic = false;
		attempts = 0;
		while (!foundCosmetic && attempts < 10 && PhotonNetwork.InRoom)
		{
			playerIDList.Clear();
			playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
			PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
			{
				Keys = playerIDList,
				SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper()
			}, delegate(GetSharedGroupDataResult result)
			{
				attempts++;
				foreach (KeyValuePair<string, SharedGroupDataRecord> datum in result.Data)
				{
					if (datum.Value.Value.Contains(itemToBuyID))
					{
						if (GorillaGameManager.instance != null)
						{
							GorillaGameManager.instance.photonView.RPC("UpdatePlayerCosmetic", RpcTarget.Others);
						}
						foundCosmetic = true;
					}
				}
			}, delegate(PlayFabError error)
			{
				attempts++;
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}
				else if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					PhotonNetwork.Disconnect();
					UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
					UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						UnityEngine.Object.Destroy(array[i]);
					}
				}
			});
			yield return new WaitForSeconds(1f);
		}
	}

	public void UpdateWardrobeModelsAndButtons()
	{
		Wardrobe[] array = wardrobes;
		for (int i = 0; i < array.Length; i++)
		{
			Wardrobe wardrobe = array[i];
			wardrobe.wardrobeItemButtons[0].currentCosmeticItem = ((cosmeticsPages[wardrobeType] * 3 < itemLists[wardrobeType].Count) ? itemLists[wardrobeType][cosmeticsPages[wardrobeType] * 3] : nullItem);
			wardrobe.wardrobeItemButtons[1].currentCosmeticItem = ((cosmeticsPages[wardrobeType] * 3 + 1 < itemLists[wardrobeType].Count) ? itemLists[wardrobeType][cosmeticsPages[wardrobeType] * 3 + 1] : nullItem);
			wardrobe.wardrobeItemButtons[2].currentCosmeticItem = ((cosmeticsPages[wardrobeType] * 3 + 2 < itemLists[wardrobeType].Count) ? itemLists[wardrobeType][cosmeticsPages[wardrobeType] * 3 + 2] : nullItem);
			for (iterator = 0; iterator < wardrobe.wardrobeItemButtons.Length; iterator++)
			{
				CosmeticItem currentCosmeticItem = wardrobe.wardrobeItemButtons[iterator].currentCosmeticItem;
				wardrobe.wardrobeItemButtons[iterator].isOn = !currentCosmeticItem.isNullItem && AnyMatch(currentWornSet, currentCosmeticItem);
				wardrobe.wardrobeItemButtons[iterator].UpdateColor();
			}
			wardrobe.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[0].currentCosmeticItem.displayName);
			wardrobe.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[1].currentCosmeticItem.displayName);
			wardrobe.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[2].currentCosmeticItem.displayName);
			wardrobe.selfDoll.SetCosmeticActiveArray(currentWornSet.ToDisplayNameArray(), currentWornSet.ToOnRightSideArray());
		}
	}

	public void UpdateShoppingCart()
	{
		for (iterator = 0; iterator < fittingRoomButtons.Length; iterator++)
		{
			if (iterator < currentCart.Count)
			{
				fittingRoomButtons[iterator].currentCosmeticItem = currentCart[iterator];
				checkoutCartButtons[iterator].currentCosmeticItem = currentCart[iterator];
				checkoutCartButtons[iterator].isOn = checkoutCartButtons[iterator].currentCosmeticItem.itemName == itemToBuy.itemName;
				fittingRoomButtons[iterator].isOn = AnyMatch(tryOnSet, fittingRoomButtons[iterator].currentCosmeticItem);
			}
			else
			{
				checkoutCartButtons[iterator].currentCosmeticItem = nullItem;
				fittingRoomButtons[iterator].currentCosmeticItem = nullItem;
				checkoutCartButtons[iterator].isOn = false;
				fittingRoomButtons[iterator].isOn = false;
			}
			checkoutCartButtons[iterator].currentImage.sprite = checkoutCartButtons[iterator].currentCosmeticItem.itemPicture;
			fittingRoomButtons[iterator].currentImage.sprite = fittingRoomButtons[iterator].currentCosmeticItem.itemPicture;
			checkoutCartButtons[iterator].UpdateColor();
			fittingRoomButtons[iterator].UpdateColor();
		}
		UpdateWardrobeModelsAndButtons();
		GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(currentWornSet, tryOnSet);
		if (GorillaTagger.Instance.myVRRig != null)
		{
			string[] array = currentWornSet.ToDisplayNameArray();
			string[] array2 = tryOnSet.ToDisplayNameArray();
			GorillaTagger.Instance.myVRRig.RPC("UpdateCosmeticsWithTryon", RpcTarget.All, array, array2);
		}
	}

	public CosmeticItem GetItemFromDict(string itemID)
	{
		if (!allCosmeticsDict.TryGetValue(itemID, out cosmeticItemVar))
		{
			return nullItem;
		}
		return cosmeticItemVar;
	}

	public string GetItemNameFromDisplayName(string displayName)
	{
		if (!allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, out returnString))
		{
			return "null";
		}
		return returnString;
	}

	public bool AnyMatch(CosmeticSet set, CosmeticItem item)
	{
		if (item.itemCategory != CosmeticCategory.Set)
		{
			return set.IsActive(item.displayName);
		}
		if (item.bundledItems.Length == 1)
		{
			return AnyMatch(set, GetItemFromDict(item.bundledItems[0]));
		}
		if (item.bundledItems.Length == 2)
		{
			if (!AnyMatch(set, GetItemFromDict(item.bundledItems[0])))
			{
				return AnyMatch(set, GetItemFromDict(item.bundledItems[1]));
			}
			return true;
		}
		if (item.bundledItems.Length >= 3)
		{
			if (!AnyMatch(set, GetItemFromDict(item.bundledItems[0])) && !AnyMatch(set, GetItemFromDict(item.bundledItems[1])))
			{
				return AnyMatch(set, GetItemFromDict(item.bundledItems[2]));
			}
			return true;
		}
		return false;
	}

	public void Initialize()
	{
		if (base.gameObject.activeSelf)
		{
			GetUserCosmeticsAllowed();
		}
	}

	public void GetLastDailyLogin()
	{
		PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest(), delegate(GetUserDataResult result)
		{
			if (result.Data.TryGetValue("DailyLogin", out userDataRecord))
			{
				lastDailyLogin = userDataRecord.Value;
			}
			else
			{
				lastDailyLogin = "NONE";
				StartCoroutine(GetMyDaily());
			}
		}, delegate(PlayFabError error)
		{
			lastDailyLogin = "FAILED";
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
				UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
		});
	}

	private IEnumerator CheckCanGetDaily()
	{
		while (true)
		{
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				currentTime = new DateTime((GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) * 10000);
				secondsUntilTomorrow = (int)(currentTime.AddDays(1.0).Date - currentTime).TotalSeconds;
				if (lastDailyLogin == null || lastDailyLogin == "")
				{
					GetLastDailyLogin();
				}
				else if (currentTime.ToString("o").Substring(0, 10) == lastDailyLogin)
				{
					checkedDaily = true;
					gotMyDaily = true;
				}
				else if (currentTime.ToString("o").Substring(0, 10) != lastDailyLogin)
				{
					checkedDaily = true;
					gotMyDaily = false;
					StartCoroutine(GetMyDaily());
				}
				else if (lastDailyLogin == "FAILED")
				{
					GetLastDailyLogin();
				}
				secondsToWaitToCheckDaily = (checkedDaily ? 60f : 10f);
				UpdateCurrencyBoard();
				yield return new WaitForSeconds(secondsToWaitToCheckDaily);
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
	}

	private IEnumerator GetMyDaily()
	{
		yield return new WaitForSeconds(10f);
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
		{
			FunctionName = "TryDistributeCurrency",
			FunctionParameter = new { }
		}, delegate
		{
			GetCurrencyBalance();
			GetLastDailyLogin();
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
				UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
				UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
		});
	}

	public void GetUserCosmeticsAllowed()
	{
		PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
		{
			PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
			{
				CatalogVersion = catalog
			}, delegate(GetCatalogItemsResult result2)
			{
				unlockedCosmetics.Clear();
				unlockedHats.Clear();
				unlockedBadges.Clear();
				unlockedFaces.Clear();
				unlockedHoldable.Clear();
				catalogItems = result2.Catalog;
				foreach (CatalogItem catalogItem in catalogItems)
				{
					searchIndex = allCosmetics.FindIndex((CosmeticItem x) => catalogItem.DisplayName == x.displayName);
					if (searchIndex > -1)
					{
						tempStringArray = null;
						hasPrice = false;
						if (catalogItem.Bundle != null)
						{
							tempStringArray = catalogItem.Bundle.BundledItems.ToArray();
						}
						if (catalogItem.VirtualCurrencyPrices.TryGetValue(currencyName, out var value))
						{
							hasPrice = true;
						}
						allCosmetics[searchIndex] = new CosmeticItem
						{
							itemName = catalogItem.ItemId,
							displayName = catalogItem.DisplayName,
							cost = (int)value,
							itemPicture = allCosmetics[searchIndex].itemPicture,
							itemCategory = allCosmetics[searchIndex].itemCategory,
							bundledItems = tempStringArray,
							canTryOn = hasPrice,
							bothHandsHoldable = allCosmetics[searchIndex].bothHandsHoldable,
							overrideDisplayName = allCosmetics[searchIndex].overrideDisplayName
						};
						allCosmeticsDict[allCosmetics[searchIndex].itemName] = allCosmetics[searchIndex];
						allCosmeticsItemIDsfromDisplayNamesDict[allCosmetics[searchIndex].displayName] = allCosmetics[searchIndex].itemName;
					}
				}
				for (int num = allCosmetics.Count - 1; num > -1; num--)
				{
					tempItem = allCosmetics[num];
					if (tempItem.itemCategory == CosmeticCategory.Set && tempItem.canTryOn)
					{
						string[] bundledItems = tempItem.bundledItems;
						foreach (string setItemName in bundledItems)
						{
							searchIndex = allCosmetics.FindIndex((CosmeticItem x) => setItemName == x.itemName);
							if (searchIndex > -1)
							{
								tempItem = new CosmeticItem
								{
									itemName = allCosmetics[searchIndex].itemName,
									displayName = allCosmetics[searchIndex].displayName,
									cost = allCosmetics[searchIndex].cost,
									itemPicture = allCosmetics[searchIndex].itemPicture,
									itemCategory = allCosmetics[searchIndex].itemCategory,
									overrideDisplayName = allCosmetics[searchIndex].overrideDisplayName,
									bothHandsHoldable = allCosmetics[searchIndex].bothHandsHoldable,
									canTryOn = true
								};
								allCosmetics[searchIndex] = tempItem;
								allCosmeticsDict[allCosmetics[searchIndex].itemName] = allCosmetics[searchIndex];
								allCosmeticsItemIDsfromDisplayNamesDict[allCosmetics[searchIndex].displayName] = allCosmetics[searchIndex].itemName;
							}
						}
					}
				}
				foreach (ItemInstance item in result.Inventory)
				{
					if (item.ItemId == "Early Access Supporter Pack")
					{
						unlockedCosmetics.Add(allCosmetics[1]);
						unlockedCosmetics.Add(allCosmetics[10]);
						unlockedCosmetics.Add(allCosmetics[11]);
						unlockedCosmetics.Add(allCosmetics[12]);
						unlockedCosmetics.Add(allCosmetics[13]);
						unlockedCosmetics.Add(allCosmetics[14]);
						unlockedCosmetics.Add(allCosmetics[15]);
						unlockedCosmetics.Add(allCosmetics[31]);
						unlockedCosmetics.Add(allCosmetics[32]);
						unlockedCosmetics.Add(allCosmetics[38]);
						unlockedCosmetics.Add(allCosmetics[67]);
						unlockedCosmetics.Add(allCosmetics[68]);
					}
					else
					{
						if (item.ItemId == BundlePlayfabItemName)
						{
							EarlyAccessButton[] array3 = earlyAccessButtons;
							for (int k = 0; k < array3.Length; k++)
							{
								_ = array3[k];
								AlreadyOwnAllBundleButtons();
							}
						}
						searchIndex = allCosmetics.FindIndex((CosmeticItem x) => item.ItemId == x.itemName);
						if (searchIndex > -1)
						{
							unlockedCosmetics.Add(allCosmetics[searchIndex]);
						}
					}
				}
				searchIndex = allCosmetics.FindIndex((CosmeticItem x) => "Slingshot" == x.itemName);
				allCosmeticsDict["Slingshot"] = allCosmetics[searchIndex];
				allCosmeticsItemIDsfromDisplayNamesDict[allCosmetics[searchIndex].displayName] = allCosmetics[searchIndex].itemName;
				foreach (CosmeticItem unlockedCosmetic in unlockedCosmetics)
				{
					if (unlockedCosmetic.itemCategory == CosmeticCategory.Hat && !unlockedHats.Contains(unlockedCosmetic))
					{
						unlockedHats.Add(unlockedCosmetic);
					}
					else if (unlockedCosmetic.itemCategory == CosmeticCategory.Face && !unlockedFaces.Contains(unlockedCosmetic))
					{
						unlockedFaces.Add(unlockedCosmetic);
					}
					else if (unlockedCosmetic.itemCategory == CosmeticCategory.Badge && !unlockedBadges.Contains(unlockedCosmetic))
					{
						unlockedBadges.Add(unlockedCosmetic);
					}
					else if (unlockedCosmetic.itemCategory == CosmeticCategory.Holdable && !unlockedHoldable.Contains(unlockedCosmetic))
					{
						unlockedHoldable.Add(unlockedCosmetic);
					}
					else if (unlockedCosmetic.itemCategory == CosmeticCategory.Gloves && !unlockedHoldable.Contains(unlockedCosmetic))
					{
						unlockedHoldable.Add(unlockedCosmetic);
					}
					else if (unlockedCosmetic.itemCategory == CosmeticCategory.Slingshot && !unlockedHoldable.Contains(unlockedCosmetic))
					{
						unlockedHoldable.Add(unlockedCosmetic);
					}
					concatStringCosmeticsAllowed += unlockedCosmetic.itemName;
				}
				CosmeticStand[] array4 = cosmeticStands;
				foreach (CosmeticStand cosmeticStand in array4)
				{
					if (cosmeticStand != null)
					{
						cosmeticStand.InitializeCosmetic();
					}
				}
				currencyBalance = result.VirtualCurrency[currencyName];
				playedInBeta = result.VirtualCurrency.TryGetValue("TC", out var value2) && value2 > 0;
				currentWornSet.LoadFromPlayerPreferences(this);
				SwitchToStage(ATMStages.Begin);
				ProcessPurchaseItemState(null, isLeftHand: false);
				UpdateShoppingCart();
				UpdateCurrencyBoard();
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
					UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
					UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array2 = UnityEngine.Object.FindObjectsOfType<GameObject>();
					for (int j = 0; j < array2.Length; j++)
					{
						UnityEngine.Object.Destroy(array2[j]);
					}
				}
				if (!tryTwice)
				{
					tryTwice = true;
					GetUserCosmeticsAllowed();
				}
			});
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
				UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
				UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
			if (!tryTwice)
			{
				tryTwice = true;
				GetUserCosmeticsAllowed();
			}
		});
	}

	private void SteamPurchase()
	{
		Debug.Log("attempting to purchase item through steam");
		PlayFabClientAPI.StartPurchase(new StartPurchaseRequest
		{
			CatalogVersion = catalog,
			Items = new List<ItemPurchaseRequest>
			{
				new ItemPurchaseRequest
				{
					ItemId = itemToPurchase,
					Quantity = 1u,
					Annotation = "Purchased via in-game store"
				}
			}
		}, delegate(StartPurchaseResult result)
		{
			Debug.Log("successfully started purchase. attempted to pay for purchase through steam");
			currentPurchaseID = result.OrderId;
			PlayFabClientAPI.PayForPurchase(new PayForPurchaseRequest
			{
				OrderId = currentPurchaseID,
				ProviderName = "Steam",
				Currency = "RM"
			}, delegate
			{
				Debug.Log("succeeded on sending request for paying with steam! waiting for response");
				buyingBundle = true;
				m_MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnAuthorizationResponse);
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
					UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
					UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
					GameObject[] array2 = UnityEngine.Object.FindObjectsOfType<GameObject>();
					for (int j = 0; j < array2.Length; j++)
					{
						UnityEngine.Object.Destroy(array2[j]);
					}
				}
				Debug.Log("failed to send request to purchase with steam!");
				Debug.Log(error.ToString());
				SwitchToStage(ATMStages.Failure);
			});
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
				UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
				UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
			Debug.Log("error in starting purchase!");
		});
	}

	public void ProcessATMState(string currencyButton)
	{
		switch (currentATMStage)
		{
		case ATMStages.Begin:
			SwitchToStage(ATMStages.Menu);
			break;
		case ATMStages.Menu:
			switch (currencyButton)
			{
			case "one":
				SwitchToStage(ATMStages.Balance);
				break;
			case "two":
				SwitchToStage(ATMStages.Choose);
				break;
			case "four":
				SwitchToStage(ATMStages.Begin);
				break;
			}
			break;
		case ATMStages.Balance:
			if (currencyButton == "four")
			{
				SwitchToStage(ATMStages.Menu);
			}
			break;
		case ATMStages.Choose:
			switch (currencyButton)
			{
			case "one":
				numShinyRocksToBuy = 1000;
				shinyRocksCost = 4.99f;
				itemToPurchase = "1000SHINYROCKS";
				SwitchToStage(ATMStages.Confirm);
				break;
			case "two":
				numShinyRocksToBuy = 2200;
				shinyRocksCost = 9.99f;
				itemToPurchase = "2200SHINYROCKS";
				SwitchToStage(ATMStages.Confirm);
				break;
			case "three":
				numShinyRocksToBuy = 5000;
				shinyRocksCost = 19.99f;
				itemToPurchase = "5000SHINYROCKS";
				SwitchToStage(ATMStages.Confirm);
				break;
			case "four":
				SwitchToStage(ATMStages.Menu);
				break;
			}
			break;
		case ATMStages.Confirm:
			if (!(currencyButton == "one"))
			{
				if (currencyButton == "four")
				{
					SwitchToStage(ATMStages.Choose);
				}
			}
			else
			{
				SteamPurchase();
				SwitchToStage(ATMStages.Purchasing);
			}
			break;
		default:
			SwitchToStage(ATMStages.Menu);
			break;
		case ATMStages.Unavailable:
		case ATMStages.Purchasing:
			break;
		}
	}

	public void SwitchToStage(ATMStages newStage)
	{
		currentATMStage = newStage;
		switch (newStage)
		{
		case ATMStages.Unavailable:
			atmText.text = "ATM NOT AVAILABLE! PLEASE TRY AGAIN LATER!";
			atmButtonsText.text = "";
			break;
		case ATMStages.Begin:
			atmText.text = "WELCOME! PRESS ANY BUTTON TO BEGIN.";
			atmButtonsText.text = "\n\n\n\n\n\n\n\n\nBEGIN   -->";
			break;
		case ATMStages.Menu:
			atmText.text = "CHECK YOUR BALANCE OR PURCHASE MORE SHINY ROCKS.";
			atmButtonsText.text = "BALANCE-- >\n\n\nPURCHASE-->\n\n\n\n\n\nBACK    -->";
			break;
		case ATMStages.Balance:
			atmText.text = "CURRENT BALANCE:\n\n" + currencyBalance;
			atmButtonsText.text = "\n\n\n\n\n\n\n\n\nBACK    -->";
			break;
		case ATMStages.Choose:
			atmText.text = "CHOOSE AN AMOUNT OF SHINY ROCKS TO PURCHASE.";
			atmButtonsText.text = "$4.99 FOR -->\n1000\n\n$9.99 FOR -->\n2200\n\n$19.99 FOR-->\n5000\n\nBACK -->";
			break;
		case ATMStages.Confirm:
			atmText.text = "YOU HAVE CHOSEN TO PURCHASE " + numShinyRocksToBuy + " SHINY ROCKS FOR $" + shinyRocksCost + ". CONFIRM TO LAUNCH A STEAM WINDOW TO COMPLETE YOUR PURCHASE.";
			atmButtonsText.text = "CONFIRM -->\n\n\n\n\n\n\n\n\nBACK    -->";
			break;
		case ATMStages.Purchasing:
			atmText.text = "PURCHASING IN STEAM...";
			atmButtonsText.text = "";
			break;
		case ATMStages.Success:
			atmText.text = "SUCCESS! NEW SHINY ROCKS BALANCE: " + (currencyBalance + numShinyRocksToBuy);
			atmButtonsText.text = "\n\n\n\n\n\n\n\n\nRETURN  -->";
			break;
		case ATMStages.Failure:
			atmText.text = "PURCHASE CANCELED. NO FUNDS WERE SPENT.";
			atmButtonsText.text = "\n\n\n\n\n\n\n\n\nRETURN  -->";
			break;
		case ATMStages.Locked:
			atmText.text = "UNABLE TO PURCHASE AT THIS TIME. PLEASE RESTART THE GAME OR TRY AGAIN LATER.";
			atmButtonsText.text = "\n\n\n\n\n\n\n\n\nRETURN  -->";
			break;
		}
	}

	public void PressCurrencyPurchaseButton(string currencyPurchaseSize)
	{
		ProcessATMState(currencyPurchaseSize);
	}

	private void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t pCallback)
	{
		PlayFabClientAPI.ConfirmPurchase(new ConfirmPurchaseRequest
		{
			OrderId = currentPurchaseID
		}, delegate
		{
			if (buyingBundle)
			{
				buyingBundle = false;
				if (PhotonNetwork.InRoom)
				{
					RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
					WebFlags flags = new WebFlags(1);
					raiseEventOptions.Flags = flags;
					object[] eventContent = new object[0];
					PhotonNetwork.RaiseEvent(9, eventContent, raiseEventOptions, SendOptions.SendReliable);
				}
				StartCoroutine(CheckIfMyCosmeticsUpdated(BundlePlayfabItemName));
			}
			SwitchToStage(ATMStages.Success);
			GetCurrencyBalance();
			UpdateCurrencyBoard();
			GetUserCosmeticsAllowed();
			GorillaTagger.Instance.offlineVRRig.GetUserCosmeticsAllowed();
		}, delegate
		{
			atmText.text = "PURCHASE CANCELLED!\n\nCURRENT BALANCE IS: ";
			UpdateCurrencyBoard();
			SwitchToStage(ATMStages.Failure);
		});
	}

	public void UpdateCurrencyBoard()
	{
		FormattedPurchaseText(finalLine);
		dailyText.text = ((!checkedDaily) ? "CHECKING DAILY ROCKS..." : (gotMyDaily ? "SUCCESSFULLY GOT DAILY ROCKS!" : "WAITING TO GET DAILY ROCKS..."));
		currencyBoardText.text = currencyBalance + "\n\n" + secondsUntilTomorrow / 3600 + " HR, " + secondsUntilTomorrow % 3600 / 60 + "MIN";
	}

	public void GetCurrencyBalance()
	{
		PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
		{
			currencyBalance = result.VirtualCurrency[currencyName];
			UpdateCurrencyBoard();
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
				UnityEngine.Object.DestroyImmediate(PhotonNetworkController.Instance);
				UnityEngine.Object.DestroyImmediate(GorillaLocomotion.Player.Instance);
				GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
		});
	}

	public string GetItemDisplayName(CosmeticItem item)
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
		EarlyAccessButton[] array = earlyAccessButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].AlreadyOwn();
		}
	}
}
