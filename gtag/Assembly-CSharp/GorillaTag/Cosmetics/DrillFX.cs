using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000334 RID: 820
	public class DrillFX : MonoBehaviour
	{
		// Token: 0x060016BC RID: 5820 RVA: 0x0007E8CC File Offset: 0x0007CACC
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

		// Token: 0x060016BD RID: 5821 RVA: 0x0007E9A8 File Offset: 0x0007CBA8
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

		// Token: 0x060016BE RID: 5822 RVA: 0x0007EA0C File Offset: 0x0007CC0C
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

		// Token: 0x060016BF RID: 5823 RVA: 0x0007EA5C File Offset: 0x0007CC5C
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

		// Token: 0x060016C0 RID: 5824 RVA: 0x0007EB72 File Offset: 0x0007CD72
		private static void HandleApplicationQuitting()
		{
			DrillFX.appIsQuitting = true;
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x0007EB7C File Offset: 0x0007CD7C
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

		// Token: 0x040018E7 RID: 6375
		[SerializeField]
		private ParticleSystem fx;

		// Token: 0x040018E8 RID: 6376
		[SerializeField]
		private AnimationCurve fxEmissionCurve;

		// Token: 0x040018E9 RID: 6377
		[SerializeField]
		private float fxMinRadiusScale = 0.01f;

		// Token: 0x040018EA RID: 6378
		[Tooltip("Right click menu has custom menu items. Anything starting with \"- \" is custom.")]
		[SerializeField]
		private AudioSource loopAudio;

		// Token: 0x040018EB RID: 6379
		[SerializeField]
		private AnimationCurve loopAudioVolumeCurve;

		// Token: 0x040018EC RID: 6380
		[Tooltip("Higher value makes it reach the target volume faster.")]
		[SerializeField]
		private float loopAudioVolumeTransitionSpeed = 3f;

		// Token: 0x040018ED RID: 6381
		[FormerlySerializedAs("layerMask")]
		[Tooltip("The collision layers the line cast should intersect with")]
		[SerializeField]
		private LayerMask lineCastLayerMask;

		// Token: 0x040018EE RID: 6382
		[Tooltip("The position in local space that the line cast starts.")]
		[SerializeField]
		private Vector3 lineCastStart = Vector3.zero;

		// Token: 0x040018EF RID: 6383
		[Tooltip("The position in local space that the line cast ends.")]
		[SerializeField]
		private Vector3 lineCastEnd = Vector3.forward;

		// Token: 0x040018F0 RID: 6384
		private static bool appIsQuitting;

		// Token: 0x040018F1 RID: 6385
		private static bool appIsQuittingHandlerIsSubscribed;

		// Token: 0x040018F2 RID: 6386
		private float maxDepth;

		// Token: 0x040018F3 RID: 6387
		private bool hasFX;

		// Token: 0x040018F4 RID: 6388
		private ParticleSystem.EmissionModule fxEmissionModule;

		// Token: 0x040018F5 RID: 6389
		private float fxEmissionMaxRate;

		// Token: 0x040018F6 RID: 6390
		private ParticleSystem.ShapeModule fxShapeModule;

		// Token: 0x040018F7 RID: 6391
		private float fxShapeMaxRadius;

		// Token: 0x040018F8 RID: 6392
		private bool hasAudio;

		// Token: 0x040018F9 RID: 6393
		private float audioMaxVolume;
	}
}
