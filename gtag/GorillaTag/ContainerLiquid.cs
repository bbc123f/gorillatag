using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag;

[AddComponentMenu("GorillaTag/ContainerLiquid (GTag)")]
[ExecuteInEditMode]
public class ContainerLiquid : MonoBehaviour
{
	[Tooltip("Used to determine the world space bounds of the container.")]
	public MeshRenderer meshRenderer;

	[Tooltip("Used to determine the local space bounds of the container.")]
	public MeshFilter meshFilter;

	[Tooltip("If you are only using the liquid mesh to calculate the volume of the container and do not need visuals then set this to true.")]
	public bool keepMeshHidden;

	[Tooltip("The object that will float on top of the liquid.")]
	public Transform floater;

	public bool useLiquidShader = true;

	public string liquidColorShaderPropertyName = "_Color";

	public string liquidPlaneNormalShaderPropertyName = "_LiquidPlaneNormal";

	public string liquidPlanePositionShaderPropertyName = "_LiquidPlanePosition";

	[Tooltip("Emits drips when pouring.")]
	public ParticleSystem spillParticleSystem;

	[SoundBankInfo]
	public SoundBankPlayer emptySoundBankPlayer;

	[SoundBankInfo]
	public SoundBankPlayer refillSoundBankPlayer;

	[SoundBankInfo]
	public SoundBankPlayer spillSoundBankPlayer;

	public Color liquidColor = new Color(0.33f, 0.25f, 0.21f, 1f);

	[Tooltip("The amount of liquid currently in the container. This value is passed to the shader.")]
	[Range(0f, 1f)]
	public float fillAmount = 0.85f;

	[Tooltip("This is what fillAmount will be after automatic refilling.")]
	public float refillAmount = 0.85f;

	[Tooltip("Set to a negative value to disable.")]
	public float refillDelay = 10f;

	[Tooltip("The point that the liquid should be considered empty and should be auto refilled.")]
	public float refillThreshold = 0.1f;

	public float wobbleMax = 0.2f;

	public float wobbleFrequency = 1f;

	public float recovery = 1f;

	public float thickness = 1f;

	public float maxSpillRate = 100f;

	[DebugReadout]
	private bool wasEmptyLastFrame;

	private int liquidColorShaderProp;

	private int liquidPlaneNormalShaderProp;

	private int liquidPlanePositionShaderProp;

	private float refillTimer;

	private float lastSineWave;

	private float lastWobble;

	private Vector2 temporalWobbleAmp;

	private Vector3 lastPos;

	private Vector3 lastVelocity;

	private Vector3 lastAngularVelocity;

	private Quaternion lastRot;

	private MaterialPropertyBlock matPropBlock;

	private Bounds localMeshBounds;

	private bool useFloater;

	[DebugReadout]
	public bool isEmpty => fillAmount <= refillThreshold;

	public Vector3 cupTopWorldPos { get; private set; }

	public Vector3 bottomLipWorldPos { get; private set; }

	public Vector3 liquidPlaneWorldPos { get; private set; }

	public Vector3 liquidPlaneWorldNormal { get; private set; }

	protected bool IsValidLiquidSurfaceValues()
	{
		if (meshRenderer != null && meshFilter != null && spillParticleSystem != null && !string.IsNullOrEmpty(liquidColorShaderPropertyName) && !string.IsNullOrEmpty(liquidPlaneNormalShaderPropertyName))
		{
			return !string.IsNullOrEmpty(liquidPlanePositionShaderPropertyName);
		}
		return false;
	}

	protected void InitializeLiquidSurface()
	{
		liquidColorShaderProp = Shader.PropertyToID(liquidColorShaderPropertyName);
		liquidPlaneNormalShaderProp = Shader.PropertyToID(liquidPlaneNormalShaderPropertyName);
		liquidPlanePositionShaderProp = Shader.PropertyToID(liquidPlanePositionShaderPropertyName);
		localMeshBounds = meshFilter.sharedMesh.bounds;
	}

	protected void InitializeParticleSystem()
	{
		ParticleSystem.MainModule main = spillParticleSystem.main;
		main.startColor = liquidColor;
	}

	protected void Awake()
	{
		matPropBlock = new MaterialPropertyBlock();
	}

	protected void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.enabled = useLiquidShader && IsValidLiquidSurfaceValues();
			if (base.enabled)
			{
				InitializeLiquidSurface();
			}
			InitializeParticleSystem();
			useFloater = floater != null;
		}
	}

	protected void LateUpdate()
	{
		UpdateRefillTimer();
		Transform transform = base.transform;
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Bounds bounds = meshRenderer.bounds;
		Vector3 a = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
		Vector3 b = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
		liquidPlaneWorldPos = Vector3.Lerp(a, b, fillAmount);
		Vector3 vector = transform.InverseTransformPoint(liquidPlaneWorldPos);
		float deltaTime = Time.deltaTime;
		temporalWobbleAmp = Vector2.Lerp(temporalWobbleAmp, Vector2.zero, deltaTime * recovery);
		float num = (float)Math.PI * 2f * wobbleFrequency;
		float num2 = Mathf.Lerp(lastSineWave, Mathf.Sin(num * Time.realtimeSinceStartup), deltaTime * Mathf.Clamp(lastVelocity.magnitude + lastAngularVelocity.magnitude, thickness, 10f));
		Vector2 vector2 = temporalWobbleAmp * num2;
		liquidPlaneWorldNormal = new Vector3(vector2.x, -1f, vector2.y).normalized;
		Vector3 vector3 = transform.InverseTransformDirection(liquidPlaneWorldNormal);
		if (useLiquidShader)
		{
			matPropBlock.SetVector(liquidPlaneNormalShaderProp, vector3);
			matPropBlock.SetVector(liquidPlanePositionShaderProp, vector);
			matPropBlock.SetVector(liquidColorShaderProp, liquidColor.linear);
			meshRenderer.SetPropertyBlock(matPropBlock);
		}
		if (useFloater)
		{
			float y = Mathf.Lerp(localMeshBounds.min.y, localMeshBounds.max.y, fillAmount);
			floater.localPosition = floater.localPosition.WithY(y);
		}
		Vector3 vector4 = (lastPos - position) / deltaTime;
		Vector3 angularVelocity = GorillaMath.GetAngularVelocity(lastRot, rotation);
		temporalWobbleAmp.x += Mathf.Clamp((vector4.x + vector4.y * 0.2f + angularVelocity.z + angularVelocity.y) * wobbleMax, 0f - wobbleMax, wobbleMax);
		temporalWobbleAmp.y += Mathf.Clamp((vector4.z + vector4.y * 0.2f + angularVelocity.x + angularVelocity.y) * wobbleMax, 0f - wobbleMax, wobbleMax);
		lastPos = position;
		lastRot = rotation;
		lastSineWave = num2;
		lastVelocity = vector4;
		lastAngularVelocity = angularVelocity;
		meshRenderer.enabled = !keepMeshHidden && !isEmpty;
		float x = transform.lossyScale.x;
		float x2 = localMeshBounds.extents.x;
		float num3 = x2 * x;
		float y2 = localMeshBounds.extents.y;
		Vector3 vector5 = localMeshBounds.center + new Vector3(0f, y2, 0f);
		cupTopWorldPos = transform.TransformPoint(vector5);
		_ = transform.up;
		Vector3 vector6 = transform.InverseTransformDirection(Vector3.down);
		Vector3 position2 = vector5 + new Vector3(vector6.x, 0f, vector6.z).normalized * x2;
		bottomLipWorldPos = transform.TransformPoint(position2);
		float num4 = Mathf.Clamp01((liquidPlaneWorldPos.y - bottomLipWorldPos.y) / (num3 * 2f));
		bool flag = num4 > 1E-05f;
		ParticleSystem.EmissionModule emission = spillParticleSystem.emission;
		emission.enabled = flag;
		if (flag)
		{
			if (!spillSoundBankPlayer.isPlaying)
			{
				spillSoundBankPlayer.Play();
			}
			spillParticleSystem.transform.position = Vector3.Lerp(bottomLipWorldPos, cupTopWorldPos, num4);
			ParticleSystem.ShapeModule shape = spillParticleSystem.shape;
			shape.radius = num3 * num4;
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			float num6 = (rateOverTime.constant = num4 * maxSpillRate);
			emission.rateOverTime = rateOverTime;
			fillAmount -= num6 * deltaTime * 0.01f;
		}
		if (isEmpty && !wasEmptyLastFrame && !emptySoundBankPlayer.isPlaying)
		{
			emptySoundBankPlayer.Play();
		}
		else if (!isEmpty && wasEmptyLastFrame && !refillSoundBankPlayer.isPlaying)
		{
			refillSoundBankPlayer.Play();
		}
		wasEmptyLastFrame = isEmpty;
	}

	public void UpdateRefillTimer()
	{
		if (!(refillDelay < 0f) && isEmpty)
		{
			if (refillTimer < 0f)
			{
				refillTimer = refillDelay;
				fillAmount = refillAmount;
			}
			else
			{
				refillTimer -= Time.deltaTime;
			}
		}
	}
}
