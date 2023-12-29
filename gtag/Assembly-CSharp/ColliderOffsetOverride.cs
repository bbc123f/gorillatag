using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColliderOffsetOverride : MonoBehaviour
{
	private void Awake()
	{
		if (this.autoSearch)
		{
			this.FindColliders();
		}
		foreach (Collider collider in this.colliders)
		{
			if (collider != null)
			{
				collider.contactOffset = 0.01f * this.targetScale;
			}
		}
	}

	public void FindColliders()
	{
		foreach (Collider item in base.gameObject.GetComponents<Collider>().ToList<Collider>())
		{
			if (!this.colliders.Contains(item))
			{
				this.colliders.Add(item);
			}
		}
	}

	public void FindCollidersRecursively()
	{
		foreach (Collider item in base.gameObject.GetComponentsInChildren<Collider>().ToList<Collider>())
		{
			if (!this.colliders.Contains(item))
			{
				this.colliders.Add(item);
			}
		}
	}

	private void AutoDisabled()
	{
		this.autoSearch = true;
	}

	private void AutoEnabled()
	{
		this.autoSearch = false;
	}

	public List<Collider> colliders;

	[HideInInspector]
	public bool autoSearch;

	public float targetScale = 1f;
}
