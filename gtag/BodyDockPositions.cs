using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BodyDockPositions : MonoBehaviourPunCallbacks
{
	[Flags]
	public enum DropPositions
	{
		LeftArm = 1,
		RightArm = 2,
		Chest = 4,
		LeftBack = 8,
		RightBack = 0x10,
		MaxDropPostions = 5,
		All = 0x1F,
		None = 0
	}

	public class DockingResult
	{
		public List<DropPositions> positionsDisabled;

		public List<DropPositions> dockedPosition;

		public DockingResult()
		{
			dockedPosition = new List<DropPositions>(2);
			positionsDisabled = new List<DropPositions>(2);
		}
	}

	public VRRig myRig;

	public GameObject[] leftHandThrowables;

	public GameObject[] rightHandThrowables;

	public TransferrableObject[] allObjects;

	private List<int> objectsToEnable = new List<int>();

	private List<int> objectsToDisable = new List<int>();

	public Transform leftHandTransform;

	public Transform rightHandTransform;

	public Transform chestTransform;

	public Transform leftArmTransform;

	public Transform rightArmTransform;

	public Transform leftBackTransform;

	public Transform rightBackTransform;

	public WorldShareableItem leftBackSharableItem;

	public WorldShareableItem rightBackShareableItem;

	public GameObject SharableItemInstance;

	public void Awake()
	{
		for (int i = 0; i < allObjects.Length; i++)
		{
			if (allObjects[i] == null)
			{
				Debug.LogError("BodyDockPositions.allObjects array has a null reference at index " + i + ".", this);
			}
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (object.Equals(myRig.creator, otherPlayer))
		{
			DeallocateSharableInstances();
		}
	}

	public WorldShareableItem AllocateSharableInstance(DropPositions position, Player owner)
	{
		switch (position)
		{
		case DropPositions.LeftBack:
			if ((object)leftBackSharableItem == null)
			{
				leftBackSharableItem = ObjectPools.instance.Instantiate(SharableItemInstance).GetComponent<WorldShareableItem>();
				leftBackSharableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, isLocalOnly: false, dontPropigate: true);
				leftBackSharableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 3);
			}
			return leftBackSharableItem;
		case DropPositions.RightBack:
			if ((object)rightBackShareableItem == null)
			{
				rightBackShareableItem = ObjectPools.instance.Instantiate(SharableItemInstance).GetComponent<WorldShareableItem>();
				rightBackShareableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, isLocalOnly: false, dontPropigate: true);
				rightBackShareableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 4);
			}
			return rightBackShareableItem;
		default:
			throw new ArgumentOutOfRangeException("position", position, null);
		}
	}

	public void DeallocateSharableInstance(WorldShareableItem worldShareable)
	{
		if (worldShareable == null)
		{
			return;
		}
		if (worldShareable == leftBackSharableItem)
		{
			if ((object)leftBackSharableItem == null)
			{
				return;
			}
			leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(leftBackSharableItem.gameObject);
			leftBackSharableItem = null;
		}
		if (worldShareable == rightBackShareableItem && (object)rightBackShareableItem != null)
		{
			rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(rightBackShareableItem.gameObject);
			rightBackShareableItem = null;
		}
	}

	public void DeallocateSharableInstances()
	{
		if ((object)rightBackShareableItem != null)
		{
			rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(rightBackShareableItem.gameObject);
		}
		if ((object)leftBackSharableItem != null)
		{
			leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(leftBackSharableItem.gameObject);
		}
		leftBackSharableItem = null;
		rightBackShareableItem = null;
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		DeallocateSharableInstances();
	}

	public static bool IsPositionLeft(DropPositions pos)
	{
		if (pos != DropPositions.LeftArm)
		{
			return pos == DropPositions.LeftBack;
		}
		return true;
	}

	public int DropZoneStorageUsed(DropPositions dropPosition)
	{
		for (int i = 0; i < myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (myRig.ActiveTransferrableObjectIndex(i) >= 0 && allObjects[myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && allObjects[myRig.ActiveTransferrableObjectIndex(i)].storedZone == dropPosition)
			{
				return myRig.ActiveTransferrableObjectIndex(i);
			}
		}
		return -1;
	}

	public TransferrableObject ItemPositionInUse(DropPositions dropPosition)
	{
		TransferrableObject.PositionState positionState = MapDropPositionToState(dropPosition);
		for (int i = 0; i < myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (myRig.ActiveTransferrableObjectIndex(i) != -1 && allObjects[myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && allObjects[myRig.ActiveTransferrableObjectIndex(i)].currentState == positionState)
			{
				return allObjects[myRig.ActiveTransferrableObjectIndex(i)];
			}
		}
		return null;
	}

	private int EnableTransferrableItem(int allItemsIndex, DropPositions startingPosition, TransferrableObject.PositionState startingState)
	{
		if (allItemsIndex < 0 || allItemsIndex >= allObjects.Length)
		{
			return -1;
		}
		if (myRig != null && myRig.isOfflineVRRig)
		{
			for (int i = 0; i < myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (myRig.ActiveTransferrableObjectIndex(i) == -1)
				{
					string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(allObjects[allItemsIndex].gameObject.name);
					if (myRig.IsItemAllowed(itemNameFromDisplayName))
					{
						myRig.SetActiveTransferrableObjectIndex(i, allItemsIndex);
						myRig.SetTransferrablePosStates(i, startingState);
						myRig.SetTransferrableItemStates(i, (TransferrableObject.ItemStates)0);
						myRig.SetTransferrableDockPosition(i, startingPosition);
						EnableTransferrableGameObject(allItemsIndex, startingPosition, startingState);
						return i;
					}
				}
			}
		}
		return -1;
	}

	public DropPositions ItemActive(int allItemsIndex)
	{
		if (!allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return DropPositions.None;
		}
		return allObjects[allItemsIndex].storedZone;
	}

	public static DropPositions OfflineItemActive(int allItemsIndex)
	{
		BodyDockPositions bodyDockPositions = null;
		if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
		{
			return DropPositions.None;
		}
		bodyDockPositions = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>();
		if (bodyDockPositions == null)
		{
			return DropPositions.None;
		}
		if (!bodyDockPositions.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return DropPositions.None;
		}
		return bodyDockPositions.allObjects[allItemsIndex].storedZone;
	}

	public void DisableTransferrableItem(int index)
	{
		TransferrableObject transferrableObject = allObjects[index];
		if (transferrableObject.gameObject.activeSelf)
		{
			transferrableObject.gameObject.Disable();
			transferrableObject.storedZone = DropPositions.None;
		}
		if (!myRig.isOfflineVRRig)
		{
			return;
		}
		for (int i = 0; i < myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (myRig.ActiveTransferrableObjectIndex(i) == index)
			{
				myRig.SetActiveTransferrableObjectIndex(i, -1);
			}
		}
	}

	public void DisableAllTransferableItems()
	{
		for (int i = 0; i < myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			int num = myRig.ActiveTransferrableObjectIndex(i);
			if (num >= 0 && num < allObjects.Length)
			{
				TransferrableObject obj = allObjects[num];
				obj.gameObject.Disable();
				obj.storedZone = DropPositions.None;
				myRig.SetActiveTransferrableObjectIndex(i, -1);
				myRig.SetTransferrableItemStates(i, (TransferrableObject.ItemStates)0);
				myRig.SetTransferrablePosStates(i, TransferrableObject.PositionState.None);
			}
		}
		DeallocateSharableInstances();
	}

	private bool AllItemsIndexValid(int allItemsIndex)
	{
		if (allItemsIndex != -1)
		{
			return allItemsIndex < allObjects.Length;
		}
		return false;
	}

	public bool PositionAvailable(int allItemIndex, DropPositions startPos)
	{
		return (allObjects[allItemIndex].dockPositions & startPos) != 0;
	}

	public DropPositions FirstAvailablePosition(int allItemIndex)
	{
		for (int i = 0; i < 5; i++)
		{
			DropPositions dropPositions = (DropPositions)(1 << i);
			if ((allObjects[allItemIndex].dockPositions & dropPositions) != 0)
			{
				return dropPositions;
			}
		}
		return DropPositions.None;
	}

	public int TransferrableItemDisable(int allItemsIndex)
	{
		if (OfflineItemActive(allItemsIndex) != 0)
		{
			DisableTransferrableItem(allItemsIndex);
		}
		return 0;
	}

	public void TransferrableItemDisableAtPosition(DropPositions dropPositions)
	{
		int num = DropZoneStorageUsed(dropPositions);
		if (num >= 0)
		{
			TransferrableItemDisable(num);
		}
	}

	public void TransferrableItemEnableAtPosition(string itemName, DropPositions dropPosition)
	{
		if (DropZoneStorageUsed(dropPosition) >= 0)
		{
			return;
		}
		List<int> list = TransferrableObjectIndexFromName(itemName);
		if (list.Count != 0)
		{
			TransferrableObject.PositionState startingState = MapDropPositionToState(dropPosition);
			if (list.Count == 1)
			{
				EnableTransferrableItem(list[0], dropPosition, startingState);
				return;
			}
			int allItemsIndex = (IsPositionLeft(dropPosition) ? list[0] : list[1]);
			EnableTransferrableItem(allItemsIndex, dropPosition, startingState);
		}
	}

	public bool TransferrableItemActive(string transferrableItemName)
	{
		List<int> list = TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int item in list)
		{
			if (TransferrableItemActive(item))
			{
				return true;
			}
		}
		return false;
	}

	public bool TransferrableItemActiveAtPos(string transferrableItemName, DropPositions dropPosition)
	{
		List<int> list = TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int item in list)
		{
			DropPositions dropPositions = TransferrableItemPosition(item);
			if (dropPositions != 0 && dropPositions == dropPosition)
			{
				return true;
			}
		}
		return false;
	}

	public bool TransferrableItemActive(int allItemsIndex)
	{
		return ItemActive(allItemsIndex) != DropPositions.None;
	}

	public TransferrableObject TransferrableItem(int allItemsIndex)
	{
		return allObjects[allItemsIndex];
	}

	public DropPositions TransferrableItemPosition(int allItemsIndex)
	{
		return ItemActive(allItemsIndex);
	}

	public bool DisableTransferrableItem(string transferrableItemName)
	{
		List<int> list = TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int item in list)
		{
			DisableTransferrableItem(item);
		}
		return true;
	}

	public DropPositions OppositePosition(DropPositions pos)
	{
		return pos switch
		{
			DropPositions.LeftArm => DropPositions.RightArm, 
			DropPositions.RightArm => DropPositions.LeftArm, 
			DropPositions.LeftBack => DropPositions.RightBack, 
			DropPositions.RightBack => DropPositions.LeftBack, 
			_ => pos, 
		};
	}

	public DockingResult ToggleWithHandedness(string transferrableItemName, bool isLeftHand, bool bothHands)
	{
		List<int> list = TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return new DockingResult();
		}
		if (!AllItemsIndexValid(list[0]))
		{
			return new DockingResult();
		}
		DropPositions startingPos = ((!isLeftHand) ? (((allObjects[list[0]].dockPositions & DropPositions.LeftArm) != 0) ? DropPositions.LeftArm : DropPositions.RightBack) : (((allObjects[list[0]].dockPositions & DropPositions.LeftArm) != 0) ? DropPositions.RightArm : DropPositions.LeftBack));
		return ToggleTransferrableItem(transferrableItemName, startingPos, bothHands);
	}

	public DockingResult ToggleTransferrableItem(string transferrableItemName, DropPositions startingPos, bool bothHands)
	{
		DockingResult dockingResult = new DockingResult();
		List<int> list = TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return dockingResult;
		}
		if (bothHands && list.Count == 2)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int allItemsIndex = list[i];
				DropPositions dropPositions = OfflineItemActive(allItemsIndex);
				if (dropPositions != 0)
				{
					TransferrableItemDisable(allItemsIndex);
					dockingResult.positionsDisabled.Add(dropPositions);
				}
			}
			if (dockingResult.positionsDisabled.Count >= 1)
			{
				return dockingResult;
			}
		}
		DropPositions dropPositions2 = startingPos;
		for (int j = 0; j < list.Count; j++)
		{
			int num = list[j];
			dropPositions2 = startingPos;
			if (bothHands && j != 0)
			{
				dropPositions2 = OppositePosition(dropPositions2);
			}
			if (!PositionAvailable(num, dropPositions2))
			{
				dropPositions2 = FirstAvailablePosition(num);
				if (dropPositions2 == DropPositions.None)
				{
					return dockingResult;
				}
			}
			if (OfflineItemActive(num) == dropPositions2)
			{
				TransferrableItemDisable(num);
				dockingResult.positionsDisabled.Add(dropPositions2);
				continue;
			}
			TransferrableItemDisableAtPosition(dropPositions2);
			dockingResult.dockedPosition.Add(dropPositions2);
			TransferrableObject.PositionState positionState = MapDropPositionToState(dropPositions2);
			if (TransferrableItemActive(num))
			{
				DropPositions item = TransferrableItemPosition(num);
				dockingResult.positionsDisabled.Add(item);
				MoveTransferableItem(num, dropPositions2, positionState);
			}
			else
			{
				EnableTransferrableItem(num, dropPositions2, positionState);
			}
		}
		return dockingResult;
	}

	private void MoveTransferableItem(int allItemsIndex, DropPositions newPosition, TransferrableObject.PositionState newPositionState)
	{
		allObjects[allItemsIndex].storedZone = newPosition;
		allObjects[allItemsIndex].currentState = newPositionState;
		allObjects[allItemsIndex].ResetToDefaultState();
	}

	public void EnableTransferrableGameObject(int allItemsIndex, DropPositions dropZone, TransferrableObject.PositionState startingPosition)
	{
		MoveTransferableItem(allItemsIndex, dropZone, startingPosition);
		allObjects[allItemsIndex].gameObject.SetActive(value: true);
	}

	public void RefreshTransferrableItems()
	{
		objectsToEnable.Clear();
		objectsToDisable.Clear();
		for (int i = 0; i < myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			bool flag = true;
			if (myRig.ActiveTransferrableObjectIndex(i) == -1 || !myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(allObjects[myRig.ActiveTransferrableObjectIndex(i)].gameObject.name)))
			{
				continue;
			}
			for (int j = 0; j < allObjects.Length; j++)
			{
				if (j == myRig.ActiveTransferrableObjectIndex(i) && allObjects[j].gameObject.activeSelf)
				{
					allObjects[j].objectIndex = i;
					flag = false;
				}
			}
			if (flag)
			{
				objectsToEnable.Add(i);
			}
		}
		for (int k = 0; k < allObjects.Length; k++)
		{
			if (!allObjects[k].gameObject.activeSelf)
			{
				continue;
			}
			bool flag2 = true;
			for (int l = 0; l < myRig.ActiveTransferrableObjectIndexLength(); l++)
			{
				if (myRig.ActiveTransferrableObjectIndex(l) == k && myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(allObjects[myRig.ActiveTransferrableObjectIndex(l)].gameObject.name)))
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				objectsToDisable.Add(k);
			}
		}
		foreach (int item in objectsToEnable)
		{
			EnableTransferrableGameObject(myRig.ActiveTransferrableObjectIndex(item), myRig.TransferrableDockPosition(item), myRig.TransferrablePosStates(item));
		}
		foreach (int item2 in objectsToDisable)
		{
			DisableTransferrableItem(item2);
		}
		UpdateHandState();
	}

	public int ReturnTransferrableItemIndex(int allItemsIndex)
	{
		for (int i = 0; i < myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
			{
				return i;
			}
		}
		return -1;
	}

	public List<int> TransferrableObjectIndexFromName(string transObjectName)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < allObjects.Length; i++)
		{
			if (allObjects[i].gameObject.name == transObjectName)
			{
				list.Add(i);
			}
		}
		return list;
	}

	private TransferrableObject.PositionState MapDropPositionToState(DropPositions pos)
	{
		return pos switch
		{
			DropPositions.RightArm => TransferrableObject.PositionState.OnRightArm, 
			DropPositions.LeftArm => TransferrableObject.PositionState.OnLeftArm, 
			DropPositions.LeftBack => TransferrableObject.PositionState.OnLeftShoulder, 
			DropPositions.RightBack => TransferrableObject.PositionState.OnRightShoulder, 
			_ => TransferrableObject.PositionState.OnChest, 
		};
	}

	private void UpdateHandState()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject[] array = ((i == 0) ? leftHandThrowables : rightHandThrowables);
			int num = ((i == 0) ? myRig.LeftThrowableProjectileIndex : myRig.RightThrowableProjectileIndex);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].SetActive(j == num);
			}
		}
	}
}
