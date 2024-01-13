using UnityEngine;

namespace OVRTouchSample;

public class HandPose : MonoBehaviour
{
	[SerializeField]
	private bool m_allowPointing;

	[SerializeField]
	private bool m_allowThumbsUp;

	[SerializeField]
	private HandPoseId m_poseId;

	public bool AllowPointing => m_allowPointing;

	public bool AllowThumbsUp => m_allowThumbsUp;

	public HandPoseId PoseId => m_poseId;
}
