using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x0200032C RID: 812
	[ExecuteInEditMode]
	public class ZoneShaderSettings : MonoBehaviour
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06001690 RID: 5776 RVA: 0x0007D7F3 File Offset: 0x0007B9F3
		// (set) Token: 0x06001691 RID: 5777 RVA: 0x0007D7FA File Offset: 0x0007B9FA
		[DebugReadout]
		public static ZoneShaderSettings defaultsInstance { get; private set; }

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06001692 RID: 5778 RVA: 0x0007D802 File Offset: 0x0007BA02
		// (set) Token: 0x06001693 RID: 5779 RVA: 0x0007D809 File Offset: 0x0007BA09
		public static bool hasDefaultsInstance { get; private set; }

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06001694 RID: 5780 RVA: 0x0007D811 File Offset: 0x0007BA11
		// (set) Token: 0x06001695 RID: 5781 RVA: 0x0007D818 File Offset: 0x0007BA18
		[DebugReadout]
		public static ZoneShaderSettings activeInstance { get; private set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06001696 RID: 5782 RVA: 0x0007D820 File Offset: 0x0007BA20
		// (set) Token: 0x06001697 RID: 5783 RVA: 0x0007D827 File Offset: 0x0007BA27
		public static bool hasActiveInstance { get; private set; }

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06001698 RID: 5784 RVA: 0x0007D82F File Offset: 0x0007BA2F
		[DebugReadout]
		private float GroundFogDepthFadeSq
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogDepthFadeSize * this._groundFogDepthFadeSize);
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06001699 RID: 5785 RVA: 0x0007D84E File Offset: 0x0007BA4E
		[DebugReadout]
		private float GroundFogHeightFade
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogHeightFadeSize);
			}
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x0007D868 File Offset: 0x0007BA68
		public void SetZoneLiquidTypeKeywordEnum(ZoneShaderSettings.EZoneLiquidType liquidType)
		{
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.None)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.Water)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.Lava)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
				return;
			}
			Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x0007D8C4 File Offset: 0x0007BAC4
		protected void Awake()
		{
			if (base.gameObject.scene.name == null)
			{
				return;
			}
			if (!this.isDefaultValues)
			{
				return;
			}
			if (ZoneShaderSettings.hasDefaultsInstance && Application.isPlaying)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			ZoneShaderSettings.defaultsInstance = this;
			ZoneShaderSettings.hasDefaultsInstance = true;
			if (!Application.isPlaying)
			{
				return;
			}
			ZoneShaderSettings.ApplyDefaultValues();
			Application.quitting += delegate()
			{
				ZoneShaderSettings.appIsQuitting = true;
				ZoneShaderSettings.hasDefaultsInstance = false;
				ZoneShaderSettings.hasActiveInstance = false;
			};
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x0007D948 File Offset: 0x0007BB48
		protected void OnDestroy()
		{
			if (base.gameObject.scene.name == null)
			{
				return;
			}
			if (ZoneShaderSettings.defaultsInstance == this)
			{
				ZoneShaderSettings.hasDefaultsInstance = false;
			}
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x0007D980 File Offset: 0x0007BB80
		protected void OnEnable()
		{
			this.hasMainWaterSurfacePlane = ((this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues) && this.mainWaterSurfacePlane != null);
			if (this.isDefaultValues || ZoneShaderSettings.appIsQuitting)
			{
				return;
			}
			if (!ZoneShaderSettings.activeInstanceQueue.Contains(this))
			{
				ZoneShaderSettings.activeInstanceQueue.Add(this);
			}
			ZoneShaderSettings.hasActiveInstance = (ZoneShaderSettings.activeInstance != null);
			if (ZoneShaderSettings.hasActiveInstance && GorillaTagger.hasInstance)
			{
				Vector3 position = GorillaTagger.Instance.transform.position;
				float sqrMagnitude = (base.transform.position - position).sqrMagnitude;
				float sqrMagnitude2 = (ZoneShaderSettings.activeInstance.transform.position - position).sqrMagnitude;
				ZoneShaderSettings.activeInstance = ((sqrMagnitude < sqrMagnitude2) ? this : ZoneShaderSettings.activeInstance);
			}
			else
			{
				ZoneShaderSettings.activeInstance = this;
			}
			ZoneShaderSettings.hasActiveInstance = (ZoneShaderSettings.activeInstance != null);
			if (!Application.isPlaying)
			{
				return;
			}
			ZoneShaderSettings.activeInstance.ApplyValues();
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x0007DA7C File Offset: 0x0007BC7C
		protected void OnDisable()
		{
			if (base.gameObject.scene.name == null)
			{
				return;
			}
			if (this.isDefaultValues || ZoneShaderSettings.appIsQuitting)
			{
				return;
			}
			int num = ZoneShaderSettings.activeInstanceQueue.IndexOf(this);
			if (num >= 0)
			{
				ZoneShaderSettings.activeInstanceQueue.RemoveAt(num);
			}
			if (!Application.isPlaying)
			{
				return;
			}
			ZoneShaderSettings activeInstance;
			if (ZoneShaderSettings.TryGetClosestActiveInstanceToPlayer(out activeInstance))
			{
				ZoneShaderSettings.hasActiveInstance = true;
				ZoneShaderSettings.activeInstance = activeInstance;
			}
			else
			{
				ZoneShaderSettings.hasActiveInstance = false;
				ZoneShaderSettings.activeInstance = null;
			}
			if (!ZoneShaderSettings.hasActiveInstance && ZoneShaderSettings.hasDefaultsInstance)
			{
				ZoneShaderSettings.activeInstance = ZoneShaderSettings.defaultsInstance;
				ZoneShaderSettings.hasActiveInstance = (ZoneShaderSettings.defaultsInstance != null);
			}
			if (ZoneShaderSettings.hasActiveInstance)
			{
				ZoneShaderSettings.activeInstance.ApplyValues();
			}
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x0007DB30 File Offset: 0x0007BD30
		private static bool TryGetClosestActiveInstanceToPlayer(out ZoneShaderSettings closest)
		{
			closest = null;
			float num = float.MaxValue;
			if (!GorillaTagger.hasInstance)
			{
				return false;
			}
			Vector3 position = GorillaTagger.Instance.transform.position;
			for (int i = ZoneShaderSettings.activeInstanceQueue.Count - 1; i >= 0; i--)
			{
				ZoneShaderSettings zoneShaderSettings = ZoneShaderSettings.activeInstanceQueue[i];
				if (zoneShaderSettings == null)
				{
					ZoneShaderSettings.activeInstanceQueue.RemoveAt(i);
				}
				else
				{
					float sqrMagnitude = (zoneShaderSettings.transform.position - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						closest = zoneShaderSettings;
						num = sqrMagnitude;
					}
				}
			}
			return closest != null;
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x0007DBC7 File Offset: 0x0007BDC7
		protected void LateUpdate()
		{
			if (!this.hasMainWaterSurfacePlane || this.mainWaterSurfacePlane.gameObject.isStatic || !ZoneShaderSettings.hasDefaultsInstance || ZoneShaderSettings.appIsQuitting || ZoneShaderSettings.activeInstance != this)
			{
				return;
			}
			this.UpdateMainPlaneShaderProperty();
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x0007DC08 File Offset: 0x0007BE08
		private void UpdateMainPlaneShaderProperty()
		{
			Transform transform = null;
			bool flag = false;
			if (this.hasMainWaterSurfacePlane && (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues))
			{
				flag = true;
				transform = this.mainWaterSurfacePlane;
			}
			else if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance.hasMainWaterSurfacePlane)
			{
				flag = true;
				transform = ZoneShaderSettings.defaultsInstance.mainWaterSurfacePlane;
			}
			if (!flag)
			{
				return;
			}
			Vector3 up = transform.up;
			float w = -Vector3.Dot(up, transform.position);
			Shader.SetGlobalVector(this.shaderParam_GlobalMainWaterSurfacePlane, new Vector4(up.x, up.y, up.z, w));
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x0007DCA4 File Offset: 0x0007BEA4
		private void ApplyValues()
		{
			if (!ZoneShaderSettings.hasDefaultsInstance || ZoneShaderSettings.appIsQuitting)
			{
				return;
			}
			this.ApplyColor(ZoneShaderSettings.groundFogColor_shaderProp, this.groundFogColor_overrideMode, this.groundFogColor, ZoneShaderSettings.defaultsInstance.groundFogColor);
			this.ApplyFloat(ZoneShaderSettings.groundFogDepthFadeSq_shaderProp, this.groundFogHeightFade_overrideMode, this.GroundFogDepthFadeSq, ZoneShaderSettings.defaultsInstance.GroundFogDepthFadeSq);
			this.ApplyFloat(ZoneShaderSettings.groundFogHeight_shaderProp, this.groundFogHeight_overrideMode, this.groundFogHeight, ZoneShaderSettings.defaultsInstance.groundFogHeight);
			this.ApplyFloat(ZoneShaderSettings.groundFogHeightFade_shaderProp, this.groundFogHeightFade_overrideMode, this.GroundFogHeightFade, ZoneShaderSettings.defaultsInstance.GroundFogHeightFade);
			if (this.zoneLiquidType_overrideMode != ZoneShaderSettings.EOverrideMode.LeaveUnchanged)
			{
				ZoneShaderSettings.EZoneLiquidType zoneLiquidTypeKeywordEnum = (this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.zoneLiquidType : ZoneShaderSettings.defaultsInstance.zoneLiquidType;
				this.SetZoneLiquidTypeKeywordEnum(zoneLiquidTypeKeywordEnum);
			}
			if (this.zoneLiquidUVScale_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalFloat(ZoneShaderSettings.shaderParam_GlobalZoneLiquidUVScale, this.zoneLiquidUVScale);
			}
			else if (this.zoneLiquidUVScale_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalFloat(ZoneShaderSettings.shaderParam_GlobalZoneLiquidUVScale, this.zoneLiquidUVScale);
			}
			if (this.underwaterTintColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalColor(ZoneShaderSettings.shaderParam_GlobalWaterTintColor, this.underwaterTintColor.linear);
			}
			else if (this.underwaterTintColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(ZoneShaderSettings.shaderParam_GlobalWaterTintColor, ZoneShaderSettings.defaultsInstance.underwaterTintColor.linear);
			}
			if (this.underwaterFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalColor(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogColor, this.underwaterFogColor.linear);
			}
			else if (this.underwaterFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogColor, ZoneShaderSettings.defaultsInstance.underwaterFogColor.linear);
			}
			if (this.underwaterFogParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogParams, this.underwaterFogParams);
			}
			else if (this.underwaterFogParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogParams, ZoneShaderSettings.defaultsInstance.underwaterFogParams);
			}
			if (this.underwaterCausticsParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsParams, this.underwaterCausticsParams);
			}
			else if (this.underwaterCausticsParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsParams, ZoneShaderSettings.defaultsInstance.underwaterCausticsParams);
			}
			if (this.underwaterCausticsTexture_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalTexture(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsTex, this.underwaterCausticsTexture);
			}
			else if (this.underwaterCausticsTexture_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalTexture(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsTex, ZoneShaderSettings.defaultsInstance.underwaterCausticsTexture);
			}
			if (this.underwaterEffectsDistanceToSurfaceFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade, this.underwaterEffectsDistanceToSurfaceFade);
			}
			else if (this.underwaterEffectsDistanceToSurfaceFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade, ZoneShaderSettings.defaultsInstance.underwaterEffectsDistanceToSurfaceFade);
			}
			if (this.liquidResidueTex_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalTexture(ZoneShaderSettings.shaderParam_GlobalLiquidResidueTex, this.liquidResidueTex);
			}
			else if (this.liquidResidueTex_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalTexture(ZoneShaderSettings.shaderParam_GlobalLiquidResidueTex, ZoneShaderSettings.defaultsInstance.liquidResidueTex);
			}
			this.UpdateMainPlaneShaderProperty();
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x0007DF8F File Offset: 0x0007C18F
		public static void ApplyDefaultValues()
		{
			if (!ZoneShaderSettings.hasDefaultsInstance || ZoneShaderSettings.appIsQuitting)
			{
				return;
			}
			ZoneShaderSettings.defaultsInstance.ApplyValues();
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x0007DFAA File Offset: 0x0007C1AA
		private void ApplyColor(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Color value, Color defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalColor(shaderProp, value.linear);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(shaderProp, defaultValue.linear);
			}
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x0007DFD7 File Offset: 0x0007C1D7
		private void ApplyFloat(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, float value, float defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalFloat(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalFloat(shaderProp, defaultValue);
			}
		}

		// Token: 0x0400189A RID: 6298
		private static bool appIsQuitting = false;

		// Token: 0x0400189F RID: 6303
		[DebugReadout]
		[NonSerialized]
		public static List<ZoneShaderSettings> activeInstanceQueue = new List<ZoneShaderSettings>(32);

		// Token: 0x040018A0 RID: 6304
		[Tooltip("These values will be used as the default global values that will be fallen back to when not in a zone and that the other scripts will reference.")]
		public bool isDefaultValues;

		// Token: 0x040018A1 RID: 6305
		private static readonly int groundFogColor_shaderProp = Shader.PropertyToID("_ZoneGroundFogColor");

		// Token: 0x040018A2 RID: 6306
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogColor_overrideMode;

		// Token: 0x040018A3 RID: 6307
		[SerializeField]
		private Color groundFogColor = new Color(0.7f, 0.9f, 1f, 1f);

		// Token: 0x040018A4 RID: 6308
		private static readonly int groundFogDepthFadeSq_shaderProp = Shader.PropertyToID("_ZoneGroundFogDepthFadeSq");

		// Token: 0x040018A5 RID: 6309
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogDepthFade_overrideMode;

		// Token: 0x040018A6 RID: 6310
		[SerializeField]
		private float _groundFogDepthFadeSize = 20f;

		// Token: 0x040018A7 RID: 6311
		private static readonly int groundFogHeight_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeight");

		// Token: 0x040018A8 RID: 6312
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeight_overrideMode;

		// Token: 0x040018A9 RID: 6313
		[SerializeField]
		private float groundFogHeight = 7.45f;

		// Token: 0x040018AA RID: 6314
		private static readonly int groundFogHeightFade_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeightFade");

		// Token: 0x040018AB RID: 6315
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeightFade_overrideMode;

		// Token: 0x040018AC RID: 6316
		[SerializeField]
		private float _groundFogHeightFadeSize = 20f;

		// Token: 0x040018AD RID: 6317
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidType_overrideMode;

		// Token: 0x040018AE RID: 6318
		[SerializeField]
		private ZoneShaderSettings.EZoneLiquidType zoneLiquidType = ZoneShaderSettings.EZoneLiquidType.Water;

		// Token: 0x040018AF RID: 6319
		private static readonly int shaderParam_GlobalZoneLiquidUVScale = Shader.PropertyToID("_GlobalZoneLiquidUVScale");

		// Token: 0x040018B0 RID: 6320
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidUVScale_overrideMode;

		// Token: 0x040018B1 RID: 6321
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private float zoneLiquidUVScale = 1f;

		// Token: 0x040018B2 RID: 6322
		private static readonly int shaderParam_GlobalWaterTintColor = Shader.PropertyToID("_GlobalWaterTintColor");

		// Token: 0x040018B3 RID: 6323
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterTintColor_overrideMode;

		// Token: 0x040018B4 RID: 6324
		[SerializeField]
		private Color underwaterTintColor = new Color(0.3f, 0.65f, 1f, 0.2f);

		// Token: 0x040018B5 RID: 6325
		private static readonly int shaderParam_GlobalUnderwaterFogColor = Shader.PropertyToID("_GlobalUnderwaterFogColor");

		// Token: 0x040018B6 RID: 6326
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogColor_overrideMode;

		// Token: 0x040018B7 RID: 6327
		[SerializeField]
		private Color underwaterFogColor = new Color(0.12f, 0.41f, 0.77f);

		// Token: 0x040018B8 RID: 6328
		private static readonly int shaderParam_GlobalUnderwaterFogParams = Shader.PropertyToID("_GlobalUnderwaterFogParams");

		// Token: 0x040018B9 RID: 6329
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogParams_overrideMode;

		// Token: 0x040018BA RID: 6330
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private Vector4 underwaterFogParams = new Vector4(-5f, 40f, 0f, 0f);

		// Token: 0x040018BB RID: 6331
		private static readonly int shaderParam_GlobalUnderwaterCausticsParams = Shader.PropertyToID("_GlobalUnderwaterCausticsParams");

		// Token: 0x040018BC RID: 6332
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsParams_overrideMode;

		// Token: 0x040018BD RID: 6333
		[Tooltip("Caustics params are: speed1, scale, alpha, unused")]
		[SerializeField]
		private Vector4 underwaterCausticsParams = new Vector4(0.075f, 0.075f, 1f, 0f);

		// Token: 0x040018BE RID: 6334
		private static readonly int shaderParam_GlobalUnderwaterCausticsTex = Shader.PropertyToID("_GlobalUnderwaterCausticsTex");

		// Token: 0x040018BF RID: 6335
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsTexture_overrideMode;

		// Token: 0x040018C0 RID: 6336
		[SerializeField]
		private Texture2D underwaterCausticsTexture;

		// Token: 0x040018C1 RID: 6337
		private static readonly int shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade = Shader.PropertyToID("_GlobalUnderwaterEffectsDistanceToSurfaceFade");

		// Token: 0x040018C2 RID: 6338
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterEffectsDistanceToSurfaceFade_overrideMode;

		// Token: 0x040018C3 RID: 6339
		[SerializeField]
		private Vector2 underwaterEffectsDistanceToSurfaceFade = new Vector2(0.0001f, 50f);

		// Token: 0x040018C4 RID: 6340
		private const string kEdTooltip_liquidResidueTex = "This is used for things like the charred surface effect when lava burns static geo.";

		// Token: 0x040018C5 RID: 6341
		private static readonly int shaderParam_GlobalLiquidResidueTex = Shader.PropertyToID("_GlobalLiquidResidueTex");

		// Token: 0x040018C6 RID: 6342
		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private ZoneShaderSettings.EOverrideMode liquidResidueTex_overrideMode;

		// Token: 0x040018C7 RID: 6343
		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private Texture2D liquidResidueTex;

		// Token: 0x040018C8 RID: 6344
		private readonly int shaderParam_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

		// Token: 0x040018C9 RID: 6345
		private bool hasMainWaterSurfacePlane;

		// Token: 0x040018CA RID: 6346
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode mainWaterSurfacePlane_overrideMode;

		// Token: 0x040018CB RID: 6347
		[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
		[SerializeField]
		private Transform mainWaterSurfacePlane;

		// Token: 0x02000510 RID: 1296
		public enum EOverrideMode
		{
			// Token: 0x0400212C RID: 8492
			LeaveUnchanged,
			// Token: 0x0400212D RID: 8493
			ApplyNewValue,
			// Token: 0x0400212E RID: 8494
			ApplyDefaultValue
		}

		// Token: 0x02000511 RID: 1297
		public enum EZoneLiquidType
		{
			// Token: 0x04002130 RID: 8496
			None,
			// Token: 0x04002131 RID: 8497
			Water,
			// Token: 0x04002132 RID: 8498
			Lava
		}
	}
}
