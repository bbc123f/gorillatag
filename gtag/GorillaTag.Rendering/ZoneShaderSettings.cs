using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering;

[ExecuteInEditMode]
public class ZoneShaderSettings : MonoBehaviour
{
	public enum EOverrideMode
	{
		LeaveUnchanged,
		ApplyNewValue,
		ApplyDefaultValue
	}

	private static bool appIsQuitting = false;

	public static ZoneShaderSettings defaultsInstance;

	public static bool hasDefaultsInstance;

	[NonSerialized]
	public static ZoneShaderSettings activeInstance;

	[NonSerialized]
	public static bool hasActiveInstance;

	[NonSerialized]
	public static List<ZoneShaderSettings> activeInstanceQueue = new List<ZoneShaderSettings>(32);

	[Tooltip("These values will be used as the default global values that will be fallen back to when not in a zone and that the other scripts will reference.")]
	public bool isDefaultValues;

	private static readonly int shaderParam_GlobalWaterTintColor = Shader.PropertyToID("_GlobalWaterTintColor");

	[SerializeField]
	private EOverrideMode underwaterTintColor_overrideMode;

	[SerializeField]
	private Color underwaterTintColor = new Color(0.3f, 0.65f, 1f, 0.2f);

	private static readonly int shaderParam_GlobalUnderwaterFogColor = Shader.PropertyToID("_GlobalUnderwaterFogColor");

	[SerializeField]
	private EOverrideMode underwaterFogColor_overrideMode;

	[SerializeField]
	private Color underwaterFogColor = new Color(0.12f, 0.41f, 0.77f);

	private static readonly int shaderParam_GlobalUnderwaterFogParams = Shader.PropertyToID("_GlobalUnderwaterFogParams");

	[SerializeField]
	private EOverrideMode underwaterFogParams_overrideMode;

	[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
	[SerializeField]
	private Vector4 underwaterFogParams = new Vector4(-5f, 40f, 0f, 0f);

	private static readonly int shaderParam_GlobalUnderwaterCausticsParams = Shader.PropertyToID("_GlobalUnderwaterCausticsParams");

	[SerializeField]
	private EOverrideMode underwaterCausticsParams_overrideMode;

	[Tooltip("Caustics params are: speed1, scale, alpha, unused")]
	[SerializeField]
	private Vector4 underwaterCausticsParams = new Vector4(0.075f, 0.075f, 1f, 0f);

	private static readonly int shaderParam_GlobalUnderwaterCausticsTex = Shader.PropertyToID("_GlobalUnderwaterCausticsTex");

	[SerializeField]
	private EOverrideMode underwaterCausticsTexture_overrideMode;

	[SerializeField]
	private Texture2D underwaterCausticsTexture;

	private static readonly int shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade = Shader.PropertyToID("_GlobalUnderwaterEffectsDistanceToSurfaceFade");

	[SerializeField]
	private EOverrideMode underwaterEffectsDistanceToSurfaceFade_overrideMode;

	[SerializeField]
	private Vector2 underwaterEffectsDistanceToSurfaceFade = new Vector2(0.0001f, 50f);

	private readonly int shaderParam__GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

	private bool hasMainWaterSurfacePlane;

	[SerializeField]
	private EOverrideMode mainWaterSurfacePlane_overrideMode;

	[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
	[SerializeField]
	private Transform mainWaterSurfacePlane;

	protected void Awake()
	{
		if (base.gameObject.scene.name == null || !isDefaultValues)
		{
			return;
		}
		if (hasDefaultsInstance && Application.isPlaying)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		defaultsInstance = this;
		hasDefaultsInstance = true;
		if (Application.isPlaying)
		{
			ApplyDefaultValues();
			Application.quitting += delegate
			{
				appIsQuitting = true;
				hasDefaultsInstance = false;
				hasActiveInstance = false;
			};
		}
	}

	protected void OnDestroy()
	{
		if (base.gameObject.scene.name != null && defaultsInstance == this)
		{
			hasDefaultsInstance = false;
		}
	}

	protected void OnEnable()
	{
		if (base.gameObject.scene.name == null)
		{
			return;
		}
		hasMainWaterSurfacePlane = mainWaterSurfacePlane_overrideMode != EOverrideMode.ApplyNewValue && mainWaterSurfacePlane != null;
		if (!isDefaultValues && !appIsQuitting)
		{
			if (!activeInstanceQueue.Contains(this))
			{
				activeInstanceQueue.Add(this);
			}
			activeInstance = this;
			hasActiveInstance = true;
			if (Application.isPlaying)
			{
				ApplyValues();
			}
		}
	}

	protected void OnDisable()
	{
		if (base.gameObject.scene.name == null || isDefaultValues || appIsQuitting)
		{
			return;
		}
		int num = activeInstanceQueue.IndexOf(this);
		if (num >= 0)
		{
			activeInstanceQueue.RemoveAt(num);
		}
		if (!Application.isPlaying)
		{
			return;
		}
		hasActiveInstance = false;
		while (!hasActiveInstance && activeInstanceQueue.Count > 0)
		{
			activeInstance = activeInstanceQueue[activeInstanceQueue.Count - 1];
			hasActiveInstance = activeInstance != null;
			if (!hasActiveInstance)
			{
				activeInstanceQueue.RemoveAt(activeInstanceQueue.Count - 1);
			}
		}
		if (!hasActiveInstance && hasDefaultsInstance)
		{
			activeInstance = defaultsInstance;
			hasActiveInstance = defaultsInstance != null;
		}
		if (hasActiveInstance)
		{
			activeInstance.ApplyValues();
		}
	}

	private void UpdateMainPlaneShaderProperty()
	{
		Transform transform = null;
		bool flag = false;
		if (hasMainWaterSurfacePlane && (mainWaterSurfacePlane_overrideMode == EOverrideMode.ApplyNewValue || isDefaultValues))
		{
			flag = true;
			transform = mainWaterSurfacePlane;
		}
		else if (mainWaterSurfacePlane_overrideMode == EOverrideMode.ApplyDefaultValue && hasDefaultsInstance && defaultsInstance.hasMainWaterSurfacePlane)
		{
			flag = true;
			transform = defaultsInstance.mainWaterSurfacePlane;
		}
		if (flag)
		{
			Vector3 up = transform.up;
			float w = 0f - Vector3.Dot(up, transform.position);
			Shader.SetGlobalVector(shaderParam__GlobalMainWaterSurfacePlane, new Vector4(up.x, up.y, up.z, w));
		}
	}

	private void ApplyValues()
	{
		if (hasDefaultsInstance && !appIsQuitting)
		{
			if (underwaterTintColor_overrideMode == EOverrideMode.ApplyNewValue || isDefaultValues)
			{
				Shader.SetGlobalColor(shaderParam_GlobalWaterTintColor, underwaterTintColor.linear);
			}
			else if (underwaterTintColor_overrideMode == EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(shaderParam_GlobalWaterTintColor, defaultsInstance.underwaterTintColor.linear);
			}
			if (underwaterFogColor_overrideMode == EOverrideMode.ApplyNewValue || isDefaultValues)
			{
				Shader.SetGlobalColor(shaderParam_GlobalUnderwaterFogColor, underwaterFogColor.linear);
			}
			else if (underwaterFogColor_overrideMode == EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(shaderParam_GlobalUnderwaterFogColor, defaultsInstance.underwaterFogColor.linear);
			}
			if (underwaterFogParams_overrideMode == EOverrideMode.ApplyNewValue || isDefaultValues)
			{
				Shader.SetGlobalVector(shaderParam_GlobalUnderwaterFogParams, underwaterFogParams);
			}
			else if (underwaterFogParams_overrideMode == EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderParam_GlobalUnderwaterFogParams, defaultsInstance.underwaterFogParams);
			}
			if (underwaterCausticsParams_overrideMode == EOverrideMode.ApplyNewValue || isDefaultValues)
			{
				Shader.SetGlobalVector(shaderParam_GlobalUnderwaterCausticsParams, underwaterCausticsParams);
			}
			else if (underwaterCausticsParams_overrideMode == EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderParam_GlobalUnderwaterCausticsParams, defaultsInstance.underwaterCausticsParams);
			}
			if (underwaterCausticsTexture_overrideMode == EOverrideMode.ApplyNewValue || isDefaultValues)
			{
				Shader.SetGlobalTexture(shaderParam_GlobalUnderwaterCausticsTex, underwaterCausticsTexture);
			}
			else if (underwaterCausticsTexture_overrideMode == EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalTexture(shaderParam_GlobalUnderwaterCausticsTex, defaultsInstance.underwaterCausticsTexture);
			}
			if (underwaterEffectsDistanceToSurfaceFade_overrideMode == EOverrideMode.ApplyNewValue || isDefaultValues)
			{
				Shader.SetGlobalVector(shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade, underwaterEffectsDistanceToSurfaceFade);
			}
			else if (underwaterEffectsDistanceToSurfaceFade_overrideMode == EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade, defaultsInstance.underwaterEffectsDistanceToSurfaceFade);
			}
			UpdateMainPlaneShaderProperty();
		}
	}

	public static void ApplyDefaultValues()
	{
		if (hasDefaultsInstance && !appIsQuitting)
		{
			defaultsInstance.ApplyValues();
		}
	}
}
