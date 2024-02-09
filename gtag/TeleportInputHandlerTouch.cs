using System;
using UnityEngine;

public class TeleportInputHandlerTouch : TeleportInputHandlerHMD
{
	private void Start()
	{
	}

	public override LocomotionTeleport.TeleportIntentions GetIntention()
	{
		if (!base.isActiveAndEnabled)
		{
			return LocomotionTeleport.TeleportIntentions.None;
		}
		if (this.InputMode == TeleportInputHandlerTouch.InputModes.SeparateButtonsForAimAndTeleport)
		{
			return base.GetIntention();
		}
		if (this.InputMode == TeleportInputHandlerTouch.InputModes.ThumbstickTeleport || this.InputMode == TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly)
		{
			Vector2 vector = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active);
			Vector2 vector2 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active);
			bool flag = OVRInput.Get(OVRInput.RawTouch.LThumbstick, OVRInput.Controller.Active);
			bool flag2 = OVRInput.Get(OVRInput.RawTouch.RThumbstick, OVRInput.Controller.Active);
			float num;
			float num2;
			if (this.InputMode == TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly && base.LocomotionTeleport.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim)
			{
				num = Mathf.Abs(Vector2.Dot(vector, Vector2.up));
				num2 = Mathf.Abs(Vector2.Dot(vector2, Vector2.up));
			}
			else
			{
				num = vector.magnitude;
				num2 = vector2.magnitude;
			}
			float num3;
			OVRInput.Controller controller;
			if (this.AimingController == OVRInput.Controller.LTouch)
			{
				num3 = num;
				controller = OVRInput.Controller.LTouch;
			}
			else if (this.AimingController == OVRInput.Controller.RTouch)
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
			if (num3 <= this.ThumbstickTeleportThreshold && (this.AimingController != OVRInput.Controller.Touch || (!flag && !flag2)) && (this.AimingController != OVRInput.Controller.LTouch || !flag) && (this.AimingController != OVRInput.Controller.RTouch || !flag2))
			{
				if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
				{
					if (!this.FastTeleport)
					{
						return LocomotionTeleport.TeleportIntentions.PreTeleport;
					}
					return LocomotionTeleport.TeleportIntentions.Teleport;
				}
				else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
				{
					return LocomotionTeleport.TeleportIntentions.Teleport;
				}
			}
			else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
			{
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			if (num3 > this.ThumbstickTeleportThreshold)
			{
				this.InitiatingController = controller;
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			return LocomotionTeleport.TeleportIntentions.None;
		}
		else
		{
			OVRInput.RawButton rawButton = this._rawButtons[(int)this.CapacitiveAimAndTeleportButton];
			if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && OVRInput.GetDown(rawButton, OVRInput.Controller.Active))
			{
				if (!this.FastTeleport)
				{
					return LocomotionTeleport.TeleportIntentions.PreTeleport;
				}
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
			{
				if (this.FastTeleport || OVRInput.GetUp(rawButton, OVRInput.Controller.Active))
				{
					return LocomotionTeleport.TeleportIntentions.Teleport;
				}
				return LocomotionTeleport.TeleportIntentions.PreTeleport;
			}
			else
			{
				if (OVRInput.GetDown(this._rawTouch[(int)this.CapacitiveAimAndTeleportButton], OVRInput.Controller.Active))
				{
					return LocomotionTeleport.TeleportIntentions.Aim;
				}
				if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && !OVRInput.GetUp(this._rawTouch[(int)this.CapacitiveAimAndTeleportButton], OVRInput.Controller.Active))
				{
					return LocomotionTeleport.TeleportIntentions.Aim;
				}
				return LocomotionTeleport.TeleportIntentions.None;
			}
		}
	}

	public override void GetAimData(out Ray aimRay)
	{
		OVRInput.Controller controller = this.AimingController;
		if (controller == OVRInput.Controller.Touch)
		{
			controller = this.InitiatingController;
		}
		Transform transform = ((controller == OVRInput.Controller.LTouch) ? this.LeftHand : this.RightHand);
		aimRay = new Ray(transform.position, transform.forward);
	}

	public Transform LeftHand;

	public Transform RightHand;

	[Tooltip("CapacitiveButtonForAimAndTeleport=Activate aiming via cap touch detection, press the same button to teleport.\nSeparateButtonsForAimAndTeleport=Use one button to begin aiming, and another to trigger the teleport.\nThumbstickTeleport=Push a thumbstick to begin aiming, release to teleport.")]
	public TeleportInputHandlerTouch.InputModes InputMode;

	private readonly OVRInput.RawButton[] _rawButtons = new OVRInput.RawButton[]
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

	private readonly OVRInput.RawTouch[] _rawTouch = new OVRInput.RawTouch[]
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
	public TeleportInputHandlerTouch.AimCapTouchButtons CapacitiveAimAndTeleportButton;

	[Tooltip("The thumbstick magnitude required to trigger aiming and teleports when InputMode==InputModes.ThumbstickTeleport")]
	public float ThumbstickTeleportThreshold = 0.5f;

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
}
