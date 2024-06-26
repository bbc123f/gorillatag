﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTagScripts
{
	[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
	public class GorillaCaveCrystalSetup : ScriptableObject
	{
		public static GorillaCaveCrystalSetup Instance
		{
			get
			{
				return GorillaCaveCrystalSetup.gInstance;
			}
		}

		private void OnEnable()
		{
			if (GorillaCaveCrystalSetup.gInstance == null)
			{
				GorillaCaveCrystalSetup.gInstance = this;
			}
		}

		public GorillaCaveCrystalSetup.CrystalDef[] GetCrystalDefs()
		{
			return (from f in typeof(GorillaCaveCrystalSetup).GetRuntimeFields()
			where f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef)
			select (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this)).ToArray<GorillaCaveCrystalSetup.CrystalDef>();
		}

		[Conditional("UNITY_EDITOR")]
		private void ProcessTarget()
		{
			if (this._target == null)
			{
				return;
			}
			GorillaCaveCrystal[] componentsInChildren = this._target.GetComponentsInChildren<GorillaCaveCrystal>();
			if (componentsInChildren.Length == 0)
			{
				return;
			}
			GorillaCaveCrystal[] array = (from c in componentsInChildren
			where c.visuals == null || !c.visuals
			select c).ToArray<GorillaCaveCrystal>();
			if (array.Length == 0)
			{
				return;
			}
			foreach (GorillaCaveCrystal gorillaCaveCrystal in array)
			{
			}
		}

		public GorillaCaveCrystalSetup()
		{
		}

		[CompilerGenerated]
		private GorillaCaveCrystalSetup.CrystalDef <GetCrystalDefs>b__19_1(FieldInfo f)
		{
			return (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this);
		}

		public Material SharedBase;

		public Texture2D CrystalAlbedo;

		public Texture2D CrystalDarkAlbedo;

		public GorillaCaveCrystalSetup.CrystalDef Red;

		public GorillaCaveCrystalSetup.CrystalDef Orange;

		public GorillaCaveCrystalSetup.CrystalDef Yellow;

		public GorillaCaveCrystalSetup.CrystalDef Green;

		public GorillaCaveCrystalSetup.CrystalDef Teal;

		public GorillaCaveCrystalSetup.CrystalDef DarkBlue;

		public GorillaCaveCrystalSetup.CrystalDef Pink;

		public GorillaCaveCrystalSetup.CrystalDef Dark;

		public GorillaCaveCrystalSetup.CrystalDef DarkLight;

		public GorillaCaveCrystalSetup.CrystalDef DarkLightUnderWater;

		[SerializeField]
		[TextArea(4, 10)]
		private string _notes;

		[Space]
		[SerializeField]
		private GameObject _target;

		private static GorillaCaveCrystalSetup gInstance;

		private static GorillaCaveCrystalSetup.CrystalDef[] gCrystalDefs;

		[Serializable]
		public class CrystalDef
		{
			public CrystalDef()
			{
			}

			public Material keyMaterial;

			public CrystalVisualsPreset visualPreset;

			[Space]
			public int low;

			public int mid;

			public int high;
		}

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			public <>c()
			{
			}

			internal bool <GetCrystalDefs>b__19_0(FieldInfo f)
			{
				return f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef);
			}

			internal bool <ProcessTarget>b__21_0(GorillaCaveCrystal c)
			{
				return c.visuals == null || !c.visuals;
			}

			public static readonly GorillaCaveCrystalSetup.<>c <>9 = new GorillaCaveCrystalSetup.<>c();

			public static Func<FieldInfo, bool> <>9__19_0;

			public static Func<GorillaCaveCrystal, bool> <>9__21_0;
		}
	}
}
