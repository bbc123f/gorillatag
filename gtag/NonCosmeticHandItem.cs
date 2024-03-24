﻿using System;
using GorillaNetworking;
using UnityEngine;

public class NonCosmeticHandItem : MonoBehaviour
{
	public void EnableItem(bool enable)
	{
		if (this.itemPrefab)
		{
			this.itemPrefab.gameObject.SetActive(enable);
		}
	}

	public NonCosmeticHandItem()
	{
	}

	public CosmeticsController.CosmeticSlots cosmeticSlots;

	public GameObject itemPrefab;
}
