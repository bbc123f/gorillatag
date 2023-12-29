using System;
using System.Collections.Generic;
using UnityEngine;

public class HeadModel : MonoBehaviour
{
	public void Awake()
	{
		this.Initialize();
	}

	private void Initialize()
	{
		if (!this.initialized && base.enabled)
		{
			this.initialized = true;
			foreach (GameObject gameObject in this.cosmetics)
			{
				if (!(gameObject == null))
				{
					if (!this.cosmeticDict.TryAdd(gameObject.name, gameObject))
					{
						Debug.LogError("HeadModel.Initialize: Cosmetic id \"" + gameObject.name + "\" already exists in cosmeticDict. Did you accidentally have 2 head models with the same name?", gameObject);
					}
					else
					{
						this.SetChildRendererWithOverride(gameObject, false, false);
					}
				}
			}
		}
	}

	public void OnEnable()
	{
		this.Initialize();
	}

	public void SetCosmeticActive(string activeCosmeticName, bool forRightSide = false)
	{
		foreach (GameObject obj in this.currentActiveObjects)
		{
			this.SetChildRendererWithOverride(obj, false, forRightSide);
		}
		this.currentActiveObjects.Clear();
		if (this.cosmeticDict.TryGetValue(activeCosmeticName, out this.objRef))
		{
			this.currentActiveObjects.Add(this.objRef);
			this.SetChildRendererWithOverride(this.objRef, true, forRightSide);
		}
	}

	public void SetCosmeticActiveArray(string[] activeCosmeticNames, bool[] forRightSideArray)
	{
		foreach (GameObject obj in this.currentActiveObjects)
		{
			this.SetChildRendererWithOverride(obj, false, false);
		}
		this.currentActiveObjects.Clear();
		for (int i = 0; i < activeCosmeticNames.Length; i++)
		{
			if (this.cosmeticDict.TryGetValue(activeCosmeticNames[i], out this.objRef))
			{
				this.currentActiveObjects.Add(this.objRef);
				this.SetChildRendererWithOverride(this.objRef, true, forRightSideArray[i]);
			}
		}
	}

	private void SetChildRendererWithOverride(GameObject obj, bool setEnabled, bool forRightSide)
	{
		GameObject gameObject = null;
		OverridePaperDoll component = obj.GetComponent<OverridePaperDoll>();
		if (component != null)
		{
			gameObject = component.rightSideOverride;
		}
		if (setEnabled && forRightSide && gameObject != null)
		{
			this.SetChildRenderers(gameObject, true);
			this.SetChildRenderers(obj, false);
			return;
		}
		if (gameObject != null)
		{
			this.SetChildRenderers(gameObject, false);
			this.SetChildRenderers(obj, setEnabled);
			return;
		}
		this.SetChildRenderers(obj, setEnabled);
	}

	private void SetChildRenderers(GameObject obj, bool setEnabled)
	{
		MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = setEnabled;
		}
		SkinnedMeshRenderer[] componentsInChildren2 = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = setEnabled;
		}
	}

	public GameObject[] cosmetics;

	private GameObject objRef;

	private List<GameObject> currentActiveObjects = new List<GameObject>();

	private Dictionary<string, GameObject> cosmeticDict = new Dictionary<string, GameObject>();

	private bool initialized;
}
