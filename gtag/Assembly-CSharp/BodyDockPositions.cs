using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class BodyDockPositions : MonoBehaviourPunCallbacks
{
	// Token: 0x06000658 RID: 1624 RVA: 0x00027D14 File Offset: 0x00025F14
	public void Awake()
	{
		for (int i = 0; i < this.allObjects.Length; i++)
		{
			if (this.allObjects[i] == null)
			{
				Debug.LogError("BodyDockPositions.allObjects array has a null reference at index " + i.ToString() + ".", this);
			}
		}
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00027D60 File Offset: 0x00025F60
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (object.Equals(this.myRig.creator, otherPlayer))
		{
			this.DeallocateSharableInstances();
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00027D84 File Offset: 0x00025F84
	public WorldShareableItem AllocateSharableInstance(BodyDockPositions.DropPositions position, Player owner)
	{
		switch (position)
		{
		case BodyDockPositions.DropPositions.None:
		case BodyDockPositions.DropPositions.LeftArm:
		case BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.LeftBack:
			if (this.leftBackSharableItem == null)
			{
				this.leftBackSharableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance).GetComponent<WorldShareableItem>();
				this.leftBackSharableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
				this.leftBackSharableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 3);
			}
			return this.leftBackSharableItem;
		default:
			if (position == BodyDockPositions.DropPositions.RightBack)
			{
				if (this.rightBackShareableItem == null)
				{
					this.rightBackShareableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance).GetComponent<WorldShareableItem>();
					this.rightBackShareableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
					this.rightBackShareableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 4);
				}
				return this.rightBackShareableItem;
			}
			if (position != BodyDockPositions.DropPositions.All)
			{
			}
			break;
		}
		throw new ArgumentOutOfRangeException("position", position, null);
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00027E7C File Offset: 0x0002607C
	public void DeallocateSharableInstance(WorldShareableItem worldShareable)
	{
		if (worldShareable == null)
		{
			return;
		}
		if (worldShareable == this.leftBackSharableItem)
		{
			if (this.leftBackSharableItem == null)
			{
				return;
			}
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
			this.leftBackSharableItem = null;
		}
		if (worldShareable == this.rightBackShareableItem)
		{
			if (this.rightBackShareableItem == null)
			{
				return;
			}
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
			this.rightBackShareableItem = null;
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00027F10 File Offset: 0x00026110
	public void DeallocateSharableInstances()
	{
		if (this.rightBackShareableItem != null)
		{
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
		}
		if (this.leftBackSharableItem != null)
		{
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
		}
		this.leftBackSharableItem = null;
		this.rightBackShareableItem = null;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00027F7B File Offset: 0x0002617B
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.DeallocateSharableInstances();
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00027F89 File Offset: 0x00026189
	public static bool IsPositionLeft(BodyDockPositions.DropPositions pos)
	{
		return pos == BodyDockPositions.DropPositions.LeftArm || pos == BodyDockPositions.DropPositions.LeftBack;
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00027F98 File Offset: 0x00026198
	public int DropZoneStorageUsed(BodyDockPositions.DropPositions dropPosition)
	{
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return -1;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) >= 0 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].storedZone == dropPosition)
			{
				return this.myRig.ActiveTransferrableObjectIndex(i);
			}
		}
		return -1;
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00028054 File Offset: 0x00026254
	public TransferrableObject ItemPositionInUse(BodyDockPositions.DropPositions dropPosition)
	{
		TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPosition);
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return null;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) != -1 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].currentState == positionState)
			{
				return this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)];
			}
		}
		return null;
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0002811C File Offset: 0x0002631C
	private int EnableTransferrableItem(int allItemsIndex, BodyDockPositions.DropPositions startingPosition, TransferrableObject.PositionState startingState)
	{
		if (allItemsIndex < 0 || allItemsIndex >= this.allObjects.Length)
		{
			return -1;
		}
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == -1)
				{
					string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[allItemsIndex].gameObject.name);
					if (this.myRig.IsItemAllowed(itemNameFromDisplayName))
					{
						this.myRig.SetActiveTransferrableObjectIndex(i, allItemsIndex);
						this.myRig.SetTransferrablePosStates(i, startingState);
						this.myRig.SetTransferrableItemStates(i, (TransferrableObject.ItemStates)0);
						this.myRig.SetTransferrableDockPosition(i, startingPosition);
						this.EnableTransferrableGameObject(allItemsIndex, startingPosition, startingState);
						return i;
					}
				}
			}
		}
		return -1;
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x000281F0 File Offset: 0x000263F0
	public BodyDockPositions.DropPositions ItemActive(int allItemsIndex)
	{
		if (!this.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return this.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00028218 File Offset: 0x00026418
	public static BodyDockPositions.DropPositions OfflineItemActive(int allItemsIndex)
	{
		if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		BodyDockPositions component = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>();
		if (component == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		if (!component.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return component.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00028288 File Offset: 0x00026488
	public void DisableTransferrableItem(int index)
	{
		TransferrableObject transferrableObject = this.allObjects[index];
		if (transferrableObject.gameObject.activeSelf)
		{
			transferrableObject.gameObject.Disable();
			transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
		}
		if (this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == index)
				{
					this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				}
			}
		}
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x000282FC File Offset: 0x000264FC
	public void DisableAllTransferableItems()
	{
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num >= 0 && num < this.allObjects.Length)
			{
				TransferrableObject transferrableObject = this.allObjects[num];
				transferrableObject.gameObject.Disable();
				transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
				this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				this.myRig.SetTransferrableItemStates(i, (TransferrableObject.ItemStates)0);
				this.myRig.SetTransferrablePosStates(i, TransferrableObject.PositionState.None);
			}
		}
		this.DeallocateSharableInstances();
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00028381 File Offset: 0x00026581
	private bool AllItemsIndexValid(int allItemsIndex)
	{
		return allItemsIndex != -1 && allItemsIndex < this.allObjects.Length;
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00028394 File Offset: 0x00026594
	public bool PositionAvailable(int allItemIndex, BodyDockPositions.DropPositions startPos)
	{
		return (this.allObjects[allItemIndex].dockPositions & startPos) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x000283A8 File Offset: 0x000265A8
	public BodyDockPositions.DropPositions FirstAvailablePosition(int allItemIndex)
	{
		for (int i = 0; i < 5; i++)
		{
			BodyDockPositions.DropPositions dropPositions = (BodyDockPositions.DropPositions)(1 << i);
			if ((this.allObjects[allItemIndex].dockPositions & dropPositions) != BodyDockPositions.DropPositions.None)
			{
				return dropPositions;
			}
		}
		return BodyDockPositions.DropPositions.None;
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x000283DC File Offset: 0x000265DC
	public int TransferrableItemDisable(int allItemsIndex)
	{
		if (BodyDockPositions.OfflineItemActive(allItemsIndex) != BodyDockPositions.DropPositions.None)
		{
			this.DisableTransferrableItem(allItemsIndex);
		}
		return 0;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x000283F0 File Offset: 0x000265F0
	public void TransferrableItemDisableAtPosition(BodyDockPositions.DropPositions dropPositions)
	{
		int num = this.DropZoneStorageUsed(dropPositions);
		if (num >= 0)
		{
			this.TransferrableItemDisable(num);
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00028414 File Offset: 0x00026614
	public void TransferrableItemEnableAtPosition(string itemName, BodyDockPositions.DropPositions dropPosition)
	{
		if (this.DropZoneStorageUsed(dropPosition) >= 0)
		{
			return;
		}
		List<int> list = this.TransferrableObjectIndexFromName(itemName);
		if (list.Count == 0)
		{
			return;
		}
		TransferrableObject.PositionState startingState = this.MapDropPositionToState(dropPosition);
		if (list.Count == 1)
		{
			this.EnableTransferrableItem(list[0], dropPosition, startingState);
			return;
		}
		int allItemsIndex = BodyDockPositions.IsPositionLeft(dropPosition) ? list[0] : list[1];
		this.EnableTransferrableItem(allItemsIndex, dropPosition, startingState);
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00028484 File Offset: 0x00026684
	public bool TransferrableItemActive(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int allItemsIndex in list)
		{
			if (this.TransferrableItemActive(allItemsIndex))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x000284F0 File Offset: 0x000266F0
	public bool TransferrableItemActiveAtPos(string transferrableItemName, BodyDockPositions.DropPositions dropPosition)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int allItemsIndex in list)
		{
			BodyDockPositions.DropPositions dropPositions = this.TransferrableItemPosition(allItemsIndex);
			if (dropPositions != BodyDockPositions.DropPositions.None && dropPositions == dropPosition)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00028564 File Offset: 0x00026764
	public bool TransferrableItemActive(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00028570 File Offset: 0x00026770
	public TransferrableObject TransferrableItem(int allItemsIndex)
	{
		return this.allObjects[allItemsIndex];
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0002857A File Offset: 0x0002677A
	public BodyDockPositions.DropPositions TransferrableItemPosition(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00028584 File Offset: 0x00026784
	public bool DisableTransferrableItem(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int index in list)
		{
			this.DisableTransferrableItem(index);
		}
		return true;
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x000285E8 File Offset: 0x000267E8
	public BodyDockPositions.DropPositions OppositePosition(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return BodyDockPositions.DropPositions.RightArm;
		}
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return BodyDockPositions.DropPositions.LeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return BodyDockPositions.DropPositions.RightBack;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return BodyDockPositions.DropPositions.LeftBack;
		}
		return pos;
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00028608 File Offset: 0x00026808
	public BodyDockPositions.DockingResult ToggleWithHandedness(string transferrableItemName, bool isLeftHand, bool bothHands)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return new BodyDockPositions.DockingResult();
		}
		if (!this.AllItemsIndexValid(list[0]))
		{
			return new BodyDockPositions.DockingResult();
		}
		BodyDockPositions.DropPositions startingPos;
		if (isLeftHand)
		{
			startingPos = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.RightArm : BodyDockPositions.DropPositions.LeftBack);
		}
		else
		{
			startingPos = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.LeftArm : BodyDockPositions.DropPositions.RightBack);
		}
		return this.ToggleTransferrableItem(transferrableItemName, startingPos, bothHands);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00028688 File Offset: 0x00026888
	public BodyDockPositions.DockingResult ToggleTransferrableItem(string transferrableItemName, BodyDockPositions.DropPositions startingPos, bool bothHands)
	{
		BodyDockPositions.DockingResult dockingResult = new BodyDockPositions.DockingResult();
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return dockingResult;
		}
		if (bothHands && list.Count == 2)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int allItemsIndex = list[i];
				BodyDockPositions.DropPositions dropPositions = BodyDockPositions.OfflineItemActive(allItemsIndex);
				if (dropPositions != BodyDockPositions.DropPositions.None)
				{
					this.TransferrableItemDisable(allItemsIndex);
					dockingResult.positionsDisabled.Add(dropPositions);
				}
			}
			if (dockingResult.positionsDisabled.Count >= 1)
			{
				return dockingResult;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			int num = list[j];
			BodyDockPositions.DropPositions dropPositions2 = startingPos;
			if (bothHands && j != 0)
			{
				dropPositions2 = this.OppositePosition(dropPositions2);
			}
			if (!this.PositionAvailable(num, dropPositions2))
			{
				dropPositions2 = this.FirstAvailablePosition(num);
				if (dropPositions2 == BodyDockPositions.DropPositions.None)
				{
					return dockingResult;
				}
			}
			if (BodyDockPositions.OfflineItemActive(num) == dropPositions2)
			{
				this.TransferrableItemDisable(num);
				dockingResult.positionsDisabled.Add(dropPositions2);
			}
			else
			{
				this.TransferrableItemDisableAtPosition(dropPositions2);
				dockingResult.dockedPosition.Add(dropPositions2);
				TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPositions2);
				if (this.TransferrableItemActive(num))
				{
					BodyDockPositions.DropPositions item = this.TransferrableItemPosition(num);
					dockingResult.positionsDisabled.Add(item);
					this.MoveTransferableItem(num, dropPositions2, positionState);
				}
				else
				{
					this.EnableTransferrableItem(num, dropPositions2, positionState);
				}
			}
		}
		return dockingResult;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x000287CF File Offset: 0x000269CF
	private void MoveTransferableItem(int allItemsIndex, BodyDockPositions.DropPositions newPosition, TransferrableObject.PositionState newPositionState)
	{
		this.allObjects[allItemsIndex].storedZone = newPosition;
		this.allObjects[allItemsIndex].currentState = newPositionState;
		this.allObjects[allItemsIndex].ResetToDefaultState();
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x000287FA File Offset: 0x000269FA
	public void EnableTransferrableGameObject(int allItemsIndex, BodyDockPositions.DropPositions dropZone, TransferrableObject.PositionState startingPosition)
	{
		this.MoveTransferableItem(allItemsIndex, dropZone, startingPosition);
		this.allObjects[allItemsIndex].gameObject.SetActive(true);
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00028818 File Offset: 0x00026A18
	public void RefreshTransferrableItems()
	{
		this.objectsToEnable.Clear();
		this.objectsToDisable.Clear();
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			bool flag = true;
			if (this.myRig.ActiveTransferrableObjectIndex(i) != -1 && this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.name)))
			{
				for (int j = 0; j < this.allObjects.Length; j++)
				{
					if (j == this.myRig.ActiveTransferrableObjectIndex(i) && this.allObjects[j].gameObject.activeSelf)
					{
						this.allObjects[j].objectIndex = i;
						flag = false;
					}
				}
				if (flag)
				{
					this.objectsToEnable.Add(i);
				}
			}
		}
		for (int k = 0; k < this.allObjects.Length; k++)
		{
			if (this.allObjects[k].gameObject.activeSelf)
			{
				bool flag2 = true;
				for (int l = 0; l < this.myRig.ActiveTransferrableObjectIndexLength(); l++)
				{
					if (this.myRig.ActiveTransferrableObjectIndex(l) == k && this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[this.myRig.ActiveTransferrableObjectIndex(l)].gameObject.name)))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					this.objectsToDisable.Add(k);
				}
			}
		}
		foreach (int idx in this.objectsToEnable)
		{
			this.EnableTransferrableGameObject(this.myRig.ActiveTransferrableObjectIndex(idx), this.myRig.TransferrableDockPosition(idx), this.myRig.TransferrablePosStates(idx));
		}
		foreach (int index in this.objectsToDisable)
		{
			this.DisableTransferrableItem(index);
		}
		this.UpdateHandState();
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00028A58 File Offset: 0x00026C58
	public int ReturnTransferrableItemIndex(int allItemsIndex)
	{
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00028A90 File Offset: 0x00026C90
	public List<int> TransferrableObjectIndexFromName(string transObjectName)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.allObjects.Length; i++)
		{
			if (!(this.allObjects[i] == null) && this.allObjects[i].gameObject.name == transObjectName)
			{
				list.Add(i);
			}
		}
		return list;
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00028AE8 File Offset: 0x00026CE8
	private TransferrableObject.PositionState MapDropPositionToState(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return TransferrableObject.PositionState.OnRightArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return TransferrableObject.PositionState.OnLeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return TransferrableObject.PositionState.OnLeftShoulder;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return TransferrableObject.PositionState.OnRightShoulder;
		}
		return TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00028B08 File Offset: 0x00026D08
	private void UpdateHandState()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject[] array = (i == 0) ? this.leftHandThrowables : this.rightHandThrowables;
			int num = (i == 0) ? this.myRig.LeftThrowableProjectileIndex : this.myRig.RightThrowableProjectileIndex;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].SetActive(j == num);
			}
		}
	}

	// Token: 0x040007B5 RID: 1973
	public VRRig myRig;

	// Token: 0x040007B6 RID: 1974
	public GameObject[] leftHandThrowables;

	// Token: 0x040007B7 RID: 1975
	public GameObject[] rightHandThrowables;

	// Token: 0x040007B8 RID: 1976
	public TransferrableObject[] allObjects;

	// Token: 0x040007B9 RID: 1977
	private List<int> objectsToEnable = new List<int>();

	// Token: 0x040007BA RID: 1978
	private List<int> objectsToDisable = new List<int>();

	// Token: 0x040007BB RID: 1979
	public Transform leftHandTransform;

	// Token: 0x040007BC RID: 1980
	public Transform rightHandTransform;

	// Token: 0x040007BD RID: 1981
	public Transform chestTransform;

	// Token: 0x040007BE RID: 1982
	public Transform leftArmTransform;

	// Token: 0x040007BF RID: 1983
	public Transform rightArmTransform;

	// Token: 0x040007C0 RID: 1984
	public Transform leftBackTransform;

	// Token: 0x040007C1 RID: 1985
	public Transform rightBackTransform;

	// Token: 0x040007C2 RID: 1986
	public WorldShareableItem leftBackSharableItem;

	// Token: 0x040007C3 RID: 1987
	public WorldShareableItem rightBackShareableItem;

	// Token: 0x040007C4 RID: 1988
	public GameObject SharableItemInstance;

	// Token: 0x020003F9 RID: 1017
	[Flags]
	public enum DropPositions
	{
		// Token: 0x04001C89 RID: 7305
		LeftArm = 1,
		// Token: 0x04001C8A RID: 7306
		RightArm = 2,
		// Token: 0x04001C8B RID: 7307
		Chest = 4,
		// Token: 0x04001C8C RID: 7308
		LeftBack = 8,
		// Token: 0x04001C8D RID: 7309
		RightBack = 16,
		// Token: 0x04001C8E RID: 7310
		MaxDropPostions = 5,
		// Token: 0x04001C8F RID: 7311
		All = 31,
		// Token: 0x04001C90 RID: 7312
		None = 0
	}

	// Token: 0x020003FA RID: 1018
	public class DockingResult
	{
		// Token: 0x06001BF7 RID: 7159 RVA: 0x00096914 File Offset: 0x00094B14
		public DockingResult()
		{
			this.dockedPosition = new List<BodyDockPositions.DropPositions>(2);
			this.positionsDisabled = new List<BodyDockPositions.DropPositions>(2);
		}

		// Token: 0x04001C91 RID: 7313
		public List<BodyDockPositions.DropPositions> positionsDisabled;

		// Token: 0x04001C92 RID: 7314
		public List<BodyDockPositions.DropPositions> dockedPosition;
	}
}
