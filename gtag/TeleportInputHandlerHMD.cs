using UnityEngine;

public class TeleportInputHandlerHMD : TeleportInputHandler
{
	[Tooltip("The button used to begin aiming for a teleport.")]
	public OVRInput.RawButton AimButton;

	[Tooltip("The button used to trigger the teleport after aiming. It can be the same button as the AimButton, however you cannot abort a teleport if it is.")]
	public OVRInput.RawButton TeleportButton;

	[Tooltip("When true, the system will not use the PreTeleport intention which will allow a teleport to occur on a button downpress. When false, the button downpress will trigger the PreTeleport intention and the Teleport intention when the button is released.")]
	public bool FastTeleport;

	public Transform Pointer { get; private set; }

	public override LocomotionTeleport.TeleportIntentions GetIntention()
	{
		if (!base.isActiveAndEnabled)
		{
			return LocomotionTeleport.TeleportIntentions.None;
		}
		if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && OVRInput.GetDown(TeleportButton))
		{
			if (!FastTeleport)
			{
				return LocomotionTeleport.TeleportIntentions.PreTeleport;
			}
			return LocomotionTeleport.TeleportIntentions.Teleport;
		}
		if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			if (OVRInput.GetUp(TeleportButton))
			{
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			return LocomotionTeleport.TeleportIntentions.PreTeleport;
		}
		if (OVRInput.Get(AimButton))
		{
			return LocomotionTeleport.TeleportIntentions.Aim;
		}
		if (AimButton == TeleportButton)
		{
			return LocomotionTeleport.TeleportIntentions.Teleport;
		}
		return LocomotionTeleport.TeleportIntentions.None;
	}

	public override void GetAimData(out Ray aimRay)
	{
		Transform centerEyeAnchor = base.LocomotionTeleport.LocomotionController.CameraRig.centerEyeAnchor;
		aimRay = new Ray(centerEyeAnchor.position, centerEyeAnchor.forward);
	}
}
