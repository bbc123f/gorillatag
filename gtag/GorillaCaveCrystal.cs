using System;
using System.Diagnostics;
using System.Linq;
using GorillaExtensions;
using GorillaTagScripts;
using UnityEngine;

public class GorillaCaveCrystal : Tappable
{
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

	public override void OnTapLocal(float tapStrength, float tapTime)
	{
		this._tapStrength = tapStrength;
		this.AnimateCrystal();
	}

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
		float num2 = 0f;
		float num3 = 0f;
		if (this._timeSinceLastTap <= num)
		{
			num2 = 0f;
			num3 = 1f;
			AnimationCurve lerpInCurve = this._lerpInCurve;
		}
		if (this._timeSinceLastTap > num)
		{
			num2 = 1f;
			num3 = 0f;
			AnimationCurve lerpOutCurve = this._lerpOutCurve;
		}
		float num4 = this._timeSinceLastTap / this._tapAnimLength;
		float num5 = Mathf.Lerp(num2, num3, num4) * 2f;
		this.visuals.lerp = num5;
	}

	public bool overrideSoundAndMaterial;

	public CrystalOctave octave;

	public CrystalNote note;

	[SerializeField]
	private MeshRenderer _crystalRenderer;

	public GorillaCaveCrystalVisuals visuals;

	[SerializeField]
	private float _tapAnimLength = 0.5f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _lerpMidpoint = 0.1f;

	[SerializeField]
	private AnimationCurve _lerpInCurve = AnimationCurve.Constant(0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve _lerpOutCurve = AnimationCurve.Constant(0f, 1f, 1f);

	[SerializeField]
	private bool _animating;

	[SerializeField]
	[Range(0f, 1f)]
	private float _tapStrength = 1f;

	[NonSerialized]
	private TimeSince _timeSinceLastTap;
}
