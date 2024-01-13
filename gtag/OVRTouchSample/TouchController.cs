using UnityEngine;

namespace OVRTouchSample;

public class TouchController : MonoBehaviour
{
	[SerializeField]
	private OVRInput.Controller m_controller;

	[SerializeField]
	private Animator m_animator;

	private bool m_restoreOnInputAcquired;

	private void Update()
	{
		m_animator.SetFloat("Button 1", OVRInput.Get(OVRInput.Button.One, m_controller) ? 1f : 0f);
		m_animator.SetFloat("Button 2", OVRInput.Get(OVRInput.Button.Two, m_controller) ? 1f : 0f);
		m_animator.SetFloat("Joy X", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).x);
		m_animator.SetFloat("Joy Y", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controller).y);
		m_animator.SetFloat("Grip", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller));
		m_animator.SetFloat("Trigger", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller));
		OVRManager.InputFocusAcquired += OnInputFocusAcquired;
		OVRManager.InputFocusLost += OnInputFocusLost;
	}

	private void OnInputFocusLost()
	{
		if (base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(value: false);
			m_restoreOnInputAcquired = true;
		}
	}

	private void OnInputFocusAcquired()
	{
		if (m_restoreOnInputAcquired)
		{
			base.gameObject.SetActive(value: true);
			m_restoreOnInputAcquired = false;
		}
	}
}
