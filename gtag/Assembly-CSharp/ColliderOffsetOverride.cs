using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class ColliderOffsetOverride : MonoBehaviour
{
	// Token: 0x06000063 RID: 99 RVA: 0x0000492C File Offset: 0x00002B2C
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

	// Token: 0x06000064 RID: 100 RVA: 0x000049A4 File Offset: 0x00002BA4
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

	// Token: 0x06000065 RID: 101 RVA: 0x00004A14 File Offset: 0x00002C14
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

	// Token: 0x06000066 RID: 102 RVA: 0x00004A84 File Offset: 0x00002C84
	private void AutoDisabled()
	{
		this.autoSearch = true;
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00004A8D File Offset: 0x00002C8D
	private void AutoEnabled()
	{
		this.autoSearch = false;
	}

	// Token: 0x04000091 RID: 145
	public List<Collider> colliders;

	// Token: 0x04000092 RID: 146
	[HideInInspector]
	public bool autoSearch;

	// Token: 0x04000093 RID: 147
	public float targetScale = 1f;
}
