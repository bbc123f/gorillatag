using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x020002F8 RID: 760
	public class HandPose : MonoBehaviour
	{
		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600149D RID: 5277 RVA: 0x00074643 File Offset: 0x00072843
		public bool AllowPointing
		{
			get
			{
				return this.m_allowPointing;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x0600149E RID: 5278 RVA: 0x0007464B File Offset: 0x0007284B
		public bool AllowThumbsUp
		{
			get
			{
				return this.m_allowThumbsUp;
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x0600149F RID: 5279 RVA: 0x00074653 File Offset: 0x00072853
		public HandPoseId PoseId
		{
			get
			{
				return this.m_poseId;
			}
		}

		// Token: 0x0400177B RID: 6011
		[SerializeField]
		private bool m_allowPointing;

		// Token: 0x0400177C RID: 6012
		[SerializeField]
		private bool m_allowThumbsUp;

		// Token: 0x0400177D RID: 6013
		[SerializeField]
		private HandPoseId m_poseId;
	}
}
