using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class HeadModel : MonoBehaviour
{
	// Token: 0x06000650 RID: 1616 RVA: 0x00027A93 File Offset: 0x00025C93
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00027A9C File Offset: 0x00025C9C
	private void Initialize()
	{
		if (!this.initialized && base.enabled)
		{
			this.initialized = true;
			foreach (GameObject gameObject in this.cosmetics)
			{
				if (!(gameObject == null))
				{
					this.cosmeticDict.Add(gameObject.name, gameObject);
					this.SetChildRendererWithOverride(gameObject, false, false);
				}
			}
		}
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00027AFD File Offset: 0x00025CFD
	public void OnEnable()
	{
		this.Initialize();
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00027B08 File Offset: 0x00025D08
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

	// Token: 0x06000654 RID: 1620 RVA: 0x00027B9C File Offset: 0x00025D9C
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

	// Token: 0x06000655 RID: 1621 RVA: 0x00027C44 File Offset: 0x00025E44
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

	// Token: 0x06000656 RID: 1622 RVA: 0x00027CAC File Offset: 0x00025EAC
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

	// Token: 0x040007B0 RID: 1968
	public GameObject[] cosmetics;

	// Token: 0x040007B1 RID: 1969
	private GameObject objRef;

	// Token: 0x040007B2 RID: 1970
	private List<GameObject> currentActiveObjects = new List<GameObject>();

	// Token: 0x040007B3 RID: 1971
	private Dictionary<string, GameObject> cosmeticDict = new Dictionary<string, GameObject>();

	// Token: 0x040007B4 RID: 1972
	private bool initialized;
}
