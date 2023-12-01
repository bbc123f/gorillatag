using System;
using UnityEngine;

namespace OVRTouchSample
{
	public class HandPose : MonoBehaviour
	{
		public bool AllowPointing
		{
			get
			{
				return this.m_allowPointing;
			}
		}

		public bool AllowThumbsUp
		{
			get
			{
				return this.m_allowThumbsUp;
			}
		}

		public HandPoseId PoseId
		{
			get
			{
				return this.m_poseId;
			}
		}

		[SerializeField]
		private bool m_allowPointing;

		[SerializeField]
		private bool m_allowThumbsUp;

		[SerializeField]
		private HandPoseId m_poseId;
	}
}
