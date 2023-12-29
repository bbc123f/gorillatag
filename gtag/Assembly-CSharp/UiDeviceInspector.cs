using System;
using TMPro;
using UnityEngine;

public class UiDeviceInspector : MonoBehaviour
{
	private void Start()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
	}

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

	private static string ToDeviceModel()
	{
		return "Touch";
	}

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

	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	[Header("Left Column Components")]
	[SerializeField]
	private TextMeshProUGUI m_title;

	[SerializeField]
	private TextMeshProUGUI m_status;

	[SerializeField]
	private UiBoolInspector m_thumbRestTouch;

	[SerializeField]
	private UiAxis1dInspector m_thumbRestForce;

	[SerializeField]
	private UiAxis1dInspector m_indexTrigger;

	[SerializeField]
	private UiAxis1dInspector m_gripTrigger;

	[SerializeField]
	private UiAxis1dInspector m_stylusTipForce;

	[SerializeField]
	private UiAxis1dInspector m_indexCurl1d;

	[SerializeField]
	private UiAxis1dInspector m_indexSlider1d;

	[Header("Right Column Components")]
	[SerializeField]
	private UiBoolInspector m_ax;

	[SerializeField]
	private UiBoolInspector m_axTouch;

	[SerializeField]
	private UiBoolInspector m_by;

	[SerializeField]
	private UiBoolInspector m_byTouch;

	[SerializeField]
	private UiBoolInspector m_indexTouch;

	[SerializeField]
	private UiAxis2dInspector m_thumbstick;

	private OVRInput.Controller m_controller;
}
