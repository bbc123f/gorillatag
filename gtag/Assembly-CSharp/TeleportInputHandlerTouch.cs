using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class TeleportInputHandlerTouch : TeleportInputHandlerHMD
{
	// Token: 0x06000305 RID: 773 RVA: 0x00012B4A File Offset: 0x00010D4A
	private void Start()
	{
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00012B4C File Offset: 0x00010D4C
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
			Vector2 lhs = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active);
			Vector2 lhs2 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active);
			bool flag = OVRInput.Get(OVRInput.RawTouch.LThumbstick, OVRInput.Controller.Active);
			bool flag2 = OVRInput.Get(OVRInput.RawTouch.RThumbstick, OVRInput.Controller.Active);
			float num;
			float num2;
			if (this.InputMode == TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly && base.LocomotionTeleport.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim)
			{
				num = Mathf.Abs(Vector2.Dot(lhs, Vector2.up));
				num2 = Mathf.Abs(Vector2.Dot(lhs2, Vector2.up));
			}
			else
			{
				num = lhs.magnitude;
				num2 = lhs2.magnitude;
			}
			float num3;
			OVRInput.Controller initiatingController;
			if (this.AimingController == OVRInput.Controller.LTouch)
			{
				num3 = num;
				initiatingController = OVRInput.Controller.LTouch;
			}
			else if (this.AimingController == OVRInput.Controller.RTouch)
			{
				num3 = num2;
				initiatingController = OVRInput.Controller.RTouch;
			}
			else if (num > num2)
			{
				num3 = num;
				initiatingController = OVRInput.Controller.LTouch;
			}
			else
			{
				num3 = num2;
				initiatingController = OVRInput.Controller.RTouch;
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
				this.InitiatingController = initiatingController;
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			return LocomotionTeleport.TeleportIntentions.None;
		}
		else
		{
			OVRInput.RawButton rawMask = this._rawButtons[(int)this.CapacitiveAimAndTeleportButton];
			if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && OVRInput.GetDown(rawMask, OVRInput.Controller.Active))
			{
				if (!this.FastTeleport)
				{
					return LocomotionTeleport.TeleportIntentions.PreTeleport;
				}
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
			{
				if (this.FastTeleport || OVRInput.GetUp(rawMask, OVRInput.Controller.Active))
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

	// Token: 0x06000307 RID: 775 RVA: 0x00012D84 File Offset: 0x00010F84
	public override void GetAimData(out Ray aimRay)
	{
		OVRInput.Controller controller = this.AimingController;
		if (controller == OVRInput.Controller.Touch)
		{
			controller = this.InitiatingController;
		}
		Transform transform = (controller == OVRInput.Controller.LTouch) ? this.LeftHand : this.RightHand;
		aimRay = new Ray(transform.position, transform.forward);
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00012DD0 File Offset: 0x00010FD0
	public TeleportInputHandlerTouch()
	{
		OVRInput.RawButton[] array = new OVRInput.RawButton[8];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.5F5673AE83EE13B46A7C1D9CE2F8CC01C37CFC893B0AC5E6E9260B79215F5ADC).FieldHandle);
		this._rawButtons = array;
		OVRInput.RawTouch[] array2 = new OVRInput.RawTouch[8];
		RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.1385A3395CDC9F7F90324CB0A06C5AC11AD4A425A35BBB7D5C9C0C33D8ADE9A0).FieldHandle);
		this._rawTouch = array2;
		this.ThumbstickTeleportThreshold = 0.5f;
		base..ctor();
	}

	// Token: 0x040003BD RID: 957
	public Transform LeftHand;

	// Token: 0x040003BE RID: 958
	public Transform RightHand;

	// Token: 0x040003BF RID: 959
	[Tooltip("CapacitiveButtonForAimAndTeleport=Activate aiming via cap touch detection, press the same button to teleport.\nSeparateButtonsForAimAndTeleport=Use one button to begin aiming, and another to trigger the teleport.\nThumbstickTeleport=Push a thumbstick to begin aiming, release to teleport.")]
	public TeleportInputHandlerTouch.InputModes InputMode;

	// Token: 0x040003C0 RID: 960
	private readonly OVRInput.RawButton[] _rawButtons;

	// Token: 0x040003C1 RID: 961
	private readonly OVRInput.RawTouch[] _rawTouch;

	// Token: 0x040003C2 RID: 962
	[Tooltip("Select the controller to be used for aiming. Supports LTouch, RTouch, or Touch for either.")]
	public OVRInput.Controller AimingController;

	// Token: 0x040003C3 RID: 963
	private OVRInput.Controller InitiatingController;

	// Token: 0x040003C4 RID: 964
	[Tooltip("Select the button to use for triggering aim and teleport when InputMode==CapacitiveButtonForAimAndTeleport")]
	public TeleportInputHandlerTouch.AimCapTouchButtons CapacitiveAimAndTeleportButton;

	// Token: 0x040003C5 RID: 965
	[Tooltip("The thumbstick magnitude required to trigger aiming and teleports when InputMode==InputModes.ThumbstickTeleport")]
	public float ThumbstickTeleportThreshold;

	// Token: 0x020003BB RID: 955
	public enum InputModes
	{
		// Token: 0x04001BA4 RID: 7076
		CapacitiveButtonForAimAndTeleport,
		// Token: 0x04001BA5 RID: 7077
		SeparateButtonsForAimAndTeleport,
		// Token: 0x04001BA6 RID: 7078
		ThumbstickTeleport,
		// Token: 0x04001BA7 RID: 7079
		ThumbstickTeleportForwardBackOnly
	}

	// Token: 0x020003BC RID: 956
	public enum AimCapTouchButtons
	{
		// Token: 0x04001BA9 RID: 7081
		A,
		// Token: 0x04001BAA RID: 7082
		B,
		// Token: 0x04001BAB RID: 7083
		LeftTrigger,
		// Token: 0x04001BAC RID: 7084
		LeftThumbstick,
		// Token: 0x04001BAD RID: 7085
		RightTrigger,
		// Token: 0x04001BAE RID: 7086
		RightThumbstick,
		// Token: 0x04001BAF RID: 7087
		X,
		// Token: 0x04001BB0 RID: 7088
		Y
	}
}
