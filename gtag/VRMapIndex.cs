using System;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class VRMapIndex : VRMap
{
	public InputFeatureUsage inputAxis;

	public float triggerTouch;

	public float triggerValue;

	public Transform fingerBone1;

	public Transform fingerBone2;

	public Transform fingerBone3;

	public float closedAngles;

	public Vector3 closedAngle1;

	public Vector3 closedAngle2;

	public Vector3 closedAngle3;

	public Vector3 startingAngle1;

	public Vector3 startingAngle2;

	public Vector3 startingAngle3;

	private int lastAngle1;

	private int lastAngle2;

	private int lastAngle3;

	private InputDevice myInputDevice;

	public Quaternion[] angle1Table;

	public Quaternion[] angle2Table;

	public Quaternion[] angle3Table;

	private float currentAngle1;

	private float currentAngle2;

	private float currentAngle3;

	private int myTempInt;

	public override void MapMyFinger(float lerpValue)
	{
		calcT = 0f;
		triggerValue = ControllerInputPoller.TriggerFloat(vrTargetNode);
		triggerTouch = ControllerInputPoller.TriggerTouch(vrTargetNode);
		calcT = 0.1f * triggerTouch;
		calcT += 0.9f * triggerValue;
		LerpFinger(lerpValue, isOther: false);
	}

	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			currentAngle1 = Mathf.Lerp(currentAngle1, calcT, lerpValue);
			currentAngle2 = Mathf.Lerp(currentAngle2, calcT, lerpValue);
			currentAngle3 = Mathf.Lerp(currentAngle3, calcT, lerpValue);
			myTempInt = (int)(currentAngle1 * 10.1f);
			if (myTempInt != lastAngle1)
			{
				lastAngle1 = myTempInt;
				fingerBone1.localRotation = angle1Table[lastAngle1];
			}
			myTempInt = (int)(currentAngle2 * 10.1f);
			if (myTempInt != lastAngle2)
			{
				lastAngle2 = myTempInt;
				fingerBone2.localRotation = angle2Table[lastAngle2];
			}
			myTempInt = (int)(currentAngle3 * 10.1f);
			if (myTempInt != lastAngle3)
			{
				lastAngle3 = myTempInt;
				fingerBone3.localRotation = angle3Table[lastAngle3];
			}
		}
		else
		{
			fingerBone1.localRotation = Quaternion.Lerp(fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle1), Quaternion.Euler(closedAngle1), calcT), lerpValue);
			fingerBone2.localRotation = Quaternion.Lerp(fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle2), Quaternion.Euler(closedAngle2), calcT), lerpValue);
			fingerBone3.localRotation = Quaternion.Lerp(fingerBone3.localRotation, Quaternion.Lerp(Quaternion.Euler(startingAngle3), Quaternion.Euler(closedAngle3), calcT), lerpValue);
		}
	}
}
