using System;
using System.Collections.Generic;
using GorillaNetworking;
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
			this.headMesh = base.GetComponent<Renderer>();
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
		this.headMesh.enabled = true;
		this.currentActiveObjects.Clear();
		this.ClearDynamicallyLoadedActiveObjects();
		CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(activeCosmeticName);
		if (!itemFromDict.isNullItem && itemFromDict.bLoadsFromResources)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load(itemFromDict.meshResourceString), base.transform) as GameObject;
			gameObject.transform.localRotation = Quaternion.Euler(itemFromDict.rotationOffset);
			gameObject.transform.localPosition = itemFromDict.positionOffset;
			if (itemFromDict.materialResourceString != string.Empty)
			{
				Resources.Load<Material>(itemFromDict.materialResourceString);
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].sharedMaterial = Resources.Load<Material>(itemFromDict.materialResourceString);
				}
			}
			this.currentDynamicallyLoadedActiveObjects.Add(gameObject);
			return;
		}
		if (this.cosmeticDict.TryGetValue(activeCosmeticName, out this.objRef))
		{
			this.currentActiveObjects.Add(this.objRef);
			this.SetChildRendererWithOverride(this.objRef, true, forRightSide);
			return;
		}
		if (activeCosmeticName != "NOTHING")
		{
			Debug.LogError("HeadModel.SetCosmeticActive: Cosmetic id \"" + activeCosmeticName + "\" not found in cosmeticDict");
		}
	}

	public void SetCosmeticActiveArray(string[] activeCosmeticNames, bool[] forRightSideArray)
	{
		foreach (GameObject obj in this.currentActiveObjects)
		{
			this.SetChildRendererWithOverride(obj, false, false);
		}
		this.headMesh.enabled = true;
		this.currentActiveObjects.Clear();
		this.ClearDynamicallyLoadedActiveObjects();
		for (int i = 0; i < activeCosmeticNames.Length; i++)
		{
			CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(activeCosmeticNames[i]);
			if (!itemFromDict.isNullItem && itemFromDict.bLoadsFromResources)
			{
				GameObject gameObject = Object.Instantiate(Resources.Load(itemFromDict.meshResourceString), base.transform) as GameObject;
				gameObject.transform.localRotation = Quaternion.Euler(itemFromDict.rotationOffset);
				gameObject.transform.localPosition = itemFromDict.positionOffset;
				if (itemFromDict.materialResourceString != string.Empty)
				{
					Resources.Load<Material>(itemFromDict.materialResourceString);
					Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].sharedMaterial = Resources.Load<Material>(itemFromDict.materialResourceString);
					}
				}
				this.currentDynamicallyLoadedActiveObjects.Add(gameObject);
			}
			else if (this.cosmeticDict.TryGetValue(activeCosmeticNames[i], out this.objRef))
			{
				this.currentActiveObjects.Add(this.objRef);
				this.SetChildRendererWithOverride(this.objRef, true, forRightSideArray[i]);
			}
			else if (activeCosmeticNames[i] != "NOTHING" && activeCosmeticNames[i] != "Slingshot")
			{
				Debug.LogError("HeadModel.SetCosmeticActive: Cosmetic id \"" + activeCosmeticNames[i] + "\" not found in cosmeticDict");
			}
		}
	}

	public void ClearDynamicallyLoadedActiveObjects()
	{
		foreach (GameObject obj in this.currentDynamicallyLoadedActiveObjects)
		{
			Object.Destroy(obj);
		}
		this.currentDynamicallyLoadedActiveObjects.Clear();
	}

	private void SetChildRendererWithOverride(GameObject obj, bool setEnabled, bool forRightSide)
	{
		GameObject gameObject = null;
		OverridePaperDoll component = obj.GetComponent<OverridePaperDoll>();
		if (component != null)
		{
			gameObject = component.rightSideOverride;
			if (setEnabled && component.replacesHeadMesh)
			{
				this.headMesh.enabled = false;
			}
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
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = setEnabled;
		}
	}

	public HeadModel()
	{
	}

	public GameObject[] cosmetics;

	private GameObject objRef;

	private List<GameObject> currentActiveObjects = new List<GameObject>();

	private Dictionary<string, GameObject> cosmeticDict = new Dictionary<string, GameObject>();

	private List<GameObject> currentDynamicallyLoadedActiveObjects = new List<GameObject>();

	private bool initialized;

	private Renderer headMesh;
}
