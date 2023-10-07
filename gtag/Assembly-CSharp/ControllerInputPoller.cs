using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x02000154 RID: 340
public class ControllerInputPoller : MonoBehaviour
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600086A RID: 2154 RVA: 0x00034377 File Offset: 0x00032577
	// (set) Token: 0x0600086B RID: 2155 RVA: 0x0003437F File Offset: 0x0003257F
	public GorillaControllerType controllerType { get; private set; }

	// Token: 0x0600086C RID: 2156 RVA: 0x00034388 File Offset: 0x00032588
	private void Awake()
	{
		if (ControllerInputPoller.instance == null)
		{
			ControllerInputPoller.instance = this;
			return;
		}
		if (ControllerInputPoller.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x000343BC File Offset: 0x000325BC
	private void Update()
	{
		InputDevice inputDevice = this.leftControllerDevice;
		if (!this.leftControllerDevice.isValid)
		{
			this.leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			if (this.leftControllerDevice.isValid)
			{
				this.controllerType = GorillaControllerType.OCULUS_DEFAULT;
				if (this.leftControllerDevice.name.ToLower().Contains("knuckles"))
				{
					this.controllerType = GorillaControllerType.INDEX;
				}
				Debug.Log(string.Format("Found left controller: {0} ControllerType: {1}", this.leftControllerDevice.name, this.controllerType));
			}
		}
		InputDevice inputDevice2 = this.rightControllerDevice;
		if (!this.rightControllerDevice.isValid)
		{
			this.rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		InputDevice inputDevice3 = this.headDevice;
		if (!this.headDevice.isValid)
		{
			this.headDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		}
		InputDevice inputDevice4 = this.leftControllerDevice;
		InputDevice inputDevice5 = this.rightControllerDevice;
		InputDevice inputDevice6 = this.headDevice;
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.leftControllerPrimaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.leftControllerSecondaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.leftControllerPrimaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.leftControllerSecondaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.leftControllerGripFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.leftControllerIndexFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.leftControllerPosition);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.leftControllerRotation);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.rightControllerPrimaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.rightControllerSecondaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.rightControllerPrimaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.rightControllerSecondaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.rightControllerGripFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.rightControllerIndexFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.rightControllerPosition);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.rightControllerRotation);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.rightControllerPrimary2DAxis);
		this.leftControllerPrimaryButton = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButton = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_LeftPrimaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_LeftSecondaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerGripFloat = SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.leftControllerIndexFloat = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.rightControllerPrimaryButton = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButton = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_RightPrimaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_RightSecondaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerGripFloat = SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerIndexFloat = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
		this.headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.headPosition);
		this.headDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.headRotation);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, 0.75f, 0.65f);
			return;
		}
		if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, 0.1f, 0.01f);
		}
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x000347C6 File Offset: 0x000329C6
	private void CalculateGrabState(float grabValue, ref bool grab, ref bool grabRelease, float grabThreshold, float grabReleaseThreshold)
	{
		grab = (grabValue >= grabThreshold);
		grabRelease = (grabValue <= grabReleaseThreshold);
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x000347DC File Offset: 0x000329DC
	public static bool GetGrab(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrab;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrab;
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x00034801 File Offset: 0x00032A01
	public static bool GetGrabRelease(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabRelease;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabRelease;
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00034826 File Offset: 0x00032A26
	public static Vector2 Primary2DAxis(XRNode node)
	{
		return ControllerInputPoller.instance.rightControllerPrimary2DAxis;
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00034834 File Offset: 0x00032A34
	public static bool PrimaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButton;
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00034859 File Offset: 0x00032A59
	public static bool SecondaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButton;
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0003487E File Offset: 0x00032A7E
	public static bool PrimaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x000348A3 File Offset: 0x00032AA3
	public static bool SecondaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x000348C8 File Offset: 0x00032AC8
	public static float GripFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerGripFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerGripFloat;
		}
		return 0f;
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x000348F1 File Offset: 0x00032AF1
	public static float TriggerFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexFloat;
		}
		return 0f;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0003491A File Offset: 0x00032B1A
	public static float TriggerTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexTouch;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexTouch;
		}
		return 0f;
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x00034943 File Offset: 0x00032B43
	public static Vector3 DevicePosition(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headPosition;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPosition;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerPosition;
		}
		return Vector3.zero;
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0003497D File Offset: 0x00032B7D
	public static Quaternion DeviceRotation(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headRotation;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerRotation;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x000349B8 File Offset: 0x00032BB8
	public static bool PositionValid(XRNode node)
	{
		if (node == XRNode.Head)
		{
			InputDevice inputDevice = ControllerInputPoller.instance.headDevice;
			return ControllerInputPoller.instance.headDevice.isValid;
		}
		if (node == XRNode.LeftHand)
		{
			InputDevice inputDevice2 = ControllerInputPoller.instance.leftControllerDevice;
			return ControllerInputPoller.instance.leftControllerDevice.isValid;
		}
		if (node == XRNode.RightHand)
		{
			InputDevice inputDevice3 = ControllerInputPoller.instance.rightControllerDevice;
			return ControllerInputPoller.instance.rightControllerDevice.isValid;
		}
		return false;
	}

	// Token: 0x04000A84 RID: 2692
	public static volatile ControllerInputPoller instance;

	// Token: 0x04000A85 RID: 2693
	public float leftControllerIndexFloat;

	// Token: 0x04000A86 RID: 2694
	public float leftControllerGripFloat;

	// Token: 0x04000A87 RID: 2695
	public float rightControllerIndexFloat;

	// Token: 0x04000A88 RID: 2696
	public float rightControllerGripFloat;

	// Token: 0x04000A89 RID: 2697
	public float leftControllerIndexTouch;

	// Token: 0x04000A8A RID: 2698
	public float rightControllerIndexTouch;

	// Token: 0x04000A8B RID: 2699
	public float rightStickLRFloat;

	// Token: 0x04000A8C RID: 2700
	public Vector3 leftControllerPosition;

	// Token: 0x04000A8D RID: 2701
	public Vector3 rightControllerPosition;

	// Token: 0x04000A8E RID: 2702
	public Vector3 headPosition;

	// Token: 0x04000A8F RID: 2703
	public Quaternion leftControllerRotation;

	// Token: 0x04000A90 RID: 2704
	public Quaternion rightControllerRotation;

	// Token: 0x04000A91 RID: 2705
	public Quaternion headRotation;

	// Token: 0x04000A92 RID: 2706
	public InputDevice leftControllerDevice;

	// Token: 0x04000A93 RID: 2707
	public InputDevice rightControllerDevice;

	// Token: 0x04000A94 RID: 2708
	public InputDevice headDevice;

	// Token: 0x04000A95 RID: 2709
	public bool leftControllerPrimaryButton;

	// Token: 0x04000A96 RID: 2710
	public bool leftControllerSecondaryButton;

	// Token: 0x04000A97 RID: 2711
	public bool rightControllerPrimaryButton;

	// Token: 0x04000A98 RID: 2712
	public bool rightControllerSecondaryButton;

	// Token: 0x04000A99 RID: 2713
	public bool leftControllerPrimaryButtonTouch;

	// Token: 0x04000A9A RID: 2714
	public bool leftControllerSecondaryButtonTouch;

	// Token: 0x04000A9B RID: 2715
	public bool rightControllerPrimaryButtonTouch;

	// Token: 0x04000A9C RID: 2716
	public bool rightControllerSecondaryButtonTouch;

	// Token: 0x04000A9D RID: 2717
	public bool leftGrab;

	// Token: 0x04000A9E RID: 2718
	public bool leftGrabRelease;

	// Token: 0x04000A9F RID: 2719
	public bool rightGrab;

	// Token: 0x04000AA0 RID: 2720
	public bool rightGrabRelease;

	// Token: 0x04000AA2 RID: 2722
	public Vector2 rightControllerPrimary2DAxis;
}
