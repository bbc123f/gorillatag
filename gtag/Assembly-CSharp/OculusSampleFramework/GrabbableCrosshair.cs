using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002CA RID: 714
	public class GrabbableCrosshair : MonoBehaviour
	{
		// Token: 0x06001342 RID: 4930 RVA: 0x0006F8CB File Offset: 0x0006DACB
		private void Start()
		{
			this.m_centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x0006F8E4 File Offset: 0x0006DAE4
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

		// Token: 0x06001344 RID: 4932 RVA: 0x0006F94D File Offset: 0x0006DB4D
		private void Update()
		{
			if (this.m_state != GrabbableCrosshair.CrosshairState.Disabled)
			{
				base.transform.LookAt(this.m_centerEyeAnchor);
			}
		}

		// Token: 0x04001625 RID: 5669
		private GrabbableCrosshair.CrosshairState m_state;

		// Token: 0x04001626 RID: 5670
		private Transform m_centerEyeAnchor;

		// Token: 0x04001627 RID: 5671
		[SerializeField]
		private GameObject m_targetedCrosshair;

		// Token: 0x04001628 RID: 5672
		[SerializeField]
		private GameObject m_enabledCrosshair;

		// Token: 0x020004E6 RID: 1254
		public enum CrosshairState
		{
			// Token: 0x04002069 RID: 8297
			Disabled,
			// Token: 0x0400206A RID: 8298
			Enabled,
			// Token: 0x0400206B RID: 8299
			Targeted
		}
	}
}
