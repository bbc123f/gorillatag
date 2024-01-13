using UnityEngine;

public class LaserPointer : OVRCursor
{
	public enum LaserBeamBehavior
	{
		On,
		Off,
		OnWhenHitTarget
	}

	public GameObject cursorVisual;

	public float maxLength = 10f;

	private LaserBeamBehavior _laserBeamBehavior;

	private bool m_restoreOnInputAcquired;

	private Vector3 _startPoint;

	private Vector3 _forward;

	private Vector3 _endPoint;

	private bool _hitTarget;

	private LineRenderer lineRenderer;

	public LaserBeamBehavior laserBeamBehavior
	{
		get
		{
			return _laserBeamBehavior;
		}
		set
		{
			_laserBeamBehavior = value;
			if (laserBeamBehavior == LaserBeamBehavior.Off || laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
			{
				lineRenderer.enabled = false;
			}
			else
			{
				lineRenderer.enabled = true;
			}
		}
	}

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	private void Start()
	{
		if ((bool)cursorVisual)
		{
			cursorVisual.SetActive(value: false);
		}
		OVRManager.InputFocusAcquired += OnInputFocusAcquired;
		OVRManager.InputFocusLost += OnInputFocusLost;
	}

	public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
	{
		_startPoint = start;
		_endPoint = dest;
		_hitTarget = true;
	}

	public override void SetCursorRay(Transform t)
	{
		_startPoint = t.position;
		_forward = t.forward;
		_hitTarget = false;
	}

	private void LateUpdate()
	{
		lineRenderer.SetPosition(0, _startPoint);
		if (_hitTarget)
		{
			lineRenderer.SetPosition(1, _endPoint);
			UpdateLaserBeam(_startPoint, _endPoint);
			if ((bool)cursorVisual)
			{
				cursorVisual.transform.position = _endPoint;
				cursorVisual.SetActive(value: true);
			}
		}
		else
		{
			UpdateLaserBeam(_startPoint, _startPoint + maxLength * _forward);
			lineRenderer.SetPosition(1, _startPoint + maxLength * _forward);
			if ((bool)cursorVisual)
			{
				cursorVisual.SetActive(value: false);
			}
		}
	}

	private void UpdateLaserBeam(Vector3 start, Vector3 end)
	{
		if (laserBeamBehavior == LaserBeamBehavior.Off)
		{
			return;
		}
		if (laserBeamBehavior == LaserBeamBehavior.On)
		{
			lineRenderer.SetPosition(0, start);
			lineRenderer.SetPosition(1, end);
		}
		else
		{
			if (laserBeamBehavior != LaserBeamBehavior.OnWhenHitTarget)
			{
				return;
			}
			if (_hitTarget)
			{
				if (!lineRenderer.enabled)
				{
					lineRenderer.enabled = true;
					lineRenderer.SetPosition(0, start);
					lineRenderer.SetPosition(1, end);
				}
			}
			else if (lineRenderer.enabled)
			{
				lineRenderer.enabled = false;
			}
		}
	}

	private void OnDisable()
	{
		if ((bool)cursorVisual)
		{
			cursorVisual.SetActive(value: false);
		}
	}

	public void OnInputFocusLost()
	{
		if ((bool)base.gameObject && base.gameObject.activeInHierarchy)
		{
			m_restoreOnInputAcquired = true;
			base.gameObject.SetActive(value: false);
		}
	}

	public void OnInputFocusAcquired()
	{
		if (m_restoreOnInputAcquired && (bool)base.gameObject)
		{
			m_restoreOnInputAcquired = false;
			base.gameObject.SetActive(value: true);
		}
	}

	private void OnDestroy()
	{
		OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
		OVRManager.InputFocusLost -= OnInputFocusLost;
	}
}
