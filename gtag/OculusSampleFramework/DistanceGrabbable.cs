using UnityEngine;

namespace OculusSampleFramework;

public class DistanceGrabbable : OVRGrabbable
{
	public string m_materialColorField;

	private GrabbableCrosshair m_crosshair;

	private GrabManager m_crosshairManager;

	private Renderer m_renderer;

	private MaterialPropertyBlock m_mpb;

	private bool m_inRange;

	private bool m_targeted;

	public bool InRange
	{
		get
		{
			return m_inRange;
		}
		set
		{
			m_inRange = value;
			RefreshCrosshair();
		}
	}

	public bool Targeted
	{
		get
		{
			return m_targeted;
		}
		set
		{
			m_targeted = value;
			RefreshCrosshair();
		}
	}

	protected override void Start()
	{
		base.Start();
		m_crosshair = base.gameObject.GetComponentInChildren<GrabbableCrosshair>();
		m_renderer = base.gameObject.GetComponent<Renderer>();
		m_crosshairManager = Object.FindObjectOfType<GrabManager>();
		m_mpb = new MaterialPropertyBlock();
		RefreshCrosshair();
		m_renderer.SetPropertyBlock(m_mpb);
	}

	private void RefreshCrosshair()
	{
		if ((bool)m_crosshair)
		{
			if (base.isGrabbed)
			{
				m_crosshair.SetState(GrabbableCrosshair.CrosshairState.Disabled);
			}
			else if (!InRange)
			{
				m_crosshair.SetState(GrabbableCrosshair.CrosshairState.Disabled);
			}
			else
			{
				m_crosshair.SetState((!Targeted) ? GrabbableCrosshair.CrosshairState.Enabled : GrabbableCrosshair.CrosshairState.Targeted);
			}
		}
		if (m_materialColorField != null)
		{
			m_renderer.GetPropertyBlock(m_mpb);
			if (base.isGrabbed || !InRange)
			{
				m_mpb.SetColor(m_materialColorField, m_crosshairManager.OutlineColorOutOfRange);
			}
			else if (Targeted)
			{
				m_mpb.SetColor(m_materialColorField, m_crosshairManager.OutlineColorHighlighted);
			}
			else
			{
				m_mpb.SetColor(m_materialColorField, m_crosshairManager.OutlineColorInRange);
			}
			m_renderer.SetPropertyBlock(m_mpb);
		}
	}
}
