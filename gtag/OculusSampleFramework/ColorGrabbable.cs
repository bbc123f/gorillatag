using System;
using UnityEngine;

namespace OculusSampleFramework;

public class ColorGrabbable : OVRGrabbable
{
	public static readonly Color COLOR_GRAB = new Color(1f, 0.5f, 0f, 1f);

	public static readonly Color COLOR_HIGHLIGHT = new Color(1f, 0f, 1f, 1f);

	private Color m_color = Color.black;

	private MeshRenderer[] m_meshRenderers;

	private bool m_highlight;

	public bool Highlight
	{
		get
		{
			return m_highlight;
		}
		set
		{
			m_highlight = value;
			UpdateColor();
		}
	}

	protected void UpdateColor()
	{
		if (base.isGrabbed)
		{
			SetColor(COLOR_GRAB);
		}
		else if (Highlight)
		{
			SetColor(COLOR_HIGHLIGHT);
		}
		else
		{
			SetColor(m_color);
		}
	}

	public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
	{
		base.GrabBegin(hand, grabPoint);
		UpdateColor();
	}

	public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
	{
		base.GrabEnd(linearVelocity, angularVelocity);
		UpdateColor();
	}

	private void Awake()
	{
		if (m_grabPoints.Length == 0)
		{
			Collider component = GetComponent<Collider>();
			if (component == null)
			{
				throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
			}
			m_grabPoints = new Collider[1] { component };
			m_meshRenderers = new MeshRenderer[1];
			m_meshRenderers[0] = GetComponent<MeshRenderer>();
		}
		else
		{
			m_meshRenderers = GetComponentsInChildren<MeshRenderer>();
		}
		m_color = new Color(UnityEngine.Random.Range(0.1f, 0.95f), UnityEngine.Random.Range(0.1f, 0.95f), UnityEngine.Random.Range(0.1f, 0.95f), 1f);
		SetColor(m_color);
	}

	private void SetColor(Color color)
	{
		for (int i = 0; i < m_meshRenderers.Length; i++)
		{
			MeshRenderer meshRenderer = m_meshRenderers[i];
			for (int j = 0; j < meshRenderer.materials.Length; j++)
			{
				meshRenderer.materials[j].color = color;
			}
		}
	}
}
