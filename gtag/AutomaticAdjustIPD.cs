using UnityEngine;
using UnityEngine.XR;

public class AutomaticAdjustIPD : MonoBehaviour
{
	public InputDevice headset;

	public float currentIPD;

	public Vector3 leftEyePosition;

	public Vector3 rightEyePosition;

	public bool testOverride;

	public Transform[] adjustXScaleObjects;

	public float sizeAt58mm = 1f;

	public float sizeAt63mm = 1.12f;

	public float lastIPD;

	private void Update()
	{
		_ = headset;
		if (!headset.isValid)
		{
			headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		}
		if (!headset.isValid || !headset.TryGetFeatureValue(CommonUsages.leftEyePosition, out leftEyePosition) || !headset.TryGetFeatureValue(CommonUsages.rightEyePosition, out rightEyePosition))
		{
			return;
		}
		currentIPD = (rightEyePosition - leftEyePosition).magnitude;
		if (!(Mathf.Abs(lastIPD - currentIPD) < 0.01f))
		{
			lastIPD = currentIPD;
			Transform[] array = adjustXScaleObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].localScale = new Vector3(Mathf.LerpUnclamped(1f, 1.12f, (currentIPD - 0.058f) / 0.0050000027f), 1f, 1f);
			}
		}
	}
}
