using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002C8 RID: 712
	public class GrabbableCrosshair : MonoBehaviour
	{
		// Token: 0x0600133B RID: 4923 RVA: 0x0006F3FF File Offset: 0x0006D5FF
		private void Start()
		{
			this.m_centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x0006F418 File Offset: 0x0006D618
		public void SetState(GrabbableCrosshair.CrosshairState cs)
		{
			this.m_state = cs;
			if (cs == GrabbableCrosshair.CrosshairState.Disabled)
			{
				this.m_targetedCrosshair.SetActive(false);
				this.m_enabledCrosshair.SetActive(false);
				return;
			}
			if (cs == GrabbableCrosshair.CrosshairState.Enabled)
			{
				this.m_targetedCrosshair.SetActive(false);
				this.m_enabledCrosshair.SetActive(true);
				return;
			}
			if (cs == GrabbableCrosshair.CrosshairState.Targeted)
			{
				this.m_targetedCrosshair.SetActive(true);
				this.m_enabledCrosshair.SetActive(false);
			}
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x0006F481 File Offset: 0x0006D681
		private void Update()
		{
			if (this.m_state != GrabbableCrosshair.CrosshairState.Disabled)
			{
				base.transform.LookAt(this.m_centerEyeAnchor);
			}
		}

		// Token: 0x04001618 RID: 5656
		private GrabbableCrosshair.CrosshairState m_state;

		// Token: 0x04001619 RID: 5657
		private Transform m_centerEyeAnchor;

		// Token: 0x0400161A RID: 5658
		[SerializeField]
		private GameObject m_targetedCrosshair;

		// Token: 0x0400161B RID: 5659
		[SerializeField]
		private GameObject m_enabledCrosshair;

		// Token: 0x020004E4 RID: 1252
		public enum CrosshairState
		{
			// Token: 0x0400205C RID: 8284
			Disabled,
			// Token: 0x0400205D RID: 8285
			Enabled,
			// Token: 0x0400205E RID: 8286
			Targeted
		}
	}
}
