﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaTagScripts;
using Photon.Pun;
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

	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfo info)
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

	public void InvokeUpdate()
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

	protected override void OnEnable()
	{
		base.OnEnable();
		GorillaCaveCrystal.GorillaCaveCrystalManager.RegisterGorillaCaveCrystal(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		GorillaCaveCrystal.GorillaCaveCrystalManager.UnregisterGorillaCaveCrystal(this);
	}

	public GorillaCaveCrystal()
	{
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

	public class GorillaCaveCrystalManager : MonoBehaviour
	{
		protected void Awake()
		{
			if (GorillaCaveCrystal.GorillaCaveCrystalManager.hasInstance && GorillaCaveCrystal.GorillaCaveCrystalManager.instance != null && GorillaCaveCrystal.GorillaCaveCrystalManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaCaveCrystal.GorillaCaveCrystalManager.SetInstance(this);
		}

		public static void CreateManager()
		{
			GorillaCaveCrystal.GorillaCaveCrystalManager.SetInstance(new GameObject("GorillaCaveCrystalManager").AddComponent<GorillaCaveCrystal.GorillaCaveCrystalManager>());
		}

		private static void SetInstance(GorillaCaveCrystal.GorillaCaveCrystalManager manager)
		{
			GorillaCaveCrystal.GorillaCaveCrystalManager.instance = manager;
			GorillaCaveCrystal.GorillaCaveCrystalManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		public static void RegisterGorillaCaveCrystal(GorillaCaveCrystal gCC)
		{
			if (!GorillaCaveCrystal.GorillaCaveCrystalManager.hasInstance)
			{
				GorillaCaveCrystal.GorillaCaveCrystalManager.CreateManager();
			}
			if (!GorillaCaveCrystal.GorillaCaveCrystalManager.allCrystals.Contains(gCC))
			{
				GorillaCaveCrystal.GorillaCaveCrystalManager.allCrystals.Add(gCC);
			}
		}

		public static void UnregisterGorillaCaveCrystal(GorillaCaveCrystal gCC)
		{
			if (!GorillaCaveCrystal.GorillaCaveCrystalManager.hasInstance)
			{
				GorillaCaveCrystal.GorillaCaveCrystalManager.CreateManager();
			}
			if (GorillaCaveCrystal.GorillaCaveCrystalManager.allCrystals.Contains(gCC))
			{
				GorillaCaveCrystal.GorillaCaveCrystalManager.allCrystals.Remove(gCC);
			}
		}

		public void Update()
		{
			for (int i = 0; i < GorillaCaveCrystal.GorillaCaveCrystalManager.allCrystals.Count; i++)
			{
				GorillaCaveCrystal.GorillaCaveCrystalManager.allCrystals[i].InvokeUpdate();
			}
		}

		public GorillaCaveCrystalManager()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static GorillaCaveCrystalManager()
		{
		}

		public static GorillaCaveCrystal.GorillaCaveCrystalManager instance;

		public static bool hasInstance = false;

		public static List<GorillaCaveCrystal> allCrystals = new List<GorillaCaveCrystal>();
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass14_0
	{
		public <>c__DisplayClass14_0()
		{
		}

		internal bool <AddVisuals>b__0(GorillaCaveCrystalSetup.CrystalDef cd)
		{
			return cd.keyMaterial == this.crystalMat;
		}

		public Material crystalMat;
	}
}
