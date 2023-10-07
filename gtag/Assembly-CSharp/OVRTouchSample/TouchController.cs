using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x020002F7 RID: 759
	public class TouchController : MonoBehaviour
	{
		// Token: 0x0600149A RID: 5274 RVA: 0x00074198 File Offset: 0x00072398
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

		// Token: 0x0600149B RID: 5275 RVA: 0x00074295 File Offset: 0x00072495
		private void OnInputFocusLost()
		{
			if (base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(false);
				this.m_restoreOnInputAcquired = true;
			}
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x000742B7 File Offset: 0x000724B7
		private void OnInputFocusAcquired()
		{
			if (this.m_restoreOnInputAcquired)
			{
				base.gameObject.SetActive(true);
				this.m_restoreOnInputAcquired = false;
			}
		}

		// Token: 0x04001771 RID: 6001
		[SerializeField]
		private OVRInput.Controller m_controller;

		// Token: 0x04001772 RID: 6002
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04001773 RID: 6003
		private bool m_restoreOnInputAcquired;
	}
}
