using System;
using GorillaTag;
using UnityEngine;

public class ZoneConditionalVisibility : MonoBehaviour
{
	private void Start()
	{
		this.zoneRoot.AddCallback(new Action<GameObject>(this.Callback), true);
	}

	private void OnDestroy()
	{
		this.zoneRoot.RemoveCallback(new Action<GameObject>(this.Callback));
	}

	private void Callback(GameObject rootObject)
	{
		if (this.invisibleWhileLoaded)
		{
			base.gameObject.SetActive(rootObject == null);
			return;
		}
		base.gameObject.SetActive(rootObject != null);
	}

	[SerializeField]
	private WatchableGameObjectSO zoneRoot;

	[SerializeField]
	private bool invisibleWhileLoaded;
}
