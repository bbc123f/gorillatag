using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020002FE RID: 766
	[AddComponentMenu("GorillaTag/ContainerLiquid (GTag)")]
	[ExecuteInEditMode]
	public class ContainerLiquid : MonoBehaviour
	{
		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06001580 RID: 5504 RVA: 0x0007714A File Offset: 0x0007534A
		[DebugReadout]
		public bool isEmpty
		{
			get
			{
				return this.fillAmount <= this.refillThreshold;
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06001581 RID: 5505 RVA: 0x0007715D File Offset: 0x0007535D
		// (set) Token: 0x06001582 RID: 5506 RVA: 0x00077165 File Offset: 0x00075365
		public Vector3 cupTopWorldPos { get; private set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06001583 RID: 5507 RVA: 0x0007716E File Offset: 0x0007536E
		// (set) Token: 0x06001584 RID: 5508 RVA: 0x00077176 File Offset: 0x00075376
		public Vector3 bottomLipWorldPos { get; private set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06001585 RID: 5509 RVA: 0x0007717F File Offset: 0x0007537F
		// (set) Token: 0x06001586 RID: 5510 RVA: 0x00077187 File Offset: 0x00075387
		public Vector3 liquidPlaneWorldPos { get; private set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06001587 RID: 5511 RVA: 0x00077190 File Offset: 0x00075390
		// (set) Token: 0x06001588 RID: 5512 RVA: 0x00077198 File Offset: 0x00075398
		public Vector3 liquidPlaneWorldNormal { get; private set; }

		// Token: 0x06001589 RID: 5513 RVA: 0x000771A4 File Offset: 0x000753A4
		protected bool IsValidLiquidSurfaceValues()
		{
			return this.meshRenderer != null && this.meshFilter != null && this.spillParticleSystem != null && !string.IsNullOrEmpty(this.liquidColorShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlaneNormalShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlanePositionShaderPropertyName);
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x00077208 File Offset: 0x00075408
		protected void InitializeLiquidSurface()
		{
			this.liquidColorShaderProp = Shader.PropertyToID(this.liquidColorShaderPropertyName);
			this.liquidPlaneNormalShaderProp = Shader.PropertyToID(this.liquidPlaneNormalShaderPropertyName);
			this.liquidPlanePositionShaderProp = Shader.PropertyToID(this.liquidPlanePositionShaderPropertyName);
			this.localMeshBounds = this.meshFilter.sharedMesh.bounds;
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x00077260 File Offset: 0x00075460
		protected void InitializeParticleSystem()
		{
			this.spillParticleSystem.main.startColor = this.liquidColor;
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x0007728B File Offset: 0x0007548B
		protected void Awake()
		{
			this.matPropBlock = new MaterialPropertyBlock();
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x00077298 File Offset: 0x00075498
		protected void OnEnable()
		{
			if (Application.isPlaying)
			{
				base.enabled = (this.useLiquidShader && this.IsValidLiquidSurfaceValues());
				if (base.enabled)
				{
					this.InitializeLiquidSurface();
				}
				this.InitializeParticleSystem();
				this.useFloater = (this.floater != null);
			}
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x000772EC File Offset: 0x000754EC
		protected void LateUpdate()
		{
			this.UpdateRefillTimer();
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Bounds bounds = this.meshRenderer.bounds;
			Vector3 a = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
			Vector3 b = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
			this.liquidPlaneWorldPos = Vector3.Lerp(a, b, this.fillAmount);
			Vector3 v = transform.InverseTransformPoint(this.liquidPlaneWorldPos);
			float deltaTime = Time.deltaTime;
			this.temporalWobbleAmp = Vector2.Lerp(this.temporalWobbleAmp, Vector2.zero, deltaTime * this.recovery);
			float num = 6.2831855f * this.wobbleFrequency;
			float d = Mathf.Lerp(this.lastSineWave, Mathf.Sin(num * Time.realtimeSinceStartup), deltaTime * Mathf.Clamp(this.lastVelocity.magnitude + this.lastAngularVelocity.magnitude, this.thickness, 10f));
			Vector2 vector = this.temporalWobbleAmp * d;
			this.liquidPlaneWorldNormal = new Vector3(vector.x, -1f, vector.y).normalized;
			Vector3 v2 = transform.InverseTransformDirection(this.liquidPlaneWorldNormal);
			if (this.useLiquidShader)
			{
				this.matPropBlock.SetVector(this.liquidPlaneNormalShaderProp, v2);
				this.matPropBlock.SetVector(this.liquidPlanePositionShaderProp, v);
				this.matPropBlock.SetVector(this.liquidColorShaderProp, this.liquidColor.linear);
				if (this.useLiquidVolume)
				{
					float value = MathUtils.Linear(this.fillAmount, 0f, 1f, this.liquidVolumeMinMax.x, this.liquidVolumeMinMax.y);
					this.matPropBlock.SetFloat(Shader.PropertyToID("_LiquidFill"), value);
				}
				this.meshRenderer.SetPropertyBlock(this.matPropBlock);
			}
			if (this.useFloater)
			{
				float y = Mathf.Lerp(this.localMeshBounds.min.y, this.localMeshBounds.max.y, this.fillAmount);
				this.floater.localPosition = this.floater.localPosition.WithY(y);
			}
			Vector3 vector2 = (this.lastPos - position) / deltaTime;
			Vector3 angularVelocity = GorillaMath.GetAngularVelocity(this.lastRot, rotation);
			this.temporalWobbleAmp.x = this.temporalWobbleAmp.x + Mathf.Clamp((vector2.x + vector2.y * 0.2f + angularVelocity.z + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.temporalWobbleAmp.y = this.temporalWobbleAmp.y + Mathf.Clamp((vector2.z + vector2.y * 0.2f + angularVelocity.x + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.lastPos = position;
			this.lastRot = rotation;
			this.lastSineWave = d;
			this.lastVelocity = vector2;
			this.lastAngularVelocity = angularVelocity;
			this.meshRenderer.enabled = (!this.keepMeshHidden && !this.isEmpty);
			float x = transform.lossyScale.x;
			float x2 = this.localMeshBounds.extents.x;
			float num2 = x2 * x;
			float y2 = this.localMeshBounds.extents.y;
			Vector3 vector3 = this.localMeshBounds.center + new Vector3(0f, y2, 0f);
			this.cupTopWorldPos = transform.TransformPoint(vector3);
			Vector3 up = transform.up;
			Vector3 vector4 = transform.InverseTransformDirection(Vector3.down);
			Vector3 position2 = vector3 + new Vector3(vector4.x, 0f, vector4.z).normalized * x2;
			this.bottomLipWorldPos = transform.TransformPoint(position2);
			float num3 = Mathf.Clamp01((this.liquidPlaneWorldPos.y - this.bottomLipWorldPos.y) / (num2 * 2f));
			bool flag = num3 > 1E-05f;
			ParticleSystem.EmissionModule emission = this.spillParticleSystem.emission;
			emission.enabled = flag;
			if (flag)
			{
				if (!this.spillSoundBankPlayer.isPlaying)
				{
					this.spillSoundBankPlayer.Play(null, null);
				}
				this.spillParticleSystem.transform.position = Vector3.Lerp(this.bottomLipWorldPos, this.cupTopWorldPos, num3);
				this.spillParticleSystem.shape.radius = num2 * num3;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				float num4 = num3 * this.maxSpillRate;
				rateOverTime.constant = num4;
				emission.rateOverTime = rateOverTime;
				this.fillAmount -= num4 * deltaTime * 0.01f;
			}
			if (this.isEmpty && !this.wasEmptyLastFrame && !this.emptySoundBankPlayer.isPlaying)
			{
				this.emptySoundBankPlayer.Play(null, null);
			}
			else if (!this.isEmpty && this.wasEmptyLastFrame && !this.refillSoundBankPlayer.isPlaying)
			{
				this.refillSoundBankPlayer.Play(null, null);
			}
			this.wasEmptyLastFrame = this.isEmpty;
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x0007789C File Offset: 0x00075A9C
		public void UpdateRefillTimer()
		{
			if (this.refillDelay < 0f || !this.isEmpty)
			{
				return;
			}
			if (this.refillTimer < 0f)
			{
				this.refillTimer = this.refillDelay;
				this.fillAmount = this.refillAmount;
				return;
			}
			this.refillTimer -= Time.deltaTime;
		}

		// Token: 0x0400178F RID: 6031
		[Tooltip("Used to determine the world space bounds of the container.")]
		public MeshRenderer meshRenderer;

		// Token: 0x04001790 RID: 6032
		[Tooltip("Used to determine the local space bounds of the container.")]
		public MeshFilter meshFilter;

		// Token: 0x04001791 RID: 6033
		[Tooltip("If you are only using the liquid mesh to calculate the volume of the container and do not need visuals then set this to true.")]
		public bool keepMeshHidden;

		// Token: 0x04001792 RID: 6034
		[Tooltip("The object that will float on top of the liquid.")]
		public Transform floater;

		// Token: 0x04001793 RID: 6035
		public bool useLiquidShader = true;

		// Token: 0x04001794 RID: 6036
		public bool useLiquidVolume;

		// Token: 0x04001795 RID: 6037
		public Vector2 liquidVolumeMinMax = Vector2.up;

		// Token: 0x04001796 RID: 6038
		public string liquidColorShaderPropertyName = "_BaseColor";

		// Token: 0x04001797 RID: 6039
		public string liquidPlaneNormalShaderPropertyName = "_LiquidPlaneNormal";

		// Token: 0x04001798 RID: 6040
		public string liquidPlanePositionShaderPropertyName = "_LiquidPlanePosition";

		// Token: 0x04001799 RID: 6041
		[Tooltip("Emits drips when pouring.")]
		public ParticleSystem spillParticleSystem;

		// Token: 0x0400179A RID: 6042
		[SoundBankInfo]
		public SoundBankPlayer emptySoundBankPlayer;

		// Token: 0x0400179B RID: 6043
		[SoundBankInfo]
		public SoundBankPlayer refillSoundBankPlayer;

		// Token: 0x0400179C RID: 6044
		[SoundBankInfo]
		public SoundBankPlayer spillSoundBankPlayer;

		// Token: 0x0400179D RID: 6045
		public Color liquidColor = new Color(0.33f, 0.25f, 0.21f, 1f);

		// Token: 0x0400179E RID: 6046
		[Tooltip("The amount of liquid currently in the container. This value is passed to the shader.")]
		[Range(0f, 1f)]
		public float fillAmount = 0.85f;

		// Token: 0x0400179F RID: 6047
		[Tooltip("This is what fillAmount will be after automatic refilling.")]
		public float refillAmount = 0.85f;

		// Token: 0x040017A0 RID: 6048
		[Tooltip("Set to a negative value to disable.")]
		public float refillDelay = 10f;

		// Token: 0x040017A1 RID: 6049
		[Tooltip("The point that the liquid should be considered empty and should be auto refilled.")]
		public float refillThreshold = 0.1f;

		// Token: 0x040017A2 RID: 6050
		public float wobbleMax = 0.2f;

		// Token: 0x040017A3 RID: 6051
		public float wobbleFrequency = 1f;

		// Token: 0x040017A4 RID: 6052
		public float recovery = 1f;

		// Token: 0x040017A5 RID: 6053
		public float thickness = 1f;

		// Token: 0x040017A6 RID: 6054
		public float maxSpillRate = 100f;

		// Token: 0x040017AB RID: 6059
		[DebugReadout]
		private bool wasEmptyLastFrame;

		// Token: 0x040017AC RID: 6060
		private int liquidColorShaderProp;

		// Token: 0x040017AD RID: 6061
		private int liquidPlaneNormalShaderProp;

		// Token: 0x040017AE RID: 6062
		private int liquidPlanePositionShaderProp;

		// Token: 0x040017AF RID: 6063
		private float refillTimer;

		// Token: 0x040017B0 RID: 6064
		private float lastSineWave;

		// Token: 0x040017B1 RID: 6065
		private float lastWobble;

		// Token: 0x040017B2 RID: 6066
		private Vector2 temporalWobbleAmp;

		// Token: 0x040017B3 RID: 6067
		private Vector3 lastPos;

		// Token: 0x040017B4 RID: 6068
		private Vector3 lastVelocity;

		// Token: 0x040017B5 RID: 6069
		private Vector3 lastAngularVelocity;

		// Token: 0x040017B6 RID: 6070
		private Quaternion lastRot;

		// Token: 0x040017B7 RID: 6071
		private MaterialPropertyBlock matPropBlock;

		// Token: 0x040017B8 RID: 6072
		private Bounds localMeshBounds;

		// Token: 0x040017B9 RID: 6073
		private bool useFloater;
	}
}
