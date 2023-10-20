using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E4 RID: 740
	public class RayToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060013F8 RID: 5112 RVA: 0x00071ACF File Offset: 0x0006FCCF
		// (set) Token: 0x060013F9 RID: 5113 RVA: 0x00071ADC File Offset: 0x0006FCDC
		public bool EnableState
		{
			get
			{
				return this._lineRenderer.enabled;
			}
			set
			{
				this._targetTransform.gameObject.SetActive(value);
				this._lineRenderer.enabled = value;
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060013FA RID: 5114 RVA: 0x00071AFB File Offset: 0x0006FCFB
		// (set) Token: 0x060013FB RID: 5115 RVA: 0x00071B03 File Offset: 0x0006FD03
		public bool ToolActivateState
		{
			get
			{
				return this._toolActivateState;
			}
			set
			{
				this._toolActivateState = value;
				this._lineRenderer.colorGradient = (this._toolActivateState ? this._highLightColorGradient : this._oldColorGradient);
			}
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x00071B30 File Offset: 0x0006FD30
		private void Awake()
		{
			this._lineRenderer.positionCount = 25;
			this._oldColorGradient = this._lineRenderer.colorGradient;
			this._highLightColorGradient = new Gradient();
			this._highLightColorGradient.SetKeys(new GradientColorKey[]
			{
				new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 0f),
				new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 1f)
			}, new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			});
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060013FD RID: 5117 RVA: 0x00071BF3 File Offset: 0x0006FDF3
		// (set) Token: 0x060013FE RID: 5118 RVA: 0x00071BFB File Offset: 0x0006FDFB
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x060013FF RID: 5119 RVA: 0x00071C04 File Offset: 0x0006FE04
		public void SetFocusedInteractable(Interactable interactable)
		{
			if (interactable == null)
			{
				this._focusedTransform = null;
				return;
			}
			this._focusedTransform = interactable.transform;
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x00071C24 File Offset: 0x0006FE24
		private void Update()
		{
			Vector3 position = this.InteractableTool.ToolTransform.position;
			Vector3 forward = this.InteractableTool.ToolTransform.forward;
			Vector3 vector = (this._focusedTransform != null) ? this._focusedTransform.position : (position + forward * 3f);
			float magnitude = (vector - position).magnitude;
			Vector3 p = position;
			Vector3 p2 = position + forward * magnitude * 0.3333333f;
			Vector3 p3 = position + forward * magnitude * 0.6666667f;
			Vector3 p4 = vector;
			for (int i = 0; i < 25; i++)
			{
				this.linePositions[i] = RayToolView.GetPointOnBezierCurve(p, p2, p3, p4, (float)i / 25f);
			}
			this._lineRenderer.SetPositions(this.linePositions);
			this._targetTransform.position = vector;
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x00071D1C File Offset: 0x0006FF1C
		public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			float num2 = num * num;
			float num3 = t * t;
			return num * num2 * p0 + 3f * num2 * t * p1 + 3f * num * num3 * p2 + t * num3 * p3;
		}

		// Token: 0x040016B1 RID: 5809
		private const int NUM_RAY_LINE_POSITIONS = 25;

		// Token: 0x040016B2 RID: 5810
		private const float DEFAULT_RAY_CAST_DISTANCE = 3f;

		// Token: 0x040016B3 RID: 5811
		[SerializeField]
		private Transform _targetTransform;

		// Token: 0x040016B4 RID: 5812
		[SerializeField]
		private LineRenderer _lineRenderer;

		// Token: 0x040016B5 RID: 5813
		private bool _toolActivateState;

		// Token: 0x040016B6 RID: 5814
		private Transform _focusedTransform;

		// Token: 0x040016B7 RID: 5815
		private Vector3[] linePositions = new Vector3[25];

		// Token: 0x040016B8 RID: 5816
		private Gradient _oldColorGradient;

		// Token: 0x040016B9 RID: 5817
		private Gradient _highLightColorGradient;
	}
}
