using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class GorillaCaveCrystalVisuals : MonoBehaviour
{
	// Token: 0x17000069 RID: 105
	// (get) Token: 0x0600091C RID: 2332 RVA: 0x000376D8 File Offset: 0x000358D8
	// (set) Token: 0x0600091D RID: 2333 RVA: 0x000376E0 File Offset: 0x000358E0
	public float lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			this._lerp = value;
		}
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x000376EC File Offset: 0x000358EC
	public void Setup()
	{
		base.TryGetComponent<MeshRenderer>(out this._renderer);
		if (this._renderer == null)
		{
			return;
		}
		this._setup = GorillaCaveCrystalSetup.Instance;
		this._sharedMaterial = this._renderer.sharedMaterial;
		this._initialized = (this.crysalPreset != null && this._renderer != null && this._sharedMaterial != null);
		this.Update();
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00037768 File Offset: 0x00035968
	private void Start()
	{
		this.UpdateAlbedo();
		this.ForceUpdate();
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00037778 File Offset: 0x00035978
	public void UpdateAlbedo()
	{
		if (!this._initialized)
		{
			return;
		}
		if (this.instanceAlbedo == null)
		{
			return;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetTexture(ShaderIds._BaseMap, this.instanceAlbedo);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x000377E8 File Offset: 0x000359E8
	private void Awake()
	{
		this.UpdateAlbedo();
		this.Update();
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x000377F8 File Offset: 0x000359F8
	private void Update()
	{
		if (!this._initialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			int hashCode = new ValueTuple<CrystalVisualsPreset, float>(this.crysalPreset, this._lerp).GetHashCode();
			if (this._lastState == hashCode)
			{
				return;
			}
			this._lastState = hashCode;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		CrystalVisualsPreset.VisualState stateA = this.crysalPreset.stateA;
		CrystalVisualsPreset.VisualState stateB = this.crysalPreset.stateB;
		Color value = Color.Lerp(stateA.albedo, stateB.albedo, this._lerp);
		Color value2 = Color.Lerp(stateA.emission, stateB.emission, this._lerp);
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetColor(ShaderIds._Color, value);
		this._block.SetColor(ShaderIds._EmissionColor, value2);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x000378E4 File Offset: 0x00035AE4
	public void ForceUpdate()
	{
		this._lastState = 0;
		this.Update();
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x000378F4 File Offset: 0x00035AF4
	private static void InitializeCrystals()
	{
		foreach (GorillaCaveCrystalVisuals gorillaCaveCrystalVisuals in Object.FindObjectsByType<GorillaCaveCrystalVisuals>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
		{
			gorillaCaveCrystalVisuals.UpdateAlbedo();
			gorillaCaveCrystalVisuals.ForceUpdate();
			gorillaCaveCrystalVisuals._lastState = -1;
		}
	}

	// Token: 0x04000B21 RID: 2849
	public CrystalVisualsPreset crysalPreset;

	// Token: 0x04000B22 RID: 2850
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerp;

	// Token: 0x04000B23 RID: 2851
	[Space]
	public MeshRenderer _renderer;

	// Token: 0x04000B24 RID: 2852
	public Material _sharedMaterial;

	// Token: 0x04000B25 RID: 2853
	[SerializeField]
	public Texture2D instanceAlbedo;

	// Token: 0x04000B26 RID: 2854
	[SerializeField]
	private bool _initialized;

	// Token: 0x04000B27 RID: 2855
	[SerializeField]
	private int _lastState;

	// Token: 0x04000B28 RID: 2856
	[SerializeField]
	public GorillaCaveCrystalSetup _setup;

	// Token: 0x04000B29 RID: 2857
	private MaterialPropertyBlock _block;

	// Token: 0x04000B2A RID: 2858
	[NonSerialized]
	private bool _ranSetupOnce;
}
