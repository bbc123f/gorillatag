using System;
using TMPro;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class UiDeviceInspector : MonoBehaviour
{
	// Token: 0x06000496 RID: 1174 RVA: 0x0001D4F3 File Offset: 0x0001B6F3
	private void Start()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x0001D508 File Offset: 0x0001B708
	private void Update()
	{
		string sourceText = UiDeviceInspector.ToDeviceModel() + " [" + UiDeviceInspector.ToHandednessString(this.m_handedness) + "]";
		this.m_title.SetText(sourceText, true);
		string text = OVRInput.IsControllerConnected(this.m_controller) ? "<color=#66ff87>o</color>" : "<color=#ff8991>x</color>";
		string text2 = (OVRInput.GetControllerOrientationTracked(this.m_controller) && OVRInput.GetControllerPositionTracked(this.m_controller)) ? "<color=#66ff87>o</color>" : "<color=#ff8991>x</color>";
		this.m_status.SetText(string.Concat(new string[]
		{
			"Connected [",
			text,
			"] Tracked [",
			text2,
			"]"
		}), true);
		this.m_thumbRestTouch.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, this.m_controller));
		this.m_indexTrigger.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller));
		this.m_gripTrigger.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller));
		this.m_thumbRestForce.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryThumbRestForce, this.m_controller));
		this.m_stylusTipForce.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryStylusForce, this.m_controller));
		this.m_indexCurl1d.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTriggerCurl, this.m_controller));
		this.m_indexSlider1d.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTriggerSlide, this.m_controller));
		this.m_ax.SetValue(OVRInput.Get(OVRInput.Button.One, this.m_controller));
		this.m_axTouch.SetValue(OVRInput.Get(OVRInput.Touch.One, this.m_controller));
		this.m_by.SetValue(OVRInput.Get(OVRInput.Button.Two, this.m_controller));
		this.m_byTouch.SetValue(OVRInput.Get(OVRInput.Touch.Two, this.m_controller));
		this.m_indexTouch.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, this.m_controller));
		this.m_thumbstick.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, this.m_controller), OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller));
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0001D707 File Offset: 0x0001B907
	private static string ToDeviceModel()
	{
		return "Touch";
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0001D70E File Offset: 0x0001B90E
	private static string ToHandednessString(OVRInput.Handedness handedness)
	{
		if (handedness == OVRInput.Handedness.LeftHanded)
		{
			return "L";
		}
		if (handedness != OVRInput.Handedness.RightHanded)
		{
			return "-";
		}
		return "R";
	}

	// Token: 0x04000540 RID: 1344
	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	// Token: 0x04000541 RID: 1345
	[Header("Left Column Components")]
	[SerializeField]
	private TextMeshProUGUI m_title;

	// Token: 0x04000542 RID: 1346
	[SerializeField]
	private TextMeshProUGUI m_status;

	// Token: 0x04000543 RID: 1347
	[SerializeField]
	private UiBoolInspector m_thumbRestTouch;

	// Token: 0x04000544 RID: 1348
	[SerializeField]
	private UiAxis1dInspector m_thumbRestForce;

	// Token: 0x04000545 RID: 1349
	[SerializeField]
	private UiAxis1dInspector m_indexTrigger;

	// Token: 0x04000546 RID: 1350
	[SerializeField]
	private UiAxis1dInspector m_gripTrigger;

	// Token: 0x04000547 RID: 1351
	[SerializeField]
	private UiAxis1dInspector m_stylusTipForce;

	// Token: 0x04000548 RID: 1352
	[SerializeField]
	private UiAxis1dInspector m_indexCurl1d;

	// Token: 0x04000549 RID: 1353
	[SerializeField]
	private UiAxis1dInspector m_indexSlider1d;

	// Token: 0x0400054A RID: 1354
	[Header("Right Column Components")]
	[SerializeField]
	private UiBoolInspector m_ax;

	// Token: 0x0400054B RID: 1355
	[SerializeField]
	private UiBoolInspector m_axTouch;

	// Token: 0x0400054C RID: 1356
	[SerializeField]
	private UiBoolInspector m_by;

	// Token: 0x0400054D RID: 1357
	[SerializeField]
	private UiBoolInspector m_byTouch;

	// Token: 0x0400054E RID: 1358
	[SerializeField]
	private UiBoolInspector m_indexTouch;

	// Token: 0x0400054F RID: 1359
	[SerializeField]
	private UiAxis2dInspector m_thumbstick;

	// Token: 0x04000550 RID: 1360
	private OVRInput.Controller m_controller;
}
