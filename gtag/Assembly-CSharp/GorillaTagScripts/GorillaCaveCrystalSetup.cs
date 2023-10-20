using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x020002AB RID: 683
	[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
	public class GorillaCaveCrystalSetup : ScriptableObject
	{
		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060011BD RID: 4541 RVA: 0x00064E66 File Offset: 0x00063066
		public static GorillaCaveCrystalSetup Instance
		{
			get
			{
				return GorillaCaveCrystalSetup.gInstance;
			}
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00064E6D File Offset: 0x0006306D
		private void OnEnable()
		{
			if (GorillaCaveCrystalSetup.gInstance == null)
			{
				GorillaCaveCrystalSetup.gInstance = this;
			}
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x00064E84 File Offset: 0x00063084
		public GorillaCaveCrystalSetup.CrystalDef[] GetCrystalDefs()
		{
			return (from f in typeof(GorillaCaveCrystalSetup).GetRuntimeFields()
			where f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef)
			select (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this)).ToArray<GorillaCaveCrystalSetup.CrystalDef>();
		}

		// Token: 0x060011C0 RID: 4544 RVA: 0x00064EDC File Offset: 0x000630DC
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

		// Token: 0x04001478 RID: 5240
		public Material SharedBase;

		// Token: 0x04001479 RID: 5241
		public Texture2D CrystalAlbedo;

		// Token: 0x0400147A RID: 5242
		public Texture2D CrystalDarkAlbedo;

		// Token: 0x0400147B RID: 5243
		public GorillaCaveCrystalSetup.CrystalDef Red;

		// Token: 0x0400147C RID: 5244
		public GorillaCaveCrystalSetup.CrystalDef Orange;

		// Token: 0x0400147D RID: 5245
		public GorillaCaveCrystalSetup.CrystalDef Yellow;

		// Token: 0x0400147E RID: 5246
		public GorillaCaveCrystalSetup.CrystalDef Green;

		// Token: 0x0400147F RID: 5247
		public GorillaCaveCrystalSetup.CrystalDef Teal;

		// Token: 0x04001480 RID: 5248
		public GorillaCaveCrystalSetup.CrystalDef DarkBlue;

		// Token: 0x04001481 RID: 5249
		public GorillaCaveCrystalSetup.CrystalDef Pink;

		// Token: 0x04001482 RID: 5250
		public GorillaCaveCrystalSetup.CrystalDef Dark;

		// Token: 0x04001483 RID: 5251
		public GorillaCaveCrystalSetup.CrystalDef DarkLight;

		// Token: 0x04001484 RID: 5252
		public GorillaCaveCrystalSetup.CrystalDef DarkLightUnderWater;

		// Token: 0x04001485 RID: 5253
		[SerializeField]
		[TextArea(4, 10)]
		private string _notes;

		// Token: 0x04001486 RID: 5254
		[Space]
		[SerializeField]
		private GameObject _target;

		// Token: 0x04001487 RID: 5255
		private static GorillaCaveCrystalSetup gInstance;

		// Token: 0x04001488 RID: 5256
		private static GorillaCaveCrystalSetup.CrystalDef[] gCrystalDefs;

		// Token: 0x020004B2 RID: 1202
		[Serializable]
		public class CrystalDef
		{
			// Token: 0x04001F7A RID: 8058
			public Material keyMaterial;

			// Token: 0x04001F7B RID: 8059
			public CrystalVisualsPreset visualPreset;

			// Token: 0x04001F7C RID: 8060
			[Space]
			public int low;

			// Token: 0x04001F7D RID: 8061
			public int mid;

			// Token: 0x04001F7E RID: 8062
			public int high;
		}
	}
}
