using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x020002F6 RID: 758
	public class HandPose : MonoBehaviour
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x00074177 File Offset: 0x00072377
		public bool AllowPointing
		{
			get
			{
				return this.m_allowPointing;
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06001497 RID: 5271 RVA: 0x0007417F File Offset: 0x0007237F
		public bool AllowThumbsUp
		{
			get
			{
				return this.m_allowThumbsUp;
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06001498 RID: 5272 RVA: 0x00074187 File Offset: 0x00072387
		public HandPoseId PoseId
		{
			get
			{
				return this.m_poseId;
			}
		}

		// Token: 0x0400176E RID: 5998
		[SerializeField]
		private bool m_allowPointing;

		// Token: 0x0400176F RID: 5999
		[SerializeField]
		private bool m_allowThumbsUp;

		// Token: 0x04001770 RID: 6000
		[SerializeField]
		private HandPoseId m_poseId;
	}
}
