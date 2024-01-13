using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadModel : MonoBehaviour
{
	public GameObject[] cosmetics;

	private GameObject objRef;

	private List<GameObject> currentActiveObjects = new List<GameObject>();

	private Dictionary<string, GameObject> cosmeticDict = new Dictionary<string, GameObject>();

	private bool initialized;

	public void Awake()
	{
		StartCoroutine(DisableAfterASecond());
	}

	private IEnumerator DisableAfterASecond()
	{
		yield return new WaitForSeconds(0.1f);
		if (!initialized && base.isActiveAndEnabled)
		{
			initialized = true;
			GameObject[] array = cosmetics;
			foreach (GameObject gameObject in array)
			{
				cosmeticDict.Add(gameObject.name, gameObject);
				SetChildRendererWithOverride(gameObject, setEnabled: false, forRightSide: false);
			}
		}
	}

	public void OnEnable()
	{
		Awake();
	}

	public void SetCosmeticActive(string activeCosmeticName, bool forRightSide = false)
	{
		foreach (GameObject currentActiveObject in currentActiveObjects)
		{
			SetChildRendererWithOverride(currentActiveObject, setEnabled: false, forRightSide);
		}
		currentActiveObjects.Clear();
		if (cosmeticDict.TryGetValue(activeCosmeticName, out objRef))
		{
			currentActiveObjects.Add(objRef);
			SetChildRendererWithOverride(objRef, setEnabled: true, forRightSide);
		}
	}

	public void SetCosmeticActiveArray(string[] activeCosmeticNames, bool[] forRightSideArray)
	{
		foreach (GameObject currentActiveObject in currentActiveObjects)
		{
			SetChildRendererWithOverride(currentActiveObject, setEnabled: false, forRightSide: false);
		}
		currentActiveObjects.Clear();
		for (int i = 0; i < activeCosmeticNames.Length; i++)
		{
			if (cosmeticDict.TryGetValue(activeCosmeticNames[i], out objRef))
			{
				currentActiveObjects.Add(objRef);
				SetChildRendererWithOverride(objRef, setEnabled: true, forRightSideArray[i]);
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
			SetChildRenderers(gameObject, setEnabled: true);
			SetChildRenderers(obj, setEnabled: false);
		}
		else if (gameObject != null)
		{
			SetChildRenderers(gameObject, setEnabled: false);
			SetChildRenderers(obj, setEnabled);
		}
		else
		{
			SetChildRenderers(obj, setEnabled);
		}
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
}
