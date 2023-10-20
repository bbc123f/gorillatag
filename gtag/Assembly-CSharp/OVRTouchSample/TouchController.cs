using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x020002F9 RID: 761
	public class TouchController : MonoBehaviour
	{
		// Token: 0x060014A1 RID: 5281 RVA: 0x00074664 File Offset: 0x00072864
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

		// Token: 0x060014A2 RID: 5282 RVA: 0x00074761 File Offset: 0x00072961
		private void OnInputFocusLost()
		{
			if (base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(false);
				this.m_restoreOnInputAcquired = true;
			}
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x00074783 File Offset: 0x00072983
		private void OnInputFocusAcquired()
		{
			if (this.m_restoreOnInputAcquired)
			{
				base.gameObject.SetActive(true);
				this.m_restoreOnInputAcquired = false;
			}
		}

		// Token: 0x0400177E RID: 6014
		[SerializeField]
		private OVRInput.Controller m_controller;

		// Token: 0x0400177F RID: 6015
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04001780 RID: 6016
		private bool m_restoreOnInputAcquired;
	}
}
