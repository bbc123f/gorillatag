using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class GorillaCaveCrystalVisuals : MonoBehaviour
{
	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000920 RID: 2336 RVA: 0x000375D8 File Offset: 0x000357D8
	// (set) Token: 0x06000921 RID: 2337 RVA: 0x000375E0 File Offset: 0x000357E0
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

	// Token: 0x06000922 RID: 2338 RVA: 0x000375EC File Offset: 0x000357EC
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

	// Token: 0x06000923 RID: 2339 RVA: 0x00037668 File Offset: 0x00035868
	private void Start()
	{
		this.UpdateAlbedo();
		this.ForceUpdate();
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x00037678 File Offset: 0x00035878
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

	// Token: 0x06000925 RID: 2341 RVA: 0x000376E8 File Offset: 0x000358E8
	private void Awake()
	{
		this.UpdateAlbedo();
		this.Update();
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x000376F8 File Offset: 0x000358F8
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

	// Token: 0x06000927 RID: 2343 RVA: 0x000377E4 File Offset: 0x000359E4
	public void ForceUpdate()
	{
		this._lastState = 0;
		this.Update();
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x000377F4 File Offset: 0x000359F4
	private static void InitializeCrystals()
	{
		foreach (GorillaCaveCrystalVisuals gorillaCaveCrystalVisuals in Object.FindObjectsByType<GorillaCaveCrystalVisuals>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
		{
			gorillaCaveCrystalVisuals.UpdateAlbedo();
			gorillaCaveCrystalVisuals.ForceUpdate();
			gorillaCaveCrystalVisuals._lastState = -1;
		}
	}

	// Token: 0x04000B25 RID: 2853
	public CrystalVisualsPreset crysalPreset;

	// Token: 0x04000B26 RID: 2854
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerp;

	// Token: 0x04000B27 RID: 2855
	[Space]
	public MeshRenderer _renderer;

	// Token: 0x04000B28 RID: 2856
	public Material _sharedMaterial;

	// Token: 0x04000B29 RID: 2857
	[SerializeField]
	public Texture2D instanceAlbedo;

	// Token: 0x04000B2A RID: 2858
	[SerializeField]
	private bool _initialized;

	// Token: 0x04000B2B RID: 2859
	[SerializeField]
	private int _lastState;

	// Token: 0x04000B2C RID: 2860
	[SerializeField]
	public GorillaCaveCrystalSetup _setup;

	// Token: 0x04000B2D RID: 2861
	private MaterialPropertyBlock _block;

	// Token: 0x04000B2E RID: 2862
	[NonSerialized]
	private bool _ranSetupOnce;
}
