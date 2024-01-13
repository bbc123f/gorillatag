using UnityEngine;

namespace OculusSampleFramework;

public class RayToolView : MonoBehaviour, InteractableToolView
{
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

	public bool EnableState
	{
		get
		{
			return _lineRenderer.enabled;
		}
		set
		{
			_targetTransform.gameObject.SetActive(value);
			_lineRenderer.enabled = value;
		}
	}

	public bool ToolActivateState
	{
		get
		{
			return _toolActivateState;
		}
		set
		{
			_toolActivateState = value;
			_lineRenderer.colorGradient = (_toolActivateState ? _highLightColorGradient : _oldColorGradient);
		}
	}

	public InteractableTool InteractableTool { get; set; }

	private void Awake()
	{
		_lineRenderer.positionCount = 25;
		_oldColorGradient = _lineRenderer.colorGradient;
		_highLightColorGradient = new Gradient();
		_highLightColorGradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 0f),
			new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		});
	}

	public void SetFocusedInteractable(Interactable interactable)
	{
		if (interactable == null)
		{
			_focusedTransform = null;
		}
		else
		{
			_focusedTransform = interactable.transform;
		}
	}

	private void Update()
	{
		Vector3 position = InteractableTool.ToolTransform.position;
		Vector3 forward = InteractableTool.ToolTransform.forward;
		Vector3 vector = ((_focusedTransform != null) ? _focusedTransform.position : (position + forward * 3f));
		float magnitude = (vector - position).magnitude;
		Vector3 p = position;
		Vector3 p2 = position + forward * magnitude * 0.3333333f;
		Vector3 p3 = position + forward * magnitude * (2f / 3f);
		Vector3 p4 = vector;
		for (int i = 0; i < 25; i++)
		{
			linePositions[i] = GetPointOnBezierCurve(p, p2, p3, p4, (float)i / 25f);
		}
		_lineRenderer.SetPositions(linePositions);
		_targetTransform.position = vector;
	}

	public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		float num2 = num * num;
		float num3 = t * t;
		return num * num2 * p0 + 3f * num2 * t * p1 + 3f * num * num3 * p2 + t * num3 * p3;
	}
}
