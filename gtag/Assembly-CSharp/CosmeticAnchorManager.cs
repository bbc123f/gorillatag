using System;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticAnchorManager : MonoBehaviour
{
	protected void Awake()
	{
		if (CosmeticAnchorManager.hasInstance && CosmeticAnchorManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		CosmeticAnchorManager.SetInstance(this);
	}

	public static void CreateManager()
	{
		CosmeticAnchorManager.SetInstance(new GameObject("CosmeticAnchorManager").AddComponent<CosmeticAnchorManager>());
	}

	private static void SetInstance(CosmeticAnchorManager manager)
	{
		CosmeticAnchorManager.instance = manager;
		CosmeticAnchorManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	public static void RegisterCosmeticAnchor(CosmeticAnchors cA)
	{
		if (!CosmeticAnchorManager.hasInstance)
		{
			CosmeticAnchorManager.CreateManager();
		}
		if (cA.AffectedByHunt() && !CosmeticAnchorManager.allAnchors.Contains(cA))
		{
			CosmeticAnchorManager.allAnchors.Add(cA);
		}
	}

	public static void UnregisterCosmeticAnchor(CosmeticAnchors cA)
	{
		if (!CosmeticAnchorManager.hasInstance)
		{
			CosmeticAnchorManager.CreateManager();
		}
		if (cA.AffectedByHunt() && CosmeticAnchorManager.allAnchors.Contains(cA))
		{
			CosmeticAnchorManager.allAnchors.Remove(cA);
		}
	}

	public void Update()
	{
		for (int i = 0; i < CosmeticAnchorManager.allAnchors.Count; i++)
		{
			CosmeticAnchorManager.allAnchors[i].TryUpdate();
		}
	}

	public static CosmeticAnchorManager instance;

	public static bool hasInstance = false;

	public static List<CosmeticAnchors> allAnchors = new List<CosmeticAnchors>();
}
