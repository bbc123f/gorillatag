using System;
using System.Diagnostics;
using System.Linq;
using GorillaExtensions;
using GorillaTagScripts;
using GT;
using UnityEngine;

public class GorillaCaveCrystal : Tappable
{
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

	private void Awake()
	{
		if (visuals == null)
		{
			UnityEngine.Debug.LogError("GorillaCaveCrystal: Disabling because visuals are null at: " + base.transform.GetPath(), this);
			base.enabled = false;
		}
		else
		{
			visuals.UpdateAlbedo();
			visuals.enabled = false;
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void SetUnderwaterVisuals()
	{
		AddVisuals();
		GorillaCaveCrystalSetup instance = GorillaCaveCrystalSetup.Instance;
		TryGetComponent<GorillaCaveCrystalVisuals>(out visuals);
		if (visuals == null)
		{
			visuals = base.gameObject.AddComponent<GorillaCaveCrystalVisuals>();
		}
		visuals.crysalPreset = instance.DarkLightUnderWater.visualPreset;
		visuals.instanceAlbedo = instance.CrystalDarkAlbedo;
		visuals.Setup();
		visuals.UpdateAlbedo();
		visuals.ForceUpdate();
	}

	public void AddVisuals()
	{
		TryGetComponent<MeshRenderer>(out _crystalRenderer);
		if (_crystalRenderer == null)
		{
			return;
		}
		Material crystalMat = _crystalRenderer.sharedMaterial;
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
		TryGetComponent<GorillaCaveCrystalVisuals>(out visuals);
		if (visuals == null)
		{
			visuals = base.gameObject.AddComponent<GorillaCaveCrystalVisuals>();
		}
		if (crystalDef.visualPreset != null)
		{
			visuals.crysalPreset = crystalDef.visualPreset;
			if (instance.CrystalDarkAlbedo != null && flag)
			{
				visuals.instanceAlbedo = instance.CrystalDarkAlbedo;
			}
			visuals.Setup();
			if (flag)
			{
				visuals.UpdateAlbedo();
			}
		}
	}

	public override void OnTapLocal(float tapStrength, float tapTime)
	{
		_tapStrength = tapStrength;
		AnimateCrystal();
	}

	private void AnimateCrystal()
	{
		if (Application.isPlaying && !(visuals == null) && !_animating)
		{
			_timeSinceLastTap = 0f;
			_animating = true;
			visuals.enabled = true;
		}
	}

	private void Update()
	{
		if (_animating)
		{
			float num = _tapAnimLength * _lerpMidpoint;
			if ((float)_timeSinceLastTap > _tapAnimLength)
			{
				_animating = false;
				visuals.lerp = 0f;
				visuals.enabled = false;
			}
			float a = 0f;
			float b = 0f;
			if ((float)_timeSinceLastTap <= num)
			{
				a = 0f;
				b = 1f;
				_ = _lerpInCurve;
			}
			if ((float)_timeSinceLastTap > num)
			{
				a = 1f;
				b = 0f;
				_ = _lerpOutCurve;
			}
			float t = (float)_timeSinceLastTap / _tapAnimLength;
			float lerp = Mathf.Lerp(a, b, t) * 2f;
			visuals.lerp = lerp;
		}
	}
}
