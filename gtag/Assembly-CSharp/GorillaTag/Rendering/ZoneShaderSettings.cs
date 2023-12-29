using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering
{
	[ExecuteInEditMode]
	public class ZoneShaderSettings : MonoBehaviour
	{
		[DebugReadout]
		public static ZoneShaderSettings defaultsInstance { get; private set; }

		public static bool hasDefaultsInstance { get; private set; }

		[DebugReadout]
		public static ZoneShaderSettings activeInstance { get; private set; }

		public static bool hasActiveInstance { get; private set; }

		[DebugReadout]
		private float GroundFogDepthFadeSq
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogDepthFadeSize * this._groundFogDepthFadeSize);
			}
		}

		[DebugReadout]
		private float GroundFogHeightFade
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogHeightFadeSize);
			}
		}

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

		protected void LateUpdate()
		{
			if (!this.hasMainWaterSurfacePlane || this.mainWaterSurfacePlane.gameObject.isStatic || !ZoneShaderSettings.hasDefaultsInstance || ZoneShaderSettings.appIsQuitting || ZoneShaderSettings.activeInstance != this)
			{
				return;
			}
			this.UpdateMainPlaneShaderProperty();
		}

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

		public static void ApplyDefaultValues()
		{
			if (!ZoneShaderSettings.hasDefaultsInstance || ZoneShaderSettings.appIsQuitting)
			{
				return;
			}
			ZoneShaderSettings.defaultsInstance.ApplyValues();
		}

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

		private static bool appIsQuitting = false;

		[DebugReadout]
		[NonSerialized]
		public static List<ZoneShaderSettings> activeInstanceQueue = new List<ZoneShaderSettings>(32);

		[Tooltip("These values will be used as the default global values that will be fallen back to when not in a zone and that the other scripts will reference.")]
		public bool isDefaultValues;

		private static readonly int groundFogColor_shaderProp = Shader.PropertyToID("_ZoneGroundFogColor");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogColor_overrideMode;

		[SerializeField]
		private Color groundFogColor = new Color(0.7f, 0.9f, 1f, 1f);

		private static readonly int groundFogDepthFadeSq_shaderProp = Shader.PropertyToID("_ZoneGroundFogDepthFadeSq");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogDepthFade_overrideMode;

		[SerializeField]
		private float _groundFogDepthFadeSize = 20f;

		private static readonly int groundFogHeight_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeight");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeight_overrideMode;

		[SerializeField]
		private float groundFogHeight = 7.45f;

		private static readonly int groundFogHeightFade_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeightFade");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeightFade_overrideMode;

		[SerializeField]
		private float _groundFogHeightFadeSize = 20f;

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidType_overrideMode;

		[SerializeField]
		private ZoneShaderSettings.EZoneLiquidType zoneLiquidType = ZoneShaderSettings.EZoneLiquidType.Water;

		private static readonly int shaderParam_GlobalZoneLiquidUVScale = Shader.PropertyToID("_GlobalZoneLiquidUVScale");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidUVScale_overrideMode;

		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private float zoneLiquidUVScale = 1f;

		private static readonly int shaderParam_GlobalWaterTintColor = Shader.PropertyToID("_GlobalWaterTintColor");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterTintColor_overrideMode;

		[SerializeField]
		private Color underwaterTintColor = new Color(0.3f, 0.65f, 1f, 0.2f);

		private static readonly int shaderParam_GlobalUnderwaterFogColor = Shader.PropertyToID("_GlobalUnderwaterFogColor");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogColor_overrideMode;

		[SerializeField]
		private Color underwaterFogColor = new Color(0.12f, 0.41f, 0.77f);

		private static readonly int shaderParam_GlobalUnderwaterFogParams = Shader.PropertyToID("_GlobalUnderwaterFogParams");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogParams_overrideMode;

		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private Vector4 underwaterFogParams = new Vector4(-5f, 40f, 0f, 0f);

		private static readonly int shaderParam_GlobalUnderwaterCausticsParams = Shader.PropertyToID("_GlobalUnderwaterCausticsParams");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsParams_overrideMode;

		[Tooltip("Caustics params are: speed1, scale, alpha, unused")]
		[SerializeField]
		private Vector4 underwaterCausticsParams = new Vector4(0.075f, 0.075f, 1f, 0f);

		private static readonly int shaderParam_GlobalUnderwaterCausticsTex = Shader.PropertyToID("_GlobalUnderwaterCausticsTex");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsTexture_overrideMode;

		[SerializeField]
		private Texture2D underwaterCausticsTexture;

		private static readonly int shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade = Shader.PropertyToID("_GlobalUnderwaterEffectsDistanceToSurfaceFade");

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterEffectsDistanceToSurfaceFade_overrideMode;

		[SerializeField]
		private Vector2 underwaterEffectsDistanceToSurfaceFade = new Vector2(0.0001f, 50f);

		private const string kEdTooltip_liquidResidueTex = "This is used for things like the charred surface effect when lava burns static geo.";

		private static readonly int shaderParam_GlobalLiquidResidueTex = Shader.PropertyToID("_GlobalLiquidResidueTex");

		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private ZoneShaderSettings.EOverrideMode liquidResidueTex_overrideMode;

		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private Texture2D liquidResidueTex;

		private readonly int shaderParam_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

		private bool hasMainWaterSurfacePlane;

		[SerializeField]
		private ZoneShaderSettings.EOverrideMode mainWaterSurfacePlane_overrideMode;

		[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
		[SerializeField]
		private Transform mainWaterSurfacePlane;

		public enum EOverrideMode
		{
			LeaveUnchanged,
			ApplyNewValue,
			ApplyDefaultValue
		}

		public enum EZoneLiquidType
		{
			None,
			Water,
			Lava
		}
	}
}
