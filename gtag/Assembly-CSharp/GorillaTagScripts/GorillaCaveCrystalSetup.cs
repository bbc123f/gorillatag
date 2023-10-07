using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x020002A9 RID: 681
	[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
	public class GorillaCaveCrystalSetup : ScriptableObject
	{
		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060011B6 RID: 4534 RVA: 0x000649FE File Offset: 0x00062BFE
		public static GorillaCaveCrystalSetup Instance
		{
			get
			{
				return GorillaCaveCrystalSetup.gInstance;
			}
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00064A05 File Offset: 0x00062C05
		private void OnEnable()
		{
			if (GorillaCaveCrystalSetup.gInstance == null)
			{
				GorillaCaveCrystalSetup.gInstance = this;
			}
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00064A1C File Offset: 0x00062C1C
		public GorillaCaveCrystalSetup.CrystalDef[] GetCrystalDefs()
		{
			return (from f in typeof(GorillaCaveCrystalSetup).GetRuntimeFields()
			where f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef)
			select (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this)).ToArray<GorillaCaveCrystalSetup.CrystalDef>();
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x00064A74 File Offset: 0x00062C74
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

		// Token: 0x0400146B RID: 5227
		public Material SharedBase;

		// Token: 0x0400146C RID: 5228
		public Texture2D CrystalAlbedo;

		// Token: 0x0400146D RID: 5229
		public Texture2D CrystalDarkAlbedo;

		// Token: 0x0400146E RID: 5230
		public GorillaCaveCrystalSetup.CrystalDef Red;

		// Token: 0x0400146F RID: 5231
		public GorillaCaveCrystalSetup.CrystalDef Orange;

		// Token: 0x04001470 RID: 5232
		public GorillaCaveCrystalSetup.CrystalDef Yellow;

		// Token: 0x04001471 RID: 5233
		public GorillaCaveCrystalSetup.CrystalDef Green;

		// Token: 0x04001472 RID: 5234
		public GorillaCaveCrystalSetup.CrystalDef Teal;

		// Token: 0x04001473 RID: 5235
		public GorillaCaveCrystalSetup.CrystalDef DarkBlue;

		// Token: 0x04001474 RID: 5236
		public GorillaCaveCrystalSetup.CrystalDef Pink;

		// Token: 0x04001475 RID: 5237
		public GorillaCaveCrystalSetup.CrystalDef Dark;

		// Token: 0x04001476 RID: 5238
		public GorillaCaveCrystalSetup.CrystalDef DarkLight;

		// Token: 0x04001477 RID: 5239
		public GorillaCaveCrystalSetup.CrystalDef DarkLightUnderWater;

		// Token: 0x04001478 RID: 5240
		[SerializeField]
		[TextArea(4, 10)]
		private string _notes;

		// Token: 0x04001479 RID: 5241
		[Space]
		[SerializeField]
		private GameObject _target;

		// Token: 0x0400147A RID: 5242
		private static GorillaCaveCrystalSetup gInstance;

		// Token: 0x0400147B RID: 5243
		private static GorillaCaveCrystalSetup.CrystalDef[] gCrystalDefs;

		// Token: 0x020004B0 RID: 1200
		[Serializable]
		public class CrystalDef
		{
			// Token: 0x04001F6D RID: 8045
			public Material keyMaterial;

			// Token: 0x04001F6E RID: 8046
			public CrystalVisualsPreset visualPreset;

			// Token: 0x04001F6F RID: 8047
			[Space]
			public int low;

			// Token: 0x04001F70 RID: 8048
			public int mid;

			// Token: 0x04001F71 RID: 8049
			public int high;
		}
	}
}
