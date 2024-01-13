using UnityEngine;

public class ChestObjectHysteresis : MonoBehaviour
{
	public float angleHysteresis;

	public float angleBetween;

	public Transform angleFollower;

	private Quaternion lastAngleQuat;

	private Quaternion currentAngleQuat;

	private void Start()
	{
		lastAngleQuat = base.transform.rotation;
		currentAngleQuat = base.transform.rotation;
	}

	private void LateUpdate()
	{
		currentAngleQuat = angleFollower.rotation;
		angleBetween = Quaternion.Angle(currentAngleQuat, lastAngleQuat);
		if (angleBetween > angleHysteresis)
		{
			base.transform.rotation = Quaternion.Slerp(currentAngleQuat, lastAngleQuat, angleHysteresis / angleBetween);
			lastAngleQuat = base.transform.rotation;
		}
		base.transform.rotation = lastAngleQuat;
	}
}
