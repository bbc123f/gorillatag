using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D0 RID: 464
[HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/rendering/material-instance")]
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Scripts/MRTK/Core/MaterialInstance")]
public class MaterialInstance : MonoBehaviour
{
	// Token: 0x06000BDE RID: 3038 RVA: 0x00049843 File Offset: 0x00047A43
	public Material AcquireMaterial(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		Material[] array = this.instanceMaterials;
		if (array != null && array.Length != 0)
		{
			return this.instanceMaterials[0];
		}
		return null;
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x00049881 File Offset: 0x00047A81
	public Material[] AcquireMaterials(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		base.gameObject.GetComponent<Material>();
		return this.instanceMaterials;
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x000498B4 File Offset: 0x00047AB4
	public void ReleaseMaterial(Object owner, bool autoDestroy = true)
	{
		this.materialOwners.Remove(owner);
		if (autoDestroy && this.materialOwners.Count == 0)
		{
			MaterialInstance.DestroySafe(this);
			if (!base.gameObject.activeInHierarchy)
			{
				this.RestoreRenderer();
			}
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x000498EC File Offset: 0x00047AEC
	public Material Material
	{
		get
		{
			return this.AcquireMaterial(null, true);
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000BE2 RID: 3042 RVA: 0x000498F6 File Offset: 0x00047AF6
	public Material[] Materials
	{
		get
		{
			return this.AcquireMaterials(null, true);
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x00049900 File Offset: 0x00047B00
	// (set) Token: 0x06000BE4 RID: 3044 RVA: 0x00049908 File Offset: 0x00047B08
	public bool CacheSharedMaterialsFromRenderer
	{
		get
		{
			return this.cacheSharedMaterialsFromRenderer;
		}
		set
		{
			if (this.cacheSharedMaterialsFromRenderer != value)
			{
				if (value)
				{
					this.cachedSharedMaterials = this.CachedRenderer.sharedMaterials;
				}
				else
				{
					this.cachedSharedMaterials = null;
				}
				this.cacheSharedMaterialsFromRenderer = value;
			}
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x00049937 File Offset: 0x00047B37
	private Renderer CachedRenderer
	{
		get
		{
			if (this.cachedRenderer == null)
			{
				this.cachedRenderer = base.GetComponent<Renderer>();
				if (this.CacheSharedMaterialsFromRenderer)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
			}
			return this.cachedRenderer;
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00049972 File Offset: 0x00047B72
	// (set) Token: 0x06000BE7 RID: 3047 RVA: 0x000499A7 File Offset: 0x00047BA7
	private Material[] CachedRendererSharedMaterials
	{
		get
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				if (this.cachedSharedMaterials == null)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
				return this.cachedSharedMaterials;
			}
			return this.cachedRenderer.sharedMaterials;
		}
		set
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				this.cachedSharedMaterials = value;
			}
			this.cachedRenderer.sharedMaterials = value;
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x000499C4 File Offset: 0x00047BC4
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x000499CC File Offset: 0x00047BCC
	private void OnDestroy()
	{
		this.RestoreRenderer();
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x000499D4 File Offset: 0x00047BD4
	private void RestoreRenderer()
	{
		if (this.CachedRenderer != null && this.defaultMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.defaultMaterials;
		}
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = null;
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00049A0C File Offset: 0x00047C0C
	private void Initialize()
	{
		if (!this.initialized && this.CachedRenderer != null)
		{
			if (!MaterialInstance.HasValidMaterial(this.defaultMaterials))
			{
				this.defaultMaterials = this.CachedRendererSharedMaterials;
			}
			else if (!this.materialsInstanced)
			{
				this.CachedRendererSharedMaterials = this.defaultMaterials;
			}
			this.initialized = true;
		}
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00049A65 File Offset: 0x00047C65
	private void AcquireInstances()
	{
		if (this.CachedRenderer != null && !MaterialInstance.MaterialsMatch(this.CachedRendererSharedMaterials, this.instanceMaterials))
		{
			this.CreateInstances();
		}
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x00049A90 File Offset: 0x00047C90
	private void CreateInstances()
	{
		this.Initialize();
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = MaterialInstance.InstanceMaterials(this.defaultMaterials);
		if (this.CachedRenderer != null && this.instanceMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.instanceMaterials;
		}
		this.materialsInstanced = true;
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x00049AE8 File Offset: 0x00047CE8
	private static bool MaterialsMatch(Material[] a, Material[] b)
	{
		int? num = (a != null) ? new int?(a.Length) : null;
		int? num2 = (b != null) ? new int?(b.Length) : null;
		if (!(num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null)))
		{
			return false;
		}
		int num3 = 0;
		for (;;)
		{
			int num4 = num3;
			num2 = ((a != null) ? new int?(a.Length) : null);
			if (!(num4 < num2.GetValueOrDefault() & num2 != null))
			{
				return true;
			}
			if (a[num3] != b[num3])
			{
				break;
			}
			num3++;
		}
		return false;
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x00049B8C File Offset: 0x00047D8C
	private static Material[] InstanceMaterials(Material[] source)
	{
		if (source == null)
		{
			return null;
		}
		Material[] array = new Material[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] != null)
			{
				if (MaterialInstance.IsInstanceMaterial(source[i]))
				{
					Debug.LogWarning("A material (" + source[i].name + ") which is already instanced was instanced multiple times.");
				}
				array[i] = new Material(source[i]);
				Material material = array[i];
				material.name += " (Instance)";
			}
		}
		return array;
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x00049C0C File Offset: 0x00047E0C
	private static void DestroyMaterials(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				MaterialInstance.DestroySafe(materials[i]);
			}
		}
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x00049C32 File Offset: 0x00047E32
	private static bool IsInstanceMaterial(Material material)
	{
		return material != null && material.name.Contains(" (Instance)");
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x00049C50 File Offset: 0x00047E50
	private static bool HasValidMaterial(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x00049C7E File Offset: 0x00047E7E
	private static void DestroySafe(Object toDestroy)
	{
		if (toDestroy != null && Application.isPlaying)
		{
			Object.Destroy(toDestroy);
		}
	}

	// Token: 0x04000F65 RID: 3941
	private Renderer cachedRenderer;

	// Token: 0x04000F66 RID: 3942
	[SerializeField]
	[HideInInspector]
	private Material[] defaultMaterials;

	// Token: 0x04000F67 RID: 3943
	private Material[] instanceMaterials;

	// Token: 0x04000F68 RID: 3944
	private Material[] cachedSharedMaterials;

	// Token: 0x04000F69 RID: 3945
	private bool initialized;

	// Token: 0x04000F6A RID: 3946
	private bool materialsInstanced;

	// Token: 0x04000F6B RID: 3947
	[SerializeField]
	[Tooltip("Whether to use a cached copy of cachedRenderer.sharedMaterials or call sharedMaterials on the Renderer directly. Enabling the option will lead to better performance but you must turn it off before modifying sharedMaterials of the Renderer.")]
	private bool cacheSharedMaterialsFromRenderer;

	// Token: 0x04000F6C RID: 3948
	private readonly HashSet<Object> materialOwners = new HashSet<Object>();

	// Token: 0x04000F6D RID: 3949
	private const string instancePostfix = " (Instance)";
}
