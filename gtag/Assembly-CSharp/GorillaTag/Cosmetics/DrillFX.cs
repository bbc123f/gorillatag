using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000332 RID: 818
	public class DrillFX : MonoBehaviour
	{
		// Token: 0x060016B3 RID: 5811 RVA: 0x0007E3E4 File Offset: 0x0007C5E4
		protected void Awake()
		{
			if (!DrillFX.appIsQuittingHandlerIsSubscribed)
			{
				DrillFX.appIsQuittingHandlerIsSubscribed = true;
				Application.quitting += DrillFX.HandleApplicationQuitting;
			}
			this.hasFX = (this.fx != null);
			if (this.hasFX)
			{
				this.fxEmissionModule = this.fx.emission;
				this.fxEmissionMaxRate = this.fxEmissionModule.rateOverTimeMultiplier;
				this.fxShapeModule = this.fx.shape;
				this.fxShapeMaxRadius = this.fxShapeModule.radius;
			}
			this.hasAudio = (this.loopAudio != null);
			if (this.hasAudio)
			{
				this.audioMaxVolume = this.loopAudio.volume;
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.Play();
			}
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x0007E4C0 File Offset: 0x0007C6C0
		protected void OnEnable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.Play();
			}
			this.ValidateLineCastPositions();
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x0007E524 File Offset: 0x0007C724
		protected void OnDisable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.Stop();
			}
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x0007E574 File Offset: 0x0007C774
		protected void LateUpdate()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			Transform transform = base.transform;
			RaycastHit raycastHit;
			Vector3 position = Physics.Linecast(transform.TransformPoint(this.lineCastStart), transform.TransformPoint(this.lineCastEnd), out raycastHit, this.lineCastLayerMask, QueryTriggerInteraction.Ignore) ? raycastHit.point : this.lineCastEnd;
			Vector3 vector = transform.InverseTransformPoint(position);
			float num = Mathf.Clamp01(Vector3.Distance(this.lineCastStart, vector) / this.maxDepth);
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = this.fxEmissionMaxRate * this.fxEmissionCurve.Evaluate(num);
				this.fxShapeModule.position = vector;
				this.fxShapeModule.radius = Mathf.Lerp(this.fxShapeMaxRadius, this.fxMinRadiusScale * this.fxShapeMaxRadius, num);
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = Mathf.MoveTowards(this.loopAudio.volume, this.audioMaxVolume * this.loopAudioVolumeCurve.Evaluate(num), this.loopAudioVolumeTransitionSpeed * Time.deltaTime);
			}
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x0007E68A File Offset: 0x0007C88A
		private static void HandleApplicationQuitting()
		{
			DrillFX.appIsQuitting = true;
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x0007E694 File Offset: 0x0007C894
		private bool ValidateLineCastPositions()
		{
			this.maxDepth = Vector3.Distance(this.lineCastStart, this.lineCastEnd);
			if (this.maxDepth > 1E-45f)
			{
				return true;
			}
			if (Application.isPlaying)
			{
				Debug.Log("DrillFX: lineCastStart and End are too close together. Disabling component.", this);
				base.enabled = false;
			}
			return false;
		}

		// Token: 0x040018DA RID: 6362
		[SerializeField]
		private ParticleSystem fx;

		// Token: 0x040018DB RID: 6363
		[SerializeField]
		private AnimationCurve fxEmissionCurve;

		// Token: 0x040018DC RID: 6364
		[SerializeField]
		private float fxMinRadiusScale = 0.01f;

		// Token: 0x040018DD RID: 6365
		[Tooltip("Right click menu has custom menu items. Anything starting with \"- \" is custom.")]
		[SerializeField]
		private AudioSource loopAudio;

		// Token: 0x040018DE RID: 6366
		[SerializeField]
		private AnimationCurve loopAudioVolumeCurve;

		// Token: 0x040018DF RID: 6367
		[Tooltip("Higher value makes it reach the target volume faster.")]
		[SerializeField]
		private float loopAudioVolumeTransitionSpeed = 3f;

		// Token: 0x040018E0 RID: 6368
		[FormerlySerializedAs("layerMask")]
		[Tooltip("The collision layers the line cast should intersect with")]
		[SerializeField]
		private LayerMask lineCastLayerMask;

		// Token: 0x040018E1 RID: 6369
		[Tooltip("The position in local space that the line cast starts.")]
		[SerializeField]
		private Vector3 lineCastStart = Vector3.zero;

		// Token: 0x040018E2 RID: 6370
		[Tooltip("The position in local space that the line cast ends.")]
		[SerializeField]
		private Vector3 lineCastEnd = Vector3.forward;

		// Token: 0x040018E3 RID: 6371
		private static bool appIsQuitting;

		// Token: 0x040018E4 RID: 6372
		private static bool appIsQuittingHandlerIsSubscribed;

		// Token: 0x040018E5 RID: 6373
		private float maxDepth;

		// Token: 0x040018E6 RID: 6374
		private bool hasFX;

		// Token: 0x040018E7 RID: 6375
		private ParticleSystem.EmissionModule fxEmissionModule;

		// Token: 0x040018E8 RID: 6376
		private float fxEmissionMaxRate;

		// Token: 0x040018E9 RID: 6377
		private ParticleSystem.ShapeModule fxShapeModule;

		// Token: 0x040018EA RID: 6378
		private float fxShapeMaxRadius;

		// Token: 0x040018EB RID: 6379
		private bool hasAudio;

		// Token: 0x040018EC RID: 6380
		private float audioMaxVolume;
	}
}
