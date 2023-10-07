using System;
using System.Diagnostics;
using System.Linq;
using GorillaExtensions;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class GorillaCaveCrystal : Tappable
{
	// Token: 0x06000915 RID: 2325 RVA: 0x00037310 File Offset: 0x00035510
	private void Awake()
	{
		if (this.visuals == null)
		{
			Debug.LogError("GorillaCaveCrystal: Disabling because visuals are null at: " + base.transform.GetPath(), this);
			base.enabled = false;
			return;
		}
		this.visuals.UpdateAlbedo();
		this.visuals.ForceUpdate();
		this.visuals.enabled = false;
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00037370 File Offset: 0x00035570
	[Conditional("UNITY_EDITOR")]
	private void SetUnderwaterVisuals()
	{
		GorillaCaveCrystalSetup instance = GorillaCaveCrystalSetup.Instance;
		base.TryGetComponent<GorillaCaveCrystalVisuals>(out this.visuals);
		if (this.visuals == null)
		{
			this.visuals = base.gameObject.AddComponent<GorillaCaveCrystalVisuals>();
		}
		this.visuals.crysalPreset = instance.DarkLightUnderWater.visualPreset;
		this.visuals.instanceAlbedo = instance.CrystalDarkAlbedo;
		this.visuals.Setup();
		this.visuals.UpdateAlbedo();
		this.visuals.ForceUpdate();
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x000373F8 File Offset: 0x000355F8
	[Conditional("UNITY_EDITOR")]
	public void AddVisuals()
	{
		base.TryGetComponent<MeshRenderer>(out this._crystalRenderer);
		if (this._crystalRenderer == null)
		{
			return;
		}
		Material crystalMat = this._crystalRenderer.sharedMaterial;
		if (crystalMat == null)
		{
			return;
		}
		GorillaCaveCrystalSetup instance = GorillaCaveCrystalSetup.Instance;
		GorillaCaveCrystalSetup.CrystalDef crystalDef = instance.GetCrystalDefs().FirstOrDefault((GorillaCaveCrystalSetup.CrystalDef cd) => cd.keyMaterial == crystalMat);
		if (crystalDef == null)
		{
			return;
		}
		bool flag = crystalDef.keyMaterial == instance.Dark.keyMaterial || crystalDef.keyMaterial == instance.DarkLight.keyMaterial || crystalDef.keyMaterial == instance.DarkLightUnderWater.keyMaterial;
		base.TryGetComponent<GorillaCaveCrystalVisuals>(out this.visuals);
		if (this.visuals == null)
		{
			this.visuals = base.gameObject.AddComponent<GorillaCaveCrystalVisuals>();
		}
		if (crystalDef.visualPreset != null)
		{
			this.visuals.crysalPreset = crystalDef.visualPreset;
			if (instance.CrystalDarkAlbedo != null && flag)
			{
				this.visuals.instanceAlbedo = instance.CrystalDarkAlbedo;
			}
			this.visuals.Setup();
			if (flag)
			{
				this.visuals.UpdateAlbedo();
			}
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x00037538 File Offset: 0x00035738
	public override void OnTapLocal(float tapStrength, float tapTime)
	{
		this._tapStrength = tapStrength;
		this.AnimateCrystal();
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00037548 File Offset: 0x00035748
	private void AnimateCrystal()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.visuals == null)
		{
			return;
		}
		if (this._animating)
		{
			return;
		}
		this._timeSinceLastTap = 0f;
		this._animating = true;
		this.visuals.enabled = true;
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00037598 File Offset: 0x00035798
	private void Update()
	{
		if (!this._animating)
		{
			return;
		}
		float num = this._tapAnimLength * this._lerpMidpoint;
		if (this._timeSinceLastTap > this._tapAnimLength)
		{
			this._animating = false;
			this.visuals.lerp = 0f;
			this.visuals.enabled = false;
		}
		float a = 0f;
		float b = 0f;
		if (this._timeSinceLastTap <= num)
		{
			a = 0f;
			b = 1f;
			AnimationCurve lerpInCurve = this._lerpInCurve;
		}
		if (this._timeSinceLastTap > num)
		{
			a = 1f;
			b = 0f;
			AnimationCurve lerpOutCurve = this._lerpOutCurve;
		}
		float t = this._timeSinceLastTap / this._tapAnimLength;
		float lerp = Mathf.Lerp(a, b, t) * 2f;
		this.visuals.lerp = lerp;
	}

	// Token: 0x04000B15 RID: 2837
	public bool overrideSoundAndMaterial;

	// Token: 0x04000B16 RID: 2838
	public CrystalOctave octave;

	// Token: 0x04000B17 RID: 2839
	public CrystalNote note;

	// Token: 0x04000B18 RID: 2840
	[SerializeField]
	private MeshRenderer _crystalRenderer;

	// Token: 0x04000B19 RID: 2841
	public GorillaCaveCrystalVisuals visuals;

	// Token: 0x04000B1A RID: 2842
	[SerializeField]
	private float _tapAnimLength = 0.5f;

	// Token: 0x04000B1B RID: 2843
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerpMidpoint = 0.1f;

	// Token: 0x04000B1C RID: 2844
	[SerializeField]
	private AnimationCurve _lerpInCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04000B1D RID: 2845
	[SerializeField]
	private AnimationCurve _lerpOutCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04000B1E RID: 2846
	[SerializeField]
	private bool _animating;

	// Token: 0x04000B1F RID: 2847
	[SerializeField]
	[Range(0f, 1f)]
	private float _tapStrength = 1f;

	// Token: 0x04000B20 RID: 2848
	[NonSerialized]
	private TimeSince _timeSinceLastTap;
}
