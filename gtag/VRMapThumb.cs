using System;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class VRMapThumb : VRMap
{
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		if (this.vrTargetNode == XRNode.LeftHand)
		{
			this.primaryButtonPress = ControllerInputPoller.instance.leftControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.leftControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		else
		{
			this.primaryButtonPress = ControllerInputPoller.instance.rightControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.rightControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
		}
		if (this.primaryButtonPress || this.secondaryButtonPress)
		{
			this.calcT = 1f;
		}
		else if (this.primaryButtonTouch || this.secondaryButtonTouch)
		{
			this.calcT = 0.1f;
		}
		this.LerpFinger(lerpValue, false);
	}

	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.myTempInt = (int)(this.currentAngle1 * 10.1f);
			if (this.myTempInt != this.lastAngle1)
			{
				this.lastAngle1 = this.myTempInt;
				this.fingerBone1.localRotation = this.angle1Table[this.lastAngle1];
			}
			this.myTempInt = (int)(this.currentAngle2 * 10.1f);
			if (this.myTempInt != this.lastAngle2)
			{
				this.lastAngle2 = this.myTempInt;
				this.fingerBone2.localRotation = this.angle2Table[this.lastAngle2];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle1), Quaternion.Euler(this.closedAngle1), this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle2), Quaternion.Euler(this.closedAngle2), this.calcT), lerpValue);
		}
	}

	public InputFeatureUsage inputAxis;

	public bool primaryButtonTouch;

	public bool primaryButtonPress;

	public bool secondaryButtonTouch;

	public bool secondaryButtonPress;

	public Transform fingerBone1;

	public Transform fingerBone2;

	public Vector3 closedAngle1;

	public Vector3 closedAngle2;

	public Vector3 startingAngle1;

	public Vector3 startingAngle2;

	public Quaternion[] angle1Table;

	public Quaternion[] angle2Table;

	private float currentAngle1;

	private float currentAngle2;

	private int lastAngle1;

	private int lastAngle2;

	private InputDevice tempDevice;

	private int myTempInt;
}
