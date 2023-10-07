using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E2 RID: 738
	public class RayToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060013F1 RID: 5105 RVA: 0x00071603 File Offset: 0x0006F803
		// (set) Token: 0x060013F2 RID: 5106 RVA: 0x00071610 File Offset: 0x0006F810
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

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060013F3 RID: 5107 RVA: 0x0007162F File Offset: 0x0006F82F
		// (set) Token: 0x060013F4 RID: 5108 RVA: 0x00071637 File Offset: 0x0006F837
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

		// Token: 0x060013F5 RID: 5109 RVA: 0x00071664 File Offset: 0x0006F864
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

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060013F6 RID: 5110 RVA: 0x00071727 File Offset: 0x0006F927
		// (set) Token: 0x060013F7 RID: 5111 RVA: 0x0007172F File Offset: 0x0006F92F
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x060013F8 RID: 5112 RVA: 0x00071738 File Offset: 0x0006F938
		public void SetFocusedInteractable(Interactable interactable)
		{
			if (interactable == null)
			{
				this._focusedTransform = null;
				return;
			}
			this._focusedTransform = interactable.transform;
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x00071758 File Offset: 0x0006F958
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

		// Token: 0x060013FA RID: 5114 RVA: 0x00071850 File Offset: 0x0006FA50
		public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			float num2 = num * num;
			float num3 = t * t;
			return num * num2 * p0 + 3f * num2 * t * p1 + 3f * num * num3 * p2 + t * num3 * p3;
		}

		// Token: 0x040016A4 RID: 5796
		private const int NUM_RAY_LINE_POSITIONS = 25;

		// Token: 0x040016A5 RID: 5797
		private const float DEFAULT_RAY_CAST_DISTANCE = 3f;

		// Token: 0x040016A6 RID: 5798
		[SerializeField]
		private Transform _targetTransform;

		// Token: 0x040016A7 RID: 5799
		[SerializeField]
		private LineRenderer _lineRenderer;

		// Token: 0x040016A8 RID: 5800
		private bool _toolActivateState;

		// Token: 0x040016A9 RID: 5801
		private Transform _focusedTransform;

		// Token: 0x040016AA RID: 5802
		private Vector3[] linePositions = new Vector3[25];

		// Token: 0x040016AB RID: 5803
		private Gradient _oldColorGradient;

		// Token: 0x040016AC RID: 5804
		private Gradient _highLightColorGradient;
	}
}
