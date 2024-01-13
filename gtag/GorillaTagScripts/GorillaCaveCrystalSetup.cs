using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GorillaTagScripts;

[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
public class GorillaCaveCrystalSetup : ScriptableObject
{
	[Serializable]
	public class CrystalDef
	{
		public Material keyMaterial;

		public CrystalVisualsPreset visualPreset;

		[Space]
		public int low;

		public int mid;

		public int high;
	}

	public Material SharedBase;

	public Texture2D CrystalAlbedo;

	public Texture2D CrystalDarkAlbedo;

	public CrystalDef Red;

	public CrystalDef Orange;

	public CrystalDef Yellow;

	public CrystalDef Green;

	public CrystalDef Teal;

	public CrystalDef DarkBlue;

	public CrystalDef Pink;

	public CrystalDef Dark;

	public CrystalDef DarkLight;

	public CrystalDef DarkLightUnderWater;

	[SerializeField]
	[TextArea(4, 10)]
	private string _notes;

	[Space]
	[SerializeField]
	private GameObject _target;

	private static GorillaCaveCrystalSetup gInstance;

	private static CrystalDef[] gCrystalDefs;

	public static GorillaCaveCrystalSetup Instance => gInstance;

	private void OnEnable()
	{
		if (gInstance == null)
		{
			gInstance = this;
		}
	}

	public CrystalDef[] GetCrystalDefs()
	{
		return (from f in typeof(GorillaCaveCrystalSetup).GetRuntimeFields()
			where f != null && f.FieldType == typeof(CrystalDef)
			select (CrystalDef)f.GetValue(this)).ToArray();
	}

	[Conditional("UNITY_EDITOR")]
	private void ProcessTarget()
	{
		if (_target == null)
		{
			return;
		}
		GorillaCaveCrystal[] componentsInChildren = _target.GetComponentsInChildren<GorillaCaveCrystal>();
		if (componentsInChildren.Length == 0)
		{
			return;
		}
		GorillaCaveCrystal[] array = componentsInChildren.Where((GorillaCaveCrystal c) => c.visuals == null || !c.visuals).ToArray();
		if (array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddVisuals();
			}
		}
	}
}
