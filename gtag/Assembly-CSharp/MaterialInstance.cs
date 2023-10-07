using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001CF RID: 463
[HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/rendering/material-instance")]
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Scripts/MRTK/Core/MaterialInstance")]
public class MaterialInstance : MonoBehaviour
{
	// Token: 0x06000BD8 RID: 3032 RVA: 0x000495DB File Offset: 0x000477DB
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

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00049619 File Offset: 0x00047819
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

	// Token: 0x06000BDA RID: 3034 RVA: 0x0004964C File Offset: 0x0004784C
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

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000BDB RID: 3035 RVA: 0x00049684 File Offset: 0x00047884
	public Material Material
	{
		get
		{
			return this.AcquireMaterial(null, true);
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000BDC RID: 3036 RVA: 0x0004968E File Offset: 0x0004788E
	public Material[] Materials
	{
		get
		{
			return this.AcquireMaterials(null, true);
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000BDD RID: 3037 RVA: 0x00049698 File Offset: 0x00047898
	// (set) Token: 0x06000BDE RID: 3038 RVA: 0x000496A0 File Offset: 0x000478A0
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

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000BDF RID: 3039 RVA: 0x000496CF File Offset: 0x000478CF
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

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000BE0 RID: 3040 RVA: 0x0004970A File Offset: 0x0004790A
	// (set) Token: 0x06000BE1 RID: 3041 RVA: 0x0004973F File Offset: 0x0004793F
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

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0004975C File Offset: 0x0004795C
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x00049764 File Offset: 0x00047964
	private void OnDestroy()
	{
		this.RestoreRenderer();
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x0004976C File Offset: 0x0004796C
	private void RestoreRenderer()
	{
		if (this.CachedRenderer != null && this.defaultMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.defaultMaterials;
		}
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = null;
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x000497A4 File Offset: 0x000479A4
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

	// Token: 0x06000BE6 RID: 3046 RVA: 0x000497FD File Offset: 0x000479FD
	private void AcquireInstances()
	{
		if (this.CachedRenderer != null && !MaterialInstance.MaterialsMatch(this.CachedRendererSharedMaterials, this.instanceMaterials))
		{
			this.CreateInstances();
		}
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x00049828 File Offset: 0x00047A28
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

	// Token: 0x06000BE8 RID: 3048 RVA: 0x00049880 File Offset: 0x00047A80
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

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00049924 File Offset: 0x00047B24
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

	// Token: 0x06000BEA RID: 3050 RVA: 0x000499A4 File Offset: 0x00047BA4
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

	// Token: 0x06000BEB RID: 3051 RVA: 0x000499CA File Offset: 0x00047BCA
	private static bool IsInstanceMaterial(Material material)
	{
		return material != null && material.name.Contains(" (Instance)");
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x000499E8 File Offset: 0x00047BE8
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

	// Token: 0x06000BED RID: 3053 RVA: 0x00049A16 File Offset: 0x00047C16
	private static void DestroySafe(Object toDestroy)
	{
		if (toDestroy != null && Application.isPlaying)
		{
			Object.Destroy(toDestroy);
		}
	}

	// Token: 0x04000F61 RID: 3937
	private Renderer cachedRenderer;

	// Token: 0x04000F62 RID: 3938
	[SerializeField]
	[HideInInspector]
	private Material[] defaultMaterials;

	// Token: 0x04000F63 RID: 3939
	private Material[] instanceMaterials;

	// Token: 0x04000F64 RID: 3940
	private Material[] cachedSharedMaterials;

	// Token: 0x04000F65 RID: 3941
	private bool initialized;

	// Token: 0x04000F66 RID: 3942
	private bool materialsInstanced;

	// Token: 0x04000F67 RID: 3943
	[SerializeField]
	[Tooltip("Whether to use a cached copy of cachedRenderer.sharedMaterials or call sharedMaterials on the Renderer directly. Enabling the option will lead to better performance but you must turn it off before modifying sharedMaterials of the Renderer.")]
	private bool cacheSharedMaterialsFromRenderer;

	// Token: 0x04000F68 RID: 3944
	private readonly HashSet<Object> materialOwners = new HashSet<Object>();

	// Token: 0x04000F69 RID: 3945
	private const string instancePostfix = " (Instance)";
}
