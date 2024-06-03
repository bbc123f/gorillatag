using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocomotionSampleSupport : MonoBehaviour
{
	private LocomotionTeleport TeleportController
	{
		get
		{
			return this.lc.GetComponent<LocomotionTeleport>();
		}
	}

	public void Start()
	{
		this.lc = Object.FindObjectOfType<LocomotionController>();
		DebugUIBuilder.instance.AddButton("Node Teleport w/ A", new DebugUIBuilder.OnClick(this.SetupNodeTeleport), -1, 0, false);
		DebugUIBuilder.instance.AddButton("Dual-stick teleport", new DebugUIBuilder.OnClick(this.SetupTwoStickTeleport), -1, 0, false);
		DebugUIBuilder.instance.AddButton("L Strafe R Teleport", new DebugUIBuilder.OnClick(this.SetupLeftStrafeRightTeleport), -1, 0, false);
		DebugUIBuilder.instance.AddButton("Walk Only", new DebugUIBuilder.OnClick(this.SetupWalkOnly), -1, 0, false);
		if (Object.FindObjectOfType<EventSystem>() == null)
		{
			Debug.LogError("Need EventSystem");
		}
		this.SetupTwoStickTeleport();
		Physics.IgnoreLayerCollision(0, 4);
	}

	public void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			if (this.inMenu)
			{
				DebugUIBuilder.instance.Hide();
			}
			else
			{
				DebugUIBuilder.instance.Show();
			}
			this.inMenu = !this.inMenu;
		}
	}

	[Conditional("DEBUG_LOCOMOTION_PANEL")]
	private static void Log(string msg)
	{
		Debug.Log(msg);
	}

	public static TActivate ActivateCategory<TCategory, TActivate>(GameObject target) where TCategory : MonoBehaviour where TActivate : MonoBehaviour
	{
		TCategory[] components = target.GetComponents<TCategory>();
		string[] array = new string[7];
		array[0] = "Activate ";
		int num = 1;
		Type typeFromHandle = typeof(TActivate);
		array[num] = ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
		array[2] = " derived from ";
		int num2 = 3;
		Type typeFromHandle2 = typeof(TCategory);
		array[num2] = ((typeFromHandle2 != null) ? typeFromHandle2.ToString() : null);
		array[4] = "[";
		array[5] = components.Length.ToString();
		array[6] = "]";
		LocomotionSampleSupport.Log(string.Concat(array));
		TActivate result = default(TActivate);
		foreach (TCategory monoBehaviour in components)
		{
			bool flag = monoBehaviour.GetType() == typeof(TActivate);
			string[] array2 = new string[5];
			int num3 = 0;
			Type type = monoBehaviour.GetType();
			array2[num3] = ((type != null) ? type.ToString() : null);
			array2[1] = " is ";
			int num4 = 2;
			Type typeFromHandle3 = typeof(TActivate);
			array2[num4] = ((typeFromHandle3 != null) ? typeFromHandle3.ToString() : null);
			array2[3] = " = ";
			array2[4] = flag.ToString();
			LocomotionSampleSupport.Log(string.Concat(array2));
			if (flag)
			{
				result = (TActivate)((object)monoBehaviour);
			}
			if (monoBehaviour.enabled != flag)
			{
				monoBehaviour.enabled = flag;
			}
		}
		return result;
	}

	protected void ActivateHandlers<TInput, TAim, TTarget, TOrientation, TTransition>() where TInput : TeleportInputHandler where TAim : TeleportAimHandler where TTarget : TeleportTargetHandler where TOrientation : TeleportOrientationHandler where TTransition : TeleportTransition
	{
		this.ActivateInput<TInput>();
		this.ActivateAim<TAim>();
		this.ActivateTarget<TTarget>();
		this.ActivateOrientation<TOrientation>();
		this.ActivateTransition<TTransition>();
	}

	protected void ActivateInput<TActivate>() where TActivate : TeleportInputHandler
	{
		this.ActivateCategory<TeleportInputHandler, TActivate>();
	}

	protected void ActivateAim<TActivate>() where TActivate : TeleportAimHandler
	{
		this.ActivateCategory<TeleportAimHandler, TActivate>();
	}

	protected void ActivateTarget<TActivate>() where TActivate : TeleportTargetHandler
	{
		this.ActivateCategory<TeleportTargetHandler, TActivate>();
	}

	protected void ActivateOrientation<TActivate>() where TActivate : TeleportOrientationHandler
	{
		this.ActivateCategory<TeleportOrientationHandler, TActivate>();
	}

	protected void ActivateTransition<TActivate>() where TActivate : TeleportTransition
	{
		this.ActivateCategory<TeleportTransition, TActivate>();
	}

	protected TActivate ActivateCategory<TCategory, TActivate>() where TCategory : MonoBehaviour where TActivate : MonoBehaviour
	{
		return LocomotionSampleSupport.ActivateCategory<TCategory, TActivate>(this.lc.gameObject);
	}

	protected void UpdateToggle(Toggle toggle, bool enabled)
	{
		if (enabled != toggle.isOn)
		{
			toggle.isOn = enabled;
		}
	}

	private void SetupNonCap()
	{
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.SeparateButtonsForAimAndTeleport;
		component.AimButton = OVRInput.RawButton.A;
		component.TeleportButton = OVRInput.RawButton.A;
	}

	private void SetupTeleportDefaults()
	{
		this.TeleportController.enabled = true;
		this.lc.PlayerController.RotationEitherThumbstick = false;
		this.TeleportController.EnableMovement(false, false, false, false);
		this.TeleportController.EnableRotation(false, false, false, false);
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.CapacitiveButtonForAimAndTeleport;
		component.AimButton = OVRInput.RawButton.A;
		component.TeleportButton = OVRInput.RawButton.A;
		component.CapacitiveAimAndTeleportButton = TeleportInputHandlerTouch.AimCapTouchButtons.A;
		component.FastTeleport = false;
		TeleportInputHandlerHMD component2 = this.TeleportController.GetComponent<TeleportInputHandlerHMD>();
		component2.AimButton = OVRInput.RawButton.A;
		component2.TeleportButton = OVRInput.RawButton.A;
		this.TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>().Thumbstick = OVRInput.Controller.LTouch;
	}

	protected GameObject AddInstance(GameObject template, string label)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(template);
		gameObject.transform.SetParent(base.transform, false);
		gameObject.name = label;
		return gameObject;
	}

	private void SetupNodeTeleport()
	{
		this.SetupTeleportDefaults();
		this.SetupNonCap();
		this.lc.PlayerController.RotationEitherThumbstick = true;
		this.TeleportController.EnableRotation(true, false, false, true);
		this.ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerLaser, TeleportTargetHandlerNode, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
		this.TeleportController.GetComponent<TeleportInputHandlerTouch>().AimingController = OVRInput.Controller.RTouch;
	}

	private void SetupTwoStickTeleport()
	{
		this.SetupTeleportDefaults();
		this.TeleportController.EnableRotation(true, false, false, true);
		this.TeleportController.EnableMovement(false, false, false, false);
		this.lc.PlayerController.RotationEitherThumbstick = true;
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly;
		component.AimingController = OVRInput.Controller.Touch;
		this.ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerParabolic, TeleportTargetHandlerPhysical, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
		this.TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>().Thumbstick = OVRInput.Controller.Touch;
	}

	private void SetupWalkOnly()
	{
		this.SetupTeleportDefaults();
		this.TeleportController.enabled = false;
		this.lc.PlayerController.EnableLinearMovement = true;
		this.lc.PlayerController.RotationEitherThumbstick = false;
	}

	private void SetupLeftStrafeRightTeleport()
	{
		this.SetupTeleportDefaults();
		this.TeleportController.EnableRotation(true, false, false, true);
		this.TeleportController.EnableMovement(true, false, false, false);
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly;
		component.AimingController = OVRInput.Controller.RTouch;
		this.ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerParabolic, TeleportTargetHandlerPhysical, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
		this.TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>().Thumbstick = OVRInput.Controller.RTouch;
	}

	public LocomotionSampleSupport()
	{
	}

	private LocomotionController lc;

	private bool inMenu;
}
