using UnityEngine;

public class TeleportInputHandlerTouch : TeleportInputHandlerHMD
{
	public enum InputModes
	{
		CapacitiveButtonForAimAndTeleport,
		SeparateButtonsForAimAndTeleport,
		ThumbstickTeleport,
		ThumbstickTeleportForwardBackOnly
	}

	public enum AimCapTouchButtons
	{
		A,
		B,
		LeftTrigger,
		LeftThumbstick,
		RightTrigger,
		RightThumbstick,
		X,
		Y
	}

	public Transform LeftHand;

	public Transform RightHand;

	[Tooltip("CapacitiveButtonForAimAndTeleport=Activate aiming via cap touch detection, press the same button to teleport.\nSeparateButtonsForAimAndTeleport=Use one button to begin aiming, and another to trigger the teleport.\nThumbstickTeleport=Push a thumbstick to begin aiming, release to teleport.")]
	public InputModes InputMode;

	private readonly OVRInput.RawButton[] _rawButtons = new OVRInput.RawButton[8]
	{
		OVRInput.RawButton.A,
		OVRInput.RawButton.B,
		OVRInput.RawButton.LIndexTrigger,
		OVRInput.RawButton.LThumbstick,
		OVRInput.RawButton.RIndexTrigger,
		OVRInput.RawButton.RThumbstick,
		OVRInput.RawButton.X,
		OVRInput.RawButton.Y
	};

	private readonly OVRInput.RawTouch[] _rawTouch = new OVRInput.RawTouch[8]
	{
		OVRInput.RawTouch.A,
		OVRInput.RawTouch.B,
		OVRInput.RawTouch.LIndexTrigger,
		OVRInput.RawTouch.LThumbstick,
		OVRInput.RawTouch.RIndexTrigger,
		OVRInput.RawTouch.RThumbstick,
		OVRInput.RawTouch.X,
		OVRInput.RawTouch.Y
	};

	[Tooltip("Select the controller to be used for aiming. Supports LTouch, RTouch, or Touch for either.")]
	public OVRInput.Controller AimingController;

	private OVRInput.Controller InitiatingController;

	[Tooltip("Select the button to use for triggering aim and teleport when InputMode==CapacitiveButtonForAimAndTeleport")]
	public AimCapTouchButtons CapacitiveAimAndTeleportButton;

	[Tooltip("The thumbstick magnitude required to trigger aiming and teleports when InputMode==InputModes.ThumbstickTeleport")]
	public float ThumbstickTeleportThreshold = 0.5f;

	private void Start()
	{
	}

	public override LocomotionTeleport.TeleportIntentions GetIntention()
	{
		if (!base.isActiveAndEnabled)
		{
			return LocomotionTeleport.TeleportIntentions.None;
		}
		if (InputMode == InputModes.SeparateButtonsForAimAndTeleport)
		{
			return base.GetIntention();
		}
		if (InputMode == InputModes.ThumbstickTeleport || InputMode == InputModes.ThumbstickTeleportForwardBackOnly)
		{
			Vector2 lhs = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
			Vector2 lhs2 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			OVRInput.Controller controller = OVRInput.Controller.Touch;
			bool flag = OVRInput.Get(OVRInput.RawTouch.LThumbstick);
			bool flag2 = OVRInput.Get(OVRInput.RawTouch.RThumbstick);
			if (InputMode == InputModes.ThumbstickTeleportForwardBackOnly && base.LocomotionTeleport.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim)
			{
				num = Mathf.Abs(Vector2.Dot(lhs, Vector2.up));
				num2 = Mathf.Abs(Vector2.Dot(lhs2, Vector2.up));
			}
			else
			{
				num = lhs.magnitude;
				num2 = lhs2.magnitude;
			}
			if (AimingController == OVRInput.Controller.LTouch)
			{
				num3 = num;
				controller = OVRInput.Controller.LTouch;
			}
			else if (AimingController == OVRInput.Controller.RTouch)
			{
				num3 = num2;
				controller = OVRInput.Controller.RTouch;
			}
			else if (num > num2)
			{
				num3 = num;
				controller = OVRInput.Controller.LTouch;
			}
			else
			{
				num3 = num2;
				controller = OVRInput.Controller.RTouch;
			}
			if (!(num3 > ThumbstickTeleportThreshold) && (AimingController != OVRInput.Controller.Touch || !(flag || flag2)) && !(AimingController == OVRInput.Controller.LTouch && flag) && !(AimingController == OVRInput.Controller.RTouch && flag2))
			{
				if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
				{
					if (!FastTeleport)
					{
						return LocomotionTeleport.TeleportIntentions.PreTeleport;
					}
					return LocomotionTeleport.TeleportIntentions.Teleport;
				}
				if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
				{
					return LocomotionTeleport.TeleportIntentions.Teleport;
				}
			}
			else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
			{
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			if (num3 > ThumbstickTeleportThreshold)
			{
				InitiatingController = controller;
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			return LocomotionTeleport.TeleportIntentions.None;
		}
		OVRInput.RawButton rawMask = _rawButtons[(int)CapacitiveAimAndTeleportButton];
		if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && OVRInput.GetDown(rawMask))
		{
			if (!FastTeleport)
			{
				return LocomotionTeleport.TeleportIntentions.PreTeleport;
			}
			return LocomotionTeleport.TeleportIntentions.Teleport;
		}
		if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			if (FastTeleport || OVRInput.GetUp(rawMask))
			{
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			return LocomotionTeleport.TeleportIntentions.PreTeleport;
		}
		if (OVRInput.GetDown(_rawTouch[(int)CapacitiveAimAndTeleportButton]))
		{
			return LocomotionTeleport.TeleportIntentions.Aim;
		}
		if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && !OVRInput.GetUp(_rawTouch[(int)CapacitiveAimAndTeleportButton]))
		{
			return LocomotionTeleport.TeleportIntentions.Aim;
		}
		return LocomotionTeleport.TeleportIntentions.None;
	}

	public override void GetAimData(out Ray aimRay)
	{
		OVRInput.Controller controller = AimingController;
		if (controller == OVRInput.Controller.Touch)
		{
			controller = InitiatingController;
		}
		Transform transform = ((controller == OVRInput.Controller.LTouch) ? LeftHand : RightHand);
		aimRay = new Ray(transform.position, transform.forward);
	}
}
