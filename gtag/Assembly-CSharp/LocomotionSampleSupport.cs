using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000A8 RID: 168
public class LocomotionSampleSupport : MonoBehaviour
{
	// Token: 0x17000033 RID: 51
	// (get) Token: 0x060003A0 RID: 928 RVA: 0x000164B8 File Offset: 0x000146B8
	private LocomotionTeleport TeleportController
	{
		get
		{
			return this.lc.GetComponent<LocomotionTeleport>();
		}
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x000164C8 File Offset: 0x000146C8
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

	// Token: 0x060003A2 RID: 930 RVA: 0x00016580 File Offset: 0x00014780
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

	// Token: 0x060003A3 RID: 931 RVA: 0x000165D8 File Offset: 0x000147D8
	[Conditional("DEBUG_LOCOMOTION_PANEL")]
	private static void Log(string msg)
	{
		Debug.Log(msg);
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x000165E0 File Offset: 0x000147E0
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

	// Token: 0x060003A5 RID: 933 RVA: 0x00016720 File Offset: 0x00014920
	protected void ActivateHandlers<TInput, TAim, TTarget, TOrientation, TTransition>() where TInput : TeleportInputHandler where TAim : TeleportAimHandler where TTarget : TeleportTargetHandler where TOrientation : TeleportOrientationHandler where TTransition : TeleportTransition
	{
		this.ActivateInput<TInput>();
		this.ActivateAim<TAim>();
		this.ActivateTarget<TTarget>();
		this.ActivateOrientation<TOrientation>();
		this.ActivateTransition<TTransition>();
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00016740 File Offset: 0x00014940
	protected void ActivateInput<TActivate>() where TActivate : TeleportInputHandler
	{
		this.ActivateCategory<TeleportInputHandler, TActivate>();
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x00016749 File Offset: 0x00014949
	protected void ActivateAim<TActivate>() where TActivate : TeleportAimHandler
	{
		this.ActivateCategory<TeleportAimHandler, TActivate>();
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x00016752 File Offset: 0x00014952
	protected void ActivateTarget<TActivate>() where TActivate : TeleportTargetHandler
	{
		this.ActivateCategory<TeleportTargetHandler, TActivate>();
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x0001675B File Offset: 0x0001495B
	protected void ActivateOrientation<TActivate>() where TActivate : TeleportOrientationHandler
	{
		this.ActivateCategory<TeleportOrientationHandler, TActivate>();
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00016764 File Offset: 0x00014964
	protected void ActivateTransition<TActivate>() where TActivate : TeleportTransition
	{
		this.ActivateCategory<TeleportTransition, TActivate>();
	}

	// Token: 0x060003AB RID: 939 RVA: 0x0001676D File Offset: 0x0001496D
	protected TActivate ActivateCategory<TCategory, TActivate>() where TCategory : MonoBehaviour where TActivate : MonoBehaviour
	{
		return LocomotionSampleSupport.ActivateCategory<TCategory, TActivate>(this.lc.gameObject);
	}

	// Token: 0x060003AC RID: 940 RVA: 0x0001677F File Offset: 0x0001497F
	protected void UpdateToggle(Toggle toggle, bool enabled)
	{
		if (enabled != toggle.isOn)
		{
			toggle.isOn = enabled;
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00016791 File Offset: 0x00014991
	private void SetupNonCap()
	{
		TeleportInputHandlerTouch component = this.TeleportController.GetComponent<TeleportInputHandlerTouch>();
		component.InputMode = TeleportInputHandlerTouch.InputModes.SeparateButtonsForAimAndTeleport;
		component.AimButton = OVRInput.RawButton.A;
		component.TeleportButton = OVRInput.RawButton.A;
	}

	// Token: 0x060003AE RID: 942 RVA: 0x000167B4 File Offset: 0x000149B4
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

	// Token: 0x060003AF RID: 943 RVA: 0x00016852 File Offset: 0x00014A52
	protected GameObject AddInstance(GameObject template, string label)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(template);
		gameObject.transform.SetParent(base.transform, false);
		gameObject.name = label;
		return gameObject;
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x00016874 File Offset: 0x00014A74
	private void SetupNodeTeleport()
	{
		this.SetupTeleportDefaults();
		this.SetupNonCap();
		this.lc.PlayerController.RotationEitherThumbstick = true;
		this.TeleportController.EnableRotation(true, false, false, true);
		this.ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerLaser, TeleportTargetHandlerNode, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
		this.TeleportController.GetComponent<TeleportInputHandlerTouch>().AimingController = OVRInput.Controller.RTouch;
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x000168C4 File Offset: 0x00014AC4
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

	// Token: 0x060003B2 RID: 946 RVA: 0x00016935 File Offset: 0x00014B35
	private void SetupWalkOnly()
	{
		this.SetupTeleportDefaults();
		this.TeleportController.enabled = false;
		this.lc.PlayerController.EnableLinearMovement = true;
		this.lc.PlayerController.RotationEitherThumbstick = false;
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0001696C File Offset: 0x00014B6C
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

	// Token: 0x04000449 RID: 1097
	private LocomotionController lc;

	// Token: 0x0400044A RID: 1098
	private bool inMenu;
}
