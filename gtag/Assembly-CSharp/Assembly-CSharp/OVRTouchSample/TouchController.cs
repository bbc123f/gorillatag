using System;
using UnityEngine;

namespace OVRTouchSample
{
	public class TouchController : MonoBehaviour
	{
		private void Update()
		{
			this.m_animator.SetFloat("Button 1", OVRInput.Get(OVRInput.Button.One, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Button 2", OVRInput.Get(OVRInput.Button.Two, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Joy X", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller).x);
			this.m_animator.SetFloat("Joy Y", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller).y);
			this.m_animator.SetFloat("Grip", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller));
			this.m_animator.SetFloat("Trigger", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller));
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		private void OnInputFocusLost()
		{
			if (base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(false);
				this.m_restoreOnInputAcquired = true;
			}
		}

		private void OnInputFocusAcquired()
		{
			if (this.m_restoreOnInputAcquired)
			{
				base.gameObject.SetActive(true);
				this.m_restoreOnInputAcquired = false;
			}
		}

		[SerializeField]
		private OVRInput.Controller m_controller;

		[SerializeField]
		private Animator m_animator;

		private bool m_restoreOnInputAcquired;
	}
}
