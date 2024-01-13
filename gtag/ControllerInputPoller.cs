using UnityEngine;
using UnityEngine.XR;

public class ControllerInputPoller : MonoBehaviour
{
	public static volatile ControllerInputPoller instance;

	public float leftControllerIndexFloat;

	public float leftControllerGripFloat;

	public float rightControllerIndexFloat;

	public float rightControllerGripFloat;

	public float leftControllerIndexTouch;

	public float rightControllerIndexTouch;

	public float rightStickLRFloat;

	public Vector3 leftControllerPosition;

	public Vector3 rightControllerPosition;

	public Vector3 headPosition;

	public Quaternion leftControllerRotation;

	public Quaternion rightControllerRotation;

	public Quaternion headRotation;

	public InputDevice leftControllerDevice;

	public InputDevice rightControllerDevice;

	public InputDevice headDevice;

	public bool leftControllerPrimaryButton;

	public bool leftControllerSecondaryButton;

	public bool rightControllerPrimaryButton;

	public bool rightControllerSecondaryButton;

	public bool leftControllerPrimaryButtonTouch;

	public bool leftControllerSecondaryButtonTouch;

	public bool rightControllerPrimaryButtonTouch;

	public bool rightControllerSecondaryButtonTouch;

	public bool leftGrab;

	public bool leftGrabRelease;

	public bool rightGrab;

	public bool rightGrabRelease;

	public Vector2 rightControllerPrimary2DAxis;

	public GorillaControllerType controllerType { get; private set; }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		_ = leftControllerDevice;
		if (!leftControllerDevice.isValid)
		{
			leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			if (leftControllerDevice.isValid)
			{
				controllerType = GorillaControllerType.OCULUS_DEFAULT;
				if (leftControllerDevice.name.ToLower().Contains("knuckles"))
				{
					controllerType = GorillaControllerType.INDEX;
				}
				Debug.Log($"Found left controller: {leftControllerDevice.name} ControllerType: {controllerType}");
			}
		}
		_ = rightControllerDevice;
		if (!rightControllerDevice.isValid)
		{
			rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		_ = headDevice;
		if (!headDevice.isValid)
		{
			headDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		}
		_ = leftControllerDevice;
		_ = rightControllerDevice;
		_ = headDevice;
		leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out leftControllerPrimaryButton);
		leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out leftControllerSecondaryButton);
		leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out leftControllerPrimaryButtonTouch);
		leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out leftControllerSecondaryButtonTouch);
		leftControllerDevice.TryGetFeatureValue(CommonUsages.grip, out leftControllerGripFloat);
		leftControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out leftControllerIndexFloat);
		leftControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out leftControllerPosition);
		leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out leftControllerRotation);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out rightControllerPrimaryButton);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out rightControllerSecondaryButton);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out rightControllerPrimaryButtonTouch);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out rightControllerSecondaryButtonTouch);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.grip, out rightControllerGripFloat);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out rightControllerIndexFloat);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out rightControllerPosition);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rightControllerRotation);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightControllerPrimary2DAxis);
		leftControllerDevice.TryGetFeatureValue(CommonUsages.indexTouch, out leftControllerIndexTouch);
		rightControllerDevice.TryGetFeatureValue(CommonUsages.indexTouch, out rightControllerIndexTouch);
		headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out headPosition);
		headDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out headRotation);
		if (controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			CalculateGrabState(leftControllerGripFloat, ref leftGrab, ref leftGrabRelease, 0.75f, 0.65f);
			CalculateGrabState(rightControllerGripFloat, ref rightGrab, ref rightGrabRelease, 0.75f, 0.65f);
		}
		else if (controllerType == GorillaControllerType.INDEX)
		{
			CalculateGrabState(leftControllerGripFloat, ref leftGrab, ref leftGrabRelease, 0.1f, 0.01f);
			CalculateGrabState(rightControllerGripFloat, ref rightGrab, ref rightGrabRelease, 0.1f, 0.01f);
		}
	}

	private void CalculateGrabState(float grabValue, ref bool grab, ref bool grabRelease, float grabThreshold, float grabReleaseThreshold)
	{
		grab = grabValue >= grabThreshold;
		grabRelease = grabValue <= grabReleaseThreshold;
	}

	public static bool GetGrab(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftGrab, 
			XRNode.RightHand => instance.rightGrab, 
			_ => false, 
		};
	}

	public static bool GetGrabRelease(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftGrabRelease, 
			XRNode.RightHand => instance.rightGrabRelease, 
			_ => false, 
		};
	}

	public static Vector2 Primary2DAxis(XRNode node)
	{
		return instance.rightControllerPrimary2DAxis;
	}

	public static bool PrimaryButtonPress(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftControllerPrimaryButton, 
			XRNode.RightHand => instance.rightControllerPrimaryButton, 
			_ => false, 
		};
	}

	public static bool SecondaryButtonPress(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftControllerSecondaryButton, 
			XRNode.RightHand => instance.rightControllerSecondaryButton, 
			_ => false, 
		};
	}

	public static bool PrimaryButtonTouch(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftControllerPrimaryButtonTouch, 
			XRNode.RightHand => instance.rightControllerPrimaryButtonTouch, 
			_ => false, 
		};
	}

	public static bool SecondaryButtonTouch(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftControllerSecondaryButtonTouch, 
			XRNode.RightHand => instance.rightControllerSecondaryButtonTouch, 
			_ => false, 
		};
	}

	public static float GripFloat(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftControllerGripFloat, 
			XRNode.RightHand => instance.rightControllerGripFloat, 
			_ => 0f, 
		};
	}

	public static float TriggerFloat(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftControllerIndexFloat, 
			XRNode.RightHand => instance.rightControllerIndexFloat, 
			_ => 0f, 
		};
	}

	public static float TriggerTouch(XRNode node)
	{
		return node switch
		{
			XRNode.LeftHand => instance.leftControllerIndexTouch, 
			XRNode.RightHand => instance.rightControllerIndexTouch, 
			_ => 0f, 
		};
	}

	public static Vector3 DevicePosition(XRNode node)
	{
		return node switch
		{
			XRNode.Head => instance.headPosition, 
			XRNode.LeftHand => instance.leftControllerPosition, 
			XRNode.RightHand => instance.rightControllerPosition, 
			_ => Vector3.zero, 
		};
	}

	public static Quaternion DeviceRotation(XRNode node)
	{
		return node switch
		{
			XRNode.Head => instance.headRotation, 
			XRNode.LeftHand => instance.leftControllerRotation, 
			XRNode.RightHand => instance.rightControllerRotation, 
			_ => Quaternion.identity, 
		};
	}

	public static bool PositionValid(XRNode node)
	{
		switch (node)
		{
		case XRNode.Head:
			_ = instance.headDevice;
			return instance.headDevice.isValid;
		case XRNode.LeftHand:
			_ = instance.leftControllerDevice;
			return instance.leftControllerDevice.isValid;
		case XRNode.RightHand:
			_ = instance.rightControllerDevice;
			return instance.rightControllerDevice.isValid;
		default:
			return false;
		}
	}
}
