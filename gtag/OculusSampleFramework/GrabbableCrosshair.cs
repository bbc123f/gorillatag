using UnityEngine;

namespace OculusSampleFramework;

public class GrabbableCrosshair : MonoBehaviour
{
	public enum CrosshairState
	{
		Disabled,
		Enabled,
		Targeted
	}

	private CrosshairState m_state;

	private Transform m_centerEyeAnchor;

	[SerializeField]
	private GameObject m_targetedCrosshair;

	[SerializeField]
	private GameObject m_enabledCrosshair;

	private void Start()
	{
		m_centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
	}

	public void SetState(CrosshairState cs)
	{
		m_state = cs;
		switch (cs)
		{
		case CrosshairState.Disabled:
			m_targetedCrosshair.SetActive(value: false);
			m_enabledCrosshair.SetActive(value: false);
			break;
		case CrosshairState.Enabled:
			m_targetedCrosshair.SetActive(value: false);
			m_enabledCrosshair.SetActive(value: true);
			break;
		case CrosshairState.Targeted:
			m_targetedCrosshair.SetActive(value: true);
			m_enabledCrosshair.SetActive(value: false);
			break;
		}
	}

	private void Update()
	{
		if (m_state != 0)
		{
			base.transform.LookAt(m_centerEyeAnchor);
		}
	}
}
