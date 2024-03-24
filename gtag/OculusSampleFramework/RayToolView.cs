using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OculusSampleFramework
{
	public class RayToolView : MonoBehaviour, InteractableToolView
	{
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

		public InteractableTool InteractableTool
		{
			[CompilerGenerated]
			get
			{
				return this.<InteractableTool>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<InteractableTool>k__BackingField = value;
			}
		}

		public void SetFocusedInteractable(Interactable interactable)
		{
			if (interactable == null)
			{
				this._focusedTransform = null;
				return;
			}
			this._focusedTransform = interactable.transform;
		}

		private void Update()
		{
			Vector3 position = this.InteractableTool.ToolTransform.position;
			Vector3 forward = this.InteractableTool.ToolTransform.forward;
			Vector3 vector = ((this._focusedTransform != null) ? this._focusedTransform.position : (position + forward * 3f));
			float magnitude = (vector - position).magnitude;
			Vector3 vector2 = position;
			Vector3 vector3 = position + forward * magnitude * 0.3333333f;
			Vector3 vector4 = position + forward * magnitude * 0.6666667f;
			Vector3 vector5 = vector;
			for (int i = 0; i < 25; i++)
			{
				this.linePositions[i] = RayToolView.GetPointOnBezierCurve(vector2, vector3, vector4, vector5, (float)i / 25f);
			}
			this._lineRenderer.SetPositions(this.linePositions);
			this._targetTransform.position = vector;
		}

		public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			float num2 = num * num;
			float num3 = t * t;
			return num * num2 * p0 + 3f * num2 * t * p1 + 3f * num * num3 * p2 + t * num3 * p3;
		}

		public RayToolView()
		{
		}

		private const int NUM_RAY_LINE_POSITIONS = 25;

		private const float DEFAULT_RAY_CAST_DISTANCE = 3f;

		[SerializeField]
		private Transform _targetTransform;

		[SerializeField]
		private LineRenderer _lineRenderer;

		private bool _toolActivateState;

		private Transform _focusedTransform;

		private Vector3[] linePositions = new Vector3[25];

		private Gradient _oldColorGradient;

		private Gradient _highLightColorGradient;

		[CompilerGenerated]
		private InteractableTool <InteractableTool>k__BackingField;
	}
}
